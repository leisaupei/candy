namespace Creeper.Driver
{
	public interface ICreeperDbConnectionOption
    {
        /// <summary>
        /// 主库
        /// </summary>
        ICreeperDbConnection Main { get; }

        /// <summary>
        /// 从库
        /// </summary>
        ICreeperDbConnection[] Secondary { get; }

    }
}
