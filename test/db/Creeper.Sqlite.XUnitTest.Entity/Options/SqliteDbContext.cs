using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generic;
using System;

namespace Creeper.Sqlite.XUnitTest.Entity.Options
{
	public class SqliteDbContext : CreeperDbContextBase
	{
		public SqliteDbContext(IServiceProvider serviceProvider) : base(serviceProvider) { }

		public override DataBaseKind DataBaseKind => DataBaseKind.Sqlite;

		public override string Name => nameof(SqliteDbContext);
	}
}
