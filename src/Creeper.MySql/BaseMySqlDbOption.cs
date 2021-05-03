using Creeper.Driver;
using System.Data;
using System.Linq;

namespace Creeper.MySql
{
	/// <summary>
	/// db配置
	/// </summary>
	/// <typeparam name="TDbMainName">主库名称</typeparam>
	/// <typeparam name="TDbSecondaryName">从库名称</typeparam>
	public abstract class BaseMySqlDbOption<TDbMainName, TDbSecondaryName> : ICreeperDbOption
		where TDbMainName : struct, ICreeperDbName
		where TDbSecondaryName : struct, ICreeperDbName
	{
		private readonly string _mainConnectionString;
		private readonly string[] _secondaryConnectionStrings;

		public BaseMySqlDbOption(string mainConnectionString, string[] secondaryConnectionStrings)
		{
			_mainConnectionString = mainConnectionString;
			_secondaryConnectionStrings = secondaryConnectionStrings;

		}

		/// <summary>
		/// 主库对象
		/// </summary>
		ICreeperDbConnectionOption ICreeperDbOption.Main =>
			new MySqlDbConnectionOption(_mainConnectionString, typeof(TDbMainName).Name);

		/// <summary>
		/// 从库数组对象
		/// </summary>
		ICreeperDbConnectionOption[] ICreeperDbOption.Secondary
			=> _secondaryConnectionStrings?.Select(f => new MySqlDbConnectionOption(f, typeof(TDbSecondaryName).Name)).ToArray()
			?? new MySqlDbConnectionOption[0];
	}
}
