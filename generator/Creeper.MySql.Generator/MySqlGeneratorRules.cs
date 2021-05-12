using Creeper.Generator.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.MySql.Generator
{
	public class MySqlGeneratorRules
	{
		public MySqlExcepts Excepts { get; set; } = new MySqlExcepts();
		public FieldIgnore FieldIgnore { get; set; } = new FieldIgnore();
	}
	public class MySqlExcepts
	{
		public ExceptsGlobal Global { get; set; } = new ExceptsGlobal();
		public Dictionary<string, ExceptsGlobal> Customs { get; set; } = new Dictionary<string, ExceptsGlobal>();
	}

	public class FieldIgnore
	{
		public string[] Insert { get; set; }
		public string[] Returning { get; set; }
	}
}
