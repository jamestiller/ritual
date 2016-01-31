Shader "Hidden/Blur Behind/Blur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Parameter ("Parameter", Vector) = (1,1,1,1)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass // Blur
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			half4 _Parameter;

			fixed4 frag (v2f i) : SV_Target
			{
				half4 coords = i.uv.xyxy;
				half4 color = 0.324 * tex2D(_MainTex, coords.xy);
				coords += _Parameter;
				color += 0.232 * (tex2D(_MainTex, coords.xy) + tex2D(_MainTex, coords.zw));
				coords += _Parameter;
				color += 0.0855 * (tex2D(_MainTex, coords.xy) + tex2D(_MainTex, coords.zw));
				coords += _Parameter;
				color += 0.0205 * (tex2D(_MainTex, coords.xy) + tex2D(_MainTex, coords.zw));
				return color;
			}
			ENDCG
		}

		Pass // Downsample
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 uv0 : TEXCOORD0;
				half2 uv1 : TEXCOORD1;
				half2 uv2 : TEXCOORD2;
				half2 uv3 : TEXCOORD3;
			};

			uniform half4 _MainTex_TexelSize;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv0 = v.uv + _MainTex_TexelSize.xy * half2(1.0h, 1.0h);
				o.uv1 = v.uv + _MainTex_TexelSize.xy * half2(-1.0h, -1.0h);
				o.uv2 = v.uv + _MainTex_TexelSize.xy * half2(1.0h, -1.0h);
				o.uv3 = v.uv + _MainTex_TexelSize.xy * half2(-1.0h, 1.0h);
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				half4 color = tex2D(_MainTex, i.uv0);
				color += tex2D(_MainTex, i.uv1);
				color += tex2D(_MainTex, i.uv2);
				color += tex2D(_MainTex, i.uv3);
				return color * 0.25h;
			}
			ENDCG
		}

		Pass // Crop
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			half4 _Parameter;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
#if UNITY_UV_STARTS_AT_TOP
				o.uv.y = 1 - o.uv.y;
#endif
				o.uv = o.uv * _Parameter.zw + _Parameter.xy;
#if UNITY_UV_STARTS_AT_TOP
				o.uv.y = 1 - o.uv.y;
#endif
				return o;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}