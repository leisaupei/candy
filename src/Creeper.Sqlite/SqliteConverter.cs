﻿using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using Creeper.SqlBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace Creeper.Sqlite
{
	public class SqliteConverter : CreeperDbConverterBase
	{
		public override DataBaseKind DataBaseKind => DataBaseKind.Sqlite;

		public override string ConvertSqlToString(ISqlBuilder sqlBuilder)
		{
			var sql = sqlBuilder.CommandText;

			foreach (var p in sqlBuilder.Params)
			{
				var value = p.Value?.ToString();
				var key = string.Concat("@", p.ParameterName);
				if (value == null)
					sql = SqlHelper.GetNullSql(sql, key);

				else if (ParamPattern.IsMatch(value) && p.DbType == DbType.String)
					sql = sql.Replace(key, value);

				else
					sql = sql.Replace(key, $"'{value}'");
			}
			return sql.Replace("\r", " ").Replace("\n", " ");
		}

		public override DbParameter GetDbParameter(string name, object value)
			=> new SQLiteParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
		=> new SQLiteConnection(connectionString);


	}

}
