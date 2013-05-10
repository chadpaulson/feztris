using UnityEngine;
using System.Collections;

public class Invader : MonoBehaviour {
	
	private BlockManager manager;
	
	void Awake() {
		GameObject cubeManager = GameObject.FindGameObjectWithTag("manager");
		manager = cubeManager.GetComponent<BlockManager>();	
	}	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void nukeMe() {
		
		manager.removeInvader(gameObject);
		
	}
	
	void OnCollisionEnter(Collision col) {
		
		//Debug.Log ("HIT");
		
		/*
		if(col.gameObject.CompareTag("ground")) {
			Debug.Log("Hit Ground");
		}*/
		
		if(col.gameObject.CompareTag("block")) {
			
			//Debug.Log("Hit Block");
			
			/*if(col.gameObject.rigidbody.velocity.x > 0.6f || col.gameObject.rigidbody.velocity.z > 0.6f || 
				col.gameObject.rigidbody.velocity.x < -0.6f || col.gameObject.rigidbody.velocity.z < -0.6f) {*/
			
			if(col.relativeVelocity.magnitude > 2f) {
			
				gameObject.rigidbody.constraints = RigidbodyConstraints.None;
				gameObject.rigidbody.AddForce(transform.TransformDirection(Vector3.forward) * 33000);
				Invoke("nukeMe", 1f);
			
			}/* else {
			
				Debug.Log ("Conditional Not Met");
				
			}*/
			
			
		}
		
	}
	
}
