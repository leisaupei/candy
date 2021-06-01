using Creeper.DbHelper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Driver
{
	public class CreeperOptions
	{
		/// <summary>
		/// 子项扩展
		/// </summary>
		internal IList<ICreeperOptionsExtension> Extensions { get; } = new List<ICreeperOptionsExtension>();

		/// <summary>
		/// 注册dbcontext
		/// </summary>
		/// <typeparam name="TDbContext"></typeparam>
		/// <param name="action"></param>
		public void AddDbContext<TDbContext>(Action<CreeperDbContextOptions> action) where TDbContext : class, ICreeperDbContext
			=> AddExtension(new DbContextExtension<TDbContext>(action));

		/// <summary>
		/// 注册扩展服务
		/// </summary>
		/// <param name="extension"></param>
		public void AddExtension(ICreeperOptionsExtension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException(nameof(extension));
			}

			Extensions.Add(extension);
		}

		/// <summary>
		/// 添加db类型转换器
		/// </summary>
		/// <typeparam name="TDbConverter"></typeparam>
		public void AddDbConverter<TDbConverter>() where TDbConverter : ICreeperDbConverter, new()
		{
			var convert = Activator.CreateInstance<TDbConverter>();
			if (!TypeHelper.DbTypeConverters.ContainsKey(convert.DataBaseKind))
				TypeHelper.DbTypeConverters[convert.DataBaseKind] = convert;
		}


		public class DbContextExtension<TDbContext> : ICreeperOptionsExtension where TDbContext : class, ICreeperDbContext
		{
			private readonly Action<CreeperDbContextOptions> _dbContextConfigure;

			public DbContextExtension(Action<CreeperDbContextOptions> dbContextConfigure)
			{
				_dbContextConfigure = dbContextConfigure;
			}

			public void AddServices(IServiceCollection services)
			{
				var options = new CreeperDbContextOptions();
				_dbContextConfigure?.Invoke(options);

				//添加数据库缓存DbCache单例与集合
				if (options.DbCacheType != null)
				{
					services.TryAddSingleton(options.DbCacheType);
					services.AddSingleton(typeof(ICreeperDbCache), options.DbCacheType);
				}

				//添加DbContext配置与单例
				var dbContextType = typeof(TDbContext);
				services.Configure(dbContextType.Name, _dbContextConfigure);
				services.TryAddSingleton(dbContextType);
				services.AddSingleton(typeof(ICreeperDbContext), dbContextType);
			}
		}
	}
}
