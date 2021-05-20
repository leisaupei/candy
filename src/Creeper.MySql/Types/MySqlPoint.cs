using System;
using System.Globalization;

namespace Creeper.MySql.Types
{
	public class MySqlPoint : MySqlGeometry, IEquatable<MySqlPoint>
	{
		public double X { get; }

		public double Y { get; }

		internal override MySqlGeometryType GeometryType => MySqlGeometryType.Point;

		public MySqlPoint(double x, double y) { X = x; Y = y; }

		public bool Equals(MySqlPoint p)
			=> X == p.X && Y == p.Y;

		public override bool Equals(object obj) => obj is MySqlPoint p && Equals(p);

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y);
		}
		public override string ToString()
		{
			if (SRID == 0 || SRID == null)
			{
				return string.Format(CultureInfo.InvariantCulture.NumberFormat, "POINT({0} {1})", X, Y);
			}

			return string.Format(CultureInfo.InvariantCulture.NumberFormat, "SRID={2};POINT({0} {1})", X, Y, SRID);
		}
		public static bool operator ==(MySqlPoint left, MySqlPoint right)
			=> Equals(left, right);

		public static bool operator !=(MySqlPoint left, MySqlPoint right)
			=> !Equals(left, right);
	}
}
