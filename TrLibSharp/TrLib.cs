using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TrLibSharp
{
  static class TrLib
  {
    private static bool is64Bit = IntPtr.Size == 8;

    public enum TR_Error
    {
      TR_OK = 0,  // No Error
      TR_LABEL_ERROR = 1, // Invalid input or output label
      TR_TRANSFORMATION_ERROR = 2,  // Transformation failed
      TR_ALLOCATION_ERROR = 3 //Memory allocation failed
    };

    public static TR_Error InitLibrary(string folder)
    {
      TR_Error err = is64Bit ? PInvoke64Bit.TR_InitLibrary(folder) : PInvoke32Bit.TR_InitLibrary(folder);
      return err;
    }

    public static void TR_ForbidUnsafeTransformations()
    {
      if (is64Bit)
      {
        PInvoke64Bit.TR_ForbidUnsafeTransformations();
      }
      else
      {
        PInvoke32Bit.TR_ForbidUnsafeTransformations();
      }
    }

    public static void TR_AllowUnsafeTransformations()
    {
      if (is64Bit)
      {
        PInvoke64Bit.TR_AllowUnsafeTransformations();
      }
      else
      {
        PInvoke32Bit.TR_AllowUnsafeTransformations();
      }
    }

    public static IntPtr TR_Open(string mlb1, string mlb2, string geoid_name)
    {
      if (is64Bit)
      {
        return PInvoke64Bit.TR_Open(mlb1, mlb2, geoid_name);
      }
      else
      {
        return PInvoke32Bit.TR_Open(mlb1, mlb2, geoid_name);
      }
    }

    public static void TR_Close(IntPtr tr)
    {
      if (is64Bit)
      {
        PInvoke64Bit.TR_Close(tr);
      }
      else
      {
        PInvoke32Bit.TR_Close(tr);
      }
    }

    public static TR_Error TR_TransformPoint(
      IntPtr TR,
      double x,
      double y,
      double z,
      out double x_o,
      out double y_o,
      out double z_o)
    {
      if (is64Bit)
      {
        return PInvoke64Bit.TR_TransformPoint(TR, x, y, z, out x_o, out y_o, out z_o);
      }
      else
      {
        return PInvoke32Bit.TR_TransformPoint(TR, x, y, z, out x_o, out y_o, out z_o);
      }
    }

    public static int TR_GetLastError()
    {
      if (is64Bit)
      {
        return PInvoke64Bit.TR_GetLastError();
      }
      else
      {
        return PInvoke32Bit.TR_GetLastError();
      }
    }

    private static class PInvoke32Bit
    {
      const string TRLIB = @"TRLIB\trlib32bit.dll";

      [DllImport(TRLIB, CallingConvention = CallingConvention.Cdecl)]
      public static extern TR_Error TR_InitLibrary(string folder);

      [DllImport(TRLIB, CallingConvention = CallingConvention.Cdecl)]
      public static extern void TR_AllowUnsafeTransformations();

      [DllImport(TRLIB, CallingConvention = CallingConvention.Cdecl)]
      public static extern void TR_ForbidUnsafeTransformations();

      [DllImport(TRLIB, CallingConvention = CallingConvention.Cdecl)]
      public static extern IntPtr TR_Open(string mlb1, string mlb2, string geoid_name);

      [DllImport(TRLIB, CallingConvention = CallingConvention.Cdecl)]
      public static extern void TR_Close(IntPtr tr);

      [DllImport(TRLIB, CallingConvention = CallingConvention.Cdecl)]
      public static extern TR_Error TR_TransformPoint(
        IntPtr TR,
        double x,
        double y,
        double z,
        out double x_o,
        out double y_o,
        out double z_o);

      [DllImport(TRLIB, CallingConvention = CallingConvention.Cdecl)]
      public static extern int TR_GetLastError();
    }

    private static class PInvoke64Bit
    {
      const string TRLIB = @"TRLIB\trlib64bit.dll";

      [DllImport(TRLIB)]
      public static extern TR_Error TR_InitLibrary(string folder);

      [DllImport(TRLIB)]
      public static extern void TR_AllowUnsafeTransformations();

      [DllImport(TRLIB)]
      public static extern void TR_ForbidUnsafeTransformations();

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

      [DllImport(TRLIB)]
      public static extern int TR_GetLastError();
    }
  }
}
