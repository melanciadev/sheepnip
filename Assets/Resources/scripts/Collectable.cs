using UnityEngine;
using System.Collections;

public class Collectable:MonoBehaviour {
	bool got = false;
	SpriteRenderer rend;
	ParticleSystem particles;
	AudioSource aud;

	void Start() {
		rend = GetComponent<SpriteRenderer>();
		particles = GetComponent<ParticleSystem>();
		aud = GetComponent<AudioSource>();
	}

	void OnTriggerEnter2D(Collider2D c) {
		if (got) return;
		if (c == Game.me.sheep.hitbox || c.transform == Game.me.sheepTr) {
			got = true;
			Content.collectables++;
			rend.enabled = false;
			particles.Emit(8);
			aud.Play();
		}
	}
}