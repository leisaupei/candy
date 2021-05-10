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

		public virtual void ModelGenerator(GeneratorGlobalOptions options, ICreeperDbConnectionOption dbOption, bool folder = false)
		{
			var modelPath = options.ModelPath;
			if (folder)
			{
				modelPath = Path.Combine(options.ModelPath, dbOption.DbName);
			}
			GeneratorGlobalOptions.RecreateDir(modelPath);

			ICreeperDbExecute execute = new CreeperDbExecute(dbOption);
			Generate(options, execute);
		}
		public abstract void Generate(GeneratorGlobalOptions options, ICreeperDbExecute execute);
	}
}
