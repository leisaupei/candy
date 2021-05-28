using Creeper.Driver;
using System;
using Creeper.Attributes;
using Creeper.Generic;
using Creeper.MySql.XUnitTest.Entity.Options;

namespace Creeper.MySql.XUnitTest.Entity.Model
{
	[CreeperDbTable(@"`test_ext`", DataBaseKind.MySql)]
	public partial class TestExtModel : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true)]
		public int Id { get; set; }

		public string Bio { get; set; }
		#endregion
	}
}
