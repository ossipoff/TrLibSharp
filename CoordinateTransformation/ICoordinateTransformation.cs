using System;

namespace TrLibSharp
{
  public interface ICoordinateTransformation
  {
    Point Transform(Point from, int toEpsgId);
  }

  public class UnsupportedEpsgIdException : Exception
  {
    public int EpsgId { get; private set; }

    public UnsupportedEpsgIdException(int epsgId) :
      base($"Unsupported EPSG ID {epsgId}")
    {
      EpsgId = epsgId;
    }
  }

  public class Point
  {
    public double X { get; set; }

    public double Y { get; set; }

    public double Z { get; set; }

    public int EpsgId { get; set; }
  }
}
