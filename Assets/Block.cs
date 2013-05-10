using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block : MonoBehaviour {
	

	public AudioSource blockFall;
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
	
			
	void OnCollisionExit(Collision col) {
		
		if(col.gameObject.CompareTag("block") || col.gameObject.CompareTag("ground")) {
			if(manager.newCubes && gameObject.rigidbody.velocity.y >= 0f && manager.blockFall) {
				manager.disableBlockFall();			
				blockFall.Play();
			}
		}
				
	}
	
	
	public void nukeBlock() {
	
		StartCoroutine(delayNuke());
		
	}
	
	
	IEnumerator delayNuke() {
	
		yield return new WaitForSeconds(0.2f);
		manager.removeBlock(gameObject);
		
	}

	
}
