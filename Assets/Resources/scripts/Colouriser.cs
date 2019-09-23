using UnityEngine;
using System.Collections;

public class Colouriser:MonoBehaviour {
	public static Color color1 = Color.red;
	public static Color color2 = Color.green;
	public static Color color3 = Color.blue;
	
	static bool update = false;
	static float tempo = 0;
	
	Material mat;
	
	void Start() {
		mat = GetComponent<SpriteRenderer>().material;
		UpdateColoursEach();
	}
	
	void Update() {
		if (update) {
			UpdateColoursEach();
		}
		mat.SetFloat("_Offset",tempo);
	}
	
	void UpdateColoursEach() {
		mat.SetColor("_Color1",color1);
		mat.SetColor("_Color2",color2);
		mat.SetColor("_Color3",color3);
	}
	
	public static void UpdateStatic() {
		update = false;
		tempo = Mathf.Sin(Time.time*2)*.5f;
	}
	
	public static void UpdateColours() {
		update = true;
	}
}