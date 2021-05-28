using Creeper.DbHelper;
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
		public List<CreeperGenerateConnection> Builders { get; set; } = new List<CreeperGenerateConnection>();
	}
	public class CreeperGenerateConnection
	{
		public string Name { get; set; }
		public ICreeperDbConnection Connection { get; set; }
		public ICreeperDbExecute DbExecute => new CreeperDbExecute(Connection);
	}
}
