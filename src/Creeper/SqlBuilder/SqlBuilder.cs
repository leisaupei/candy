using Creeper.DbHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Creeper.SqlBuilder.ExpressionAnalysis;
using Creeper.Driver;
using Creeper.Generic;
using Creeper.Extensions;

namespace Creeper.SqlBuilder
{
	public abstract class SqlBuilder<TBuilder, TModel> : ISqlBuilder
		where TBuilder : class, ISqlBuilder
		where TModel : class, ICreeperDbModel, new()
	{
		#region Identity
		/// <summary>
		/// 是否使用缓存
		/// </summary>
		private DbCacheType _cacheType = DbCacheType.None;

		/// <summary>
		/// 缓存过期时间
		/// </summary>
		private TimeSpan? _dbCacheExpireTime;
		private DataBaseType _dataBaseType = DataBaseType.Default;
		private readonly ICreeperDbContext _dbContext;
		private readonly ICreeperDbExecute _dbExecute;

		/// <summary>
		/// 类型转换
		/// </summary>
		private TBuilder This => this as TBuilder;

		/// <summary>
		/// 主表
		/// </summary>
		protected string MainTable { get; }

		/// <summary>
		/// 
		/// </summary>
		protected ICreeperDbConverter DbConverter { get; }

		/// <summary>
		/// 主表别名, 默认为: "a"
		/// </summary>
		protected string MainAlias { get; set; } = "a";

		/// <summary>
		/// where条件列表
		/// </summary>
		protected List<string> WhereList { get; } = new List<string>();

		/// <summary>
		/// 是否返回默认值, 默认: false
		/// </summary>
		public bool IsReturnDefault { get; set; } = false;

		/// <summary>
		/// 返回实例类型
		/// </summary>
		public Type Type { get; set; }

		/// <summary>
		/// 参数列表
		/// </summary>
		public List<DbParameter> Params { get; } = new List<DbParameter>();

		/// <summary>
		/// 返回类型
		/// </summary>
		public PipeReturnType ReturnType { get; set; }

		/// <summary>
		/// 查询字段
		/// </summary>
		public string Fields { get; set; }

		/// <summary>
		/// where条件数量
		/// </summary>
		public int WhereCount => WhereList.Count;
		#endregion

		#region Constructor
		protected SqlBuilder(ICreeperDbContext dbContext) : this() => _dbContext = dbContext;

		protected SqlBuilder(ICreeperDbExecute dbExecute) : this() => _dbExecute = dbExecute;

		protected SqlBuilder()
		{
			var table = EntityHelper.GetDbTable<TModel>();
			DbConverter = TypeHelper.GetConverter(table.DataBaseKind);

			if (string.IsNullOrEmpty(MainTable)) MainTable = table.TableName;
		}
		#endregion

		/// <summary>
		/// 选择主库还是从库, Default预设策略
		/// </summary>
		/// <returns></returns>
		public TBuilder By(DataBaseType dataBaseType)
		{
			_dataBaseType = dataBaseType;
			return This;
		}

		/// <summary>
		/// 使用数据库缓存, 仅支持FirstOrDefault,ToScalar方法
		/// </summary>
		/// <returns></returns>
		public TBuilder ByCache(TimeSpan? expireTime = null)
		{
			_ = _dbContext.DbCache ?? throw new CreeperDbCacheNotFoundException();
			_cacheType = DbCacheType.Default;
			_dbCacheExpireTime = expireTime;
			return This;
		}

		/// <summary>
		/// 使用主键缓存
		/// </summary>
		/// <returns></returns>
		[Obsolete]
		public TBuilder ByPkCache(TimeSpan? expireTime = null)
		{
			_ = _dbContext.DbCache ?? throw new CreeperDbCacheNotFoundException();
			_cacheType = DbCacheType.PkCache;
			_dbCacheExpireTime = expireTime;
			return This;
		}

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="parameterName"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public TBuilder AddParameter(string parameterName, object value)
			=> AddParameter(DbConverter.GetDbParameter(parameterName, value));

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="value"></param>
		/// <param name="parameterName"></param>
		/// <returns></returns>
		public TBuilder AddParameter(out string parameterName, object value)
			=> AddParameter(parameterName = ParameterCounting.Index, value);

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="ps"></param>
		/// <returns></returns>
		public TBuilder AddParameter(DbParameter ps)
		{
			Params.Add(ps);
			return This;
		}

		/// <summary>
		/// 添加参数
		/// </summary>
		/// <param name="ps"></param>
		/// <returns></returns>
		public TBuilder AddParameters(IEnumerable<DbParameter> ps)
		{
			Params.AddRange(ps);
			return This;
		}

		#region ToScalar

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <returns></returns>
		protected object ToScalar() => GetCacheResult(() => DbExecute.ExecuteScalar(CommandText, CommandType.Text, Params.ToArray()));

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <returns></returns>
		protected ValueTask<object> ToScalarAsync(CancellationToken cancellationToken)
			=> GetCacheResultAsync(() => DbExecute.ExecuteScalarAsync(CommandText, CommandType.Text, Params.ToArray(), cancellationToken));

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <returns></returns>
		protected TKey ToScalar<TKey>() => GetCacheResult(() => DbExecute.ExecuteScalar<TKey>(CommandText, CommandType.Text, Params.ToArray()));

		/// <summary>
		/// 返回第一个元素
		/// </summary>
		/// <returns></returns>
		protected ValueTask<TKey> ToScalarAsync<TKey>(CancellationToken cancellationToken)
			=> GetCacheResultAsync(() => DbExecute.ExecuteScalarAsync<TKey>(CommandText, CommandType.Text, Params.ToArray(), cancellationToken));
		#endregion

		#region ToList
		/// <summary>
		/// 返回list 
		/// </summary>
		/// <typeparam name="TResult">model type</typeparam>
		/// <returns></returns>
		protected List<TResult> ToList<TResult>()
			=> DbExecute.ExecuteDataReaderList<TResult>(CommandText, CommandType.Text, Params.ToArray());

		/// <summary>
		/// 返回list 
		/// </summary>
		/// <typeparam name="TResult">model type</typeparam>
		/// <returns></returns>
		protected Task<List<TResult>> ToListAsync<TResult>(CancellationToken cancellationToken)
			=> DbExecute.ExecuteDataReaderListAsync<TResult>(CommandText, CommandType.Text, Params.ToArray(), cancellationToken);
		#endregion

		#region FirstOrDefault
		/// <summary>
		/// 返回第一行
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		protected TResult FirstOrDefault<TResult>()
			=> GetCacheResult(() => DbExecute.ExecuteDataReaderModel<TResult>(CommandText, CommandType.Text, Params.ToArray()));

		/// <summary>
		/// 返回第一行
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		protected Task<TResult> FirstOrDefaultAsync<TResult>(CancellationToken cancellationToken)
			=> GetCacheResultAsync(() => DbExecute.ExecuteDataReaderModelAsync<TResult>(CommandText, CommandType.Text, Params.ToArray(), cancellationToken));
		#endregion

		#region ToRows
		/// <summary>
		/// 返回行数
		/// </summary>
		/// <returns></returns>
		protected int ToAffectedRows()
		{
			ReturnType = PipeReturnType.Rows;
			return DbExecute.ExecuteNonQuery(CommandText, CommandType.Text, Params.ToArray());
		}
		/// <summary>
		/// 返回行数
		/// </summary>
		/// <returns></returns>
		protected ValueTask<int> ToAffectedRowsAsync(CancellationToken cancellationToken)
		{
			ReturnType = PipeReturnType.Rows;
			return DbExecute.ExecuteNonQueryAsync(CommandText, CommandType.Text, Params.ToArray(), cancellationToken);
		}
		#endregion

		/// <summary>
		/// 输出管道元素
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		protected TBuilder Pipe<TResult>(PipeReturnType returnType)
		{
			Type = typeof(TResult);
			ReturnType = returnType;
			return This;
		}

		#region Override
		/// <summary>
		/// Override ToString()
		/// </summary>
		/// <returns></returns>
		public override string ToString() => ToString(null);

		/// <summary>
		/// 输出sql语句
		/// </summary>
		public string CommandText => GetCommandText();

		/// <summary>
		/// 调试或输出用
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public string ToString(string field)
		{
			if (!string.IsNullOrEmpty(field)) Fields = field;
			return DbConverter.ConvertSqlToString(this);
		}

		/// <summary>
		/// 设置sql语句
		/// </summary>
		/// <returns></returns>
		public abstract string GetCommandText();
		#endregion

		#region Implicit
		//public static implicit operator string(SqlBuilder<TBuilder, TModel> builder) => builder.ToString();
		#endregion

		#region private
		private static readonly string _cachePrefix = "creeper_cache_";
		private TResult GetCacheResult<TResult>(Func<TResult> fn)
		{
			if (_cacheType == DbCacheType.None) return fn.Invoke();
			var key = string.Concat(_cachePrefix, ToString().GetMD5String());
			if (_dbContext.DbCache.Exists(key)) return (TResult)_dbContext.DbCache.Get(key, typeof(TResult));
			var ret = fn.Invoke();
			_dbContext.DbCache.Set(key, ret, _dbCacheExpireTime);
			return ret;
		}
		private async Task<TResult> GetCacheResultAsync<TResult>(Func<Task<TResult>> fn)
		{
			if (_cacheType == DbCacheType.None) return await fn.Invoke();
			var key = string.Concat(_cachePrefix, ToString().GetMD5String());
			if (await _dbContext.DbCache.ExistsAsync(key)) return (TResult)await _dbContext.DbCache.GetAsync(key, typeof(TResult));
			var ret = await fn.Invoke();
			await _dbContext.DbCache.SetAsync(key, ret, _dbCacheExpireTime);
			return ret;
		}
		private async ValueTask<TResult> GetCacheResultAsync<TResult>(Func<ValueTask<TResult>> fn) => await GetCacheResultAsync(() => fn.Invoke().AsTask());

		private ICreeperDbExecute DbExecute
			=> _dbExecute ?? _dbContext.Get(_dataBaseType) ?? throw new CreeperDbExecuteNotFoundException();
		#endregion

	}
}
