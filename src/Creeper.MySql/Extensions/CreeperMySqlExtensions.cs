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
		/// <param name="option"></param>
		/// <param name="action"></param>
		/// <remarks>
		/// 1. 使用Mysql数据库连接字符串时, 默认添加Allow User Variables=True参数, 以使用Update Returning
		/// </remarks>
		/// <returns></returns>
		public static void AddMySqlDbContext<TDbContext>(this CreeperOptions option, Action<CreeperDbContextOptions> action) where TDbContext : class, ICreeperDbContext
		{
			option.AddMySqlOption();
			option.AddDbContext<TDbContext>(action);
		}

		/// <summary>
		/// MySql选项配置
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public static void AddMySqlOption(this CreeperOptions option)
		{
			option.AddDbConverter<MySqlConverter>();
			option.AddExtension(new CreeperMySqlOptionsExtension());
		}

		/// <summary>
		/// 因Mysql空间数据为自定义类型, 所以放置控制开关, 默认是false
		/// </summary>
		/// <param name="_"></param>
		public static void UseMySqlGeometry(this CreeperDbContextOptions _)
		{
			MySqlConverter.UseGeometryType = true;
		}
	}
}
