
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.PostgreSql.XUnitTest.Entity.Model;
using Creeper.PostgreSql.XUnitTest.Entity.Options;
using Creeper.PostgreSql.XUnitTest.Model;
using Creeper.SqlBuilder;
using Meta.xUnitTest.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Creeper.PostgreSql.XUnitTest
{
	[Order(2)]
	public class SelectNormal : BaseTest
	{
		public SelectNormal(ITestOutputHelper output) : base(output)
		{
		}

		[Fact]
		public void FirstOrDefault()
		{
			var info = DbContext.Select<StudentModel>().Where(a => a.People_id == StuPeopleId1).By(Generic.DataBaseType.Secondary).FirstOrDefault();

			Assert.Equal(StuPeopleId1, info.People_id);
		}

		[Fact]
		public void Frist()
		{
			var peopleId = DbContext.Select<StudentModel>().Where(a => a.People_id == StuPeopleId1).FirstOrDefault<Guid>("people_id");
			var info = DbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault<ToOneTTestModel>("name,id");

			var emptyNullablePeopleId = DbContext.Select<StudentModel>().Where(a => a.People_id == Guid.Empty).FirstOrDefault<Guid?>("people_id");
			var emptyPeopleId = DbContext.Select<StudentModel>().Where(a => a.People_id == Guid.Empty).FirstOrDefault<Guid>("people_id");

			Assert.IsType<ToOneTTestModel>(info);
			Assert.Equal(StuPeopleId1, info.Id);
			Assert.Equal(StuPeopleId1, peopleId);
			Assert.Null(emptyNullablePeopleId);
			Assert.Equal(Guid.Empty, emptyPeopleId);
		}

		[Fact]
		public void FristTuple()
		{
			var info = DbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1)
				.FirstOrDefault<(Guid id, string name)>(a => new { a.Id, a.Name });
			// if not found
			var notFoundInfo = DbContext.Select<PeopleModel>().Where(a => a.Id == Guid.Empty).FirstOrDefault<(Guid id, string name)>("id,name");
			var notFoundNullableInfo = DbContext.Select<PeopleModel>().Where(a => a.Id == Guid.Empty).FirstOrDefault<(Guid? id, string name)>("id,name");


			Assert.Equal(StuPeopleId1, info.id);
			Assert.Equal(Guid.Empty, notFoundInfo.id);
			Assert.Null(notFoundNullableInfo.id);
		}

		[Fact]
		public void SelectByDbCache()
		{
			Stopwatch sw = Stopwatch.StartNew();
			var info = DbContext.Select<StudentModel>().Where(a => a.Stu_no == StuNo1).ByCache(TimeSpan.FromMinutes(2)).FirstOrDefault();

			var a = sw.ElapsedMilliseconds.ToString();
			info = DbContext.Select<StudentModel>().Where(a => a.Stu_no == StuNo1).ByCache().FirstOrDefault();
			sw.Stop();
			var b = sw.ElapsedMilliseconds.ToString();
			//var key = _dbContext.Select<StudentModel>().Where(a => a.Stu_no == StuNo1).ByCache().ToScalar(a => a.Id);
			//Assert.Equal(StuNo1, info.Stu_no);
		}

		[Fact]
		public void ToOneDictonary()
		{
			// all
			var info = DbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault<Dictionary<string, object>>();
			// option
			var info1 = DbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault<Hashtable>("name,id");
			Assert.Equal(StuPeopleId1, Guid.Parse(info["id"].ToString()));
			Assert.Equal(StuPeopleId1, Guid.Parse(info1["id"].ToString()));
		}

		[Fact]
		public void ToList()
		{
			var info = DbContext.Select<PeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToList();

			Assert.Contains(info, f => f.Id == StuPeopleId1);
			Assert.Contains(info, f => f.Id == StuPeopleId2);
		}

		[Fact]
		public void ToListTuple()
		{
			var info = DbContext.Select<PeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToList<(Guid, string)>("id,name");

			Assert.Contains(info, f => f.Item1 == StuPeopleId1);
			Assert.Contains(info, f => f.Item1 == StuPeopleId2);
		}

		[Fact]
		public void ToListDictonary()
		{
			// all
			var info = DbContext.Select<PeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToList<Dictionary<string, object>>();
			// option
			var info1 = DbContext.Select<PeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).ToList<Dictionary<string, object>>("name,id");

			Assert.Contains(info, f => f["id"].ToString() == StuPeopleId1.ToString());
			Assert.Contains(info, f => f["id"].ToString() == StuPeopleId2.ToString());
			Assert.Contains(info1, f => f["id"].ToString() == StuPeopleId1.ToString());
			Assert.Contains(info1, f => f["id"].ToString() == StuPeopleId2.ToString());
		}

		[Fact]
		public void ToPipe()
		{
			object[] obj = DbContext.ExecuteDataReaderPipe(new ISqlBuilder[] {
				SelectBuilder<PeopleModel>.Select().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).PipeToList(),
				DbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).PipeFirstOrDefault<(Guid, string)>("id,name"),
				DbContext.Select<PeopleModel>().Where(a =>a.Id==StuPeopleId1).PipeToList<(Guid, string)>("id,name"),
				DbContext.Select<PeopleModel>().WhereAny(a => a.Id, new[] { StuPeopleId1, StuPeopleId2 }).PipeToList<Dictionary<string, object>>("name,id"),
				DbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).PipeFirstOrDefault<ToOneTTestModel>("name,id"),
				DbContext.Select<StudentModel>().Where(a => a.People_id == StuPeopleId1).PipeFirstOrDefault<Guid>("people_id"),
				DbContext.Select<StudentModel>().UnionLeftJoin<PeopleModel>((a,b) => a.People_id == b.Id).Where(a => a.People_id == StuPeopleId2).PipeUnionFirstOrDefault<PeopleModel>(),
				 });
			var info = obj[0].ToObjectArray().OfType<PeopleModel>();
			var info1 = ((Guid, string))obj[1];
			var info2 = obj[2].ToObjectArray().OfType<(Guid, string)>();
			var info3 = obj[3].ToObjectArray().OfType<Dictionary<string, object>>();
			var info4 = (ToOneTTestModel)obj[4];
			var info5 = (Guid)obj[5];
			var info7 = obj[6];
			Assert.Contains(info, f => f.Id == StuPeopleId1);
			Assert.Equal(StuPeopleId1, info1.Item1);
			Assert.Contains(info2, f => f.Item1 == StuPeopleId1);
			Assert.Contains(info3, f => f["id"].ToString() == StuPeopleId1.ToString());
			Assert.Equal(StuPeopleId1, info4.Id);
			Assert.Equal(StuPeopleId1, info5);
			//Assert.Null(info6);
		}

		[Fact]
		public void Count()
		{
			var count = DbContext.Select<PeopleModel>().Count();
		}

		[Fact]
		public void Max()
		{
			var maxAge = DbContext.Select<PeopleModel>().Max(a => a.Age);

			maxAge = DbContext.Select<StudentModel>().InnerJoin<PeopleModel>((a, b) => a.People_id == b.Id).Max<PeopleModel, int>(b => b.Age);
			Assert.True(maxAge >= 0);
		}

		[Fact]
		public void Min()
		{
			var minAge = DbContext.Select<PeopleModel>().Min(a => a.Age);
			Assert.True(minAge >= 0);
		}

		[Fact, Description("the type of T must be same as the column's type")]
		public async void Avg()
		{
			var avgAge1 = await DbContext.Select<PeopleModel>().FirstOrDefaultAsync<Guid>(a => a.Id);
			var avgAge = DbContext.Select<PeopleModel>().Avg<decimal>(a => a.Age);
			Assert.True(avgAge >= 0);
		}

		[Fact, Description("same usage as FirstOrDefault<T>(), but T is ValueType")]
		public void Scalar()
		{
			var id = DbContext.Select<PeopleModel>().Where(a => a.Id == StuPeopleId1).FirstOrDefault<Guid>(a => a.Id);
			Assert.Equal(StuPeopleId1, id);
		}

		[Fact]
		public void OrderBy()
		{
			var infos = DbContext.Select<PeopleModel>().InnerJoin<StudentModel>((a, b) => a.Id == b.People_id)
				.OrderByDescending(a => a.Age).ToList();
		}

		[Fact]
		public void GroupBy()
		{
			var result = DbContext.Select<StudentModel>().GroupBy(a => a.Grade_id).ToList(a => a.Grade_id);
		}

		[Fact]
		public void Page()
		{
			var result = DbContext.Select<StudentModel>().Page(1, 10).ToList();
		}

		[Fact]
		public void Limit()
		{
			var result = DbContext.Select<StudentModel>().Take(10).ToList();
		}

		[Fact]
		public void Skip()
		{
			var result = DbContext.Select<StudentModel>().Skip(10).ToList();
		}

		[Fact, Description("using with group by expression")]
		public void Having()
		{
			var result = DbContext.Select<StudentModel>().GroupBy(a => a.Grade_id).Having("COUNT(1) >= 10").ToList(a => a.Grade_id);
		}

		[Fact, Description("")]
		public void Union()
		{
			Stopwatch stop = new Stopwatch();
			stop.Start();
			var union0 = DbContext.Select<StudentModel>().UnionInnerJoin<PeopleModel>((a, b) => a.People_id == b.Id && a.Stu_no == StuNo1).UnionFirstOrDefault<PeopleModel>();

			var a = stop.ElapsedMilliseconds;
			var union1 = DbContext.Select<StudentModel>().UnionInnerJoin<PeopleModel>((a, b) => a.People_id == b.Id && a.Stu_no == StuNo1).UnionFirstOrDefault<PeopleModel>();
			var b = stop.ElapsedMilliseconds;
			var union2 = DbContext.Select<StudentModel>().UnionInnerJoin<PeopleModel>((a, b) => a.People_id == b.Id && a.Stu_no == StuNo1).UnionFirstOrDefault<PeopleModel>();
			var c = stop.ElapsedMilliseconds;
			stop.Stop();

			Output.WriteLine(string.Concat(a, ",", b, ",", c));
		}

		[Fact, Description("the type of T must be same as the column's type")]
		public void Sum()
		{
			var avgAge = DbContext.Select<PeopleModel>().Sum<long>(a => a.Age);
			Assert.True(avgAge >= 0);
		}
	}
}
