using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Creeper.MySql.Types
{
	public class MySqlGeometryCollection : MySqlGeometry, IEquatable<MySqlGeometryCollection>, IEnumerable<MySqlGeometry>
	{
		internal override MySqlGeometryType GeometryType => MySqlGeometryType.GeometryCollection;

		readonly MySqlGeometry[] _geometries;

		public MySqlGeometry this[int index] => _geometries[index];

		public IEnumerator<MySqlGeometry> GetEnumerator() => ((IEnumerable<MySqlGeometry>)_geometries).GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public MySqlGeometryCollection(MySqlGeometry[] geometries) => _geometries = geometries;

		public MySqlGeometryCollection(IEnumerable<MySqlGeometry> geometries) => _geometries = geometries.ToArray();

		public bool Equals(MySqlGeometryCollection other)
		{
			if (other is null || _geometries.Length != other._geometries.Length)
				return false;
			for (var i = 0; i < _geometries.Length; i++)
				if (!_geometries[i].Equals(other._geometries[i]))
					return false;
			return true;
		}

		public override bool Equals(object obj)
			=> obj is MySqlGeometryCollection collection && Equals(collection);

		public static bool operator ==(MySqlGeometryCollection left, MySqlGeometryCollection right)
			=> left is null ? right is null : left.Equals(right);

		public static bool operator !=(MySqlGeometryCollection left, MySqlGeometryCollection right) => !(left == right);

		public override int GetHashCode()
		{
			return HashCode.Combine(SRID, _geometries);
		}
	}
}
