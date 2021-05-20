using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Creeper.MySql.Types
{
	public class MySqlMultiPolygon : MySqlGeometry, IEnumerable<MySqlPolygon>, IEquatable<MySqlMultiPolygon>
	{
		internal override MySqlGeometryType GeometryType => MySqlGeometryType.MultiPolygon;

		readonly MySqlPolygon[] _polygons;

		public IEnumerator<MySqlPolygon> GetEnumerator() => ((IEnumerable<MySqlPolygon>)_polygons).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public MySqlMultiPolygon(MySqlPolygon[] polygons)
			=> _polygons = polygons;

		public MySqlMultiPolygon(IEnumerable<MySqlPolygon> polygons)
			=> _polygons = polygons.ToArray();

		public bool Equals(MySqlMultiPolygon other)
		{
			if (other is null || _polygons.Length != other._polygons.Length)
				return false;
			for (var i = 0; i < _polygons.Length; i++)
				if (_polygons[i] != other._polygons[i]) return false;
			return true;
		}

		public override bool Equals(object obj)
			=> obj is MySqlMultiPolygon polygon && Equals(polygon);

		public override int GetHashCode()
		{
			return HashCode.Combine(SRID, _polygons);
		}

		public static bool operator ==(MySqlMultiPolygon left, MySqlMultiPolygon right)
			=> left is null ? right is null : left.Equals(right);

		public static bool operator !=(MySqlMultiPolygon left, MySqlMultiPolygon right) => !(left == right);

	}
}
