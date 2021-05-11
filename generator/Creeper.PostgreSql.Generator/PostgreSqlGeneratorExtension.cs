using Creeper.Generator.Common.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.PostgreSql.Generator
{
	public class PostgreSqlGeneratorExtension : ICreeperGeneratorExtension
	{
		private readonly Action<PostgreSqlRules> _postgreSqlRulesAction;

		public PostgreSqlGeneratorExtension(Action<PostgreSqlRules> postgreSqlRulesAction)
		{
			_postgreSqlRulesAction = postgreSqlRulesAction;
		}

		public void RegisterExtension(IServiceCollection services)
		{

			services.Configure(_postgreSqlRulesAction);
			services.AddSingleton<ICreeperGeneratorProvider, PostgreSqlGeneratorProvider>();

			services.AddCreeperDbContext(option =>
			{
				option.AddPostgreSqlDbOption();
			});
		}

	}

}
