using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Data.Common;

namespace Creeper.MySql.XUnitTest.Entity.Options
{
	public class MySqlDbContext : CreeperDbContextBase
	{
		public MySqlDbContext(IServiceProvider serviceProvider) : base(serviceProvider) { }

		public override DataBaseKind DataBaseKind => DataBaseKind.MySql;

		public override string Name => nameof(MySqlDbContext);

	}
}
