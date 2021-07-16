using Creeper.Generic;
using Creeper.SqlBuilder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace Creeper.Driver
{
	public interface ICreeperDbConverter
	{
		/// <summary>
		/// 数据库种类
		/// </summary>
		DataBaseKind DataBaseKind { get; }

		/// <summary>
		/// 数据库对应的字符串类型
		/// </summary>
		string CastStringDbType { get; }

		/// <summary>
		/// 数据库字符串连接符
		/// </summary>
		string StringConnectWord { get; }

		/// <summary>
		/// 数据库字段标识符号
		/// </summary>
		string DbFieldMark { get; }

		/// <summary>
		/// 转化数据库返回值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value"></param>
		/// <returns></returns>
		T ConvertDbData<T>(object value);

		/// <summary>
		/// 转化数据库返回值
		/// </summary>
		/// <param name="value"></param>
		/// <param name="convertType"></param>
		/// <returns></returns>
		object ConvertDbData(object value, Type convertType);

		/// <summary>
		/// 数据库返回数据转化为可用的实体模型
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="convertType"></param>
		/// <returns></returns>
		object ConvertDataReader(IDataReader reader, Type convertType);

		/// <summary>
		/// 数据库返回数据转化为可用的实体模型
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objReader"></param>
		/// <returns></returns>
		T ConvertDataReader<T>(IDataReader objReader);

		/// <summary>
		/// 把sql语句转化成string, debug的时候使用看结果
		/// </summary>
		/// <param name="sqlBuilder"></param>
		/// <returns></returns>
		string ConvertSqlToString(ISqlBuilder sqlBuilder);

		/// <summary>
		/// 获取dbparameter
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		DbParameter GetDbParameter(string name, object value);

		/// <summary>
		/// 获取dbconnection
		/// </summary>
		/// <param name="connectionString"></param>
		/// <returns></returns>
		DbConnection GetDbConnection(string connectionString);

		/// <summary>
		/// 通过object类型判断设置特别的数据库参数, 如MySqlGeometry类型
		/// </summary>
		/// <param name="format">包含'{0}'的字符串格式</param>
		/// <param name="value">传入值, 传出转换后的值</param>
		/// <returns>是否通过设置</returns>
		bool SetSpecialDbParameter(out string format, ref object value);

		/// <summary>
		/// 自定义输出, 
		/// </summary>
		/// <param name="type">输出类型</param>
		/// <param name="format">包含'{0}'的字符串格式</param>
		/// <returns></returns>
		bool TrySpecialOutput(Type type, out string format);

		/// <summary>
		/// 获取增补sql语句
		/// </summary>
		/// <param name="mainTable">表</param>
		/// <param name="primaryKeys">主键集合</param>
		/// <param name="identityKeys">自增键集合</param>
		/// <param name="upsertSets">需要设置的值</param>
		/// <param name="returning"></param>
		/// <returns></returns>
		string GetUpsertCommandText<TModel>(string mainTable, IList<string> primaryKeys, IList<string> identityKeys, IDictionary<string, string> upsertSets, bool returning) where TModel : class, ICreeperDbModel, new();

		/// <summary>
		/// 获取更新sql语句
		/// </summary>
		/// <param name="mainTable"></param>
		/// <param name="mainAlias"></param>
		/// <param name="setList"></param>
		/// <param name="whereList"></param>
		/// <param name="returning"></param>
		/// <param name="pks"></param>
		/// <returns></returns>

		string GetUpdateCommandText<TModel>(string mainTable, string mainAlias, List<string> setList, List<string> whereList, bool returning, string[] pks) where TModel : class, ICreeperDbModel, new();

		/// <summary>
		/// 获取insert sql语句
		/// </summary>
		/// <param name="mainTable"></param>
		/// <param name="insertKeyValuePairs"></param>
		/// <param name="wheres"></param>
		/// <param name="returning"></param>
		/// <returns></returns>
		string GetInsertCommandText<TModel>(string mainTable, Dictionary<string, string> insertKeyValuePairs, string[] wheres, bool returning) where TModel : class, ICreeperDbModel, new();
	}
}
