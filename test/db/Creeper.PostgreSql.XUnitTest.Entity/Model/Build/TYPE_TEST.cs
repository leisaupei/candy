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
	[CreeperDbTable(@"`type_test`", typeof(DbDemo), DataBaseKind.MySql)]
	public partial class TYPE_TEST : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true)]
		public  Id { get; set; }

		public  Bigint_t { get; set; }

		public  Binary_t { get; set; }

		public  Bit_t { get; set; }

		public  Blob_t { get; set; }

		public  Char_t { get; set; }

		public  Date_t { get; set; }

		public  Datetime_t { get; set; }

		public  Decimal_t { get; set; }

		public  Double_t { get; set; }

		public  Enum_t { get; set; }

		public  Float_t { get; set; }

		public  Geometry_t { get; set; }

		public  Geometrycollection_t { get; set; }

		public  Integer_t { get; set; }

		public  Json_t { get; set; }

		public  Linestring_t { get; set; }

		public  Numeric_t { get; set; }

		public  Point_t { get; set; }

		public  Polygon_t { get; set; }

		public  Real_t { get; set; }

		public  Set_t { get; set; }

		public  Smallint_t { get; set; }

		public  Text_t { get; set; }

		public  Time_t { get; set; }

		public  Timestamp_t { get; set; }

		public  Tinyblob_t { get; set; }

		public  Tinyint_t { get; set; }

		public  Tinytext_t { get; set; }

		public  Varbinary_t { get; set; }

		public  Varchar_t { get; set; }
		#endregion
	}
}
