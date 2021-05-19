using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generator.Common;
using Creeper.Generator.Common.Extensions;
using Creeper.Generator.Common.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Creeper.MySql.Generator
{
	/// <summary>
	/// 
	/// </summary>
	public class MySqlDbOptionsGenerator
	{

		private readonly ICreeperDbExecute _dbExecute;
		private readonly MySqlGeneratorRules _mySqlRules;
		private readonly CreeperGeneratorGlobalOptions _options;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dbExecute"></param>
		/// <param name="mySqlRules"></param>
		/// <param name="options"></param>
		public MySqlDbOptionsGenerator(ICreeperDbExecute dbExecute, MySqlGeneratorRules mySqlRules, CreeperGeneratorGlobalOptions options)
		{
			_dbExecute = dbExecute;
			_mySqlRules = mySqlRules;
			_options = options;
		}

		/// <summary>
		/// 生成枚举数据库枚举类型(覆盖生成)
		/// </summary>
		public void Generate()
		{
			InitDbNamesFile();
			GenerateEnum();
			UpdateDbNamesFile();
		}

		private static readonly Regex _regex = new Regex(@"^[0-9]");
		private void GenerateEnum()
		{
			using var connection = _dbExecute.ConnectionOptions.GetConnection();
			var db = connection.Database;
			var sql = $@"
SELECT 
	`TABLE_NAME` AS `tablename`,
	`COLUMN_NAME` AS `name`,
	`DATA_TYPE` AS `dbdatatype`,
	`COLUMN_TYPE` AS `columntype`
FROM `INFORMATION_SCHEMA`.`COLUMNS` 
WHERE `TABLE_SCHEMA`='{db}' and `DATA_TYPE` = 'enum' ORDER BY `ORDINAL_POSITION`

		";
			var list = _dbExecute.ExecuteDataReaderList<EnumTypeInfo>(sql);
			if (list.Count == 0)
				return;

			using (StreamWriter writer = new(File.Create(_options.GetMultipleEnumCsFullName(_dbExecute.ConnectionOptions.DbName)), Encoding.UTF8))
			{
				CreeperGenerator.WriteAuthorHeader.Invoke(writer);
				writer.WriteLine("using System;");
				writer.WriteLine();
				writer.WriteLine("namespace {0}", _options.GetModelNamespaceFullName(_dbExecute.ConnectionOptions.DbName));
				writer.WriteLine("{");
				foreach (var item in list)
				{
					if (!item.ColumnType.StartsWith("enum(", StringComparison.OrdinalIgnoreCase))
						continue;
					var elementStr = item.ColumnType.Replace("enum(", "").Replace(")", "");

					var elist = new List<string>();
					foreach (var e in elementStr.Split(','))
					{
						var element = e.Trim('\'');
						if (_regex.IsMatch(element))
							throw new NotSupportedException("暂不支持起始为数字的枚举成员: " + item.TableName + '.' + item.Name);
						elist.Add(element);
					}
					item.Elements = elist.ToArray();
					if (item.Elements.Length == 0)
						continue;

					item.Elements[0] += " = 1";

					writer.WriteLine("\tpublic enum {0}", GenerateHelper.ExceptUnderlineToUpper(item.TableName) + GenerateHelper.ExceptUnderlineToUpper(item.Name));
					writer.WriteLine("\t{");
					writer.WriteLine($"\t\t{string.Join(", ", item.Elements)}");
					writer.WriteLine("\t}");

				}
				writer.WriteLine("}");
			}
		}

		public void UpdateDbNamesFile()
		{
			var fileName = _options.GetDbNamesFileFullName(Generic.DataBaseKind.MySql);
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


		public void InitDbNamesFile()
		{
			var fileName = _options.GetDbNamesFileFullName(Generic.DataBaseKind.MySql);
			using var writer = new StreamWriter(File.Create(fileName), Encoding.UTF8);
			writer.WriteLine("using Creeper.Driver;");
			writer.WriteLine();
			writer.WriteLine("namespace {0}", _options.OptionsNamespace);
			writer.WriteLine("{");
			writer.WriteLine("}"); // namespace end
		}
	}
}