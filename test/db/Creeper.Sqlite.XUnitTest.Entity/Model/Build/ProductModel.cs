using Creeper.Driver;
using System;
using Creeper.Attributes;
using Creeper.Generic;

namespace Creeper.Sqlite.XUnitTest.Entity.Model
{
	[CreeperDbTable(@"product", DataBaseKind.Sqlite)]
	public partial class ProductModel : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true)]
		public long Id { get; set; }

		public string Name { get; set; }

		public double? Price { get; set; }

		public byte[] Img { get; set; }

		public long? Stock { get; set; }

		public long? Category_id { get; set; }

		#endregion
	}
}
