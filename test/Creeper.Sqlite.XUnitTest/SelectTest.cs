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
		public void Insert()
		{
			var categoryName = DbContext.Select<ProductModel>(a => a.Id == Pid)
				.InnerJoin<CategoryModel>((a, b) => a.Category_id.Value == b.Id)
				.FirstOrDefault<CategoryModel, string>(b => b.Name);
		}
	}
}
