using System;
using Creeper.Access;
using Creeper.Access.Extensions;
using Creeper.Driver;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperAccessExtensions
	{
		/// <summary>
		/// 添加Access数据库配置
		/// </summary>
		/// <param name="option"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void AddAccessDbContext<TDbContext>(this CreeperOptions option, Action<CreeperDbContextOptions> action) where TDbContext : class, ICreeperDbContext
		{
			option.AddAccessOption();
			option.AddDbContext<TDbContext>(action);
		}

		/// <summary>
		/// Access选项配置
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public static void AddAccessOption(this CreeperOptions option)
		{
			option.AddDbConverter<AccessConverter>();
			option.AddExtension(new CreeperAccessOptionsExtension());
		}
	}
}
