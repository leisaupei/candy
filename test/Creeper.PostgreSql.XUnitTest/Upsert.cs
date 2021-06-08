using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions.Ordering;
using System.Data.Common;
using Npgsql;
using Creeper.SqlBuilder;
using Creeper.PostgreSql.XUnitTest.Entity.Model;
using System.Reflection;
using Creeper.Extensions;
using Creeper.Driver;

namespace Creeper.PostgreSql.XUnitTest
{
	public class Upsert : BaseTest
	{
		[Theory, Description("自增主键")]
		[InlineData((int)default)]
		[InlineData(2)]
		public void IdentityPrimaryKey(int id)
		{
			var affrows = DbContext.Upsert(new TestIdenPkModel
			{
				Id = id,
				Age = 20,
				Name = "小云"
			});
		}

		[Theory, Description("随机唯一主键")]
		[InlineData("00000000-0000-0000-0000-000000000000")]
		[InlineData("7707d5e2-0ed0-4f80-931a-1d766028df08")]
		public void GuidPrimaryKey(Guid id)
		{
			var affrows = DbContext.UpsertOnly(new TestUuidPkModel
			{
				Id = id,
				Age = 20,
				Name = "小明"
			});
		}

		[Theory, Description("Guid主键, 包含一个自增字段")]
		[InlineData("00000000-0000-0000-0000-000000000000")]
		[InlineData("7707d5e2-0ed0-4f80-931a-1d766028df08")]
		public void GuidPrimaryKeyWithIdentityField(Guid id)
		{
			var affrows = DbContext.UpsertOnly(new TestIdenNopkModel
			{
				Id = id,
				Age = 20,
				Name = "小云"
			});
		}

		[Theory, Description("复合主键, 包含一个自增主键和随机唯一主键")]
		[InlineData("00000000-0000-0000-0000-000000000000", 0)]
		[InlineData("7707d5e2-0ed0-4f80-931a-1d766028df08", 1)]
		[InlineData("00000000-0000-0000-0000-000000000000", 2)]
		public void GuidIdentityPrimaryKey(Guid id, int idenId)
		{
			var affrows = DbContext.Upsert(new TestUuidIdenPkModel
			{
				Id = id,
				Id_sec = idenId,
				Age = 20,
				Name = "小明"
			});
		}
	}
}
