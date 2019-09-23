using UnityEngine;
using System.Collections;

public class Fence:MonoBehaviour {
	Transform tr;
	Animator anim;
	AnimHeight height;
	Transform hitbox;
	TopOnly topOnly;
	AudioSource aud;
	int h;
	
	void Start() {
		tr = transform;
		anim = tr.Find("sprite").GetComponent<Animator>();
		height = tr.Find("sprite").GetComponent<AnimHeight>();
		hitbox = tr.Find("hitbox");
		topOnly = hitbox.GetComponent<TopOnly>();
		aud = GetComponent<AudioSource>();
		topOnly.Set(false);
		SetHighness();
	}
	
	void Update() {
		if (h != Game.me.sheep.highness) {
			if (h > 0) aud.Play();
			SetHighness();
		}
		if (height.height > 3) {
			if (!topOnly.working) {
				topOnly.Set(true);
			}
			hitbox.localPosition = new Vector3(0,height.height-.8f,0);
		} else {
			if (topOnly.working) {
				topOnly.Set(false);
			}
			hitbox.localPosition = new Vector3(0,height.height-1,0);
		}
	}

	void SetHighness() {
		h = Game.me.sheep.highness;
		anim.SetInteger("highness",h);
	}
}