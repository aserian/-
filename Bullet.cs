using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public int power =1;
	private Enemy enemy;
	private GameObject Player;
	private int speed = 10;
	// Use this for initialization
	void Start () {
		Destroy (gameObject, 5);
 	}

	void Update()
	{
		float a = transform.eulerAngles.z * (Mathf.PI / 180.0f);
		Vector3 dir = new Vector3 (Mathf.Cos (a), Mathf.Sin (a), 0.0f);
		transform.position += dir * speed * Time.deltaTime;
	}
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Enemy") {
			Destroy (gameObject);
		}
	}
}
