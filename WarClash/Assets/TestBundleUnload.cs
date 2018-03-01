using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TestBundleUnload : MonoBehaviour
{

	// Use this for initialization
	void OnEnable ()
	{
	    int x = 1;
        Assert.AreEqual(x, 1, "equal ");
	    //var a = GetComponent<Animator>();
	    //   a.Play("Standing 1H Magic Attack 01", -1, 0);
//	    AssetResources.LoadAssetImmediatly("WK_archer.prefab");
	}
	
	// Update is called once per frame
	void OnDisable () {
		//AssetResources.UnloadAsset("WK_archer.prefab");
  //      GC.Collect();
	 //   Resources.UnloadUnusedAssets();
	}
}
