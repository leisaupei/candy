﻿using System;
using Creeper.Generic;
namespace Creeper.Attributes
{
    /// <summary>
    /// 数据库表特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CreeperDbTableAttribute : Attribute
    {
        private readonly Type _dbName;
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; }
        public string DbName => _dbName.Name;
        public DataBaseKind DbKind { get; set; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbName"></param>
        public CreeperDbTableAttribute(string tableName, Type dbName, DataBaseKind dbKind)
        {
            TableName = tableName;
            _dbName = dbName;
			DbKind = dbKind;
        }
    }
}
