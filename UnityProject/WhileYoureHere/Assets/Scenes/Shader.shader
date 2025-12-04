Shader "Custom/Green"
{
    Properties
    {
        _BaseColor ("base color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Vertex shader: transform vertex to clip space
            float4x4 unity_matrix_mvp;

            float4 vert(float4 v : POSITION) : SV_POSITION
            {
                return mul(unity_matrix_mvp, v);
            }
            
            float4 frag() : SV_Target
            {
                return float4(0, 1, 0, 1);
            }

            ENDHLSL
        }
    }
}
