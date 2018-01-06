Shader "BB/VoxelCreator/VoxelSurface" {
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard vertex:vert fullforwardshadows
		#pragma target 3.0

		struct Input {
			fixed4 vertexColor;
		};

    struct v2f {
      float4 pos : SV_POSITION;
      fixed4 color : COLOR;
    };

    void vert(inout appdata_full v, out Input o) {
      UNITY_INITIALIZE_OUTPUT(Input, o);
      o.vertexColor = v.color;
    }

		void surf (Input input, inout SurfaceOutputStandard o) {
			fixed4 c = input.vertexColor;
			o.Albedo = c.rgb;
			o.Metallic = 0;
			o.Smoothness = 0;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
