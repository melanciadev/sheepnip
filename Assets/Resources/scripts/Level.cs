using UnityEngine;
using System.Collections;

public class Level:MonoBehaviour {
	public string levelName = "";
	public int width = 18;
	[System.NonSerialized]
	public int index = -1;
	[System.NonSerialized]
	public int start = -1;
	[System.NonSerialized]
	public int end = -1;
	
	#if UNITY_EDITOR
	void OnDrawGizmos() {
		if (width < 18) width = 18;
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(new Vector3(width*.5f,0,0)+transform.localPosition,new Vector3(width,10,25));
	}
	#endif
}