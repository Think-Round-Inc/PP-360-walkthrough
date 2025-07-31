Shader "Custom/MultiMaskShader" {
    Properties{
        _MainTex("Main Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _Mask1("Mask 1", 2D) = "white" {}
        _Mask2("Mask 2", 2D) = "white" {}
        _Mask3("Mask 3", 2D) = "white" {}
        _Mask4("Mask 4", 2D) = "white" {}
        _Mask5("Mask 5", 2D) = "white" {}
        _Mask6("Mask 6", 2D) = "white" {}
        _Mask7("Mask 7", 2D) = "white" {}
        _EnableMask1("Enable Mask 1", Range(0, 1)) = 1
        _EnableMask2("Enable Mask 2", Range(0, 1)) = 1
        _EnableMask3("Enable Mask 3", Range(0, 1)) = 1
        _EnableMask4("Enable Mask 4", Range(0, 1)) = 1
        _EnableMask5("Enable Mask 5", Range(0, 1)) = 1
        _EnableMask6("Enable Mask 6", Range(0, 1)) = 1
        _EnableMask7("Enable Mask 7", Range(0, 1)) = 1
    }

        SubShader{
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 100
            Cull Off

            Blend SrcAlpha OneMinusSrcAlpha

            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                sampler2D _Mask1;
                sampler2D _Mask2;
                sampler2D _Mask3;
                sampler2D _Mask4;
                sampler2D _Mask5;
                sampler2D _Mask6;
                sampler2D _Mask7;
                float4 _Color;
                float4 _MainTex_ST;
                float _EnableMask1;
                float _EnableMask2;
                float _EnableMask3;
                float _EnableMask4;
                float _EnableMask5;
                float _EnableMask6;
                float _EnableMask7;

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
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target {
                    fixed4 mainTex = tex2D(_MainTex, i.uv);
                    fixed4 mask1 = tex2D(_Mask1, i.uv);
                    fixed4 mask2 = tex2D(_Mask2, i.uv);
                    fixed4 mask3 = tex2D(_Mask3, i.uv);
                    fixed4 mask4 = tex2D(_Mask4, i.uv);
                    fixed4 mask5 = tex2D(_Mask5, i.uv);
                    fixed4 mask6 = tex2D(_Mask6, i.uv);
                    fixed4 mask7 = tex2D(_Mask7, i.uv);

                    // Apply the enabled masks
                    if (_EnableMask1 >= 0.5)
                        if (mask1.r < .1)
                            mainTex.a = 0;

                    if (_EnableMask2 >= 0.5)
                        if (mask2.r < .01)
                            discard;

                    if (_EnableMask3 > 0)
                        mainTex *= mask3;
                    if (_EnableMask4 > 0)
                        mainTex *= mask4;
                    if (_EnableMask5 > 0)
                        mainTex *= mask5;
                    if (_EnableMask6 > 0)
                        mainTex *= mask6;
                    if (_EnableMask7 > 0)
                        mainTex *= mask7;
                    

                    return mainTex * _Color;
                }
                ENDCG
            }
        }
            FallBack "Transparent/VertexLit"
}
