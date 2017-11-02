using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

struct GridData
{
    public bool FlagAsTarget;
}
class GridService
{
    private static GridData[,] _gridData = new GridData[20,20];

    public static void Init()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
            {
                _gridData[i,j] = new GridData();
            }
        }
    }

    public static bool IsTarget(int x, int y)
    {
        return _gridData[y, x].FlagAsTarget;
    }

    public static void TagAsTarget(int x, int y)
    {
        _gridData[y, x].FlagAsTarget = true;
    }
}
