using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.DbHelper
{
	internal class TypeHelper
	{
		/// <summary>
		/// 静态数据库类型转换器
		/// </summary>
		public static IDictionary<DataBaseKind, ICreeperDbTypeConverter> DbTypeConverts = new Dictionary<DataBaseKind, ICreeperDbTypeConverter>();

		/// <summary>
		/// 实例键值对
		/// </summary>
		public static IReadOnlyDictionary<string, List<ICreeperDbConnectionOption>> ExecuteOptions;

		/// <summary>
		/// 通过数据库类型获取转换器
		/// </summary>
		/// <param name="dataBaseKind"></param>
		/// <returns></returns>
		public static ICreeperDbTypeConverter GetConverter(DataBaseKind dataBaseKind)
			=> DbTypeConverts.TryGetValue(dataBaseKind, out var convert)
			? convert : throw new NoCreeperDbTypeConverteException(dataBaseKind.ToString());
	}
}
