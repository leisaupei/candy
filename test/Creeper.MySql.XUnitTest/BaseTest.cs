using Creeper.Driver;
using Creeper.MySql.XUnitTest.Entity.Options;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Creeper.MySql.XUnitTest
{
	public class BaseTest
	{

		public const string TestMainConnectionString = "server=192.168.1.15;userid=root;pwd=123456;port=3306;database=demo;sslmode=none;";

		public const string TestSecondaryConnectionString = "server=192.168.1.15;userid=root;pwd=123456;port=3306;database=demo;sslmode=none;";

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
					options.AddMySqlDbContext<MySqlDbContext>(a =>
					{
						a.UseConnectionString(TestMainConnectionString, new[] { TestSecondaryConnectionString });
						a.UseMySqlGeometry();
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
