using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Displacement/Fisheye")]
public class Fisheye:UnityStandardAssets.ImageEffects.PostEffectsBase {
	public float strengthX = .05f;
	public float strengthY = .05f;
	public Shader fishEyeShader = null;
	Material fisheyeMaterial = null;
	
	public override bool CheckResources() {
		CheckSupport (false);
		fisheyeMaterial = CheckShaderAndCreateMaterial(fishEyeShader,fisheyeMaterial);
		if (!isSupported) {
			ReportAutoDisable();
		}
		return isSupported;
	}
	
	void OnRenderImage(RenderTexture source,RenderTexture destination) {
		if (!CheckResources()) {
			Graphics.Blit(source,destination);
			return;
		}
		const float oneOverBaseSize = 80f/512; // to keep values more like in the old version of fisheye
		float ar = (float)source.width/source.height;
		fisheyeMaterial.SetVector("intensity",new Vector4(strengthX*ar*oneOverBaseSize,strengthY*oneOverBaseSize,strengthX*ar*oneOverBaseSize,strengthY*oneOverBaseSize));
		Graphics.Blit(source,destination,fisheyeMaterial);
	}
}