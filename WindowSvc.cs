using System.Text;
using System.Runtime.InteropServices;
using MonitorUtil.Models;

class WindowSvc
{
  #region user32.dll interop
  [DllImport("user32.dll")]
  public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

  [DllImport("user32.dll", CharSet = CharSet.Auto)]
  public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool IsWindowVisible(IntPtr hWnd);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  static extern bool IsZoomed(IntPtr hWnd);

  [DllImport("user32.dll")]
  static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

  [DllImport("user32.dll")]
  public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

  [DllImport("user32.dll", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

  [DllImport("user32.dll")]
  public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorProc lpfnEnum, IntPtr dwData);

  [DllImport("user32.dll", CharSet = CharSet.Auto)]
  public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

  [DllImport("user32.dll")]
  public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

  private const uint SWP_NOZORDER = 0x0004;
  private const uint SWP_NOSIZE = 0x0001;
  private const int SW_RESTORE = 9;
  private const int SW_MAXIMIZE = 3;

  public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

  public delegate bool EnumMonitorProc(IntPtr hMonitor, IntPtr hdc, ref RECT lprcClip, IntPtr dwData);

  #endregion

  private static List<RECT> mRects = new List<RECT>();
  private static List<MOVE> mToSwap = new List<MOVE>();

  public static void DoSwap(List<MOVE> monitorsToSwap)
  {
    mToSwap = monitorsToSwap;

    EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, new EnumMonitorProc(EnumMonitorCallback), IntPtr.Zero);
    mRects = mRects
      .OrderBy(x => x.Left)
      .ToList();

    if (mRects.Count <= 1)
      return;

    // Call EnumWindows to get all visible windows
    EnumWindows(new EnumWindowsProc(EnumWindowCallback), IntPtr.Zero);
  }

  // Callback function for EnumWindows
  private static bool EnumWindowCallback(IntPtr hWnd, IntPtr lParam)
  {
    if (IsTaskbar(hWnd) || !IsWindowVisible(hWnd))
      return true;

    foreach (var swap in mToSwap)
    {
      if (GetWindowRect(hWnd, out var windoPos))
      {
        // Determine which monitor the window is on based on its coordinates
        int monitorIndex = -1;

        for (int i = 0; i < mRects.Count; i++)
        {
          if (IsWindowOnMonitor(i, windoPos))
          {
            monitorIndex = i;
            break;
          }
        }
        if (monitorIndex == -1)
          return true;

        if (monitorIndex == swap.Src)
        {
          MoveWindowFromTo(windoPos, swap.Src, swap.Dest);
        }
        else if (monitorIndex == swap.Dest)
        {
          MoveWindowFromTo(windoPos, swap.Dest, swap.Src);
        }
      }


    }
    return true;

    void MoveWindowFromTo(RECT windoPos, int srcMonitor, int destMonitor)
    {
      var maximized = IsZoomed(hWnd);
      if (maximized) ShowWindow(hWnd, SW_RESTORE);

      int x = mRects[destMonitor].Left + windoPos.Left - mRects[srcMonitor].Left;
      int y = windoPos.Top;
      int cx = windoPos.Right - windoPos.Left;
      int cy = windoPos.Bottom - windoPos.Top;
      SetWindowPos(hWnd, IntPtr.Zero, x, y, cx, cy, SWP_NOZORDER | SWP_NOSIZE);

      if (maximized) ShowWindow(hWnd, SW_MAXIMIZE);
    }

    bool IsWindowOnMonitor(int monitor, RECT windoPos)
    {
      var maximized = IsZoomed(hWnd);

      if (maximized)
      {
        // only check middle.  Experience indicates bounds go outside monitor by 7px.
        var xMid = (windoPos.Left + windoPos.Right) / 2;
        var yMid = (windoPos.Top + windoPos.Bottom) / 2;
        return xMid >= mRects[monitor].Left && xMid <= mRects[monitor].Right &&
          yMid >= mRects[monitor].Top && yMid <= mRects[monitor].Bottom;
      }

      return windoPos.Left >= mRects[monitor].Left && windoPos.Left <= mRects[monitor].Right &&
                windoPos.Top >= mRects[monitor].Top && windoPos.Bottom <= mRects[monitor].Bottom;
    }
  }

  // Callback function for EnumDisplayMonitors
  private static bool EnumMonitorCallback(IntPtr hMonitor, IntPtr hdc, ref RECT lprcClip, IntPtr dwData)
  {
    mRects.Add(lprcClip);
    return true;
  }

  private static bool IsTaskbar(IntPtr hWnd)
  {
    StringBuilder className = new StringBuilder(256);
    GetClassName(hWnd, className, className.Capacity);

    // Check if the window's class name matches the taskbar class name
    var cName = className.ToString();
    var tBarNames = new[] { "Shell_TrayWnd", "Shell_SecondaryTrayWnd" };
    return tBarNames.Contains(cName);
  }



}
