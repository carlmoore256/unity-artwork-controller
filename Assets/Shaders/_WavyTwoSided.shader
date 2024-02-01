Shader "Custom/MirrorZAxisSurface"
{
    Properties {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Amplitude ("Amplitude", Float) = 0.1
        _Frequency ("Frequency", Float) = 0.05
        _PhaseFactor ("Phase Factor", Float) = 1.0
        _Scale ("Scale", Float) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows addshadow vertex:vert

        struct Input {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        sampler2D _MainTex;
        sampler2D _BumpMap;
        half _Metallic;
        half _Glossiness;

        float _Amplitude;
        float _Frequency;
        float _PhaseFactor;
        float


        void vert(inout appdata_full v) {
            float phase = dot(v.vertex.xyz, float3(_PhaseFactor, _PhaseFactor, _PhaseFactor));
            v.vertex.y += _Amplitude * sin(_Frequency * _Time.y + phase);
            // v.vertex.z += _Amplitude * cos(_Frequency * _Time.y + phase);
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
        }
        ENDCG
    }
    Fallback "Standard"
}
