using Creeper.Attributes;
using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Extensions;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Creeper.SqlBuilder
{
	/// <summary>
	/// upsert 语句实例
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public sealed class UpsertBuilder<TModel> : WhereBuilder<UpsertBuilder<TModel>, TModel>
		where TModel : class, ICreeperDbModel, new()
	{
		/// <summary>
		/// 字段列表
		/// </summary>
		private readonly Dictionary<string, string> _upsertSets = new Dictionary<string, string>();
		private readonly List<string> _fields = new List<string>();
		private readonly List<string> _primaryKeys = new List<string>();
		private readonly List<string> _identityKeys = new List<string>();


		internal UpsertBuilder(ICreeperDbContext dbContext) : base(dbContext) { }

		internal UpsertBuilder(ICreeperDbExecute dbExecute) : base(dbExecute) { }

		/// <summary>
		/// 插入更新
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		internal UpsertBuilder<TModel> Upsert(TModel model)
		{
			EntityHelper.GetAllFields<TModel>(p =>
			{
				string name = DbConverter.WithQuotationMarks(p.Name.ToLower());
				_fields.Add(name);
				object value = p.GetValue(model);
				var column = p.GetCustomAttribute<CreeperDbColumnAttribute>();
				if (column != null)
				{
					if ((column.IgnoreFlags & IgnoreWhen.Insert) != 0)
						return;

					//如果自增字段而且没有赋值, 那么忽略此字段
					if (IngoreIdentity(column, value, p.PropertyType))
						_identityKeys.Add(name);

					//如果是Guid主键而且没有赋值, 那么生成一个值
					if (column.Primary)
					{
						_primaryKeys.Add(name);
						value = SetNewGuid(value);
					}

				}
				//value = SetDefaultDateTime(name, value);
				Set(name, value);
			});
			if (_primaryKeys.Count == 0)
				throw new NoPrimaryKeyException<TModel>();

			return this;
		}
		private static bool IngoreIdentity(CreeperDbColumnAttribute column, object value, Type propertyType)
		{
			if (column.Identity)
			{
				var def = Activator.CreateInstance(propertyType);

				if (def is null || value.ToString() == def.ToString())
					return true;
			}
			return false;
		}

		private static object SetNewGuid(object value)
		{
			if (value is Guid g && g == default)
				value = Guid.NewGuid();
			return value;
		}

		/// <summary>
		/// 设置某字段的值
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private UpsertBuilder<TModel> Set(string key, object value)
		{
			if (value == null)
			{
				_upsertSets[key] = "null";
				return this;
			}
			var isSpecial = DbConverter.SetSpecialDbParameter(out string format, ref value);

			AddParameter(out string index, value);

			var pName = string.Concat("@", index);

			_upsertSets[key] = !isSpecial ? pName : string.Format(format, pName);
			return this;
		}

		/// <summary>
		/// 返回修改行数
		/// </summary>
		/// <returns></returns>
		public new int ToAffectedRows() => base.ToAffectedRows();

		/// <summary>
		/// 返回修改行数
		/// </summary>
		/// <returns></returns>
		public new ValueTask<int> ToAffectedRowsAsync(CancellationToken cancellationToken = default)
			=> base.ToAffectedRowsAsync(cancellationToken);

		/// <summary>
		/// 返回受影响行数
		/// </summary>
		/// <returns></returns>
		public UpsertBuilder<TModel> PipeToAffectedRows() => base.Pipe<int>(PipeReturnType.Rows);

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <returns></returns>
		public int ToAffectedRows<TResult>(out TResult info)
		{
			ReturnType = PipeReturnType.One;
			info = FirstOrDefault<TResult>();
			return info != null ? 1 : 0;
		}

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <returns></returns>
		public TModel FirstOrDefault()
		{
			ReturnType = PipeReturnType.One;
			return FirstOrDefault<TModel>();
		}

		/// <summary>
		/// 插入数据库并返回数据
		/// </summary>
		/// <returns></returns>
		public Task<TModel> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
		{
			ReturnType = PipeReturnType.One;
			return base.FirstOrDefaultAsync<TModel>(cancellationToken);
		}

		#region Override
		public override string ToString() => base.ToString();

		public override string GetCommandText()
		{
			if (!_upsertSets.Any())
				throw new ArgumentNullException(nameof(_upsertSets));

			return DbConverter.GetUpsertCommandText(MainTable, _primaryKeys, _identityKeys, _upsertSets, ReturnType == PipeReturnType.One);

		}
		#endregion
	}
}
