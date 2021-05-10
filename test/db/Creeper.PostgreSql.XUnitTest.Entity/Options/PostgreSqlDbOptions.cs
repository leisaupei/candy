using Creeper.PostgreSql.XUnitTest.Entity.Model;
using System;
using Newtonsoft.Json.Linq;
using Npgsql.TypeMapping;
using Creeper.PostgreSql.Extensions;
using Npgsql;
using Creeper.PostgreSql;
using Creeper.Driver;

namespace Creeper.PostgreSql.XUnitTest.Entity.Options
{
	#region Main
	public class MainPostgreSqlDbOption : BasePostgreSqlDbOption<DbMain, DbSecondary>
	{
		public MainPostgreSqlDbOption(string mainConnectionString, string[] secondaryConnectionStrings) : base(mainConnectionString, secondaryConnectionStrings) { }
		public override DbConnectionOptions Options => new DbConnectionOptions()
		{
			MapAction = conn =>
			{
				conn.TypeMapper.UseNewtonsoftJson();
				conn.TypeMapper.UseSystemXmlDocument();
				conn.TypeMapper.MapEnum<Model.EtDataState>("public.et_data_state", PostgreSqlTranslator.Instance);
				conn.TypeMapper.MapComposite<Model.Info>("public.info");
			}
		};
	}
	#endregion

}
