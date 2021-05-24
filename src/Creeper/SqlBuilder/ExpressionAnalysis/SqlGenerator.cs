﻿using System.Collections.Generic;
using System.Data.Common;
using System;
using System.Linq.Expressions;
using Creeper.SqlBuilder.ExpressionAnalysis;
using Creeper.DbHelper;
using Creeper.Generic;
using Creeper.Driver;

namespace Creeper.SqlBuilder.ExpressionAnalysis
{
	/// <summary>
	/// lambda表达式转为where条件sql
	/// </summary>
	public class SqlGenerator
	{
		#region Expression转成Where/Selector/UnionExpression
		/// <summary>
		/// 获取参数返回的sql语句
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="fnCreateParameter"></param>
		/// <returns></returns>
		public static ExpressionModel GetExpression(Expression expression, Func<string, object, DbParameter> fnCreateParameter, ICreeperDbConverter converter)
		{
			ConditionBuilder conditionBuilder = new ConditionBuilder(converter);
			conditionBuilder.Build(expression);
			var argumentsLength = conditionBuilder.Arguments.Length;

			var ps = new DbParameter[argumentsLength];

			var indexs = new string[argumentsLength];

			for (int i = 0; i < argumentsLength; i++)
			{
				var index = ParameterCounting.Index;
				ps[i] = fnCreateParameter(index, conditionBuilder.Arguments[i]);
				indexs[i] = string.Concat("@", index);
			}
			string cmdText = string.Format(conditionBuilder.Condition, indexs);
			return new ExpressionModel(cmdText, ps, conditionBuilder.Alias);
		}

		/// <summary>
		/// 获取selector
		/// </summary>
		/// <param name="selector">表达式</param>
		/// <param name="converter">数据库类型转换</param>
		/// <param name="alias">是否包含别名</param>
		/// <param name="special">是否包含特殊转换</param>
		/// <returns></returns>
		public static string GetSelector(Expression selector, ICreeperDbConverter converter, bool alias = true, bool special = false)
		{
			ConditionBuilder conditionBuilder = new ConditionBuilder(converter);
			conditionBuilder.Build(selector);

			var key = conditionBuilder.Condition;
			if (!alias)
			{
				var keyArray = key.Split('.');
				key = keyArray.Length > 1 ? keyArray[1] : key;
			}

			if (special)
			{
				if (selector is LambdaExpression le && converter.TrySpecialOutput(le.ReturnType, out string format))
					key = string.Format(format, key);
			}
			return key;
		}
		#endregion
	}
}
