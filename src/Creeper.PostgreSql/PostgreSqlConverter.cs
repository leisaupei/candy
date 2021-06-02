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
	public class PostgreSqlConverter : CreeperDbConverterBase
	{
		public override DataBaseKind DataBaseKind => DataBaseKind.PostgreSql;

		public override string DbFieldMark => "\"";

		public override object ConvertDbData(object value, Type convertType)
		{
			return convertType switch
			{
				var t when t == typeof(PostgisGeometry) => (PostgisGeometry)value,
				// jsonb json 类型
				var t when PostgreSqlTypeMappingExtensions.JTypes.Contains(t) => JToken.Parse(value?.ToString() ?? "{}"),
				var t when t == typeof(NpgsqlTsQuery) => NpgsqlTsQuery.Parse(value.ToString()),
				var t when t == typeof(BitArray) && value is bool b => new BitArray(1, b),
				_ => base.ConvertDbData(value, convertType),
			};
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

				else if (ParamPattern.IsMatch(value) && p.DbType == DbType.String)
					sql = sql.Replace(key, value);

				else if (value.Contains("array"))
					sql = sql.Replace(key, value);

				else
					sql = sql.Replace(key, $"'{value}'");
			}
			return sql.Replace("\r", " ").Replace("\n", " ");
		}

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