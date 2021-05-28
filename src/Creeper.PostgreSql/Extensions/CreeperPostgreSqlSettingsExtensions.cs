using System;
using Creeper.Driver;
using Creeper.PostgreSql;
using Creeper.PostgreSql.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperPostgreSqlSettingsExtensions
	{
		/// <summary>
		/// 添加PostgreSql数据库配置
		/// </summary>
		/// <param name="settings"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void AddPostgreSqlDbContext<TDbContext>(this CreeperOptions settings, Action<CreeperDbContextOptions> action) where TDbContext : class, ICreeperDbContext
		{
			settings.AddPostgreSqlOption();
			settings.AddDbContext<TDbContext>(action);
		}
		/// <summary>
		/// PostgreSql选项配置
		/// </summary>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static void AddPostgreSqlOption(this CreeperOptions settings)
		{
			settings.AddDbConverter<PostgreSqlConverter>();
			settings.AddExtension(new CreeperPostgreSqlOptionsExtensions());
		}
	}
}
