Shader "Custom/Kaleidoscope"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Reflections ("Reflections", Range(1, 20)) = 4
        _Stretch ("Stretch", Range(0, 10)) = 1.0
        _Intensity ("Intensity", Range(0, 1)) = 1.0
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
            float _Reflections;
            float _Stretch;
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // map uv to [-1,1] range
                float2 uv = i.uv * 2 - 1;

                // calculate angle and radius
                float angle = atan2(uv.y, uv.x);
                float radius = length(uv) * _Stretch;

                // rotate and mirror uv
                angle = fmod(angle, 3.14159 / _Reflections); // adjust this to control the number of reflections
                float2 mirroredUv = float2(cos(angle), sin(angle)) * radius;

                // map mirroredUv back to [0,1] range and sample texture
                mirroredUv = mirroredUv * 0.5 + 0.5;
                // fixed4 col = tex2D(_MainTex, mirroredUv);

                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 effectCol = tex2D(_MainTex, mirroredUv);
                fixed4 finalCol = lerp(col, effectCol, _Intensity);

                return finalCol;
            }
            ENDCG
        }
    }
}
