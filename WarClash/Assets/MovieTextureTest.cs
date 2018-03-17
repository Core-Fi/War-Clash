using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MovieTextureTest : MonoBehaviour {
    public VideoPlayer vp;
    public RawImage image;
    private RenderTexture rt;
	// Use this for initialization
	void OnEnable () {
        rt = new RenderTexture(300, 300, 0);
        vp.renderMode = VideoRenderMode.RenderTexture;
        vp.targetTexture = rt;
        image.texture = rt;
	}
    private void OnDisable()
    {
        Debug.Log("on disable");
        image.texture = null;
        vp.targetTexture = null;
        rt.Release();
        Object.Destroy(rt);
        Resources.UnloadUnusedAssets();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
