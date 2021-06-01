using Creeper.Driver;
using System;
using Creeper.Attributes;
using Creeper.Generic;
using Creeper.MySql.XUnitTest.Entity.Options;

namespace Creeper.MySql.XUnitTest.Entity.Model
{
	[CreeperDbTable(@"`student`", DataBaseKind.MySql)]
	public partial class StudentModel : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true, Identity = true)]
		public int Id { get; set; }

		public int People_id { get; set; }

		public int? Stu_no { get; set; }

		public string Class_name { get; set; }

		public string Teacher_name { get; set; }
		#endregion
	}
}
