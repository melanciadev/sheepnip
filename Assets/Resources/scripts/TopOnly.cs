using UnityEngine;
using System.Collections;

public class TopOnly:MonoBehaviour {
	public bool working = true;
	
	Transform tr;
	BoxCollider2D hitbox;
	
	float pos;
	float sheepPos;
	
	void Start() {
		tr = transform;
		hitbox = GetComponent<BoxCollider2D>();
		pos = tr.position.y;
		sheepPos = Game.me.sheepTr.localPosition.y;
	}
	
	void FixedUpdate() {
		if (working) {
			float newPos = tr.position.y;
			float newSheepPos = Game.me.sheepTr.localPosition.y;
			if (!Mathf.Approximately(newPos,pos) || !Mathf.Approximately(newSheepPos,sheepPos)) {
				pos = newPos;
				sheepPos = newSheepPos;
				if (hitbox) Physics2D.IgnoreCollision(hitbox,Game.me.sheep.hitbox,sheepPos-.3f < hitbox.bounds.max.y);
			}
		}
	}
	
	public void Set(bool b) {
		if (b) {
			working = true;
			pos = tr.position.y;
			sheepPos = Game.me.sheepTr.localPosition.y;
            if (hitbox) Physics2D.IgnoreCollision(hitbox,Game.me.sheep.hitbox,sheepPos-.3f < hitbox.bounds.max.y);
		} else {
			working = false;
            if (hitbox) Physics2D.IgnoreCollision(hitbox,Game.me.sheep.hitbox,false);
		}
	}
}