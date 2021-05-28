using Creeper.Generic;
using System;
using System.Data.Common;

namespace Creeper.Driver
{
	public class CreeperDbContextOptions
	{
		/// <summary>
		/// 数据库缓存类
		/// </summary>
		internal Type DbCacheType { get; private set; } = null;

		/// <summary>
		/// 数据库连接配置
		/// </summary>
		internal Action<DbConnection> DbConnectionOptions { get; private set; } = null;

		/// <summary>
		/// 从库
		/// </summary>
		internal string[] Secondary { get; set; }

		/// <summary>
		/// 主库
		/// </summary>
		internal string Main { get; set; }

		/// <summary>
		/// 主从策略
		/// </summary>
		public DataBaseTypeStrategy DbTypeStrategy { get; set; } = DataBaseTypeStrategy.MainIfSecondaryEmpty;

		/// <summary>
		/// 添加DbCache
		/// </summary>
		/// <typeparam name="TDbCache"></typeparam>
		public void UseCache<TDbCache>() where TDbCache : ICreeperDbCache
		{
			DbCacheType = typeof(TDbCache);
		}

		/// <summary>
		/// 设置主从数据库
		/// </summary>
		/// <param name="main"></param>
		/// <param name="secondary"></param>
		public void UseConnectionString(string main, string[] secondary = null)
		{
			if (string.IsNullOrWhiteSpace(main))
			{
				throw new ArgumentException($"“{nameof(main)}”不能为 Null 或空白", nameof(main));
			}

			Main = main;
			Secondary = secondary;
		}
		/// <summary>
		/// 设置数据库链接
		/// </summary>
		/// <param name="action"></param>
		public void UseDbConnectionOptions(Action<DbConnection> action)
		{
			DbConnectionOptions = action;
		}
	}
}
