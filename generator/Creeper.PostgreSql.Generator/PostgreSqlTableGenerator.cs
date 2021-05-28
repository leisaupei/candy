using Creeper.Driver;
using Creeper.Generator.Common;
using Creeper.Generator.Common.Models;
using Creeper.Generator.Common.Options;
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
	public class PostgreSqlTableGenerator
	{
		private readonly CreeperGeneratorGlobalOptions _options;
		/// <summary>
		/// schema 名称
		/// </summary>
		private string _schemaName;

		/// <summary>
		/// 表/视图
		/// </summary>
		private TableViewModel _table;

		private readonly CreeperGenerateConnection _connection;
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
		private string ModelClassName => DalClassName + _options.ModelSuffix;

		/// <summary>
		/// DAL名称
		/// </summary>
		private string DalClassName => Types.DeletePublic(_schemaName, _table.Name, isView: _isView);

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
		public PostgreSqlTableGenerator(CreeperGenerateConnection connection, FieldIgnore fieldIgnore, CreeperGeneratorGlobalOptions options)
		{
			_connection = connection;
			_fieldIgnore = fieldIgnore;
			_options = options;
		}

		public void Generate(string schemaName, TableViewModel table)
		{
			_schemaName = schemaName;
			_table = table;
			Console.WriteLine($"Generating postgresql {_schemaName}.{_table.Name}...");
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
			var sql = $@"
SELECT 
	c.attname AS name,
	c.attnotnull AS isnotnull, 
	d.description AS comment, 
	e.typcategory, 
	(f.is_identity = 'YES') as isidentity, 
	format_type(c.atttypid,c.atttypmod) AS type_comment, 
	c.attndims as dimensions,  
	(CASE WHEN f.character_maximum_length IS NULL THEN c.attlen ELSE f.character_maximum_length END) AS length,  
	(CASE WHEN e.typelem = 0 THEN e.typname WHEN e.typcategory = 'G' THEN format_type (c.atttypid, c.atttypmod) ELSE e2.typname END) AS dbtype,  
	(CASE WHEN e.typelem = 0 THEN e.typtype ELSE e2.typtype END) AS dbdatatype, 
	ns.nspname, 
	COALESCE(pc.contype = 'u',false) as isunique ,
	f.column_default,
	COALESCE(idx.indisprimary, false) as isprimarykey
FROM pg_class a  
INNER JOIN pg_namespace b ON a.relnamespace = b.oid  
INNER JOIN pg_attribute c ON attrelid = a.oid  
LEFT OUTER JOIN pg_description d ON c.attrelid = d.objoid AND c.attnum = d.objsubid AND c.attnum > 0  
INNER JOIN pg_type e ON e.oid = c.atttypid  
LEFT JOIN pg_type e2 ON e2.oid = e.typelem  
INNER JOIN information_schema.COLUMNS f ON f.table_schema = b.nspname AND f.TABLE_NAME = a.relname AND COLUMN_NAME = c.attname  
LEFT JOIN pg_namespace ns ON ns.oid = e.typnamespace and ns.nspname <> 'pg_catalog'  
LEFT JOIN pg_constraint pc ON pc.conrelid = a.oid and pc.conkey[1] = c.attnum and pc.contype = 'u'
LEFT JOIN pg_index idx ON c.attrelid = idx.indrelid AND c.attnum = ANY (idx.indkey)  
WHERE (b.nspname='{_schemaName}' and a.relname='{_table.Name}')  
ORDER BY c.attnum ASC
";
			_fieldList = _connection.DbExecute.ExecuteDataReaderList<TableFieldModel>(sql);

			foreach (var f in _fieldList)
			{
				f.DbType = f.DbType.TrimStart('_');
				f.NpgsqlDbType = Types.ConvertDbTypeToNpgsqlDbType(f.DbDataType, f.DbType, f.IsArray);
				f.CSharpType = Types.ConvertPgDbTypeToCSharpType(f.DbDataType, f.DbType);

				if (f.DbType == "xml")
					MappingOptions.XmlTypeName.Add(_connection.Name);
				if (f.DbType == "geometry")
				{
					_isGeometryTable = true;
					MappingOptions.GeometryTableTypeName.Add(_connection.Name);
				}

				if (f.IsEnum)
					f.CSharpType = Types.DeletePublic(f.Nspname, f.CSharpType);

				if (f.DbDataType == "c")
					f.RelType = Types.DeletePublic(f.Nspname, f.CSharpType);
				else
				{
					string notnull = "";
					if (!_notAddQues.Contains(f.CSharpType) && !f.IsArray)
					{
						notnull = f.IsNotNull ? "" : "?";
					}
					string array = f.IsArray ? "[".PadRight(Math.Max(0, f.Dimensions), ',') + "]" : "";
					f.RelType = $"{f.CSharpType}{notnull}{array}";
				}


				if (f.Column_default?.StartsWith("nextval(") ?? false)
					f.IsIdentity = true;
			}
		}

		/// <summary>
		/// 生成Model.cs文件
		/// </summary>
		private void ModelGenerator()
		{
			string _filename = Path.Combine(_options.GetMultipleModelPath(_connection.Name), ModelClassName + ".cs");

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
using NpgsqlTypes;
using System.Threading.Tasks;
using System.Threading;
using Creeper.Attributes;
using Creeper.Generic;
using {1};
{3}
namespace {0}
{{
{2}	[CreeperDbTable(@""""""{4}"""".""""{6}"""""", DataBaseKind.{7})]
	public partial class {5} : ICreeperDbModel
	{{
		#region Properties",
_options.GetModelNamespaceFullName(_connection.Name),
_options.OptionsNamespace,
CreeperGenerator.WriteComment(_table.Description, 1),
_isGeometryTable ? "using Npgsql.LegacyPostgis;" + Environment.NewLine : "",
_schemaName,
ModelClassName,
_table.Name,
_connection.DbExecute.ConnectionOptions.DataBaseKind.ToString());

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
				var fullFieldName = string.Concat(_schemaName, '.', _table.Name, '.', item.Name);
				var ignores = new List<string>();
				if (_fieldIgnore.Insert.Contains(fullFieldName.ToLower()))
					ignores.Add("IgnoreWhen.Insert");
				if (_fieldIgnore.Returning.Contains(fullFieldName))
					ignores.Add("IgnoreWhen.Returning");
				if (ignores.Count > 0)
					element.Add("IgnoreFlags = " + string.Join(" | ", ignores));
				#endregion

				#endregion

				writer.Write(CreeperGenerator.WriteComment(item.Comment, 2));
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
	}
}