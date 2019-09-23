using UnityEngine;
using System.Collections;

public class ShowPlayer:MonoBehaviour {
	public void JustDoIt() {
		Game.me.sheep.sprite.enabled = true;
	}
}