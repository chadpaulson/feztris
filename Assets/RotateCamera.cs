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
		
		#if UNITY_STANDALONE || UNITY_WEBPLAYER
			if(Input.GetButtonDown("Rotate")) {
				emptyObj.transform.Rotate(0, 90, 0);
				blockManager.rotateCube();
			}
			if(Input.GetButtonDown("RotateBack")) {
				emptyObj.transform.Rotate(0,-90, 0);
				blockManager.rotateCube();
			}
		#endif
		

		#if UNITY_ANDROID
			int fingerCount = 0;
	        foreach (Touch touch in Input.touches) {
				if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
					fingerCount++;
	            
			}
	        if (fingerCount > 0) {
				if(blockManager.canRotate) {
					blockManager.canRotate = false;				
					emptyObj.transform.Rotate(0, 90, 0);
					blockManager.rotateCube();
				}
			}
		#endif
		
	}
	
}
