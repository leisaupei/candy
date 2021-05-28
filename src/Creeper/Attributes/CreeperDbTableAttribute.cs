using System;
using Creeper.Generic;
namespace Creeper.Attributes
{
	/// <summary>
	/// 数据库表特性
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true)]
	public class CreeperDbTableAttribute : Attribute
	{
		/// <summary>
		/// 表名
		/// </summary>
		public string TableName { get; }
		public DataBaseKind DbKind { get; set; }
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="tableName">表名</param>
		public CreeperDbTableAttribute(string tableName, DataBaseKind dbKind)
		{
			TableName = tableName;
			DbKind = dbKind;
		}
	}
}
