using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Creeper.MySql.Types
{
	public class MySqlMultiPoint : MySqlGeometry, IEnumerable<MySqlPoint>, IEquatable<MySqlMultiPoint>
	{
		readonly MySqlPoint[] _points;

		internal override MySqlGeometryType GeometryType => MySqlGeometryType.MultiPoint;

		public MySqlMultiPoint(IEnumerable<MySqlPoint> points) => _points = points.ToArray();

		public MySqlMultiPoint(MySqlPoint[] points) => _points = points;

		public bool Equals(MySqlMultiPoint other)
		{
			if (other == null || _points.Length != other._points.Length)
				return false;

			for (var i = 0; i < _points.Length; i++)
				if (!_points[i].Equals(other._points[i]))
					return false;

			return true;
		}

		public static bool operator ==(MySqlMultiPoint p1, MySqlMultiPoint p2) => p1 is null ? p2 is null : p1.Equals(p2);

		public static bool operator !=(MySqlMultiPoint p1, MySqlMultiPoint p2) => !(p1 == p2);

		public IEnumerator<MySqlPoint> GetEnumerator() => ((IEnumerable<MySqlPoint>)_points).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public override bool Equals(object obj) => Equals(obj as MySqlPolygon);

		public override int GetHashCode()
		{
			return HashCode.Combine(SRID, _points);
		}
	}
}
