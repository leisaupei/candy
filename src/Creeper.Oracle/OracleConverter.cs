using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using Creeper.SqlBuilder;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace Creeper.Oracle
{
	internal class OracleConverter : CreeperDbConverterBase
	{
		public override DataBaseKind DataBaseKind => DataBaseKind.Oracle;

		public override DbParameter GetDbParameter(string name, object value)
			=> new OracleParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
		=> new OracleConnection(connectionString);


	}

}
