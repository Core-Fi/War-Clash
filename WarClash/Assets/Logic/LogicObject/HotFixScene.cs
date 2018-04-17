using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Logic.LogicObject;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using Debug = UnityEngine.Debug;

namespace Logic.LogicObject
{
    class HotFixScene : IScene
    {
        private int wait = 10;
        private int curTick = 0;
        public void Destroy()
        {
            
        }
       
        public void Init()
        {
         //   StartUpdatePatch();
            curTick = 0;
        }

        public void FixedUpdate(long deltaTime)
        {
            curTick++;
            if (wait == curTick)
            {
                LogicCore.SP.SceneManager.SwitchScene( new BattleScene("scene02"));
                var bs = LogicCore.SP.SceneManager.CurrentScene as BattleScene;
                //var mp = bs.CreateSceneObject<MainPlayer>();
            }
        }

        public void Update(float deltaTime)
        {

        }

        private void StartUpdatePatch()
        {
            Main.SP.StartCoroutine(LoadHashFileFromServer());
            Main.SP.StartCoroutine(LoadManifestFile());
            Main.SP.StartCoroutine(LoadAssetInfosFile());
        }
        private List<string> _needDownLoadList = new List<string>();
        IEnumerator LoadAssetInfosFile()
        {
            UnityWebRequest www = UnityWebRequest.Get("http://47.94.204.158/AB/assetInfos.txt");
            yield return www.Send();
            if (www.isError)
            {
                DLog.Log(www.error);
            }
            else
            {
                SaveToPersistentPath("assetInfos.txt", www.downloadHandler.data);
                www.Dispose();
            }
        }
        IEnumerator LoadManifestFile()
        {
            UnityWebRequest www = UnityWebRequest.Get("http://47.94.204.158/AB/AB");
            yield return www.Send();
            if (www.isError)
            {
                DLog.Log(www.error);
            }
            else
            {
                SaveToPersistentPath("AB", www.downloadHandler.data);
                www.Dispose();
            }
        }
        IEnumerator LoadHashFileFromServer()
        {
            UnityWebRequest www = UnityWebRequest.Get("http://47.94.204.158/AB/assetBundleHash.txt");
            yield return www.Send();
            if (www.isError)
            {
                DLog.Log(www.error);
            }
            else
            {
                // Show results as text
                _needDownLoadList.Clear();
               // System.Diagnostics.Stopwatch sw  = new Stopwatch();
                var remoteBundleHash = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(www.downloadHandler.text);
                var localBundleHash = AssetResources.LoadBundleHash();
                foreach (var bh in remoteBundleHash)
                {
                    string hash;
                    if (localBundleHash.TryGetValue(bh.Key, out hash))
                    {
                        if (!hash.Equals(bh.Value))
                        {
                            _needDownLoadList.Add(bh.Key);
                        }
                    }
                    else
                        _needDownLoadList.Add(bh.Key);
                }
                for (int i = 0; i < _needDownLoadList.Count; i++)
                {
                    DLog.Log("Start Download "+_needDownLoadList[i]);
                    Main.SP.StartCoroutine(DownLoadAssetBundle(_needDownLoadList[i], (s, bytes) =>
                    {
                        _needDownLoadList.Remove(s);
                        DLog.Log(s+" Download Finish");
                       SaveToPersistentPath(s, bytes);
                    }));
                }
                SaveToPersistentPath("assetBundleHash.txt", www.downloadHandler.data);
                www.Dispose();
            }
        }

        private void SaveToPersistentPath(string path, byte[] bytes)
        {
            var psersistentPath = Path.Combine(AssetResources.PersistentUrl, path);
            System.IO.FileInfo file = new System.IO.FileInfo(psersistentPath);
            file.Directory.Create();
            System.IO.File.WriteAllBytes(file.FullName, bytes);
        }
        private IEnumerator DownLoadAssetBundle(string path, Action<string, byte[]> onloadFinish)
        {
            UnityWebRequest www = UnityWebRequest.Get("http://47.94.204.158/AB/" + path);
            yield return www.Send();
            if (www.isError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var data = www.downloadHandler.data;
                www.Dispose();
                onloadFinish.Invoke(path, data);
            }
        }
    }
}
