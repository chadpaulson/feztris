using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour {
		
	// Use this for initialization
	void Start () {
	
		Camera cam = gameObject.GetComponent<Camera>();
		#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
		cam.fieldOfView = 60f;
		#endif
		#if UNITY_ANDROID || UNITY_IPHONE
		cam.fieldOfView = 50f;
		#endif
		
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
	
}
