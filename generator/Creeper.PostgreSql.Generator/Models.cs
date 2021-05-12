using Creeper.Driver;
using Creeper.Generator.Common.Extensions;
using Creeper.Generic;
using NpgsqlTypes;
using System.Collections.Generic;

namespace Creeper.PostgreSql.Generator
{
	public class MappingOptions
	{

		/// <summary>
		/// 
		/// </summary>
		public static List<string> XmlTypeName { get; } = new List<string>();

		/// <summary>
		/// 
		/// </summary>
		public static List<string> GeometryTableTypeName { get; } = new List<string>();
	}

	/// <summary>
	/// 
	/// </summary>
	public class TableFieldModel
	{
		/// <summary>
		/// 字段名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 字段名称 
		/// </summary>
		public string NameUpCase => Name.ToUpperPascal();
		/// <summary>
		/// 字段数据库长度
		/// </summary> 
		public int Length { get; set; }
		/// <summary>
		/// 标识
		/// </summary>
		public string Comment { get; set; }
		/// <summary>
		/// C#类型
		/// </summary>
		public string RelType { get; set; }
		/// <summary>
		/// 数据库类型
		/// </summary>
		public string DbType { get; set; }
		/// <summary>
		/// 数据类型
		/// </summary>
		public string DbDataType { get; set; }
		/// <summary>
		/// 是否自增
		/// </summary>
		public bool IsIdentity { get; set; }
		/// <summary>
		/// 是否数组
		/// </summary>
		public bool IsArray => Dimensions > 0;
		/// <summary>
		/// 是否枚举
		/// </summary>
		public bool IsEnum => DbDataType == "e";
		/// <summary>
		/// 是否非空
		/// </summary>
		public bool IsNotNull { get; set; }
		/// <summary>
		/// npgsql 数据库类型
		/// </summary>
		public NpgsqlDbType NpgsqlDbType { get; set; }
		/// <summary>
		/// 是否主键
		/// </summary>
		public bool IsPrimaryKey { get; set; }
		/// <summary>
		/// 类型分类
		/// </summary>
		public string Typcategory { get; set; }
		/// <summary>
		/// 类型schema(命名空间)
		/// </summary>
		public string Nspname { get; set; }
		/// <summary>
		/// 是否唯一键
		/// </summary>
		public bool IsUnique { get; set; }
		/// <summary>
		/// C#类型
		/// </summary>
		public string CSharpType { get; set; }
		/// <summary>
		/// 维度
		/// </summary>
		public int Dimensions { get; set; }
		/// <summary>
		/// 默认值
		/// </summary>
		public string Column_default { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public class EnumTypeInfo
	{
		/// <summary>
		/// 
		/// </summary>
		public int Oid { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Typname { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string Nspname { get; set; }
		/// <summary>
		/// 注释
		/// </summary>
		public string Description { get; set; }
	}
	/// <summary>
	/// 
	/// </summary>
	public class CompositeTypeInfo
	{
		/// <summary>
		/// 数据库复合类型名称
		/// </summary>
		public string Typename { get; set; }
		/// <summary>
		/// 命名空间
		/// </summary>
		public string Nspname { get; set; }
		/// <summary>
		/// 字段名称
		/// </summary>
		public string Attname { get; set; }
		/// <summary>
		/// 数据库类型名称 如:varchar
		/// </summary>
		public string Typname { get; set; }
		/// <summary>
		/// 维度
		/// </summary>
		public short Attndims { get; set; }
		/// <summary>
		/// 数据库分类类型
		/// </summary>
		public string Typtype { get; set; }
		/// <summary>
		/// 类型描述
		/// </summary>
		public string Description { get; set; }

	}
}
