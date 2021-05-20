using System.Text;

namespace Creeper.MySql.Types
{
	public abstract class MySqlGeometry
	{
		public int? SRID { get; set; }
		internal abstract MySqlGeometryType GeometryType { get; }
		

		//
		// 摘要:
		//     Get value from WKT format SRID=0;POINT (x y) or POINT (x y)
		//
		// 参数:
		//   value:
		//     WKT string format
		public static MySqlGeometry Parse(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}

			if (!value.Contains("SRID") && !value.Contains("POINT(") && !value.Contains("POINT ("))
			{
				throw new FormatException("String does not contain a valid geometry value");
			}

			MySqlGeometry mySqlGeometryValue = new MySqlGeometry(0.0, 0.0);
			TryParse(value, out mySqlGeometryValue);
			return mySqlGeometryValue;
		}

	}
	public enum MySqlGeometryType
	{
		Point = 1,
		LineString = 2,
		Polygon = 3,
		MultiPoint = 4,
		MultiLineString = 5,
		MultiPolygon = 6,
		GeometryCollection = 7
	}
}
