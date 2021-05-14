using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Creeper.MySql.XUnitTest
{
	public class UnitTest1 : BaseTest
	{
		[Fact]
		public void Test1()
		{
			var path = Path.GetDirectoryName(@"d:\workspace\abcd.txt");
			//_dbContext.ExecuteNonQuery("insert into test(`name`,`age`) values('¿Ó–ﬁ∆§',20);");

		}
	}
}
