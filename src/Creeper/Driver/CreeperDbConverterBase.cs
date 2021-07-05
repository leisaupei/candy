using Creeper.DbHelper;
using Creeper.Extensions;
using Creeper.Generic;
using Creeper.SqlBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Creeper.Driver
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class CreeperDbConverterBase : ICreeperDbConverter
	{
		protected static readonly Regex ParamPattern = new(@"(^(\-|\+)?\d+(\.\d+)?$)|(^SELECT\s.+\sFROM\s)|(true)|(false)", RegexOptions.IgnoreCase);

		/// <summary>
		/// 
		/// </summary>
		public abstract DataBaseKind DataBaseKind { get; }

		public virtual string CastStringDbType => "VARCHAR";

		public virtual string StringConnectWord => "||";

		public virtual string DbFieldMark => "\"";

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public T ConvertDbData<T>(object value) => (T)ConvertDbData(value, typeof(T).GetOriginalType());

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="convertType"></param>
		/// <returns></returns>
		public virtual object ConvertDbData(object value, Type convertType)
		{
			if (value.GetType() == convertType)
				return value;
			var converter = TypeDescriptor.GetConverter(convertType);
			return converter.CanConvertFrom(value.GetType()) ? converter.ConvertFrom(value) : Convert.ChangeType(value, convertType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="convertType"></param>
		/// <returns></returns>
		public object ConvertDataReader(IDataReader reader, Type convertType)
		{
			bool isValueOrString = convertType == typeof(string) || convertType.IsValueType;
			object model;
			if (convertType.IsTuple())
			{
				int columnIndex = -1;
				model = GetValueTuple(convertType, reader, ref columnIndex);
			}
			else if (isValueOrString || convertType.IsEnum)
			{
				model = CheckType(reader[0], convertType);
			}
			else
			{
				model = Activator.CreateInstance(convertType);
				bool isDictionary = typeof(IDictionary).IsAssignableFrom(convertType); //判断是否字典类型

				for (int i = 0; i < reader.FieldCount; i++)
				{
					var value = reader[i].IsNullOrDBNull() ? null : reader[i];
					if (isDictionary)
						model.GetType().GetMethod("Add").Invoke(model, new[] { reader.GetName(i), value });
					else
					{
						if (value != null)
							SetPropertyValue(convertType, value, model, reader.GetName(i));
					}
				}
			}
			return model;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objReader"></param>
		/// <returns></returns>
		public T ConvertDataReader<T>(IDataReader objReader) => (T)ConvertDataReader(objReader, typeof(T));

		/// <summary>
		/// 遍历元组类型
		/// </summary>
		/// <param name="objType"></param>
		/// <param name="dr"></param>
		/// <param name="columnIndex"></param>
		/// <returns></returns>
		private object GetValueTuple(Type objType, IDataReader dr, ref int columnIndex)
		{
			if (objType.IsTuple())
			{
				FieldInfo[] fs = objType.GetFields();
				Type[] types = new Type[fs.Length];
				object[] parameters = new object[fs.Length];
				for (int i = 0; i < fs.Length; i++)
				{
					types[i] = fs[i].FieldType;
					parameters[i] = GetValueTuple(types[i], dr, ref columnIndex);
				}
				ConstructorInfo info = objType.GetConstructor(types);
				return info.Invoke(parameters);
			}
			// 当元组里面含有实体类
			if (objType.IsClass && !objType.IsSealed && !objType.IsAbstract)
			{
				if (!objType.GetInterfaces().Any(f => f == typeof(ICreeperDbModel)))
					throw new NotSupportedException("only the generate models.");

				var model = Activator.CreateInstance(objType);
				var isSet = false; // 这个实体类是否有赋值 没赋值直接返回 default

				var fs = EntityHelper.GetFields(objType);
				for (int i = 0; i < fs.Length; i++)
				{
					++columnIndex;
					if (!dr[columnIndex].IsNullOrDBNull())
					{
						isSet = true;
						SetPropertyValue(objType, dr[columnIndex], model, fs[i]);
					}
				}
				return isSet ? model : default;
			}
			else
			{
				++columnIndex;
				return CheckType(dr[columnIndex], objType);
			}
		}

		/// <summary>
		/// 反射设置实体类字段值
		/// </summary>
		/// <param name="objType">获取属性的类的类型</param>
		/// <param name="value">字段的值</param>
		/// <param name="model">需要设置的类</param>
		/// <param name="fs">字段名称</param>
		private void SetPropertyValue(Type objType, object value, object model, string fs)
		{
			var p = objType.GetProperty(fs, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if (p != null) p.SetValue(model, CheckType(value, p.PropertyType));
		}

		/// <summary>
		/// 对可空类型转化
		/// </summary>
		/// <param name="value"></param>
		/// <param name="valueType"></param>
		/// <returns></returns>
		private object CheckType(object value, Type valueType)
		{
			if (value.IsNullOrDBNull()) return null;
			valueType = valueType.GetOriginalType();
			return ConvertDbData(value, valueType);
		}

		protected Type GetOriginalType(Type type) => type.GetOriginalType();

		protected string[] GetPrimaryKeys<T>() => EntityHelper.GetPkFields<T>();
		protected string[] GetIdentityPrimaryKeys<T>() => EntityHelper.GetIdentityPkFields<T>();

		public virtual string ConvertSqlToString(ISqlBuilder sqlBuilder)
		{
			var sql = sqlBuilder.CommandText;

			foreach (var p in sqlBuilder.Params)
			{
				var value = p.Value?.ToString();
				var key = string.Concat("@", p.ParameterName);
				if (value == null) sql = SqlHelper.GetNullSql(sql, key);
				else if (ParamPattern.IsMatch(value) && p.DbType == DbType.String) sql = sql.Replace(key, value);
				else sql = sql.Replace(key, $"'{value}'");
			}
			return sql.Replace("\r", " ").Replace("\n", " ");
		}

		public abstract DbParameter GetDbParameter(string name, object value);

		public abstract DbConnection GetDbConnection(string connectionString);

		public virtual bool SetSpecialDbParameter(out string format, ref object value)
		{
			format = null;
			return false;
		}

		public virtual bool TrySpecialOutput(Type type, out string format)
		{
			format = null;
			return false;
		}

		public virtual string GetUpsertCommandText(string mainTable, IList<string> primaryKeys, IList<string> identityKeys, IDictionary<string, string> upsertSets, bool returning)
			=> throw new NotSupportedException();

		public virtual string GetUpdateCommandText(string mainTable, string mainAlias, List<string> setList, List<string> whereList, bool returning, string[] pks)
			=> throw new NotImplementedException();

		public virtual string GetInsertCommandText<TModel>(string mainTable, Dictionary<string, string> insertKeyValuePairs, string[] wheres, bool returning) where TModel : class, ICreeperDbModel, new()
		{
			throw new NotImplementedException();
		}
	}
}
