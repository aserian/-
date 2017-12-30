using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemycon : MonoBehaviour {
	Enemycon1 enemy;
	Animator playerAnim;
	public GameObject item;
	public int attackPoint = 1;
	[System.NonSerialized] public Animator anim;
	public int hpp = 10;
	private Life life;
	private Player player;
	private Bullet bullet;
	private const string MAIN_CAMERA_TAG_NAME = "MainCamera";
	private bool _isRendered = false;
	int ataackHash = 0;
	// Use this for initialization
	void Awake () {
		enemy = GetComponentInParent<Enemycon1> ();
		playerAnim = Player.GetAnimator ();
		life = GameObject.FindGameObjectWithTag ("HP").GetComponent<Life> ();
		anim =  GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void OnTriggerEnter2D (Collider2D other) {

		if (_isRendered) {
			if (other.tag == "Bullet") {
				enemy.ActionDamage ();
				Camera.main.GetComponent<Cameraa> ().AddCameraSize (-0.01f, -0.3f);
				if (hpp <= 0) {
					if (Random.Range (0, 4) == 0) {
						Instantiate (item, transform.position, transform.rotation);
					}
				}
			}
			if (other.tag == "player_t") {
				enemy.ActionDamage ();
				if (hpp <= 0) {
					if (Random.Range (0, 4) == 0) {
						Instantiate (item, transform.position, transform.rotation);
					}
				}

				AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo (0);
				if (ataackHash != stateInfo.nameHash) {
					ataackHash = stateInfo.nameHash;
					enemy.ActionDamage ();
				}
			}
		}
	}
	void OnCollisionEnter2D(Collision2D col){
		if(col.gameObject.tag =="player"){
			life.LifeDown (attackPoint);
		}
	}
	void OnWillRenderObject()
	{
		if (Camera.current.tag == MAIN_CAMERA_TAG_NAME) {
			_isRendered = true;

		}
	}

	public void Life(int Hp)
	{
		hpp -= Hp;
	}


	void Update(){
		if (player != null) {

			AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo (0);

			if (ataackHash != 0 && stateInfo.nameHash == Player.idle) {
				ataackHash = 0;
			}
		}
	}

	IEnumerator Wait(){
		yield return new WaitForSeconds (5);

	}
}
