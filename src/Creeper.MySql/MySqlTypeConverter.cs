using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using Creeper.SqlBuilder;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace Creeper.MySql
{
	public class MySqlTypeConverter : CreeperDbTypeConvertBase
	{
		private static readonly Regex _paramPattern = new Regex(@"(^(\-|\+)?\d+(\.\d+)?$)|(^SELECT\s.+\sFROM\s)|(true)|(false)", RegexOptions.IgnoreCase);

		public override DataBaseKind DataBaseKind => DataBaseKind.MySql;

		public override string DbFieldMark => "`";

		public override T ConvertDbData<T>(object value)
		{
			return (T)ConvertDbData(value, typeof(T));
		}

		public override object ConvertDbData(object value, Type convertType)
		{
			if (value is null) return value;
			try
			{
				switch (convertType)
				{
					default:
						var converter = TypeDescriptor.GetConverter(convertType);
						return converter.CanConvertFrom(value.GetType()) ? converter.ConvertFrom(value) : Convert.ChangeType(value, convertType);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public override string ConvertSqlToString(ISqlBuilder sqlBuilder)
		{
			var sql = sqlBuilder.CommandText;

			foreach (var p in sqlBuilder.Params)
			{
				var value = GetParamValue(p.Value);
				var key = string.Concat("@", p.ParameterName);
				if (value == null)
					sql = SqlHelper.GetNullSql(sql, key);

				else if (_paramPattern.IsMatch(value) && p.DbType == DbType.String)
					sql = sql.Replace(key, value);

				else if (value.Contains("array"))
					sql = sql.Replace(key, value);

				else
					sql = sql.Replace(key, $"'{value}'");
			}
			return sql.Replace("\r", " ").Replace("\n", " ");
		}

		public override DbParameter GetDbParameter(string name, object value)
			=> new MySqlParameter(name, value);

		public static string GetParamValue(object value)
		{
			return value?.ToString();
		}

		public override DbConnection GetDbConnection(string connectionString)
		=> new MySqlConnection(connectionString);
	}

}
