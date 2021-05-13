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
		/// <param name="dbType"></param>
		/// <returns></returns>
		public static string ConvertMySqlDataTypeToCSharpType(string dataType)
		{
			switch (dataType)
			{
				//确定
				case "bigint": return "long";
				case "int":
				case "integer": return "int";

				case "smallint": return "short";
				case "time": return "TimeSpan";

				case "timestamp":
				case "date": 
				case "datetime": return "DateTime";

				case "numeric": 
				case "decimal": return "decimal";

				case "char": return "char";
				case "float": return "float";
				case "double": return "double";

				case "bit": return "bool";
				case "binary": return "	byte[]";
				
				case "text": 
				case "varchar":return "string";
				//未确定
				case "blob": return "NpgsqlBox";

				case "enum":

				case "geometry": return "(IPAddress, int)";
				case "geometrycollection": return "IPAddress";


				case "linestring":
				case "longblob": return "int";


				case "longtext":
				case "mediumblob": return "long";

				case "mediumint":
				case "mediumtext": return "TimeSpan";

				case "multilinestring":
				case "multipoint": return "JToken";

				case "multipolygon": return "NpgsqlLine";
				case "point": return "NpgsqlLSeg";
				case "polygon": return "PhysicalAddress";
				case "real": return "NpgsqlPath";
				case "set": return "NpgsqlPoint";

				case "tinyblob":
				case "tinyint":
				case "varbinary": return "string";
				case "year":
				default:
					return dataType;
			}
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

		/// <summary>
		/// 去除下划线并首字母大写
		/// </summary>
		/// <param name="str"></param>
		/// <param name="len"></param>
		/// <returns></returns>
		public static string ExceptUnderlineToUpper(string str)
		{
			var strArr = str.Split('_');
			str = string.Empty;
			foreach (var item in strArr)
				str = string.Concat(str, item.ToUpperPascal());
			return str;
		}
	}
}
