using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


public class ABTest : Editor
{
    private const string RequiredUrl = "Assets/RequiredResources/";
    private const string TextConfig = "TextConfigs";
    public static Dictionary<Object, int> RefCount = new Dictionary<Object, int>();
    public static List<GameObject> CaculatedGo = new List<GameObject>(); 
    public static List<System.Type> Types = new List<System.Type>() {typeof(RuntimeAnimatorController),typeof(Texture2D), typeof(Mesh), typeof(Material), typeof(Shader), typeof(AnimationClip)};
    [MenuItem("Tools/BuildAssetBundle(Win)")]
    public static void BuildAb()
    {
        ClearAbName();
        SetAssetBundleName();
        var path = Path.Combine(Application.dataPath, @"..\AB");
        var manifest = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows64);
        
        if (manifest!=null)
            DoAfterBuild(path, manifest);
    }

    public static void DoAfterBuild(string path, AssetBundleManifest manifest)
    {
        var assetInfosInBundle = new Dictionary<string, AssetInfo>();
        var assetBundleHash = new Dictionary<string, string>(); ;
        var bundles = manifest.GetAllAssetBundles();
        for (var i = 0; i < bundles.Length; i++)
        {
            var hash = manifest.GetAssetBundleHash(bundles[i]);
            assetBundleHash.Add(bundles[i], hash.ToString());
            var allAssets = AssetDatabase.GetAssetPathsFromAssetBundle(bundles[i]);
            for (var j = 0; j < allAssets.Length; j++)
            {
                var fileName = Path.GetFileName(allAssets[j]);
                assetInfosInBundle[fileName] = new AssetInfo() { AssetPath = allAssets[j].ToLower(), BundleName = bundles[i] };
            }
        }
        var txt = Newtonsoft.Json.JsonConvert.SerializeObject(assetInfosInBundle, Formatting.Indented);
        byte[] text = Encoding.UTF8.GetBytes(txt);
        byte[] compress = Compress(text);
        File.WriteAllBytes(path + @"/assetInfos.txt", compress);

        var assetBundleHashTxt = Newtonsoft.Json.JsonConvert.SerializeObject(assetBundleHash, Formatting.Indented);
        byte[] hashText = Encoding.UTF8.GetBytes(assetBundleHashTxt);
        byte[] compressHash = Compress(hashText);
        File.WriteAllBytes(path + @"/assetBundleHash.txt", compressHash);
    }
    public static byte[] Compress(byte[] raw)
    {
        return raw;
        using (MemoryStream memory = new MemoryStream())
        {
            using (GZipStream gzip = new GZipStream(memory,
                CompressionMode.Compress, true))
            {
                gzip.Write(raw, 0, raw.Length);
            }
            return memory.ToArray();
        }
    }

    [MenuItem("Tools/BuildAssetBundle(Android)")]
    public static void BuildAbForAndroid()
    {
        ClearAbName();
        SetAssetBundleName();
        var path = Path.Combine(Application.streamingAssetsPath, @"AB");
        if (Directory.Exists(path))
        {
           // Directory.Delete(path, true);
        }
        Directory.CreateDirectory(path);
        var manifest = BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);
        DoAfterBuild(path, manifest);
        ZipUtility.Zip(new string[] {path}, path+".zip");
        Directory.Delete(path);
    }
    [MenuItem("Tools/SetAssetBundleName")]
    public static void SetAssetBundleName()
    {
        var paths = AssetDatabase.GetAllAssetPaths();
        List<string> filterPaths = new List<string>(paths.Length);
        foreach (var path in paths)
        {
            if(path.Contains(RequiredUrl) && !AssetDatabase.IsValidFolder(path))
            {
                filterPaths.Add(path);
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                if (path.Contains(TextConfig))
                {
                    var directory = Path.GetDirectoryName(path);
                    var ds = directory.Split('/');
                    SetAssetBundleName( ds[ds.Length-1], asset);
                }
                else
                {

                    if (asset is GameObject)
                    {
                        var go = asset as GameObject;
                        DoGameObject(go, path);
                    }
                    else if (asset is SceneAsset)
                    {
                        var scene = asset as SceneAsset;
                        DoScene(scene, path);
                    }
                    else
                    {
                        DoAsset(asset, path);
                    }
                }
            }
        }
    }
    [MenuItem("Tools/ClearAssetBundleName")]
    private static void ClearAbName()
    {
        var paths = AssetDatabase.GetAllAssetPaths();
        foreach (var path in paths)
        {
            if (path.Contains(RequiredUrl))
            {
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                if (asset != null)
                {
                    ClearAb(asset);
                }
            }
        }
    }
    static void ClearAb(Object go)
    {
        SetAssetBundleName(string.Empty, go);
        var objs = EditorUtility.CollectDependencies(new Object[] { go });
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            if(obj==null)
                continue;
            System.Type t = obj.GetType();
            if (Types.Contains(t) || t.Equals(typeof(SceneAsset)) || t.Equals(typeof(GameObject)))
                SetAssetBundleName(string.Empty, obj);
        }
    }

    public static void SetAssetBundleName(string ap, Object obj)
    {
        var rPath = AssetDatabase.GetAssetPath(obj);
        AssetImporter ai = AssetImporter.GetAtPath(rPath);
        if (ai == null)
        {
        }
        else
        {
            if (ap != string.Empty)
            {
                ap = ap.Replace(RequiredUrl, string.Empty);
            }
            ai.assetBundleName = ap;
        }
    }
    public static string GetExactPathName(string pathName)
    {
        if (!(File.Exists(pathName) || Directory.Exists(pathName)))
            return pathName;

        var di = new DirectoryInfo(pathName);

        if (di.Parent != null)
        {
            return Path.Combine(
                GetExactPathName(di.Parent.FullName),
                di.Parent.GetFileSystemInfos(di.Name)[0].Name);
        }
        else
        {
            return di.Name.ToUpper();
        }
    }
    static void DoScene(SceneAsset scene, string path)
    {
        SetAssetBundleName(path, scene);
        var objs = EditorUtility.CollectDependencies(new Object[] { scene });
        foreach (Object obj in objs)
        {
            if(obj is GameObject && !CaculatedGo.Contains(obj as GameObject))
            {
                CaculatedGo.Add(obj as GameObject);
                var ap = AssetDatabase.GetAssetPath(obj);
                DoGameObject(obj as GameObject, ap, true);
            }
        }
    }
    
    static void DoGameObject(GameObject go, string path, bool inSceneObj = false)
    {
        if (inSceneObj)
        {
            int count = CheckCount(go);
            if (count > 1)
            { 
                SetAssetBundleName(path, go);
            }
        }
        else
        {
            SetAssetBundleName(path, go);
        }
        CollectDependencies(go, path);
    }
    static void DoAsset(Object go, string path)
    {
        bool done = false;
        if(go is Texture2D)
        {
            done = DoUISprite(go as Texture);
        }
        if(!done)
        {
            var ap = AssetDatabase.GetAssetPath(go);
            int count = CheckCount(go);
            if (count > 1)
            {
                SetAssetBundleName(ap, go);
            }
        }
    }
    static void CollectDependencies(Object go, string path)
    {
        var objs = EditorUtility.CollectDependencies(new Object[] { go });
        foreach (Object obj in objs)
        {
            if(obj == null)
                continue;
            if (Types.Contains(obj.GetType()))
            {
                DoAsset(obj, path);
            }
        }
    }
    static bool DoUISprite(Object go)
    {
        var r_path = AssetDatabase.GetAssetPath(go);
        TextureImporter ti = AssetImporter.GetAtPath(r_path) as TextureImporter;
        if(ti!=null && ti.textureType == TextureImporterType.Sprite && !ti.assetBundleName.Equals(string.Empty))
        {
            ti.assetBundleName = ti.spritePackingTag;
            return true;
        }
        return false;
    }
    private static int CheckCount(Object obj)
    {
        if (!RefCount.ContainsKey(obj)) RefCount[obj] = 0;
        return ++RefCount[obj];
    }

}
