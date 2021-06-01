using System;
using Creeper.Driver;
using Creeper.PostgreSql;
using Creeper.PostgreSql.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperPostgreSqlExtensions
	{
		/// <summary>
		/// 添加PostgreSql数据库配置
		/// </summary>
		/// <param name="option"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void AddPostgreSqlDbContext<TDbContext>(this CreeperOptions option, Action<CreeperDbContextOptions> action) where TDbContext : class, ICreeperDbContext
		{
			option.AddPostgreSqlOption();
			option.AddDbContext<TDbContext>(action);
		}
		/// <summary>
		/// PostgreSql选项配置
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public static void AddPostgreSqlOption(this CreeperOptions option)
		{
			option.AddDbConverter<PostgreSqlConverter>();
			option.AddExtension(new CreeperPostgreSqlOptionsExtensions());
		}
	}
}
