using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


class ManagerDriver
{
    public List<IManager> managers = new List<IManager>()
    {
        new ViewManager(),
        new SceneLoadManager(),
        new GameNetManager(),
        new InputManager()
    };

    public void Update()
    {
        for (int i = 0; i < managers.Count; i++)
        {
            managers[i].Update();
        }
    }

}
