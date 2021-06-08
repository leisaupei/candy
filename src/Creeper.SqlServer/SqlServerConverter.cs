using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using Creeper.SqlBuilder;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace Creeper.SqlServer
{
	internal class SqlServerConverter : CreeperDbConverterBase
	{
		public override DataBaseKind DataBaseKind => DataBaseKind.SqlServer;

		public override string DbFieldMark => "`";

		public override DbParameter GetDbParameter(string name, object value)
			=> new SqlParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
		=> new SqlConnection(connectionString);
	}

}
