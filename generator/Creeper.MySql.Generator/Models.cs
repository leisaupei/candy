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
		public bool IsNotNull { get; set; }
		/// <summary>
		/// 是否主键
		/// </summary>
		public bool IsPrimaryKey { get; set; }
		/// <summary>
		/// C#类型
		/// </summary>
		public string RelType { get; set; }
		/// <summary>
		/// 是否自增
		/// </summary>
		public bool IsIdentity { get; set; }

	}
	/// <summary>
	/// 
	/// </summary>
	public class PrimarykeyInfo
	{
		/// <summary>
		/// 
		/// </summary>
		public string Field { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public string TypeName { get; set; }
		public string FieldUpCase => Field.ToUpperPascal();
	}
}
