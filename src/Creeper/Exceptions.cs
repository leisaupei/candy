using Creeper.Generic;
using System;
using System.Data.Common;

namespace Creeper
{
	public class CreeperException : Exception
	{
		public CreeperException(string message) : base(message)
		{
		}

		public CreeperException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
	public class NoPrimaryKeyException<T> : CreeperException
	{
		public NoPrimaryKeyException() : base(typeof(T).Name + "没有主键标识")
		{
		}
	}
	public class DbConverterNotFoundException : CreeperException
	{
		public DbConverterNotFoundException(DataBaseKind dbKind) : base(dbKind + "没有添加相应的数据库种类的转换器")
		{
		}
	}
	public class CreeperSqlExecuteException : CreeperException
	{
		public CreeperSqlExecuteException(string message, Exception innerException) : base(message, innerException)
		{

		}
	}
	public class DbConnectionOptionNotFoundException : CreeperException
	{
		public DbConnectionOptionNotFoundException(DataBaseType dataBaseType, DataBaseTypeStrategy dataBaseTypeStrategy) : base($"没找到相应的数据库配置, 策略: {dataBaseTypeStrategy}, 类型: {dataBaseType}")
		{

		}
	}
}
