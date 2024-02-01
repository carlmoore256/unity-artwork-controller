Shader "Custom/StandardDoubleSided"
{
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        [Toggle] _DoubleSided ("Double Sided", Float) = 0
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 200
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma multi_compile _ _DOUBLE_SIDED

        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        sampler2D _MainTex;
        sampler2D _BumpMap;
        half _Metallic;
        half _Glossiness;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        }

        ENDCG
    }
    FallBack "Standard"
}
