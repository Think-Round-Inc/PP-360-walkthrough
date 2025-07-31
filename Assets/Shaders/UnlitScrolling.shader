// Unlit Scrolling Shader for Unity with Direction Control
Shader "Custom/UnlitScrollingDirection"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ScrollSpeed("Scroll Speed", Range(-10, 10)) = 1.0
        _ScrollDirection("Scroll Direction", Vector) = (1, 0, 0, 0)
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _ScrollSpeed;
                float4 _ScrollDirection;

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    // Calculate the offset based on the scroll direction and speed
                    float2 offset = _Time.y * _ScrollDirection.yx * _ScrollSpeed;
                    float2 newUV = i.uv + offset % 1;

                    // Sample the texture
                    fixed4 col = tex2D(_MainTex, newUV);

                    return col;
                }
                ENDCG
            }
        }
}

