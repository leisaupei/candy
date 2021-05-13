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

namespace Creeper.PostgreSql.XUnitTest.Entity.Model
{
	[CreeperDbTable(@"""public"".""lsp_coll""", typeof(DbMain), DataBaseKind.PostgreSql)]
	public partial class LspCollViewModel : ICreeperDbModel
	{
		#region Properties
		public Guid? Id { get; set; }

		public int? Age { get; set; }

		public string Name { get; set; }

		public bool? Sex { get; set; }

		public DateTime? Create_time { get; set; }

		public string Address { get; set; }

		public JToken Address_detail { get; set; }

		public EtDataState? State { get; set; }
		#endregion
	}
}
