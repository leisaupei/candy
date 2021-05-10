using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Net.NetworkInformation;
using NpgsqlTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Creeper.Attributes;
using Creeper.Generic;
using Creeper.PostgreSql.XUnitTest.Entity.Options;

namespace Creeper.PostgreSql.XUnitTest.Entity.Model
{

	[CreeperDbTable(@"""gmallmq"".""int_test""", typeof(DbMain), DataBaseKind.PostgreSql)]
	public partial class GmallmqIntTestModel : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true)]
		public int Id { get; set; }

		public string Ints { get; set; }
		#endregion
	}
}
