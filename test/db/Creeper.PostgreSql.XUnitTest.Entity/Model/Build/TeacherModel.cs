using Creeper.Driver;
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
	[CreeperDbTable(@"""public"".""teacher""", typeof(DbMain), DataBaseKind.PostgreSql)]
	public partial class TeacherModel : ICreeperDbModel
	{
		#region Properties
		/// <summary>
		/// 学号
		/// </summary>
		public string Teacher_no { get; set; }

		public Guid People_id { get; set; }

		public DateTime Create_time { get; set; }

		[CreeperDbColumn(Primary = true)]
		public Guid Id { get; set; }
		#endregion
	}
}
