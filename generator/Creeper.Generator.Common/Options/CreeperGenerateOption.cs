using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common.Options
{
	/// <summary>
	/// 传入生成配置
	/// </summary>
	public class CreeperGenerateOption
	{
		/// <summary>
		/// 默认主库名称
		/// </summary>
		public const string MASTER_DATABASE_TYPE_NAME = "Main";
		/// <summary>
		/// 项目名称
		/// </summary>
		public string ProjectName { get; set; }
		/// <summary>
		/// 输出路径
		/// </summary>
		public string OutputPath { get; set; }
		/// <summary>
		/// 数据库(多库字段)
		/// </summary>
		public bool Sln { get; set; } = true;
		/// <summary>
		/// 字符串连接
		/// </summary>
		public List<ICreeperDbConnectionOption> Connections { get; set; } = new List<ICreeperDbConnectionOption>();
	}
}
