using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FIRE{
	ANGLE,
	HOMING,
	HOMING_S

}
public class Bullet_en : MonoBehaviour {
	public float speedV = 10.0f;
	public float speedA = 0.0f;
	public FIRE fireType = FIRE.HOMING;
	public int attackPoint = 1;
	public Vector2 attackNock;
	public bool penetration = false;
	public float angele = 0.0f;

	public float lifeT = 3.0f;
	public float homingT =0.0f;
	public float homingAV = 180.0f;
	public float homingAA = 20.0f;

	public Vector3 bulletSV = Vector3.zero;
	public Vector3 bulletSA = Vector3.zero;
	protected GameObject player;
	public Sprite hite;
	public Vector3 hitEff = Vector3.one;
	public float rotateV = 360.0f;
	[System.NonSerialized] public Transform ownwer;
	[System.NonSerialized] public GameObject traget;
	[System.NonSerialized] public bool attackEn;
	protected  Rigidbody2D rigidbody2D;
	float fireTime;
	Vector3 posTarget;
	float homing1;
	Quaternion homingrote;
	float speed;
	private Life life;
	// Use this for initialization
	void Start () {
			rigidbody2D = GetComponent<Rigidbody2D> ();
			life = GameObject.FindGameObjectWithTag ("HP").GetComponent<Life> ();
			ownwer = GetComponent<Transform> ();
			if (!ownwer) {

				return;
			}
		if (Player.GetGameObject () != null) {
			traget = Player.GetGameObject ();
			posTarget = traget.transform.position + new Vector3 (0.0f, 0.0f, 0.0f);
		}
			switch (fireType) {
			case FIRE.ANGLE:
				speed = (ownwer.localScale.x < 0.0f) ? -speedV : +speedV;
				break;
			case FIRE.HOMING:
				speed = speedV;
				homingrote = Quaternion.LookRotation (posTarget - transform.position);
				break;
			case FIRE.HOMING_S:
				speed = speedV;
				break;

			}
			fireTime = Time.fixedTime;
			homing1 = angele;
			attackEn = true;
			Destroy (this.gameObject, lifeT);
		}
	void OnTriggerEnter2D(Collider2D col)
	{
		if (!ownwer) {
			return;
		}
		if ((col.isTrigger ||
		    (ownwer.tag == "player" && col.tag == "player") ||
		    (ownwer.tag == "player" && col.tag == "Bullet") ||
		    (ownwer.tag == "Enemy" && col.tag == "Enemy") ||
		    (ownwer.tag == "Enemy" && col.tag == "Enemy_B") ||
		    (ownwer.tag == "Enemy" && col.tag == "Enemey_d"))) {
			return;
		}
		if (col.tag == "player") {
			life.LifeDown (attackPoint);
		}
		if (!penetration) {
			GetComponent<SpriteRenderer> ().sprite = hite;
			GetComponent<SpriteRenderer> ().color = new Color (1.0f, 1.0f, 1.0f, 0.5f);
			transform.localScale = hitEff;
			Destroy (this.gameObject, 0.1f);
		}
	}
	void FixedUpdate(){
		bool homin = ((Time.fixedTime - fireTime) < homingT);
		if(homin){
			posTarget = traget.transform.position + new Vector3 (0.0f, 1.0f, 0.0f);
	}
		switch(fireType){
		case FIRE.ANGLE :
			rigidbody2D.velocity = Quaternion.Euler (0.0f, 0.0f, angele) * new Vector3 (speed, 0.0f, 0.0f);
			break;

		case FIRE.HOMING :
			{
				if(homin){
					homingrote = Quaternion.LookRotation (posTarget - transform.position);
				}
				Vector3 vecMove = (homingrote * Vector3.forward)*speed;
				rigidbody2D.velocity = Quaternion.Euler (0.0f, 0.0f, angele) * vecMove;
			}
			break;

		case FIRE.HOMING_S :
			if (homin) {
				float targetAAA = Mathf.Atan2 (posTarget.y - transform.position.y, posTarget.x - transform.position.x) * Mathf.Rad2Deg;
				float deltaA = Mathf.DeltaAngle (targetAAA, homing1);
				float deltaHoming = homingAV * Time.fixedDeltaTime;
				if (Mathf.Abs (deltaA) >= deltaHoming) {
					homing1 += (deltaA < 0.0f) ? +deltaHoming : -deltaHoming;

				}
					homingAV += (homingAA * Time.fixedDeltaTime);
					homingrote = Quaternion.Euler (0.0f, 0.0f, homing1);
				}
				rigidbody2D.velocity = (homingrote * Vector3.right) * speed;
				break;
			}
			speed += speedA * Time.fixedDeltaTime;
			  
			transform.localScale += bulletSV;
			bulletSV += bulletSA * Time.fixedDeltaTime;
			if(transform.localScale.x <0.0f || transform.localScale.y<0.0f || transform.localScale.z<0.0f){
				Destroy (gameObject);
			}
		}
	}