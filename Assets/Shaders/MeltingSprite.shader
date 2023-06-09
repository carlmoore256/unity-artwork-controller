Shader "Custom/MeltingSprite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _MeltingAmount ("Melting Amount", Range(0,100)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex VectorVert
            #pragma fragment SpriteFrag
            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            sampler2D _NoiseTex;
            float _MeltingAmount;

            v2f VectorVert(appdata_t IN)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                
                // Add the 'melting' effect
                float noise = tex2Dlod(_NoiseTex, float4(IN.texcoord,0,0)).r;
                OUT.vertex.y += noise * _MeltingAmount;

                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.texcoord = IN.texcoord;

                #ifdef UNITY_COLORSPACE_GAMMA
                fixed4 color = IN.color;
                #else
                fixed4 color = fixed4(GammaToLinearSpace(IN.color.rgb), IN.color.a);
                #endif

                OUT.color = color * _Color * _RendererColor;

                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }
        ENDCG
        }
    }
}
