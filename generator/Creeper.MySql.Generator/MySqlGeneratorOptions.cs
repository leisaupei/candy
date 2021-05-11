using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.MySql.Generator
{
	public class MySqlRules
	{
		public MySqlExcepts Excepts { get; set; } = new MySqlExcepts();
		public FieldIgnore FieldIgnore { get; set; } = new FieldIgnore();
	}
	public class MySqlExcepts
	{
		public MySqlExceptsGlobal Global { get; set; } = new MySqlExceptsGlobal();
		public Dictionary<string, MySqlExceptsGlobal> Customs { get; set; } = new Dictionary<string, MySqlExceptsGlobal>();
	}
	public class MySqlExceptsGlobal
	{
		public string[] Schemas { get; set; } = new string[0];
		public string[] Tables { get; set; } = new string[0];
		public string[] Views { get; set; } = new string[0];
		public string[] Composites { get; set; } = new string[0];
	}
	public class FieldIgnore
	{
		public string[] Insert { get; set; }
		public string[] Returning { get; set; }
	}
}
