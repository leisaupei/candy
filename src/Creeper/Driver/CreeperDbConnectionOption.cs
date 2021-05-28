namespace Creeper.Driver
{
	public class CreeperDbConnectionOption : ICreeperDbConnectionOption
    {
        public CreeperDbConnectionOption(CreeperDbConnection main, CreeperDbConnection[] secondary)
        {
            Main = main;
            Secondary = secondary;
        }

        /// <summary>
        /// 主库
        /// </summary>
        public ICreeperDbConnection Main { get; }

        /// <summary>
        /// 从库
        /// </summary>
        public ICreeperDbConnection[] Secondary { get; }

    }
}
