Shader "Custom/UnlitBasicBlendShow" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _FillX("Fill for X", Range(0, 1)) = 1
        _FillY("Fill for Y", Range(0, 1)) = 1
        _RotationAngle("Rotation Angle", Range(0, 360)) = 0
    }

        SubShader{
            Tags { "RenderType" = "Opaque" }

            Pass {
                ZWrite On
                Blend SrcAlpha OneMinusSrcAlpha
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _Color;
                float _FillX;
                float _FillY;
                float _RotationAngle;

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    float2 uv = i.uv;
                    float2 fill = float2(_FillX, _FillY);

                    if (uv.x > fill.x || uv.y > fill.y)
                        discard;

                    // Calculate the rotation
                    float angle = _RotationAngle * (3.14159 / 180.0);
                    float2 center = float2(0.5, 0.5);
                    float2 rotatedUV = float2(
                        (uv.x - center.x) * cos(angle) - (uv.y - center.y) * sin(angle),
                        (uv.x - center.x) * sin(angle) + (uv.y - center.y) * cos(angle)
                    ) + center;

                    fixed4 texColor = tex2D(_MainTex, rotatedUV);
                    fixed4 resultColor = texColor * _Color;
                    resultColor.a *= texColor.a;

                    return resultColor;
                }
                ENDCG
            }
        }

            FallBack "Diffuse"
}
