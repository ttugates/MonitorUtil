using System;
using System.Text;
using System.Runtime.InteropServices;
using MonitorUtil.Models;

class WindowSvc
{
  [DllImport("user32.dll")]
  public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

  [DllImport("user32.dll", CharSet = CharSet.Auto)]
  public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool IsWindowVisible(IntPtr hWnd);

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

  [DllImport("user32.dll", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

  // Define constants for SetWindowPos flags
  private const uint SWP_NOZORDER = 0x0004;
  private const uint SWP_NOSIZE = 0x0001;

  // Delegate for the EnumWindows callback function
  public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

  public static void SwapWindows()
  {
    EnumWindows(new EnumWindowsProc(EnumWindowCallback), IntPtr.Zero);
  }

  private static bool EnumWindowCallback(IntPtr hWnd, IntPtr lParam)
  {
    if (IsWindowVisible(hWnd))
    {
      // Get the window title
      StringBuilder windowTitle = new StringBuilder(256);
      GetWindowText(hWnd, windowTitle, windowTitle.Capacity);

      // If the title contains "Notepad++", move the window
      if (windowTitle.ToString().Contains("Notepad++"))
      {
        // Get the window's current coordinates
        RECT rect;
        if (GetWindowRect(hWnd, out rect))
        {
          // Calculate the new position by adding 500 pixels to the left coordinate
          int newLeft = rect.Left + 500;

          // Move the window to the new position, keeping its current size
          SetWindowPos(hWnd, IntPtr.Zero, newLeft, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, SWP_NOZORDER | SWP_NOSIZE);

          // Output the window information including the new position
          Console.WriteLine($"Moved Window Handle: {hWnd.ToInt64()}, Title: {windowTitle.ToString()}");
          Console.WriteLine($"New Coordinates: Left = {newLeft}, Top = {rect.Top}, Right = {rect.Right}, Bottom = {rect.Bottom}");
          Console.WriteLine($"Width = {rect.Right - rect.Left}, Height = {rect.Bottom - rect.Top}");
        }
        else
        {
          Console.WriteLine($"Failed to get coordinates for window: {windowTitle.ToString()}");
        }
      }
      else
      {
        // Output window info without moving it if it doesn't match the title
        Console.WriteLine($"Window Handle: {hWnd.ToInt64()}, Title: {windowTitle.ToString()}");
      }
    }

    return true; // Continue enumeration
  }
}
