Shader "Custom/Wavy" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed", Range(0, 10)) = 1
        _Amplitude ("Amplitude", Range(0, 1)) = 0.1
    }
 
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Opaque"}
        LOD 100
 
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
 
            // Texture and properties
            uniform sampler2D _MainTex;
            uniform float _Speed;
            uniform float _Amplitude;
 
            // Vertex data
            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
 
            // Output data for fragment shader
            struct v2f {
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : NORMAL;
                float4 vertex : SV_POSITION;
            };
 
            // Vertex shader
            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // o.worldNormal = mul(unity_ObjectToWorld, v.normal).xyz;
                return o;
            }
 
            // Fragment shader
            fixed4 frag (v2f i) : SV_Target {
                // Calculate wave displacement
                float3 offset = _Amplitude * sin(i.worldPos.y * _Speed + _Time.y);
 
                // Apply displacement to position
                float4 worldPos = float4(i.worldPos + offset, 1);
 
                // Sample texture
                fixed4 col = tex2D(_MainTex, i.uv);
 
                // Return final color
                return col;
            }
 
            ENDCG
        }
    }
 
    FallBack "Diffuse"
}
