using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour {
	
	
	public GameObject block;
	public GameObject invader;
	public Texture selectorTex;
	public Texture selectorTex2;
	public bool selectorClear;
	public bool newCubes = false;	
	public bool blockFall = true;
	public bool canRotate = true;
	public bool cubeRotation = false;
	public bool colorSwap = false;
	public int cubeSide;
	public AudioSource invaderHit;
	private int score = 0;
	private GUIText modeDisplay;	
	private int gameMode = 0; // 0 - default, 1 - invader
	private int lastColorIndex;
	private int pivots = 0;
	private GameObject selCursor;
	private GameObject selector;
	private GameObject selector2;
	private GameObject blnk;
	private AudioSource blockClear;
	private AudioSource blockPop;
	private int selectorMode = 0; // 0 - landscape, 1 - portrait
	private int selector2Index;
	private float[] selectorSkip = new float[0];
	private float cubeAlpha = 1.0f;
	private float bgAlpha = 0.4f;
	private List<GameObject> blocks = new List<GameObject>();
	private List<GameObject> invaders = new List<GameObject>();
	private const GameObject defaultBlock = null;
	private const List<GameObject> defaultBlocks = null;	
	private List<Color> blockColors = new List<Color>{
		new Color(255f/255f, 46f/255f, 3f/255f), // red
		new Color(40f/255f, 130f/255f, 51f/255f), // green
		new Color(56f/255f, 98f/255f, 252f/255f), // blue		
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
	#if UNITY_ANDROID
	private Touch firstTouch;
	private bool touchStart = false;
	private GameObject touchBlock = null;
	#endif
	
	// Use this for initialization
	void Start () {
		
		GameObject modeGUI = GameObject.FindGameObjectWithTag("modeD");
		this.modeDisplay = modeGUI.GetComponent<GUIText>();
		this.modeDisplay.text = "Mode: Classic";
		
		this.blnk = new GameObject();
		
		AudioSource[] audioSources = (AudioSource[])GetComponents<AudioSource>();
		this.blockClear = audioSources[0];
		this.blockPop = audioSources[1];
		this.invaderHit = audioSources[2];
		
		initBlocks();
		
		this.initCube();
		
		initRepeats();
		
		Screen.sleepTimeout = 0;
		
	}
	
	
	// Update is called once per frame
	void Update () {
		
		#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER
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
			if(Input.GetButtonDown("ToggleMode")) {
				toggleGameMode();
			}
			if(Input.GetKey("escape")) {
				System.Diagnostics.Process.GetCurrentProcess().Kill();	
			}
			if(Input.GetButtonDown("Rotate")) {
				int newSide = this.rotateCube("right");
				StartCoroutine(moveCube(this.cubeSide, newSide));
			}
			if(Input.GetButtonDown("RotateBack")) {
				int newSide = this.rotateCube("left");
				StartCoroutine(moveCube(this.cubeSide, newSide));
			}		
		#endif

		
		#if UNITY_ANDROID
	        foreach (Touch touch in Input.touches) {
				if(touch.phase == TouchPhase.Began && !this.touchStart && !this.colorSwap) {
					this.firstTouch = touch;
					this.touchStart = true;
					RaycastHit touchHit;
					Ray touchRay = Camera.main.ScreenPointToRay(touch.position);
					if(Physics.Raycast(touchRay, out touchHit, Mathf.Infinity)) {
						this.touchBlock = touchHit.rigidbody.gameObject;
					}	
				}
				if(touch.phase == TouchPhase.Ended && this.touchStart && this.touchBlock == null) {
					if(touch.position.x > this.firstTouch.position.x) {
						int newSide = this.rotateCube("right");
						StartCoroutine(moveCube(this.cubeSide, newSide));					
					} else {
						int newSide = this.rotateCube("left");
						StartCoroutine(moveCube(this.cubeSide, newSide));				
					}				
				}
				if(touch.phase == TouchPhase.Ended && this.touchStart && this.touchBlock != null && !this.colorSwap) {
					float hDiff;
					float vDiff;
					string[] swipeDir = new string[2];
					Vector3	hDirection;
					if(touch.position.x > this.firstTouch.position.x) {
						hDiff = touch.position.x - this.firstTouch.position.x;
						swipeDir[0] = "right";
					} else {
						hDiff = this.firstTouch.position.x - touch.position.x;
						swipeDir[0] = "left";
					}
					if(touch.position.y > this.firstTouch.position.y) {
						vDiff = touch.position.y - this.firstTouch.position.y;
						swipeDir[1] = "up";
					} else {
						vDiff = this.firstTouch.position.y - touch.position.y;
						swipeDir[1] = "down";
					}
					if(Mathf.Abs(hDiff) > Mathf.Abs(vDiff)) {	
						if(swipeDir[0] == "right") {
							hDirection = Vector3.right;
						} else {
							hDirection = Vector3.left;
						}
					} else {
						if(swipeDir[1] == "up") {
							hDirection = Vector3.up;
						} else {
							hDirection = Vector3.down;
						}
					}
					GameObject sel2 = this.getSelectorTouch(this.touchBlock, hDirection);
					if(sel2.CompareTag("block")) {
						this.selector = this.touchBlock;
						this.selector2 = sel2;
						this.swapIt();
					}	
					this.touchBlock = null;
				}		
				if(touch.phase == TouchPhase.Ended) {
					this.touchStart = false;;	
				}
			}		
			if(Input.GetKeyDown(KeyCode.Escape)) {
				Application.Quit();
			}			
		#endif
				
	}
	
	
	void FixedUpdate() {
		
		//clearAllBlocks();
		
	}
	

	void toggleGameMode() {
	
		this.modeDisplay.text = "";
		
		if(this.gameMode == 0) {
			this.gameMode = 1;
		} else {
			this.gameMode = 0;
		}
		
		initReset();
		
	}
	
	
	void reset() {
			
		this.score = 0;
		this.modeDisplay.text = "Score: " + this.score.ToString("N0");
		
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
		

		int c = this.invaders.Count;
		List<GameObject> invaderClones = new List<GameObject>();
		foreach(GameObject invader in this.invaders) {
			invaderClones.Add(invader);
		}
		
		if(c > 0) {
			
			for(int i = 0; i < c; i++) {
				removeInvader(invaderClones[i]);	
			}
			
		}		
		
		if(this.gameMode == 1) {
					
			this.modeDisplay.text = "Mode: Invader";
			
		} else {
			
			this.modeDisplay.text = "Mode: Classic";
			
		}
		
		
		initBlocks();
		
		// init selector
		this.initCube();
		
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
	
		if(!this.colorSwap) {
			StartCoroutine(delaySelection());
		}
		
	}
	
	
	IEnumerator delaySelection() {
	
		if(!this.colorSwap) {
			if(this.gameMode == 0) {
				yield return new WaitForSeconds(0.1f);
			} else if(this.gameMode == 1) {
				yield return new WaitForSeconds(0.3f);
			}
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
		
	}
	
	void initReset() {
	
		StartCoroutine(delayReset());
		
	}
	
	void enableRotation() {
		this.canRotate = true;
	}
	
	void initRepeats() {
		
		InvokeRepeating("newBlock", 2f, 1.6f);
		if(this.gameMode == 1) {
			InvokeRepeating("newInvader", 3f, 8f);	
		}
		InvokeRepeating("enableBlockFall", 0.5f, 3.5f);
		#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBPLAYER
			InvokeRepeating("clearAllBlocks", 0.5f, 0.2f);
		#endif
		#if UNITY_ANDROID
			InvokeRepeating("clearAllBlocks", 0.5f, 0.55f);
			InvokeRepeating("enableRotation", 0.5f, 1.0f);
		#endif
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
		
		/*
		for(int i = 0; i < block_coords.Count; i++) {
			
			dropBlock(block_coords[i][0], 3.75f, block_coords[i][1], randColor());
			
		}		

		
		for(int i = 0; i < block_coords.Count; i++) {
			
			dropBlock(block_coords[i][0], 4.75f, block_coords[i][1], randColor());
			
		}

		for(int i = 0; i < block_coords.Count; i++) {
			
			dropBlock(block_coords[i][0], 5.75f, block_coords[i][1], randColor());
			
		}*/
		
		for(int i = 0; i < block_coords.Count; i++) {
			
			int r = Random.Range(0,2);			
			if(r == 1) {
				dropBlock(block_coords[i][0], 3.75f, block_coords[i][1], randColor());

			}
			
		}
		
		StartCoroutine(moveCube(1,1,0f));
		
	}
	
	
	void newInvader() {
	
		int i = Random.Range(0, block_coords.Count);
		List<float> invader_coords = setInvaderCoords(block_coords[i][0], block_coords[i][1]);
		dropInvader(invader_coords[0], 19f, invader_coords[1]);
		
	}
	
	
	void dropInvader(float x, float y, float z) {
		
		invaders.Add((GameObject) Instantiate(invader, new Vector3(x, y, z), transform.rotation));
		
	}
	
	
	List<float> setInvaderCoords(float x, float z) {
		
		
		int i = Random.Range(0,2);		
		
		
		if(x == 3f && z == 3f) {
			if(i == 0) {
				x += 0.7f;	
			} else {
				z += 0.7f;
			}
		} else if(x == -2 && z == -2) {
			if(i == 0) {
				x += -0.7f;	
			} else {
				z += -0.7f;
			}			
		} else if(x == 3) {
			x += 0.7f;
		} else if(z == 3) {
			z += 0.7f;
		} else if(x == -2) {
			x += -0.7f;
		} else if(z == -2) {
			z += -0.7f;
		}
		
		return new List<float>{x, z};
		
	}
	
	
	void newBlock() {
	
		if(!newCubes) {
			newCubes = true;
		}
		
		if(!this.cubeRotation) {
			int i = Random.Range(0, block_coords.Count);
			dropBlock(block_coords[i][0], 19f, block_coords[i][1], randColor());
		}
		
	}
	
	
	void dropBlock(float x, float y, float z, Color color) {
		
		blocks.Add((GameObject) Instantiate(block, new Vector3(x, y, z), transform.rotation));
		blocks[blocks.Count-1].renderer.material.shader = Shader.Find("Transparent/Diffuse");
		blocks[blocks.Count-1].renderer.material.color = color;
		if(isBlockInSide(blocks[blocks.Count-1])) {
			setAlpha(blocks[blocks.Count-1], cubeAlpha);
		} else {
			setAlpha(blocks[blocks.Count-1], bgAlpha);
		}
				
		
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
		
		if(!this.colorSwap) {
		
			this.selectorClear = false;
			float x = 0f;
			float z = 0f;
			float y = 10f;
	
			setSelectionPoints(ref x, ref z);
			
			foreach(GameObject block in blocks) {
				
				setSelectionVector(block, ref x, ref z, ref y);
				
				/*
				if(isBlockInSide(block)) {
					setAlpha(block, cubeAlpha);
					setSelectionVector(block, ref x, ref z, ref y);
				} else {
					setAlpha(block, bgAlpha);
				} */
				
			}
			
			#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
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
			#endif
			
		}
		
	}
	
	
	void toggleSelectorMode() {
		
		if(!this.colorSwap) {
		
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
			
	}	
	
	
	bool isValidSelection(GameObject cursor) {
		
		GameObject sel = getSelectorHalf(cursor);
		
		if(sel.CompareTag("block")) {
			return true;
		} else {
			return false;
		}
		
	}
	
	#if UNITY_ANDROID
	GameObject getSelectorTouch(GameObject cursor, Vector3 hDirection) {
	
		RaycastHit hit;
		float hitDistance = 1f;
		Vector3 hitDirection = transform.TransformDirection(hDirection);
		if(Physics.Raycast(cursor.transform.position, hitDirection, out hit, hitDistance)) {
			return hit.collider.gameObject;	
		} else {
			return this.blnk;
		}
		
	}	
	#endif
	
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

				return this.blnk;
				
			}
			
		}
		
	}
	
	
	void moveSelector(string direction) {
			
		if(this.selCursor && !this.colorSwap) {
		
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
			
		} else if(!this.colorSwap) {
			
			newSelection();
			
		}
		
	}
	
	
	void updateSelector(GameObject newSelector) {
	
		if(!this.colorSwap) {
		
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
				this.selector.renderer.material.shader = Shader.Find("Decal");
				this.selector.renderer.material.SetTexture("_DecalTex", this.selectorTex2);
				this.selector2.renderer.material.shader = Shader.Find("Decal");
				this.selector2.renderer.material.SetTexture("_DecalTex", this.selectorTex2);
				
			} else {
			
				toggleSelectorMode();
				
			}

		}
		
	}	
	
	
	private void updateCubeSide() {
	
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
		
	}
	
	
	private float getAngleForCubeSide(int cubeSide) {
		
		if(cubeSide == 1){
			return 0f;
		} else if(cubeSide == 2) {
			return 90f;
		} else if(cubeSide == 3) {
			return 180f;
		} else if(cubeSide == 4) {
			return 270f;	
		}
		
		return -1f;
		
	}
	
	
	private void initCube() {
		
		this.updateCubeSide();
		this.selectorSkip = new float[0];
		if(!this.colorSwap) {
			newSelection();
		}
		
	}
	
	
	private IEnumerator swapColors(Color selColor, Color sel2Color, float swapTime = 0.283f) {
		
		if(this.colorSwap) {
		
			float i = 0f;
			float rate = 1f/swapTime;
			
			while(i < 1f) {
			
				i = i + Time.deltaTime * rate;
				if(this.selector2) {
					this.selector2.renderer.material.color = Color.Lerp(this.selector2.renderer.material.color, selColor, Mathf.SmoothStep(0f, 1f, i));	
				}
				if(this.selector) {
					this.selector.renderer.material.color = Color.Lerp(this.selector.renderer.material.color, sel2Color, Mathf.SmoothStep(0f, 1f, i));
				}
				
				yield return null;				
				
			}
			
		}
		this.colorSwap = false;
		
	}
	
	private IEnumerator removeBlocks(List<GameObject> blocks,float start=1f, float end=0.002f, float rate=6.54f) {
		
		float i = 0f;
		while(i < 1f) {
		
			i = i + Time.deltaTime * rate;
			float alpha = Mathf.Lerp(start,end,i);
			
			foreach(GameObject block in blocks) {
				setAlpha(block,alpha);				
			}
			
			yield return null;
			
		}
		
		foreach(GameObject block in blocks) {
			removeBlock(block);
		}
		
	}
	
	
	private IEnumerator moveCube(int startSide, int newSide, float rotateTime = 0.121f) {
		
		this.cubeRotation = true;
		this.cubeSide = newSide;
		Quaternion newRotation = Quaternion.Euler(0, this.getAngleForCubeSide(newSide), 0);
		Quaternion curRotation = Quaternion.Euler(0, this.getAngleForCubeSide(startSide), 0);
		float i = 0f;
		float rate = 1f/rotateTime;
		while(i < 1f) {
			i = i + Time.deltaTime * rate;
			transform.rotation = Quaternion.Lerp(curRotation, newRotation, Mathf.SmoothStep(0f, 1f, i));
			yield return null;
		}
		
		i = 0f;
		float toCube;
		float toBG;
		rate = 1f/0.09945f;
		while(i < 1f) {
		
			i = i + Time.deltaTime * rate;
			toCube = Mathf.Lerp(bgAlpha,cubeAlpha,i);
			toBG = Mathf.Lerp(cubeAlpha,bgAlpha,i);
			
			foreach(GameObject block in blocks) {
				
				if(isBlockInSide(block)) {
					setAlpha(block,toCube);
				} else {
					if(block.renderer.material.color.a != bgAlpha) {
						setAlpha(block,toBG);
					}
				}
				
			}
			
			yield return null;
			
		}
		
		this.initCube();
		this.cubeRotation = false;
		
	}
	
	
	public int rotateCube(string direction) {
		
		int newCubeSide = 0;
		
		if(direction == "right" && this.cubeSide < 4) {
			newCubeSide = this.cubeSide + 1;
		} else if(direction == "right" && this.cubeSide == 4) {
			newCubeSide = 1;
		} else if(direction == "left" && this.cubeSide > 1) {
			newCubeSide = this.cubeSide - 1;
		} else if(direction == "left" && this.cubeSide == 1) {
			newCubeSide = 4;
		}
		
		return newCubeSide;
		
	}

	
	void clearAllBlocks() {
		
		// check for matches
		List<GameObject> vBlocks = new List<GameObject>();
		foreach(GameObject b in this.blocks) {
			if(isBlockInSide(b)) {
				vBlocks.Add(b);
			}
			
		}
		if(clearBlocks(blocks: vBlocks)) {
			if(this.gameMode == 0) {
				blockClear.PlayDelayed(0.05f);
			} else if(this.gameMode == 1) {
				blockPop.PlayDelayed(0.05f); // change this up later
			}
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
		if(this.selector && this.selector2 && !this.colorSwap) {
			this.colorSwap = true;
			Color selColor = this.selector.renderer.material.color;
			Color sel2Color = this.selector2.renderer.material.color;
			
			//lerpedColor = Color.Lerp(Color.white, Color.black, Time.time);
			StartCoroutine(swapColors(selColor,sel2Color));
			
			
			//this.selector2.renderer.material.color = selColor;
			//this.selector.renderer.material.color = sel2Color;			
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
			
			if(this.gameMode == 0) {
				StartCoroutine(removeBlocks(matches));
			}
			
			foreach(GameObject match in matches) {
				if(this.gameMode == 1) {
					match.rigidbody.constraints = RigidbodyConstraints.None;
					match.rigidbody.AddForce(transform.TransformDirection(Vector3.back * 100000));
					Block bScript = match.GetComponent<Block>();
					bScript.nukeBlock();
				}
				this.score += 10;
			}
			
			this.modeDisplay.text = "Score: " + this.score.ToString("N0");
			return true;
			
		}
		
		return false;
		
	}
	
	
	public void removeInvader(GameObject invader) {
	
		int i = this.invaders.IndexOf(invader);
		
		if(invaders[i]) {
			Destroy(this.invaders[i]);
			this.invaders.RemoveAt(i);
		}
		
	}
	
	
	public void removeBlock(GameObject block) {
		
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
				hit.collider.gameObject.rigidbody.velocity.y >= -0.03f) {
			
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
					hit.collider.gameObject.rigidbody.velocity.y >= -0.03f) {
				
					direction = Vector3.forward;
					return hit.collider.gameObject;
					
				}
			
			}
			
		}

		return this.blnk;
		
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
			
		}
		
		return matches;
		
	}
	
	
}
