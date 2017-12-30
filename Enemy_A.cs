using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Enemy_A : Enemy_bos {
	public int aiIfRun = 50;
	public int aiIfJump = 10;
	public int aiIfESCAPE = 10;
	public int damageA = 1;
	public int aiIfRturn = 10;
	int firecount =0;
	public int firea = 4;
	private Rigidbody2D rigidbody2D;
	public override void SetCombatAIState(ENEMYAISTS sts){
		base.SetCombatAIState (sts);
		switch (aiState) {
		case ENEMYAISTS.ACTIONSELECT:
			break;
		case ENEMYAISTS.WAIT:
			aiActionL =  Random.Range (0.0f, 1.0f);
			break;
		case ENEMYAISTS.RUNTOPLAYER:
			aiActionL = 3.0f;
			break;
		case ENEMYAISTS.JUMPTOPLAYER:
			aiActionL = 1.0f;
			break;
		case ENEMYAISTS.ESCAPE:
			aiActionL = Random.Range (2.0f, 5.0f);
			break;
		case ENEMYAISTS.RETRUNTODGPILE:
			aiActionL = 3.0f;
			break;
		}
	}

	public override void FixedUpdateAI () {
		if (player != null) {
		switch(aiState){
		case ENEMYAISTS.ACTIONSELECT:
			int n = SelectRandomAIState ();
			if (n < aiIfRun) {
				SetAIState (ENEMYAISTS.RUNTOPLAYER, 1.0f);
			} else if (n < aiIfRun + aiIfESCAPE) {
				SetAIState (ENEMYAISTS.ESCAPE, Random.Range (2.0f, 5.0f));
			} else if (n < aiIfRun + aiIfESCAPE + aiIfRturn) {
				if (dogPile != null) {
					SetAIState (ENEMYAISTS.RETRUNTODGPILE, 3.0f);
				   }
				}
				else  {
					SetAIState (ENEMYAISTS.WAIT,  Random.Range (0.0f, 0.3f));
				}
			enemy.Action (0.0f);
			break;

		case ENEMYAISTS.WAIT:
			enemy.ActionLook (player, 0.1f);
			enemy.Action (0.0f);
			break;

		case ENEMYAISTS.RUNTOPLAYER:
			if (GetDistanePlayerY () < 3.0f) {
				if (!enemy.ActionMoveToNear (player, 2.0f)) {
					Attack_A ();
				}
			} else { 
				if (GetDistanePlayerX()>3.0f &&!enemy.ActionMoveToNear (player, 5.0f)) {
					Attack_A ();
				}
			}
				break;
		case ENEMYAISTS.RETRUNTODGPILE:
			if (enemy.ActionMoveToNear (dogPile, 2.0f)) {
				if (GetDistanePlayer () < 2.0f) {
					Attack_A ();
				}
			} else {
				SetAIState (ENEMYAISTS.ACTIONSELECT, 1.0f);
			}
			break;

		case ENEMYAISTS.ESCAPE:
			if (!enemy.ActionMoveToFar (player, 4.0f)) {
				Attack_B();
			}
			break;
		}
			
			float x = Mathf.Abs (transform.position.x - player.transform.position.x);
			if (x < 2.0f && (player.transform.position.y < 1 && player.transform.position.y > -1)) {
				Attack_A ();
			}
		}
	}
	
	// Update is called once per frame
	void Attack_A () {
		enemy.ActionLook (player, 0.1f);
		enemy.Action (0.0f);
		enemy.ActionAttack ("Toko_Punch", damageA);
		enemy.attackNock = new Vector2 (500.0f, 2000.0f);
		SetAIState (ENEMYAISTS.FREEZ, 1.0f);
	}
	void Attack_B () {
		enemy.ActionLook (player, 0.1f);
		enemy.Action (0.0f);
		enemy.ActionAttack ("Toko_Attack1", damageA);

		firecount++;
		if (firecount > firea) {
			firecount = 0;
		}
		SetAIState (ENEMYAISTS.FREEZ, 1.0f);
	}
}
