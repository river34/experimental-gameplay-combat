using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	private float left_boundary;
	private float speed;
	private float destory_delay;
	private float dead_destroy_delay;
	private Animator animator;
	private bool is_dead;

	// Use this for initialization
	void Start () {
		left_boundary = -4f;
		speed = Random.Range (2f, 2.5f);
		destory_delay = 1f;
		dead_destroy_delay = 0.5f;

		// animation
		animator = GetComponent <Animator> ();
		is_dead = false;
	}

	// Update is called once per frame
	void Update () {
		if (!is_dead)
		{
			if (transform.position.x > left_boundary)
			{
				transform.position += Vector3.left * Time.deltaTime * speed;
			}
			else
			{
				Destroy (gameObject, destory_delay);
			}
		}
	}

	/*
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.name == "Player")
		{
			if (other.gameObject.GetComponent <PlayerController> ().GetAttacking ())
			{
				Destroy (gameObject);
				print ("Destroy");
			}
		}
	}
	*/

	public void KilledByPlayer ()
	{
		if (!is_dead)
		{
			is_dead = true;
			animator.SetTrigger ("Killed");
			Destroy (gameObject, dead_destroy_delay);
		}
	}
}
