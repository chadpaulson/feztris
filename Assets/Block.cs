using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block : MonoBehaviour {
	
	
	//public BlockManager manager;
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
			
		if(manager.newCubes && gameObject.rigidbody.velocity.y < 0f && manager.blockFall) {
			manager.disableBlockFall();			
			//Debug.Log (gameObject.rigidbody.velocity.y);
			blockFall.Play();
		}
		
		/*
		if(this.manager.newCubes) {
			blockFall.Play();
		} else {
			Debug.Log ("NOpe");
		}*/
		
		/*
		List<GameObject> blocks = new List<GameObject>{
			gameObject,
			col.collider.gameObject,
		};
		
		if(manager.newCubes) {
			if(manager.clearBlocks(blocks: blocks)) {
				manager.selectorClear = true;
				manager.initSelection();
			}
		}*/
		
	}

	
}
