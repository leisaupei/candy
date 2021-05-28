using Creeper.Generic;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.Driver
{
	public interface ICreeperDbConnection
	{
		/// <summary>
		/// 数据库连接字符串
		/// </summary>
		string ConnectionString { get; }

		/// <summary>
		/// 数据库类型
		/// </summary>
		DataBaseKind DataBaseKind { get; }

		/// <summary>
		/// 获取dbconnection
		/// </summary>
		/// <returns></returns>
		DbConnection GetConnection();

		/// <summary>
		/// 数据库连接配置(在DbConnection.Open()之后执行)
		/// </summary>
		Action<DbConnection> DbConnectionOptions { get; set; }

		/// <summary>
		/// 获取dbconnection
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken);

	}
}
