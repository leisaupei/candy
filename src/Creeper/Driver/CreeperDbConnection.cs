using Creeper.DbHelper;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.Driver
{
	public class CreeperDbConnection : ICreeperDbConnection
	{
		public CreeperDbConnection(string connectionString, DataBaseKind dataBaseKind)
		{
			ConnectionString = connectionString;
			DataBaseKind = dataBaseKind;
		}

		public string ConnectionString { get; }
		public DataBaseKind DataBaseKind { get; }
		public Action<DbConnection> DbConnectionOptions { get; set; }

		private ICreeperDbConverter _dbConverter;
		private ICreeperDbConverter DbConverter => _dbConverter ??= TypeHelper.GetConverter(DataBaseKind);
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

			DbConnection connection = DbConverter.GetDbConnection(ConnectionString);

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
