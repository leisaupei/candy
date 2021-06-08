using Creeper.DbHelper;
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
	internal class SqliteConverter : CreeperDbConverterBase
	{
		public override DataBaseKind DataBaseKind => DataBaseKind.Sqlite;

		public override DbParameter GetDbParameter(string name, object value)
			=> new SQLiteParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
		=> new SQLiteConnection(connectionString);


	}

}
