/* ##########################################################
 * #        .net standard 2.1 + data base Code Maker        #
 * #                author by leisaupei                     #
 * #          https://github.com/leisaupei/creeper          #
 * ##########################################################
 */
using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Options;
using Creeper.Generic;
using Creeper.MySql.Generator;
using Creeper.PostgreSql;
using Creeper.PostgreSql.Generator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
namespace Creeper.Generator
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.WriteLine(@"
##########################################################
#        .net standard 2.1 + data base Code Maker        #
#                author by leisaupei                     #
#           https://github.com/leisaupei/creeper         #
##########################################################
> Parameters description:
	-o output path
	-p project name
	-s create .sln file, *optional(t/f) default: f.
	--b build options, arguments must be at the end
		host	database host
		port	database port
		user	database username
		pwd		database password
		db		database name
		name	database enum type name, 'main' if only one. *optional
		type	postgresql/mysql at presents
> Single Example: -o d:\workspace\test -p SimpleTest -s t --b host=localhost;port=5432;user=postgres;pwd=123456;db=postgres;name=main;type=postgresql

> Multiple Example: -o d:\workspace\test -p SimpleTest -s t --b host=localhost;port=5432;user=postgres;pwd=123456;db=postgres;name=main;type=postgresql host=localhost;port=5432;user=postgres;pwd=123456;db=postgres;name=main;type=postgresql
");

			if (!args?.Any() ?? true)
			{
				Console.WriteLine("please type in parameters.");
				Console.ReadKey();
				return;
			}

			var serviceProvider = BuildServiceProvider();

			var generatorFactory = serviceProvider.GetService<ICreeperGeneratorProviderFactory>();
			var creeperGenerator = serviceProvider.GetService<ICreeperGenerator>();
			var creeperDbContext = serviceProvider.GetService<ICreeperDbContext>();

			var model = new CreeperGenerateOption();
			for (int i = 0; i < args.Length; i += 2)
			{
				//host=localhost;port=5432;user=postgres;pwd=123456;db=postgres;type=name;type=postgresql
				var finish = false;
				switch (args[i].ToLower())
				{
					case "-o": model.OutputPath = args[i + 1]; break;
					case "-p": model.ProjectName = args[i + 1]; break;
					case "-s": model.Sln = args[i + 1].ToLower() == "t"; break;
					case "--b":
						var builds = args[(i + 1)..];
						foreach (var build in builds)
						{
							var ps = build.Split(';');
							var type = ps.FirstOrDefault(a => a.StartsWith("type="));

							if (type == null || !Enum.TryParse<DataBaseKind>(type.Split('=')[1], true, out var kind))
								throw new ArgumentException("choose one of ", string.Join(",", Enum.GetNames<DataBaseKind>()));

							model.Builders.Add(generatorFactory[kind].GetDbConnectionOptionFromString(build));
						}
						finish = true;
						break;
				}
				if (finish) break;
			}

			if (model.Builders.Count > 1 && model.Builders.Any(a => string.IsNullOrWhiteSpace(a.Name)))
				throw new ArgumentNullException(nameof(CreeperGenerateConnection.Name));

			if (model.Builders.GroupBy(a => a.Name.ToLower()).Any(a => a.Count() > 1))
				throw new ArgumentException($"包含{nameof(CreeperGenerateConnection.Name)}相同的参数");

			creeperGenerator.Gen(model);
			Console.WriteLine("successful...");
			//Console.ReadKey();
		}

		private static ServiceProvider BuildServiceProvider()
		{
			IConfiguration cfg = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, false)
				.AddJsonFile("appsettings.postgresql.json", true, false)
				.AddJsonFile("appsettings.mysql.json", true, false)
				.Build();

			IServiceCollection services = new ServiceCollection();

			services.AddSingleton(cfg);

			//postgresql 
			var postgreSqlRules = cfg.GetSection("GenerateRules:PostgreSqlRules").Get<PostgreSqlGeneratorRules>();
			//mysql
			var mySqlRules = cfg.GetSection("GenerateRules:MySqlRules").Get<MySqlGeneratorRules>();
			services.AddCreeperGenerator(option =>
			{
				option.UseMySqlRules(o => Copy(mySqlRules, o));

				option.UsePostgreSqlRules(o => Copy(postgreSqlRules, o));
			});
			var serviceProvider = services.BuildServiceProvider();
			return serviceProvider;
		}

		private static TTo Copy<TFrom, TTo>(TFrom fromValue) where TTo : new()
		{
			var toValue = Activator.CreateInstance<TTo>();
			Copy(fromValue, toValue);
			return toValue;
		}
		private static void Copy<TFrom, TTo>(TFrom fromValue, TTo toValue) where TTo : new()
		{
			if (fromValue is null)
			{
				throw new ArgumentNullException(nameof(fromValue));
			}

			var toProperties = typeof(TTo).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var fromType = typeof(TFrom);

			foreach (var toProperty in toProperties)
			{
				var fromProperty = fromType.GetProperty(toProperty.Name, BindingFlags.Public | BindingFlags.Instance);
				if (fromProperty.PropertyType == toProperty.PropertyType)
					fromProperty.SetValue(toValue, fromProperty.GetValue(fromValue));
			}
		}
		private static Action<TTo> Copy<TFrom, TTo>(Action<TFrom> from) where TTo : new()
		{
			if (from is null)
			{
				throw new ArgumentNullException(nameof(from));
			}

			var fromValue = Activator.CreateInstance<TFrom>();
			from.Invoke(fromValue);

			return new Action<TTo>(toValue => Copy(fromValue, toValue));
		}
	}
}
