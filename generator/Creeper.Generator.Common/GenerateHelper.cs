﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common
{
	public class GenerateHelper
	{
		/// <summary>
		/// 排除字符串转换器, 匹配字符串: '*','%'
		/// </summary>
		/// <param name="column"></param>
		/// <param name="excepts"></param>
		/// <returns></returns>
		public static string ExceptConvert(string column, string[] excepts)
		{
			var exceptPattern = new List<string>();
			var exceptEqual = new List<string>();
			Array.ForEach(excepts, e =>
			{
				if (e.Contains('%'))
					exceptPattern.Add($"lower({column}) NOT LIKE '{e.ToLower()}'");
				else if (e.Contains('*'))
					exceptPattern.Add($"lower({column}) NOT LIKE '{e.Replace('*', '%').ToLower()}'");
				else
					exceptEqual.Add($"'{e.ToLower()}'");
			});
			var wheres = new List<string>();
			if (exceptEqual.Count > 0)
				wheres.Add($"lower({column}) NOT IN ({string.Join(", ", exceptEqual)})");
			if (exceptPattern.Count > 0)
				wheres.Add(string.Join(" AND ", exceptPattern));

			if (wheres.Count > 0)
				return $"({string.Join(" AND ", wheres)})";
			else
				return " 1=1 ";
		}

		/// <summary>
		/// 首字母大写
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string ToUpperPascal(string s) => string.IsNullOrEmpty(s) ? s : $"{s[0..1].ToUpper()}{s[1..]}";
	}
}
