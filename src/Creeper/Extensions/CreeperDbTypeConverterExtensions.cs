using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Extensions
{
	public static class CreeperDbTypeConverterExtensions
	{
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
