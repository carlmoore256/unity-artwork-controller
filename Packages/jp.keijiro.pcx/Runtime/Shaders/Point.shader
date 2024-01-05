// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

Shader "Point Cloud/Point"
{
    Properties
    {
        _Tint("Tint", Color) = (0.5, 0.5, 0.5, 1)
        _PointSize("Point Size", Float) = 0.05
        _Wavyness("Wavyness", Float) = 0.0
        _TimeFactor("Time Factor", Float) = 1.0
        _Amplitude("Amplitude", Float) = 0.1
        _PhaseOffset("Phase Offset", Vector) = (0, 0, 0, 0)
        _XYZSpeed("XYZ Speed", Vector) = (1, 1, 1, 1)
        [Toggle] _Distance("Apply Distance", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM

            #pragma vertex Vertex
            #pragma fragment Fragment

            #pragma multi_compile_fog
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #pragma multi_compile _ _DISTANCE_ON
            #pragma multi_compile _ _COMPUTE_BUFFER

            #include "UnityCG.cginc"
            #include "Common.cginc"

            struct Attributes
            {
                float4 position : POSITION;
                half3 color : COLOR;
            };

            struct Varyings
            {
                float4 position : SV_Position;
                half3 color : COLOR;
                half psize : PSIZE;
                UNITY_FOG_COORDS(0)
            };

            half4 _Tint;
            float4x4 _Transform;
            half _PointSize;
            half _Wavyness;
            half _TimeFactor;
            half _Amplitude;
            float4 _PhaseOffset;
            float4 _XYZSpeed;

        #if _COMPUTE_BUFFER
            StructuredBuffer<float4> _PointBuffer;
        #endif

        #if _COMPUTE_BUFFER
            Varyings Vertex(uint vid : SV_VertexID)
        #else
            Varyings Vertex(Attributes input)
        #endif
            {
            #if _COMPUTE_BUFFER
                float4 pt = _PointBuffer[vid];
                float4 pos = mul(_Transform, float4(pt.xyz, 1));
                half3 col = PcxDecodeColor(asuint(pt.w));
            #else
                float4 pos = input.position;
                half3 col = input.color;
            #endif

            #ifdef UNITY_COLORSPACE_GAMMA
                col *= _Tint.rgb * 2;
            #else
                col *= LinearToGammaSpace(_Tint.rgb) * 2;
                col = GammaToLinearSpace(col);
            #endif

                pos.x += cos(pos.y * _Wavyness + (_Time.x * _TimeFactor * _XYZSpeed.x) + _PhaseOffset.x) * (_Amplitude * 0.01);
                pos.y += sin(pos.x * _Wavyness + (_Time.y * _TimeFactor * _XYZSpeed.y) + _PhaseOffset.y) * (_Amplitude * 0.01);
                pos.z += cos(pos.z * _Wavyness + (_Time.z * _TimeFactor * _XYZSpeed.z) + _PhaseOffset.z) * (_Amplitude * 0.01);

                Varyings o;
                o.position = UnityObjectToClipPos(pos);
                o.color = col;
            #ifdef _DISTANCE_ON
                o.psize = _PointSize / o.position.w * _ScreenParams.y;
            #else
                o.psize = _PointSize;
            #endif
                UNITY_TRANSFER_FOG(o, o.position);
                return o;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                half4 c = half4(input.color, _Tint.a);
                UNITY_APPLY_FOG(input.fogCoord, c);
                return c;
            }

            ENDCG
        }
    }
    CustomEditor "Pcx.PointMaterialInspector"
}
