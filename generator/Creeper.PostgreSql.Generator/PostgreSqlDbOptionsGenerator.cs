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

		private readonly CreeperGenerateConnection _connection;
		private readonly PostgreSqlGeneratorRules _postgreSqlRules;
		private readonly CreeperGeneratorGlobalOptions _options;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dbExecute"></param>
		/// <param name="postgreSqlRules"></param>
		/// <param name="options"></param>
		public PostgreSqlDbOptionsGenerator(CreeperGenerateConnection connection, PostgreSqlGeneratorRules postgreSqlRules, CreeperGeneratorGlobalOptions options)
		{
			_connection = connection;
			_postgreSqlRules = postgreSqlRules;
			_options = options;
		}

		/// <summary>
		/// 生成枚举数据库枚举类型(覆盖生成)
		/// </summary>
		public void Generate()
		{

			InitDbContextFile();

			var enums = GenerateEnum();
			var composites = GenerateComposites();
			UpdateDbContextFile(enums, composites);
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
			var list = _connection.DbExecute.ExecuteDataReaderList<EnumTypeInfo>(sql);
			if (list.Count == 0)
				return list;

			using (StreamWriter writer = new StreamWriter(File.Create(_options.GetMultipleEnumCsFullName(_connection.Name)), Encoding.UTF8))
			{
				CreeperGenerator.WriteAuthorHeader.Invoke(writer);
				writer.WriteLine("using System;");
				writer.WriteLine();
				writer.WriteLine("namespace {0}", _options.GetModelNamespaceFullName(_connection.Name));
				writer.WriteLine("{");
				foreach (var item in list)
				{
					var sqlEnums = $@"SELECT enumlabel FROM pg_enum a  WHERE enumtypid=@oid ORDER BY oid asc";
					var enums = _connection.DbExecute.ExecuteDataReaderList<string>(sqlEnums, System.Data.CommandType.Text, new[] { new NpgsqlParameter("oid", item.Oid) });
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
			List<CompositeTypeInfo> composites = _connection.DbExecute.ExecuteDataReaderList<CompositeTypeInfo>(sql);

			if (composites.Count == 0) return composites;

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

			using StreamWriter writer = new StreamWriter(File.Create(_options.GetMultipleCompositesCsFullName(_connection.Name)), Encoding.UTF8);
			CreeperGenerator.WriteAuthorHeader.Invoke(writer);
			writer.WriteLine("using System;");
			writer.WriteLine("using Newtonsoft.Json;");
			writer.WriteLine();
			writer.WriteLine("namespace {0}", _options.GetModelNamespaceFullName(_connection.Name));
			writer.WriteLine("{");
			writer.WriteLine(sb);
			writer.WriteLine("}");

			return composites;
		}

		private void UpdateDbContextFile(List<EnumTypeInfo> enums, List<CompositeTypeInfo> composites)
		{
			var fileName = _options.GetDbContextFileFullName(Generic.DataBaseKind.PostgreSql);
			var lines = File.ReadAllLines(fileName).ToList();
			var writeLines = new List<string>();
			var dbMainName = _options.GetDbNameNameMain(_connection.Name);
			var className = dbMainName.TrimStart('D', 'b');

			writeLines.Add($"\t#region {_connection.Name}");
			writeLines.Add($"\tpublic class {_connection.Name}DbContext : CreeperDbContextBase");
			writeLines.Add("\t{");
			writeLines.Add($"\t\tpublic {_connection.Name}DbContext(IServiceProvider serviceProvider) : base(serviceProvider) {{ }}");
			writeLines.Add("");
			writeLines.Add("\t\tpublic override DataBaseKind DataBaseKind => DataBaseKind.PostgreSql;");
			writeLines.Add("");
			writeLines.Add($"\t\tpublic override string Name => nameof({_connection.Name}DbContext);");
			writeLines.Add("");
			writeLines.Add("\t\tprotected override Action<DbConnection> DbConnectionOptions => connection =>");
			writeLines.Add("\t\t{");
			writeLines.Add("\t\t\tvar c = (NpgsqlConnection)connection; ");
			writeLines.Add("\t\t\tc.TypeMapper.UseNewtonsoftJson();");
			if (MappingOptions.XmlTypeName.Contains(_connection.Name))
				writeLines.Add("\t\t\tc.TypeMapper.UseSystemXmlDocument();");
			if (MappingOptions.GeometryTableTypeName.Contains(_connection.Name))
				writeLines.Add("\t\t\tc.TypeMapper.UseLegacyPostgis();");
			foreach (var item in enums)
				writeLines.Add($"\t\t\tc.TypeMapper.MapEnum<{_options.GetMappingNamespaceName(_connection.Name)}.{Types.DeletePublic(item.Nspname, item.Typname)}>(\"{item.Nspname}.{item.Typname}\", PostgreSqlTranslator.Instance);");
			foreach (var item in composites.GroupBy(a => $"{a.Nspname}.{a.Typename}"))
			{
				var structName = item.FirstOrDefault();
				writeLines.Add($"\t\t\tc.TypeMapper.MapComposite<{_options.GetMappingNamespaceName(_connection.Name)}.{Types.DeletePublic(structName.Nspname, structName.Typename)}>(\"{item.Key}\");");
			}
			writeLines.Add("\t\t};");
			writeLines.Add("\t}");
			writeLines.Add("\t#endregion");
			writeLines.Add("");
			lines.InsertRange(lines.Count - 1, writeLines);
			File.WriteAllLines(fileName, lines);
		}

		public void InitDbContextFile()
		{
			var fileName = _options.GetDbContextFileFullName(Generic.DataBaseKind.PostgreSql);
			if (File.Exists(fileName)) return;
			using var writer = new StreamWriter(File.Create(fileName), Encoding.UTF8);
			writer.WriteLine("using Creeper.DbHelper;");
			writer.WriteLine("using Creeper.Driver;");
			writer.WriteLine("using Creeper.Generic;");
			writer.WriteLine("using System;");
			writer.WriteLine("using System.Data.Common;");
			writer.WriteLine("using Npgsql;");
			writer.WriteLine("using Creeper.PostgreSql.Extensions;");
			writer.WriteLine("using Creeper.PostgreSql;");
			writer.WriteLine();
			writer.WriteLine("namespace {0}", _options.OptionsNamespace);
			writer.WriteLine("{");
			writer.WriteLine("}"); // namespace end
		}
	}
}