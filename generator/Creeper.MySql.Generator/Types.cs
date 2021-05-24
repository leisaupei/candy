using Creeper.Generator.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Creeper.MySql.Generator
{
	/// <summary>
	/// 
	/// </summary>
	public static class Types
	{
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string ConvertArrayToSql<T>(IEnumerable<T> value)
		{
			return string.Join(", ", value.Select(f => $"'{f}'"));
		}
		/// <summary>
		/// 数据库类型转化成C#类型String
		/// </summary>
		/// <param name="dataType"></param>
		/// <param name="isNullable"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string ConvertMySqlDataTypeToCSharpType(string dataType, int length)
		{
			var cSharpType = dataType;
			switch (dataType)
			{
				//确定
				case "bigint": cSharpType = "long"; break;

				case "tinyint":
					cSharpType = length switch
					{
						var l when l == 1 => "bool",
						_ => "sbyte",
					};
					break;

				case "int":
				case "mediumint":
				case "integer": cSharpType = "int"; break;

				case "year":
				case "smallint": cSharpType = "short"; break;
				case "time": cSharpType = "TimeSpan"; break;

				case "timestamp":
				case "date":
				case "datetime": cSharpType = "DateTime"; break;

				case "numeric":
				case "decimal": cSharpType = "decimal"; break;

				case "float": cSharpType = "float"; break;

				case "real":
				case "double": cSharpType = "double"; break;

				case "bit":
					cSharpType = length switch
					{
						var l when l == 1 => "bool",
						var l when l <= 8 => "byte",
						var l when l <= 16 => "ushort",
						var l when l <= 32 => "uint",
						var l when l > 32 => "ulong",
						_ => "ulong",
					};
					break;

				case "tinyblob":
				case "longblob":
				case "mediumblob":
				case "blob":
				case "binary":
				case "varbinary":
					cSharpType = "byte[]"; break;


				case "tinytext":
				case "mediumtext":
				case "longtext":
				case "char":
				case "text":
				case "json":
				case "set":
				case "varchar": cSharpType = "string"; break;

				case "point": cSharpType = "Creeper.MySql.Types.MySqlPoint"; break;
				case "multipoint": cSharpType = "Creeper.MySql.Types.MySqlMultiPoint"; break;

				case "polygon": cSharpType = "Creeper.MySql.Types.MySqlPolygon"; break;
				case "multipolygon": cSharpType = "Creeper.MySql.Types.MySqlMultiPolygon"; break;

				case "linestring": cSharpType = "Creeper.MySql.Types.MySqlLineString"; break;
				case "multilinestring": cSharpType = "Creeper.MySql.Types.MySqlMultiLineString"; break;

				case "geometry": cSharpType = "Creeper.MySql.Types.MySqlGeometry"; break;
				case "geometrycollection": cSharpType = "Creeper.MySql.Types.MySqlGeometryCollection"; break;

				case "enum":
					return dataType;
				default:
					return dataType;
			}
			return cSharpType;
		}

		/// <summary>
		/// 数据库类型转化成NpgsqlDbType String
		/// </summary>
		/// <param name="dbType"></param>
		/// <param name="isArray"></param>
		/// <returns></returns>
		public static string ConvertDbTypeToNpgsqlDbTypeString(string dbType, bool isArray)
		{
			string _type;
			switch (dbType)
			{
				case "bit": _type = "NpgsqlDbType.Bit"; break;
				case "varbit": _type = "NpgsqlDbType.Varbit"; break;

				case "bool": _type = "NpgsqlDbType.Boolean"; break;
				case "box": _type = "NpgsqlDbType.Box"; break;
				case "bytea": _type = "NpgsqlDbType.Bytea"; break;
				case "circle": _type = "NpgsqlDbType.Circle"; break;

				case "float4": _type = "NpgsqlDbType.Real"; break;
				case "float8": _type = "NpgsqlDbType.Double"; break;

				case "money": _type = "NpgsqlDbType.Money"; break;
				case "decimal":
				case "numeric": _type = "NpgsqlDbType.Numeric"; break;

				case "cid": _type = "NpgsqlDbType.Cid"; break;
				case "cidr": _type = "NpgsqlDbType.Cidr"; break;
				case "inet": _type = "NpgsqlDbType.Inet"; break;

				case "serial2":
				case "int2": _type = "NpgsqlDbType.Smallint"; break;

				case "serial4":
				case "int4": _type = "NpgsqlDbType.Integer"; break;

				case "serial8":
				case "int8": _type = "NpgsqlDbType.Bigint"; break;

				case "time": _type = "NpgsqlDbType.Time"; break;
				case "interval": _type = "NpgsqlDbType.Interval"; break;

				case "json": _type = "NpgsqlDbType.Json"; break;
				case "jsonb": _type = "NpgsqlDbType.Jsonb"; break;

				case "line": _type = "NpgsqlDbType.Line"; break;
				case "lseg": _type = "NpgsqlDbType.LSeg"; break;
				case "macaddr": _type = "NpgsqlDbType.MacAddr"; break;
				case "path": _type = "NpgsqlDbType.Path"; break;
				case "point": _type = "NpgsqlDbType.Point"; break;
				case "polygon": _type = "NpgsqlDbType.Polygon"; break;

				case "xml": _type = "NpgsqlDbType.Xml"; break;
				case "char": _type = "NpgsqlDbType.InternalChar"; break;
				case "bpchar": _type = "NpgsqlDbType.Char"; break;
				case "varchar": _type = "NpgsqlDbType.Varchar"; break;
				case "text": _type = "NpgsqlDbType.Text"; break;

				case "name": _type = "NpgsqlDbType.Name"; break;
				case "date": _type = "NpgsqlDbType.Date"; break;
				case "timetz": _type = "NpgsqlDbType.TimeTz"; break;
				case "timestamp": _type = "NpgsqlDbType.Timestamp"; break;
				case "timestamptz": _type = "NpgsqlDbType.TimestampTz"; break;

				case "tsquery": _type = "NpgsqlDbType.TsQuery"; break;
				case "tsvector": _type = "NpgsqlDbType.TsVector"; break;
				case "int2vector": _type = "NpgsqlDbType.Int2Vector"; break;
				case "hstore": _type = "NpgsqlDbType.Hstore"; break;
				case "macaddr8": _type = "NpgsqlDbType.MacAddr8"; break;
				case "uuid": _type = "NpgsqlDbType.Uuid"; break;
				case "oid": _type = "NpgsqlDbType.Oid"; break;
				case "oidvector": _type = "NpgsqlDbType.Oidvector"; break;
				case "refcursor": _type = "NpgsqlDbType.Refcursor"; break;
				case "regtype": _type = "NpgsqlDbType.Regtype"; break;
				case "tid": _type = "NpgsqlDbType.Tid"; break;
				case "xid": _type = "NpgsqlDbType.Xid"; break;
				default: _type = ""; break;
			}
			if (isArray)
			{
				//	var need = new string[] { "varchar", "bpchar", "date", "time" };
				if (_type.IsNotNullOrEmpty())
					_type += " | NpgsqlDbType.Array";
			}
			_type = _type.IsNotNullOrEmpty() ? ", " + _type : "";
			return _type;
		}

		/// <summary>
		/// 排除生成whereor条件的字段类型
		/// </summary>
		public static bool MakeWhereOrExceptType(string type)
		{
			string[] arr = { "datetime", "geometry", "jtoken", "byte[]" };
			if (arr.Contains(type.ToLower().Replace("?", "")))
				return false;
			return true;
		}


	}
}
