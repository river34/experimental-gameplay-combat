using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	private AudioSource opening;
	private AudioSource gaming;
	private AudioSource ending;
	private AudioSource saw;
	private AudioSource jump;

	// Use this for initialization
	void Start () {
		opening = transform.Find ("Opening").GetComponent <AudioSource> ();
		gaming = transform.Find ("Gaming").GetComponent <AudioSource> ();
		ending = transform.Find ("Ending").GetComponent <AudioSource> ();
		saw = transform.Find ("Saw").GetComponent <AudioSource> ();
		jump = transform.Find ("Jump").GetComponent <AudioSource> ();
	}

	public void PlayMusic (string name)
	{
		if (name == "opening")
		{
			opening.Play ();
		}
		if (name == "gaming")
		{
			opening.Stop ();
			gaming.Play ();
			ending.Stop ();
		}
		if (name == "ending")
		{
			gaming.Stop ();
			ending.Play ();
		}
	}

	public void PlaySound (string name)
	{
		if (name == "saw")
		{
			if (!saw.isPlaying)
			{
				saw.Play ();
			}
		}
		if (name == "jump")
		{
			if (!jump.isPlaying)
			{
				jump.Play ();
			}
		}
	}
}
