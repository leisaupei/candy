using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generator.Common;
using Creeper.Generator.Common.Extensions;
using Creeper.Generator.Common.Options;
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

		private readonly ICreeperDbExecute _dbExecute;
		private readonly PostgreSqlGeneratorRules _postgreSqlRules;
		private readonly CreeperGeneratorGlobalOptions _options;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dbExecute"></param>
		/// <param name="postgreSqlRules"></param>
		/// <param name="options"></param>
		public PostgreSqlDbOptionsGenerator(ICreeperDbExecute dbExecute, PostgreSqlGeneratorRules postgreSqlRules, CreeperGeneratorGlobalOptions options)
		{
			_dbExecute = dbExecute;
			_postgreSqlRules = postgreSqlRules;
			_options = options;
		}

		/// <summary>
		/// 生成枚举数据库枚举类型(覆盖生成)
		/// </summary>
		public void Generate()
		{

			InitDbOptionsFile();
			InitDbNamesFile();

			var enums = GenerateEnum();
			var composites = GenerateComposites();
			UpdateDbNamesFile();
			UpdateDbOptionsFile(enums, composites);
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

			using (StreamWriter writer = new StreamWriter(File.Create(_options.GetMultipleEnumCsFullName(_dbExecute.ConnectionOptions.DbName)), Encoding.UTF8))
			{
				CreeperGenerator.WriteAuthorHeader.Invoke(writer);
				writer.WriteLine("using System;");
				writer.WriteLine();
				writer.WriteLine("namespace {0}", _options.GetModelNamespaceFullName(_dbExecute.ConnectionOptions.DbName));
				writer.WriteLine("{");
				foreach (var item in list)
				{
					var sqlEnums = $@"SELECT enumlabel FROM pg_enum a  WHERE enumtypid=@oid ORDER BY oid asc";
					var enums = _dbExecute.ExecuteDataReaderList<string>(sqlEnums, System.Data.CommandType.Text, new[] { new NpgsqlParameter("oid", item.Oid) });
					if (enums.Count == 0) continue;

					enums[0] += " = 1";
					writer.Write(CreeperGenerator.WriteComment(item.Description, 1));
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
				sb.Append(CreeperGenerator.WriteComment(structName.Description, 1));
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

			using StreamWriter writer = new StreamWriter(File.Create(_options.GetMultipleCompositesCsFullName(_dbExecute.ConnectionOptions.DbName)), Encoding.UTF8);
			CreeperGenerator.WriteAuthorHeader.Invoke(writer);
			writer.WriteLine("using System;");
			writer.WriteLine("using Newtonsoft.Json;");
			writer.WriteLine();
			writer.WriteLine("namespace {0}", _options.GetModelNamespaceFullName(_dbExecute.ConnectionOptions.DbName));
			writer.WriteLine("{");
			writer.WriteLine(sb);
			writer.WriteLine("}");

			return composites;
		}

		public void UpdateDbNamesFile()
		{
			var fileName = _options.GetDbNamesFileFullName(Generic.DataBaseKind.PostgreSql);
			var lines = File.ReadAllLines(fileName).ToList();
			var mainDbName = _options.GetDbNameNameMain(_dbExecute.ConnectionOptions.DbName);
			var secondaryDbName = _options.GetDbNameNameSecondary(_dbExecute.ConnectionOptions.DbName);
			var writeLines = new List<string>
			{
				"\t/// <summary>",
				$"\t/// {mainDbName}主库",
				"\t/// </summary>",
				$"\tpublic struct {mainDbName} : ICreeperDbName {{ }}",
				"\t/// <summary>",
				$"\t/// {mainDbName}从库",
				"\t/// </summary>",
				$"\tpublic struct {secondaryDbName} : ICreeperDbName {{ }}"
			};
			lines.InsertRange(lines.Count - 1, writeLines);
			File.WriteAllLines(fileName, lines);
		}

		private void UpdateDbOptionsFile(List<EnumTypeInfo> enums, List<CompositeTypeInfo> composites)
		{
			var fileName = _options.GetDbOptionsFileFullName(Generic.DataBaseKind.PostgreSql);
			var lines = File.ReadAllLines(fileName).ToList();
			var writeLines = new List<string>();
			var dbMainName = _options.GetDbNameNameMain(_dbExecute.ConnectionOptions.DbName);
			var className = dbMainName.TrimStart('D', 'b');

			writeLines.Add($"\t#region {_dbExecute.ConnectionOptions.DbName}");
			writeLines.Add(string.Format("\tpublic class {0}PostgreSqlDbOption : BasePostgreSqlDbOption<{1}, {2}>", className, dbMainName, _options.GetDbNameNameSecondary(_dbExecute.ConnectionOptions.DbName)));
			writeLines.Add("\t{");
			writeLines.Add(string.Format("\t\tpublic {0}PostgreSqlDbOption(string mainConnectionString, string[] secondaryConnectionStrings) : base(mainConnectionString, secondaryConnectionStrings) {{ }}", className));
			writeLines.Add("\t\tpublic override DbConnectionOptions Options => new DbConnectionOptions()");
			writeLines.Add("\t\t{");
			writeLines.Add("\t\t\tMapAction = conn =>");
			writeLines.Add("\t\t\t{");
			writeLines.Add("\t\t\t\tconn.TypeMapper.UseNewtonsoftJson();");
			if (MappingOptions.XmlTypeName.Contains(_dbExecute.ConnectionOptions.DbName))
				writeLines.Add("\t\t\t\tconn.TypeMapper.UseSystemXmlDocument();");
			if (MappingOptions.GeometryTableTypeName.Contains(_dbExecute.ConnectionOptions.DbName))
				writeLines.Add("\t\t\t\tconn.TypeMapper.UseLegacyPostgis();");
			foreach (var item in enums)
				writeLines.Add($"\t\t\t\tconn.TypeMapper.MapEnum<{_options.GetMappingNamespaceName(_dbExecute.ConnectionOptions.DbName)}.{Types.DeletePublic(item.Nspname, item.Typname)}>(\"{item.Nspname}.{item.Typname}\", PostgreSqlTranslator.Instance);");
			foreach (var item in composites.GroupBy(a => $"{a.Nspname}.{a.Typename}"))
			{
				var structName = item.FirstOrDefault();
				writeLines.Add($"\t\t\t\tconn.TypeMapper.MapComposite<{_options.GetMappingNamespaceName(_dbExecute.ConnectionOptions.DbName)}.{Types.DeletePublic(structName.Nspname, structName.Typename)}>(\"{item.Key}\");");
			}
			writeLines.Add("\t\t\t}");
			writeLines.Add("\t\t};");
			writeLines.Add("\t}");
			writeLines.Add("\t#endregion");
			writeLines.Add("");
			lines.Insert(0, $"using {_options.GetModelNamespaceFullName(_dbExecute.ConnectionOptions.DbName)};");
			lines.InsertRange(lines.Count - 1, writeLines);
			File.WriteAllLines(fileName, lines);
		}

		public void InitDbOptionsFile()
		{
			var fileName = _options.GetDbOptionsFileFullName(Generic.DataBaseKind.PostgreSql);
			using var writer = new StreamWriter(File.Create(fileName), Encoding.UTF8);
			//CreeperGenerator.WriteAuthorHeader.Invoke(writer);
			writer.WriteLine("using System;");
			writer.WriteLine("using Newtonsoft.Json.Linq;");
			writer.WriteLine("using Npgsql.TypeMapping;");
			writer.WriteLine("using Creeper.PostgreSql.Extensions;");
			writer.WriteLine("using Npgsql;");
			writer.WriteLine("using Creeper.PostgreSql;");
			writer.WriteLine("using Creeper.Driver;");
			writer.WriteLine();
			writer.WriteLine("namespace {0}", _options.OptionsNamespace);
			writer.WriteLine("{");
			writer.WriteLine("}"); // namespace end
		}

		public void InitDbNamesFile()
		{
			var fileName = _options.GetDbNamesFileFullName(Generic.DataBaseKind.PostgreSql);
			using var writer = new StreamWriter(File.Create(fileName), Encoding.UTF8);
			writer.WriteLine("using Creeper.Driver;");
			writer.WriteLine();
			writer.WriteLine("namespace {0}", _options.OptionsNamespace);
			writer.WriteLine("{");
			writer.WriteLine("}"); // namespace end
		}
	}
}