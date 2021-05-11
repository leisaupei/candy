using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generator.Common.Contracts;
using Creeper.Generator.Common.Extensions;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using Creeper.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Creeper.MySql.Generator
{
	public class MySqlGeneratorProvider : CreeperGeneratorProviderBase
	{

		public MySqlGeneratorProvider()
		{
		}

		public override DataBaseKind DataBaseKind => DataBaseKind.MySql;

		public override void Generate(CreeperGeneratorGlobalOptions options, ICreeperDbExecute execute)
		{

			List<TableViewModel> tableList = GetTables(execute);
		}

		public override ICreeperDbConnectionOption GetDbConnectionOptionFromString(string conn)
		{
			var strings = conn.Split(';');
			var connectionString = string.Empty;
			string dbName = null;
			foreach (var item in strings)
			{
				var sp = item.Split('=');
				var left = sp[0];
				var right = sp[1];

				switch (left.ToLower())
				{
					//server=192.168.1.15;userid=root;pwd=123456;port=3306;database=demo;sslmode=none;
					case "host": connectionString += $"server={right};"; break;
					case "port": connectionString += $"port={right};"; break;
					case "user": connectionString += $"userid={right};"; break;
					case "pwd": connectionString += $"pwd={right};"; break;
					case "db": connectionString += $"database={right};"; break;
					case "name":
						if (string.IsNullOrEmpty(right) || right.ToLower() == CreeperGenerateOption.MASTER_DATABASE_TYPE_NAME.ToLower())
							dbName = CreeperGenerateOption.MASTER_DATABASE_TYPE_NAME;
						else
							dbName = right.ToUpperPascal();
						break;
				}
			}
			connectionString += $"sslmode=none;";
			dbName = string.IsNullOrEmpty(dbName) ? CreeperGenerateOption.MASTER_DATABASE_TYPE_NAME : dbName;
			ICreeperDbConnectionOption connection = new MySqlConnectionOption(connectionString, dbName);
			return connection;
		}

		/// <summary>
		/// 获取所有表
		/// </summary>
		/// <param name="schemaName"></param>
		/// <returns></returns>
		private List<TableViewModel> GetTables(ICreeperDbExecute execute)
		{
			using var connection = execute.ConnectionOptions.GetConnection();
			var db = connection.Database;

			var sql = $@"SELECT 
`table_name` AS `name`, 
(case `table_type` when 'base table' then 'table' else lower(`table_type`) end) as `type`, 
`table_comment` as `description`
FROM `information_schema`.`TABLES`
where `table_schema` = '{db}' and `table_type` in ('base table', 'view'); ";
			return execute.ExecuteDataReaderList<TableViewModel>(sql);
		}
	}
}
