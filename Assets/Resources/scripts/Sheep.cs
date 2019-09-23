using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sheep:MonoBehaviour {
	Transform tr; //transform
	Rigidbody2D rb; //rigidbody
	Transform spriteTr; //sprite transform
	[System.NonSerialized]
	public SpriteRenderer sprite; //sprite
	[System.NonSerialized]
	public BoxCollider2D hitbox; //hitbox
	Animator anim; //animação
	Transform arrowTr; //seta transform
	SpriteRenderer arrow; //seta
	AudioPlay aud;
	
	//colisão
	List<Collision2D> collisions = new List<Collision2D>(); //lista de colisões
	bool updateCollisions = false; //bool q verifica se tem q atualizar as colisões
	float cmin,cmax; //mínimo e máximo das colisões, pra ver como ficam as patas
	bool customForce = false; //isso faz com q ele ignore a velocidade na hora de arrumar
	Transform colAttached = null; //transform que a ovelha estava pisando
	Vector3 colAttachedPos; //posição do transform dito
	
	//status da ovelha
	public int highness = 0; //doidera da ovelha
	public float highnessFloat = 0; //doidera da ovelha em float
	public bool highnessUpdate = false; //true quando a highness foi alterada
	
	//outras infos
	bool grounded = false; //se está no chão
	int facing = 1; //direção na qual olha
	float velocity = 0; //velocidade no eixo x
	float velMultiplier = 3; //ovelhas por segundo quando anda
	float velDelta = 0; //mudança na velocidade quando anda
	float stopDelta = 0; //mudança na velocidade quando para
	float jumpTempo = 0; //tempo q ele ignora pulo novo
	[System.NonSerialized]
	public bool dead = false; //indica se a ovelha morreu
	int groundMode = 0;
		//0: parada normal
		//1: com as patas da frente soltas
		//2: com as patas de trás soltas
		//3: com as patas pro meio, por piso pequeno
		//4: com as patas esticadas, por pisos distantes
	
	int a;
	
	void Start() {
		tr = transform;
		rb = GetComponent<Rigidbody2D>();
		spriteTr = tr.Find("sprite");
		sprite = spriteTr.GetComponent<SpriteRenderer>();
		hitbox = tr.Find("hitbox").GetComponent<BoxCollider2D>();
		anim = spriteTr.GetComponent<Animator>();
		arrowTr = tr.Find("arrow");
		arrow = arrowTr.GetComponent<SpriteRenderer>();
		aud = spriteTr.GetComponent<AudioPlay>();

		sprite.enabled = false;
		dead = false;
		arrow.enabled = false;
		UpdateHighness();
	}
	
	void Update() {
		if (Game.me.loading || dead || !sprite.enabled) return;
		if (updateCollisions) {
			//se receber update do Collide(), atualizar e desligar a variável
			UpdateCollisions();
			updateCollisions = false;
		}
		highnessUpdate = false;
		if (highnessFloat > 1) {
			highnessFloat -= Time.deltaTime/16f;
			if (highnessFloat < 1) highnessFloat = 1;
			int i = Mathf.CeilToInt(highnessFloat);
			if (highness != i) {
				highness = i;
				highnessUpdate = true;
				UpdateHighness();
			}
		}
		if (grounded) {
			//caso esteja no chão
			if (!customForce && highness > 0 && jumpTempo == 0 && Input.GetButton("jump")) {
				//recebe input e pula dependendo da highness da ovelha
				switch (highness) {
					case 1: rb.AddForce(new Vector2(0,6.4f),ForceMode2D.Impulse); break;
					case 2: rb.AddForce(new Vector2(0,9),ForceMode2D.Impulse); break;
					case 3: rb.AddForce(new Vector2(0,11),ForceMode2D.Impulse); break;
				}
				jumpTempo = .125f;
				grounded = false;
				colAttached = null;
				aud.Play();
				anim.SetBool("jumping",true);
				anim.SetBool("falling",false);
			} else {
				anim.SetBool("jumping",false);
				anim.SetBool("falling",false);
			}
			velDelta = 8;
			stopDelta = 4;
		} else {
			//caso contrário
			velDelta = 3;
			stopDelta = 1;
			if (rb.velocity.y < 0) {
				anim.SetBool("jumping",false);
				anim.SetBool("falling",true);
			} else {
				anim.SetBool("jumping",true);
				anim.SetBool("falling",false);
			}
		}
		if (jumpTempo > 0) {
			jumpTempo -= Time.deltaTime;
			if (jumpTempo < 0) jumpTempo = 0;
		}
		//altera a velocidade da ovelha de acordo com o input
		int walk = (int)Input.GetAxisRaw("horizontal");
		if (walk != 0) {
			facing = walk;
			if (walk == 1) {
				if (spriteTr.localScale.x < 0) {
					spriteTr.localScale = new Vector3(-spriteTr.localScale.x,spriteTr.localScale.y,spriteTr.localScale.z);
				}
				if (velocity < 1) {
					velocity += Time.deltaTime*velDelta;
					if (velocity > 1) velocity = 1;
				}
			} else {
				if (spriteTr.localScale.x > 0) {
					spriteTr.localScale = new Vector3(-spriteTr.localScale.x,spriteTr.localScale.y,spriteTr.localScale.z);
				}
				if (velocity > -1) {
					velocity -= Time.deltaTime*velDelta;
					if (velocity < -1) velocity = -1;
				}
			}
			anim.SetBool("walking",true);
			anim.SetBool("balancing",false);
		} else {
			if (velocity != 0) {
				if (velocity > 0) {
					velocity -= Time.deltaTime*stopDelta;
					if (velocity < 0) velocity = 0;
				} else {
					velocity += Time.deltaTime*stopDelta;
					if (velocity > 0) velocity = 0;
				}
			}
			anim.SetBool("walking",false);
			anim.SetBool("balancing",groundMode > 0);
		}
		if (tr.localPosition.y >= 5) {
			arrow.enabled = true;
			arrowTr.localPosition = new Vector3(0,4.5f-tr.localPosition.y,0);
		} else {
			arrow.enabled = false;
		}
	}
	
	public void AddCustomForce(Vector2 f) {
		customForce = true;
		grounded = false;
		colAttached = null;
		aud.Play();
		anim.SetBool("jumping",true);
		jumpTempo = .125f;
		rb.velocity = f;
	}
	
	void FixedUpdate() {
		if (Game.me.loading || dead || !sprite.enabled) return;
		//altera velocidade no FixedUpdate(), já q teoricamente é um update de física
		//(coisas de física ficam nesse update diferente, mesmo)
		if (tr.localPosition.x < 0) {
			tr.localPosition = new Vector3(0,tr.localPosition.y,tr.localPosition.z);
		} else if (tr.localPosition.x > Game.me.totalWidth) {
			tr.localPosition = new Vector3(Game.me.totalWidth,tr.localPosition.y,tr.localPosition.z);
		}
		if (velocity != 0) {
			tr.localPosition = new Vector3(tr.localPosition.x+velocity*velMultiplier*Time.fixedDeltaTime,tr.localPosition.y,tr.localPosition.z);
		}
		Game.me.UpdateLevel();
		if (grounded) {
			if (jumpTempo == 0 && !Mathf.Approximately(rb.velocity.x,0)) {
				rb.velocity = new Vector2(0,rb.velocity.y);
			}
			if (colAttached != null) {
				//aqui serve pra mover a ovelha de acordo com o objeto q tem embaixo dela
				Vector3 pos = colAttached.position;
				tr.localPosition = new Vector3(tr.localPosition.x+(pos.x-colAttachedPos.x),tr.localPosition.y+(pos.y-colAttachedPos.y),tr.localPosition.z);
				colAttachedPos = pos;
			}
		}
		if (customForce) {
			if (Mathf.Approximately(rb.velocity.y,0)) {
				customForce = false;
			}
		} else {
			if (highness == 1) {
				if (rb.velocity.y > 6.4f) {
					rb.velocity = new Vector2(rb.velocity.x,6.4f);
				}
			} else if (highness == 2) {
				if (rb.velocity.y > 9) {
					rb.velocity = new Vector2(rb.velocity.x,9);
				}
			} else if (highness == 3) {
				if (rb.velocity.y > 11) {
					rb.velocity = new Vector2(rb.velocity.x,11);
				}
			}
		}
	}
	
	void OnCollisionEnter2D(Collision2D c) {
		Collide(c,true);
	}
	void OnCollisionStay2D(Collision2D c) {
		Collide(c,true);
	}
	void OnCollisionExit2D(Collision2D c) {
		Collide(c,false);
	}
	void Collide(Collision2D c,bool b) {
		//recebe uma colisão. b indica se é para adicionar a colisão ou removê-la
		for (a = 0; a < collisions.Count; a++) {
			if (c.collider == collisions[a].collider) {
				if (!b) {
					collisions.RemoveAt(a);
					updateCollisions = true;
					return;
				}
				collisions[a] = c;
				updateCollisions = true;
				return;
			}
		}
		if (b) collisions.Add(c);
		updateCollisions = true;
	}
	
	void UpdateCollisions() {
		//atualização de colisão. ele passa por toda a lista e vê como a ovelha tá
		grounded = false;
		Transform last = colAttached;
		colAttached = null;
		for (a = 0; a < collisions.Count; a++) {
			foreach (var contact in collisions[a].contacts) {
				if (contact.normal.y > .5f) {
					//a colisão vem por baixo
					if (!grounded) {
						grounded = true;
						cmin = cmax = contact.point.x-tr.localPosition.x;
						colAttached = collisions[a].transform;
						if (colAttached != last) {
							colAttachedPos = colAttached.position;
						}
					} else {
						if (cmin > contact.point.x-tr.localPosition.x) {
							cmin = contact.point.x-tr.localPosition.x;
						} else if (cmax < contact.point.x-tr.localPosition.x) {
							cmax = contact.point.x-tr.localPosition.x;
						}
					}
				}
			}
		}
		//aqui verifica como ela tá apoiada no chão, se tá no cantinho etc
		if (grounded) {
			if (cmin > 0) {
				groundMode = (facing == 1)?2:1;
			} else if (cmax < 0) {
				groundMode = (facing == 1)?1:2;
			} else if (Mathf.Abs(cmax-cmin) < .5f) {
				groundMode = 3;
			} else if (Physics2D.Raycast(new Vector2(tr.localPosition.x,tr.localPosition.y-.49f),-Vector2.up,.125f).collider == null) {
				groundMode = 4;
			} else {
				groundMode = 0;
			}
		}
	}
	
	public void AddHighness() {
		if (highness < 1) {
			highnessFloat = 1.00001f;
			Content.timeStart = Time.time;
			Game.me.audioBg.Play();
			Game.me.audioBg.volume = 1;
			Game.me.audioBad.Play();
			Game.me.audioBad.volume = .67f;
		} else {
			highnessFloat = Mathf.Round(highnessFloat)+1;
			if (highnessFloat > 3) {
				Die();
			}
		}
		Game.me.fisheyeTempo = 1;
	}
	
	public void Die() {
		if (dead) return;
		dead = true;
		Game.me.FadeOut(false);
	}
	
	void UpdateHighness() { 
		if (highness == 0) {
			Colouriser.color1 = new Color(.875f,.875f,.875f,1);
			Colouriser.color2 = new Color(.5f,.5f,.5f,1);
			Colouriser.color3 = new Color(.125f,.125f,.125f,1);
		} else if (highness == 1) {
			Colouriser.color1 = new Color32(160,242,201,255);
			Colouriser.color2 = new Color32(216,126,218,255);
			Colouriser.color3 = new Color32(119,170,195,255);
		} else if (highness == 2) {
			Colouriser.color1 = new Color32(255,234,114,255);
			Colouriser.color2 = new Color32(86,225,93,255);
			Colouriser.color3 = new Color32(114,95,216,255);
		} else if (highness == 3) {
			Colouriser.color1 = new Color32(255,255,0,255);
			Colouriser.color2 = new Color32(255,0,0,255);
			Colouriser.color3 = new Color32(114,95,216,255);
		}
		Colouriser.UpdateColours();
		Game.me.background.SetInteger("highness",highness);
		Game.me.blur.blurAmount = Mathf.Clamp01((highness-1)*.35f);
		Game.me.blankTempo = 1;
		//mudar as cores de acordo com a highness e coisa assim
	}
}