using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	// player
	private PlayerController player;

	// enemy
	public List <GameObject> enemycards;
	public GameObject enemy;
	private float enemy_start_x;
	private bool is_enemy_start;
	private Transform enemies;
	private GameObject enemycard;
	private List <Transform> enemies_in_card;
	private float enemycard_renew_time;
	private float enemycard_start_time;
	private float enemycard_destory_delay;

	// game control
	private GameObject hidden;
	private float tutorial_time;
	private float game_start_time;
	public bool is_end;

	// curtain
	private Transform curtain;
	private float curtain_speed;
	private float curtain_highest_position_y;
	private float curtain_lowest_position_y;

	// danger
	private Transform danger;
	private float danger_speed;
	private float danger_original_position_x;
	private float danger_final_position_x;
	private ParticleSystem danger_particle;
	private float original_lifetime;

	// tutorial
	private bool is_tutorial_finished;
	private Transform tutorial;
	private List <Transform> tutorial_texts;
	private float tutorial_speed;
	private float tutorial_text_start_position_x;
	private float tutorial_text_end_position_x;
	private float tutorial_close_x_start;
	private float tutorial_close_x_end;
	private float tutorial_close_speed;
	public bool skip_tutorial;
	private float color_speed;

	// credits
	private bool is_credits_finished;
	public bool skip_credits;

	// game statistics
	private int kills;
	private int misses;
	private float distance;
	private int max_misses;
	private float distance_speed;

	// UI
	public GameObject UI_kill;

	// Use this for initialization
	void Start () {
		// enemy
		enemies = transform.Find ("Enemies");
		enemy_start_x = 10f;
		is_enemy_start = false;
		enemycard_renew_time = 8f;
		enemycard_start_time = Time.time;
		enemycard_destory_delay = 8f;

		// game control
		tutorial_time = 10f;
		game_start_time = Time.time;
		hidden = transform.Find ("Hidden").gameObject;
		if (hidden.activeSelf)
		{
			hidden.SetActive (false);
		}
		is_end = false;

		// curtain
		curtain = transform.Find ("Curtain");
		curtain_speed = 2f;
		curtain_highest_position_y = 4f;
		curtain_lowest_position_y = 0f;

		// danger
		danger = transform.Find ("Danger");
		danger_speed = 1f;
		danger_original_position_x = -10f;
		danger_final_position_x = -3.5f;
		danger.position = new Vector3 (danger_original_position_x, danger.position.y, danger.position.z);
		danger_particle = danger.GetComponent <ParticleSystem> ();
		original_lifetime = danger_particle.startLifetime;

		// game statistics
		kills = 0;
		misses = 0;
		max_misses = 100;
		distance = 0;
		distance_speed = 2f;

		// tutorials
		skip_tutorial = false;
		is_tutorial_finished = false;
		tutorial_speed = 6f;
		tutorial_text_start_position_x = 6f;
		tutorial_text_end_position_x = -12f;
		tutorial_close_x_start = -0.5f;
		tutorial_close_x_end = -3f;
		tutorial_close_speed = 0.6f;
		color_speed = 0.3f;
		tutorial = transform.Find ("Tutorial");
		tutorial_texts = new List <Transform> ();
		foreach (Transform child in tutorial)
		{
			tutorial_texts.Add (child);
			child.position += Vector3.right * tutorial_text_start_position_x;
			child.gameObject.GetComponent <TextMesh> ().color = new Color (1f, 1f, 1f, 0);
		}

		// player controller
		player = transform.Find ("Player").gameObject.GetComponent <PlayerController> ();

		// UI
		HideUI ();
	}

	// Update is called once per frame
	void Update () {
		if (!is_end)
		{
			if (curtain.position.y < curtain_highest_position_y)
			{
				curtain.position += Vector3.up * Time.deltaTime * curtain_speed;
			}
			else if (curtain.position.y != curtain_highest_position_y)
			{
				curtain.position = new Vector3 (curtain.position.x, curtain_highest_position_y, curtain.position.z);
			}
			UpdateCredits ();
			UpdateTutorial ();
			UpdateStatue ();
			UpdateEnemies ();
			UpdateUI ();
		}
		else
		{
			if (curtain.position.y > curtain_lowest_position_y)
			{
				curtain.position += Vector3.down * Time.deltaTime * curtain_speed;
			}
			else if (curtain.position.y != curtain_lowest_position_y)
			{
				curtain.position = new Vector3 (curtain.position.x, curtain_lowest_position_y, curtain.position.z);
			}
			else
			{
				if (Input.anyKey)
				{
					// restart game
					print ("anyKey");
					Init ();
				}
			}
			HideUI ();
		}
	}

	void UpdateStatue ()
	{
		if (!is_enemy_start && Time.time - game_start_time > tutorial_time && is_tutorial_finished)
		{
			is_enemy_start = true;
		}

		distance += Time.deltaTime * distance_speed;

		if (misses > 0 && danger.position.x < danger_final_position_x)
		{
			danger.position += Vector3.right * Time.deltaTime * danger_speed;
		}
		else if (danger.position.x != danger_final_position_x)
		{
			new Vector3 (danger_final_position_x, danger.position.y, danger.position.z);
		}

		if (danger.position.x != danger_final_position_x)
		{
			danger_particle.startLifetime = original_lifetime + Mathf.Round (misses / 10);
		}

		if (misses > max_misses)
		{
			End ();
		}
	}

	void UpdateEnemies ()
	{
		if (is_enemy_start)
		{
			// pick one enemycard
			if (enemycard == null)
			{
				Vector3 position = enemies.position;
				position.x += enemy_start_x;
				enemycard = Instantiate (enemycards[Random.Range (0, enemycards.Count-1)], position, enemies.rotation, enemies);
				enemies_in_card = new List <Transform> ();
				foreach (Transform child in enemycard.transform)
				{
					if (child.CompareTag (Tags.ENEMY))
					{
						enemies_in_card.Add (child);
					}
				}
				foreach (Transform child in enemies_in_card)
				{
					Instantiate (enemy, child.position, child.rotation, enemycard.transform);
					// enemies_in_card.Remove (child);
					Destroy (child.gameObject);
				}
				enemycard_start_time = Time.time;
			}
			else
			{
				if (Time.time - enemycard_start_time >= enemycard_renew_time)
				{
					Destroy (enemycard, enemycard_destory_delay);
					enemycard = null;
				}
			}
		}
	}

	void Init ()
	{
		is_end = false;
		danger.position = new Vector3 (danger_original_position_x, danger.position.y, danger.position.z);
		danger_particle = danger.GetComponent <ParticleSystem> ();
		original_lifetime = danger_particle.startLifetime;
		kills = 0;
		misses = 0;
		distance = 0;
		player.Init ();
	}

	public void End ()
	{
		is_end = true;
	}

	public void AddKills ()
	{
		kills ++;
		UpdateUI ();
	}

	public void AddMisses ()
	{
		misses ++;
	}

	void UpdateCredits ()
	{

	}

	void UpdateTutorial ()
	{
		if (!is_end && !is_tutorial_finished && !skip_tutorial)
		{
			if (tutorial_texts.Count > 0)
			{
				Transform tutorial_text = tutorial_texts[0];
				if (tutorial_text.position.x > tutorial_text_end_position_x)
				{
					if (tutorial_text.position.x > tutorial_close_x_start)
					{
						tutorial_text.position += Vector3.left * Time.deltaTime * tutorial_speed;
					}
					else if (tutorial_text.position.x > tutorial_close_x_end)
					{
						tutorial_text.position += Vector3.left * Time.deltaTime * tutorial_close_speed;
						Color color = tutorial_text.gameObject.GetComponent <TextMesh> ().color;
						color.a += Time.deltaTime * color_speed;
						tutorial_text.gameObject.GetComponent <TextMesh> ().color = color;
					}
					else
					{
						tutorial_text.position += Vector3.left * Time.deltaTime * tutorial_speed;
						Color color = tutorial_text.gameObject.GetComponent <TextMesh> ().color;
						color.a -= Time.deltaTime * color_speed * 3;
						tutorial_text.gameObject.GetComponent <TextMesh> ().color = color;
					}
				}
				else
				{
					tutorial_texts.Remove (tutorial_text);
					Destroy (tutorial_text.gameObject);
				}
			}
			else
			{
				Destroy (tutorial.gameObject);
				is_tutorial_finished = true;
			}
		}

		if (!is_end && !is_tutorial_finished && skip_tutorial)
		{
			Destroy (tutorial.gameObject);
			is_tutorial_finished = true;
		}
	}

	public void UpdateUI ()
	{
		if (!is_end && is_tutorial_finished)
		{
			ShowUI ();
			UI_kill.GetComponent <Text> ().text = "Kills: " + kills;
		}
	}

	void HideUI ()
	{
		if (UI_kill.activeSelf)
		{
			UI_kill.SetActive (false);
		}
	}

	void ShowUI ()
	{
		if (!UI_kill.activeSelf)
		{
			UI_kill.SetActive (true);
		}
	}
}
