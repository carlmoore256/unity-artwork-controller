Shader "Custom/TestDistort" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Start ("Start", Vector) = (0,0,0,0)
        _End ("End", Vector) = (1,1,1,1)
        _Distortion ("Distortion", Range(-1, 1)) = 0
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass {
            CGPROGRAM
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
            float4 _MainTex_ST;
            float4 _Color;
            float4 _Start;
            float4 _End;
            float _Distortion;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // o.uv += float2(sin(o.uv.y * 10.0) * _Distortion, 0);
                o.uv += float2(sin(o.uv.y * 10.0 + _Time.y * 10.0) * 0.1, 0);

                o.uv.x += float2(sin(o.uv.y * 10.0 + _Time.y * 10.0) * 0.1, 0);
                // Debug.DrawLine(v.vertex, v.vertex + float4(sin(o.uv.y * 50.0 + _Time.y * 50.0) * 0.1, 0, 0, 0), Color.red);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float4 start = _Start * _Color;
                float4 end = _End * _Color;
                float4 gradient = lerp(start, end, i.uv.x);
                gradient.a = tex2D(_MainTex, i.uv).a;
                return gradient;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
