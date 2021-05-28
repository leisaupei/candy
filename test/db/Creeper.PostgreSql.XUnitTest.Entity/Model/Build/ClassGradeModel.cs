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
	/// <summary>
	/// 班级
	/// </summary>
	[CreeperDbTable(@"""class"".""grade""", DataBaseKind.PostgreSql)]
	public partial class ClassGradeModel : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true, IgnoreFlags = IgnoreWhen.Insert | IgnoreWhen.Returning)]
		public Guid Id { get; set; }

		/// <summary>
		/// 班级名称
		/// </summary>
		public string Name { get; set; }

		public DateTime Create_time { get; set; }
		#endregion
	}
}
