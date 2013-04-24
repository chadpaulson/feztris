using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour {
	
	public GameObject block;
	private int cubeSide;
	private GameObject selCursor;
	private GameObject selector;
	private GameObject selector2;
	private bool selectorClear; // clean up
	private int selectorMode = 0; // 0 - landscape, 1 - portrait
	private int selectorIndex;
	private int selector2Index; // clean up / consolidate into selector list?
	private float[] selectorSkip;
	private float selAlpha = 0.4f;
	private float cubeAlpha = 1.0f;
	private float bgAlpha = 0.2f;
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
		
		this.selectorSkip = new float[0];
		
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
			toggleSelectorMode();
		}
		
		if(Input.GetButtonDown("Control")) {
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
	
	bool isInBounds(GameObject cursor) {
			
		if(this.cubeSide == 1) {
		
			if(this.selectorMode == 0 && cursor.transform.position.x >= -2f && cursor.transform.position.x < 2f) {
				return true;
			} else if(this.selectorMode == 1 && cursor.transform.position.x >= -2f && cursor.transform.position.x <= 3f) {
				return true;
			}
			
		} else if(this.cubeSide == 2) {

			if(this.selectorMode == 0 && cursor.transform.position.z <= 3f && cursor.transform.position.z > -1f) {
				return true;
			} else if(this.selectorMode == 1 && cursor.transform.position.z <= 3f && cursor.transform.position.z >= -2f) {
				return true;
			}			
			
		} else if(this.cubeSide == 3) {
			
			if(this.selectorMode == 0 && cursor.transform.position.x <= 3f && cursor.transform.position.x > -1f) {
				return true;
			} else if(this.selectorMode == 1 && cursor.transform.position.x <= 3f && cursor.transform.position.x >= -2f) {
				return true;
			}			
			
		} else if(this.cubeSide == 4) {
			
			if(this.selectorMode == 0 && cursor.transform.position.z >= -2f && cursor.transform.position.z < 2f) {
				return true;
			} else if(this.selectorMode == 1 && cursor.transform.position.z >= -2f && cursor.transform.position.z <= 3f) {
				return true;
			}			
			
		}
		
		return false;
		
	}
	
	bool isBlockInSide(GameObject block) {
	
		if(this.cubeSide == 1 && block.transform.position.z == -2f) {
			return true;
		} else if(this.cubeSide == 2 && block.transform.position.x == -2f) {
			return true;
		} else if(this.cubeSide == 3 && block.transform.position.z == 3f) {
			return true;
		} else if(this.cubeSide == 4 && block.transform.position.x == 3f) {
			return true;
		}
		
		return false;
		
	}
	
	void setSelectionPoints(ref float x, ref float z) {
	
		if(this.cubeSide == 1) {
			// lowest x in lh corner
			x = 100f;
			z = -2f;
		} else if(this.cubeSide == 2) {
			// highest z in lh corner
			z = -100f;
			x = -2f;			
		} else if(this.cubeSide == 3) {
			// highest x in lh corner
			x = -100f;
			z = 3f;			
		} else if(this.cubeSide == 4) {	
			// lowest z in lh corner
			z = 100f;
			x = 3f;			
		}		
		
	}
	
	void setSelectionVector(GameObject block, ref float x, ref float z, ref float y) {
		if(block.transform.position.y < y) {
			y = block.transform.position.y;
		}
			
		if(this.cubeSide == 1) {
			
			// lowest x
			if(this.selectorSkip.Length == 2) {
				if(block.transform.position.x > this.selectorSkip[0] && block.transform.position.x < x) {
					x = block.transform.position.x;
				}
			} else if(block.transform.position.x < x) {
				x = block.transform.position.x;
			}				
			
		} else if(this.cubeSide == 2) {
			
			// highest z
			if(this.selectorSkip.Length == 2) {
				if(block.transform.position.z < this.selectorSkip[1] && block.transform.position.z > z) {
					z = block.transform.position.z;	
				}
			} else if(block.transform.position.z > z) {
				z = block.transform.position.z;
			}				
			
		} else if(this.cubeSide == 3) {
			
			// highest x
			if(this.selectorSkip.Length == 2) {
				if(block.transform.position.x < this.selectorSkip[0] && block.transform.position.x > x) {
					x = block.transform.position.x;	
				}
			} else if(block.transform.position.x > x) {
				x = block.transform.position.x;
			}				
			
		} else if(this.cubeSide == 4) {
			
			// lowest z
			if(this.selectorSkip.Length == 2) {
				if(block.transform.position.z > this.selectorSkip[1] && block.transform.position.z < z) {
					z = block.transform.position.z;
				}
			} else if(block.transform.position.z < z) {
				z = block.transform.position.z;
			}				
			
		}
			

		
	}
	
	void newSelection() {
		
		Debug.Log("New Selection Called.");
		
		this.selectorClear = false;
		float x = 0f;
		float z = 0f;
		float y = 10f;
		//bool initial = false;
		
		setSelectionPoints(ref x, ref z);
		
		foreach(GameObject block in blocks) {
			if(isBlockInSide(block)) {
				setAlpha(block, cubeAlpha);
				setSelectionVector(block, ref x, ref z, ref y);
			} else {
				setAlpha(block, bgAlpha);
			}
			
		}
				
		// locate new selector
		foreach(GameObject block in blocks) {
			if(block.transform.position.x == x && block.transform.position.z == z && Mathf.Floor(block.transform.position.y) == Mathf.Floor(y)) {
				if(isValidSelection(block)) {
					updateSelector(block);
				} else {
					this.selectorSkip = new float[2] {block.transform.position.x, block.transform.position.z};
					newSelection();
				}
 				break;
			}
		}
		
		Debug.Log("SIDE: " + cubeSide + " X: " + x + " Y: " + y + " Z: " + z);
		
	}
	
	void toggleSelectorMode() {
			
		if(this.selectorMode == 0) {
			this.selectorMode = 1;
		} else {
			this.selectorMode = 0;
		}
		
		if(isValidSelection(this.selCursor)) {
			updateSelector(this.selCursor);
		} else {
			newSelection();
		}
			
	}	
		
	bool isValidSelection(GameObject cursor) {
		
		GameObject sel = getSelectorHalf(cursor);
		
		if(sel.CompareTag("block")) {
			return true;
		} else {
			return false;
		}
		
	}
	
	GameObject getSelectorHalf(GameObject cursor) {
	
		Vector3 hitDirection = transform.TransformDirection(new Vector3(1, 0, 0));
		RaycastHit hit;
		float hitDistance = 1f;
		
		if(this.selectorMode == 0) {
			hitDirection = transform.TransformDirection(Vector3.right);
		} else if(this.selectorMode == 1) {
			hitDirection = transform.TransformDirection(Vector3.up);
		}
		
		if(Physics.Raycast(cursor.transform.position, hitDirection, out hit, hitDistance)) {
			return hit.collider.gameObject;	
		} else {

			// try opposite direction
			if(this.selectorMode == 0) {
				hitDirection = transform.TransformDirection(Vector3.left);
			} else if(this.selectorMode == 1) {
				hitDirection = transform.TransformDirection(Vector3.down);
			}
			
			if(Physics.Raycast(cursor.transform.position, hitDirection, out hit, hitDistance)) {
				return hit.collider.gameObject;	
			} else {
						
				Debug.Log ("No hit");
				GameObject obj = new GameObject();
				return obj;
				
			}
			
		}
		
			/*
		
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
				
			}*/
		
	}
	
	void moveSelector(string direction) {
			
		if(this.selCursor) {
		
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
			
			if(Physics.Raycast(this.selCursor.transform.position, hitDirection, out hit, hitDistance)) {
			
				if(hit.collider.gameObject.CompareTag("block") && isValidSelection(hit.collider.gameObject)) {
	
					Debug.Log ("attempt");
					updateSelector(hit.collider.gameObject);
					
				} else if(isInBounds(this.selCursor) && direction != "up" && direction != "down") {
			
					this.selCursor = hit.collider.gameObject;
					moveSelector(direction);
					Debug.Log ("In bounds");
					
				} else if(!isInBounds(this.selCursor)) {
					
					this.selCursor = this.selector;
					
				}
				
			} else {
					
				this.selCursor = this.selector;
					
			}
			
		} else {
			
			newSelection();
			
		}
		
	}
	
	void updateSelector(GameObject newSelector) {
	
		if(this.selector) {
			setAlpha(this.selector, cubeAlpha);
		}
		if(this.selector2) {
			setAlpha(this.selector2, cubeAlpha);
		}
		this.selCursor = newSelector;
		this.selector = newSelector;
		this.selector2 = getSelectorHalf(this.selector);
		this.selectorIndex = blocks.IndexOf(newSelector);
		setAlpha(this.selector, selAlpha);
		setAlpha(this.selector2, selAlpha);
		
	}	
	
	public void rotateCube() {
		
		this.selectorSkip = new float[0];
		float angle = Mathf.Floor(transform.rotation.eulerAngles.y);
		
		if(angle == 0f) {
			this.cubeSide = 1;
		} else if(angle == 90f) {
			this.cubeSide = 2;
		} else if(angle == 180f) {
			this.cubeSide = 3;
		} else if(angle == 270f) {
			this.cubeSide = 4;
		}
			
		newSelection();
		
		Debug.Log ("Side: " + this.cubeSide);
		
	}
	
	void setAlpha(GameObject obj, float alpha) {
	
		obj.renderer.material.color = new Color(obj.renderer.material.color.r,
			obj.renderer.material.color.g, obj.renderer.material.color.b, alpha);
		
	}
	
}
