Shader "Custom/DoubleSided" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            Cull Off // Disables backface culling

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _BumpMap;
            float4 _MainTex_ST;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                float3 normal = UnpackNormal(tex2D(_BumpMap, i.uv));
                float3 lightDir = normalize(UnityWorldSpaceLightDir(i.vertex));
                float diff = max(0.0, dot(normal, lightDir));
                col *= diff;
                return col;
            }
            ENDCG
        }
    }
}
