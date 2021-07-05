﻿using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Creeper.SqlBuilder.ExpressionAnalysis
{
	internal class ConditionBuilder : ExpressionVisitor
	{
		/// <summary>
		/// string条件
		/// </summary>
		public string Condition { get; private set; }

		/// <summary>
		/// 参数
		/// </summary>
		public object[] Arguments { get; private set; }

		/// <summary>
		/// 数据库别名
		/// </summary>
		public string[] Alias { get; private set; }

		/// <summary>
		/// 字段是否加引号
		/// </summary>
		private readonly List<object> _arguments = new List<object>();
		private readonly HashSet<string> _alias = new HashSet<string>();
		private readonly Stack<string> _conditionParts = new Stack<string>();
		private readonly ICreeperDbConverter _converter;

		public ConditionBuilder(ICreeperDbConverter converter)
		{
			_converter = converter;
		}

		public void Build(Expression expression)
		{
			if (expression is null)
			{
				throw new ArgumentNullException(nameof(expression));
			}

			var evaluator = new PartialEvaluator();
			var evaluatedExpression = evaluator.Eval(expression);

			Visit(evaluatedExpression);

			Arguments = _arguments.ToArray();
			Condition = _conditionParts.Count > 0 ? _conditionParts.Pop() : null;
			Alias = new string[_alias.Count];
			_alias.CopyTo(Alias);
		}

		protected override Expression VisitNew(NewExpression node)
		{
			for (int i = 0; i < node.Arguments.Count; i++)
			{
				Visit(node.Arguments[i]);
				if (i != node.Arguments.Count - 1)
					_conditionParts.Push(", ");
			}
			MergeConditionParts();
			return node;
		}

		protected override Expression VisitUnary(UnaryExpression node)
		{
			if (node == null) return node;
			// !xxx.Equal("") 非表达式语句
			if (node.NodeType == ExpressionType.Not) _conditionParts.Push("NOT");
			if (node.NodeType == ExpressionType.ArrayLength)
			{
				_conditionParts.Push("array_length(");
				Visit(node.Operand);
				_conditionParts.Push(",1)");
				MergeConditionParts("{0}{1}{2}");
				return node;
			}
			//这里只是添加NOT标记, 直接请求父类的访问方法继续递归
			return base.VisitUnary(node);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node == null) return node;
			var isVisit = false;
			//表达式是否包含转换类型的表达式, 一般枚举类型运算需要用到
			if (node.Left.NodeType == ExpressionType.Convert) isVisit = VisitConvert((UnaryExpression)node.Left, node.Right, true);

			else if (node.Right.NodeType == ExpressionType.Convert) isVisit = VisitConvert((UnaryExpression)node.Right, node.Left, false);

			else if (node.NodeType == ExpressionType.ArrayIndex && node.Left.NodeType == ExpressionType.MemberAccess) //array[1]表达式包含数组索引
			{
				Visit(node.Left);

				//数据库索引从1开始
				_conditionParts.Push(string.Concat("[", (int)node.Right.GetExpressionValue() + 1, "]"));
				MergeConditionParts();
				return node;
			}
			if (!isVisit) //如果没有被解析, 那么使用通用解析方法
			{
				Visit(node.Left);
				Visit(node.Right);
			}
			var right = _conditionParts.Pop();
			var left = _conditionParts.Pop();

			string cond = node.GetCondition(left, right);

			if (cond != null) _conditionParts.Push(cond);
			return node;
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			if (node == null) return node;

			if (node.Value == null)
				_conditionParts.Push(null);

			else
			{
				_arguments.Add(node.Value);
				string cond = null;
				if (node.IsImplementation<IList>()) //如果是list/array类型
				{
					if (node.Type.GetElementType() == typeof(string)) //如果是字符串数据, 部分数据库需要强制转换
						cond = string.Format("CAST({{{0}}} AS {1}[])", _arguments.Count - 1, _converter.CastStringDbType);
				}

				if (cond == null) cond = string.Format("{{{0}}}", _arguments.Count - 1);

				if (cond != null) _conditionParts.Push(cond);
			}
			return node;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node == null) return node;

			var propertyInfo = node.Member as PropertyInfo;
			if (propertyInfo == null) return node;

			if (node.Expression.NodeType == ExpressionType.Parameter)
			{
				var p = node.Expression.ToString();

				// 返回当前表达式里面所有的参数
				if (!_alias.Contains(p)) _alias.Add(p);
			}

			// 返回数据库成员字段
			_conditionParts.Push(string.Format("{0}", node.GetOriginExpression().ToDatebaseField(_converter)));

			return node;
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			if (node == null) return node;

			string connector = _converter.StringConnectWord; //获取字符串连接符
			bool useDefault = true; //是否使用默认表达式访问方式

			string format = null;
			var not = _conditionParts.TryPop(out var result) ? result : string.Empty; //非运算符
			switch (node.Method.Name)
			{
				case "StartsWith": //Like 'xxx%',
					format = string.Concat("{0} ", not, " ", IgnoreCaseConvert(node.Arguments), " ''", connector, "{1}", connector, "'%'");
					break;

				case "Contains": //Like '%xxx%',
					if (node.Object?.Type == typeof(string)) //如果是String.Contains, 那么使用like
						format = string.Concat("{0} ", not, " ", IgnoreCaseConvert(node.Arguments), " '%'", connector, "{1}", connector, "'%'");

					//其他情况使用 IEnumerable.Contains
					else
					{
						useDefault = false;
						var opr = not == "NOT" ? "<>" : "=";
						format = string.Concat("{0} ", opr, " {1}");
						var method = not == "NOT" ? "ALL" : "ANY";

						for (int i = 0; i < node.Arguments.Count; i++) //遍历Visit成员表达式
						{
							Expression arg = node.Arguments[i];
							//如果当前表达式是IEnumerable类型, 且不是
							if (arg.IsImplementation<IEnumerable>() && arg.Type != typeof(string))
							{
								format = format.Replace($"{{{i}}}", $"{method}({{{i}}})");
								arg = arg.ToArrayExpression();
							}
							Visit(arg);
						}
						if (!format.StartsWith("{0}")) //ALL/ANY只能在操作符后面, 这里添加前后对调方法, 用PostgreSql为例
						{
							var conds = format.Split($" {opr} ").Reverse();
							format = string.Join($" {opr} ", conds);
						}
					}
					break;

				case "EndsWith": //Like '%xxx',
					format = string.Concat("{0} ", not, " ", IgnoreCaseConvert(node.Arguments), " '%'", connector, "{1}", connector, "''");
					break;

				case "Equals":
					format = string.Concat("({0} ", not == "NOT" ? "<>" : "=", " {1})");
					break;

				case "ToString": //a.Name.ToString()=>(CAST a.Name AS VARCHAR)
					if (node.Object.NodeType != ExpressionType.MemberAccess) goto default;
					useDefault = false;
					_conditionParts.Push("CAST(");
					VisitMember((MemberExpression)node.Object);
					_conditionParts.Push(string.Format(" AS {0})", _converter.CastStringDbType));
					MergeConditionParts();
					break;

				default: //如果没有特殊的方法解析, 直接返回方法的返回值, 用常量表达式Visit
					useDefault = false;
					var constantExpression = node.GetConstantFromExression(node.Type);
					VisitConstant(constantExpression);
					break;
			}

			if (useDefault)
			{
				Visit(node.Object);
				Visit(node.Arguments[0]);
			}

			if (format != null) MergeConditionParts(format);

			return node;
		}

		/// <summary>
		/// 忽略大小写用ILIKE, 否则用LIKE
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		private string IgnoreCaseConvert(ReadOnlyCollection<Expression> arguments)
		{
			var ignoreCase = VisitStringContainCulture(arguments);
			return ignoreCase ? "ILIKE" : "LIKE";
		}

		/// <summary>
		/// 检查模糊查询是否忽略大小写
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		private bool VisitStringContainCulture(ReadOnlyCollection<Expression> arguments)
		{
			if (arguments.Count != 2) return false;
			if (arguments[1].ToString().EndsWith("IgnoreCase")) return true;
			return false;
		}

		/// <summary>
		/// 访问转换类型的表达式, 常用于枚举类型转换
		/// </summary>
		/// <param name="unaryExpression">数据库成员一元表达式</param>
		/// <param name="expression">包含变量输出的表达式</param>
		/// <param name="unaryFirst">判断表达式左右顺序</param>
		/// <returns>是否完成解析标志</returns>
		private bool VisitConvert(UnaryExpression unaryExpression, Expression expression, bool unaryFirst)
		{
			//获取成员表达式类型, 如果是枚举类型就继续, 否则结束
			var unaryExpressionOperandType = unaryExpression.Operand.Type.GetOriginalType();
			if (!unaryExpressionOperandType.IsEnum) return false;

			//因为枚举在lambda表达式会转化为int类型, 则不是int类型就结束
			var unaryExpressionType = unaryExpression.Type.GetOriginalType();
			if (unaryExpressionType != typeof(int)) return false;

			//如果是常量则直接输出, 否则输出表达式的值
			object expressionValue = expression is ConstantExpression ce ? ce.Value : expression.GetExpressionValue();

			//获取int值枚举
			var enumValue = Enum.ToObject(unaryExpressionOperandType, expressionValue);

			//以枚举类型包装常量表达式
			var constantExpression = Expression.Constant(enumValue, unaryExpressionOperandType);

			//下面是调整lambda表达式保持一致
			if (unaryFirst)
			{
				VisitUnary(unaryExpression);
				VisitConstant(constantExpression);
			}
			else
			{
				VisitConstant(constantExpression);
				VisitUnary(unaryExpression);
			}
			return true;
		}

		/// <summary>
		/// 按照格式合并所有条件部件
		/// </summary>
		/// <param name="format"></param>
		private void MergeConditionParts(string format)
		{
			var length = _conditionParts.Count;
			var parts = new string[length];

			for (int i = length - 1; i > -1; i--) parts[i] = _conditionParts.Pop();

			if (format != null) _conditionParts.Push(string.Format(format, parts));
		}

		/// <summary>
		/// 连接所有条件部件
		/// </summary>
		private void MergeConditionParts()
		{
			var connect = string.Concat(_conditionParts.Reverse());
			_conditionParts.Clear();
			_conditionParts.Push(connect);
		}

		#region 其他
		//private static string BinarExpressionProvider(Expression left, Expression right, ExpressionType type)
		//{
		//	string sb = "(";
		//	//先处理左边
		//	sb += ExpressionRouter(left);

		//	sb += type.OperatorCast();

		//	//再处理右边
		//	var tmpStr = ExpressionRouter(right);

		//	if (tmpStr == "null")
		//	{
		//		if (sb.EndsWith(" ="))
		//			sb = sb[0..^1] + " IS NULL";
		//		else if (sb.EndsWith("<>"))
		//			sb = sb[0..^2] + " IS NOT NULL";
		//	}
		//	else
		//		sb += tmpStr;
		//	return sb += ")";
		//}

		//private static string ExpressionRouter(Expression expression)
		//{
		//	switch (expression)
		//	{
		//		case BinaryExpression be:
		//			return BinarExpressionProvider(be.Left, be.Right, be.NodeType);

		//		case MemberExpression me:
		//			return me.Member.Name;

		//		case NewArrayExpression ae:
		//			StringBuilder tmpstr = new();
		//			foreach (var ex in ae.Expressions)
		//			{
		//				tmpstr.Append(ExpressionRouter(ex));
		//				tmpstr.Append(',');
		//			}
		//			return tmpstr.ToString(0, tmpstr.Length - 1);

		//		case MethodCallExpression mce:
		//			switch (mce.Method.Name)
		//			{
		//				case "Like": return string.Format("({0} like {1})", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));

		//				case "NotLike": return string.Format("({0} Not like {1})", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));

		//				case "In": return string.Format("{0} In ({1})", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));

		//				case "NotIn": return string.Format("{0} Not In ({1})", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));

		//				case "StartWith": return string.Format("{0} like '{1}%'", ExpressionRouter(mce.Arguments[0]), ExpressionRouter(mce.Arguments[1]));
		//			}
		//			break;

		//		case ConstantExpression ce:

		//			if (ce.Value == null)
		//				return "null";

		//			else if (ce.Value is ValueType)
		//				return ce.Value.ToString();

		//			else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
		//				return string.Format("'{0}'", ce.Value.ToString());
		//			break;

		//		case UnaryExpression ue:
		//			return ExpressionRouter(ue.Operand);
		//	}
		//	return null;
		//}


		#endregion
	}
}
