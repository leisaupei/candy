using Creeper.Driver;
using System;
using Creeper.Attributes;
using Creeper.Generic;

namespace Creeper.Sqlite.XUnitTest.Entity.Model
{
	[CreeperDbTable(@"category", DataBaseKind.Sqlite)]
	public partial class CategoryModel : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true)]
		public long Id { get; set; }

		public string Name { get; set; }
		#endregion
	}
}
