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
using Creeper.DbHelper;
using Creeper.Driver;

namespace Creeper.PostgreSql.XUnitTest
{
	[Order(5)]
	public class Delete : BaseTest
	{
		[Fact]
		public void DeleteData()
		{
			var affrows = DbContext.Delete<PeopleModel>().Where(a => a.Id == Guid.Parse("3058b8a2-2e59-42df-908e-f003c3256a9b")).ToAffectedRows();
		}
	}
}
