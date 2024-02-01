// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/TerrainAngleColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorSteep ("Steep Color", Color) = (1,0,0,1)
        _ColorFlat ("Flat Color", Color) = (0,1,0,1)
        _PerlinScale ("Perlin Scale", Range(0,5)) = 1
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColorSteep;
            float4 _ColorFlat;
            float _PerlinScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = TRANSFORM_TEX(v.vertex.xy, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float angle = dot(i.worldNormal, float3(0,1,0));
                
                // Lerp between colors based on angle
                fixed4 col = lerp(_ColorSteep, _ColorFlat, angle);

                // Apply Perlin noise alteration
                float p = tex2Dlod(_MainTex, float4(i.worldPos.xy * _PerlinScale,0,0)).r;
                col.rgb *= p;

                return col;
            }
            ENDCG
        }
    }
}
