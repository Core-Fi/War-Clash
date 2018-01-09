using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;
[Serializable]
public struct AssetInfo
{
    public string BundleName;
    public string AssetPath;
}

public enum WaitingType
{
    Asset, Bundle
}
public struct WaitingBundle
{
    public WaitingType WaitingType;
    public string Name;
    public System.Action<string, UnityEngine.Object> Action;
}

class BundleLoader : IPool
{
    public Action<string, AssetBundle> OnLoadFinish;
    public string BundleName;
    protected IEnumerator LoadCoroutine;
    private string[] dependencies;
    private int count;
    private Object[] depenObjs;

    private bool loadNow;
   // private bool 
    public void Start(string bundleName, Action<string, AssetBundle> onLoadFinish, bool now = false)
    {
        this.loadNow = now;
        this.BundleName = bundleName;
        LoadCoroutine = LoadMainAsset();
        dependencies = AssetResources.Manifest.GetAllDependencies(bundleName);
        AssetResources.AddLoadingLoader(this);
        if (dependencies.Length > 0)
        {
            depenObjs = new Object[dependencies.Length];
            for (int i = dependencies.Length-1; i >= 0; i--)
            {
                AssetResources.LoadBundle(dependencies[i], OnDependencyLoadFinish, loadNow);
            }
        }
        else
        {
            if (loadNow)
            {
                LoadMainAssetNow();
            }
            else
            {
                Main.SP.StartCoroutine(LoadCoroutine);
            }
           
        }
    }
    public void OnDependencyLoadFinish(string dPath, Object bundle)
    {
        count ++;
        if(count == dependencies.Length)
        {
            if (loadNow)
            {
                LoadMainAssetNow();
            }
            else
            {
                Main.SP.StartCoroutine(LoadCoroutine);
            }
        }
    }
    //System.Diagnostics.Stopwatch sw = new Stopwatch();
    //sw.Start();
    IEnumerator LoadMainAsset()
    {
        var asyn = AssetBundle.LoadFromFileAsync(AssetResources.GetAssetPath(BundleName));
        yield return asyn.assetBundle;
        var assetsReq = asyn.assetBundle.LoadAllAssetsAsync();
        yield return assetsReq;
        AssetResources.RemoveFinishedLoader(this, BundleName, assetsReq.allAssets, asyn.assetBundle);
        Pool.SP.Recycle(this);
    }

    void LoadMainAssetNow()
    {
        var assetBundle = AssetBundle.LoadFromFile(AssetResources.GetAssetPath(BundleName));
        var assetsReq = assetBundle.LoadAllAssets();
        AssetResources.RemoveFinishedLoader(this, BundleName, assetsReq, assetBundle);
        Pool.SP.Recycle(this);
    }

    
    public void Reset()
    {
        dependencies = null;
        count = 0;
        depenObjs = null;
        BundleName = null;
        OnLoadFinish = null;
    }
}


class AssetResources
{
    public static string BaseUrl;
    public static string PersistentUrl;
    public static AssetBundleManifest Manifest;
    public static Dictionary<string, AssetInfo> AssetsInfos; 
    private static readonly List<BundleLoader> LoadingList = new List<BundleLoader>();
    private static readonly Dictionary<string,Dictionary<string, Object>> BundleNameAssets = new Dictionary<string, Dictionary<string, Object>>(); 
    private static readonly Dictionary<string, AssetBundle> LoadedBundles = new Dictionary<string, AssetBundle>(); 
    private static readonly List<WaitingBundle> WaitingList = new List<WaitingBundle>();
    public static void UnloadBundles()
    {
        foreach (var loadedBundle in LoadedBundles)
        {
            loadedBundle.Value.Unload(false);
        }
        LoadedBundles.Clear();
    }
    public static void RemoveFinishedLoader(BundleLoader l, string bundleName, Object[] assets, AssetBundle bundle)
    {
        string[] assetsNames = bundle.GetAllAssetNames();
        if (!BundleNameAssets.ContainsKey(bundleName))
        {
            BundleNameAssets[bundleName] = new Dictionary<string, Object>();
            for (int i = 0; i < assetsNames.Length; i++)
            {
                BundleNameAssets[bundleName][assetsNames[i]] = assets[i];
            }
            LoadedBundles[bundleName] = bundle;
        }
        LoadingList.Remove(l);
        for (int i = 0; i < WaitingList.Count; i++)
        {
            var w = WaitingList[i];
            string waitingBundleName = string.Empty;
            if (w.WaitingType == WaitingType.Asset)
            {
                var bundleInfo = GetAssetInfo(w.Name);
                if (bundleInfo.BundleName.Equals(bundleName))
                {
                    WaitingList.RemoveAt(i);
                    i--;
                    w.Action.Invoke(w.Name,
                        BundleNameAssets[bundleInfo.BundleName][bundleInfo.AssetPath]);
                }
            }
            else
            {
                if (w.Name.Equals(bundleName))
                {
                    WaitingList.RemoveAt(i);
                    i--;
                    w.Action.Invoke(w.Name, bundle);
                }
            }
        }
    }
    public static void AddLoadingLoader(BundleLoader l)
    {
        LoadingList.Add(l);
    }
    public static string GetAssetPath(string bundlename)
    {
        var persistentPath = AssetResources.PersistentUrl + bundlename;
        if (File.Exists(AssetResources.PersistentUrl + bundlename))
        {
            return persistentPath;
        }
        else
        {
            return AssetResources.BaseUrl + bundlename;
        }
    }
    public static AssetInfo GetAssetInfo(string assetName)
    {
        if (AssetsInfos.ContainsKey(assetName))
        {
            return AssetsInfos[assetName];
        }
        else
        {
            throw new Exception("Asset "+assetName+" Not Exsit");
        }
    }

    public static AssetBundle GetBundleByName(string bundleName)
    {
        if (LoadedBundles.ContainsKey(bundleName))
        {
            return LoadedBundles[bundleName];
        }
        else
        {
            return null;
        }
    }

    static AssetResources()
    {
#if UNITY_EDITOR
        BaseUrl = System.IO.Path.Combine(Application.dataPath, @"..\AB\");
        PersistentUrl = System.IO.Path.Combine(Application.persistentDataPath, @"AB\");
#elif UNITY_ANDROID
        BaseUrl = Path.Combine(Application.streamingAssetsPath, @"AB/");
        PersistentUrl = Path.Combine(Application.persistentDataPath, @"AB/");
#endif
    }
    public static Dictionary<string, string> LoadBundleHash()
    {
        var assetBundleHashPath = GetAssetPath("assetBundleHash.txt");
        var txt = Utility.ReadBytesFromAbsolutePath(assetBundleHashPath);
        var destr = Encoding.UTF8.GetString(txt);
        var dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(destr);
        return dic;
    }
    private static void LoadManifest()
    {
        var bundle = AssetBundle.LoadFromFile(GetAssetPath("AB"));
        Manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        bundle.Unload(false);
        if(Manifest == null)
        {
            throw new System.Exception("Manifest not found ");
        }
        var assetInfoPath = GetAssetPath("assetInfos.txt");
        var txt = Utility.ReadBytesFromAbsolutePath(assetInfoPath);
      //  byte[] decompress = Utility.Decompress(txt);

        var destr = Encoding.UTF8.GetString(txt);
        AssetsInfos = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, AssetInfo>>(destr);
        if (AssetsInfos == null)
        {
            throw new System.Exception("BundleAssetsDic not found at path "+ BaseUrl + "assetInfos.txt");
        }
    }
    public static void LoadAsset(string assetName, System.Action<string, UnityEngine.Object> action, bool now=false)
    {
        if (Manifest == null)
        {
            LoadManifest();
        }
        var assetInfo =  GetAssetInfo(assetName);
        if (BundleNameAssets.ContainsKey(assetInfo.BundleName))
        {
            action.Invoke(assetName, BundleNameAssets[assetInfo.BundleName][assetInfo.AssetPath]);
        }
        else if (IsLoading(assetName, WaitingType.Asset))
        {
            WaitingList.Add(new WaitingBundle() { Action = action, Name = assetName, WaitingType = WaitingType.Asset});
        }
        else
        {
            WaitingList.Add(new WaitingBundle() { Action = action, Name = assetName, WaitingType = WaitingType.Asset});
            var loader = Pool.SP.Get(typeof(BundleLoader)) as BundleLoader;
            loader.Start(assetInfo.BundleName, null, now);
        }
    }

    public static void LoadBundle(string bundleName, System.Action<string, UnityEngine.Object> action, bool now = false)
    {
        if (Manifest == null)
        {
            LoadManifest();
        }
        if (LoadedBundles.ContainsKey(bundleName))
        {
            action.Invoke(bundleName, LoadedBundles[bundleName]);
        }
        else if (IsLoading(bundleName, WaitingType.Bundle))
        {
            WaitingList.Add(new WaitingBundle() { Action = action, Name = bundleName, WaitingType = WaitingType.Bundle });
        }
        else
        {
            WaitingList.Add(new WaitingBundle() { Action = action, Name = bundleName, WaitingType = WaitingType.Bundle });
            var loader = Pool.SP.Get(typeof(BundleLoader)) as BundleLoader;
            loader.Start(bundleName, null, now);
        }
    }
    private static bool IsLoading(string name, WaitingType wt)
    {
        string bundleName = string.Empty;
        if (wt == WaitingType.Asset)
        {
            var bundleInfo = GetAssetInfo(name);
            bundleName = bundleInfo.BundleName;
        }
        else
        {
            bundleName = name;
        }
        for (int i = 0; i < LoadingList.Count; i++)
        {
            if (LoadingList[i].BundleName.Equals(bundleName))
            {
                return true;
            }
        }
        return false;
    }

}
