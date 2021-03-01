Shader "Custom/SpriteOverlay" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Colour("Silhouette Colour", Color) = (0,0,0,0)
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }

		CGPROGRAM
#pragma surface surf Lambert

		sampler2D _MainTex;

	struct Input
	{
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		half4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG

		Pass{
		Name "SILHOUETTE"
		ZTest Greater
		Color[_Colour]
	}
	}
		FallBack "Diffuse"
}