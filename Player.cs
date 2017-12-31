using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour {
	public float speed = 4f;  //速さ
	public int power =1;　//攻撃力
	public float jumpPower = 10; //ジャンプ力
　　　　 public float dir =1.0f; //向き
	public GameObject bullet;  //弾オブジェクト
	public GameObject player;　　//プレイヤーオブジェクト
	public   int MAX_JUMP_COUNT = 2; //最大ジャンプ回数
	public Life life; //体力スクリプト呼び出し
	protected bool gameClear =false; //ゲームクリア判定
	public Text clearText; //クリアテキスト呼び出し
	public float climingSpeedDown = 3f;　//梯子を下る速度
	public float climingSpeedUp = 3f;　　//梯子を上る速度
	public float climingSpeedLeft = 2f;　　　
	public float climingSpeedRight = 2f;
	private Collider2D hitLadder;  //梯子判定
	Camera camera; //カメラ呼び出し
	public LayerMask ladderLayer; //梯子レイヤー
	bool down1 = false; //しゃがむモーション判定
	public readonly static int down = Animator.StringToHash("Base Layer.down");
	public readonly static int combo3 =Animator.StringToHash("Base Layer.conme");
   　　 public readonly static int combo4 =Animator.StringToHash("Base Layer.combo2");
	public readonly static int combo5 = Animator.StringToHash("Base Layer.combo3");
	public readonly static int run = Animator.StringToHash("Base Layer.wal");
	public readonly static int run2 = Animator.StringToHash("Base Layer.motion");
	public readonly static int idle = Animator.StringToHash("Base Layer.adguagua");
	public readonly static int Jump = Animator.StringToHash("Base Layer.Jump");
	public readonly static int Jump1 = Animator.StringToHash("Base Layer.Jump_up");
	public readonly static int Jumpd = Animator.StringToHash("Base Layer.Jump_down");
	public readonly static int Jumpd2 = Animator.StringToHash ("Base Layer.Jump douwn 0");
	public readonly static int damage2  = Animator.StringToHash("Base Layer.dm");
	public readonly static int Avoid = Animator.StringToHash ("Base Layer.Avoid");
	public readonly static int Shot = Animator.StringToHash ("Base Layer.shot_loop");
	public readonly static int Jump_a = Animator.StringToHash ("Base Layer.Jump_A");
	public readonly static int DashShot = Animator.StringToHash ("Base Layer.SHOT");
	public readonly static int Jump_Shot = Animator.StringToHash("Base Layer.Jump_SHOT");
	protected Renderer renderer; //レンダー呼び出し
	protected AudioSource sounds;//サウンド呼び出し
	protected Transform ground_L; //地面判定左位置
	protected Transform ground_C;　//地面判定真ん中位置
	protected Transform ground_R; //地面判定右位置
	private bool groundd = false; //地面が水かどうか判定
	private float gravt; //重力
	Animator anim; //アニメーション呼び出し
	public bool grounded = false; //地面判定
	private Rigidbody2D rigidbody2D; //物理シュミレーション呼び出し
	private Ladder ladder; //梯子スクリプト呼び出し
	private BoxCollider2D boxCollider; //四角当たり判定呼び出し
	bool flor1 = false; //抜ける床かどうか判定
	public bool onLadder = false;	//梯子の上かどうか判定
	[System.NonSerialized] public Vector3 enemyActive;
	[System.NonSerialized] public Vector3 enemyActive2;
	[System.NonSerialized] public float groundY = 0.0f;
	public float x;
	bool gameOver = false;
	public float v;
	volatile bool atk =false;
	volatile bool atk2 = false;
	float velY;
	const string cma = "Base Layer.cma";
	// Use this for initialization
	  void Start () {
		boxCollider = GetComponent<BoxCollider2D>();
		anim = GetComponent<Animator> ();
		camera = GetComponent<Camera> ();
		rigidbody2D = GetComponent<Rigidbody2D> ();
		renderer = GetComponent<Renderer> ();
		BoxCollider2D boxcol2D = transform.Find ("Collider_EnemyActive").GetComponent<BoxCollider2D> ();
		enemyActive = new Vector3 (boxcol2D.offset.x - boxcol2D.size.x / 2.0f, boxcol2D.offset.y - boxcol2D.size.y/2.0f);
		enemyActive2 = new Vector3 (boxcol2D.offset.x - boxcol2D.size.x / 2.0f, boxcol2D.offset.y - boxcol2D.size.y/2.0f);
		boxcol2D.transform.gameObject.SetActive (false);
		ground_L = transform.Find ("Ground_L");
		ground_C = transform.Find ("Ground_C");
		ground_R = transform.Find ("Ground_R");
		gravt = rigidbody2D.gravityScale;
	}
	public void EnebleAttackInput()
	{
		atk = true;
	}

	public void SetNextAttack(string name){
		if (atk2 == true) {
			atk2 = false;
			anim.Play (name);
		}
	}
	void Grounded()
	{
		grounded = false;
		Collider2D[][] groundCC = new Collider2D[3][];
		groundCC [0] = Physics2D.OverlapPointAll (ground_L.position);
		groundCC [1] = Physics2D.OverlapPointAll (ground_C.position);
		groundCC [2] = Physics2D.OverlapPointAll (ground_R.position);

		foreach (Collider2D[] groundCL in groundCC) {
			foreach (Collider2D groundC in groundCL) {
				if (groundC != null) {
					if (!groundC.isTrigger) {
						grounded = true;
					}
				}
			}
		}

	}
	void Dash()
	{
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		x = Input.GetAxisRaw ("Horizontal");

		if (stateInfo.fullPathHash == combo3 || stateInfo.fullPathHash == combo4 || stateInfo.fullPathHash == combo5 || stateInfo.fullPathHash == damage2 ||
			stateInfo.fullPathHash == Avoid || stateInfo.fullPathHash == down || stateInfo.fullPathHash == Jump_a) {
			x = 0;
		} else if ( x!=0 && !Input.GetKey ("left shift") && stateInfo.fullPathHash != Shot && !Input.GetKey ("a") 
			&& stateInfo.fullPathHash != DashShot && stateInfo.fullPathHash != damage2) {
			rigidbody2D.velocity = new Vector2 (x * (speed * 2), rigidbody2D.velocity.y);
			Vector2 temp = transform.localScale;
			temp.x = x;
			transform.localScale = temp;
			anim.SetBool ("Dash", false);
			if (!groundd) {
				anim.SetBool ("Dashspeed", true);
			}
		} else if (Input.GetKey ("a") && x != 0) {
			rigidbody2D.velocity = new Vector2 (x * (speed * 2), rigidbody2D.velocity.y);
			Vector2 temp = transform.localScale;
			temp.x = x;
			transform.localScale = temp;
			anim.SetBool ("SHOT",true);
		} else{
			rigidbody2D.velocity = new Vector2 (0, rigidbody2D.velocity.y);
			anim.SetBool ("Dashspeed", false);
			anim.SetBool ("SHOT", false);
		}
		if (x!=0 && Input.GetKey ("left shift") && stateInfo.fullPathHash != Shot) {
			rigidbody2D.velocity = new Vector2 (x * speed, rigidbody2D.velocity.y);

			Vector2 temp = transform.localScale;
			temp.x = x;
			transform.localScale = temp;
			if (!groundd) {
				anim.SetBool ("Dash", true);
			}
		} else {
			anim.SetBool ("Dash", false);
		}
	}
	void LadderDecision()
	{
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		if (onLadder) {
			rigidbody2D.gravityScale = 0;
		} else if (stateInfo.fullPathHash == Jump_a) {
			rigidbody2D.gravityScale = gravt + 2;
		} else {
			rigidbody2D.gravityScale = gravt;
		}
		v = Input.GetAxisRaw("Vertical");
		if (onLadder && ladder) {

			bool stillOnLadder = true;
			Vector2 pos = transform.position;
			Vector2 pointA = new Vector2(pos.x + boxCollider.offset.x - (boxCollider.size.x / 2), pos.y + boxCollider.offset.y - (boxCollider.size.y / 2));
			Vector2 pointB = new Vector2(pos.x + boxCollider.offset.x + (boxCollider.size.x / 2), pos.y + boxCollider.offset.y + (boxCollider.size.y / 2));

			if (v < 0 && ladder.allowDown) {

				if (!ladder.fallOffBottom) {
					stillOnLadder = Physics2D.OverlapArea(pointA, new Vector2(pointB.x, pointB.y - 0.1f), ladderLayer);
				}


				if (!ladder.fallOffBottom && !stillOnLadder) {
					rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,0);

				} else if(v<0&& Input.GetKey("space")){ 

					rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x, 0);
				}else{

					rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,(v * (climingSpeedDown + (ladder.defaultSpeedY <= 0 ? ladder.defaultSpeedY : -ladder.defaultSpeedY))));
				}
			} else if (v > 0 && ladder.allowUp) {

				if (!ladder.fallOffTop) {
					stillOnLadder = Physics2D.OverlapArea(new Vector2(pointA.x, pointA.y + 0.1f), pointB, ladderLayer);
				}

				if (!ladder.fallOffTop && !stillOnLadder) {
					rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);

				} else {

					rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,(v * (climingSpeedUp + ladder.defaultSpeedY)));
				}

			} else {

				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,(ladder.defaultSpeedY));
			}

			if (grounded && (x < 0 || x > 0)) {
				Unstick();

			} else if (ladder.snapToMiddle) {

				if ((v == 0) && ((ladder.fallOffLeft && x < 0) || (ladder.fallOffRight && x > 0))) {

					Unstick();

				} else {

					if (transform.position.x > (hitLadder.transform.position.x - 0.05f) 
						&& transform.position.x < (hitLadder.transform.position.x + 0.05f)) {
						rigidbody2D.velocity = new Vector2(0,rigidbody2D.velocity.y);

					} 
				}
			} else {
				if (x < 0 && ladder.allowLeft) {
					if (!ladder.fallOffLeft) {
						stillOnLadder = Physics2D.OverlapArea(pointA, new Vector2(pointB.x - 0.1f, pointB.y), ladderLayer);
					}
					if (!ladder.fallOffLeft && !stillOnLadder) {
						rigidbody2D.velocity = new Vector2(0,rigidbody2D.velocity.y);
					} else {
						rigidbody2D.velocity = new Vector2((x * (climingSpeedLeft + (ladder.defaultSpeedX <= 0 ? ladder.defaultSpeedX : -ladder.defaultSpeedX))),rigidbody2D.velocity.y);
					}
				} else if (x > 0 && ladder.allowRight) {
					if (!ladder.fallOffRight) {
						stillOnLadder = Physics2D.OverlapArea(new Vector2(pointA.x + 0.1f, pointA.y), pointB, ladderLayer);
					}


					if (!ladder.fallOffRight && !stillOnLadder) {

						rigidbody2D.velocity = new Vector2(0,rigidbody2D.velocity.y);

					} else {

						rigidbody2D.velocity = new Vector2((x * (climingSpeedRight + ladder.defaultSpeedX)),rigidbody2D.velocity.y);
					}

				} else {

					rigidbody2D.velocity = new Vector2((ladder.defaultSpeedX),rigidbody2D.velocity.y);
				}
			}
		}

	}
	  void FixedUpdate()
	{
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		if (!gameClear && !gameOver) {

			Grounded ();
			LadderDecision ();
			Dash ();
			} else {
				clearText.enabled = true;
				anim.SetBool ("Dash", true);
				rigidbody2D.velocity = new Vector2 (speed, rigidbody2D.velocity.y);
				Invoke ("Coll", 5);
			}
	}
	void Attack(){
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		if (Input.GetKeyDown("s")) {
			if (stateInfo.fullPathHash ==Jump || stateInfo.fullPathHash ==Jumpd || 
				stateInfo.fullPathHash == Jumpd2 || stateInfo.fullPathHash == Jump1|| 
				stateInfo.fullPathHash == Jump|| stateInfo.fullPathHash ==Jump_Shot ) {
				anim.Play ("Jump_A");
			}
		}

		if (Input.GetKeyDown ("s") && stateInfo.fullPathHash !=Jump &&stateInfo.fullPathHash != Jump1 && stateInfo.fullPathHash != Jumpd
			&& stateInfo.fullPathHash !=Jumpd2 && grounded) {
			if (transform.localScale.x >= 0) {
				rigidbody2D.velocity = new Vector2 (3, rigidbody2D.velocity.y);
			} else {
				rigidbody2D.velocity = new Vector2 (-3, rigidbody2D.velocity.y);
			}
			if (stateInfo.fullPathHash == idle || stateInfo.fullPathHash == run || stateInfo.fullPathHash == run2 
				||stateInfo.fullPathHash == down || stateInfo.fullPathHash == DashShot || stateInfo.fullPathHash ==Shot) {
				anim.Play ("conme");
			} else {
				if (atk) {
					atk = false;
					atk2 = true;
				}
			}
	     }
	}
	void Avoidance()
	{
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		if (Input.GetKeyDown ("w")&&grounded&&stateInfo.fullPathHash != Avoid) {
			anim.Play ("Avoid");
			if (transform.localScale.x > 0) {
				Instantiate (bullet, transform.position + new Vector3 (1.5f, -0.5f, .0f), Quaternion.Euler (0, 0, -45));
				Invoke ("sp", 0.1f);
				Invoke ("sp2", 0.3f);
				Invoke ("sp3", 0.5f);
				Invoke ("sp4", 0.7f);
				Invoke ("sp5", 0.9f);
				rigidbody2D.velocity = new Vector2 (7, rigidbody2D.velocity.y);
			} else {
				Instantiate (bullet, transform.position + new Vector3 (-1.5f, -0.5f, .0f), Quaternion.Euler (0, 0, 225));
				Invoke ("sp", 0.1f);
				Invoke ("sp2", 0.3f);
				Invoke ("sp3", 0.5f);
				Invoke ("sp4", 0.7f);
				Invoke ("sp5", 0.9f);
				rigidbody2D.velocity = new Vector2 (-7, rigidbody2D.velocity.y);
			}

		}

	}
	void Avoidance1()
	{
		if(Input.GetKeyDown("e")&&x!=0&&grounded){
			if (transform.localScale.x > 0)
				transform.position += new Vector3 (2.0f, 0f, 0f);
			else
				transform.position += new Vector3 (-2.0f, 0f, 0f);
		}
	}
	void Jumping()
	{
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		if (Input.GetKeyDown ("d") && groundd) {
			anim.Play ("Jump");
			rigidbody2D.velocity = Vector2.up * (jumpPower - 2);
		}
		if (Input.GetKeyDown ("d") && 0 < MAX_JUMP_COUNT&& stateInfo.fullPathHash != Avoid && !flor1) {
			if (!grounded) {
				MAX_JUMP_COUNT = 1;
			}
			if (x != 0 && !Input.GetKey ("a") && stateInfo.fullPathHash != Jump_Shot) {
				anim.Play ("Jump_up");
			} else if (x == 0 && !Input.GetKey ("a") && stateInfo.fullPathHash != Jump_Shot) {
				anim.Play ("Jump");
			} else if (Input.GetKey ("a")&&Input.GetKeyDown("d") ) {
				anim.Play ("Jump_SHOT");
			}
			rigidbody2D.velocity = Vector2.up * jumpPower;
			MAX_JUMP_COUNT--;
		}

		if (((Input.GetKeyUp ("d") && stateInfo.fullPathHash == Jump) 
			|| (Input.GetKeyUp ("d") && stateInfo.fullPathHash == Jump1)||(Input.GetKeyUp ("d") && stateInfo.fullPathHash == Jump_Shot&&Input.GetKeyUp ("a"))) && MAX_JUMP_COUNT == 1) {
			rigidbody2D.velocity = Vector3.zero;
		}

		 velY = rigidbody2D.velocity.y;
		bool isJumping = velY > 0.1f&& !Input.GetKey("a")? true : false;
		anim.SetBool ("isJumping",isJumping );
		bool isFalling = velY < -0.1f && !Input.GetKey("a") ? true : false;
		anim.SetBool ("isFalling", isFalling);
		if (velY <- 0.1f && !grounded && !Input.GetKeyDown("s")&&stateInfo.fullPathHash !=Jump_a && 
			!Input.GetKey("a")&&stateInfo.fullPathHash != damage2) {
			anim.SetTrigger ("Jump_down");
		}

	}
	void Shotting()
	{
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		if (Input.GetKey ("a")&& grounded && stateInfo.fullPathHash != DashShot  && x ==0 &&stateInfo.fullPathHash !=combo3 &&stateInfo.fullPathHash !=combo4&&
			stateInfo.fullPathHash !=combo5){ 
			anim.SetBool ("Shot",true);
		} else {
			anim.SetBool ("Shot",false);
		}
		if (velY <- 0.1f && !grounded && !Input.GetKeyDown("s")&&stateInfo.fullPathHash !=Jump_a && 
			Input.GetKey("a")&&stateInfo.fullPathHash != damage2) {
			anim.Play ("Jump_SHOT_D");
		}
	}


	void JumpShot()
	{
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		float veloY = rigidbody2D.velocity.y;
		bool Jump_shot_up = veloY >0.1f && Input.GetKey("a")? true : false;
		anim.SetBool ("jump_SHOT", Jump_shot_up);
		bool Jump_shot = veloY <-0.1f && Input.GetKey("a")&& !grounded? true : false;
		anim.SetBool ("jump_SHOT_D", Jump_shot);

	

	}

	void Down()
	{
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		if ((v < -0.1 && stateInfo.fullPathHash ==idle&& !onLadder&& !ladder) || (v<-0.1 && stateInfo.fullPathHash == run) 
			||(v<-0.1 && stateInfo.fullPathHash == run2)|| (v<-0.1 &&  grounded &&stateInfo.fullPathHash != Jump_a &&
				stateInfo.fullPathHash != combo3&&stateInfo.fullPathHash !=combo4 &&
				stateInfo.fullPathHash != combo5 && stateInfo.fullPathHash != Jump1 && 
				stateInfo.fullPathHash != Jump && stateInfo.fullPathHash != Avoid && 
				stateInfo.fullPathHash != Jump_Shot &&stateInfo.fullPathHash != damage2)){
			
			down1 = true;
			anim.SetBool ("down", down1);
		} else if(v ==0 || Input.GetKeyDown("d") || onLadder || ladder|| stateInfo.fullPathHash == Jump 
			|| stateInfo.fullPathHash == Jumpd || stateInfo.fullPathHash ==Jump1 || 
			stateInfo.fullPathHash ==Jumpd2 || 
			!grounded || stateInfo.fullPathHash == Avoid || 
			stateInfo.fullPathHash == combo3 || stateInfo.fullPathHash == combo4|| stateInfo.fullPathHash == combo5 
			|| stateInfo.fullPathHash == Jump_Shot || stateInfo.fullPathHash == Shot  ||stateInfo.fullPathHash ==damage2 && grounded){
			down1 = false;
			anim.SetBool ("down", down1);
		}
	}
	void LadderAllow()
	{
		Clamp ();
		Vector2 pos = transform.position;
		Vector2 pointA = new Vector2 (pos.x + boxCollider.offset.x - (boxCollider.size.x / 2), pos.y + boxCollider.offset.y - (boxCollider.size.y / 2));
		Vector2 pointB = new Vector2 (pos.x + boxCollider.offset.x + (boxCollider.size.x / 2), pos.y + boxCollider.offset.y + (boxCollider.size.y / 2));
		hitLadder = Physics2D.OverlapArea (pointA, pointB, ladderLayer);

		if (!onLadder && hitLadder) {
			float m = Input.GetAxis ("Vertical");
			if (!onLadder && (m > 0.1 || m < -0.1)) {
				Stick ();
			}
		} 
		if (!hitLadder && onLadder) {

			Unstick ();
		}
		if (onLadder && ladder && ladder.allowJump && Input.GetKeyDown ("d")) {

			Unstick ();
			grounded = true;
		}
		if (onLadder && ladder && ladder.allowJump && Input.GetKey ("d") && v < 0) {
			Unk ();
			grounded = true;
		}



	}
	// Update is called once per frame
	void Update () {
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		if (!gameClear && !gameOver) {
			Jumping ();
			Avoidance ();
			Avoidance1 ();
			Attack();
			JumpShot ();
			Shotting ();
			Down ();
			LadderAllow ();
			if (gameObject.transform.position.y < Camera.main.transform.position.y - 8) {
				life.GameOver ();
			}
		}
	}
	void sp()
	{
		if (transform.localScale.x > 0) 
			Instantiate (bullet, transform.position + new Vector3 (1.2f, -0.3f, 0.0f), Quaternion.Euler (0, 0, -22));
		else if(transform.localScale.x<0)
			Instantiate (bullet, transform.position + new Vector3 (-1.2f, -0.3f, 0.0f), Quaternion.Euler (0, 0, 203));
	}
	void sp2()
	{
		if (transform.localScale.x > 0) 
		Instantiate (bullet, transform.position + new Vector3 (0.9f, 0.0f, 0.0f),Quaternion.Euler(0,0,0));
		else if(transform.localScale.x<0)
			Instantiate (bullet, transform.position + new Vector3 (-0.9f, 0.0f, 0.0f),Quaternion.Euler(0,0,180));
	}
	void sp3()
	{
		if (transform.localScale.x > 0) 
		Instantiate (bullet, transform.position + new Vector3 (0.6f, 1.4f, 0.0f),Quaternion.Euler(0,0,22));
		else if(transform.localScale.x<0)
			Instantiate (bullet, transform.position + new Vector3 (-0.6f, 1.4f, 0.0f),Quaternion.Euler(0,0,158));
	}
	void sp4()
	{
		if (transform.localScale.x > 0) {
			Instantiate (bullet, transform.position + new Vector3 (0.3f, 1.7f, 0.0f), Quaternion.Euler (0, 0, 44));
		} else if (transform.localScale.x < 0) {
			Instantiate (bullet, transform.position + new Vector3 (-0.3f, 1.7f, 0.0f), Quaternion.Euler (0, 0, 136));
		}
	}
	void sp5()
	{
		if (transform.localScale.x > 0) {
			Instantiate (bullet, transform.position + new Vector3 (0.0f, 2.0f, 0.0f), Quaternion.Euler (0, 0, 88));
		} else if (transform.localScale.x < 0) {
			Instantiate (bullet, transform.position + new Vector3 (0.0f, 2.0f, 0.0f), Quaternion.Euler (0, 0,90));
		}
	}
	void wait()
	{
		if (transform.localScale.x >0) {
			Instantiate (bullet, transform.position + new Vector3 (2.0f, 0.5f, .0f), Quaternion.Euler (0, 0, 0));
		}
		else if(transform.localScale.x <0){
			Instantiate (bullet, transform.position + new Vector3 (-2.0f, 0.5f, 0.0f),Quaternion.Euler(0,0,180));
	}
	}

	void  Clamp()
	{
		Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0,0));
		Vector2 max = Camera.main.ViewportToWorldPoint (new Vector2 (1, 1));
		Vector2 pos = transform.position;
		pos.x = Mathf.Clamp (pos.x, min.x, max.x);
		pos.y = Mathf.Clamp (pos.y, min.y, max.y);
		transform.position = pos;

	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "ClearZone") {

			gameClear = true;
		}
	
		if (col.tag == "Bullet1") {
			if (col.gameObject.transform.localPosition.x < transform.localPosition.x) {
				rigidbody2D.AddForce(new Vector2(1000f,1000f));
			} else if (col.gameObject.transform.localPosition.x > transform.localPosition.x){
				rigidbody2D.AddForce(new Vector2(-1000f,1000f));
			}
			StartCoroutine ("Damage");
		}
}
	void Coll()
	{
		Application.LoadLevel ("title");

	}
public void OnCollisionEnter2D(Collision2D col){
	if (!gameClear) { 
			if ((col.gameObject.CompareTag ("Ground") &&grounded||col.gameObject.CompareTag("Flor")
				&&grounded||col.gameObject.CompareTag("flor1")&&grounded)&&MAX_JUMP_COUNT ==0){
				MAX_JUMP_COUNT = 2;
			}
			if (col.gameObject.CompareTag ("Enemy") ) {
				if (col.gameObject.transform.localPosition.x <transform.localPosition.x) {
					rigidbody2D.AddForce(new Vector2(1000f,1000f));
				} else if (col.gameObject.transform.localPosition.x > transform.localPosition.x){
					rigidbody2D.AddForce(new Vector2(-1000f,1000f));
				}
					StartCoroutine ("Damage");
			}
			if (col.gameObject.tag == "thorns") {
				life.LifeDown(10);
			}

	}
}
	public void OnCollisionExit2D(Collision2D other){
		if( other.gameObject.tag == "Flor"&& transform.parent !=null)
		{
			transform.SetParent(null);
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "water") {
			groundd = false;
		}
		if (other.tag == "flor1"||other.tag=="Flor") {
			flor1 = false;
	   }
	}
	void OnCollisionStay2D(Collision2D coll2){
		if (coll2.gameObject.tag == "Flor"&&transform.parent ==null) {
			transform.SetParent (coll2.transform);
		}

		if (coll2.gameObject.tag == "Road" || coll2.gameObject.tag == "MoveObject" || coll2.gameObject.tag == "Enemy") {
			groundY = transform.position.y;
		}
	}
	public void OnTriggerStay2D(Collider2D other){
		if (other.tag == "water") {
			groundd = true;
	}
		if (other.tag == "flor1"||other.tag=="Flor") {
			if (v < 0) {
				flor1 = true;
			}
		}
}
	public void gravity(){
		rigidbody2D.gravityScale = 0;
	}

IEnumerator Damage()
{
		
	gameObject.layer = LayerMask.NameToLayer("PlayerDamage");
	int count = 25;
		anim.Play ("dm");
	while(count>0){
			renderer.material.color = new Color (1, 1, 1, 0);
		yield return new WaitForSeconds(0.05f);
			renderer.material.color = new Color (1, 1, 1, 1);
			yield return new WaitForSeconds(0.05f);
		count--;

	}
	gameObject.layer = LayerMask.NameToLayer("Player");

}
	public static GameObject GetGameObject()
	{
		return GameObject.Find("player");
	}
	public static Transform GetTransform()
	{
		return GameObject.Find("player").transform;
	}

	public static Player GetController(){

		return GameObject.FindGameObjectWithTag ("player").
			GetComponent<Player> ();
	}
	public static Animator GetAnimator(){
		return GameObject.FindGameObjectWithTag("player").
			GetComponent<Animator> ();
	}
void Unstick() {
	onLadder = false;
	ladder = null;
	
}
	void Unk(){
		onLadder = false;
		ladder = null;
	}
	void Stick() {
		ladder = hitLadder.GetComponent<Ladder>();
			OnLadder ();
			if (ladder.allowJump) {
				if (ladder.allowDoubleJump) {
					MAX_JUMP_COUNT = 2;
				} else {
					MAX_JUMP_COUNT = 1;
				}
			} else {
				MAX_JUMP_COUNT = 0;

			}
	}

	void OnLadder(){
		if (!Input.GetKey ("d") && (v > 0)) {
			onLadder = true;
		} else if (!Input.GetKey ("d") && v < 0) {
			onLadder = true;
		}
	}
}
