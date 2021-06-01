﻿using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Xunit.Abstractions;
using Creeper.Driver;
using Creeper.Generic;
using Creeper.PostgreSql.XUnitTest.Entity.Options;
using Creeper.PostgreSql.XUnitTest.Extensions;

namespace Creeper.PostgreSql.XUnitTest
{
	public class BaseTest
	{
		public const string TestMainConnectionString = "host=192.168.1.15;port=5432;username=postgres;password=123456;database=postgres;maximum pool size=10;pooling=true;Timeout=10;CommandTimeout=10;";

		public const string TestSecondaryConnectionString = "host=192.168.1.15;port=5432;username=postgres;password=123456;database=postgres;maximum pool size=10;pooling=true;Timeout=10;CommandTimeout=20;";
		public static readonly Guid StuPeopleId1 = Guid.Parse("da58b577-414f-4875-a890-f11881ce6341");

		public static readonly Guid StuPeopleId2 = Guid.Parse("5ef5a598-e4a1-47b3-919e-4cc1fdd97757");
		public static readonly Guid GradeId = Guid.Parse("81d58ab2-4fc6-425a-bc51-d1d73bf9f4b1");

		public static readonly string StuNo1 = "1333333";
		public static readonly string StuNo2 = "1333334";

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
					options.AddPostgreSqlDbContext<PostgreSqlDbContext>(t =>
					{
						t.UseCache<CustomDbCache>();
						t.DbTypeStrategy = DataBaseTypeStrategy.MainIfSecondaryEmpty;
						t.UseConnectionString(TestMainConnectionString, new[] { TestSecondaryConnectionString });
					});
				});
				var serviceProvider = services.BuildServiceProvider();
				DbContext = serviceProvider.GetService<ICreeperDbContext>();

				//JsonConvert.DefaultSettings = () =>
				//{
				//	var st = new JsonSerializerSettings
				//	{
				//		Formatting = Formatting.Indented,
				//	};
				//	st.Converters.Add(new StringEnumConverter());
				//	st.Converters.Add(new IPConverter());
				//	st.Converters.Add(new PhysicalAddressConverter());
				//	st.Converters.Add(new NpgsqlTsQueryConverter());
				//	st.Converters.Add(new NpgsqlTsVectorConverter());
				//	st.Converters.Add(new BitArrayConverter());
				//	st.Converters.Add(new NpgsqlPointListConverter());
				//	st.Converters.Add(new BooleanConverter());
				//	st.Converters.Add(new DateTimeConverter());

				//	st.ContractResolver = new LowercaseContractResolver();
				//	return st;
				//};
			}
		}

		public BaseTest(ITestOutputHelper output) : this()
		{
			Output = output;
		}

	}
}
