using Creeper.Driver;
using Creeper.MySql.XUnitTest.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Creeper.MySql.XUnitTest
{
	public class Upsert : BaseTest
	{
		[Fact]
		public void Identity()
		{
			var ass = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
			//var result = DbContext.UpsertOnly(new PeopleModel
			//{
			//	Age = 20,
			//	Name = "222",
			//	Stu_no = 12
			//});
		}
	}
}
