using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Data.Common;
using Npgsql;
using Creeper.PostgreSql.Extensions;
using Creeper.PostgreSql;

namespace Creeper.PostgreSql.XUnitTest.Entity.Options
{
	#region PostgreSql
	public class PostgreSqlDbContext : CreeperDbContextBase
	{
		public PostgreSqlDbContext(IServiceProvider serviceProvider) : base(serviceProvider) { }

		public override DataBaseKind DataBaseKind => DataBaseKind.PostgreSql;

		public override string Name => nameof(PostgreSqlDbContext);

		protected override Action<DbConnection> DbConnectionOptions => connection =>
		{
			var c = (NpgsqlConnection)connection; 
			c.TypeMapper.UseNewtonsoftJson();
			c.TypeMapper.UseSystemXmlDocument();
			c.TypeMapper.UseLegacyPostgis();
			c.TypeMapper.MapEnum<Model.EtDataState>("public.et_data_state", PostgreSqlTranslator.Instance);
			c.TypeMapper.MapComposite<Model.Info>("public.info");
		};
	}
	#endregion

}
