using System.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Xunit.Extensions.Ordering;
using System.Data.Common;
using Npgsql;
using Creeper.SqlBuilder;
using Creeper.PostgreSql.XUnitTest.Entity.Model;
using System.Reflection;

namespace Creeper.PostgreSql.XUnitTest
{
	[Order(5)]
	public class Delete //: BaseTest
	{
		[Fact]
		public void Union()
		{

			var temp = TestEnum.A | TestEnum.B;
			var aa = (int)temp;
			var bo = temp.HasFlag(TestEnum.A | TestEnum.B);
			var bo1 = temp.HasFlag(TestEnum.C);
			var istrue = (temp & TestEnum.A & TestEnum.B) != 0;
			var istrue1 = (temp & TestEnum.C & TestEnum.B) != 0;

			temp = temp & (~TestEnum.B);

		}
		[Flags]
		public enum TestEnum : uint
		{
			x = 0,
			A = 1,
			B = 2,
			C = 4,
		}

	}
}
