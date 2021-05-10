using Creeper.Driver;

namespace Creeper.PostgreSql.XUnitTest.Entity.Options
{
	/// <summary>
	/// DbMain主库
	/// </summary>
	public struct DbMain : ICreeperDbName { }
	/// <summary>
	/// DbSecondary从库
	/// </summary>
	public struct DbSecondary : ICreeperDbName { }
}
