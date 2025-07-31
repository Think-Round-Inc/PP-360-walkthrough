Shader "Custom/Masked"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _MaskTex("Mask Texture", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma multi_compile
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv_MainTex : TEXCOORD0;
                    float2 uv_MaskTex : TEXCOORD1;
                };

                struct v2f
                {
                    float2 uv_MainTex : TEXCOORD0;
                    float2 uv_MaskTex : TEXCOORD1;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                sampler2D _MaskTex;
                float4 _MainTex_ST;
                float4 _Color;


                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv_MainTex =  TRANSFORM_TEX(v.uv_MainTex, _MainTex);
                    o.uv_MaskTex = v.uv_MaskTex;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 mainColor = tex2D(_MainTex, i.uv_MainTex);
                    fixed4 maskColor = tex2D(_MaskTex, i.uv_MaskTex);

                    if (maskColor.r == 0.0)
                        discard;

                    fixed4 finalColor = mainColor * _Color;

                    return finalColor;
                }
                ENDCG
            }
        }
}
