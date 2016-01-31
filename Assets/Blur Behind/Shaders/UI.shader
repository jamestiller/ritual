Shader "Blur Behind/UI" {
	Properties{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15
	}

	SubShader {
		Tags {
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil {
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass {
			CGPROGRAM
			#pragma multi_compile _ BLUR_BEHIND_SET
			#pragma vertex vert
			#pragma fragment frag

			#ifdef BLUR_BEHIND_SET
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			struct appdata_t {
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord  : TEXCOORD0;
				half2 viewPos	: TEXCOORD1;
				float4 worldPosition : TEXCOORD2;
			};

			fixed4 _Color;
			float4 _BlurBehindRect;

			bool _UseClipRect;
			float4 _ClipRect;

			bool _UseAlphaClip;

			v2f vert(appdata_t IN) {
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1, 1);
				#endif
				OUT.viewPos = (OUT.vertex.xy / OUT.vertex.w + 1.0) * 0.5;
				OUT.viewPos = (OUT.viewPos - _BlurBehindRect.xy) / _BlurBehindRect.zw;
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _BlurBehindTex;

			fixed4 frag(v2f IN) : SV_Target {
				half4 color = tex2D(_MainTex, IN.texcoord);
				color.a *= IN.color.a;

				if (_UseClipRect) {
					color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				}
				if (_UseAlphaClip) {
					clip(color.a - 0.001);
				}

				color.rgb = tex2D(_BlurBehindTex, IN.viewPos).rgb;
				return color;
			}

			#else // If no Blur Behind component is active
			float4 vert() : SV_POSITION {
				return 0;
			}

			fixed4 frag() : SV_Target {
				discard;
				return 0;
			}
			#endif

			ENDCG
		}
	}
}
