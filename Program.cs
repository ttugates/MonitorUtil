class Program
{
  static void Main(string[] args)
  {
    if (TryParseArgs(args, out var mShifts))
    {
      WindowSvc.DoSwap(mShifts);
    }
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

}
