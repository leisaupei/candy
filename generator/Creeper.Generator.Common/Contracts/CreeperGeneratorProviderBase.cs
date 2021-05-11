using Creeper.DbHelper;
using Creeper.Driver;
using Creeper.Generator.Common.Options;
using Creeper.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Creeper.Generator.Common.Contracts
{
	public abstract class CreeperGeneratorProviderBase : ICreeperGeneratorProvider
	{
		public abstract DataBaseKind DataBaseKind { get; }

		public abstract ICreeperDbConnectionOption GetDbConnectionOptionFromString(string conn);

		public virtual void ModelGenerator(CreeperGeneratorGlobalOptions options, ICreeperDbConnectionOption dbOption, bool folder = false)
		{
			var modelPath = options.ModelPath;
			if (folder)
			{
				modelPath = Path.Combine(options.ModelPath, dbOption.DbName);
			}
			CreeperGeneratorGlobalOptions.RecreateDir(modelPath);

			ICreeperDbExecute execute = new CreeperDbExecute(dbOption);
			Generate(options, execute);
		}
		public abstract void Generate(CreeperGeneratorGlobalOptions options, ICreeperDbExecute execute);
	}
}
