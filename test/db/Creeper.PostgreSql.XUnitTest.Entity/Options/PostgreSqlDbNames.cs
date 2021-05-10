using Creeper.Driver;

namespace Creeper.PostgreSql.XUnitTest.Entity.Options
{
	/// <summary>
	/// 主库
	/// </summary>
	public struct DbMain : ICreeperDbName { }
	/// <summary>
	/// 从库
	/// </summary>
	public struct DbSecondary : ICreeperDbName { }
}
