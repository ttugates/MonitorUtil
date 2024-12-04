using System.Runtime.InteropServices;

namespace MonitorUtil.Models
{
  [StructLayout(LayoutKind.Sequential)]
  public struct WINDOWPLACEMENT
  {
    public uint length;
    public uint flags;
    public uint showCmd;
    public RECT rcNormalPosition;
    public RECT rcMonitor;
  }
}
