using Creeper.Generic;
using System;
using System.Data.Common;

namespace Creeper
{
	public class CreeperException : Exception
	{
		public CreeperException(string message) : base(message) { }

		public CreeperException(string message, Exception innerException) : base(message, innerException) { }
	}
	public class NoPrimaryKeyException<T> : CreeperException
	{
		public NoPrimaryKeyException() : base(typeof(T).Name + "没有主键标识") { }
	}
	public class DbConverterNotFoundException : CreeperException
	{
		public DbConverterNotFoundException(DataBaseKind dbKind) : base($"没有添加相应的数据库种类的转换器, 使用Add{dbKind}Option()注入相应数据库配置") { }
	}
	public class DbCacheNotFoundException : CreeperException
	{
		public DbCacheNotFoundException() : base("DbCache为空, 使用UseDbCache<T>方法注入数据库缓存示例") { }
	}
	public class CreeperDbTableAttributeNotFoundException : CreeperException
	{
		public CreeperDbTableAttributeNotFoundException() : base("没有找到CreeperDbTableAttribute特性") { }
	}
	public class DbExecuteNotFoundException : CreeperException
	{
		public DbExecuteNotFoundException() : base("DbExecute为空") { }
	}
	public class CreeperSqlExecuteException : CreeperException
	{
		public CreeperSqlExecuteException(string message, Exception innerException) : base(message, innerException) { }
	}
	public class DbConnectionOptionNotFoundException : CreeperException
	{
		public DbConnectionOptionNotFoundException(DataBaseType dataBaseType, DataBaseTypeStrategy dataBaseTypeStrategy) : base($"没找到相应的数据库配置, 策略: {dataBaseTypeStrategy}, 类型: {dataBaseType}") { }
	}
}
