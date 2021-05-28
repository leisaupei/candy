using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using Creeper.MySql.Types;
using Creeper.MySql.XUnitTest.Entity.Model;
using Creeper.MySql.XUnitTest.Entity.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;
using TestSpace = MySql.Data.Types;
namespace Creeper.MySql.XUnitTest
{
	public class Class1
	{

		[Fact]
		public void Init()
		{
			IServiceCollection services = new ServiceCollection();

			//services.AddSingleton<TestDbContext>();
			//services.AddSingleton<ICreeperDbContext, TestDbContext>();

			services.TryAddSingleton<Test1>();
			services.TryAddSingleton<Test2>();
			services.AddSingleton<ITest, Test1>();
			services.AddSingleton<ITest, Test2>();


			var provider = services.BuildServiceProvider();
			var tes = provider.GetService<Test1>();
			var tes1 = provider.GetService<Test2>();
			var tess = provider.GetService<IEnumerable<ITest>>();

			var dbContext = provider.GetService<MySqlDbContext>();
		}

		public interface ITest
		{

		}
		public class Test1 : ITest
		{
			public Guid Id { get; set; } = Guid.NewGuid();
		}
		public class Test2 : ITest
		{
			public Guid Id { get; set; } = Guid.NewGuid();

		}
	}
}
