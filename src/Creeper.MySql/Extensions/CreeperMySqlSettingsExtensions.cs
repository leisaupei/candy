using System;
using Creeper.Driver;
using Creeper.MySql;
using Creeper.MySql.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperMySqlSettingsExtensions
	{
		/// <summary>
		/// 添加MySql数据库配置
		/// </summary>
		/// <param name="setting"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void AddMySqlDbContext<TDbContext>(this CreeperOptions setting, Action<CreeperDbContextOptions> action) where TDbContext : class, ICreeperDbContext
		{
			setting.AddMySqlOption();
			setting.AddDbContext<TDbContext>(action);
		}

		/// <summary>
		/// MySql选项配置
		/// </summary>
		/// <param name="setting"></param>
		/// <returns></returns>
		public static void AddMySqlOption(this CreeperOptions setting)
		{
			setting.AddDbConverter<MySqlConverter>();
			setting.AddExtension(new CreeperMySqlOptionsExtension());
		}
	}
}
