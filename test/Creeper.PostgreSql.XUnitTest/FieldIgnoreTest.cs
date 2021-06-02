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
	public class FieldIgnoreTest : BaseTest
	{
		[Fact]
		public void Insert()
		{
			Assert.ThrowsAny<CreeperException>(() =>
			{
				var info = new ClassGradeModel
				{
					Create_time = DateTime.Now,
					Name = "Õ¯“≥…Ëº∆",
					Id = Guid.Parse("4890d2a6-0185-43de-8d7d-06306d6e33e4")
				};
				var result = DbContext.Insert(info);
			});
		}
		[Fact]
		public void Returning()
		{
			var info = DbContext.Select<ClassGradeModel>().Where(a => a.Id == Guid.Parse("81d58ab2-4fc6-425a-bc51-d1d73bf9f4b1")).FirstOrDefault();
			Assert.Equal(info?.Id, Guid.Empty);

		}
	}
}
