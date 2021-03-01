Shader "Interface/PaintPoint"
{
	Properties
	{

	}

	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float3 position : WORLD_POSITION;
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			uniform float3 _Point;
			
			v2f vert (appdata v)
			{
				v2f o;

				o.position = v.vertex;
				o.vertex = float4(v.uv * 2 - 1, 0, v.vertex.w);
				o.uv = v.uv;

				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{
				float d = distance(i.position, _Point);

				if (d < 0.5)
					return fixed4(1, 0, 0, 1);

				return fixed4(0, 0, 0, 0);
			}
			ENDCG
		}
	}
}
