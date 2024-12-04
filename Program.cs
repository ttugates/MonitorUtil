
using MonitorUtil.Models;

class Program
{
  static void Main(string[] args)
  {
    WindowSvc.DoSwap(new List<MOVE> { 
      new MOVE(2, 3),
      new MOVE(0, 2)
    });

  }


}
