using System;
using System.Data.Common;
using System.Data.SQLite;
using Xunit;
using Creeper.Driver;
using Creeper.Sqlite.XUnitTest.Entity.Model;

namespace Creeper.Sqlite.XUnitTest
{
	public class CLRTest : BaseTest
	{
		const long Pid = 1;

		[Fact]
		public void InsertModel()
		{
			DbContext.Insert(new ProductModel
			{
				Img = new byte[] { 12, 21, 1 },
				Price = 12.32,
				Name = "¿Ó–ﬁ∆§",
				Stock = 123,
				Id = new Random().Next(10000),
				Category_id = 1
			});
		}

		[Theory]
		[InlineData(9223372036234775807)]
		public void Integer(long p)
		{
			var affrows = DbContext.Update<ProductModel>().Set(a => a.Stock, p).Where(a => a.Id == Pid).ToAffectedRows();
			var result = DbContext.Select<ProductModel>(a => a.Id == Pid).FirstOrDefault(a => a.Stock);
			Assert.Equal(1, affrows);
			Assert.Equal(p, result);
		}

		[Theory]
		[InlineData("≤‚ ‘")]
		public void Text(string c)
		{
			var affrows = DbContext.Update<ProductModel>().Set(a => a.Name, c).Where(a => a.Id == Pid).ToAffectedRows();
			var result = DbContext.Select<ProductModel>(a => a.Id == Pid).FirstOrDefault(a => a.Name);
			Assert.Equal(1, affrows);
			Assert.Equal(c, result);
		}

		[Theory]
		[InlineData(12345.21)]
		public void Real(double d)
		{
			var affrows = DbContext.Update<ProductModel>().Set(a => a.Price, d).Where(a => a.Id == Pid).ToAffectedRows();
			var result = DbContext.Select<ProductModel>(a => a.Id == Pid).FirstOrDefault(a => a.Price);
			Assert.Equal(1, affrows);
			Assert.Equal(d, result);
		}

		[Theory]
		[InlineData(new byte[] { 123, 23 })]
		public void Blob(byte[] b)
		{
			var affrows = DbContext.Update<ProductModel>().Set(a => a.Img, b).Where(a => a.Id == Pid).ToAffectedRows();
			var result = DbContext.Select<ProductModel>(a => a.Id == Pid).FirstOrDefault(a => a.Img);
			Assert.Equal(1, affrows);
			Assert.Equal(b.Length, result.Length);
		}
	}
}
