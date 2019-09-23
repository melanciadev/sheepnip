using UnityEngine;
using System.Collections;

public class AudioPlay:MonoBehaviour {
	AudioSource aud;

	void Start() {
		aud = GetComponent<AudioSource>();
	}

	public void Play() {
		aud.Stop();
		aud.Play();
	}
}