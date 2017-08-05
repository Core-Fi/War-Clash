
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ResourceLoader : IPool
{
    private string path;
    private System.Action<string, UnityEngine.Object> func;
    string[] dependencies;
    AssetBundle[] depens;
    Object[] depenObjs;
    bool isSub = false;
    public void Start(string path, System.Action<string, UnityEngine.Object> func, bool isSub = false)
    {
        this.path = path;
        this.func = func;
        this.isSub = isSub;
        dependencies = Resource.manifest.GetAllDependencies(path);
        if (!isSub && dependencies.Length > 0)
        {
            depens = new AssetBundle[dependencies.Length];
            depenObjs = new Object[dependencies.Length];
            for (int i = dependencies.Length-1; i >= 0; i--)
            {
                var loader = Pool.SP.Get(typeof(ResourceLoader)) as ResourceLoader;
                loader.Start(dependencies[i], OnDependencyLoadFinish, true);
            }
        }
        else
        {
            if(isSub)
                Main.inst.StartCoroutine(LoadDependency());
            else
                Main.inst.StartCoroutine(LoadMainAsset());
        }
    }
    public void OnDependencyLoadFinish(string path, Object bundle)
    {
        for (int i = 0; i < dependencies.Length; i++)
        {
            if(dependencies[i].Equals(path))
            {
                depens[i] = bundle as AssetBundle;
            }
        }
        bool isFinish = true;
        for (int i = 0; i < depens.Length; i++)
        {
            if (depens[i] == null)
            {
                isFinish = false;
                break;
            }
        }
        if(isFinish)
        {
            for (int i = 0; i < depens.Length; i++)
            {
                depenObjs[i] = depens[i].LoadAsset(dependencies[i]);
            }
            Main.inst.StartCoroutine(LoadMainAsset());
        }
    }
    IEnumerator LoadDependency()
    {
        string finalpath = Resource.baseURL + path;
        string assetName = assetName = path;
        var asyn = AssetBundle.LoadFromFileAsync(finalpath);
        yield return asyn;
        func.Invoke(path, asyn.assetBundle);
        Pool.SP.Recycle(this);
    }
    IEnumerator LoadMainAsset()
    {
        string finalpath = Resource.baseURL + path;
        string assetName = Resource.baseURL2 + path;
        var asyn = AssetBundle.LoadFromFileAsync(finalpath);
        yield return asyn;
        var obj = asyn.assetBundle.LoadAsset(assetName);
        if (depens != null)
        {
            for (int i = 0; i < depens.Length; i++)
            {
                depens[i].Unload(false);
            }
        }
        asyn.assetBundle.Unload(false);
        func.Invoke(path, obj);
        Pool.SP.Recycle(this);
    }
    public void Reset()
    {
        path = null;
        func = null;
        dependencies = null;
        depens = null;
        depenObjs = null;
        isSub = false;
    }

}
class Resource
{
    public static string baseURL;
    public static string baseURL2 = "Assets/RequiredResources/";
    public static AssetBundleManifest manifest;
    public static void LoadManifest()
    {
        string path = baseURL = System.IO.Path.Combine(Application.dataPath,@"..\AB\");
        var bundle = AssetBundle.LoadFromFile(baseURL + "AB");
        manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        if(manifest == null)
        {
            throw new System.Exception("manifest not found ");
        }
    }
    public static void Load(string path, System.Action<string, UnityEngine.Object> func)
    {
        if (manifest == null)
        {
            LoadManifest();
        }
        var loader = Pool.SP.Get(typeof(ResourceLoader)) as ResourceLoader;
        loader.Start(path, func);
    }

    

}
