using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour {
	
	public GameObject block;
	private int cubeSide;
	private GameObject selector;
	private List<GameObject> blocks = new List<GameObject>();
	private List<List<float>> block_coords = new List<List<float>>{
	
		new List<float>{3f,-2f},
		new List<float>{3f,-1f},
		new List<float>{3f,0f},
		new List<float>{3f,1f},
		new List<float>{3f,2f},
		new List<float>{3f,3f},
		new List<float>{2f,3f},
		new List<float>{1f,3f},
		new List<float>{0f,3f},
		new List<float>{-1f,3f},
		new List<float>{-2f,3f},
		new List<float>{-2f,2f},
		new List<float>{-2f,1f},
		new List<float>{-2f,0f},
		new List<float>{-2f,-1f},
		new List<float>{-2f,-2f},
		new List<float>{-1f,-2f},
		new List<float>{0f,-2f},
		new List<float>{1f,-2f},
		new List<float>{2f,-2f},
		
	};
	
	
	// Use this for initialization
	void Start () {
		
		//block_coords.Add (new List<float>(new float[] {2f,-1f}));
		initBlocks();
		
		
		
		// init selector
		rotateCube();
		
	}
	
	
	// Update is called once per frame
	void Update () {
	
		
		if(Input.GetButtonDown("Right")) {
			moveSelector("right");
		}
		
		if(Input.GetButtonDown("Left")) {
			moveSelector("left");
		}
		
		if(Input.GetButtonDown("Up")) {
			moveSelector("up");	
		}
		
		if(Input.GetButtonDown("Down")) {
			moveSelector("down");
		}
		
		

	}

	
	Color randColor() {
	
		return new Color(Random.value, Random.value, Random.value);
		
	}
	
	
	void initBlocks() {
		
		for(int i = 0; i < block_coords.Count; i++) {
		
			blocks.Add((GameObject) Instantiate(block, new Vector3(block_coords[i][0], 0.75f, block_coords[i][1]), transform.rotation));
			//blocks[blocks.Count-1].renderer.material.color = randColor();
			
		}
		
		for(int i = 0; i < block_coords.Count; i++) {
		
			blocks.Add((GameObject) Instantiate(block, new Vector3(block_coords[i][0], 1.75f, block_coords[i][1]), transform.rotation));
			//blocks[blocks.Count-1].renderer.material.color = randColor();
			
		}
		
		for(int i = 0; i < block_coords.Count; i++) {
			
			int r = Random.Range(0,2);			
			if(r == 1) {
				blocks.Add((GameObject) Instantiate(block, new Vector3(block_coords[i][0], 2.75f, block_coords[i][1]), transform.rotation));
				//blocks[blocks.Count-1].renderer.material.color = randColor();				
			}
			
		}
		
		
		
	}
	
	
	void dropBlocks() {
		
		
		
	}
	
	
	void initSelection() {
	
		float z = 0f;
		float x = 0f;
		float y = 10f;
		
		if(this.cubeSide == 1) {
		
			// lowest x in lh corner
			z = -2f;
			foreach(GameObject block in blocks) {
			
				if(block.transform.position.z == -2f) {
					block.renderer.material.color = Color.green;
					if(block.transform.position.x < x) {
						x = block.transform.position.x;
					}
					if(block.transform.position.y < y) {
						y = block.transform.position.y;
					}
				} else {
					block.renderer.material.color = Color.grey;
				}
				
			}
			
		} else if(this.cubeSide == 2) {
			
			// highest z in lh corner
			x = -2f;
			foreach(GameObject block in blocks) {
			
				if(block.transform.position.x == -2f) {
					block.renderer.material.color = Color.green;
					if(block.transform.position.z > z) {
						z = block.transform.position.z;
					}
					if(block.transform.position.y < y) {
						y = block.transform.position.y;
					}					
				} else {
					block.renderer.material.color = Color.grey;
				}
				
			}
			
		} else if(this.cubeSide == 3) {
			
			// highest x in lh corner
			z = 3f;
			foreach(GameObject block in blocks) {
			
				if(block.transform.position.z == 3f) {
					block.renderer.material.color = Color.green;
					if(block.transform.position.x > x) {
						x = block.transform.position.x;
					}
					if(block.transform.position.y < y) {
						y = block.transform.position.y;
					}
				} else {
					block.renderer.material.color = Color.grey;
				}
				
			}
			
		} else if(this.cubeSide == 4) {
			
			// lowest z in lh corner
			x = 3f;
			foreach(GameObject block in blocks) {
			
				if(block.transform.position.x == 3f) {
					block.renderer.material.color = Color.green;
					if(block.transform.position.z < z) {
						z = block.transform.position.z;
					}
					if(block.transform.position.y < y) {
						y = block.transform.position.y;
					}					
				} else {
					block.renderer.material.color = Color.grey;
				}
				
			}
			
		}			
			
		foreach(GameObject block in blocks) {
			if(block.transform.position.x == x && block.transform.position.z == z && Mathf.Floor(block.transform.position.y) == Mathf.Floor(y)) {
				this.selector = block;
				//block.renderer.material.color = Color.magenta;
				//Debug.Log ("Found");
				break;
			}
		}
			
		
		//Debug.Log ("T: " + Time.time + " X: " + x + " Z: " + z + " Y: " + y);

		
		this.selector.renderer.material.color = Color.yellow;
		
	}
	
	void moveSelector(string direction) {
	
		Vector3 hitDirection = transform.TransformDirection(new Vector3(1, 0, 0));
		RaycastHit hit;
		float hitDistance = 20f;
		
		if(direction == "right") {
			hitDirection = transform.TransformDirection(Vector3.right);
		} else if(direction == "left") {
			hitDirection = transform.TransformDirection(Vector3.left);
		} else if(direction == "up") {
			hitDirection = transform.TransformDirection(Vector3.up);
		} else if(direction == "down") {
			hitDirection = transform.TransformDirection(Vector3.down);
		}
		
		if(Physics.Raycast(selector.transform.position, hitDirection, out hit, hitDistance)) {
		
			if(hit.collider.gameObject) {
			
				//hit.collider.gameObject.renderer.material.color = Color.blue;
				updateSelector(hit.collider.gameObject);
				
			}
			
		}
		
	}
	
	void updateSelector(GameObject newSelector) {
	
		selector.renderer.material.color = Color.green;
		selector = newSelector;
		selector.renderer.material.color = Color.yellow;
		
	}	
	
	public void rotateCube() {
		
		float angle = Mathf.Floor(transform.rotation.eulerAngles.y);
		
		if(angle == 0f) {
			cubeSide = 1;
		} else if(angle == 90f) {
			cubeSide = 2;
		} else if(angle == 180f) {
			cubeSide = 3;
		} else if(angle == 270f) {
			cubeSide = 4;
		}
			
		initSelection();
		
	}
	
	
}
