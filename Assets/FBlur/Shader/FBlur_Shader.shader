Shader "FBlur/Fast Gaussian"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_UIMask ("Texture", 2D) = "white" {}
		_UIBlured ("Texture", 2D) = "white" {}
		_Blur("Blur",Float) = 0.01
	}
	SubShader
	{
	  ZTest Off Cull Off ZWrite Off Blend Off

         Blend SrcAlpha OneMinusSrcAlpha
			CGINCLUDE
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;

				float2 BlurCord1 : TEXCOORD1;
				float2 BlurCord2 : TEXCOORD2;
				float2 BlurCord3 : TEXCOORD3;
				float2 BlurCord4 : TEXCOORD4;
				float2 BlurCord5 : TEXCOORD5;

			};

			float _Blur;
			sampler2D _MainTex;
			sampler2D _UIMask;
			sampler2D _UIBlured;
			float4 _MainTex_TexelSize;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;

				float2 Offset = float2(_Blur,_Blur);

				float ux = v.uv.x + Offset;
				float uy = v.uv.y + Offset;

				float zux = v.uv.x - Offset;
				float zuy = v.uv.y - Offset;

				o.BlurCord1 = v.uv.xy;
				o.BlurCord2 = (float2(ux , uy));
				o.BlurCord3 = (float2(zux , zuy));
				o.BlurCord4 = (float2(ux , zuy));
				o.BlurCord5 = (float2(zux , uy));

				return o;
			}

			fixed4 fragBlur (v2f i) : SV_Target
			{
				fixed4 blur = fixed4(0,0,0,0);
				blur += tex2D(_MainTex, i.BlurCord1) * 0.204164;
				blur += tex2D(_MainTex, i.BlurCord2) * 0.304005;
				blur += tex2D(_MainTex, i.BlurCord3) * 0.304005;
				blur += tex2D(_MainTex, i.BlurCord4) * 0.093913;
				blur += tex2D(_MainTex, i.BlurCord5) * 0.093913;

				return blur;
			}

			fixed4 fragCombine (v2f i) : SV_Target
			{
				fixed2 fixAA = i.uv;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
                     fixAA.y = 1-fixAA.y;
				#endif

				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 uiblured = tex2D(_UIBlured, fixed2(i.uv.x,fixAA.y));
				fixed4 uimask = tex2D(_UIMask, fixed2(i.uv.x,fixAA.y));

				fixed4 mainOutput = col.rgba * (1.0 - (uimask.a * 1));
				fixed4 blendOutput = uiblured.rgba * uimask.a * 1;         

				return fixed4(mainOutput.r+blendOutput.r,mainOutput.g+blendOutput.g,mainOutput.b+blendOutput.b,mainOutput.a + blendOutput.a);
			}
			ENDCG

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragBlur
			
			#include "UnityCG.cginc"
			ENDCG
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragCombine
			
			#include "UnityCG.cginc"
			ENDCG
		}
	}
}
