using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common
{
	public interface ICreeperGeneratorExtension
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="services"></param>
		public void RegisterExtension(IServiceCollection services);

	}
}
