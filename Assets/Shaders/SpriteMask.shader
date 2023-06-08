Shader "Custom/SpriteMask" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
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
            sampler2D _MaskTex;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // change it a bit
                o.uv.x = o.uv.x * sin(_Time.y * 10);
                o.uv.y = o.uv.y * cos(_Time.y * 10);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // fixed4 col = tex2D(_MainTex, i.uv);
                // fixed4 mask = tex2D(_MaskTex, i.uv);
                // col.a *= mask.a;
                // return col;
                float2 uv = i.uv;
                fixed4 color = fixed4(uv.x, uv.y, 0, 1);

                // Return the computed color
                return color;
            }
            ENDCG    

        }
    } // Add this closing brace
}
