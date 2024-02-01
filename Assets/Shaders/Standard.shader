Shader "Custom/Standard"
{
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        sampler2D _MainTex;
        sampler2D _BumpMap;
        half _Metallic;
        half _Glossiness;

        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            // Normal map
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
    }
    FallBack "Standard"
}
