﻿using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Creeper.Extensions
{
	internal static class Extensions
	{
		/// <summary>
		/// 判断数组为空
		/// </summary>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> value) => !value?.Any() ?? true;

		/// <summary>
		///  将首字母转小写
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string ToLowerPascal(this string s) => string.IsNullOrEmpty(s) ? s : $"{s.Substring(0, 1).ToLower()}{s[1..]}";

		/// <summary>
		/// 类型是否元组
		/// </summary>
		/// <param name="tupleType"></param>
		/// <returns></returns>
		public static bool IsTuple(this Type tupleType) => typeof(ITuple).IsAssignableFrom(tupleType);

		/// <summary>
		/// 当类型是Nullable&lt;T&gt;,则返回T, 否则返回传入类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Type GetOriginalType(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) ? Nullable.GetUnderlyingType(type) : type;

		/// <summary>
		/// 判断object值是不是空值或DBNull
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static bool IsNullOrDBNull(this object obj) => obj is DBNull || obj == null;

		/// <summary>
		/// 判断type是否继承typeof(T)
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsImplementation<T>(this Type type)
		{
			return typeof(T).IsAssignableFrom(type);
		}
		/// <summary>
		/// md5校验
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		public static string GetMD5String(this string content)
		{
			byte[] result = Encoding.UTF8.GetBytes(content);
			using (MD5 md5 = MD5.Create())
				return BitConverter.ToString(md5.ComputeHash(result)).Replace("-", "").ToLower();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="converter"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string WithQuotationMarks(this ICreeperDbConverter converter, string value)
		{
			if (string.IsNullOrWhiteSpace(converter.DbFieldMark)) return value;

			return string.Concat(converter.DbFieldMark, value, converter.DbFieldMark);
		}
	}
}
