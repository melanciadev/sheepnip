using UnityEngine;
using System.Collections;

public class Game:MonoBehaviour {
	//variável estática q serve como singleton
	public static Game me = null;
	
	//lista de cenas.
	//coloque aqui e no Build Settings as cenas que serão usadas!!
	//a ordem das strings importa, e ela diz qual a ordem dos blocos.
	string[] sceneNames = new string[] {
		"map1",
		"map2",
		"map3",
		"map4",
		"map5",
		"map6",
		"map7",
		"map8",
		"map9"
	};
	Level[] levels;
	[System.NonSerialized]
	public int levelIndex;
	[System.NonSerialized]
	public int levelCount;
	float levelTempo = 0;
	
	[System.NonSerialized]
	public bool loading;
	[System.NonSerialized]
	public bool credits;
	float fadeTempo = 0;
	float startTempo = 1;
	
	//referências a gameobjects e seus componentes.
	//acessar os componentes diretamente por .GetComponent<T>
	//ou por .transform (q internamente é a mesma coisa que
	//.GetComponent<Transform>!!) é meio lento...
	
	Transform tr; //transform do objeto "game"
	Transform camTr; //transform da câmera
	Camera cam; //câmera
	
	[System.NonSerialized]
	public Animator background;
	Fisheye fisheye; //efeito de fisheye da câmera
	[System.NonSerialized]
	public MotionBlur blur; //efeito de motion blur
	[System.NonSerialized]
	public float fisheyeTempo = 0;
	SpriteRenderer fade;
	SpriteRenderer blank;
	Animator fading;

	Transform creditsTr;
	Transform creditsScroll;
	float creditsTempo = 0;
	
	[System.NonSerialized]
	public float blankTempo = 0;
	
	Transform meterTr; //transform de tudo do medidor
	Transform meterHeight; //o medidor em si, talvez temporário esse
	
	[System.NonSerialized]
	public Transform sheepTr; //transform da ovelha
	[System.NonSerialized]
	public Sheep sheep; //ovelha
	Transform badTripTr;
	
	[System.NonSerialized]
	public AudioSource audioBg;
	[System.NonSerialized]
	public AudioSource audioBad;
	
	float camX; //posição da câmera
	float minX,maxX; //limites da câmera
	[System.NonSerialized]
	public int totalWidth; //limites do personagem
	
	public float badTripPos = 0;
	
	int width,height; //resolução da janela

	void Start() {
		//o objeto, assim que é criado, assume papel como singleton
		if (me != null) {
			Destroy(gameObject);
			return;
		}
		me = this;
		Content.Start();
		loading = true;
		credits = false;
		
		//marcar as referências
		tr = transform;
		camTr = tr.Find("cam");
		cam = camTr.GetComponent<Camera>();
		
		background = camTr.Find("bg").GetComponent<Animator>();
		fisheye = camTr.GetComponent<Fisheye>();
		blur = camTr.GetComponent<MotionBlur>();
		fade = camTr.Find("fade").GetComponent<SpriteRenderer>();
		blank = camTr.Find("blank").GetComponent<SpriteRenderer>();
		fading = camTr.Find("fading").GetComponent<Animator>();

		creditsTr = camTr.Find("credits");
		creditsScroll = creditsTr.Find("scroll");
		
		meterTr = camTr.Find("meter");
		meterHeight = meterTr.Find("height");
		
		sheepTr = tr.Find("sheep");
		sheep = sheepTr.GetComponent<Sheep>();
		badTripTr = tr.Find("badtrip");

		audioBg = camTr.GetComponent<AudioSource>();
		audioBad = badTripTr.GetComponent<AudioSource>();
		
		//carregar cenas
		//na vdd isso depende. se o cara tá no _main, ele carrega o jogo normal.
		//senão, ele carrega só as coisas atuais, para testar essa fase específica.
		#if UNITY_EDITOR
		if (Application.loadedLevelName == "_main") {
		#endif
			//carregar todas as cenas
			levelCount = sceneNames.Length;
			levels = new Level[levelCount];
			StartCoroutine(LoadLevels());
		#if UNITY_EDITOR
		} else {
			//carreegar só a cena atual.
			//isso só funciona no editor pq né
			levels = new Level[] {(Level)GameObject.FindObjectsOfType(typeof(Level))[0]};
			levels[0].index = 0;
			levels[0].start = 0;
			levels[0].end = levels[0].width;
			totalWidth = levels[0].width;
			levelCount = 1;
			loading = false;
			
			//iniciar nível
			levelIndex = 0;
			UpdateLevel();
			UpdateCameraLimits();
			camX = minX;
			camTr.localPosition = new Vector3(camX,camTr.localPosition.y,camTr.localPosition.z);
		}
		#endif
		
		fisheye.strengthX = fisheye.strengthY = 0;
		#if UNITY_EDITOR
		if (Application.loadedLevelName == "_main" || Application.loadedLevelName == "map1") {
			meterTr.localPosition = new Vector3(meterTr.localPosition.x,meterTr.localPosition.y,500);
		} else {
			meterTr.localPosition = new Vector3(meterTr.localPosition.x,meterTr.localPosition.y,0);
			sheep.AddHighness();
		}
		#else
		meterTr.localPosition = new Vector3(meterTr.localPosition.x,meterTr.localPosition.y,500);
		#endif
		
		badTripPos = -10;
		badTripTr.localPosition = new Vector3(-10,badTripTr.localPosition.y,badTripTr.localPosition.z);
		fade.color = Color.black;
		creditsTr.localPosition = new Vector3(0,0,-500);
		blank.color = new Color(1,1,1,0);
		
		//verifica se mostra o tempo ou não --- JU
		if (Content.firstRun) {
			tr.Find("tutorial").localPosition = Vector3.zero;
			tr.Find("status").localPosition = new Vector3(0,0,500);
		} else {
			tr.Find("status").localPosition = Vector3.zero;
			tr.Find("tutorial").localPosition = new Vector3(0,0,500);
			float timeFinal = Content.timeEnd-Content.timeStart;
			string prefix;
			if (Content.gotThrough) {
				prefix = "completed in ";
			} else {
				prefix = "lived for ";
			}
			tr.Find("status/text").GetComponent<TextMesh>().text = prefix+ ((int)(timeFinal / 60)).ToString("D2") +":" + (timeFinal % 60).ToString("00.000") +"\n"+ Content.collectablesLast +"/8 flowers colected";
		}
		
		//atualizar resolução do jogo
		SetCamera();
	}
	
	IEnumerator LoadLevels() {
		for (int a = 0; a < levelCount; a++) {
			Application.LoadLevelAdditive(sceneNames[a]);
		}
		yield return 0;
		var levelsTemp = (Level[])GameObject.FindObjectsOfType(typeof(Level));
		totalWidth = 0;
		for (int a = 0; a < levelCount; a++) {
			levels[a] = null;
			foreach (var temp in levelsTemp) {
				if (temp.levelName == sceneNames[a]) {
					levels[a] = temp;
					break;
				}
			}
			if (levels[a] == null) {
				throw new System.InvalidOperationException("A cena \""+sceneNames[a]+"\" não encontrou um componente com este nome!");
			}
			levels[a].index = a;
			levels[a].start = totalWidth;
			levels[a].transform.localPosition = new Vector3(totalWidth,0,0);
			totalWidth += levels[a].width;
			levels[a].end = totalWidth;
		}
		loading = false;
		
		//iniciar nível
		levelIndex = 0;
		UpdateLevel();
		UpdateCameraLimits();
		camX = minX;
		camTr.localPosition = new Vector3(camX,0,0);
	}
	
	void Update() {
		//verifica se tem q resetar o jogo
		if (Input.GetButtonDown("reset")) {
			Application.LoadLevel(0);
			return;
		}
		if (blankTempo > 0) {
			blankTempo -= Time.deltaTime;
			if (blankTempo < 0) blankTempo = 0;
			blank.color = new Color(1,1,1,blankTempo*.25f+.5f);
		}
		if (startTempo > 0) {
			startTempo -= Time.deltaTime;
			audioBg.volume -= Time.deltaTime;
			audioBad.volume -= Time.deltaTime;
			if (startTempo < 0) startTempo = 0;
			fade.color = new Color(fade.color.r,fade.color.g,fade.color.b,startTempo);
		}
		if (levelTempo > 0) {
			levelTempo -= Time.deltaTime;
			audioBg.volume -= Time.deltaTime;
			audioBad.volume -= Time.deltaTime;
			fade.color = new Color(fade.color.r,fade.color.g,fade.color.b,(3-levelTempo)*.5f);
			if (levelTempo <= 0) {
				levelTempo = 0;
				Content.firstRun = false;
				Application.LoadLevel(1);
				return;
			}
		}
		if (credits) {
			const int duration = 30;
			if (creditsTempo < duration) {
				creditsTempo += Time.deltaTime*1.5f;
				if (creditsTempo >= duration || Input.GetButtonDown("jump")) {
					levelTempo = 4;
					fading.SetBool("animate",true);
					fade.color = new Color(0,0,0,0);
				}
			} else {
				creditsTempo += Time.deltaTime;
			}
			creditsScroll.localPosition = new Vector3(0,creditsTempo - 10,0);
			return;
		}
		if (fadeTempo > 0) {
			fadeTempo -= Time.deltaTime;
			fade.color = new Color(fade.color.r,fade.color.g,fade.color.b,2-fadeTempo);
			if (fadeTempo <= 0) {
				startTempo = 1;
				credits = true;
				creditsTr.localPosition = Vector3.zero;
			}
		}
		//atualiza o sistema de cores
		Colouriser.UpdateStatic();
		//atualiza a resolução sempre que detectar mudança na janela
		if (width != Screen.width || height != Screen.height) {
			SetCamera();
		}
		//atualiza o fisheye
		if (fisheyeTempo > 0) {
			fisheyeTempo -= Time.deltaTime;
			if (fisheyeTempo < 0) fisheyeTempo = 0;
			fisheye.strengthX = fisheye.strengthY = fisheyeTempo*fisheyeTempo*.25f+.05f;
		} else if (fisheyeTempo < 0) {
			fisheyeTempo += Time.deltaTime;
			if (fisheyeTempo > 0) fisheyeTempo = 0;
			fisheye.strengthX = fisheye.strengthY = -fisheyeTempo*fisheyeTempo*.05f+.05f;
		}
		//atualiza o medidor
		if (sheep.highnessUpdate) {
			if (sheep.highness > 0) {
				meterTr.localPosition = new Vector3(meterTr.localPosition.x,meterTr.localPosition.y,0);
			} else {
				meterTr.localPosition = new Vector3(meterTr.localPosition.x,meterTr.localPosition.y,500);
			}
		}
		if (sheep.highness > 0 && fadeTempo == 0) {
			if (levelIndex == levelCount-1 && sheepTr.localPosition.y < -5) {
				FadeOut(true);
			} else {
				badTripPos += Time.deltaTime;
				if (badTripPos >= sheepTr.localPosition.x) {
					sheep.Die();
				}
				badTripTr.localPosition = new Vector3(badTripPos,badTripTr.localPosition.y,badTripTr.localPosition.z);
			}
		}
		meterHeight.localPosition = new Vector3(meterHeight.localPosition.x,sheep.highnessFloat-1,meterHeight.localPosition.z);
		meterHeight.localScale = new Vector3(meterHeight.localScale.x,(sheep.highnessFloat-1)*.4f,meterHeight.localScale.z);
	}
	
	void LateUpdate() {
		if (loading) return;
		float pos = Mathf.Clamp(sheepTr.localPosition.x,minX,maxX);
		camX = Mathf.Lerp(camX,pos,Time.deltaTime*8);
		if (Mathf.Abs(camX-pos) < .01f) {
			camX = pos;
			if (levelIndex == levelCount-1) {
				if (badTripPos < minX-16) {
					badTripPos = minX-16;
				}
			}
		}
		camTr.localPosition = new Vector3(camX,0,0);
	}
	
	void SetCamera() {
		//a proporção é sempre 16:9!! por convenção, na real
		//ele redimensiona o retângulo da câmera
		const float p = 16f/9;
		width = Screen.width;
		height = Screen.height;
		float prop = (float)width/height;
		if (prop > p) {
			float a = height*p;
			cam.pixelRect = new Rect((width-a)*.5f,0,a,height);
		} else if (prop < p) {
			float a = width/p;
			cam.pixelRect = new Rect(0,(height-a)*.5f,width,a);
		} else {
			cam.pixelRect = new Rect(0,0,width,height);
		}
	}
	
	public void UpdateLevel() {
		if (levelIndex > 0 && sheepTr.localPosition.x < levels[levelIndex].start) {
			if (levelIndex == levelCount-1) {
				sheepTr.localPosition = new Vector3(levels[levelIndex].start,sheepTr.localPosition.y,sheepTr.localPosition.z);
			} else {
				levelIndex--;
				UpdateCameraLimits();
			}
		} else if (levelIndex < levelCount-1 && sheepTr.localPosition.x > levels[levelIndex].end) {
			levelIndex++;
			UpdateCameraLimits();
		}
	}
	
	void UpdateCameraLimits() {
		if (levels[levelIndex].width == 18) {
			minX = maxX = levels[levelIndex].start+9;
		} else {
			minX = levels[levelIndex].start+8.8888888f;
			maxX = levels[levelIndex].end-8.8888888f;
		}
	}
	
	public void FadeOut(bool b) {
		Content.timeEnd = Time.time;
		Content.gotThrough = b;
		if (b) {
			fade.color = new Color(1,1,1,0);
			fadeTempo = 2;
		} else {
			fade.color = new Color(0,0,0,0);
			fading.SetBool("animate",true);
			levelTempo = 3;
		}
	}
}