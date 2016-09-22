using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TrLibSharp
{
  public class TrLibCoordinateTransformation : ICoordinateTransformation
  {
    private readonly string geoidsDirectory;

    public TrLibCoordinateTransformation() :
      this(@"TRLIB/geoids")
    {
    }

    public TrLibCoordinateTransformation(string geoidsDirectory)
    {
      this.geoidsDirectory = geoidsDirectory;
    }

    public Point Transform(Point from, int toEpsgId)
    {
      if (!epsgIdToMiniLabelMap.ContainsKey(from.EpsgId))
      {
        throw new UnsupportedEpsgIdException(from.EpsgId);
      }

      if (!epsgIdToMiniLabelMap.ContainsKey(toEpsgId))
      {
        throw new UnsupportedEpsgIdException(toEpsgId);
      }

      var fromMiniLabel = epsgIdToMiniLabelMap[from.EpsgId];
      var toMiniLabel = epsgIdToMiniLabelMap[toEpsgId];

      if (fromMiniLabel.StartsWith("geo"))
      {
        from.X = DegToRad(from.X);
        from.Y = DegToRad(from.Y);
      }

      IntPtr tr = IntPtr.Zero;
      TrLib.TR_Error error;
      double oX, oY, oZ;

      error = TrLib.InitLibrary(geoidsDirectory);

      if (error != TrLib.TR_Error.TR_OK)
      {
        throw new TrLibException(TrLibException.ErrorTypes.Initialization, TrLib.TR_GetLastError());
      }

      tr = TrLib.TR_Open(fromMiniLabel, toMiniLabel, "");

      if (tr == IntPtr.Zero)
      {
        throw new TrLibException(TrLibException.ErrorTypes.Initialization, TrLib.TR_GetLastError());
      }

      try
      {
        TrLib.TR_AllowUnsafeTransformations();
        error = TrLib.TR_TransformPoint(tr, from.X, from.Y, from.Z, out oX, out oY, out oZ);
      }
      finally
      {
        TrLib.TR_Close(tr);
      }

      if (error != TrLib.TR_Error.TR_OK)
      {
        throw TR_ErrorToTrLibException(error);
      }

      if (toMiniLabel.StartsWith("geo"))
      {
        oX = RadToDeg(oX);
        oY = RadToDeg(oY);
        oZ = RadToDeg(oZ);
      }

      Point to = new Point()
      {
        X = oX,
        Y = oY,
        Z = oZ,
        EpsgId = toEpsgId
      };

      return to;
    }

    private static TrLibException TR_ErrorToTrLibException(TrLib.TR_Error error)
    {
      switch (error)
      {
        case TrLib.TR_Error.TR_LABEL_ERROR:
          return new TrLibException(TrLibException.ErrorTypes.Label, TrLib.TR_GetLastError());
        case TrLib.TR_Error.TR_TRANSFORMATION_ERROR:
          return new TrLibException(TrLibException.ErrorTypes.Transformation, TrLib.TR_GetLastError());
        case TrLib.TR_Error.TR_ALLOCATION_ERROR:
          return new TrLibException(TrLibException.ErrorTypes.Allocation, TrLib.TR_GetLastError());
        default:
          throw new ArgumentException($"{error} is not a known TRLIB error");
      }
    }

    private static Dictionary<int, string> epsgIdToMiniLabelMap = new Dictionary<int, string>()
    {
      { 4326, "geo_wgs84" },
      { 25832, "utm32_etrs89" },
      { 34005, "s34s" }
    };

    private static double DegToRad(double angle)
    {
      return Math.PI * angle / 180.0;
    }

    private static double RadToDeg(double angle)
    {
      return angle * (180.0 / Math.PI);
    }
  }

  class TrLibException : Exception
  {
    public enum ErrorTypes
    {
      Initialization,
      Allocation,
      Transformation,
      Label
    }

    public ErrorTypes ErrorType { get; private set; }

    public int ErrorCode { get; set; }

    public TrLibException(ErrorTypes errorType, int errorCode) :
      base($"{messageErrorTypeMap[errorType]}")
    {
      ErrorType = errorType;
      ErrorCode = errorCode;
    }

    private static Dictionary<ErrorTypes, string> messageErrorTypeMap = new Dictionary<ErrorTypes, string>()
    {
      { ErrorTypes.Initialization, "TRLIB was unable to initialize" },
      { ErrorTypes.Allocation, "TRLIB encountered a memory allocation error" },
      { ErrorTypes.Transformation, "TRLIB encountered an error during transformation" },
      { ErrorTypes.Label, "TRLIB does not recognize input or output label" }
    };
  }
}
