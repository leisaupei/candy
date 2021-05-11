﻿/* ##########################################################
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

			ServiceProvider serviceProvider = BuildServiceProvider();

			var generatorFactory = serviceProvider.GetService<ICreeperGeneratorProviderFactory>();
			var creeperGenerator = serviceProvider.GetService<ICreeperGenerator>();
			var creeperDbContext = serviceProvider.GetService<ICreeperDbContext>();
			if (args?.Length > 0)
			{
				CreeperGenerateOption model = new CreeperGenerateOption();
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
								var type = ps.FirstOrDefault(a => a.Contains("type="));
								if (type == null)
									throw new ArgumentException("choose one of ", string.Join(",", Enum.GetNames<DataBaseKind>()));
								var kindStr = type.Split('=')[1];

								var kind = Enum.TryParse<DataBaseKind>(kindStr, ignoreCase: true, out var value)
									? value : throw new ArgumentException("choose one of ", string.Join(",", Enum.GetNames<DataBaseKind>()));

								model.Connections.Add(generatorFactory[kind].GetDbConnectionOptionFromString(build));
							}
							finish = true;
							break;
						default:
							break;
					}
					if (finish) break;
				}
				creeperGenerator.Gen(model);
				Console.WriteLine("successful...");
			}
			Console.ReadKey();
		}

		private static ServiceProvider BuildServiceProvider()
		{
			IConfiguration cfg = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, false)
				.AddJsonFile("appsettings.postgresql.json", true, false)
				.Build();

			IServiceCollection services = new ServiceCollection();

			services.AddSingleton(cfg);

			//postgresql 
			var postgreSqlRules = cfg.GetSection("GenerateRules:PostgreSqlRules").Get<PostgreSqlRules>();
			services.AddCreeperGenerator(option =>
			{
				option.UsePostgreSqlRules(o =>
				{
					o.Excepts = postgreSqlRules.Excepts;
					o.FieldIgnore = postgreSqlRules.FieldIgnore;
				});
			});
			var serviceProvider = services.BuildServiceProvider();
			return serviceProvider;
		}
	}
}
