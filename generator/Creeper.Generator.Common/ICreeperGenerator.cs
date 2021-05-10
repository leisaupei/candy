using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common
{
	public interface ICreeperGenerator
	{
		/// <summary>
		/// 运行数据库生成逻辑
		/// </summary>
		/// <param name="option"></param>
		void Gen(CreeperGeneratorBuilder option);
	}
}
