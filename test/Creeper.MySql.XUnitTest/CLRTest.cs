using Creeper.Extensions;
using Creeper.MySql.XUnitTest.Entity.Model;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Xunit;
using TestSpace = MySql.Data.Types;
namespace Creeper.MySql.XUnitTest
{
	public class CLRTest : BaseTest
	{
		public const int Pid = 1;

		[Theory]
		[InlineData(new byte[] { 0, 23, 1 })]
		public void Binary(byte[] bs)
		{
			var obj = _dbContext.ExecuteScalar("select `binary_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Binary_t, bs).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Binary_t);
			Assert.Equal(1, affrows);
			Assert.True(bs[0] == result[0]);
		}

		[Theory]
		[InlineData(new byte[] { 0, 23, 1 })]
		public void Varbinary(byte[] bs)
		{
			var obj = _dbContext.ExecuteScalar("select `varbinary_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Varbinary_t, bs).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Varbinary_t);
			Assert.Equal(1, affrows);
			Assert.True(bs[0] == result[0] && result.Length == bs.Length);
		}

		[Theory]
		[InlineData(9223372036234775807)]
		public void Bigint(long p)
		{
			var obj = _dbContext.ExecuteScalar("select `bigint_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Bigint_t, p).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Bigint_t);
			Assert.Equal(1, affrows);
			Assert.Equal(p, result);
		}

		[Theory]
		[InlineData(200)]
		public void Bit(byte b)
		{
			var obj = _dbContext.ExecuteScalar("select `bit_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Bit_t, b).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Bit_t);
			Assert.Equal(1, affrows);
			Assert.Equal(b, result);
		}

		[Theory]
		[InlineData(new byte[] { 0, 23, 1 })]
		public void Blob(byte[] bs)
		{
			var obj = _dbContext.ExecuteScalar("select `blob_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Blob_t, bs).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Blob_t);
			Assert.Equal(1, affrows);
			Assert.True(bs[0] == result[0] && bs.Length == result.Length);
		}

		[Theory]
		[InlineData(new byte[] { 0, 23, 1 })]
		public void TinyBlob(byte[] bs)
		{
			var obj = _dbContext.ExecuteScalar("select `tinyblob_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Tinyblob_t, bs).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Tinyblob_t);
			Assert.Equal(1, affrows);
			Assert.True(bs[0] == result[0] && bs.Length == result.Length);
		}

		[Theory]
		[InlineData("测试char")]
		public void Char(string str)
		{
			var obj = _dbContext.ExecuteScalar("select `char_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Char_t, str).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Char_t);
			Assert.Equal(1, affrows);
			Assert.Equal(str, result);
		}

		[Theory]
		[InlineData("2021-1-1 20:01:23")]
		public void Date(DateTime dt)
		{
			var obj = _dbContext.ExecuteScalar("select `date_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Date_t, dt).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Date_t);
			Assert.Equal(1, affrows);
			Assert.Equal(dt.Date, result?.Date);
			Assert.True(result?.Hour == 0 && result?.Minute == 0 && result?.Second == 0);

		}

		[Theory]
		[InlineData("2021-1-1 20:01:23")]
		public void Datetime(DateTime dt)
		{
			var obj = _dbContext.ExecuteScalar("select `datetime_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Datetime_t, dt).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Datetime_t);
			Assert.Equal(1, affrows);
			Assert.Equal(dt, result);
		}
		[Theory]
		[InlineData(23.34)]
		public void Decimal(decimal d)
		{
			var obj = _dbContext.ExecuteScalar("select `decimal_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Decimal_t, d).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Decimal_t);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData(23.34)]
		public void Double(double d)
		{
			var obj = _dbContext.ExecuteScalar("select `double_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Double_t, d).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Double_t);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData(TypeTestEnumT.已删除)]
		public void Enum(TypeTestEnumT d)
		{
			var obj = _dbContext.ExecuteScalar("select `enum_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Enum_t, TypeTestEnumT.已删除).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Enum_t);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData(12.33)]
		public void Float(float f)
		{
			var obj = _dbContext.ExecuteScalar("select `float_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Float_t, f).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Float_t);
			Assert.Equal(1, affrows);
			Assert.Equal(f, result);
		}

		[Fact]
		public void Geometry()
		{
			var obj = _dbContext.ExecuteScalar("select `geometry_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			MySqlGeometry geometry = new MySqlGeometry(22.141777, 113.754228);
			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Geometry_t, geometry).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Geometry_t);
			Assert.Equal(1, affrows);
			Assert.Equal(geometry.XCoordinate, result?.XCoordinate);
			Assert.Equal(geometry.YCoordinate, result?.YCoordinate);
		}

		[Fact]
		public void GeometryCollection()
		{
			var obj = _dbContext.ExecuteScalar<string>("select `geometrycollection_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();
			//var ge = new MySqlGeometry(MySqlDbType.Geometry, (byte[])obj);
			//MySqlGeometry geometry = new MySqlGeometry(22.141777, 113.754228);
			//var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Geometrycollection_t, geometry).Where(a => a.Id == Pid).ToAffectedRows();
			//var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Geometrycollection_t);
			//Assert.Equal(1, affrows);
			//Assert.Equal(geometry.XCoordinate, result.XCoordinate);
			//Assert.Equal(geometry.YCoordinate, result.YCoordinate);
		}

		[Theory]
		[InlineData(12)]
		public void Integer(int i)
		{
			var obj = _dbContext.ExecuteScalar("select `integer_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Integer_t, i).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Integer_t);
			Assert.Equal(1, affrows);
			Assert.Equal(i, result);
		}

		[Theory]
		[InlineData(12)]
		public void TinyInt(sbyte i)
		{
			var obj = _dbContext.ExecuteScalar("select `tinyint_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Tinyint_t, i).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Tinyint_t);
			Assert.Equal(1, affrows);
			Assert.Equal(i, result);
		}

		[Theory]
		[InlineData(@"{""s"":""sss""}")]
		public void Json(string s)
		{
			var obj = _dbContext.ExecuteScalar("select `json_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Json_t, s).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Json_t);
			//Assert.Equal(1, affrows);
			//Assert.Equal(i, result);
		}

		[Fact]
		public void Linestring()
		{

			var obj = _dbContext.ExecuteScalar("select `linestring_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();
			//var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Json_t, s).Where(a => a.Id == Pid).ToAffectedRows();
			//var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Json_t);
			//Assert.Equal(1, affrows);
			//Assert.Equal(i, result);
		}

		[Fact]
		public void Polygon()
		{
			var obj = _dbContext.ExecuteScalar("select `polygon_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();
			//var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Json_t, s).Where(a => a.Id == Pid).ToAffectedRows();
			//var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Json_t);
			//Assert.Equal(1, affrows);
			//Assert.Equal(i, result);
		}

		[Theory]
		[InlineData(12.33)]
		public void Real(double d)
		{
			var obj = _dbContext.ExecuteScalar("select `real_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Real_t, d).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Real_t);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData(0x10)]
		public void Smallint(short d)
		{
			var obj = _dbContext.ExecuteScalar("select `smallint_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Smallint_t, d).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Smallint_t);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData(12.33)]
		public void Numeric(decimal d)
		{
			var obj = _dbContext.ExecuteScalar("select `numeric_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Numeric_t, d).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Numeric_t);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData("abcd")]
		public void Text(string s)
		{
			var obj = _dbContext.ExecuteScalar("select `text_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Text_t, s).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Text_t);
			Assert.Equal(1, affrows);
			Assert.Equal(s, result);
		}

		[Theory]
		[InlineData("abcd")]
		public void TinyText(string s)
		{
			var obj = _dbContext.ExecuteScalar("select `tinytext_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Tinytext_t, s).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Tinytext_t);
			Assert.Equal(1, affrows);
			Assert.Equal(s, result);
		}

		[Theory]
		[InlineData("abcd")]
		public void Varchar(string s)
		{
			var obj = _dbContext.ExecuteScalar("select `varchar_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Varchar_t, s).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Varchar_t);
			Assert.Equal(1, affrows);
			Assert.Equal(s, result);
		}

		[Theory]
		[InlineData(2021)]
		public void Year(short s)
		{
			var obj = _dbContext.ExecuteScalar("select `year_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Year_t, s).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Year_t);
			Assert.Equal(1, affrows);
			Assert.Equal(s, result);
		}

		[Fact]
		public void Time()
		{
			var obj = _dbContext.ExecuteScalar("select `time_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var s = TimeSpan.FromSeconds(20);
			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Time_t, s).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Time_t);
			Assert.Equal(1, affrows);
			Assert.Equal(s, result);
		}

		[Theory]
		[InlineData("2021-1-1 20:01:23")]
		public void Timestamp(DateTime dt)
		{
			var obj = _dbContext.ExecuteScalar("select `timestamp_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Timestamp_t, dt).Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Timestamp_t);
			Assert.Equal(1, affrows);
			Assert.Equal(dt, result);
		}

		[Fact]
		public void Set()
		{
			var obj = _dbContext.ExecuteScalar("select `set_t` from `type_test` where `id` = 1 ");
			var type = obj.GetType();

			var affrows = _dbContext.Update<TypeTestModel>().Set(a => a.Set_t, "2,3").Where(a => a.Id == Pid).ToAffectedRows();
			var result = _dbContext.Select<TypeTestModel>().Where(a => a.Id == Pid).FirstOrDefault(a => a.Set_t);
			Assert.Equal(1, affrows);
		}
	}
}
