﻿using Creeper.Driver;
using Creeper.Generic;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Creeper.Generator.Common
{
	public class CreeperGenerator : ICreeperGenerator
	{
		private readonly ICreeperGeneratorProviderFactory _generatorProviderFactory;
		private readonly IConfiguration _cfg;
		private string _modelSuffix;
		private string _modelNamespace;
		private string _dbStandardSuffix;

		public static Action<StreamWriter> WriteAuthorHeader { get; private set; }
		public CreeperGenerator(ICreeperGeneratorProviderFactory generatorProviderFactory, IConfiguration cfg)
		{
			_generatorProviderFactory = generatorProviderFactory;
			_cfg = cfg;
			_modelSuffix = _cfg["ModelSuffix"];
			_modelNamespace = _cfg["ModelNamespace"];
			_dbStandardSuffix = _cfg["DbStandardSuffix"];
			WriteAuthorHeader = AuthorWriter;
		}

		public void AuthorWriter(StreamWriter writer)
		{
			if (!Convert.ToBoolean(_cfg["AuthorHeader"])) return;
			writer.WriteLine("/* ################################################################################");
			writer.WriteLine(" * # 此文件由生成器创建或覆盖。see: https://github.com/leisaupei/creeper");
			writer.WriteLine(" * ################################################################################");
			writer.WriteLine(" */");
		}
		public void Gen(CreeperGeneratorBuilder option)
		{
			if (!Directory.Exists(option.OutputPath))
				Directory.CreateDirectory(option.OutputPath);

			var packageReference = option.Connections.GroupBy(a => a.DataBaseKind.ToString()).Select(a => a.Key)
				.Select(a => string.Format("\t\t<PackageReference Include=\"Creeper.{2}\" Version=\"{0}\" />{1}", _cfg["CreeperNugetVersion"], Environment.NewLine, a)).ToList();

			var generateOptions = new GeneratorGlobalOptions(option, _modelNamespace, _dbStandardSuffix, _modelSuffix);
			GenerateCsproj(generateOptions, packageReference);

			CreateSln(generateOptions);

			foreach (var connection in option.Connections)
			{
				_generatorProviderFactory[connection.DataBaseKind].ModelGenerator(generateOptions, connection, option.Connections.Count > 1);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private void GenerateCsproj(GeneratorGlobalOptions options, List<string> packageReference)
		{
			if (File.Exists(options.CsProjFileFullName))
				return;
			using StreamWriter writer = new StreamWriter(File.Create(options.CsProjFileFullName), Encoding.UTF8);
			writer.WriteLine(@"<Project Sdk=""Microsoft.NET.Sdk"">");
			writer.WriteLine();
			writer.WriteLine("\t<PropertyGroup>");
			writer.WriteLine("\t\t<TargetFramework>netstandard2.1</TargetFramework>");
			writer.WriteLine("\t</PropertyGroup>");
			writer.WriteLine();
			writer.WriteLine("\t<ItemGroup>");
			writer.WriteLine("\t\t<PackageReference Include=\"Creeper\" Version=\"{0}\" />", _cfg["CreeperNugetVersion"]);
			foreach (var item in packageReference)
			{
				writer.WriteLine(item);
			}
			writer.WriteLine("\t</ItemGroup>");
			writer.WriteLine();
			writer.WriteLine("</Project>");
		}
		/// <summary>
		/// 创建sln解决方案文件
		/// </summary>
		private void CreateSln(GeneratorGlobalOptions options)
		{
			if (Directory.GetFiles(options.BaseOptions.OutputPath).Any(f => f.Contains(".sln")))
				return;

			if (File.Exists(options.SlnFileFullName)) return;

			using StreamWriter writer = new StreamWriter(File.Create(options.SlnFileFullName), Encoding.UTF8);
			writer.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
			writer.WriteLine("# Visual Studio 15>");
			writer.WriteLine($"VisualStudioVersion = 15.0.26430.13");

			Guid dbId = Guid.NewGuid();
			writer.WriteLine("Project(\"{0}\") = \"{1}.{3}\", \"{1}.{3}\\{1}.{3}.csproj\", \"{2}\"", Guid.NewGuid(), options.BaseOptions.ProjectName, dbId, _dbStandardSuffix);
			writer.WriteLine($"EndProject");

			writer.WriteLine("Global");
			writer.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
			writer.WriteLine("\t\tDebug|Any CPU = Debug|Any CPU");
			writer.WriteLine("\t\tRelease|Any CPU = Release|Any CPU");
			writer.WriteLine("\tEndGlobalSection");

			writer.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");
			writer.WriteLine($"\t\t{dbId}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
			writer.WriteLine($"\t\t{dbId}.Debug|Any CPU.Build.0 = Debug|Any CPU");
			writer.WriteLine($"\t\t{dbId}.Release|Any CPU.ActiveCfg = Release|Any CPU");
			writer.WriteLine($"\t\t{dbId}.Release|Any CPU.Build.0 = Release|Any CPU");
			writer.WriteLine("\tEndGlobalSection");
			writer.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
			writer.WriteLine("\t\tHideSolutionNode = FALSE");
			writer.WriteLine("\tEndGlobalSection");
			writer.WriteLine("EndGlobal");

		}

	}
}
