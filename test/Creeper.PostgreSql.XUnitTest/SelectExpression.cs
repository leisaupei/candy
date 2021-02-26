using Creeper.Extensions;
using Creeper.PostgreSql.XUnitTest.Entity.Model;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.Ordering;

namespace Creeper.PostgreSql.XUnitTest
{
	[Order(3)]
	public class SelectExpression : BaseTest
	{
		public SelectExpression(ITestOutputHelper output) : base(output)
		{
		}

		[Fact]
		public void OrderBy()
		{
			var list = _dbContext.Select<PeopleModel>().OrderByDescending(a => a.Create_time).ToList();
		}
		[Fact]
		public void Join()
		{
			var union = _dbContext.Select<StudentModel>()
					.InnerJoin<ClassmateModel>((a, b) => a.Id == b.Student_id)
					.InnerJoin<ClassmateModel, TeacherModel>((b, c) => b.Teacher_id == c.Id)
					.InnerJoin<PeopleModel>((a, d) => a.People_id == d.Id && d.Id == StuPeopleId1)
					.ToOne();

			Assert.Null(union);
		}
		[Fact]
		public void WhereStaticMember()
		{
			var info = _dbContext.Select<StudentModel>()
					.Where(a => a.People_id == StuPeopleId1)
					.ToOne();

			Assert.Equal(info.People_id, StuPeopleId1);
		}
		[Fact]
		public void WhereInternalMember()
		{
			var id = StuPeopleId1;
			var info = _dbContext.Select<StudentModel>()
					.Where(a => a.People_id == id)
					.ToOne();

			Assert.Equal(info.People_id, StuPeopleId1);
		}
		[Fact]
		public void WhereExists()
		{
			var info = _dbContext.Select<StudentModel>()
				.WhereExists(_dbContext.Select<PeopleModel>().Field(b => b.Id).Where(b => b.Id == StuPeopleId1))
				.ToOne();
		}
		[Fact]
		public void WhereClassProperty()
		{
			var model = new TestModel
			{
				Id = StuPeopleId1
			};
			var info = _dbContext.Select<StudentModel>()
					.Where(a => a.People_id == model.Id)
					.ToOne();

			Assert.Equal(StuPeopleId1, info.People_id);
		}
		[Fact]
		public void WhereNull()
		{
			var info = _dbContext.Select<PeopleModel>()
					.Where(a => a.Sex == null)
					.ToOne();

			Assert.Null(info?.Sex);
		}
		[Fact]
		public void WhereDefault()
		{
			var info = _dbContext.Select<PeopleModel>()
					.Where(a => a.Id == default)
					.ToOne();

			Assert.Null(info?.Sex);
		}
		[Fact]
		public void WhereInnerClassPeoperty()
		{
			var model = new ParentTestModel
			{
				Info = new TestModel
				{
					Id = StuPeopleId1
				}
			};
			var model1 = new ParentPreantTestModel
			{
				Info = new ParentTestModel
				{
					Info = new TestModel
					{
						Id = StuPeopleId1
					}
				}
			};
			var info = _dbContext.Select<StudentModel>()
					 .Where(a => a.People_id == model.Info.Id)
					 .ToOne();
			info = _dbContext.Select<StudentModel>()
					 .Where(a => a.People_id == model1.Info.Info.Id)
					 .ToOne();
			Assert.Equal(info.People_id, StuPeopleId1);
		}
		[Fact]
		public void WhereClassStaticMember()
		{
			var info = _dbContext.Select<StudentModel>()
				  .Where(a => a.People_id == TestModel._id)
				  .ToOne();
			Assert.Equal(info.People_id, StuPeopleId1);
		}
		[Fact]
		public void WhereConst()
		{
			var info = _dbContext.Select<PeopleModel>()
				.Where(a => a.Id == Guid.Empty)
				.ToOne();
			info = _dbContext.Select<PeopleModel>()
				.Where(a => a.Name == "leisaupei")
				.ToOne();
			Assert.Equal("leisaupei", info.Name);
		}
		[Fact]
		public void WhereMethodLast()
		{
			var info = _dbContext.Select<PeopleModel>()
				  .Where(a => a.Name == "leisaupei".ToString())
				  .ToOne();
			Assert.Equal("leisaupei", info.Name);
		}
		[Fact]
		public void WhereDifficultMethod()
		{
			var info = _dbContext.Select<PeopleModel>()
				  .Where(a => a.Create_time < DateTime.Now.AddDays(-1))
				  .ToOne();
			Assert.NotNull(info);
		}
		[Fact]
		public void WhereMethod()
		{
			var info = _dbContext.Select<PeopleModel>()
				  .Where(a => a.Id == Guid.Parse(StuPeopleId1.ToString()))
				  .ToOne();
			Assert.Equal(info.Id, StuPeopleId1);
		}
		[Fact]
		public void WhereNewArray()
		{
			var info = _dbContext.Select<TypeTestModel>()
				.Where(a => a.Array_type == new[] { 1 })
				.ToOne();
			var info1 = _dbContext.Select<TypeTestModel>()
				.Where(a => a.Uuid_array_type == new[] { Guid.Empty })
				.ToOne();
			if (info != null)
				Assert.Equal(1, info.Array_type.FirstOrDefault());
		}

		[Fact]
		public void WhereNewClass()
		{
			var info = _dbContext.Select<PeopleModel>()
				  .Where(a => a.Id == new TestModel()
				  {
					  Id = StuPeopleId1
				  }.Id)
				  .ToOne();
			Assert.Equal(StuPeopleId1, info.Id);
		}
		[Fact]
		public void WhereNewStruct()
		{
			var info = _dbContext.Select<PeopleModel>()
				  .Where(a => a.Id == new Guid())
				  .ToOne();
			Assert.True(info == null || info?.Id == new Guid());
		}
		[Fact]
		public void WhereMultiple()
		{
			var info = _dbContext.Select<PeopleModel>()
				  .Where(a => (a.Id == new Guid() && a.Id == StuPeopleId1) || a.Id == StuPeopleId2)
				  .ToOne();
			Assert.Equal(StuPeopleId2, info.Id);
		}
		[Fact]
		public void UnionMultiple()
		{
			var info = _dbContext.Select<PeopleModel>()
				.InnerJoin<StudentModel>((a, b) => a.Id == b.People_id && (b.People_id == StuPeopleId1 || a.Id == StuPeopleId2))
				.Where<StudentModel>(b => b.People_id == StuPeopleId1)
				.ToOne();
			Assert.Equal(StuPeopleId1, info.Id);
		}
		[Fact]
		public void WhereOperationExpression()
		{
			var info = _dbContext.Select<PeopleModel>().Where(a => DateTime.Today - a.Create_time > TimeSpan.FromDays(2)).ToOne();
			Assert.NotNull(info);
		}
		[Fact]
		public void WhereIndexParameter()
		{
			Guid[] id = new[] { StuPeopleId1, Guid.Empty };
			var info = _dbContext.Select<PeopleModel>().Where(a => a.Id == id[0]).ToOne();
			Assert.NotNull(info);
		}
		[Fact]
		public void WhereFieldParameter()
		{
			var arr = new[] { 1 };
			var info = _dbContext.Select<TypeTestModel>().Where(a => a.Array_type[1] == arr[0]).ToOne();
			Assert.NotNull(info);
		}
		[Fact]
		public void WhereFieldLength()
		{
			var info = _dbContext.Select<TypeTestModel>().Where(a => a.Array_type.Length == 2).ToOne();
			Assert.NotNull(info);
		}
		[Fact]
		public void WhereEnum()
		{
			EDataState value = EDataState.正常;
			var info = _dbContext.Select<TypeTestModel>().Where(a => a.Enum_type == value).ToOne();
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Enum_type == EDataState.正常).ToOne();
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Int4_type == (int)value).ToOne();
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Int4_type == (int)EDataState.正常).ToOne();
			Assert.NotNull(info);
		}
		[Fact]
		public void WhereContains()
		{
			var xx = new PeopleModel();
			xx.Id = Guid.Empty;
			var b = new PeopleModel();
			b.Id = Guid.Empty;
			TypeTestModel info = null;

			info = _dbContext.Select<TypeTestModel>().Where(a => new[] { xx.Id, b.Id }.Contains(a.Id)).ToOne();
			Assert.NotNull(info);

			info = _dbContext.Select<TypeTestModel>().Where(a => new[] { 1, 3, 4 }.Contains(a.Int4_type.Value)).ToOne();
			Assert.NotNull(info);

			//a.int_type <> all(array[2,3])
			info = _dbContext.Select<TypeTestModel>().Where(a => !a.Array_type.Contains(3)).ToOne();
			Assert.NotNull(info);

			//3 = any(a.array_type)
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Array_type.Contains(3)).ToOne();
			Assert.NotNull(info);

			//var ints = new int[] { 2, 3 }.Select(f => f).ToList();
			//a.int_type = any(array[2,3])
			info = _dbContext.Select<TypeTestModel>().Where(a => new int[] { 2, 3 }.Select(f => f).ToArray().Contains(a.Int4_type.Value)).ToOne();
			Assert.NotNull(info);

			info = _dbContext.Select<TypeTestModel>().Where(a => new[] { (int)EDataState.已删除, (int)EDataState.正常 }.Contains(a.Int4_type.Value)).ToOne();

			Assert.NotNull(info);
		}
		[Fact]
		public void WhereStringLike()
		{
			TypeTestModel info = null;


			//'xxx' like a.Varchar_type || '%'
			info = _dbContext.Select<TypeTestModel>().Where(a => "xxxxxxxxxxxx".StartsWith(a.Varchar_type)).ToOne();
			Assert.NotNull(info);
			//a.varchar_type like '%xxx%'
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Varchar_type.Contains("xxx")).ToOne();
			Assert.NotNull(info);
			//a.varchar_type like 'xxx%'
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Varchar_type.StartsWith("xxx")).ToOne();
			Assert.NotNull(info);
			//a.varchar_type like '%xxx'
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Varchar_type.EndsWith("xxx")).ToOne();
			Assert.NotNull(info);

			//a.varchar_type ilike '%xxx%'
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Varchar_type.Contains("xxx", StringComparison.OrdinalIgnoreCase)).ToOne();
			Assert.NotNull(info);
			//a.varchar_type ilike 'xxx%'
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Varchar_type.StartsWith("xxx", StringComparison.OrdinalIgnoreCase)).ToOne();
			Assert.NotNull(info);
			//a.varchar_type ilike '%xxx'
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Varchar_type.EndsWith("xxx", StringComparison.OrdinalIgnoreCase)).ToOne();
			Assert.NotNull(info);
		}
		[Fact]
		public void WhereToString()
		{
			TypeTestModel info = null;
			//a.varchar_type::text = 'xxxx'
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Varchar_type.ToString() == "xxxx").ToOne();
			Assert.NotNull(info);
			ParentPreantTestModel model = new ParentPreantTestModel { Name = "xxxx" };
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Varchar_type.ToString() == model.Name.ToString()).ToOne();
			Assert.NotNull(info);

		}
		[Fact]
		public void WhereEqualsFunction()
		{
			TypeTestModel info = null;
			Func<string, string> fuc = (str) =>
			{
				return "xxxx";
			};
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Varchar_type == fuc("xxxx")).ToOne();
			Assert.NotNull(info);
		}
		[Fact]
		public void WhereBlock()
		{
			TypeTestModel info = null;
			var judge = 0;
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Varchar_type == (judge == 0 ? "lsp" : "")).ToOne();
			Assert.NotNull(info);
		}
		[Fact]
		public void WhereArrayEqual()
		{
			TypeTestModel info = null;
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Array_type == new[] { 0, 1 }).ToOne();
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Uuid_array_type == new[] { Guid.Empty }).ToOne();
			info = _dbContext.Select<TypeTestModel>().Where(a => new[] { "广东" } == a.Varchar_array_type).ToOne();
			info = _dbContext.Select<TypeTestModel>().Where(a => a.Varchar_array_type == new[] { "广东" }).ToOne();
			Assert.NotNull(info);
		}
		[Fact]
		public void WhereCoalesce()
		{
			TypeTestModel info = null;
			var sum = _dbContext.Select<TypeTestModel>().Sum(a => a.Int8_type ?? 0, 0);
			info = _dbContext.Select<TypeTestModel>().Where(a => (a.Int4_type ?? 3) == 3).ToOne();
			Assert.NotNull(info);

		}
		[Fact]
		public void WhereEqualFieldWithNamespace()
		{
			var info = _dbContext.Select<StudentModel>()
					.Where(a => a.People_id == Creeper.PostgreSql.XUnitTest.BaseTest.StuPeopleId1)
					.ToOne();

			Assert.Equal(info.People_id, Creeper.PostgreSql.XUnitTest.BaseTest.StuPeopleId1);
		}
		[Fact]
		public void WhereCompareSelf()
		{
			var info = _dbContext.Select<StudentModel>()
					.Where(a => a.People_id == a.Id || a.People_id == StuPeopleId1)
					.ToOne();

			Assert.Equal(info.People_id, StuPeopleId1);
		}
		[Fact]
		public void WhereIn()
		{
			var info = _dbContext.Select<StudentModel>()
					.WhereIn(a => a.People_id, _dbContext.Select<StudentModel>().Field(a => a.People_id).Where(a => a.People_id == StuPeopleId1))
					.ToOne();

			Assert.Equal(info.People_id, StuPeopleId1);
		}
		[Fact]
		public void Test()
		{
			//	ParentPreantTestModel model = new ParentPreantTestModel { Name = "xxx" };
			//	var info = _dbContext.Select<TypeTestModel>() .Where(a => !a.Varchar_type.Contains(model.Name, StringComparison.OrdinalIgnoreCase)).ToOne();
			//Expression<Func<ClassmateModel, Guid>> expression = f => f.Grade_id;
			//MemberExpression body = expression.Body as MemberExpression;
			//var equ = Expression.Equal(body, Expression.Constant(Guid.Empty, typeof(Guid)));
			//var lambda = Expression.Lambda<Func<ClassmateModel, bool>>(equ, body.Expression as ParameterExpression);
			//SqlExpressionVisitor.Instance.VisitCondition(lambda);

		}
		public class ParentPreantTestModel
		{
			public string Name { get; set; }
			public ParentTestModel Info { get; set; }
		}
		public class ParentTestModel
		{
			public TestModel Info { get; set; }
		}
		public class TestModel
		{
			public static Guid _id = StuPeopleId1;
			public Guid Id { get; set; }
		}
	}
}