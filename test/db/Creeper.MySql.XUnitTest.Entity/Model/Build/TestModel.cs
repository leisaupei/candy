using Creeper.Driver;
using System;
using Creeper.Attributes;
using Creeper.Generic;
using Creeper.MySql.XUnitTest.Entity.Options;

namespace Creeper.MySql.XUnitTest.Entity.Model
{
	/// <summary>
	/// 测试用表
	/// </summary>
	[CreeperDbTable(@"`test`", DataBaseKind.MySql)]
	public partial class TestModel : ICreeperDbModel
	{
		#region Properties
		/// <summary>
		/// 主键id
		/// 唯一键
		/// </summary>
		[CreeperDbColumn(Primary = true)]
		public int Id { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 年龄
		/// </summary>
		public int? Age { get; set; }
		#endregion
	}
}
