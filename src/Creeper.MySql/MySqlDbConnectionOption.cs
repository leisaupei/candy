using Creeper.Driver;
using Creeper.Generic;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.MySql
{
	public class MySqlDbConnectionOption : ICreeperDbConnectionOption
	{
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="connectionString">数据库连接</param>
		/// <param name="dbName">数据库别名</param>
		public MySqlDbConnectionOption(string connectionString, string dbName)
		{
			ConnectionString = connectionString;
			DbName = dbName;
		}

		public string DbName { get; }

		/// <summary>
		/// 数据库连接
		/// </summary>
		public string ConnectionString { get; }

		/// <summary>
		/// 数据库类型
		/// </summary>
		public DataBaseKind DataBaseKind { get; } = DataBaseKind.MySql;

		/// <summary>
		/// 创建连接
		/// </summary>
		/// <returns></returns>
		public DbConnection GetConnection() => GetConnectionAsync(false, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();

		/// <summary>
		/// 创建连接
		/// </summary>
		/// <returns></returns>
		public Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken)
			=> cancellationToken.IsCancellationRequested ? Task.FromCanceled<DbConnection>(cancellationToken) : GetConnectionAsync(true, cancellationToken);

		async Task<DbConnection> GetConnectionAsync(bool async, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
				return await Task.FromCanceled<DbConnection>(cancellationToken);

			DbConnection connection = new MySqlConnection(ConnectionString);

			if (connection == null)
				throw new ArgumentNullException(nameof(connection));

			if (async)
				await connection.OpenAsync(cancellationToken);
			else
				connection.Open();

			return connection;
		}
	}
}
