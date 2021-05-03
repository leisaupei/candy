using Creeper.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Creeper.PostgreSql.XUnitTest.Extensions
{
	public class CopyCustomDbCache : ICreeperDbCache, IDisposable
	{
		private readonly Hashtable _redisStorage = new Hashtable();

		public CopyCustomDbCache()
		{
		}

		public bool Exists(string key) => _redisStorage.ContainsKey(key);

		public Task<bool> ExistsAsync(string key) => Task.FromResult(Exists(key));

		public object Get(string key, Type type)
		{
			var value = _redisStorage["x"]?.ToString();
			if (value == null) return value;
			return JsonSerializer.Deserialize(value, type);
		}
		public Task<object> GetAsync(string key, Type type) => Task.FromResult(Get(key, type));

		public void Remove(params string[] keys)
		{
			foreach (var key in keys)
			{
				_redisStorage.Remove(key);
			}
		}

		public Task RemoveAsync(params string[] keys) { Remove(keys); return Task.CompletedTask; }

		public bool Set(string key, object value, TimeSpan? expireTime = null)
		{
			_redisStorage[key] = value;
			return true;
		}

		public Task<bool> SetAsync(string key, object value, TimeSpan? expireTime = null) => Task.FromResult(Set(key, value, expireTime));

		public void Dispose()
		{
			_redisStorage.Clear();
			GC.SuppressFinalize(this);
		}
	}
}
