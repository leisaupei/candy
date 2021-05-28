using Creeper.Driver;
using Creeper.Generator.Common.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.MySql.Generator
{
	public class MySqlGeneratorExtension : ICreeperOptionsExtension
	{
		private readonly Action<MySqlGeneratorRules> _mySqlRulesAction;

		public MySqlGeneratorExtension(Action<MySqlGeneratorRules> mySqlRulesAction)
		{
			_mySqlRulesAction = mySqlRulesAction;
		}

		public void AddServices(IServiceCollection services)
		{

			services.Configure(_mySqlRulesAction);
			services.AddSingleton<ICreeperGeneratorProvider, MySqlGeneratorProvider>();

			services.AddCreeper(a => a.AddMySqlOption());
		}

	}

}
