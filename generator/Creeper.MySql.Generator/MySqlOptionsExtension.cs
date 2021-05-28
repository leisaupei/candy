using Creeper.Generator.Common.Options;
using Creeper.MySql.Generator;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class MySqlOptionsExtension
	{
		public static CreeperGeneratorOptions UseMySqlRules(this CreeperGeneratorOptions options, Action<MySqlGeneratorRules> action)
		{
			options.AddExtension(new MySqlGeneratorExtension(action));
			return options;
		}
	}
}
