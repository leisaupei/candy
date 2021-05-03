﻿using System;
using System.ComponentModel;

namespace Creeper.Generic
{
	/// <summary>
	/// 联表方式
	/// </summary>
	public enum UnionEnum
	{
		INNER_JOIN = 1, LEFT_JOIN, RIGHT_JOIN, LEFT_OUTER_JOIN, RIGHT_OUTER_JOIN
	}

	/// <summary>
	/// 管道返回值类型
	/// </summary>
	public enum PipeReturnType
	{
		/// <summary>
		/// 返回第一项
		/// </summary>
		One = 1,
		/// <summary>
		/// 列表
		/// </summary>
		List,
		/// <summary>
		/// 受影响行数
		/// </summary>
		Rows
	}

	/// <summary>
	/// 数据库类型
	/// </summary>
	public enum DataBaseType
	{
		/// <summary>
		/// 主库
		/// </summary>
		Main = 1,
		/// <summary>
		/// 从库
		/// </summary>
		Secondary
	}

	/// <summary>
	/// 缓存策略, 仅支持FirstOrDefault与ToScalar方法
	/// </summary>
	internal enum DbCacheType
	{
		/// <summary>
		/// 不使用数据库缓存
		/// </summary>
		None = 0,

		/// <summary>
		/// 默认缓存策略, 面对所有使用
		/// </summary>
		Default = 1,

		/// <summary>
		/// 主键缓存策略, 面对所有使用, 暂时不支持
		/// </summary>
		PkCache = 2,
	}

	/// <summary>
	/// 数据库主从使用策略
	/// </summary>
	public enum DataBaseTypeStrategy
	{
		/// <summary>
		/// 从库优先, 没有从库会报错
		/// </summary>
		SecondaryFirst = 1,
		/// <summary>
		/// 从库优先, 如果从库是Empty自动使用主库
		/// </summary>
		SecondaryFirstOfMainIfEmpty = 2,
		/// <summary>
		/// 只使用主库
		/// </summary>
		OnlyMain = 3,
	}

	/// <summary>
	/// 数据库种类
	/// </summary>
	public enum DataBaseKind
	{
		/// <summary>
		/// Future
		/// </summary>
		SqlServer = 1,

		/// <summary>
		/// Future
		/// </summary>
		Access = 2,

		/// <summary>
		/// Future
		/// </summary>
		MySql = 3,

		/// <summary>
		/// Future
		/// </summary>
		Oracle = 4,

		PostgreSql = 5,

		/// <summary>
		/// Future
		/// </summary>
		Sqlite = 6,
	}

	/// <summary>
	/// 参照的分割字段类型
	/// </summary>
	public enum SplitType
	{
		/// <summary>
		/// 时间类型 从1970-1-1 开始按照N年分割
		/// </summary>
		DateTimeEveryYears = 2,
		/// <summary>
		/// 时间类型 从1970-1-1 开始按照N个月分割
		/// </summary>
		DateTimeEveryMonths = 3,
		/// <summary>
		/// 时间类型 从1970-1-1 开始按照N月分割
		/// </summary>
		DateTimeEveryDays = 4,
		/// <summary>
		/// int类型 每个int一个表
		/// </summary>
		IntEveryValue = 11,
		/// <summary>
		/// int类型 多个int类型一个表
		/// </summary>
		IntEveryValues = 12,
		/// <summary>
		/// 枚举 每个枚举类型一个表
		/// </summary>
		EnumEveryValue = 21,
		/// <summary>
		/// 枚举 多个枚举类型一个表
		/// </summary>
		EnumEveryValues = 22,
		/// <summary>
		/// Guid 按照首字母分表
		/// </summary>
		UuidEveryFirstLetter = 31
	}

}
