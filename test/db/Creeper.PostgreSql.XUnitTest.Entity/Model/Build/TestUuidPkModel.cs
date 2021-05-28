﻿using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Net;
using NpgsqlTypes;
using System.Threading.Tasks;
using System.Threading;
using Creeper.Attributes;
using Creeper.Generic;
using Creeper.PostgreSql.XUnitTest.Entity.Options;

namespace Creeper.PostgreSql.XUnitTest.Entity.Model
{
	[CreeperDbTable(@"""test"".""uuid_pk""", DataBaseKind.PostgreSql)]
	public partial class TestUuidPkModel : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true)]
		public Guid Id { get; set; }

		/// <summary>
		/// 名字
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 年龄
		/// </summary>
		public int? Age { get; set; }
		#endregion
	}
}
