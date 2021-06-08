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
		internal static bool UseGeometryType = false;
		public override DataBaseKind DataBaseKind => DataBaseKind.MySql;

		public override string DbFieldMark => "`";
		public override string CastStringDbType => "CHAR";
		public override object ConvertDbData(object value, Type convertType)
		{
			switch (convertType)
			{
				case var t when _geometryTypes.Contains(t):
					return MySqlGeometry.Parse(value.ToString());

				default:
					var converter = TypeDescriptor.GetConverter(convertType);
					return converter.CanConvertFrom(value.GetType()) ? converter.ConvertFrom(value) : Convert.ChangeType(value, convertType);
			}
		}

		public override DbParameter GetDbParameter(string name, object value)
			=> new MySqlParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
		=> new MySqlConnection(connectionString);

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
		/// <returns></returns>
		/// 

		/// 

		/// <param name="allKeys"></param><param name="returning"></param>
		
		public override string GetUpsertCommandText(string mainTable, IList<string> primaryKeys, IList<string> identityKeys, IDictionary<string, string> upsertSets, IList<string> allKeys, bool returning)
		{
			//自增主键
			var exceptIdentityKey = upsertSets.Keys.Except(identityKeys);

			return @$"INSERT INTO {mainTable}({string.Join(", ", exceptIdentityKey)}) VALUES({string.Join(", ", exceptIdentityKey.Select(a => upsertSets[a]))}) 
ON DUPLICATE KEY UPDATE {string.Join(", ", exceptIdentityKey.Except(primaryKeys).Select(a => $"{a} = {upsertSets[a]}"))}";
		}
	}

}
