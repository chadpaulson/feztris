using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour {
	
	private GameObject emptyObj;
	private BlockManager blockManager;
	
	// Use this for initialization
	void Start () {
	
		emptyObj = GameObject.Find("EmptyGameObject");
		blockManager = emptyObj.GetComponent<BlockManager>();
		
	}
	
	// Update is called once per frame
	void Update () {
			
		if(Input.GetButtonDown("Rotate")) {
			emptyObj.transform.Rotate(0, 90, 0);
			blockManager.rotateCube();
		}
		
		if(Input.GetButtonDown("RotateBack")) {
			emptyObj.transform.Rotate(0,-90, 0);
			blockManager.rotateCube();
		}
		
	}
	
}
