﻿using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using Creeper.PostgreSql.Extensions;
using Creeper.SqlBuilder;
using Newtonsoft.Json.Linq;
using Npgsql;
using Npgsql.LegacyPostgis;
using NpgsqlTypes;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace Creeper.PostgreSql
{
	public class PostgreSqlTypeConverter : CreeperDbTypeConvertBase
	{
		public override DataBaseKind DataBaseKind => DataBaseKind.PostgreSql;

		public override string DbFieldMark => "\"";

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
					case var t when t == typeof(PostgisGeometry):
						return (PostgisGeometry)value;
					// jsonb json 类型
					case var t when PostgreSqlTypeMappingExtensions.JTypes.Contains(t):
						return JToken.Parse(value?.ToString() ?? "{}");

					case var t when t == typeof(NpgsqlTsQuery):
						return NpgsqlTsQuery.Parse(value.ToString());

					case var t when t == typeof(BitArray) && value is bool b:
						return new BitArray(1, b);

					default:
						var converter = TypeDescriptor.GetConverter(convertType);
						return converter.CanConvertFrom(value.GetType()) ? converter.ConvertFrom(value) : System.Convert.ChangeType(value, convertType);
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

		private static readonly Regex _paramPattern = new Regex(@"(^(\-|\+)?\d+(\.\d+)?$)|(^SELECT\s.+\sFROM\s)|(true)|(false)", RegexOptions.IgnoreCase);

		public static string GetParamValue(object value)
		{
			Type type = value.GetType();
			if (type.IsArray)
			{
				var arrStr = (value as object[]).Select(a => $"'{a?.ToString() ?? ""}'");
				return $"array[{string.Join(",", arrStr)}]";
			}
			return value?.ToString();
		}
		public override DbConnection GetDbConnection(string connectionString)
			=> new NpgsqlConnection(connectionString);

		public override DbParameter GetDbParameter(string name, object value)
			=> new NpgsqlParameter(name, value);
	}
}
