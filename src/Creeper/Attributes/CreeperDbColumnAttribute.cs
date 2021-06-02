using Creeper.Generic;
using System;

namespace Creeper.Attributes
{
	/// <summary>
	/// 主键特性
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, Inherited = true)]
	public class CreeperDbColumnAttribute : Attribute
	{
		/// <summary>
		/// 主键
		/// </summary>
		public bool Primary { get; set; } = false;

		/// <summary>
		/// 字段忽略, Flags
		/// </summary>
		public IgnoreWhen IgnoreFlags { get; set; } = IgnoreWhen.None;

		/// <summary>
		/// 自增字段
		/// </summary>
		public bool Identity { get; set; } = false;

		public CreeperDbColumnAttribute() { }
	}

	/// <summary>
	/// 数据库字段忽略策略
	/// </summary>
	[Flags]
	public enum IgnoreWhen
	{

		/// <summary>
		/// 不忽略
		/// </summary>
		None = 0x0,

		/// <summary>
		/// 插入数据时, 
		/// </summary>
		Insert = 0x1,

		/// <summary>
		/// 数据库查询返回数据时, 包括Select, Insert/Update/Upsert Returning
		/// </summary>
		Returning = 0x2,
	}
}
