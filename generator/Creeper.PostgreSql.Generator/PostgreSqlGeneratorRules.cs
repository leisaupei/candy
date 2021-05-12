using Creeper.Generator.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.PostgreSql.Generator
{
	public class PostgreSqlGeneratorRules
	{
		public PostgreSqlExcepts Excepts { get; set; } = new PostgreSqlExcepts();
		public FieldIgnore FieldIgnore { get; set; } = new FieldIgnore();
	}
	public class PostgreSqlExcepts
	{
		public PostgreSqlExceptsGlobal Global { get; set; } = new PostgreSqlExceptsGlobal();
		public Dictionary<string, PostgreSqlExceptsGlobal> Customs { get; set; } = new Dictionary<string, PostgreSqlExceptsGlobal>();
	}
	public class PostgreSqlExceptsGlobal : ExceptsGlobal
	{
		public string[] Schemas { get; set; } = new string[0];
		public string[] Composites { get; set; } = new string[0];
	}
}
