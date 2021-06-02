﻿using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using Creeper.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.Driver
{
	public static class CreeperDbContextExtensions
	{
		#region Select
		/// <summary>
		/// 查询数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static SelectBuilder<TModel> Select<TModel>(this ICreeperDbContext dbContext) where TModel : class, ICreeperDbModel, new()
			=> new SelectBuilder<TModel>(dbContext);

		/// <summary>
		/// 查询数据, 等同于Select&lt;TModel&gt;().Where(Expression&lt;Func&lt;TModel, bool&gt;&gt;)
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static SelectBuilder<TModel> Select<TModel>(this ICreeperDbContext dbContext, Expression<Func<TModel, bool>> selector) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Select<TModel>().Where(selector);
		#endregion

		#region Insert
		/// <summary>
		/// 插入数据, 返回插入数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static InsertBuilder<TModel> Insert<TModel>(this ICreeperDbContext dbContext) where TModel : class, ICreeperDbModel, new()
			=> new InsertBuilder<TModel>(dbContext);

		/// <summary>
		/// 仅插入数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="model"></param>
		/// <returns>受影响行数</returns>
		public static int InsertOnly<TModel>(this ICreeperDbContext dbContext, TModel model) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Insert<TModel>().Set(model).ToAffectedRows();

		/// <summary>
		/// 仅插入数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>受影响行数</returns>
		public static ValueTask<int> InsertOnlyAsync<TModel>(this ICreeperDbContext dbContext, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Insert<TModel>().Set(model).ToAffectedRowsAsync(cancellationToken);

		/// <summary>
		/// 插入多条数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="models"></param>
		/// <returns>受影响行数</returns>
		public static int InsertOnly<TModel>(this ICreeperDbContext dbContext, IEnumerable<TModel> models) where TModel : class, ICreeperDbModel, new()
		{
			var sqlBuilders = models.Select(model => dbContext.Insert<TModel>().Set(model).PipeToAffectedRows());
			return dbContext.Get(DataBaseType.Main).ExecuteDataReaderPipe(sqlBuilders).OfType<int>().Sum();
		}

		/// <summary>
		/// 插入多条数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="models"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>受影响行数</returns>
		public static async ValueTask<int> InsertOnlyAsync<TModel>(this ICreeperDbContext dbContext, IEnumerable<TModel> models, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
		{
			var sqlBuilders = models.Select(model => dbContext.Insert<TModel>().Set(model).PipeToAffectedRows());
			var affrows = await dbContext.Get(DataBaseType.Main).ExecuteDataReaderPipeAsync(sqlBuilders, cancellationToken);
			return affrows.OfType<int>().Sum();
		}

		/// <summary>
		/// 插入单条数据, 返回插入数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="model"></param>
		/// <returns>插入的数据</returns>
		public static TModel Insert<TModel>(this ICreeperDbContext dbContext, TModel model) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Insert<TModel>().Set(model).ToAffectedRows(out TModel result) > 0 ? result : default;

		/// <summary>
		/// 插入单条数据, 返回插入数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>插入的数据</returns>
		public static Task<TModel> InsertAsync<TModel>(this ICreeperDbContext dbContext, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Insert<TModel>().Set(model).FirstOrDefaultAsync(cancellationToken);
		#endregion

		#region Update
		/// <summary>
		/// 更新数据
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static UpdateBuilder<TModel> Update<TModel>(this ICreeperDbContext dbContext) where TModel : class, ICreeperDbModel, new()
			=> new UpdateBuilder<TModel>(dbContext);

		/// <summary>
		/// 更新数据, 自动带上主键条件
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static UpdateBuilder<TModel> Update<TModel>(this ICreeperDbContext dbContext, TModel model) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Update<TModel>().Where(model);

		/// <summary>
		/// 更新数据, 自动带上主键条件
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static UpdateBuilder<TModel> Update<TModel>(this ICreeperDbContext dbContext, IEnumerable<TModel> models) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Update<TModel>().Where(models);

		/// <summary>
		/// 以主键为条件更新数据, 仅返回受影响行数; 此处程序覆盖数据库方法, 只适用某些可视数据覆盖后端数据场景, 请谨慎使用
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static int UpdateOnly<TModel>(this ICreeperDbContext dbContext, TModel model) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Update<TModel>().Set(model).ToAffectedRows();

		/// <summary>
		/// 以主键为条件更新数据, 仅返回受影响行数; 此处程序覆盖数据库方法, 只适用某些可视数据覆盖后端数据场景, 请谨慎使用
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static ValueTask<int> UpdateOnlyAsync<TModel>(this ICreeperDbContext dbContext, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Update<TModel>().Set(model).ToAffectedRowsAsync(cancellationToken);

		/// <summary>
		/// 以主键为条件更新数据, 仅返回受影响行数; 此处程序覆盖数据库方法, 只适用某些可视数据覆盖后端数据场景, 请谨慎使用
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static int UpdateOnly<TModel>(this ICreeperDbContext dbContext, IEnumerable<TModel> models) where TModel : class, ICreeperDbModel, new()
		{
			var sqlBuilders = models.Select(model => dbContext.Update<TModel>().Set(model).PipeToAffectedRows());
			return dbContext.Get(DataBaseType.Main).ExecuteDataReaderPipe(sqlBuilders).OfType<int>().Sum();
		}

		/// <summary>
		/// 以主键为条件更新数据, 仅返回受影响行数; 此处程序覆盖数据库方法, 只适用某些可视数据覆盖后端数据场景, 请谨慎使用
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static async ValueTask<int> UpdateOnlyAsync<TModel>(this ICreeperDbContext dbContext, IEnumerable<TModel> models, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
		{
			var sqlBuilders = models.Select(model => dbContext.Update<TModel>().Set(model).PipeToAffectedRows());
			var affrows = await dbContext.Get(DataBaseType.Main).ExecuteDataReaderPipeAsync(sqlBuilders, cancellationToken);
			return affrows.OfType<int>().Sum();
		}

		/// <summary>
		/// 以主键为条件更新数据, 返回更新后数据; 此处程序覆盖数据库方法, 只适用某些可视数据覆盖后端数据场景, 请谨慎使用
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static TModel UpdateOne<TModel>(this ICreeperDbContext dbContext, TModel model) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Update<TModel>().Set(model).ToAffectedRows(out TModel result) > 0 ? result : default;

		/// <summary>
		/// 以主键为条件更新数据, 返回更新后数据; 此处程序覆盖数据库方法, 只适用某些可视数据覆盖后端数据场景, 请谨慎使用
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns></returns>
		public static Task<TModel> UpdateOneAsync<TModel>(this ICreeperDbContext dbContext, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
		=> dbContext.Update<TModel>().Set(model).FirstOrDefaultAsync(cancellationToken);

		#endregion

		#region Delete
		/// <summary>
		/// 删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <returns>受影响行数</returns>
		public static DeleteBuilder<TModel> Delete<TModel>(this ICreeperDbContext dbContext) where TModel : class, ICreeperDbModel, new()
			=> new DeleteBuilder<TModel>(dbContext);

		/// <summary>
		/// 以主键为条件删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="model"></param>
		/// <returns>受影响行数</returns>
		public static int Delete<TModel>(this ICreeperDbContext dbContext, TModel model) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Delete<TModel>().Where(model).ToAffectedRows();

		/// <summary>
		/// 以主键为条件删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="model"></param>
		/// <returns>受影响行数</returns>
		public static ValueTask<int> DeleteAsync<TModel>(this ICreeperDbContext dbContext, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Delete<TModel>().Where(model).ToAffectedRowsAsync(cancellationToken);

		/// <summary>
		/// 以主键为条件删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="models"></param>
		/// <returns>受影响行数</returns>
		public static int Delete<TModel>(this ICreeperDbContext dbContext, IEnumerable<TModel> models) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Delete<TModel>().Where(models).ToAffectedRows();

		/// <summary>
		/// 以主键为条件删除数据, 仅返回受影响行数
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="models"></param>
		/// <returns>受影响行数</returns>
		public static ValueTask<int> DeleteAsync<TModel>(this ICreeperDbContext dbContext, IEnumerable<TModel> models, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Delete<TModel>().Where(models).ToAffectedRowsAsync(cancellationToken);
		#endregion

		#region Upsert
		/// <summary>
		/// 根据数据库主键更新/插入, 仅返回受影响行数
		/// </summary>
		/// <remarks>
		/// 主键值为default(不赋值或忽略)时, 必定是插入;
		/// 若主键条件的行存在, 则更新该行; 否则插入一行, 主键取决于类型规则。
		/// - 整型自增主键: 根据数据库自增标识
		/// - 随机唯一主键: Guid程序会自动生成, 其他算法需要赋值;
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="model"></param>
		/// <returns>受影响行数</returns>
		public static int UpsertOnly<TModel>(this ICreeperDbContext dbContext, TModel model) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Insert<TModel>().Upsert(model).ToAffectedRows();

		/// <summary>
		/// 根据数据库主键更新/插入, 仅返回受影响行数
		/// </summary>
		/// <remarks>
		/// 主键值为default(不赋值或忽略)时, 必定是插入;
		/// 若主键条件的行存在, 则更新该行; 否则插入一行, 主键取决于类型规则。
		/// - 整型自增主键: 根据数据库自增标识
		/// - 随机唯一主键: Guid程序会自动生成, 其他算法需要赋值;
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>受影响行数</returns>
		public static ValueTask<int> UpsertOnlyAsync<TModel>(this ICreeperDbContext dbContext, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Insert<TModel>().Upsert(model).ToAffectedRowsAsync(cancellationToken);

		/// <summary>
		/// 根据数据库主键更新/插入, 仅返回受影响行数
		/// </summary>
		/// <remarks>
		/// 主键值为default(不赋值或忽略)时, 必定是插入;
		/// 若主键条件的行存在, 则更新该行; 否则插入一行, 主键取决于类型规则。
		/// - 整型自增主键: 根据数据库自增标识
		/// - 随机唯一主键: Guid程序会自动生成, 其他算法需要赋值;
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="models"></param>
		/// <returns>受影响行数</returns>
		public static int UpsertOnly<TModel>(this ICreeperDbContext dbContext, IEnumerable<TModel> models) where TModel : class, ICreeperDbModel, new()
		{
			var sqlBuilders = models.Select(model => dbContext.Insert<TModel>().Upsert(model).PipeToAffectedRows());
			return dbContext.Get(DataBaseType.Main).ExecuteDataReaderPipe(sqlBuilders).OfType<int>().Sum();
		}

		/// <summary>
		/// 根据数据库主键更新/插入, 仅返回受影响行数
		/// </summary>
		/// <remarks>
		/// 主键值为default(不赋值或忽略)时, 必定是插入;
		/// 若主键条件的行存在, 则更新该行; 否则插入一行, 主键取决于类型规则。
		/// - 整型自增主键: 根据数据库自增标识
		/// - 随机唯一主键: Guid程序会自动生成, 其他算法需要赋值;
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="models"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>受影响行数</returns>
		public static async ValueTask<int> UpsertOnlyAsync<TModel>(this ICreeperDbContext dbContext, IEnumerable<TModel> models, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
		{
			var sqlBuilders = models.Select(model => dbContext.Insert<TModel>().Upsert(model).PipeToAffectedRows());
			var affrows = await dbContext.Get(DataBaseType.Main).ExecuteDataReaderPipeAsync(sqlBuilders, cancellationToken);
			return affrows.OfType<int>().Sum();
		}

		/// <summary>
		/// 根据数据库主键更新/插入, 返回更新/插入数据
		/// </summary>
		/// <remarks>
		/// 主键值为default(不赋值或忽略)时, 必定是插入;
		/// 若主键条件的行存在, 则更新该行; 否则插入一行, 主键取决于类型规则。
		/// - 整型自增主键: 根据数据库自增标识
		/// - 随机唯一主键: Guid程序会自动生成, 其他算法需要赋值;
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="model"></param>
		/// <returns>受影响行数</returns>
		public static TModel Upsert<TModel>(this ICreeperDbContext dbContext, TModel model) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Insert<TModel>().Upsert(model).FirstOrDefault();

		/// <summary>
		///根据数据库主键更新/插入, 返回更新/插入数据
		/// </summary>
		/// <remarks>
		/// 主键值为default(不赋值或忽略)时, 必定是插入;
		/// 若主键条件的行存在, 则更新该行; 否则插入一行, 主键取决于类型规则。
		/// - 整型自增主键: 根据数据库自增标识
		/// - 随机唯一主键: Guid程序会自动生成, 其他算法需要赋值;
		/// </remarks>
		/// <typeparam name="TModel"></typeparam>
		/// <param name="dbContext"></param>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>受影响行数</returns>
		public static Task<TModel> UpsertAsync<TModel>(this ICreeperDbContext dbContext, TModel model, CancellationToken cancellationToken = default) where TModel : class, ICreeperDbModel, new()
			=> dbContext.Insert<TModel>().Upsert(model).FirstOrDefaultAsync(cancellationToken);
		#endregion
	}
}
