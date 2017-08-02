using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class ManagerDriver : Singleton<ManagerDriver>
{
    public List<IManager> managers = new List<IManager>()
    {
        new ViewManager(),


    };

    public void Update()
    {
        for (int i = 0; i < managers.Count; i++)
        {
            managers[i].Update();
        }
    }

}
