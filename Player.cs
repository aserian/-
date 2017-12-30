using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour {
	public float speed = 4f;
	public float jumpPower = 700;
	public LayerMask groundLayer;
	public GameObject mainCamera;
	public GameObject bullet;
	public  const int MAX_JUMP_COUNT = 2;
	public Life life;
	private int jumping = 0;
	private bool gameClear =false;
	public Text clearText;
	private Renderer renderer;
	private AudioSource sounds;
	private int timeCount;
	private Rigidbody2D rigidbody2D;
	private Animator anim;
	 bool isGrounded = false;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		rigidbody2D = GetComponent<Rigidbody2D> ();
		renderer = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!gameClear) {
			if (Input.GetKeyDown ("space") && jumping<MAX_JUMP_COUNT) {
				isGrounded = true;
			}

			float velY = rigidbody2D.velocity.y;
			bool isJumping = velY > 0.1f ? true : false;

			bool isFalling = velY < -0.1f ? true : false;
			anim.SetBool ("isJumping", isJumping);
			anim.SetBool ("isFalling", isFalling);
			timeCount += 1;
			if (Input.GetKey ("left ctrl")) {
				sounds = GetComponent<AudioSource> ();
				sounds.PlayOneShot (sounds.clip);
				anim.SetTrigger ("Shot");
					Instantiate (bullet, transform.position + new Vector3 (3.0f, 0.5f, 0.0f), transform.rotation);

				}
				if (gameObject.transform.position.y < Camera.main.transform.position.y - 8) {

					life.GameOver ();
				}
		}

	}
	void FixedUpdate()
	{
		if (isGrounded) {
			anim.SetTrigger ("Jump");
			rigidbody2D.velocity = Vector2.zero;
			rigidbody2D.AddForce (Vector2.up * jumpPower);
			jumping++;
			isGrounded = false;

		}
		if (!gameClear) {
			float x = Input.GetAxisRaw ("Horizontal");
			if (x != 0) {
				rigidbody2D.velocity = new Vector2 (x * speed, rigidbody2D.velocity.y);

				Vector2 temp = transform.localScale;
				temp.x = x;
				transform.localScale = temp;
				anim.SetBool ("Dash", true);

				Vector2 min = Camera.main.ViewportToWorldPoint (new Vector2 (0, 0));
				Vector2 max = Camera.main.ViewportToWorldPoint (new Vector2 (1, 1));
				Vector2 pos = transform.position;
				pos.x = Mathf.Clamp (pos.x, min.x, max.x);
				pos.y = Mathf.Clamp (pos.y, min.y, max.y);
				transform.position = pos;
			} else {
				rigidbody2D.velocity = new Vector2 (0, rigidbody2D.velocity.y);
				anim.SetBool ("Dash", false);
			}
		} else {
			clearText.enabled = true;
			anim.SetBool ("Dash", true);
			rigidbody2D.velocity = new Vector2 (speed, rigidbody2D.velocity.y);
			Invoke ("Coll", 5);
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag =="ClearZone"){

			gameClear =true;
		}
}
	void Coll()
	{
		Application.LoadLevel ("title");

	}
public void OnCollisionEnter2D(Collision2D col){
	if (!gameClear) {
		if (col.gameObject.CompareTag ("Ground")) {
			jumping = 0;
		}

		if (col.gameObject.CompareTag ("Enemy")) {
			StartCoroutine ("Damage");
		}
	}
}
IEnumerator Damage()
{

	gameObject.layer = LayerMask.NameToLayer("PlayerDamage");

	int count = 10;

	while(count>0){
		renderer.material.color =new Color(1,1,1,0);
		yield return new WaitForSeconds(0.05f);
		renderer.material.color =new Color(1,1,1,1);
		count--;

	}
	gameObject.layer = LayerMask.NameToLayer("Player");

}
}