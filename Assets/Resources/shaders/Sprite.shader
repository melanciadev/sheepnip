// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sheepnip/Sprite"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color1 ("Color #1", Color) = (1,1,1,1)
		_Color2 ("Color #2", Color) = (1,1,1,1)
		_Color3 ("Color #3", Color) = (1,1,1,1)
		_Offset ("Offset", Range (-1, 1)) = 0
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color1;
			fixed4 _Color2;
			fixed4 _Color3;
			fixed _Offset;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 rgb = tex2D(_MainTex,IN.texcoord);
				fixed4 c;
				fixed g = lerp(rgb.r,rgb.r+_Offset,(1-rgb.r)*rgb.r);
				if (g < .25) {
					c = lerp(fixed4(0,0,0,1),_Color3,g*4);
				} else if (g < .5) {
					c = lerp(_Color3,_Color2,(g-.25)*4);
				} else if (g < .75) {
					c = lerp(_Color2,_Color1,(g-.5)*4);
				} else {
					c = lerp(_Color1,fixed4(1,1,1,1),(g-.75)*4);
				}
				c.a = rgb.a;
				return c;
			}
		ENDCG
		}
	}
}
