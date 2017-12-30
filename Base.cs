using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour {
	public Vector2 velocityMin = new Vector2 (-100.0f, -100.0f);
	public Vector2 velocityMax= new Vector2 (100.0f, 100.0f);
	[System.NonSerialized] public float hpMax =10.0f;
	public float hp =10.0f;
	[System.NonSerialized] public float dir =1.0f;
	[System.NonSerialized] public float speed =6.0f;
	[System.NonSerialized] public float bas =1.0f;
	[System.NonSerialized] public bool active =false;
	[System.NonSerialized] public bool jump =false;
	[System.NonSerialized] public bool grounded =false;
	[System.NonSerialized] public bool groundedPrev =false;
	[System.NonSerialized] public Animator anim;
	public Rigidbody2D rigidbody2D;
	protected Transform ground_L;
	protected Transform ground_C;
	protected Transform ground_R;

	protected float speedVx = 0.0f;
	protected float sppedVxAdd = 0.0f;
	protected float grav = 10.0f;
	protected float jumpstar = 0.0f;
	protected GameObject ground_OnRo;
	protected GameObject ground_OnMo;
	protected GameObject ground_OnEn;

	protected bool addForceVx = false;
	protected float addForceTIme = 0.0f;
	protected bool addVelo = false;
	protected float addVeloVx= 0.0f;
	protected float addVeloVy = 0.0f;

	protected bool setveloVxe = false;
	protected bool setveloVye = false;
	protected float setVeloVx= 0.0f;
	protected float setVeloVy = 0.0f;

	public bool super = false;
	public bool super_dm = true;

	public GameObject[] fire;

	protected virtual void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
		ground_L = transform.Find ("Ground_L");
		ground_C = transform.Find ("Ground_C");
		ground_R = transform.Find ("Ground_R");

		dir = (transform.localScale.x > 0.0f) ? 1 : -1;
		bas = transform.localScale.x * dir;
		transform.localScale = new Vector3 (bas, transform.localScale.y, transform.localScale.z);

		active = true;
		grav = rigidbody2D.gravityScale;
	}
	protected virtual void Start()
	{

	}

	protected virtual void Update()
	{
	}
	// Update is called once per frame
	protected virtual void FixedUpdate () {
		if (transform.position.y < -30.0f) {
			Dead (false);
		}
		groundedPrev = grounded;
		grounded = false;
			
		ground_OnRo = null;
		ground_OnMo = null;
		ground_OnEn = null;
		Collider2D[][] groundCC = new Collider2D[3][];
		groundCC [0] = Physics2D.OverlapPointAll (ground_L.position);
		groundCC [1] = Physics2D.OverlapPointAll (ground_C.position);
		groundCC [2] = Physics2D.OverlapPointAll (ground_R.position);

		foreach (Collider2D[] groundCL in groundCC) {
			foreach (Collider2D groundC in groundCL) {
				if (groundC != null) {
					if (!groundC.isTrigger) {

						grounded = true;
						if (groundC.tag == "Road") {
							ground_OnRo = groundC.gameObject;
						} else if (groundC.tag == "Move") {
							ground_OnMo = groundC.gameObject;
						} else if (groundC.tag == "Enemy") {
							ground_OnEn = groundC.gameObject;
						}
					}
				}
			}
		}
		FixedUpdateCharacter ();
		if (addForceVx) {
			if (Time.fixedTime - addForceTIme > 0.5f) {
				addForceVx = false;
			}
		} else {
			rigidbody2D.velocity = new Vector2 (speedVx + sppedVxAdd , rigidbody2D.velocity.y);

		}
		if (addVelo) {
			addVelo = false;
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x + addVeloVx, rigidbody2D.velocity.y + addVeloVy);
		}
		if (setveloVxe) {
			setveloVxe = false;
			rigidbody2D.velocity = new Vector2 (setVeloVx, rigidbody2D.velocity.y);
		}
		if (setveloVye) {
			setveloVye = false;
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, setVeloVy);

		}
		float vx = Mathf.Clamp(rigidbody2D.velocity.x,velocityMin.x,velocityMax.x);
		float vy = Mathf.Clamp (rigidbody2D.velocity.y, velocityMin.y, velocityMax.y);
		rigidbody2D.velocity = new Vector2 (vx, vy);
	}
	public virtual void AddForceAnimatorVx(float vx){
		if(vx !=0.0f){
			rigidbody2D.AddForce (new Vector2 (vx * dir, 0.0f));
			addForceVx = true;
			addForceTIme = Time.fixedTime;

	    }
	}
	public virtual void AddForceAnimatorVy(float vy){
		if(vy != 0.0f){
		rigidbody2D.AddForce (new Vector2 (0.0f, vy));
		jump = true;
		jumpstar = Time.fixedTime;

	}
}
public virtual void AddVelocityVx(float vx){
		addVelo = true;
		addVeloVx = vx * dir;
   }
	public virtual void AddVelocityVy(float vy){
		addVelo = true;
		addVeloVy = vy;
}
	public virtual void SetVelocityVx(float vx){
		setveloVxe = true;
		setVeloVx = vx*dir;
	}
	public virtual void SetVelocityVy(float vy){
		setveloVye = true;
		setVeloVy = vy;
	}
	public virtual void SetLightGravity(){
		rigidbody2D.velocity = new Vector2 (0.0f, 0.0f);
		rigidbody2D.gravityScale = 0.1f;
	}
	public void EnableSupperArmor(){

		super = true;
	}
	public void DisableSuperArmor(){

		super = false;
}
protected virtual void FixedUpdateCharacter(){
	}

	public virtual void  Action(float n){
		if(n != 0.0f){
			dir = Mathf.Sign (n);
			speedVx = speed * n;
			anim.SetTrigger ("Run");
		}else{
			speedVx = 0;
			anim.SetTrigger ("Idle");
		}
	}
	public void ActionF(){
		Transform gof = transform.Find ("Muzzle");
		foreach (GameObject fireO in fire) {
			GameObject goes = Instantiate (fireO, gof.position, Quaternion.identity) as GameObject;
			goes.GetComponent<Bullet_en> ().ownwer = transform;
		}

	}
	public bool ActionLook(GameObject go, float near){
		if(Vector3.Distance(transform.position,go.transform.position)>near){
			dir = (transform.position.x < go.transform.position.x) ? +1 : -1;
			return true;
 
	}
		return false;
}
	public bool ActionMoveToNear(GameObject go, float near){
		if(Vector3.Distance(transform.position,go.transform.position)>near){
			Action((transform.position.x < go.transform.position.x) ? +1.0f : -1.0f);
			return true;

		}
		return false;
	}

	public bool ActionMoveToFar(GameObject go, float far){
		if(Vector3.Distance(transform.position,go.transform.position)<far){
			Action((transform.position.x > go.transform.position.x) ? +1.0f : -1.0f);
			return true;

		}
		return false;
	}

	public virtual void Dead(bool gameOver){
		if(!active){
			return;
		}
		active = false;
		anim.SetTrigger ("Dead");
	}
	public virtual bool SetHP(float _hp, float _hpMax)
	{
		hp = _hp;
		hpMax = _hpMax;
		return(hp <= 0);
	}

}



