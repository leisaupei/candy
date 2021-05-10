using System;
using Newtonsoft.Json;

namespace Creeper.PostgreSql.XUnitTest.Entity.Model
{
	public partial struct Info
	{
		public Guid? Id { get; set; }
		public string Name { get; set; }
	}

}
