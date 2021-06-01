using Creeper.Driver;
using Creeper.Extensions;
using Creeper.PostgreSql.XUnitTest.Entity.Model;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using Xunit;
using Xunit.Extensions.Ordering;

namespace Creeper.PostgreSql.XUnitTest
{
	[Order(1)]
	public class Insert : BaseTest
	{
		[Fact, Order(1), Description("name of create_time can be ignored if use 'Create_time = Datetime.Now' in ModelInsert")]
		public void ModelInsertReturnModel()
		{
			var info = DbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault();
			if (info == null)
			{
				info = DbContext.Insert(new PeopleModel
				{
					Address = "xxx",
					Id = StuPeopleId1,
					Age = 10,
					Create_time = DateTime.Now, // you can ignore if use Datetime.Now;
					Name = "leisaupei",
					Sex = true,
					State = EtDataState.正常,
					Address_detail = new JObject
					{
						["province"] = "广东",
						["city"] = "广州"
					},
				});

				Assert.NotNull(info);
			}
			info = DbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId2).FirstOrDefault();
			if (info == null)
			{
				// else you can
				info = DbContext.Insert(new PeopleModel
				{
					Address = "xxx",
					Id = StuPeopleId2,
					Age = 10,
					Create_time = DateTime.Now,
					Name = "leisaupei",
					Sex = true,
					State = EtDataState.正常,
					Address_detail = new JObject
					{
						["province"] = "广东",
						["city"] = "广州"
					},
				});
				Assert.NotNull(info);
			}
		}
		[Fact, Order(2)]
		public void ModelInsertReturnModifyRows()
		{
			var info = DbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId2).FirstOrDefault();
			if (info != null) return;

			var row = DbContext.InsertOnly(new PeopleModel
			{
				Address = "xxx",
				Id = StuPeopleId2,
				Age = 10,
				Create_time = DateTime.Now,
				Name = "nickname",
				Sex = true,
				State = EtDataState.正常,
				Address_detail = new JObject
				{
					["province"] = "广东",
					["city"] = "广州"
				},
			});
			Assert.Equal(1, row);
		}
		[Fact, Order(3)]
		public void InsertCustomizedDictonary()
		{
			var info = DbContext.Select<ClassGradeModel>().Where(a => a.Id == GradeId).FirstOrDefault();
			if (info != null) return;

			var affrows = DbContext.Insert<ClassGradeModel>().Set(a => a.Id, GradeId)
				.Set(f => f.Name, "移动互联网")
				.Set(a => a.Create_time, DateTime.Now)
				.ToAffectedRows(out info); //return modify rows out model

			Assert.NotNull(info);
			Assert.Equal(1, affrows);
		}
		[Fact, Order(4)]
		public void InsertCustomized()
		{
			var info = DbContext.Select<StudentModel>().Where(a => a.People_id == StuPeopleId1).FirstOrDefault();
			if (info == null)
			{
				var affrows = DbContext.Insert<StudentModel>().Set(f => f.Id, Guid.NewGuid())
								.Set(a => a.People_id, StuPeopleId1)
								.Set(a => a.Stu_no, StuNo1)
								.Set(a => a.Grade_id, GradeId)
								.Set(a => a.Create_time, DateTime.Now)
								.ToAffectedRows(out info);
				Assert.Equal(1, affrows);
				Assert.NotNull(info);
			}

			var info1 = DbContext.Select<StudentModel>().Where(a => a.People_id == StuPeopleId2).FirstOrDefault();
			if (info1 == null)
			{
				var affrows1 = DbContext.Insert<StudentModel>().Set(a => a.Id, Guid.NewGuid())
								.Set(a => a.People_id, StuPeopleId2)
								.Set(a => a.Stu_no, StuNo2)
								.Set(a => a.Grade_id, GradeId)
								.Set(a => a.Create_time, DateTime.Now)
								.ToAffectedRows(out info1);
				Assert.NotNull(info1);
				Assert.Equal(1, affrows1);
			}
		}
		[Fact, Order(4)]
		public void InsertMultiple()
		{
			var info = DbContext.Insert<PeopleModel>().Set(new PeopleModel
			{
				Address = "xxx",
				Id = Guid.NewGuid(),
				Age = 10,
				Create_time = DateTime.Now,
				Name = "nickname",
				Sex = true,
				State = EtDataState.正常,
				Address_detail = new JObject
				{
					["province"] = "广东",
					["city"] = "广州"
				}
			}).WhereNotExists(DbContext.Select<PeopleModel>().Where(a => a.Name == "小明")).ToAffectedRows();
			var arr = new[] {
				new PeopleModel
				{
					Address = "xxx",
					Id = Guid.NewGuid(),
					Age = 10,
					Create_time = DateTime.Now,
					Name = "nickname",
					Sex = true,
					State = EtDataState.正常,
					Address_detail = new JObject
					{
						["province"] = "广东",
						["city"] = "广州"
					},
				},
				new PeopleModel
				{
					Address = "xxx",
					Id = Guid.NewGuid(),
					Age = 10,
					Create_time = DateTime.Now,
					Name = "nickname",
					Sex = true,
					State = EtDataState.正常,
					Address_detail = new JObject
					{
						["province"] = "广东",
						["city"] = "广州"
					},
				}
			};
			var rows = DbContext.InsertOnly(arr);

			Assert.NotEqual(0, rows);
		}
	}
}
