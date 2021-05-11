using Microsoft.Extensions.DependencyInjection;

namespace Creeper.Generator.Common.Contracts
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
