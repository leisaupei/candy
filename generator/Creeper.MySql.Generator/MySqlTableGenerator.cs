using Creeper.Driver;
using Creeper.Generator.Common;
using Creeper.Generator.Common.Extensions;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Creeper.MySql.Generator
{
	/// <summary>
	/// 
	/// </summary>
	public class MySqlTableGenerator
	{
		private readonly CreeperGeneratorGlobalOptions _options;

		/// <summary>
		/// 表/视图
		/// </summary>
		private TableViewModel _table;

		private readonly ICreeperDbExecute _dbExecute;
		private readonly FieldIgnore _fieldIgnore;

		/// <summary>
		/// 是不是空间表
		/// </summary>
		private bool _isGeometryTable = false;

		/// <summary>
		/// 是否视图
		/// </summary>
		private bool _isView = false;

		/// <summary>
		/// 字段列表
		/// </summary>
		private List<TableFieldModel> _fieldList = new List<TableFieldModel>();

		/// <summary>
		/// Model名称
		/// </summary>
		private string ModelClassName => _table.Name.ToUpperPascal() + (_isView ? "View" : null) + _options.ModelSuffix;

		private static readonly string[] _notAddQues = { "string", "JToken", "byte[]", "object", "IPAddress", "Dictionary<string, string>", "NpgsqlTsQuery", "NpgsqlTsVector", "BitArray", "PhysicalAddress", "XmlDocument", "PostgisGeometry" };

		/// <summary>
		/// 构建函数
		/// </summary>
		/// <param name="projectName"></param>
		/// <param name="modelPath"></param>
		/// <param name="dalPath"></param>
		/// <param name="schemaName"></param>
		/// <param name="table"></param>
		/// <param name="type"></param>
		public MySqlTableGenerator(ICreeperDbExecute dbExecute, FieldIgnore fieldIgnore, CreeperGeneratorGlobalOptions options)
		{
			_dbExecute = dbExecute;
			_fieldIgnore = fieldIgnore;
			_options = options;
		}

		public void Generate(TableViewModel table)
		{
			_table = table;
			Console.WriteLine($"Generating mysql {_table.Name}...");
			GetFieldList();
			if (table.Type == "view")
				_isView = true;

			ModelGenerator();
		}

		/// <summary>
		/// 获取字段
		/// </summary>
		private void GetFieldList()
		{
			string db = string.Empty;
			using (var connection = _dbExecute.ConnectionOptions.GetConnection())
			{
				db = connection.Database;
			}

			var sql = $@"
SELECT 
	`COLUMN_COMMENT` as `comment`,
	`COLUMN_NAME` AS `name`,
	`IS_NULLABLE` = 'YES' AS `isnullable`,
	`DATA_TYPE` as `dbdatatype`,
	`CHARACTER_MAXIMUM_LENGTH` as `length`,
	`COLUMN_KEY` = 'PRI' as `isprimarykey`
 FROM `INFORMATION_SCHEMA`.`COLUMNS` 
WHERE `TABLE_SCHEMA`='{db}' AND `TABLE_NAME`='{_table.Name}' ORDER BY `ORDINAL_POSITION`
";
			_fieldList = _dbExecute.ExecuteDataReaderList<TableFieldModel>(sql);

			foreach (var f in _fieldList)
			{
				f.RelType = Types.ConvertMySqlDataTypeToCSharpType(f.DbDataType);
			}
		}

		/// <summary>
		/// 生成Model.cs文件
		/// </summary>
		private void ModelGenerator()
		{
			string _filename = Path.Combine(_options.GetMultipleModelPath(_dbExecute.ConnectionOptions.DbName), ModelClassName + ".cs");

			using StreamWriter writer = new StreamWriter(File.Create(_filename), Encoding.UTF8);
			CreeperGenerator.WriteAuthorHeader.Invoke(writer);
			writer.WriteLine(@"using Creeper.Driver;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Creeper.Attributes;
using Creeper.Generic;
using {1};
namespace {0}
{{
{2}	[CreeperDbTable(@""`{5}`"", typeof({4}), DataBaseKind.{6})]
	public partial class {3} : ICreeperDbModel
	{{
		#region Properties",
_options.GetModelNamespaceFullName(_dbExecute.ConnectionOptions.DbName),
_options.OptionsNamespace,
 WriteComment(_table.Description, 1),
ModelClassName,
CreeperGeneratorGlobalOptions.GetDbNameNameMain(_dbExecute.ConnectionOptions.DbName),
_table.Name,
_dbExecute.ConnectionOptions.DataBaseKind.ToString());

			for (int i = 0; i < _fieldList.Count; i++)
			{
				TableFieldModel item = _fieldList[i];

				#region CreeperDbColumn
				var element = new List<string>();
				if (item.IsPrimaryKey)
					element.Add("Primary = true");

				if (item.IsIdentity)
					element.Add("Identity = true");

				#region Ignore
				var fullFieldName = string.Concat(_table.Name, '.', item.Name);
				var ignores = new List<string>();
				if (_fieldIgnore.Insert.Contains(fullFieldName.ToLower()))
					ignores.Add("IgnoreWhen.Insert");
				if (_fieldIgnore.Returning.Contains(fullFieldName))
					ignores.Add("IgnoreWhen.Returning");
				if (ignores.Count > 0)
					element.Add("IgnoreFlags = " + string.Join(" | ", ignores));
				#endregion

				#endregion

				writer.Write(WriteComment(item.Comment, 2));
				if (element.Count > 0)
					writer.WriteLine(@"{1}{1}[CreeperDbColumn({0})]", string.Join(", ", element), '\t');
				writer.WriteLine(@"{2}{2}public {0} {1} {{ get; set; }}", item.RelType, item.NameUpCase, '\t');
				if (i != _fieldList.Count - 1)
					writer.WriteLine();
			}
			writer.WriteLine("\t\t#endregion");
			writer.WriteLine("\t}");
			writer.WriteLine("}");

			writer.Flush();
		}

		#region Private Method
		/// <summary>
		/// 写评论
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="comment"></param>
		public static StringBuilder WriteComment(string comment, int tab)
		{
			var sb = new StringBuilder();
			if (string.IsNullOrWhiteSpace(comment)) return sb;
			var tabStr = string.Empty;
			for (int i = 0; i < tab; i++)
				tabStr += "\t";
			if (comment.Contains("\n"))
			{
				comment = comment.Replace("\r\n", string.Concat(Environment.NewLine, tabStr, "/// "));
			}
			sb.AppendLine(tabStr + "/// <summary>");
			sb.AppendLine(tabStr + $"/// {comment}");
			sb.AppendLine(tabStr + "/// </summary>");
			return sb;
		}

		#endregion
	}
}