using UnityEngine;
using System.Collections;

public static class Content {
	public static bool awake = false;
	public static int collectables;
	public static int collectablesLast;
	public static int collectableMax;
	public static bool firstRun;
	public static bool gotThrough;
	public static float timeStart;
	public static float timeEnd;
	
	public static void Start() {
		if (!awake) {
			awake = true;
			collectableMax = 8;
			firstRun = true;
			gotThrough = false;
			collectables = 0;
		}
		collectablesLast = collectables;
		collectables = 0;
	}
}