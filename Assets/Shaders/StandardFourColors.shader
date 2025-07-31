Shader "Custom/StandardFourColors" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _ColorTopLeft("Color Top Left", Color) = (1, 1, 1, 1)
        _ColorTopRight("Color Top Right", Color) = (1, 1, 1, 1)
        _ColorBottomLeft("Color Bottom Left", Color) = (1, 1, 1, 1)
        _ColorBottomRight("Color Bottom Right", Color) = (1, 1, 1, 1)
        _RotationAngle("Rotation Angle", Range(0, 360)) = 0
        _Brightness("Brightness", Range(0.0, 10.0)) = 1.0
    }

        SubShader{
            Tags { "RenderType" = "Opaque" }

            CGPROGRAM
            #pragma surface surf Lambert

            sampler2D _MainTex;
            float4 _ColorTopLeft;
            float4 _ColorTopRight;
            float4 _ColorBottomLeft;
            float4 _ColorBottomRight;
            float _RotationAngle;
            sampler2D _Lightmap;
            float _Brightness;

            struct Input {
                float2 uv_MainTex;
                float2 uv2_Lightmap;
            };

            void surf(Input IN, inout SurfaceOutput o) {
                float2 uv = IN.uv_MainTex;
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

                // Sample the lightmap and combine it with the surface color
                fixed4 lightmapColor = tex2D(_Lightmap, IN.uv2_Lightmap);
                resultColor *= lightmapColor;

                o.Albedo = resultColor.rgb * _Brightness;
                o.Alpha = resultColor.a;
            }
            ENDCG
        }
}
