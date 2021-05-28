using System;
using Creeper.Driver;
using Creeper.MySql;
using Creeper.MySql.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperMySqlExtensions
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
		/// <summary>
		/// 因Mysql空间数据为自定义类型, 所以放置控制开关
		/// </summary>
		/// <param name="_"></param>
		public static void UseMySqlGeometry(this CreeperDbContextOptions _)
		{
			MySqlConverter.UseGeometryType = true;
		}
	}
}
