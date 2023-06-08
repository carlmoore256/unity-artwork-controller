Shader "Custom/SpriteTestShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MaskTex ("Mask Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex   : SV_POSITION;
                float4 color    : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _PixelSnap;

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                UNITY_TRANSFER_FOG(OUT, OUT.vertex);
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap(OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                // fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                // c.rgb *= c.a;

                // c.rgb = c.rgb * (1 - c.a) + c.a * c.rgb;

                // fixed4 col = tex2D(_MainTex, IN.uv);
                // fixed4 mask = tex2D(_MaskTex, IN.uv);
                // col.a *= mask.a;

                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;

                // // Sample the mask texture
                // fixed4 mask = tex2D(_MaskTex, IN.texcoord);

                // // Multiply the mask alpha by the sprite alpha
                // mask.a *= c.a;

                // // Apply the mask to the sprite color
                // c *= mask;

                // // Premultiply the output color alpha
                // c.rgb *= c.a;


                return c;

                // return c;
            }
            ENDCG
        }
    }
}
