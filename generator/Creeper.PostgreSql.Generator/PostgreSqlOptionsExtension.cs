﻿using Creeper.Generator.Common.Options;
using System;

namespace Creeper.PostgreSql.Generator
{
	public static class PostgreSqlOptionsExtension
	{

		public static CreeperGeneratorOptions UsePostgreSqlRules(this CreeperGeneratorOptions options, Action<PostgreSqlRules> action)
		{

			options.RegisterExtension(new PostgreSqlGeneratorExtension(action));

			return options;
		}
	}

}
