Shader "Custom/FlowVelocity" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _VelocityTex ("Velocity Texture", 2D) = "white" {}
        _FlowStrength ("Flow Strength", Float) = 1.0
        _Displacement ("Displacement", Float) = 0.02
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _VelocityTex;
            float _FlowStrength;
            float _Displacement;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Hardcoded texel size for a known texture size, e.g., 256x256
                float2 texelSize = float2(1.0 / 256.0, 1.0 / 256.0);
            
                float2 smoothedVelocity = float2(0.0, 0.0);
                float totalWeight = 0.0;
            
                // Manually apply offsets to sample surrounding points
                for (int x = -1; x <= 1; x++) {
                    for (int y = -1; y <= 1; y++) {
                        // Compute texture coordinate offset for smoothing
                        float2 offset = float2(x, y) * texelSize;
                        float4 texel = tex2D(_VelocityTex, i.uv + offset);
                        // float2 velocity = (texel.rg - 0.5) * 2.0;
                        float velocity = ((texel.b - 0.5) * 2.0) * _Displacement;
                        float weight = 1.0; // Simple uniform weight, could be adjusted based on distance
                        smoothedVelocity += velocity * weight;
                        totalWeight += weight;
                    }
                }
            
                smoothedVelocity /= totalWeight;
            
                // Use the smoothed velocity for further calculations
                float2 flowUV = i.uv + smoothedVelocity * _FlowStrength * _Time.y;
            
                // Sample the main texture with adjusted UVs
                fixed4 col = tex2D(_MainTex, flowUV);
                return col;
            }
            
            
            ENDCG
        }
    }
}
