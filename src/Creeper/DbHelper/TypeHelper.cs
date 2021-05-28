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
		public static IDictionary<DataBaseKind, ICreeperDbConverter> DbTypeConverters = new Dictionary<DataBaseKind, ICreeperDbConverter>();

		/// <summary>
		/// 通过数据库类型获取转换器
		/// </summary>
		/// <param name="dataBaseKind"></param>
		/// <returns></returns>
		public static ICreeperDbConverter GetConverter(DataBaseKind dataBaseKind)
			=> DbTypeConverters.TryGetValue(dataBaseKind, out var convert)
			? convert : throw new DbConverterNotFoundException(dataBaseKind);
	}
}
