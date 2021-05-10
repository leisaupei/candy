using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generator.Common;
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

		public override void Generate(string modelPath, CreeperGenerateOption option, bool folder, ICreeperDbExecute execute)
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
					case "name": dbName = string.IsNullOrEmpty(right) ? CreeperGenerateOption.MASTER_DATABASE_TYPE_NAME : GenerateHelper.ToUpperPascal(right); break;
				}
			}
			connectionString += $"sslmode=none;";
			dbName = string.IsNullOrEmpty(dbName) ? CreeperGenerateOption.MASTER_DATABASE_TYPE_NAME : dbName;
			ICreeperDbConnectionOption connections = new MySqlConnectionOption(connectionString, dbName);
			return connections;
		}

		private static string ToUpperPascal(string s) => string.IsNullOrEmpty(s) ? s : $"{s[0..1].ToUpper()}{s[1..]}";

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

		public override Action GetFinallyGen()
		{
			return () => { };
		}
	}
}
