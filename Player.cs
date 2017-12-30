using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public float speed = 4f;
	public float jumpPower = 700;
	public LayerMask groundLayer;
	public GameObject mainCamera;
	public GameObject bullet;
	public GameObject Damage;

	private Rigidbody2D rigidbody2D;
	private Animator anim;
	 bool isGrounded = false;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		rigidbody2D = GetComponent<Rigidbody2D> ();
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("space") && !isGrounded) {
				anim.SetTrigger ("Jump");
			isGrounded = true;
			rigidbody2D.AddForce (Vector2.up * jumpPower);
		}
		float velY = rigidbody2D.velocity.y;
		bool isJumping = velY > 0.1f ? true : false;

		bool isFalling = velY < -0.1f ? true : false;
		anim.SetBool ("isJumping", isJumping);
		anim.SetBool ("isFalling", isFalling);

		if (Input.GetKeyDown ("left ctrl" )) {

			anim.SetTrigger ("Shot");
			Instantiate (bullet, transform.position + new Vector3 (3.0f, 0.5f,0.0f), transform.rotation );

		}

	}
	void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.CompareTag("Ground"))
		{
			isGrounded = false;
		}

	}
	void FixedUpdate()
	{
		float x = Input.GetAxisRaw ("Horizontal");
		if (x != 0) {
			rigidbody2D.velocity = new Vector2 (x * speed, rigidbody2D.velocity.y);

			Vector2 temp = transform.localScale;
			temp.x = x;
			transform.localScale = temp;
			anim.SetBool ("Dash", true);
		}
		else {
			rigidbody2D.velocity = new Vector2 (0, rigidbody2D.velocity.y);
			anim.SetBool ("Dash", false);
		}

		Vector3 cameraPos = mainCamera.transform.position;
		mainCamera.transform.position = cameraPos;
		Vector2 min = Camera.main.ViewportToWorldPoint (new Vector2 (0, 0));
		Vector2 max = Camera.main.ViewportToWorldPoint (new Vector2 (1, 1));
		Vector2 pos = transform.position;
		pos.x = Mathf.Clamp (pos.x, min.x, max.x);
		pos.y = Mathf.Clamp (pos.y, min.y, max.y);
		transform.position = pos;
	}
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Ene") {
			Destroy (gameObject);
			Instantiate (Damage, transform.position, transform.rotation);
		}
	}
}