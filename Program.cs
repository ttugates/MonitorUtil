class Program
{
  static void Main(string[] args)
  {
    bool success = false;

    if (TryParseArgs(args, out var mShifts))    
      success = WindowSvc.DoSwap(mShifts);

    if (!success)
      WriteOutHelp();
  }

  static bool TryParseArgs(string[] args, out List<int> res)
  {
    res = args
      .Select<string, int?>(x =>
        Int32.TryParse(x.Trim(), out var i)
        ? i
        : null
      )
      .OfType<int>()
      .ToList();

    return res.Count > 1;
  }

  static void WriteOutHelp()
  {
    Console.WriteLine("This program is used to quickly swap all windows between multiple monitors.");
    Console.WriteLine("Pass space delimited monitor numbers as command args to run.");
    Console.WriteLine("Monitors are 0 base indexed, from left to right.  Monitors with identical left alignment are indexed top to bottom.");
    Console.WriteLine("e.g. c:\\>MonitorUtil.exe 0 2 3");
    Console.WriteLine("The preceeding command will shuffle windows in the direction of 0 > 2 > 3 > 0");
    Console.WriteLine("Pass the arguments in referse to shuffle the opposite direction.");
    Console.WriteLine("");
    Console.WriteLine("Press any key to exit");
    Console.ReadLine();
  }

}
