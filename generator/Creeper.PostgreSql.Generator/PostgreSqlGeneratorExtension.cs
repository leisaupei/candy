using Creeper.Driver;
using Creeper.Generator.Common.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.PostgreSql.Generator
{
	public class PostgreSqlGeneratorExtension : ICreeperOptionsExtension
	{
		private readonly Action<PostgreSqlGeneratorRules> _postgreSqlRulesAction;

		public PostgreSqlGeneratorExtension(Action<PostgreSqlGeneratorRules> postgreSqlRulesAction)
		{
			_postgreSqlRulesAction = postgreSqlRulesAction;
		}

		public void AddServices(IServiceCollection services)
		{

			services.Configure(_postgreSqlRulesAction);
			services.AddSingleton<ICreeperGeneratorProvider, PostgreSqlGeneratorProvider>();

			services.AddCreeper(option => option.AddPostgreSqlOption());
		}

	}

}
