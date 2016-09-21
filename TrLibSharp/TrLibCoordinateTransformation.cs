using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace TrLibSharp
{
  public class TrLibCoordinateTransformation : ICoordinateTransformation
  {
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

      IntPtr tr = IntPtr.Zero;
      TrLib.TR_Error error;
      double oX, oY, oZ;

      TrLib.InitLibrary(@"TRLIB/geoids");

      tr = TrLib.TR_Open(fromMiniLabel, toMiniLabel, "");

      if (tr == IntPtr.Zero)
      {
        throw new TrLibException(TrLibException.ErrorTypes.Initialization);
      }

      try
      {
        TrLib.SetThreadMode(false);
        error = TrLib.TR_TransformPoint(tr, from.X, from.Y, from.Z, out oX, out oY, out oZ);
      }
      finally
      {
        TrLib.TR_Close(tr);
      }

      switch (error)
      {
        case TrLib.TR_Error.TR_LABEL_ERROR:
          throw new TrLibException(TrLibException.ErrorTypes.Label);
        case TrLib.TR_Error.TR_TRANSFORMATION_ERROR:
          throw new TrLibException(TrLibException.ErrorTypes.Transformation);
        case TrLib.TR_Error.TR_ALLOCATION_ERROR:
          throw new TrLibException(TrLibException.ErrorTypes.Allocation);
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

    private static Dictionary<int, string> epsgIdToMiniLabelMap = new Dictionary<int, string>()
    {
      { 25832, "utm32_etrs89" },
      { 34005, "s34s" }
    };

    public static class TrLib
    {
      const string TRLIB = @"TRLIB\trlib64bit.dll";

      public enum TR_Error
      {
        TR_OK = 0,  // No Error
        TR_LABEL_ERROR = 1, // Invalid input or output label
        TR_TRANSFORMATION_ERROR = 2,  // Transformation failed
        TR_ALLOCATION_ERROR = 3 //Memory allocation failed


      };

      [DllImport(TRLIB)]
      private static extern TR_Error TR_InitLibrary(string folder);

      public static TR_Error InitLibrary(string folder)
      {
        TR_Error err = TR_InitLibrary(folder);
        return err;
      }

      [DllImport(TRLIB)]
      private static extern void TR_AllowUnsafeTransformations();

      [DllImport(TRLIB)]
      private static extern void TR_ForbidUnsafeTransformations();

      public static void AllowUnsafeTransformations()
      {
        TR_AllowUnsafeTransformations();
      }

      public static void ForbidUnsafeTransformations()
      {
        TR_ForbidUnsafeTransformations();
      }

      public static void SetThreadMode(bool on)
      {
        if (on)
          ForbidUnsafeTransformations();
        else
          AllowUnsafeTransformations();
      }

      [DllImport(TRLIB)]
      public static extern IntPtr TR_Open(string mlb1, string mlb2, string geoid_name);

      [DllImport(TRLIB)]
      public static extern void TR_Close(IntPtr tr);

      [DllImport(TRLIB)]
      public static extern TR_Error TR_TransformPoint(
        IntPtr TR,
        double x,
        double y,
        double z,
        out double x_o,
        out double y_o,
        out double z_o);
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

    public TrLibException(ErrorTypes errorType) :
      base($"{messageErrorTypeMap[errorType]}")
    {
      ErrorType = errorType;
    }

    private static Dictionary<ErrorTypes, string> messageErrorTypeMap = new Dictionary<ErrorTypes, string>()
    {
      { ErrorTypes.Initialization, "TRLIB was unable to initialize" },
      { ErrorTypes.Allocation, "TRLIB encountered a memory allocation error" },
      { ErrorTypes.Allocation, "TRLIB encountered an error during transformation" },
      { ErrorTypes.Label, "TRLIB does not recognize input or output label" }
    };
  }
}
