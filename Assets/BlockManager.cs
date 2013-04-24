using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour {
	
	public GameObject block;
	private int cubeSide;
	private GameObject selector;
	private bool selectorClear;
	private int selectorIndex;
	private float selAlpha = 0.8f;
	private float cubeAlpha = 1.0f;
	private float bgAlpha = 0.45f;
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
		
		if(Input.GetButtonDown("Space")) {
			if(this.selector) {
				this.blocks.RemoveAt(selectorIndex);
				Destroy(this.selector);
				this.selectorClear = true;
				initSelection();
			}
		}
		
		if(Input.GetButtonDown("Command")) {
			initBlocks();
			rotateCube();
		}
		
		

	}
	
	void initSelection() {
	
		StartCoroutine(delaySelection());
		
	}
	
	IEnumerator delaySelection() {
	
		yield return new WaitForSeconds(0.4f);
		if(this.selectorClear) {
			newSelection();
		}
		
	}
	
	Color randColor() {
	
		return new Color(Random.value, Random.value, Random.value);
		
	}
	
	
	void initBlocks() {
		
		for(int i = 0; i < block_coords.Count; i++) {
		
			dropBlock(block_coords[i][0], 0.75f, block_coords[i][1], new Color(Random.value, Random.value, Random.value, 0.05f));
			
		}
		
		for(int i = 0; i < block_coords.Count; i++) {
			
			dropBlock(block_coords[i][0], 1.75f, block_coords[i][1], new Color(Random.value, Random.value, Random.value, 0.05f));
			
		}
		
		for(int i = 0; i < block_coords.Count; i++) {
			
			int r = Random.Range(0,2);			
			if(r == 1) {
				dropBlock(block_coords[i][0], 2.75f, block_coords[i][1], new Color(Random.value, Random.value, Random.value, 0.05f));
			}
			
		}
		
		
		
	}
	
	
	void dropBlock(float x, float y, float z, Color color) {
		
		blocks.Add((GameObject) Instantiate(block, new Vector3(x, y, z), transform.rotation));
		blocks[blocks.Count-1].renderer.material.shader = Shader.Find("Transparent/Diffuse");
		blocks[blocks.Count-1].renderer.material.color = color;
		
	}
	
	
	void newSelection() {
		
		Debug.Log("New Selection Called.");
		
		this.selectorClear = false;
		float z = 0f;
		float x = 0f;
		float y = 10f;
		bool initial = false;
		
		if(this.cubeSide == 1) {
		
			// lowest x in lh corner
			z = -2f;
			foreach(GameObject block in blocks) {
				if(block.transform.position.z == -2f) {
					if(!initial) {
						x = block.transform.position.x;
						initial = true;
					}					
					setAlpha(block, cubeAlpha);
					if(block.transform.position.x < x) {
						x = block.transform.position.x;
					}
					if(block.transform.position.y < y) {
						y = block.transform.position.y;
					}
				} else {
					setAlpha(block, bgAlpha);
				}
				
			}
			
		} else if(this.cubeSide == 2) {
			
			// highest z in lh corner
			x = -2f;
			foreach(GameObject block in blocks) {
				if(block.transform.position.x == -2f) {
					if(!initial) {
						z = block.transform.position.z;	
						initial = true;
					}					
					setAlpha(block, cubeAlpha);
					if(block.transform.position.z > z) {
						z = block.transform.position.z;
					}
					if(block.transform.position.y < y) {
						y = block.transform.position.y;
					}					
				} else {
					setAlpha(block, bgAlpha);
				}
				
			}
			
		} else if(this.cubeSide == 3) {
			
			// highest x in lh corner
			z = 3f;
			foreach(GameObject block in blocks) {
				if(block.transform.position.z == 3f) {
					if(!initial) {
						x = block.transform.position.x;	
						initial = true;
					}					
					setAlpha(block, cubeAlpha);
					if(block.transform.position.x > x) {
						x = block.transform.position.x;
					}
					if(block.transform.position.y < y) {
						y = block.transform.position.y;
					}
				} else {
					setAlpha(block, bgAlpha);
				}
				
			}
			
		} else if(this.cubeSide == 4) {
			
			// lowest z in lh corner
			x = 3f;
			foreach(GameObject block in blocks) {
				if(block.transform.position.x == 3f) {
					if(!initial) {
						z = block.transform.position.z;	
						initial = true;
					}
					setAlpha(block, cubeAlpha);
					if(block.transform.position.z < z) {
						z = block.transform.position.z;
					}
					if(block.transform.position.y < y) {
						y = block.transform.position.y;
					}					
				} else {
					setAlpha(block, bgAlpha);
				}
				
			}
			
		}			
		
		// locate new selector
		foreach(GameObject block in blocks) {
			if(block.transform.position.x == x && block.transform.position.z == z && Mathf.Floor(block.transform.position.y) == Mathf.Floor(y)) {
				this.selector = block;
				this.selectorIndex = blocks.IndexOf(block);
				setAlpha(this.selector, selAlpha);
 				break;
			}
		}
		
		Debug.Log("SIDE: " + cubeSide + " X: " + x + " Y: " + y + " Z: " + z);
		
	}
	
	void moveSelector(string direction) {
			
		if(this.selector) {
		
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
			
			if(Physics.Raycast(this.selector.transform.position, hitDirection, out hit, hitDistance)) {
			
				if(hit.collider.gameObject.CompareTag("block")) {
	
					updateSelector(hit.collider.gameObject);
					
				}
				
			}
			
		} else {
			
			newSelection();
			
		}
		
	}
	
	void updateSelector(GameObject newSelector) {
	
		if(this.selector) {
			setAlpha(this.selector, cubeAlpha);
		}
		this.selector = newSelector;
		this.selectorIndex = blocks.IndexOf(newSelector);
		setAlpha(this.selector, selAlpha);
		
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
			
		newSelection();
		
	}
	
	void setAlpha(GameObject obj, float alpha) {
	
		obj.renderer.material.color = new Color(obj.renderer.material.color.r,
			obj.renderer.material.color.g, obj.renderer.material.color.b, alpha);
		
	}
	
}
