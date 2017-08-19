using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

class UIPath
{
    public const string UI_Battle = "Battle.prefab";


    private static IEnumerable<Type> viewTypes;

    private static Type[] GetTypesInNamespace(System.Reflection.Assembly assembly, string nameSpace)
    {
        var types = assembly.GetTypes();
        return assembly.GetTypes().Where((t) => {
            if (t != null && t.Namespace != null)
                return t.Namespace.Contains(nameSpace);
            return false;
        }).ToArray();
    }
    public static Type GetType(string name)
    {
        if (UIPath.viewTypes == null)
        {
            var assembly = System.Reflection.Assembly.GetAssembly(typeof (View));
            UIPath.viewTypes = Assembly.GetAssembly(typeof (View)).GetTypes().Where(t => t.IsSubclassOf(typeof (View)));
        }
        foreach (var viewType in viewTypes)
        {
           var attributes =  viewType.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                if (attribute is UIViewAttribute)
                {
                    var a = (attribute as UIViewAttribute);
                    if (a.path.Equals(name))
                    {
                        return viewType;
                    }
                }
            }
        }
        return null;
    }

}

public class UIViewAttribute : Attribute
{
    public string path;

    public UIViewAttribute(string path)
    {
        this.path = path;
    }
}