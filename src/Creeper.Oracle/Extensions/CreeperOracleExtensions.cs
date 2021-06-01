using System;
using Creeper.Driver;
using Creeper.Oracle.Extensions;
using Creeper.Oracle;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperOracleExtensions
	{
		/// <summary>
		/// 添加sqlite数据库配置
		/// </summary>
		/// <param name="option"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		public static void AddOracleDbContext<TDbContext>(this CreeperOptions option, Action<CreeperDbContextOptions> action) where TDbContext : class, ICreeperDbContext
		{
			option.AddSqliteOption();
			option.AddDbContext<TDbContext>(action);
		}

		/// <summary>
		/// sqlite选项配置
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public static void AddSqliteOption(this CreeperOptions option)
		{
			option.AddDbConverter<OracleConverter>();
			option.AddExtension(new CreeperOracleOptionsExtension());
		}
	}
}
