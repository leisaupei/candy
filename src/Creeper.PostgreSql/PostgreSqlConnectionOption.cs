using Creeper.Driver;
using Creeper.Generic;
using Creeper.PostgreSql;
using Npgsql;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.PostgreSql
{
	public class PostgreSqlConnectionOption : CreeperDbConnectionOptionBase
	{
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="connectionString">数据库连接</param>
		/// <param name="dbName">数据库别名</param>
		/// <param name="options">配置</param>
		public PostgreSqlConnectionOption(string connectionString, string dbName, DbConnectionOptions options) : base(connectionString, dbName, DataBaseKind.PostgreSql)
		{
		}

		/// <summary>
		/// 针对不同类型的数据库需要响应的配置
		/// </summary>
		public DbConnectionOptions Options { get; }

		public override void SetDbOptions(DbConnection connection)
		{
			Options?.MapAction?.Invoke((NpgsqlConnection)connection);
		}
	}
}
