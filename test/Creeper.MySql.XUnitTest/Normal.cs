using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using Creeper.MySql.Types;
using Creeper.MySql.XUnitTest.Entity.Model;
using Creeper.MySql.XUnitTest.Entity.Options;
using Creeper.SqlBuilder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;
using TestSpace = MySql.Data.Types;
namespace Creeper.MySql.XUnitTest
{
	public class Normal : BaseTest
	{

		[Fact]
		public void Join()
		{
			var info = DbContext.Select<PeopleModel>()
				.InnerJoinUnion<StudentModel>((a, b) => a.Id == b.People_id)
				.FirstOrDefaultUnion<StudentModel>();

			var union = DbContext.Select<PeopleModel>().Where(a => a.Id == 2)
					.Union(SelectBuilder.Select<PeopleModel>().Where(a => a.Id == 2))
					.ToList();
		}

		[Fact]
		public void Insert()
		{
			var people = DbContext.InsertOnly(new StudentModel
			{
				Class_name = "ศํผþ",
				Teacher_name = "ะกร๗",
				People_id = 2,
				Stu_no = 124,
			});
		}


		[Fact]
		public void UpdateReturning()
		{
			var info = DbContext.ExecuteDataReaderModel<PeopleModel>(@"
	UPDATE `people` SET `name` = '111', `age` =20 WHERE `id` = 1;
	select * from `people` where id = 1;");
		}
		[Fact]
		public void InsertReturning()
		{
			var info = DbContext.ExecuteDataReaderModel<PeopleModel>(@"
INSERT INTO `demo`.`people`(`name`, `age`) VALUES ('222', 20);
select *from  `demo`.`people` where `id` = LAST_INSERT_ID();");

			MySqlCommand cmd = new MySqlCommand();
		}

	}
}
