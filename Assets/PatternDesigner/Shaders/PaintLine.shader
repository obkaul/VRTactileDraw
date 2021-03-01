Shader "Interface/PaintLine"
{
	Properties {

	}

	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		struct Input {
			float3 worldPos;
		};

		static const int MAX_KEYS = 1000;

		static const float WIDTH = 0.03;

		uniform float3 _Positions[MAX_KEYS];

		float3 perpendicular(float3 a, float3 d, float3 p) {
			return a + dot(p - a, d) * d - p;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			float3 origin = float3(0, 0, 0);

			for (int i = 1; i < MAX_KEYS; i++) {
				float3 p0 = _Positions[i - 1];
				float3 p1 = _Positions[i];

				if (distance(p1, origin) < 0.001)
					break;

				float3 forward = normalize(p1 - p0);
				float3 normal = normalize(perpendicular(p0, forward, origin));
				//float3 right = Vector3.Cross(forward, normal).normalized;

				float3 w = IN.worldPos;

				if (dot(normal, w - origin) <= 0)
					continue;

				float3 projW = w - dot(w - p0, normal) * normal;

				if (distance(projW, p0) < WIDTH) {
					o.Albedo = fixed3(1, 0, 0);
					return;
				}

				if (distance(projW, p1) < WIDTH) {
					o.Albedo = fixed3(1, 0, 0);
					return;
				}

				float d0 = dot(projW - p0, forward);

				if (d0 > 0 && d0 <= length(p1 - p0))
				{
					float d1 = length(perpendicular(p0, forward, projW));

					if (d1 < WIDTH) {
						o.Albedo = fixed3(1, 0, 0);

						return;
					}
				}
			}

			o.Albedo = fixed3(1, 1, 1);
		}
		ENDCG
	}
	FallBack Off
}
