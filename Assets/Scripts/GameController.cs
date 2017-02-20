using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public List <GameObject> enemycards;
	public GameObject enemy;
	public PlayerController player;
	private float enemy_start_x;
	private bool is_enemy_start;
	private Transform enemies;
	private Transform curtain;
	private GameObject enemycard;
	private GameObject hidden;
	private List <Transform> enemies_in_card;
	private float enemycard_renew_time;
	private float enemycard_start_time;
	private float enemycard_destory_delay;
	private float tutorial_time;
	private float game_start_time;
	public bool is_end;
	private float curtain_speed;
	private float curtain_highest_position_y;
	private float curtain_lowest_position_y;

	// Use this for initialization
	void Start () {
		enemy_start_x = 10f;
		is_enemy_start = false;
		enemycard_renew_time = 6f;
		enemycard_start_time = Time.time;
		enemycard_destory_delay = 5f;
		tutorial_time = 10f;
		game_start_time = Time.time;
		enemies = transform.Find ("Enemies");
		curtain = transform.Find ("Curtain");
		hidden = transform.Find ("Hidden").gameObject;
		if (hidden.activeSelf)
		{
			hidden.SetActive (false);
		}
		is_end = false;
		curtain_speed = 2f;
		curtain_highest_position_y = 4f;
		curtain_lowest_position_y = 0f;
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
			UpdateStatue ();
			UpdateEnemies ();
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
		}
	}

	void UpdateStatue ()
	{
		if (!is_enemy_start && Time.time - game_start_time > tutorial_time)
		{
			is_enemy_start = true;
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
		player.Init ();
	}

	public void End ()
	{
		is_end = true;
	}
}
