using UnityEngine;
using System.Collections;

public class BoredSheep:MonoBehaviour {
	public AudioClip[] clips;
	
	Transform tr;
	Animator anim;
	AudioSource aud;
	int h;
	
	void Start() {
		tr = transform;
		anim = tr.Find("sprite").GetComponent<Animator>();
		aud = GetComponent<AudioSource>();
		SetHighness();
	}
	
	void Update() {
		if (h != Game.me.sheep.highness) {
			SetHighness();
		}
	}
	
	void SetHighness() {
		h = Game.me.sheep.highness;
		anim.SetInteger("highness",h);
	}
	
	void OnCollisionEnter2D(Collision2D c) {
		if (c.transform == Game.me.sheepTr) {
			if (Game.me.sheep.highness == 1) {
				aud.clip = clips[0];
			} else if (Game.me.sheep.highness == 2) {
				aud.clip = clips[1];
				Game.me.sheep.AddCustomForce(new Vector2(0,11));
			} else if (Game.me.sheep.highness == 3) {
				aud.clip = clips[2];
				float y = 9;
				if (tr.localPosition.y > -2 && tr.localPosition.y > Game.me.sheepTr.localPosition.y) {
					y = -4;
				}
				if (Game.me.sheepTr.position.x-tr.position.x > 0) {
					Game.me.sheep.AddCustomForce(new Vector2(9,y));
				} else {
					Game.me.sheep.AddCustomForce(new Vector2(-9,y));
				}
			}
			aud.time = 0;
			aud.Play();
		}
	}
}