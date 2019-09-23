using UnityEngine;
using System.Collections;

public class Plant:MonoBehaviour {
	Transform tr;
	Animator anim;
	AnimHeight height;
	Transform hitbox;

	void Start() {
		tr = transform;
		anim = tr.Find("sprite").GetComponent<Animator>();
		height = tr.Find("sprite").GetComponent<AnimHeight>();
		hitbox = tr.Find("hitbox");
	}
	
	void Update() {
		if (Game.me.sheep.highness < 1) return;
		anim.SetInteger("highness",Game.me.sheep.highness);
		hitbox.localPosition = new Vector3(0,height.height,0);
	}
}