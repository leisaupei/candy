using System;
using System.Data.Common;
using System.Data.SQLite;
using Xunit;
using Creeper.Driver;
using Creeper.Sqlite.XUnitTest.Entity.Model;

namespace Creeper.Sqlite.XUnitTest
{
	public class SelectTest : BaseTest
	{
		const long Pid = 1;
		[Fact]
		public void Select()
		{
			var categoryName = DbContext.Select<ProductModel>(a => a.Id == Pid)
				.InnerJoin<CategoryModel>((a, b) => a.Category_id.Value == b.Id)
				.FirstOrDefault<CategoryModel, string>(b => b.Name);
		}
		[Fact]
		public void Transaction()
		{
			DbContext.Transaction(execute =>
			{
				execute.Insert(new ProductModel
				{
					Img = new byte[] { 12, 21, 1 },
					Price = 12.32,
					Name = "¿Ó–ﬁ∆§",
					Stock = 123,
					Id = new Random().Next(10000),
					Category_id = 1
				});
			});
		}
	}
}
