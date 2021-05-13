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
	/// <summary>
	/// VIEW
	/// </summary>
	[CreeperDbTable(@"`test_view`", typeof(DbDemo), DataBaseKind.MySql)]
	public partial class TEST_VIEWView : ICreeperDbModel
	{
		#region Properties
		/// <summary>
		/// 主键id
		/// 唯一键
		/// </summary>
		public  Id { get; set; }

		/// <summary>
		/// 姓名
		/// </summary>
		public  Name { get; set; }

		/// <summary>
		/// 年龄
		/// </summary>
		public  Age { get; set; }
		#endregion
	}
}
