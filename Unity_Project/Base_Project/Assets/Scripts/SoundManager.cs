using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public AudioClip backgroundAudio;

	// Use this for initialization
	void Start () {
	
		GetComponent<AudioSource> ().clip = backgroundAudio;
		AudioListener.volume = GameRepository.GetVolumeLevel() / 10.0F;
		PlayBackgroundSfx ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*---------------------------------------------------------------------------------------------------------------*/
	
	private void PlayBackgroundSfx()
	{
		GetComponent<AudioSource> ().Play ();
	}
}
