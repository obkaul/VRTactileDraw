Shader "Interface/Intensity" {

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

		static const int MOTOR_COUNT = 100;
		static const fixed3 white = fixed3(1, 1, 1);
		static const fixed3 green = fixed3(0, 1, 0);
		static const fixed3 yellow = fixed3(1, 1, 0);
		static const fixed3 red = fixed3(1, 0, 0);

		uniform float3 _Positions[MOTOR_COUNT];
		uniform float _Intensities[MOTOR_COUNT];

		void surf(Input IN, inout SurfaceOutput o) {
			fixed intensity = 0;

			for (int i = 0; i < MOTOR_COUNT; i++) {
				fixed d = distance(IN.worldPos, _Positions[i]);
				intensity += _Intensities[i] / (0.1 + 30 * d + 100 * d * d);
			}

			if (intensity < 0.333) {
				o.Albedo = lerp(white, green, intensity / 0.333);
			}
			else if (intensity >= 0.333 && intensity < 0.666) {
				o.Albedo = lerp(green, yellow, (intensity - 0.333) / 0.333);
			}
			else if (intensity >= 0.666) {
				o.Albedo = lerp(yellow, red, (intensity - 0.666) / 0.333);
			}
		}
		ENDCG
	}
	FallBack Off
}
