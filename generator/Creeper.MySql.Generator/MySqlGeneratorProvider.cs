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
		private readonly MySqlGeneratorRules _mySqlRule;

		public MySqlGeneratorProvider(IOptions<MySqlGeneratorRules> mySqlRuleAccessor)
		{
			_mySqlRule = mySqlRuleAccessor.Value;
		}

		public override DataBaseKind DataBaseKind => DataBaseKind.MySql;

		public override void Generate(CreeperGeneratorGlobalOptions options, ICreeperDbExecute execute)
		{

			List<TableViewModel> tableList = GetTables(execute);
			foreach (var item in tableList)
			{
				MySqlTableGenerator td = new MySqlTableGenerator(execute, _mySqlRule.FieldIgnore, options);
				td.Generate(item);
			}
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
`TABLE_NAME` AS `name`, 
(CASE `TABLE_TYPE` WHEN 'BASE TABLE' THEN 'table' ELSE lower(`TABLE_TYPE`) END) AS `type`, 
`TABLE_COMMENT` AS `description`
FROM  `information_schema`.`TABLES`
WHERE `TABLE_SCHEMA` = '{db}' AND `TABLE_TYPE` IN ('BASE TABLE', 'VIEW'); ";
			return execute.ExecuteDataReaderList<TableViewModel>(sql);
		}
	}
}
