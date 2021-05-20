using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Creeper.MySql.Types
{
	public class MySqlLineString : MySqlGeometry, IEnumerable<MySqlPoint>, IEquatable<MySqlLineString>
	{
		readonly MySqlPoint[] _points;

		internal override MySqlGeometryType GeometryType => MySqlGeometryType.LineString;


		public IEnumerator<MySqlPoint> GetEnumerator()
			=> ((IEnumerable<MySqlPoint>)_points).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public MySqlLineString(IEnumerable<MySqlPoint> points) => _points = points.ToArray();

		public MySqlLineString(MySqlPoint[] points) => _points = points;

		public int PointCount => _points.Length;

		public bool Equals(MySqlLineString other)
		{
			if (other is null || _points.Length != other._points.Length)
				return false;
			for (var i = 0; i < _points.Length; i++)
				if (!_points[i].Equals(other._points[i]))
					return false;
			return true;
		}

		public override bool Equals(object obj) => Equals(obj as MySqlLineString);

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public static bool operator ==(MySqlLineString left, MySqlLineString right)
			=> left is null ? right is null : left.Equals(right);

		public static bool operator !=(MySqlLineString left, MySqlLineString right) => !(left == right);

	}
}
