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
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace Creeper.Access
{
	internal class AccessConverter : CreeperDbConverterBase
	{
		public override DataBaseKind DataBaseKind => DataBaseKind.Access;

		public override DbParameter GetDbParameter(string name, object value)
			=> new OleDbParameter(name, value);

		public override DbConnection GetDbConnection(string connectionString)
		=> new OleDbConnection(connectionString);


	}

}
