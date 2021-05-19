using Creeper.Driver;
using System;
using Creeper.Attributes;
using Creeper.Generic;
using Creeper.MySql.XUnitTest.Entity.Options;

namespace Creeper.MySql.XUnitTest.Entity.Model
{
	[CreeperDbTable(@"`test_view`", typeof(DbMain), DataBaseKind.MySql)]
	public partial class TestViewViewModel : ICreeperDbModel
	{
		#region Properties
		/// <summary>
		/// 主键id
		/// 唯一键
		/// </summary>
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
