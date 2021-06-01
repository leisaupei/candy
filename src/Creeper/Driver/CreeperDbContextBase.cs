﻿using Creeper.SqlBuilder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Creeper.Driver;
using Creeper.Generic;
using System.Collections.ObjectModel;

namespace Creeper.Driver
{
	/// <summary>
	/// 
	/// </summary>
	public abstract class CreeperDbContextBase : ICreeperDbContext
	{
		/// <summary>
		/// 获取Configure配置的Name
		/// </summary>
		public abstract string Name { get; }

		/// <summary>
		/// 数据库种类
		/// </summary>
		public abstract DataBaseKind DataBaseKind { get; }

		/// <summary>
		/// 数据库连接配置
		/// </summary>
		protected virtual Action<DbConnection> DbConnectionOptions { get; }

		/// <summary>
		/// 数据库策略
		/// </summary>
		public DataBaseTypeStrategy DbTypeStrategy { get; }

		/// <summary>
		/// 数据库缓存
		/// </summary>
		public ICreeperDbCache DbCache { get; protected set; }

		/// <summary>
		/// 数据库配置
		/// </summary>
		public ICreeperDbConnectionOption DbOption { get; }

		//public CreeperDbContext(CreeperDbContextOptions creeperOptions)
		//{
		//	DbTypeStrategy = creeperOptions.DbTypeStrategy;
		//	var main = Build(creeperOptions.Main);
		//	var secondary = creeperOptions.Secondary?.Select(a => Build(a)).ToArray();
		//	DbOption = new CreeperDbConnectionOption(main, secondary);

		//	CreeperDbConnection Build(string connectionString)
		//	{
		//		return new CreeperDbConnection(connectionString, DataBaseKind) { DbConnectionOptions = this.DbConnectionOptions + creeperOptions.DbConnectionOptions };
		//	}
		//}
		public CreeperDbContextBase(IServiceProvider serviceProvider)
		{
			var creeperOptions = serviceProvider.GetService<IOptionsMonitor<CreeperDbContextOptions>>().Get(Name);

			if (creeperOptions.DbCacheType != null)
				DbCache = (ICreeperDbCache)serviceProvider.GetService(creeperOptions.DbCacheType);

			DbTypeStrategy = creeperOptions.DbTypeStrategy;
			var main = Build(creeperOptions.Main);
			var secondary = creeperOptions.Secondary?.Select(a => Build(a)).ToArray() ?? new CreeperDbConnection[0];
			DbOption = new CreeperDbConnectionOption(main, secondary);

			CreeperDbConnection Build(string connectionString)
			{
				return new CreeperDbConnection(connectionString, DataBaseKind) { DbConnectionOptions = DbConnectionOptions + creeperOptions.DbConnectionOptions };
			}
		}
		#region GetExecuteOption
		/// <summary>
		/// 获取连接配置
		/// </summary>
		/// <exception cref="DbConnectionOptionNotFoundException">没有找到对应名称实例</exception>
		/// <returns>对应实例</returns>
		private ICreeperDbConnection GetExecuteOption(DataBaseType dataBaseType)
		{
			ICreeperDbConnection option = GetOption(dataBaseType);
			if (option == null)
				throw new DbConnectionOptionNotFoundException(dataBaseType, DbTypeStrategy);
			return option;
		}
		private ICreeperDbConnection GetOption(DataBaseType dataBaseType)
		{
			return dataBaseType switch
			{
				DataBaseType.Default => DbTypeStrategy switch
				{
					DataBaseTypeStrategy.MainIfSecondaryEmpty => DbOption.Secondary.Any()
						? GetOption(DataBaseType.Secondary)
						: GetOption(DataBaseType.Main),
					DataBaseTypeStrategy.OnlyMain => GetOption(DataBaseType.Main),
					DataBaseTypeStrategy.OnlySecondary => GetOption(DataBaseType.Secondary),
					_ => null
				},
				DataBaseType.Main => DbOption.Main,
				DataBaseType.Secondary => DbOption.Secondary.Any()
					? DbOption.Secondary[DbOption.Secondary.Length == 1 ? 0 : Math.Abs(Guid.NewGuid().GetHashCode() % DbOption.Secondary.Length)]
					: null,
				_ => null,
			};
		}
		#endregion

		#region GetExecute
		/// <summary>
		///  获取连接实例
		/// </summary>
		/// <returns></returns>
		public ICreeperDbExecute GetExecute(DataBaseType dataBaseType)
			=> new CreeperDbExecute(GetExecuteOption(dataBaseType));
		#endregion

		#region Transaction
		public ICreeperDbExecute BeginTransaction()
			=> GetExecute(DataBaseType.Main).BeginTransaction();

		public ValueTask<ICreeperDbExecute> BeginTransactionAsync(CancellationToken cancellationToken = default)
			=> GetExecute(DataBaseType.Main).BeginTransactionAsync(cancellationToken);

		public void Transaction(Action<ICreeperDbExecute> action)
			=> GetExecute(DataBaseType.Main).Transaction(action);

		public ValueTask TransactionAsync(Action<ICreeperDbExecute> action, CancellationToken cancellationToken = default)
			=> GetExecute(DataBaseType.Main).TransactionAsync(action, cancellationToken);

		#endregion

		#region ExcuteDataReader
		public void ExecuteDataReader(Action<DbDataReader> action, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default)
			=> GetExecute(dataBaseType).ExecuteDataReader(action, cmdText, cmdType, cmdParams);

		public ValueTask ExecuteDataReaderAsync(Action<DbDataReader> action, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default)
			=> GetExecute(dataBaseType).ExecuteDataReaderAsync(action, cmdText, cmdType, cmdParams, cancellationToken);

		public List<T> ExecuteDataReaderList<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default)
			=> GetExecute(dataBaseType).ExecuteDataReaderList<T>(cmdText, cmdType, cmdParams);

		public Task<List<T>> ExecuteDataReaderListAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default)
			=> GetExecute(dataBaseType).ExecuteDataReaderListAsync<T>(cmdText, cmdType, cmdParams, cancellationToken);

		public T ExecuteDataReaderModel<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default)
			=> GetExecute(dataBaseType).ExecuteDataReaderModel<T>(cmdText, cmdType, cmdParams);

		public Task<T> ExecuteDataReaderModelAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default)
			=> GetExecute(dataBaseType).ExecuteDataReaderModelAsync<T>(cmdText, cmdType, cmdParams, cancellationToken);
		#endregion

		#region ExecuteDataReaderPipe
		public object[] ExecuteDataReaderPipe(IEnumerable<ISqlBuilder> builders, DataBaseType dataBaseType = DataBaseType.Default)
			=> GetExecute(dataBaseType).ExecuteDataReaderPipe(builders);

		public Task<object[]> ExecuteDataReaderPipeAsync(IEnumerable<ISqlBuilder> builders, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default)
			=> GetExecute(dataBaseType).ExecuteDataReaderPipeAsync(builders, cancellationToken);
		#endregion

		#region ExecuteNonQuery
		public int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null)
			=> GetExecute(DataBaseType.Main).ExecuteNonQuery(cmdText, cmdType, cmdParams);

		public ValueTask<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default)
			=> GetExecute(DataBaseType.Main).ExecuteNonQueryAsync(cmdText, cmdType, cmdParams, cancellationToken);
		#endregion

		#region ExecuteScalar
		public object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default)
			=> GetExecute(dataBaseType).ExecuteScalar(cmdText, cmdType, cmdParams);

		public T ExecuteScalar<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default)
			=> GetExecute(dataBaseType).ExecuteScalar<T>(cmdText, cmdType, cmdParams);

		public ValueTask<object> ExecuteScalarAsync(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default)
			=> GetExecute(dataBaseType).ExecuteScalarAsync(cmdText, cmdType, cmdParams, cancellationToken);

		public ValueTask<T> ExecuteScalarAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default)
			=> GetExecute(dataBaseType).ExecuteScalarAsync<T>(cmdText, cmdType, cmdParams, cancellationToken);
		#endregion

	}
}
