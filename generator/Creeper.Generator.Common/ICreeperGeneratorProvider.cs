using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Creeper.Generator.Common
{
    public interface ICreeperGeneratorProvider
    {
        /// <summary>
        /// 数据库种类
        /// </summary>
        /// <value></value>
        DataBaseKind DataBaseKind { get; }

        /// <summary>
        /// 传入数据库连接字符串解析为数据库连接配置
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        ICreeperDbConnectionOption GetDbConnectionOptionFromString(string conn);

        /// <summary>
        /// 数据库表实体类生成器
        /// </summary>
        /// <param name="modelPath">实体类存放目录</param>
        /// <param name="option">生成配置</param>
        /// <param name="dbOption">数据库配置</param>
        /// <param name="folder">是否使用分类文件夹</param>
        void ModelGenerator(string modelPath, CreeperGenerateOption option, ICreeperDbConnectionOption dbOption, bool folder = false);

        /// <summary>
        /// 数据库实体类生成完成后操作
        /// </summary>
        /// <returns></returns>
        Action GetFinallyGen();
    }
}
