using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Creeper.MySql.Types
{
	public class MySqlPolygon : MySqlGeometry, IEnumerable<MySqlPoint>, IEquatable<MySqlPolygon>
	{
		readonly MySqlPoint[] _points;

		internal override MySqlGeometryType GeometryType => MySqlGeometryType.MultiPolygon;

		public MySqlPolygon(IEnumerable<MySqlPoint> points) => _points = points.ToArray();

		public bool Equals(MySqlPolygon other)
		{
			if (other == null || _points.Length != other._points.Length)
				return false;

			for (var i = 0; i < _points.Length; i++)
				if (!_points[i].Equals(other._points[i]))
					return false;

			return true;
		}

		public static bool operator ==(MySqlPolygon left, MySqlPolygon right) => left is null ? right is null : left.Equals(right);

		public static bool operator !=(MySqlPolygon left, MySqlPolygon right) => !(left == right);

		public IEnumerator<MySqlPoint> GetEnumerator() => ((IEnumerable<MySqlPoint>)_points).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override bool Equals(object obj) => Equals(obj as MySqlPolygon);

		public override int GetHashCode()
		{
			return HashCode.Combine(SRID, _points);
		}
	}
}
