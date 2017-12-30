using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemycon1 : Base {
	public float inithpMax = 5.0f;
	public float initspeed = 6.0f;
	public bool jump1 = true;
	public Vector2 jumpPower = new Vector2 (0.0f, 1500.0f);
	public int addScore = 500;
	[System.NonSerialized] public bool cameraRendered = false;
	[System.NonSerialized] public bool attack = false;
	[System.NonSerialized] public int attackdm = 1;
	[System.NonSerialized] public Vector2 attackNock = Vector3.zero;
	public readonly static int attack1 =Animator.StringToHash("Base Layer.Toko_Attack1");
	public readonly static int punch =Animator.StringToHash("Base Layer.Toko_Punch");
	public readonly static int run = Animator.StringToHash("Base Layer.Toko_Run");
	public readonly static int idle = Animator.StringToHash("Base Layer.Toko_Idle");
	public readonly static int Jump = Animator.StringToHash("Base Layer.jump");
	public readonly static int damage  = Animator.StringToHash("Base Layer.Toko_Damage");
	public readonly static int dead  = Animator.StringToHash("Base Layer.Toko_Knockdown");
	Player playerctr;
	Animator playeranim;
	protected override void Awake () {
		base.Awake ();
		rigidbody2D = GetComponent<Rigidbody2D> ();
		playerctr = Player.GetController ();
		playeranim = playerctr.GetComponent<Animator> ();
		hp = hpMax;
		speed = initspeed;
	}
	protected override void FixedUpdateCharacter(){
		if (!cameraRendered) {
			return;
		}

		if(jump){
			if ((grounded && !groundedPrev) || (grounded && Time.fixedTime > jumpstar + 1.0f)) {
				jump = false;

			}
			if(Time.fixedTime> jumpstar +1.0f){
				if (rigidbody2D.gravityScale < grav) {
					rigidbody2D.gravityScale = grav;
				}
			}
		}else{
			rigidbody2D.gravityScale = grav;
		}
		transform.localScale = new Vector3 (bas * dir, transform.localScale.y, transform.localScale.z);

		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		if(stateInfo.nameHash == Enemycon1.damage||stateInfo.nameHash == Enemycon1.dead){
			speedVx = 0.0f;
			rigidbody2D.velocity = new Vector2 (0.0f, rigidbody2D.velocity.y);
		}
		}
	public bool ActionJump(){
		if (jump1 && grounded && !jump) {
			anim.SetTrigger ("Jump");
			rigidbody2D.AddForce (jumpPower);
			jump = true;
			jumpstar = Time.fixedTime;

		}
		return jump;
	}

	public void ActionAttack(string atkname,int damage1){
		attack = true;
		attackdm = damage1;
		anim.Play (atkname);
	}


	public void ActionDamage(){
		int damage1 = 0;
		if (hp <= 0) {
			return;
		}
		if(super){
			anim.SetTrigger ("super");
	}
		AnimatorStateInfo stateInfo = playeranim.GetCurrentAnimatorStateInfo (0);
		if(stateInfo.nameHash == Player.combo5){
			damage1 = 3;
			if(!super || super_dm){
				anim.SetTrigger ("Damege");
				jump = true;
				jumpstar = Time.fixedTime;
				AddForceAnimatorVy (1500.0f);
				Debug.Log (string.Format (">>> dmg Jump{0}", stateInfo.nameHash));
			}
		}else if(!grounded){
			damage1 = 2;
			if(!super || super_dm){
				anim.SetTrigger ("Damege");
				jump = true;
				jumpstar = Time.fixedTime;
				Debug.Log (string.Format (">>> dmg {0}", stateInfo.nameHash));

			}
		}else{
			damage1 = 1;

	}
		if(SetHP(hp -damage1,hpMax)){
			Dead(false);
		}
	}
	public override void Dead(bool gameOver){
		base.Dead (gameOver);
		Destroy (gameObject, 1.0f);

	}

}