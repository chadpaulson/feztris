using UnityEngine;
using System.Collections;

public class Invader : MonoBehaviour {
	
	private bool land = false;
	private bool climb = false;
	private bool hit = false;
	private BlockManager manager;
	private AudioSource invaderLand;
	
	void Awake() {
		GameObject cubeManager = GameObject.FindGameObjectWithTag("manager");
		manager = cubeManager.GetComponent<BlockManager>();	
	}	
	
	// Use this for initialization
	void Start () {
	
		AudioSource[] audioSources = (AudioSource[])GetComponents<AudioSource>();
		this.invaderLand = audioSources[0];
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void FixedUpdate() {
		
		if(this.climb) {
			gameObject.rigidbody.AddForce(Vector3.up * 77);
		}
		
	}
	
	void nukeMe() {
		
		manager.removeInvader(gameObject);
		
	}
	
	void OnCollisionEnter(Collision col) {
		
		
		if(col.gameObject.CompareTag("ground") && !this.land) {
			this.land = true;
			this.invaderLand.Play();
			this.climb = true;
			gameObject.GetComponent<TrailRenderer>().enabled = false;
		}
		
		
		if(col.gameObject.CompareTag("block")) {
	
			if(col.relativeVelocity.magnitude > 1f && !this.hit) {
				this.hit = true;
				//Debug.Log ("Hit Invader!");
				this.climb = false;
				gameObject.rigidbody.constraints = RigidbodyConstraints.None;
				gameObject.rigidbody.AddForce(col.rigidbody.velocity * 100000);
				manager.invaderHit.PlayDelayed(0.6f);
				Invoke("nukeMe", 1.4f);
			
			}
			
		}
		
		
	}
	
}
