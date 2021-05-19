﻿using Creeper.Driver;
using System;
using Creeper.Attributes;
using Creeper.Generic;
using Creeper.MySql.XUnitTest.Entity.Options;

namespace Creeper.MySql.XUnitTest.Entity.Model
{
	[CreeperDbTable(@"`type_test`", typeof(DbMain), DataBaseKind.MySql)]
	public partial class TypeTestModel : ICreeperDbModel
	{
		#region Properties
		[CreeperDbColumn(Primary = true)]
		public int Id { get; set; }

		public long? Bigint_t { get; set; }

		public byte[] Binary_t { get; set; }

		public byte? Bit_t { get; set; }

		public byte[] Blob_t { get; set; }

		public string Char_t { get; set; }

		public DateTime? Date_t { get; set; }

		public DateTime? Datetime_t { get; set; }

		public decimal? Decimal_t { get; set; }

		public double? Double_t { get; set; }

		public TypeTestEnumT? Enum_t { get; set; }

		public float? Float_t { get; set; }

		public global::MySql.Data.Types.MySqlGeometry? Geometry_t { get; set; }

		public System.Drawing.Point[][] Geometrycollection_t { get; set; }

		public int? Integer_t { get; set; }

		public string Json_t { get; set; }

		public System.Drawing.Point[] Linestring_t { get; set; }

		public decimal? Numeric_t { get; set; }

		public System.Drawing.Point? Point_t { get; set; }

		public System.Drawing.Point[] Polygon_t { get; set; }

		public double? Real_t { get; set; }

		public string Set_t { get; set; }

		public short? Smallint_t { get; set; }

		public string Text_t { get; set; }

		public TimeSpan? Time_t { get; set; }

		public DateTime? Timestamp_t { get; set; }

		public byte[] Tinyblob_t { get; set; }

		public sbyte? Tinyint_t { get; set; }

		public string Tinytext_t { get; set; }

		public byte[] Varbinary_t { get; set; }

		public string Varchar_t { get; set; }

		public short? Year_t { get; set; }
		#endregion
	}
}