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

		public abstract CreeperGenerateConnection GetDbConnectionOptionFromString(string conn);

		public abstract void ModelGenerator(CreeperGeneratorGlobalOptions options, CreeperGenerateConnection connection);
	}
}
