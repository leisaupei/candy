using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generator.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Creeper.PostgreSql.Generator
{
	/// <summary>
	/// 
	/// </summary>
	public class PostgreSqlDbOptionsGenerator
	{
		/// <summary>
		/// 多库名称, 单库忽略
		/// </summary>
		private string _typeName = string.Empty;

		private readonly ICreeperDbExecute _dbExecute;
		private readonly PostgreSqlRules _postgreSqlRules;
		private readonly bool _folder;
		private readonly GeneratorGlobalOptions _options;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dbExecute"></param>
		/// <param name="postgreSqlRules"></param>
		/// <param name="folder">是否多库</param>
		/// <param name="options"></param>
		public PostgreSqlDbOptionsGenerator(ICreeperDbExecute dbExecute, PostgreSqlRules postgreSqlRules, bool folder, GeneratorGlobalOptions options)
		{
			_dbExecute = dbExecute;
			_postgreSqlRules = postgreSqlRules;
			_folder = folder;
			_options = options;
		}

		/// <summary>
		/// 生成枚举数据库枚举类型(覆盖生成)
		/// </summary>
		/// <param name="rootPath">根目录</param>
		/// <param name="modelPath">Model目录</param>
		/// <param name="projectName">项目名称</param>
		/// <param name="typeName">多库标签</param>
		public void Generate(string typeName)
		{
			_typeName = _folder ? typeName : "";

			InitPostgreSqlDbOptionsCs();
			InitPostgreSqlDbNamesCs();

			var enums = GenerateEnum();
			var composites = GenerateComposites();
			UpdatePostgreSqlDbNamesCs();
			UpdatePostgreSqlDbOptionsCs(enums, composites);
		}


		private List<EnumTypeInfo> GenerateEnum()
		{
			var sql = $@"
SELECT a.oid, a.typname, b.nspname, CAST(obj_description(a.oid,'pg_type') AS VARCHAR) AS description 
FROM pg_type a  
INNER JOIN pg_namespace b ON a.typnamespace = b.oid 
WHERE a.typtype='e'  
ORDER BY oid asc  
";
			var list = _dbExecute.ExecuteDataReaderList<EnumTypeInfo>(sql);
			if (list.Count == 0)
				return list;

			using (StreamWriter writer = new StreamWriter(File.Create(_options.EnumCsFullName), Encoding.UTF8))
			{
				CreeperGenerator.WriteAuthorHeader.Invoke(writer);
				writer.WriteLine("using System;");
				writer.WriteLine();
				writer.WriteLine("namespace {0}.{1}.{2}{3}", _options.BaseOptions.ProjectName, _options.DbStandardSuffix, _options.ModelNamespace, NamespaceSuffix);
				writer.WriteLine("{");
				foreach (var item in list)
				{
					var sqlEnums = $@"SELECT enumlabel FROM pg_enum a  WHERE enumtypid=@oid ORDER BY oid asc";
					var enums = _dbExecute.ExecuteDataReaderList<string>(sqlEnums, System.Data.CommandType.Text, new[] { new NpgsqlParameter("oid", item.Oid) });
					if (enums.Count == 0) continue;

					enums[0] += " = 1";
					writer.Write(PostgreSqlTableGenerator.WriteComment(item.Description, 1));
					writer.WriteLine($"\tpublic enum {Types.DeletePublic(item.Nspname, item.Typname)}");
					writer.WriteLine("\t{");
					writer.WriteLine($"\t\t{string.Join(", ", enums)}");
					writer.WriteLine("\t}");

				}
				writer.WriteLine("}");
			}
			return list;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public List<CompositeTypeInfo> GenerateComposites()
		{
			var sql = $@"
SELECT ns.nspname, a.typname as typename, c.attname, d.typname, c.attndims, d.typtype, CAST(obj_description(a.oid,'pg_type') AS VARCHAR) AS description
FROM pg_type a 
INNER JOIN pg_class b on b.reltype = a.oid and b.relkind = 'c'
INNER JOIN pg_attribute c on c.attrelid = b.oid and c.attnum > 0
INNER JOIN pg_type d on d.oid = c.atttypid
INNER JOIN pg_namespace ns on ns.oid = a.typnamespace
LEFT JOIN pg_namespace ns2 on ns2.oid = d.typnamespace
WHERE {GenerateHelper.ExceptConvert("ns.nspname || '.' || a.typname", _postgreSqlRules.Excepts.Global.Composites)}
";
			List<CompositeTypeInfo> composites = _dbExecute.ExecuteDataReaderList<CompositeTypeInfo>(sql);

			if (composites.Count < 0) return composites;

			var group = composites.GroupBy(a => $"{a.Nspname}.{a.Typename}").ToList();
			StringBuilder sb = new StringBuilder();

			foreach (var g in group)
			{
				var structName = g.FirstOrDefault();
				sb.Append(PostgreSqlTableGenerator.WriteComment(structName.Description, 1));
				sb.AppendLine($"\tpublic partial struct {Types.DeletePublic(structName.Nspname, structName.Typename)}");
				sb.AppendLine("\t{");
				foreach (var member in g)
				{
					var isArray = member.Attndims > 0;
					string _type = Types.ConvertPgDbTypeToCSharpType(member.Typtype, member.Typname);
					var _notnull = string.Empty;
					if (_type != "string" && _type != "JToken" && _type != "byte[]" && !isArray && _type != "object" && _type != "IPAdress")
						_notnull = "?";
					string _array = isArray ? "[]" : "";
					var relType = $"{_type}{_notnull}{_array}";

					sb.AppendLine($"\t\tpublic {relType} {member.Attname.ToUpperPascal()} {{ get; set; }}");
				}
				sb.AppendLine("\t}");
			}

			using StreamWriter writer = new StreamWriter(File.Create(_options.CompositesCsFullName), Encoding.UTF8);
			CreeperGenerator.WriteAuthorHeader.Invoke(writer);
			writer.WriteLine("using System;");
			writer.WriteLine("using Newtonsoft.Json;");
			writer.WriteLine();
			writer.WriteLine($"namespace {_projectName}.{CreeperGenerator.DbStandardSuffix}.{CreeperGenerator.Namespace}{NamespaceSuffix}");
			writer.WriteLine("{");
			writer.WriteLine(sb);
			writer.WriteLine("}");

			return composites;
		}

		private string TypeName => _typeName == CreeperGeneratorBaseOptions.MASTER_DATABASE_TYPE_NAME ? "" : _typeName;

		public void UpdatePostgreSqlDbNamesCs()
		{
			var lines = OpenFileAndReadAllLines(_optionsPath, DbNameFileName).ToList();
			var writeLines = new List<string>
			{
				"\t/// <summary>",
				$"\t/// {TypeName}主库",
				"\t/// </summary>",
				$"\tpublic struct Db{_typeName.ToUpperPascal()} : ICreeperDbName {{ }}",
				"\t/// <summary>",
				$"\t/// {TypeName}从库",
				"\t/// </summary>",
				$"\tpublic struct Db{TypeName + CreeperDbContext.SecondarySuffix} : ICreeperDbName {{ }}"
			};
			lines.InsertRange(lines.Count - 1, writeLines);
			File.WriteAllLines(Path.Combine(_optionsPath, DbNameFileName), lines);
		}

		private void UpdatePostgreSqlDbOptionsCs(List<EnumTypeInfo> enums, List<CompositeTypeInfo> composites)
		{
			var lines = OpenFileAndReadAllLines(_optionsPath, DbOptionsFileName).ToList();
			var writeLines = new List<string>();

			writeLines.Add($"\t#region {_typeName}");
			writeLines.Add(string.Format("\tpublic class {0}PostgreSqlDbOption : BasePostgreSqlDbOption<Db{0}, Db{1}>", _typeName.ToUpperPascal(), TypeName + CreeperDbContext.SecondarySuffix));
			writeLines.Add("\t{");
			writeLines.Add(string.Format("\t\tpublic {0}PostgreSqlDbOption(string mainConnectionString, string[] secondaryConnectionStrings) : base(mainConnectionString, secondaryConnectionStrings) {{ }}", _typeName.ToUpperPascal(), TypeName));
			writeLines.Add("\t\tpublic override DbConnectionOptions Options => new DbConnectionOptions()");
			writeLines.Add("\t\t{");
			writeLines.Add("\t\t\tMapAction = conn =>");
			writeLines.Add("\t\t\t{");
			writeLines.Add("\t\t\t\tconn.TypeMapper.UseNewtonsoftJson();");
			if (MappingOptions.XmlTypeName.Contains(_typeName))
				writeLines.Add("\t\t\t\tconn.TypeMapper.UseSystemXmlDocument();");
			if (MappingOptions.GeometryTableTypeName.Contains(_typeName))
				writeLines.Add("\t\t\t\tconn.TypeMapper.UseLegacyPostgis();");
			foreach (var item in enums)
				writeLines.Add($"\t\t\t\tconn.TypeMapper.MapEnum<{CreeperGenerator.Namespace}{NamespaceSuffix}.{Types.DeletePublic(item.Nspname, item.Typname)}>(\"{item.Nspname}.{item.Typname}\", PostgreSqlTranslator.Instance);");
			foreach (var item in composites.GroupBy(a => $"{a.Nspname}.{a.Typename}"))
			{
				var structName = item.FirstOrDefault();
				writeLines.Add($"\t\t\t\tconn.TypeMapper.MapComposite<{CreeperGenerator.Namespace}{NamespaceSuffix}.{Types.DeletePublic(structName.Nspname, structName.Typename)}>(\"{item.Key}\");");
			}
			writeLines.Add("\t\t\t}");
			writeLines.Add("\t\t};");
			writeLines.Add("\t}");
			writeLines.Add("\t#endregion");
			writeLines.Add("");
			lines.Insert(0, $"using {_projectName}.{CreeperGenerator.DbStandardSuffix}.{CreeperGenerator.Namespace}{NamespaceSuffix};");
			lines.InsertRange(lines.Count - 1, writeLines);
			File.WriteAllLines(Path.Combine(_optionsPath, DbOptionsFileName), lines);
		}
		public string[] OpenFileAndReadAllLines(string path, string fileName)
		{
			CreateDirectory(path);
			fileName = Path.Combine(path, fileName);
			return File.ReadAllLines(fileName);

		}

		public void CreateDirectory(string path)
		{
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		public StreamWriter OpenFile(string fileName)
		{
			return new StreamWriter(File.Create(fileName), Encoding.UTF8);
		}

		public void InitPostgreSqlDbOptionsCs()
		{
			var fileName = Path.Combine(_optionsPath, DbOptionsFileName);
			if (File.Exists(fileName))
				File.Delete(fileName);

			using var writer = OpenFile(fileName);
			//CreeperGenerator.WriteAuthorHeader.Invoke(writer);
			//writer.Write(_sbNamespace);
			writer.WriteLine("using System;");
			writer.WriteLine("using Newtonsoft.Json.Linq;");
			writer.WriteLine("using Npgsql.TypeMapping;");
			writer.WriteLine("using Creeper.PostgreSql.Extensions;");
			writer.WriteLine("using Npgsql;");
			writer.WriteLine("using Creeper.PostgreSql;");
			writer.WriteLine("using Creeper.Driver;");
			writer.WriteLine();
			writer.WriteLine($"namespace {_projectName}.{CreeperGenerator.DbStandardSuffix}.Options");
			writer.WriteLine("{");
			writer.WriteLine("}"); // namespace end
		}

		public void InitPostgreSqlDbNamesCs()
		{
			var fileName = Path.Combine(_optionsPath, DbNameFileName);
			if (File.Exists(fileName))
				File.Delete(fileName);

			using var writer = OpenFile(fileName);
			writer.WriteLine("using Creeper.Driver;");
			writer.WriteLine();
			writer.WriteLine($"namespace {_projectName}.{CreeperGenerator.DbStandardSuffix}.Options");
			writer.WriteLine("{");
			writer.WriteLine("}"); // namespace end
		}
	}
}