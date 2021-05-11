using Creeper.Generator.Common.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.MySql.Generator
{
	public class MySqlGeneratorExtension : ICreeperGeneratorExtension
	{
		private readonly Action<MySqlRules> _mySqlRulesAction;

		public MySqlGeneratorExtension(Action<MySqlRules> mySqlRulesAction)
		{
			_mySqlRulesAction = mySqlRulesAction;
		}

		public void RegisterExtension(IServiceCollection services)
		{

			services.Configure(_mySqlRulesAction);
			services.AddSingleton<ICreeperGeneratorProvider, MySqlGeneratorProvider>();

			services.AddCreeperDbContext(option =>
			{
				option.AddMySqlDbOption();
			});
		}

	}

}
