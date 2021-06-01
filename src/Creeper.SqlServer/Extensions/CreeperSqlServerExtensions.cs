using System;
using Creeper.Driver;
using Creeper.SqlServer;
using Creeper.SqlServer.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperSqlServerExtensions
	{
		/// <summary>
		/// 添加SqlServer数据库配置
		/// </summary>
		/// <param name="option"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void AddSqlServerDbContext<TDbContext>(this CreeperOptions option, Action<CreeperDbContextOptions> action) where TDbContext : class, ICreeperDbContext
		{
			option.AddSqlServerOption();
			option.AddDbContext<TDbContext>(action);
		}

		/// <summary>
		/// SqlServer选项配置
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public static void AddSqlServerOption(this CreeperOptions option)
		{
			option.AddDbConverter<SqlServerConverter>();
			option.AddExtension(new CreeperSqlServerOptionsExtension());
		}
	}
}
