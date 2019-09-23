using UnityEngine;
using System.Collections;

public class Intro:MonoBehaviour {
	AudioSource aud;
	SpriteRenderer[] sprites;
	int spriteLen;
	float tempo = 0;
	int spriteIndex = 0;

	bool baahd = false;

	void Start() {
		Game.me = null;
		aud = GetComponent<AudioSource>();
		var tr = transform;
		spriteLen = tr.childCount;
		sprites = new SpriteRenderer[spriteLen];
		for (int a = 0; a < sprites.Length; a++) {
			var t = tr.Find(a.ToString());
			if (t == null) {
				spriteLen = a;
				break;
			}
			sprites[a] = t.GetComponent<SpriteRenderer>();
			if (sprites[a] == null) {
				spriteLen = a;
				break;
			}
			sprites[a].color = new Color(sprites[a].color.r,sprites[a].color.g,sprites[a].color.b,0);
		}
	}

	void Update() {
		if (Input.GetButtonDown("reset")) {
			Application.LoadLevel(0);
			return;
		}
		tempo += Time.deltaTime;
		if (tempo >= 4) {
			sprites[spriteIndex].color = Color.clear;
			while (tempo >= 4) tempo -= 4;
			spriteIndex++;
			if (spriteIndex >= spriteLen) {
				Application.LoadLevel(1);
				return;
			}
		} else if (tempo >= 3) {
			sprites[spriteIndex].color = new Color(sprites[spriteIndex].color.r,sprites[spriteIndex].color.g,sprites[spriteIndex].color.b,4-tempo);
		} else if (tempo >= 1) {
			sprites[spriteIndex].color = new Color(sprites[spriteIndex].color.r,sprites[spriteIndex].color.g,sprites[spriteIndex].color.b,1);
			if (Input.GetButton("jump")) tempo = 3;
			if (spriteIndex == 2 && !baahd) {
				baahd = true;
				aud.Play();
			}
		} else {
			sprites[spriteIndex].color = new Color(sprites[spriteIndex].color.r,sprites[spriteIndex].color.g,sprites[spriteIndex].color.b,tempo);
		}
	}
}