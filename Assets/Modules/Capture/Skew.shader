Shader "Skew" {
	Properties {
		_MainTex("Texture", 2D) = "white" {}
		tl("Top Left", vector) = (0, 0, 0, 0)
		tr("Top Right", vector) = (1, 0, 0, 0)
		bl("Bottom Left", vector) = (0, 1, 0, 0)
		br("Bottom Right", vector) = (1, 1, 0, 0)
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag

			#include "UnityCG.cginc"

			struct VertexData {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct FragmentData {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			FragmentData Vert(VertexData v) {
				FragmentData o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			float4 tl, tr, bl, br;

			fixed4 Frag(FragmentData i) : SV_Target {
				float x = i.uv.x, y = i.uv.y;
				float2 t = lerp(tl, tr, x), b = lerp(bl, br, x);
				float2 uv = lerp(t, b, y);
				fixed4 col = tex2D(_MainTex, uv);
				return col;
			}
			ENDCG
		}
	}
}
