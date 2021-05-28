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
		/// <summary>
		/// 数据库种类
		/// </summary>
		public DataBaseKind DataBaseKind { get; }

		public CreeperDbTableAttribute(string tableName, DataBaseKind dataBaseKind)
		{
			TableName = tableName;
			DataBaseKind = dataBaseKind;
		}
	}
}
