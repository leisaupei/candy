﻿using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Creeper.Generic
{
	public class CreeperOptions
	{
		/// <summary>
		/// 数据库连接集合
		/// </summary>
		internal IList<ICreeperDbOption> DbOptions { get; } = new List<ICreeperDbOption>();

		/// <summary>
		/// 子项扩展
		/// </summary>
		internal IList<ICreeperOptionsExtension> Extensions { get; } = new List<ICreeperOptionsExtension>();

		/// <summary>
		/// 数据库缓存类
		/// </summary>
		internal Type DbCacheType { get; private set; } = null;

		/// <summary>
		/// 主从策略
		/// </summary>
		public DataBaseTypeStrategy DbTypeStrategy { get; set; } = DataBaseTypeStrategy.SecondaryFirstOfMainIfEmpty;

		/// <summary>
		/// 默认使用的数据库配置struct
		/// </summary>
		public Type DefaultDbOptionName { get; set; }

		/// <summary>
		/// 添加数据库配置
		/// </summary>
		/// <param name="dbOption"></param>
		public void AddDbOption(ICreeperDbOption dbOption)
			=> DbOptions.Add(dbOption);

		/// <summary>
		/// 添加db类型转换器
		/// </summary>
		/// <typeparam name="TDbConverter"></typeparam>
		public void TryAddDbConverter<TDbConverter>() where TDbConverter : ICreeperDbConverter, new()
		{
			var convert = Activator.CreateInstance<TDbConverter>();
			if (!TypeHelper.DbTypeConverters.ContainsKey(convert.DataBaseKind))
				TypeHelper.DbTypeConverters[convert.DataBaseKind] = convert;
		}

		/// <summary>
		/// 添加DbCache
		/// </summary>
		/// <typeparam name="TDbCache"></typeparam>
		public void UseCache<TDbCache>() where TDbCache : ICreeperDbCache
		{
			DbCacheType = typeof(TDbCache);
		}

		/// <summary>
		/// 注册扩展服务
		/// </summary>
		/// <param name="extension"></param>
		public void RegisterExtension(ICreeperOptionsExtension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException(nameof(extension));
			}

			Extensions.Add(extension);
		}
	}

}
