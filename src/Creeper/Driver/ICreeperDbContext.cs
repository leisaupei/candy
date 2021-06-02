using Creeper.Generic;
using Creeper.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.Driver
{
	public interface ICreeperDbContext
	{
		/// <summary>
		/// 数据库缓存实例
		/// </summary>
		ICreeperDbCache DbCache { get; }

		/// <summary>
		/// 开启事务
		/// </summary>
		/// <returns></returns>
		ICreeperDbExecute BeginTransaction();

		/// <summary>
		/// 开启事务
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<ICreeperDbExecute> BeginTransactionAsync(CancellationToken cancellationToken = default);

		/// <summary>
		/// 读取数据返回
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		void ExecuteDataReader(Action<DbDataReader> action, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 读取数据返回
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask ExecuteDataReaderAsync(Action<DbDataReader> action, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取多行记录, 用实体类列表接收
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		List<T> ExecuteDataReaderList<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 获取多行记录, 用实体类列表接收
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<T>> ExecuteDataReaderListAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取一行记录用实体类接收
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		T ExecuteDataReaderModel<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 获取一行记录用实体类接收
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<T> ExecuteDataReaderModelAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 执行多个sql语句
		/// </summary>
		/// <param name="builders"></param>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		object[] ExecuteDataReaderPipe(IEnumerable<ISqlBuilder> builders, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 执行多个sql语句
		/// </summary>
		/// <param name="builders"></param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<object[]> ExecuteDataReaderPipeAsync(IEnumerable<ISqlBuilder> builders, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取command text修改行数
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null);

		/// <summary>
		/// 获取command text修改行数
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <returns></returns>
		ValueTask<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取单个返回值
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 获取单个返回值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		T ExecuteScalar<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default);

		/// <summary>
		/// 获取单个返回值
		/// </summary>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<object> ExecuteScalarAsync(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取单个返回值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cmdText">sql语句</param>
		/// <param name="cmdType">command type</param>
		/// <param name="cmdParams">command parameters</param>
		/// <param name="dataBaseType"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		ValueTask<T> ExecuteScalarAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);

		/// <summary>
		/// 获取主/从数据库请求示例
		/// </summary>
		/// <param name="dataBaseType"></param>
		/// <returns></returns>
		ICreeperDbExecute Get(DataBaseType dataBaseType);

		/// <summary>
		/// 事务, 自动提交事务, 当action抛出异常时回滚事务
		/// </summary>
		/// <param name="action"></param>
		void Transaction(Action<ICreeperDbExecute> action);

		/// <summary>
		/// 事务, 自动提交事务, 当action抛出异常时回滚事务
		/// </summary>
		/// <param name="action"></param>
		/// <param name="cancellationToken"></param>
		ValueTask TransactionAsync(Action<ICreeperDbExecute> action, CancellationToken cancellationToken = default);
	}
}
