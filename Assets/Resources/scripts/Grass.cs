using UnityEngine;
using System.Collections;

public class Grass:MonoBehaviour {
	Animator anim;
	int h;

	void Start() {
		anim = GetComponent<Animator>();
		SetHighness();
	}

	void Update() {
		if (h != Game.me.sheep.highness) {
			SetHighness();
		}
	}

	void SetHighness() {
		h = Game.me.sheep.highness;
		anim.SetBool("transformTo01",h == 1);
		anim.SetBool("transformTo02",h == 2);
		anim.SetBool("transformTo03",h == 3);
	}
}