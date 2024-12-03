using System;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
  // Import necessary Windows API functions
  [DllImport("user32.dll")]
  public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpEnumFunc, IntPtr dwData);

  [DllImport("user32.dll")]
  public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

  [DllImport("user32.dll")]
  public static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

  [DllImport("user32.dll")]
  public static extern IntPtr MonitorFromRect(ref RECT lprc, uint dwFlags);

  // Define the delegate for monitor enumeration
  public delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdc, ref RECT lprcClip, IntPtr dwData);

  // Define structures
  [StructLayout(LayoutKind.Sequential)]
  public struct MONITORINFO
  {
    public uint cbSize;
    public RECT rcMonitor;
    public RECT rcWork;
    public uint dwFlags;
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct RECT
  {
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;

    public RECT(int left, int top, int right, int bottom)
    {
      Left = left;
      Top = top;
      Right = right;
      Bottom = bottom;
    }
  }

  [StructLayout(LayoutKind.Sequential)]
  public struct POINT
  {
    public int x;
    public int y;
  }

  public static List<MONITORINFO>? Monitors;

  static void Main(string[] args)
  {
    Monitors = new List<MONITORINFO>();
    // Enumerate all the monitors connected to the system
    EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, MonitorEnumProcCallback, IntPtr.Zero);
    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();
  }

  // Callback for EnumDisplayMonitors to retrieve monitor info
  private static bool MonitorEnumProcCallback(IntPtr hMonitor, IntPtr hdc, ref RECT lprcClip, IntPtr dwData)
  {
    // Create a MONITORINFO structure
    MONITORINFO monitorInfo = new MONITORINFO();
    monitorInfo.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO));

    // Get the monitor info using GetMonitorInfo
    if (GetMonitorInfo(hMonitor, ref monitorInfo))
    {
      // Print out information about the monitor
      Console.WriteLine("Monitor found:");
      Console.WriteLine($"  Left: {monitorInfo.rcMonitor.Left}");
      Console.WriteLine($"  Top: {monitorInfo.rcMonitor.Top}");
      Console.WriteLine($"  Right: {monitorInfo.rcMonitor.Right}");
      Console.WriteLine($"  Bottom: {monitorInfo.rcMonitor.Bottom}");
    }
    else
    {
      Console.WriteLine("Failed to get monitor info.");
    }
    if(Monitors != null)
      Monitors.Add(monitorInfo);

    return true; // Continue enumeration
  }
}
