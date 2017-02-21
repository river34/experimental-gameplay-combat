using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

	private float left_boundary;
	private float base_speed;
	private float speed;
	private float destory_delay;
	private float dead_destroy_delay;
	private bool is_missed;
	private Animator animator;
	private bool is_dead;
	private GameController game;

	// Use this for initialization
	void Start () {
		// game controller
		game = GameObject.Find ("Root").GetComponent <GameController> ();

		left_boundary = -4f;
		base_speed = 0.2f;
		speed = Random.Range (0.8f, 1.2f);
		speed += game.GetDifficultyLevel () * base_speed;
		destory_delay = 1f;
		dead_destroy_delay = 1f;
		is_missed = false;

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
				if (!is_missed)
				{
					is_missed = true;
					game.AddMisses ();
					Destroy (gameObject, destory_delay);
				}
			}
		}
	}

	public void KilledByPlayer ()
	{
		if (!is_dead)
		{
			is_dead = true;
			game.AddKills ();
			animator.SetTrigger ("Killed");
			Destroy (gameObject, dead_destroy_delay);
		}
	}
}
