using System.Collections;
using System.Collections.Generic;
using Logic.LogicObject;
using UnityEngine;

public enum CameraMode
{
    FollowPlayer
}
public class CameraController : MonoBehaviour
{
    public float Angle = 60;
    public float Distance = 10;
    public static CameraController Instance;
    public CameraMode CameraMode;
    private Transform _parent;
    void Awake()
    {
        Instance = this;
        _parent = transform.parent;
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
	    if (CameraMode == CameraMode.FollowPlayer)
	    {
	        if (SceneObject.MainPlayer != null)
	        {
	            var posi = SceneObject.MainPlayer.Position.ToVector3();
                Vector3 newPosi = new Vector3(posi.x, Distance, posi.z - Distance / Mathf.Tan(Angle * Mathf.Deg2Rad));
                transform.rotation = Quaternion.Euler(Angle, 0,0);
	            _parent.position =Vector3.Lerp(_parent.position, newPosi, Time.deltaTime*6); 
	        }
	    }
	}
}
