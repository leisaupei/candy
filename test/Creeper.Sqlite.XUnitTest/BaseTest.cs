using Creeper.Driver;
using Creeper.Sqlite.XUnitTest.Entity.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Creeper.Sqlite.XUnitTest
{
	public class BaseTest
	{

		public const string MainConnectionString = "data source=../../../../../sql/demo.db";

		static bool _isInit;
		protected ITestOutputHelper Output { get; }

		protected static ICreeperDbContext DbContext { get; set; }

		public BaseTest()
		{
			if (!_isInit)
			{
				_isInit = true;
				var services = new ServiceCollection();
				services.AddCreeper(options =>
				{
					options.AddSqliteDbContext<SqliteDbContext>(a =>
					{
						a.DbTypeStrategy = Generic.DataBaseTypeStrategy.OnlyMain;
						a.UseConnectionString(MainConnectionString);
					});
				});
				var serviceProvider = services.BuildServiceProvider();
				DbContext = serviceProvider.GetService<ICreeperDbContext>();

			}
		}

		public BaseTest(ITestOutputHelper output) : this()
		{
			Output = output;
		}
	}

}
