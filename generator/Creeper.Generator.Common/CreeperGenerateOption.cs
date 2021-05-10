using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Creeper.Generator.Common
{
	public class CreeperGeneratorBuilder : CreeperGeneratorBaseOptions
	{
		public List<ICreeperDbConnectionOption> Connections { get; set; } = new List<ICreeperDbConnectionOption>();
	}
	/// <summary>
	/// 传入生成配置
	/// </summary>
	public class CreeperGeneratorBaseOptions
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
	}

	/// <summary>
	/// 项目构建路径配置参数
	/// </summary>
	public class GeneratorGlobalOptions
	{
		/// <summary>
		/// model(build)文件目录
		/// </summary>
		public string ModelPath { get; set; }
		/// <summary>
		/// dboptions文件目录
		/// </summary>
		public string DbOptionsPath { get; set; }
		/// <summary>
		/// db层根目录
		/// </summary>
		public string RootPath { get; set; }
		/// <summary>
		/// 基础配置
		/// </summary>
		public CreeperGeneratorBaseOptions BaseOptions { get; }
		/// <summary>
		/// 项目命名空间
		/// </summary>
		public string ModelNamespace { get; }
		/// <summary>
		/// 类库名称后缀
		/// </summary>
		public string DbStandardSuffix { get; }
		/// <summary>
		/// 类名后缀
		/// </summary>
		public string ModelSuffix { get; }
		/// <summary>
		/// .csproj文件名称
		/// </summary>
		public string CsProjFileName { get; }
		/// <summary>
		/// .csproj文件名称(包含路径)
		/// </summary>
		public string CsProjFileFullName { get; }
		/// <summary>
		/// .sln名称包含路径
		/// </summary>
		public string SlnFileFullName { get; }
		/// <summary>
		/// 枚举文件_Enum.cs全称(包含文件路径)
		/// </summary>
		public string EnumCsFullName { get; }
		/// <summary>
		/// _Composites.cs全称(包含文件路径)
		/// </summary>
		public string CompositesCsFullName { get; }
		/// <summary>
		/// options目录.cs文件命名空间
		/// </summary>
		public string OptionsNamespace { get; }
		private const string DbNameFileName = "DbNames.cs";
		private const string DbOptionsFileName = "DbOptions.cs";
		private const string EnumFileName = "_Enums.cs";
		private const string CompositesFileName = "_Composites.cs";
		private const string DbOptionsFolderName = "Options";
		private const string DbNamePrefix = "Db";

		public GeneratorGlobalOptions(CreeperGeneratorBaseOptions baseOptions, string modelNamespace, string dbStandardSuffix, string modelSuffix)
		{
			BaseOptions = baseOptions;
			ModelNamespace = modelNamespace;
			DbStandardSuffix = dbStandardSuffix;
			ModelSuffix = modelSuffix;

			RootPath = Path.Combine(BaseOptions.OutputPath, BaseOptions.ProjectName + "." + DbStandardSuffix);
			DbOptionsPath = Path.Combine(RootPath, DbOptionsFolderName);
			ModelPath = Path.Combine(RootPath, ModelNamespace, "Build");
			CsProjFileName = string.Format("{0}.{1}.csproj", BaseOptions.ProjectName, DbStandardSuffix);
			CsProjFileFullName = Path.Combine(RootPath, CsProjFileName);
			SlnFileFullName = Path.Combine(BaseOptions.OutputPath, $"{BaseOptions.ProjectName}.sln");
			EnumCsFullName = Path.Combine(ModelPath, EnumFileName);
			CompositesCsFullName = Path.Combine(ModelPath, CompositesFileName);
			OptionsNamespace = string.Format("{0}.{1}.{2}", BaseOptions.ProjectName, DbStandardSuffix, DbOptionsFolderName);
			CreateDir(RootPath);
			RecreateDir(DbOptionsPath);
			RecreateDir(ModelPath);
		}
		public static string GetDbNameNameMain(string typeName = null)
		{
			if (string.IsNullOrEmpty(typeName) || typeName.ToLower() == CreeperGeneratorBaseOptions.MASTER_DATABASE_TYPE_NAME.ToLower())
				return DbNamePrefix + CreeperGeneratorBaseOptions.MASTER_DATABASE_TYPE_NAME;
			return DbNamePrefix + typeName.ToUpperPascal();
		}
		public static string GetDbNameNameSecondary(string typeName = null)
		{
			if (string.IsNullOrEmpty(typeName) || typeName.ToLower() == CreeperGeneratorBaseOptions.MASTER_DATABASE_TYPE_NAME.ToLower())
				return DbNamePrefix + DataBaseType.Secondary.ToString();
			return DbNamePrefix + typeName.ToUpperPascal() + DataBaseType.Secondary.ToString(); ;
		}
		public string GetMappingNamespaceName(string typeName = null)
		{
			var namespaceName = ModelNamespace;
			if (!string.IsNullOrEmpty(typeName) && typeName.ToLower() != CreeperGeneratorBaseOptions.MASTER_DATABASE_TYPE_NAME.ToLower())
				namespaceName += "." + typeName;
			return namespaceName;
		}
		public string GetModelNamespaceFullName(string typeName = null)
		{
			var namespaceName = string.Format("{0}.{1}.{2}", BaseOptions.ProjectName, DbStandardSuffix, ModelNamespace);
			if (!string.IsNullOrEmpty(typeName) && typeName.ToLower() != CreeperGeneratorBaseOptions.MASTER_DATABASE_TYPE_NAME.ToLower())
				namespaceName += "." + typeName;
			return namespaceName;
		}
		public string GetModelPath(string typeName = null)
		{
			var namespaceName = string.Format("{0}.{1}.{2}", BaseOptions.ProjectName, DbStandardSuffix, ModelNamespace);
			if (!string.IsNullOrEmpty(typeName) && typeName.ToLower() != CreeperGeneratorBaseOptions.MASTER_DATABASE_TYPE_NAME.ToLower())
				namespaceName += "." + typeName;
			return namespaceName;
		}
		public static void RecreateDir(string path)
		{
			if (Directory.Exists(path))
				Directory.Delete(path, true);
			Directory.CreateDirectory(path);
		}
		public static void CreateDir(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}
		public string GetDbNamesFileFullName(DataBaseKind dataBaseKind)
		{
			return Path.Combine(DbOptionsPath, dataBaseKind.ToString() + DbNameFileName);
		}
		public string GetDbOptionsFileFullName(DataBaseKind dataBaseKind)
		{
			return Path.Combine(DbOptionsPath, dataBaseKind.ToString() + DbOptionsFileName);
		}
	}
}
