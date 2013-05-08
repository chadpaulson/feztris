using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour {
	
	
	public GameObject block;
	public Texture selectorTex;
	public Texture selectorTex2;
	public bool selectorClear;
	public bool newCubes = false;
	public AudioSource blockClear;
	public bool blockFall = true;
	private int lastColorIndex;
	private int cubeSide;
	private int pivots = 0;
	private GameObject selCursor;
	private GameObject selector;
	private GameObject selector2;
	private int selectorMode = 0; // 0 - landscape, 1 - portrait
	private int selectorIndex;
	private int selector2Index;
	private float[] selectorSkip = new float[0];
	private float cubeAlpha = 1.0f;
	private float bgAlpha = 0.6f;
	private List<GameObject> blocks = new List<GameObject>();
	private const GameObject defaultBlock = null;
	private const List<GameObject> defaultBlocks = null;	
	private List<Color> blockColors = new List<Color>{
		//new Color(255f/255f, 46f/255f, 3f/255f), // red
		new Color(40f/255f, 130f/255f, 51f/255f), // green
		//new Color(56f/255f, 98f/255f, 252f/255f), // blue		
		new Color(56f/255f, 243f/255f, 252f/255f), // aqua
		new Color(88f/255f, 219f/255f, 103f/255f), // light green
		new Color(237f/255f, 231f/255f, 161f/255f), // light yellow / tan
		new Color(245f/255f, 225f/255f, 51f/255f), // bright yellow
		new Color(232f/255f, 58f/255f, 229f/255f), // purple
	};
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
		
		initBlocks();
		
		rotateCube();
		
		initRepeats();

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
		
		if(Input.GetButtonDown("SwapBlocks")) {
			swapIt();
		}
		
		if(Input.GetButtonDown("ToggleSelector")) {
			toggleSelectorMode();
		}
		
		if(Input.GetButtonDown("Reset")) {
			initReset();	
		}
		
		clearAllBlocks();

	}
	
	
	void reset() {
			
		int count = blocks.Count;
		List<GameObject> sons = new List<GameObject>();
		foreach(GameObject b in blocks) {
			sons.Add(b);
		}
		
		
		if(count > 0) {

			for(int i = 0; i < count; i++) {				

				removeBlock(sons[i]);
			}
			
		}
				
		//block_coords.Add (new List<float>(new float[] {2f,-1f}));
		initBlocks();
		
		// init selector
		rotateCube();
		
		initRepeats();			
				
	}
	
	
	void enableBlockFall() {
	
		this.blockFall = true;
		
	}
	
	public void disableBlockFall() {
	
		this.blockFall = false;
		
	}
	
	
	void createColors() {
	
		for(int i = 0; i < 7; i++) {
			this.blockColors.Add(new Color(Random.value, Random.value, Random.value));
		}
		
	}
	
	
	public void checkSelection() {
	
		StartCoroutine(delaySelection());
		
	}
	
	
	IEnumerator delaySelection() {
	
		yield return new WaitForSeconds(0.1f);
		if(this.selectorClear) {
			if(this.selector && !this.selector2) {
				updateSelector(this.selector);
			} else if(this.selector2 && !this.selector) {
				updateSelector(this.selector2);
			} else if(!this.selector && !this.selector2) {
				newSelection();
			}
		}
		
	}
	
	void initReset() {
	
		StartCoroutine(delayReset());
		
	}
	
	
	void initRepeats() {
		
		InvokeRepeating("newBlock", 2f, 1.5f);
		//InvokeRepeating("clearAllBlocks", 0.5f, 0.1f);
		InvokeRepeating("enableBlockFall", 0.5f, 1f);
		
	}
	
	
	IEnumerator delayReset() {
	
		newCubes = false;
		CancelInvoke();
		yield return new WaitForSeconds(2f);
		reset();
		
	}
	
	
	Color randColor() {
	
		int i = Random.Range(0, this.blockColors.Count);
		
		if(i == this.lastColorIndex) {
			randColor();
		} else {
			this.lastColorIndex = i;	
		}
		
		return this.blockColors[i];
		
	}
	
	
	void initBlocks() {
		
		for(int i = 0; i < block_coords.Count; i++) {
		
			dropBlock(block_coords[i][0], 0.75f, block_coords[i][1], randColor());
			
		}
		
		for(int i = 0; i < block_coords.Count; i++) {
			
			dropBlock(block_coords[i][0], 1.75f, block_coords[i][1], randColor());
			
		}

		for(int i = 0; i < block_coords.Count; i++) {
			
			dropBlock(block_coords[i][0], 2.75f, block_coords[i][1], randColor());
			
		}
		
		for(int i = 0; i < block_coords.Count; i++) {
			
			dropBlock(block_coords[i][0], 3.75f, block_coords[i][1], randColor());
			
		}		
		
		for(int i = 0; i < block_coords.Count; i++) {
			
			int r = Random.Range(0,2);			
			if(r == 1) {
				dropBlock(block_coords[i][0], 4.75f, block_coords[i][1], randColor());
			}
			
		}
		
		
		
	}
	
	void newBlock() {
	
		if(!newCubes) {
			newCubes = true;
		}
		
		int i = Random.Range(0, block_coords.Count);
		dropBlock(block_coords[i][0], 19f, block_coords[i][1], randColor());
		
	}
	
	
	void dropBlock(float x, float y, float z, Color color) {
		
		blocks.Add((GameObject) Instantiate(block, new Vector3(x, y, z), transform.rotation));
		blocks[blocks.Count-1].renderer.material.shader = Shader.Find("Transparent/Diffuse");
		blocks[blocks.Count-1].renderer.material.color = color;
		
		// manage initial block colors
		if(!newCubes) {
			List<GameObject> matching = getMatching(block: blocks[blocks.Count-1]);
			if(matching.Count > 0) {
				foreach(GameObject match in matching) {
					match.renderer.material.color = randColor();
				}
			}
		}
		
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
		
		this.selectorClear = false;
		float x = 0f;
		float z = 0f;
		float y = 10f;

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

				GameObject obj = new GameObject();
				return obj;
				
			}
			
		}
		
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
	
					updateSelector(hit.collider.gameObject);
					
				} else if(isInBounds(this.selCursor) && direction != "up" && direction != "down") {
			
					this.selCursor = hit.collider.gameObject;
					moveSelector(direction);
					
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
	
		this.selCursor = newSelector;
		GameObject sel2 = getSelectorHalf(newSelector);
		
		if(sel2.CompareTag("block")) {
		
			this.selectorClear = false;
			if(this.selector) {
				this.selector.renderer.material.shader = Shader.Find("Transparent/Diffuse");
			}
			if(this.selector2) {
				this.selector2.renderer.material.shader = Shader.Find("Transparent/Diffuse");
			}
			this.selector = newSelector;
			this.selector2 = sel2;
			this.selectorIndex = blocks.IndexOf(newSelector);
			this.selector.renderer.material.shader = Shader.Find("Decal");
			this.selector.renderer.material.SetTexture("_DecalTex", this.selectorTex2);
			this.selector2.renderer.material.shader = Shader.Find("Decal");
			this.selector2.renderer.material.SetTexture("_DecalTex", this.selectorTex2);
			
		} else {
		
			toggleSelectorMode();
			
		}
		
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
		
	}
	
	
	void clearAllBlocks() {
		
		// check for matches
		List<GameObject> vBlocks = new List<GameObject>();
		foreach(GameObject block in blocks) {
			if(isBlockInSide(block)) {
				vBlocks.Add(block);
			}
			
		}
		if(clearBlocks(blocks: vBlocks)) {
			blockClear.PlayDelayed(0.05f);
			this.selectorClear = true;
			checkSelection();
		}
		
	}
	
	
	void setAlpha(GameObject obj, float alpha) {
	
		obj.renderer.material.color = new Color(obj.renderer.material.color.r,
			obj.renderer.material.color.g, obj.renderer.material.color.b, alpha);
		
	}
	
	
	void swapIt() {
	
		// swap colors
		if(this.selector && this.selector2) {
			Color selColor = this.selector.renderer.material.color;
			Color sel2Color = this.selector2.renderer.material.color;
			this.selector2.renderer.material.color = selColor;
			this.selector.renderer.material.color = sel2Color;			
		}
		
	}
	
	
	public bool clearBlocks(GameObject block = defaultBlock, List<GameObject> blocks = defaultBlocks) {
	
		List<GameObject> matches = new List<GameObject>();
		
		if(block != null && blocks == null) {
			matches = getMatching(block: block);
		} else if(block == null && blocks != null) {
			matches = getMatching(blocks: blocks);
		}
				
		if(matches.Count > 0) {
			
			foreach(GameObject match in matches) {
				removeBlock(match);
			}
			
			//Debug.Log ("Adios! (" + matches.Count + ")");
						
			return true;
			
		}
		
		return false;
		
	}
	
	
	void removeBlock(GameObject block) {
		
		int i = this.blocks.IndexOf(block);
		
		if(blocks[i]) {
			Destroy(this.blocks[i]);
			this.blocks.RemoveAt(i);
		}
		
	}
	
	
	GameObject matchNext(GameObject block, ref Vector3 direction) {
		
		Vector3 hitDirection = transform.TransformDirection(direction);
		RaycastHit hit;
		float hitDistance = 1f;
				
		if(Physics.Raycast(block.transform.position, hitDirection, out hit, hitDistance)) {
			
			if(hit.collider.gameObject.CompareTag("block") && 
				hit.collider.gameObject.renderer.material.color.r == block.renderer.gameObject.renderer.material.color.r && 
				hit.collider.gameObject.renderer.material.color.g == block.renderer.gameObject.renderer.material.color.g &&
				hit.collider.gameObject.renderer.material.color.b == block.renderer.gameObject.renderer.material.color.b &&
				hit.collider.gameObject.rigidbody.velocity.y >= 0f) {
			
				return hit.collider.gameObject;
				
			}
		
		} else if((direction == Vector3.left || direction == Vector3.right) && this.pivots == 0) {
			
			this.pivots = 1;
			hitDirection = transform.TransformDirection(Vector3.forward);
			
			if(Physics.Raycast(block.transform.position, hitDirection, out hit, hitDistance)) {
				
				if(hit.collider.gameObject.CompareTag("block") && 
					hit.collider.gameObject.renderer.material.color.r == block.renderer.gameObject.renderer.material.color.r && 
					hit.collider.gameObject.renderer.material.color.g == block.renderer.gameObject.renderer.material.color.g &&
					hit.collider.gameObject.renderer.material.color.b == block.renderer.gameObject.renderer.material.color.b &&
					hit.collider.gameObject.rigidbody.velocity.y >= 0f) {
				
					direction = Vector3.forward;
					return hit.collider.gameObject;
					
				}
			
			}
			
		}

		GameObject obj = new GameObject();
		return obj;
		
	}
	
	
	List<GameObject> getMatching(GameObject block = defaultBlock, List<GameObject> blocks = defaultBlocks) {
		
		List<GameObject> matchingBlocks = new List<GameObject>();
		
		if(block != null && blocks == null) {
			matchingBlocks.Add(block);
		} else if(block == null && blocks.Count > 0) {
			matchingBlocks = blocks;	
		}

		List<GameObject> hor = new List<GameObject>();
		List<GameObject> ver = new List<GameObject>();
		List<GameObject> matches = new List<GameObject>();
		
		foreach(GameObject matchingBlock in matchingBlocks) {

			hor = new List<GameObject>();
			ver = new List<GameObject>();
			bool addBlock = false;
			
			// check up
			Vector3 upDir = Vector3.up;
			GameObject upMatch = matchNext(matchingBlock, ref upDir);
			while(upMatch.CompareTag("block")) {
			
				if(!hor.Contains(upMatch)) {
					hor.Add(upMatch);
				}
				upMatch = matchNext(upMatch, ref upDir);
				
			}
	
			// check down
			Vector3 downDir = Vector3.down;
			GameObject downMatch = matchNext(matchingBlock, ref downDir);
			while(downMatch.CompareTag("block")) {
			
				if(!hor.Contains(downMatch)) {
					hor.Add(downMatch);
				}
				downMatch = matchNext(downMatch, ref downDir);
				
			}
	
			// check left
			Vector3 leftDir = Vector3.left;
			GameObject leftMatch = matchNext(matchingBlock, ref leftDir);
			while(leftMatch.CompareTag("block")) {
			
				if(!ver.Contains(leftMatch)) {
					ver.Add(leftMatch);
				}
				leftMatch = matchNext(leftMatch, ref leftDir);
				
			}
			this.pivots = 0;
	
			// check right
			Vector3 rightDir = Vector3.right;
			GameObject rightMatch = matchNext(matchingBlock, ref rightDir);
			while(rightMatch.CompareTag("block")) {
			
				if(!ver.Contains(rightMatch)) {
					ver.Add(rightMatch);
				}
				rightMatch = matchNext(rightMatch, ref rightDir);
				
			}
			this.pivots = 0;
					
			if(hor.Count >= 2) {
				foreach(GameObject h in hor) {
					if(!matches.Contains(h)) {
						matches.Add(h);
						addBlock = true;
					}
				}
			}
			
			if(ver.Count >=2) {
				foreach(GameObject v in ver) {
					if(!matches.Contains(v)) {
						matches.Add(v);
						addBlock = true;
					}
				}
			}
			
			if(addBlock && !matches.Contains(matchingBlock)) {
				matches.Add(matchingBlock);	
			}
			
			//Debug.Log ("H: " + hor.Count + " V: " + ver.Count);
			
		}
		
		return matches;
		
	}
	
	
}
