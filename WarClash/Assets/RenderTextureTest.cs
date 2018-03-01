using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextureTest : MonoBehaviour {
    public Camera camera;
    public RawImage rawImage;
    // Use this for initialization
    public RenderTexture renderTexture;
    void OnEnable () {
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        camera.targetTexture = renderTexture;
       // camera.Render();
        //RenderTexture.active = renderTexture;
        //RenderTexture.active = null;
        //camera.targetTexture = null;
        rawImage.texture = renderTexture;
    }

    void OnDisable()
    {
        camera.targetTexture = null;
        renderTexture.Release();
        // camera.targetTexture.Release();
    }
	// Update is called once per frame
	
}
