using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// position
	private bool is_up;
	private bool is_up_last_frame;
	private bool is_on_ground;
	private bool is_in_sky;
	private bool is_going_down;
	private float position_y;
	private float position_y_last_frame;
	private float max_up_speed = 8f;
	private float up_speed = 0f;
	private float up_force = 1.5f * 2;
	private float down_speed = 0f;
	private float gravity = 0.98f * 2;
	private Vector3 initial_position = new Vector3 (-2f, 1.2f, -2f);

	// animation
	private bool is_attacking;
	private bool is_attacking_last_frame;
	private bool is_killed;
	private bool is_dead;
	private Animator animator;
	// private Animator timer_animator;
	public Transform attack_timer;
	public GameObject attack_count_down;
	public Transform cd_timer;
	public GameObject cd_count_down;
	public float timer_position_offset;

	// cd time
	private float attack_time_limit;
	private float attack_start_time;
	private float attack_time;
	private float cd_time_limit;
	private float cd_start_time;
	private float cd_time;
	private bool is_attacking_triggered;

	// game controller
	private GameController game;
	private bool is_notified;

	// Use this for initialization
	void Start () {
		// position
		is_up = false;
		is_up_last_frame = false;
		is_on_ground = false;
		is_in_sky = false;
		is_going_down = false;
		transform.position = initial_position;
		position_y = transform.position.y;
		position_y_last_frame = transform.position.y;
		up_speed = max_up_speed;

		// animation
		is_attacking = false;
		is_attacking_last_frame = false;
		is_killed = false;
		is_dead = false;
		animator = GetComponent <Animator> ();
		// timer_animator = transform.Find ("Timer").gameObject.GetComponent <Animator> ();
		timer_position_offset = 0.4f;

		// cd time
		attack_time_limit = 3f;
		attack_start_time = Time.time;
		attack_time = Time.time;
		cd_time_limit = 3f;
		cd_start_time = Time.time;
		cd_time = Time.time;
		is_attacking_triggered = false;

		// game controller
		game = GameObject.Find ("Root").GetComponent <GameController> ();
		is_notified = false;
	}

	// Update is called once per frame
	void Update () {
		if (!is_dead)
		{
			CheckControls ();
			UpdateStatus ();
			UpdateMovement ();
			UpdateAnimation ();
		}
	}

	void LateUpdate () {
		LateUpdateStatues ();
	}

	void CheckControls ()
	{
		if (Input.GetKey ("up") || Input.GetKey ("w"))
		{
			// print ("up arrow key is held down");
			is_up = true;
		}
		else
		{
			is_up = false;
		}

		if (Input.GetKey ("down") || Input.GetKey ("s"))
		{
			// print ("down arrow key is held down");
		}

		if (Input.GetKey ("left") || Input.GetKey ("a"))
		{
			// print ("left arrow key is held down");
		}

		if (Input.GetKey ("right") || Input.GetKey ("d"))
		{
			// print ("right arrow key is held down");
			is_attacking = true;
		}
		else
		{
			is_attacking = false;
		}
	}

	void UpdateStatus ()
	{
		position_y = transform.position.y;

		if (position_y - position_y_last_frame < 0 || is_in_sky)
		{
			is_going_down = true;
		}
		else
		{
			is_going_down = false;
		}

		if (is_going_down)
		{
			is_up = false;
		}

		// cd time
		if (!is_attacking_last_frame)
		{
			attack_start_time = Time.time;
		}
		if (is_attacking_last_frame)
		{
			cd_start_time = Time.time;
		}
		attack_time = Time.time - attack_start_time;
		if (is_attacking && attack_time >= attack_time_limit)
		{
			is_attacking = false;
		}
		cd_time = Time.time - cd_start_time;
		if (is_attacking && is_attacking_triggered && !animator.GetBool ("Attack") && cd_time <= cd_time_limit)
		{
			is_attacking = false;
		}
		if (is_attacking_triggered && !animator.GetBool ("Attack") && cd_time > cd_time_limit)
		{
			is_attacking_triggered = false;
		}
	}

	void UpdateMovement ()
	{
		if (is_up_last_frame)
		{
			if (up_speed > 0)
			{
				up_speed -= (up_force - gravity) * Time.deltaTime;
			}
			else
			{
				up_speed = 0;
			}

			transform.position += Vector3.up * Time.deltaTime * Mathf.Max (up_speed - down_speed, 0);
		}
		else if (!is_on_ground)
		{
			down_speed += gravity * Time.deltaTime;
			transform.position += Vector3.down * Time.deltaTime * down_speed;
		}
		if (is_on_ground)
		{
			down_speed = 0f;
		}
		else if (is_in_sky)
		{
			up_speed = max_up_speed;
		}

		attack_timer.position = transform.position + Vector3.up * timer_position_offset;
		cd_timer.position = transform.position + Vector3.up * timer_position_offset;
	}

	void UpdateAnimation ()
	{
		if (is_attacking)
		{
			// play attack animation
			// if collision detected, ... -> implement in OnCollisionEnter2D
			if (!animator.GetBool ("Attack"))
			{
				is_attacking_triggered = true;
				animator.SetBool ("Attack", true);
				// timer_animator.SetBool ("AttackCountDown", true);
			}

			// generate attack timer
			if (attack_time < attack_time_limit / 3) // 3
			{
				if (attack_timer.childCount <= 0)
				{
					for (int i = 0; i < 3; i++)
					{
						Vector3 position = attack_timer.position;
						position.x += (i-1) * 0.2f;
						position.y += (i%2-1) * 0.06f;
						Instantiate (attack_count_down, position, attack_timer.rotation, attack_timer);
					}
				}
			}
			else if (attack_time < attack_time_limit / 3 * 2) // 2
			{
				if (attack_timer.childCount >= 3)
				{
					Destroy (attack_timer.GetChild (2).gameObject);
				}
			}
			else // 1
			{
				if (attack_timer.childCount >= 2)
				{
					Destroy (attack_timer.GetChild (1).gameObject);
				}
			}
		}
		else // !is_attacking
		{
			if (animator.GetBool ("Attack"))
			{
				// end attack timer
				animator.SetBool ("Attack", false);
				// timer_animator.SetBool ("AttackCountDown", false);
				// start cd timer
				// timer_animator.SetTrigger ("CDCountDown");

				// generate cd timer
				if (cd_timer.childCount <= 0)
				{
					for (int i = 0; i < 3; i++)
					{
						Vector3 position = cd_timer.position;
						position.x += (i-1) * 0.2f;
						position.y += (i%2-1) * 0.06f;
						Instantiate (cd_count_down, position, cd_timer.rotation, cd_timer);
					}
				}
			}

			// cd timer
			if (cd_time < cd_time_limit / 3) // 3
			{
				//
			}
			else if (cd_time < cd_time_limit / 3 * 2) // 2
			{
				if (cd_timer.childCount >= 3)
				{
					Destroy (cd_timer.GetChild (2).gameObject);
				}
			}
			else if (cd_time < cd_time_limit) // 1
			{
				if (cd_timer.childCount >= 2)
				{
					Destroy (cd_timer.GetChild (1).gameObject);
				}
			}
			else // 0
			{
				foreach (Transform child in cd_timer)
				{
					Destroy (child.gameObject);
				}
			}

			if (attack_timer.childCount > 0)
			{
				foreach (Transform child in attack_timer)
				{
					Destroy (child.gameObject);
				}
			}
		}

		if (cd_time > cd_time_limit)
		{
			foreach (Transform child in cd_timer)
			{
				Destroy (child.gameObject);
			}
		}

		if (is_killed && !is_dead)
		{
			animator.SetTrigger ("Killed");
			is_dead = true;
			attack_timer.gameObject.SetActive (false);
			cd_timer.gameObject.SetActive (false);
		}
	}

	void LateUpdateStatues ()
	{
		is_up_last_frame = is_up;
		is_attacking_last_frame = is_attacking;
		position_y_last_frame = position_y;

		if (!is_notified && is_dead)
		{
			game.End ();
			is_notified = true;
		}
	}

	void OnCollisionEnter2D (Collision2D other)
	{
		if (other.gameObject.name == "Ground")
		{
			is_on_ground = true;
		}
		if (other.gameObject.name == "Sky")
		{
			is_in_sky = true;
		}
	}

	void OnCollisionExit2D (Collision2D other)
	{
		if (other.gameObject.name == "Ground")
		{
			is_on_ground = false;
		}
		if (other.gameObject.name == "Sky")
		{
			is_in_sky = false;
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.gameObject.CompareTag (Tags.ENEMY))
		{
			if (is_attacking)
			{
				other.gameObject.tag = "Finish";
				other.gameObject.GetComponent <EnemyController> ().KilledByPlayer ();
			}
			else
			{
				is_killed = true;
			}
		}
	}

	public bool GetAttacking ()
	{
		return is_attacking;
	}

	public void Init ()
	{
		is_notified = false;
		is_dead = false;
		is_killed = false;
		animator.SetTrigger ("Revived");
		transform.position = initial_position;
		position_y = transform.position.y;
		position_y_last_frame = transform.position.y;
		attack_timer.gameObject.SetActive (true);
		cd_timer.gameObject.SetActive (true);
	}
}
