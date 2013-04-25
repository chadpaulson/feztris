using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block : MonoBehaviour {
	
	
	public BlockManager manager;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	void OnCollisionEnter(Collision col) {
	
		List<GameObject> blocks = new List<GameObject>{
			gameObject,
			col.collider.gameObject,
		};
		
		if(manager.newCubes) {
			if(manager.clearBlocks(blocks: blocks)) {
				manager.selectorClear = true;
				manager.initSelection();
			}
		}
		
	}

	
}
