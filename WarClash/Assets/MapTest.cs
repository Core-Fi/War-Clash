using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTest : MonoBehaviour {

    GameMap gm = new GameMap();
	// Use this for initialization
	void Start () {
        gm.CreateMap(new MapRandomConfig() { MainRoomCountRange = new Tuple<int, int>(8, 13) });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
