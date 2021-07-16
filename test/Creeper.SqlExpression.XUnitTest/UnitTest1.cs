using Xunit;
using System.Collections;
using System.Linq;
using Creeper.SqlBuilder.ExpressionAnalysis;
using Newtonsoft.Json;
using Creeper.Generic;
using System;
using System.Reflection;

namespace Creeper.SqlExpression.XUnitTest
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{

			//Action<TestsssModel> action1 = obj => obj.Age = 192;
			//Action<TestModel> action2 = Copy<TestsssModel, TestModel>(action1);

			//var t3 = new TestModel();
			//action2.Invoke(t3);
			//var t1 = new TestsssModel { Age = 10 };
			//var t3 = new TestModel();
			//Copy(t1, t3);
		}
		private static TTo Copy<TFrom, TTo>(TFrom fromValue) where TTo : new()
		{
			var toValue = Activator.CreateInstance<TTo>();
			Copy(fromValue, toValue);
			return toValue;
		}
		private static void Copy<TFrom, TTo>(TFrom fromValue, TTo toValue) where TTo : new()
		{
			if (fromValue is null)
			{
				throw new ArgumentNullException(nameof(fromValue));
			}

			var toProperties = typeof(TTo).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			var fromType = typeof(TFrom);

			foreach (var toProperty in toProperties)
			{
				var fromProperty = fromType.GetProperty(toProperty.Name, BindingFlags.Public | BindingFlags.Instance);
				if (fromProperty.PropertyType == toProperty.PropertyType)
					fromProperty.SetValue(toValue, fromProperty.GetValue(fromValue));
			}
		}
		public static Action<TTo> Copy<TFrom, TTo>(Action<TFrom> from) where TTo : new()
		{
			if (from is null)
			{
				throw new ArgumentNullException(nameof(from));
			}

			var fromValue = Activator.CreateInstance<TFrom>();
			from.Invoke(fromValue);

			return new Action<TTo>(toValue => Copy(fromValue, toValue));
		}
		public class TestModel
		{
			public int Age { get; set; }
		}
		public class TestsssModel : TestModel
		{

		}
	}
}
