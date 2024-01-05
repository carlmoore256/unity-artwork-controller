Shader "Custom/PointCloud"
{
    Properties
    {
        _PointSize("Point Size", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            float _PointSize;

            v2f vert (appdata v)
            {
                v2f o;
                float4 worldPosition = mul(unity_ObjectToWorld, v.vertex);
                float4 viewPosition = mul(UNITY_MATRIX_V, worldPosition);
                o.vertex = mul(UNITY_MATRIX_P, viewPosition);

                // Calculate point size based on distance
                float dist = length(viewPosition.xyz);
                float pointSize = _PointSize / dist; // Adjust _PointSize as needed

                // Apply point size (this is a very basic way, might need adjustment)
                o.vertex.xy += pointSize * o.vertex.w;

                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Simple color output
                return i.color;
            }
            ENDCG
        }
    }
}
