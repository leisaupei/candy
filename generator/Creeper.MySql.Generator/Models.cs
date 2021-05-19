using Creeper.Generator.Common.Extensions;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.MySql.Generator
{
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
		/// 数据类型
		/// </summary>
		public string DbDataType { get; set; }
		/// <summary>
		/// 是否非空
		/// </summary>
		public bool IsNullable { get; set; }
		/// <summary>
		/// 是否主键
		/// </summary>
		public bool IsPrimaryKey { get; set; }
		/// <summary>
		/// C#类型()
		/// </summary>
		public string RelType { get; set; }
		/// <summary>
		/// 是否自增
		/// </summary>
		public bool IsIdentity { get; set; }

	}

	public class EnumTypeInfo
	{
		/// <summary>
		/// 表名
		/// </summary>
		public string TableName { get; set; }

		/// <summary>
		/// 枚举成员
		/// </summary>
		public string ColumnType { get; set; }

		/// <summary>
		/// 字段名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 成员
		/// </summary>
		public string[] Elements { get; set; }
	}
}
