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
	public abstract class CreeperDbConnectionOptionBase : ICreeperDbConnectionOption
	{
		public CreeperDbConnectionOptionBase(string connectionString, string dbName, DataBaseKind dataBaseKind)
		{
			ConnectionString = connectionString;
			DbName = dbName;
			DataBaseKind = dataBaseKind;
		}

		public string DbName { get; }
		public string ConnectionString { get; }
		public DataBaseKind DataBaseKind { get; }

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

			DbConnection connection = TypeHelper.GetConverter(DataBaseKind).GetDbConnection(ConnectionString);

			if (connection == null)
				throw new ArgumentNullException(nameof(connection));

			if (async)
				await connection.OpenAsync(cancellationToken);
			else
				connection.Open();

			SetDbOptions(connection);

			return connection;
		}

		public virtual void SetDbOptions(DbConnection connection)
		{

		}
	}
}
