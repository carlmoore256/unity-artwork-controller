Shader "Custom/EchoEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PrevTex ("Previous Texture", 2D) = "white" {}
        _EchoIntensity ("Echo Intensity", Range(0,1)) = 0.5
        _Intensity ("Intensity", Range(0,1)) = 0.5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

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
            sampler2D _PrevTex;
            float _EchoIntensity;
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // o.uv.y = 1 - v.uv.y;  // Flip the Y coordinate
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 flippedUv = i.uv;
                flippedUv.y = 1 - flippedUv.y;  // Flip the y coordinate
                // i.uv.y = 1 - i.uv.y;  // Flip the y coordinate
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 prevCol = tex2D(_PrevTex, flippedUv);
                fixed4 echoCol = col + prevCol * _EchoIntensity;
                // fixed4 echoCol = lerp(col, prevCol, _EchoIntensity);
                fixed4 finalCol = lerp(col, echoCol, _Intensity);
                return finalCol;

                // fixed4 col = tex2D(_MainTex, i.uv);
                // fixed4 prevCol = tex2D(_PrevTex, i.uv);

                // col.rgb = col.rgb + prevCol.rgb * _EchoIntensity;
                // // col.rgb = lerp(col.rgb, prevCol.rgb, _EchoIntensity);


                // return col;
            }
            ENDCG
        }
    }
}
