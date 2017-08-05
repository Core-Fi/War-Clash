using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ABTest : Editor {

    public static Dictionary<Object, int> refCount = new Dictionary<Object, int>();
    public static List<GameObject> caculatedGO = new List<GameObject>(); 
    public static List<System.Type> types = new List<System.Type>() {typeof(RuntimeAnimatorController),typeof(Texture2D), typeof(Mesh), typeof(Material), typeof(Shader), typeof(AnimationClip)};
    [MenuItem("Tools/BuildAssetBundle(Win)")]
    public static void BuildAB()
    {
        ClearABName();
        SetAssetBundleName();
        var path = Path.Combine(Application.dataPath, @"..\AB");
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression|BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows64);
    }
    [MenuItem("Tools/BuildAssetBundle(Android)")]
    public static void BuildABForAndroid()
    {
        ClearABName();
        SetAssetBundleName();
        var path = Path.Combine(Application.streamingAssetsPath, @"AB");
        BuildPipeline.BuildAssetBundles(path, BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android);
    }
    [MenuItem("Tools/SetAssetBundleName")]
    public static void SetAssetBundleName()
    {
        var paths = AssetDatabase.GetAllAssetPaths();
        List<string> filterPaths = new List<string>(paths.Length);
        foreach (var path in paths)
        {
            if(path.Contains("Assets/RequiredResources"))
            {
                filterPaths.Add(path);
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
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
    [MenuItem("Tools/ClearAssetBundleName")]
    private static void ClearABName()
    {
        var paths = AssetDatabase.GetAllAssetPaths();
        foreach (var path in paths)
        {
            if (path.Contains("Assets/RequiredResources/"))
            {
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                ClearAB(asset);
            }
        }
    }
    static void ClearAB(Object go)
    {
        SetAssetBundleName(string.Empty, go);
        var objs = EditorUtility.CollectDependencies(new Object[] { go });
        for (int i = 0; i < objs.Length; i++)
        {
            var obj = objs[i];
            System.Type t = obj.GetType();
            if (types.Contains(t) || t.Equals(typeof(SceneAsset)) || t.Equals(typeof(GameObject)))
                SetAssetBundleName(string.Empty, obj);
        }
    }
    static AssetBundle ab;
    [MenuItem("Tools/Caculate")]
    public static void Compare()
    {
        
        //WWW www = new WWW("file:///D:/TestProjs/ABTest/New Unity Project/AB/AB");
        //while (!www.isDone)
        //{
            
        //}
        //var manifest = (AssetBundleManifest)www.assetBundle.LoadAsset("AB");
    //    manifest.
        if(ab == null)
            ab = AssetBundle.LoadFromFile(@"D:\TestProjs\ABTest\New Unity Project\AB\assets\boxworld\Canvas.prefab");
        Debug.LogError(ab == null);
        var obj = ab.LoadAsset("Canvas");
        GameObject go = GameObject.Instantiate(obj) as GameObject;
        Debug.LogError(obj==null);
        ab.Unload(false);
    }

    public static void SetAssetBundleName(string ap, Object obj)
    {
        var r_path = AssetDatabase.GetAssetPath(obj);
        AssetImporter ai = AssetImporter.GetAtPath(r_path);
        if (ai == null)
        {
           // Debug.LogError(ap + "  " + obj.GetType());
        }
        else
        {
            ap = ap.Replace("Assets/RequiredResources/", string.Empty);
            ai.assetBundleName = ap;
        }
    }

    static void DoScene(SceneAsset scene, string path)
    {
        SetAssetBundleName(path, scene);
        var objs = EditorUtility.CollectDependencies(new Object[] { scene });
        foreach (Object obj in objs)
        {
            if(obj is GameObject && !caculatedGO.Contains(obj as GameObject))
            {
                caculatedGO.Add(obj as GameObject);
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
        DoAsset(go, path);
    }
    static void DoAsset(Object go, string path)
    {
        var objs = EditorUtility.CollectDependencies(new Object[] { go });
        foreach (Object obj in objs)
        {
            if (types.Contains(obj.GetType()))
            {
                var ap = AssetDatabase.GetAssetPath(obj);
                int count = CheckCount(obj);
                if (count > 1)
                {
                    SetAssetBundleName(ap, obj);
                }
            }
        }
    }
    private static int CheckCount(Object obj)
    {
        if (!refCount.ContainsKey(obj)) refCount[obj] = 0;
        return ++refCount[obj];
    }

}
