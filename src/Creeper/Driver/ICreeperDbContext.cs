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
		ICreeperDbExecute BeginTransaction();
		ValueTask<ICreeperDbExecute> BeginTransactionAsync(CancellationToken cancellationToken = default);
		void ExecuteDataReader(Action<DbDataReader> action, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default);
		ValueTask ExecuteDataReaderAsync(Action<DbDataReader> action, string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);
		List<T> ExecuteDataReaderList<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default);
		Task<List<T>> ExecuteDataReaderListAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);
		T ExecuteDataReaderModel<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default);
		Task<T> ExecuteDataReaderModelAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);
		object[] ExecuteDataReaderPipe(IEnumerable<ISqlBuilder> builders, DataBaseType dataBaseType = DataBaseType.Default);
		Task<object[]> ExecuteDataReaderPipeAsync(IEnumerable<ISqlBuilder> builders, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);
		int ExecuteNonQuery(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null);
		ValueTask<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, CancellationToken cancellationToken = default);
		object ExecuteScalar(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default);
		T ExecuteScalar<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default);
		ValueTask<object> ExecuteScalarAsync(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);
		ValueTask<T> ExecuteScalarAsync<T>(string cmdText, CommandType cmdType = CommandType.Text, DbParameter[] cmdParams = null, DataBaseType dataBaseType = DataBaseType.Default, CancellationToken cancellationToken = default);
		ICreeperDbExecute GetExecute(DataBaseType dataBaseType);
		void Transaction(Action<ICreeperDbExecute> action);
		ValueTask TransactionAsync(Action<ICreeperDbExecute> action, CancellationToken cancellationToken = default);
	}
}
