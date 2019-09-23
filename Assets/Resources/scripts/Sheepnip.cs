using UnityEngine;
using System.Collections;

public class Sheepnip:MonoBehaviour {
	public AudioClip overdose;
	
	public float revive = 5;
	float deadTempo = 0;
	Animator anim;
	ParticleSystem particles;
	AudioSource aud;

	void Start() {
		anim = GetComponent<Animator>();
		particles = GetComponent<ParticleSystem>();
		aud = GetComponent<AudioSource>();
		anim.SetBool("down",false);
		anim.SetBool("up",true);
		anim.SetBool("tooHigh",false);
	}
	
	void Update() {
		if (deadTempo > 0) {
			deadTempo -= Time.deltaTime;
			if (deadTempo <= 0) {
				deadTempo = 0;
				anim.SetBool("down",false);
				anim.SetBool("up",true);
				particles.Play();
			}
		}
		anim.SetBool("tooHigh",Game.me.sheep.highnessFloat >= 2.5f);
	}
	
	void OnTriggerEnter2D(Collider2D c) {
		if (deadTempo > 0) return;
		if (c == Game.me.sheep.hitbox || c.transform == Game.me.sheepTr) {
			Game.me.sheep.AddHighness();
			deadTempo = revive;
			anim.SetBool("down",true);
			anim.SetBool("up",false);
			particles.Stop();
			if (Game.me.sheep.dead) {
				aud.clip = overdose;
			}
			aud.Play();
		}
	}
}