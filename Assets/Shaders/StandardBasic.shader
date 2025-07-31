Shader "Custom/StandardBasic" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _DiscardBlack("Discard Black Pixels", Range(0, 1)) = 0
    }

        SubShader{
            Tags { "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma surface surf Lambert

            struct Input {
                float2 uv_MainTex;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _DiscardBlack;

            void surf(Input IN, inout SurfaceOutput o) {
                fixed4 texColor = tex2D(_MainTex, IN.uv_MainTex);

                if (_DiscardBlack > 0 && texColor.r == 0 && texColor.g == 0 && texColor.b == 0) {
                    discard;
                }

                o.Albedo = texColor.rgb * _Color.rgb;
                o.Alpha = texColor.a;
            }
            ENDCG
        }
}
