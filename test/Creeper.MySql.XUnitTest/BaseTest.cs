using Creeper.Driver;
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

		public static bool IsInit;
		protected readonly ITestOutputHelper _output;
		protected static ICreeperDbContext _dbContext;

		public BaseTest()
		{
			if (!IsInit)
			{
				IsInit = true;
				var services = new ServiceCollection();
				services.AddCreeperDbContext(options =>
				{
					options.AddMySqlDbOption();
					options.AddDbOption(new TestDbOption());
				});
				var serviceProvider = services.BuildServiceProvider();
				_dbContext = serviceProvider.GetService<ICreeperDbContext>();

			}
		}

		public BaseTest(ITestOutputHelper output) : this()
		{
			_output = output;
		}
		public class TestDbOption : ICreeperDbOption
		{
			public ICreeperDbConnectionOption Main => new MySqlConnectionOption(TestMainConnectionString, "DbMain");

			public ICreeperDbConnectionOption[] Secondary => Array.Empty<MySqlConnectionOption>();
		}
	}

}
