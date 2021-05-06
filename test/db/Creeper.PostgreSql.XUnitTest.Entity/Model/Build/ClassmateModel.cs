﻿using Creeper.Driver;
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

	[CreeperDbTable(@"""public"".""classmate""", typeof(DbMain), DataBaseKind.PostgreSql)]
	public partial class ClassmateModel : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true)]
		public Guid Teacher_id { get; set; }

		[CreeperDbColumn(Primary = true)]
		public Guid Student_id { get; set; }

		[CreeperDbColumn(Primary = true)]
		public Guid Grade_id { get; set; }

		public DateTime? Create_time { get; set; }
		#endregion
	}
}
