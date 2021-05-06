using Creeper.Driver;
using Creeper.Generic;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.MySql
{
	public class MySqlConnectionOption : CreeperDbConnectionOptionBase
	{
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="connectionString">数据库连接</param>
		/// <param name="dbName">数据库别名</param>
		public MySqlConnectionOption(string connectionString, string dbName) : base(connectionString, dbName, DataBaseKind.MySql) { }
	}
}
