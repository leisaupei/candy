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
    public class NoCreeperDbTypeConverteException : CreeperException
    {
        public NoCreeperDbTypeConverteException(string dbName) : base(dbName + "没有添加相应的数据库类型转换器")
        {
        }
    }
    public class CreeperSqlExecuteException : CreeperException
    {
        public CreeperSqlExecuteException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
