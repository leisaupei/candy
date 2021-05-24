using System;
using Creeper.Driver;
using Creeper.Generic;
using Creeper.MySql;
using Creeper.MySql.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CreeperOptionsExtensions
	{
		/// <summary>
		/// 添加MySql数据库配置
		/// </summary>
		/// <typeparam name="TDbMainName">主库名称泛型</typeparam>
		/// <typeparam name="TDbSecondaryName">从库名称泛型</typeparam>
		/// <param name="options"></param>
		/// <param name="mySqlDbOption">postgresql连接配置, db层目录下/Options/MySqlDbOptions.cs</param>
		/// <returns></returns>
		public static CreeperOptions AddMySql<TDbMainName, TDbSecondaryName>(this CreeperOptions options, BaseMySqlDbOption<TDbMainName, TDbSecondaryName> mySqlDbOption)
			where TDbMainName : struct, ICreeperDbName
			where TDbSecondaryName : struct, ICreeperDbName
		{
			options.AddMySqlDbOption();
			options.AddDbOption(mySqlDbOption);
			return options;
		}

		/// <summary>
		/// MySql选项配置
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public static CreeperOptions AddMySqlDbOption(this CreeperOptions options)
		{
			options.TryAddDbConverter<MySqlConverter>();
			options.RegisterExtension(new MySqlCreeperOptionsExtension());
			return options;
		}
	}
}
