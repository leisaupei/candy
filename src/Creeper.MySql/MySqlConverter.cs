using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using Creeper.MySql.Types;
using Creeper.SqlBuilder;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace Creeper.MySql
{
	internal class MySqlConverter : CreeperDbConverterBase
	{
		static readonly Type[] _geometryTypes = new[] {
			typeof(MySqlGeometry),
			typeof(MySqlGeometryCollection),
			typeof(MySqlLineString),
			typeof(MySqlMultiLineString),
			typeof(MySqlMultiPoint),
			typeof(MySqlMultiPolygon),
			typeof(MySqlPoint),
			typeof(MySqlPolygon)
		};
		internal static bool UseGeometryType { get; set; } = false;
		public override DataBaseKind DataBaseKind => DataBaseKind.MySql;

		public override string DbFieldMark => "`";
		public override string CastStringDbType => "CHAR";
		public override object ConvertDbData(object value, Type convertType)
		{
			if (UseGeometryType && _geometryTypes.Contains(convertType))
				return MySqlGeometry.Parse(value.ToString());

			return base.ConvertDbData(value, convertType);
		}

		public override DbParameter GetDbParameter(string name, object value)
			=> new MySqlParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
		{
			var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
			connectionStringBuilder.AllowUserVariables = true;

			var connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
			return connection;
		}
		public override bool SetSpecialDbParameter(out string format, ref object value)
		{
			if (UseGeometryType && _geometryTypes.Contains(value.GetType()))
			{
				format = "ST_GeomFromText({0})";
				value = value.ToString();
				return true;
			}
			return base.SetSpecialDbParameter(out format, ref value);
		}
		public override bool TrySpecialOutput(Type type, out string format)
		{
			if (UseGeometryType && _geometryTypes.Contains(type))
			{
				format = "ST_AsText({0})";
				return true;
			}

			return base.TrySpecialOutput(type, out format);
		}

		/// <summary>
		/// 获取增补表达式
		/// </summary>
		/// <remarks>
		/// 注意: MySql派生唯一键会受影响
		/// 主键值为default(不赋值或忽略)时, 必定是插入。<br/>
		/// 若主键条件的行存在, 则更新该行; 否则插入一行, 主键取决于类型规则。<br/>
		/// - 整型自增主键: 根据数据库自增标识;<br/>
		/// - 随机唯一主键: Guid程序会自动生成, 其他算法需要赋值;
		/// </remarks>
		/// <param name="mainTable">表</param>
		/// <param name="primaryKeys">主键集合</param>
		/// <param name="identityKeys">没有赋值的自增键集合</param>
		/// <param name="upsertSets">需要设置的值</param>
		/// <param name="returning">是否返回</param>
		/// <returns></returns>
		public override string GetUpsertCommandText<TModel>(string mainTable, IList<string> primaryKeys, IList<string> identityKeys, IDictionary<string, string> upsertSets, bool returning)
		{
			if (returning)
				throw new NotSupportedException("mysql is not supported returning");
			//自增主键
			var exceptIdentityKey = upsertSets.Keys.Except(identityKeys);

			return @$"INSERT INTO {mainTable}({string.Join(", ", exceptIdentityKey)}) VALUES({string.Join(", ", exceptIdentityKey.Select(a => upsertSets[a]))}) 
ON DUPLICATE KEY UPDATE {string.Join(", ", exceptIdentityKey.Except(primaryKeys).Select(a => $"{a} = {upsertSets[a]}"))}";
		}

		public override string GetUpdateCommandText<TModel>(string mainTable, string mainAlias, List<string> setList, List<string> whereList, bool returning, string[] pks)
		{
			string ret1 = null, ret2 = null, ret3 = null;
			if (returning)
			{
				for (int i = 0; i < pks.Length; i++)
				{
					ret1 += string.Format("@{0}1 := ''", pks[i]);
					ret2 += string.Format("{1}{0}{1} = (SELECT @{0}1 := {1}{0}{1})", pks[i], DbFieldMark);
					ret3 += string.Format("{1}{0}{1} = @{0}1", pks[i], DbFieldMark);
					if (i != pks.Length - 1)
					{
						ret1 += ", ";
						ret2 += ", ";
						ret3 += " AND ";
					}
				}
				ret1 = $"SET {ret1};";
				ret2 = $", {ret2}";
				ret3 = $"; SELECT * FROM {mainTable} WHERE {ret3}";
			}
			return $"{ret1}UPDATE {mainTable} AS {mainAlias} SET {string.Join(",", setList)}{ret2} WHERE {string.Join(" AND ", whereList)} {ret3}";
		}
		public override string GetInsertCommandText<TModel>(string mainTable, Dictionary<string, string> insertKeyValuePairs, string[] wheres, bool returning)
		{
			var sql = $"INSERT INTO {mainTable} ({string.Join(", ", insertKeyValuePairs.Keys)})";
			if (wheres.Length == 0)
				sql += $" VALUES({string.Join(", ", insertKeyValuePairs.Values)})";
			else
				sql += $" SELECT {string.Join(", ", insertKeyValuePairs.Values)} FROM DUAL WHERE {string.Join(" AND ", wheres)}";

			if (returning)
			{
				var columns = GetReturningColumns<TModel>();
				var idpks = GetIdentityPrimaryKeys<TModel>();
				if (idpks.Length == 1)
				{
					sql += $"; SELECT {columns} FROM {mainTable} WHERE `{idpks[0]}` = @@IDENTITY";
				}
				else
				{
					var pks = GetPrimaryKeys<TModel>();
					sql += $"; SELECT {columns} FROM {mainTable} WHERE {string.Join(" AND ", pks.Select(a => $"`{a}` = {insertKeyValuePairs[a]}"))}";
				}
			}
			return sql;
		}
	}
}
