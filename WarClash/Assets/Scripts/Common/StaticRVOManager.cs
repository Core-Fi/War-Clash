using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class StaticRVOManager : Singleton<StaticRVOManager>
{
    private List<Building> _obstacles = new List<Building>();
    public void AddBuilding(Building b)
    {
        _obstacles.Add(b);
    }

    public void FixedUpdate()
    {
        

    }

}
