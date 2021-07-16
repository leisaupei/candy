using Creeper.DbHelper;
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;

namespace Creeper.PostgreSql
{
	internal class PostgreSqlConverter : CreeperDbConverterBase
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

		public override string GetUpsertCommandText<TModel>(string mainTable, IList<string> primaryKeys, IList<string> identityKeys, IDictionary<string, string> upsertSets, bool returning)
		{
			var ret = returning ? $"RETURNING *" : null;
			if (identityKeys.Count > 0)
			{
				//自增主键
				var exceptIdentityKey = upsertSets.Keys.Except(identityKeys);

				var pksWhere = string.Join(" AND ", primaryKeys.Select(a => $"{a} = {upsertSets[a]}"));
				return @$"WITH upsert AS (
						UPDATE {mainTable} SET {string.Join(", ", exceptIdentityKey.Except(primaryKeys).Select(a => $"{a} = {upsertSets[a]}"))} 
						WHERE {pksWhere} RETURNING {string.Join(", ", primaryKeys)}
					) 
					INSERT INTO {mainTable} ({string.Join(", ", exceptIdentityKey)})
					SELECT {string.Join(", ", exceptIdentityKey.Select(a => upsertSets[a]))}
					WHERE NOT EXISTS(SELECT 1 FROM upsert WHERE {pksWhere})
					{ret}";
			}
			else
			{
				return @$"
	INSERT INTO {mainTable} ({string.Join(", ", upsertSets.Keys)}) VALUES({string.Join(", ", upsertSets.Values)})
	ON CONFLICT({string.Join(", ", primaryKeys)}) DO UPDATE
	SET {string.Join(", ", upsertSets.Keys.Except(primaryKeys).Select(a => $"{a} = EXCLUDED.{a}"))}
	{ret}";
			}
		}
		public override string GetUpdateCommandText<TModel>(string mainTable, string mainAlias, List<string> setList, List<string> whereList, bool returning, string[] pks)
		{
			var columns = GetReturningColumns<TModel>();
			return $"UPDATE {mainTable} AS {mainAlias} SET {string.Join(",", setList)} WHERE {string.Join(" AND ", whereList)} {(returning ? $"RETURNING {columns}" : null)}";
		}

		public override string GetInsertCommandText<TModel>(string mainTable, Dictionary<string, string> insertKeyValuePairs, string[] wheres, bool returning)
		{
			var columns = GetReturningColumns<TModel>();
			var sql = $"INSERT INTO {mainTable} ({string.Join(", ", insertKeyValuePairs.Keys)})";
			if (wheres.Length == 0)
				sql += $" VALUES({string.Join(", ", insertKeyValuePairs.Values)})";
			else
				sql += $" SELECT {string.Join(", ", insertKeyValuePairs.Values)} WHERE {string.Join(" AND ", wheres)}";

			return string.Concat(sql, returning ? $" RETURNING {columns}" : null);
		}
	}
}
