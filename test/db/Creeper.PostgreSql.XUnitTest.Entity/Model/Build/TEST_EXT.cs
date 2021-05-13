using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Creeper.Attributes;
using Creeper.Generic;
using Creeper.PostgreSql.XUnitTest.Entity.Options;
namespace Creeper.PostgreSql.XUnitTest.Entity.Model.Demo
{
	[CreeperDbTable(@"`test_ext`", typeof(DbDemo), DataBaseKind.MySql)]
	public partial class TEST_EXT : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true)]
		public  Id { get; set; }

		public  Bio { get; set; }
		#endregion
	}
}
