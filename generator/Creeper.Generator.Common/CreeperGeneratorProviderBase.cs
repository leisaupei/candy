using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Creeper.Generator.Common
{
	public abstract class CreeperGeneratorProviderBase : ICreeperGeneratorProvider
	{
		public abstract DataBaseKind DataBaseKind { get; }

		public abstract ICreeperDbConnectionOption GetDbConnectionOptionFromString(string conn);

		public abstract Action GetFinallyGen();

		public virtual void ModelGenerator(string modelPath, CreeperGenerateOption option, ICreeperDbConnectionOption dbOption, bool folder = false)
		{
			if (folder) modelPath = Path.Combine(modelPath, dbOption.DbName);

			CreeperGenerator.RecreateDir(modelPath);

			ICreeperDbExecute execute = new CreeperDbExecute(dbOption);
			Generate(modelPath, option, folder, execute);
		}
		public abstract void Generate(string modelPath, CreeperGenerateOption option, bool folder, ICreeperDbExecute execute);
	}
}
