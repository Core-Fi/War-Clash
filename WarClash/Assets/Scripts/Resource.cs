
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using UnityEngine;

class ResourceLoader : Loader
{
    private string bundlePath;
    private string[] dependencies;
    private AssetBundle[] depens;
    private Object[] depenObjs;
    public override void Start(string path, System.Action<string, UnityEngine.Object> func)
    {
        this.path = path;
        this.func = func;
        bundlePath = Resource.GetBundleName(path);
        dependencies = Resource.Manifest.GetAllDependencies(bundlePath);
        if (dependencies.Length > 0)
        {
            depens = new AssetBundle[dependencies.Length];
            depenObjs = new Object[dependencies.Length];
            for (int i = dependencies.Length-1; i >= 0; i--)
            {
                var loader = Pool.SP.Get(typeof(Loader)) as Loader;
                if(loader!=null)
                    loader.Start(dependencies[i], OnDependencyLoadFinish);
            }
        }
        else
        {
            Main.inst.StartCoroutine(LoadMainAsset());
        }
    }
    public void OnDependencyLoadFinish(string dPath, Object bundle)
    {
        for (int i = 0; i < dependencies.Length; i++)
        {
            if(dependencies[i].Equals(dPath))
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
    IEnumerator LoadMainAsset()
    {
        string finalpath = Resource.BaseUrl + bundlePath;
        string assetName = Resource.BaseUrl2 + path;
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
    public override void Reset()
    {
        base.Reset();
        dependencies = null;
        depens = null;
        depenObjs = null;
    }
}

class Loader : IPool
{
    public string path;
    public System.Action<string, UnityEngine.Object> func;
    public virtual void Start(string path, System.Action<string, UnityEngine.Object> func)
    {
        this.path = path;
        this.func = func;
        Main.inst.StartCoroutine(Load());
    }
    private IEnumerator Load()
    {
        string finalpath = Resource.BaseUrl + path;
        string assetName = assetName = path;
        var asyn = AssetBundle.LoadFromFileAsync(finalpath);
        yield return asyn;
        func.Invoke(path, asyn.assetBundle);
        Pool.SP.Recycle(this);
    }
    public virtual void Reset()
    {
        path = null;
        func = null;
    }
}

class Resource
{
    public static string BaseUrl;
    public static string BaseUrl2 = "Assets/RequiredResources/";
    public static AssetBundleManifest Manifest;
    public static Dictionary<string, string> AssetsBundleDic;
    public static string[] AllBundles;
    public static int[] BundleNameHash;

    public static string GetBundleName(string path)
    {
        int hash = path.GetHashCode();
        for (int i = 0; i < BundleNameHash.Length; i++)
        {
            if (BundleNameHash[i] == hash)
            {
                return AllBundles[i];
            }
        }
        if (AssetsBundleDic.ContainsKey(path))
        {
            return AssetsBundleDic[path];
        }
        return null;
    }
    public static void LoadManifest()
    {
        string path = BaseUrl = System.IO.Path.Combine(Application.dataPath,@"..\AB\");
        var bundle = AssetBundle.LoadFromFile(BaseUrl + "AB");
        Manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        if(Manifest == null)
        {
            throw new System.Exception("Manifest not found ");
        }
        AllBundles = Manifest.GetAllAssetBundles();
        BundleNameHash = new int[AllBundles.Length];
        for (var i = 0; i < BundleNameHash.Length; i++)
        {
            BundleNameHash[i] = AllBundles[i].GetHashCode();
        }
        var txt = File.ReadAllText(BaseUrl + "bundles.txt");
        AssetsBundleDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(txt);
        if (AssetsBundleDic == null)
        {
            throw new System.Exception("AssetBundleIndexDic not found ");
        }
    }
    public static void Load(string path, System.Action<string, UnityEngine.Object> func)
    {
        if (Manifest == null)
        {
            LoadManifest();
        }
        var loader = Pool.SP.Get(typeof(ResourceLoader)) as ResourceLoader;
        loader.Start(path, func);
    }

    

}
