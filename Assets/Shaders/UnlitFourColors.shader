Shader "Custom/UnlitFourColors"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ColorTopLeft("Color Top Left", Color) = (1, 1, 1, 1)
        _ColorTopRight("Color Top Right", Color) = (1, 1, 1, 1)
        _ColorBottomLeft("Color Bottom Left", Color) = (1, 1, 1, 1)
        _ColorBottomRight("Color Bottom Right", Color) = (1, 1, 1, 1)
        _RotationAngle("Rotation Angle", Range(0, 360)) = 0
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile_instancing
                #include "UnityCG.cginc"

                struct appdata
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
                float4 _MainTex_ST;
                float4 _ColorTopLeft;
                float4 _ColorTopRight;
                float4 _ColorBottomLeft;
                float4 _ColorBottomRight;
                float _RotationAngle;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv = i.uv;
                    float angle = _RotationAngle * (3.14159 / 180.0);
                    float2 center = float2(0.5, 0.5);
                    float2 rotatedUV = float2(
                        (uv.x - center.x) * cos(angle) - (uv.y - center.y) * sin(angle),
                        (uv.x - center.x) * sin(angle) + (uv.y - center.y) * cos(angle)
                        ) + center;

                    // Calculate the UV coordinates for each corner
                    float2 uvTopLeft = float2(rotatedUV.x * 0.5, rotatedUV.y * 0.5);
                    float2 uvTopRight = float2(rotatedUV.x * 0.5 + 0.5, rotatedUV.y * 0.5);
                    float2 uvBottomLeft = float2(rotatedUV.x * 0.5, rotatedUV.y * 0.5 + 0.5);
                    float2 uvBottomRight = float2(rotatedUV.x * 0.5 + 0.5, rotatedUV.y * 0.5 + 0.5);

                    // Sample the colors for each corner
                    fixed4 colorTopLeft = _ColorTopLeft;
                    fixed4 colorTopRight = _ColorTopRight;
                    fixed4 colorBottomLeft = _ColorBottomLeft;
                    fixed4 colorBottomRight = _ColorBottomRight;

                    fixed4 texColor = tex2D(_MainTex, rotatedUV);

                    // Discard pixels that are black in the main texture
                    if (texColor.r == 0 && texColor.g == 0 && texColor.b == 0)
                        discard;

                    // Determine which corner the UV belongs to
                    bool topLeft = rotatedUV.x < 0.5 && rotatedUV.y < 0.5;
                    bool topRight = rotatedUV.x >= 0.5 && rotatedUV.y < 0.5;
                    bool bottomLeft = rotatedUV.x < 0.5 && rotatedUV.y >= 0.5;
                    bool bottomRight = rotatedUV.x >= 0.5 && rotatedUV.y >= 0.5;

                    // Apply the color for the corresponding corner
                    fixed4 resultColor = texColor;
                    if (topLeft)
                        resultColor *= colorTopLeft;
                    else if (topRight)
                        resultColor *= colorTopRight;
                    else if (bottomLeft)
                        resultColor *= colorBottomLeft;
                    else if (bottomRight)
                        resultColor *= colorBottomRight;
                  
                    return resultColor;
                }
                ENDCG
            }
        }
}
