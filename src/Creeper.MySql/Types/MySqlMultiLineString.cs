using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Creeper.MySql.Types
{
	public class MySqlMultiLineString : MySqlGeometry, IEnumerable<MySqlLineString>, IEquatable<MySqlMultiLineString>
	{
		internal override MySqlGeometryType GeometryType => MySqlGeometryType.MultiLineString;

		readonly MySqlLineString[] _lineStrings;

		internal MySqlMultiLineString(MySqlPoint[][] pointArray)
		{
			_lineStrings = new MySqlLineString[pointArray.Length];
			for (var i = 0; i < pointArray.Length; i++)
				_lineStrings[i] = new MySqlLineString(pointArray[i]);
		}

		public IEnumerator<MySqlLineString> GetEnumerator() => ((IEnumerable<MySqlLineString>)_lineStrings).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public MySqlMultiLineString(MySqlLineString[] linestrings)
			=> _lineStrings = linestrings;

		public MySqlMultiLineString(IEnumerable<MySqlLineString> linestrings)
			=> _lineStrings = linestrings.ToArray();


		public MySqlLineString this[int index] => _lineStrings[index];

		public bool Equals(MySqlMultiLineString other)
		{
			if (other is null || _lineStrings.Length != other._lineStrings.Length)
				return false;

			for (var i = 0; i < _lineStrings.Length; i++)
				if (_lineStrings[i] != other._lineStrings[i])
					return false;
			return true;
		}

		public override bool Equals(object obj)
			=> obj is MySqlMultiLineString multiLineString && Equals(multiLineString);

		public static bool operator ==(MySqlMultiLineString x, MySqlMultiLineString y)
			=> x is null ? y is null : x.Equals(y);

		public static bool operator !=(MySqlMultiLineString x, MySqlMultiLineString y) => !(x == y);


		public override int GetHashCode()
		{
			return HashCode.Combine(SRID, _lineStrings);
		}
	}
}
