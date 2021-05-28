using Creeper.Generator.Common.Options;
using Creeper.PostgreSql.Generator;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class PostgreSqlOptionsExtension
	{

		public static CreeperGeneratorOptions UsePostgreSqlRules(this CreeperGeneratorOptions options, Action<PostgreSqlGeneratorRules> action)
		{

			options.AddExtension(new PostgreSqlGeneratorExtension(action));

			return options;
		}
	}

}
