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

            float4 _BaseColor;

            // Vertex shader: transform vertex to clip space
            float4x4 unity_matrix_mvp;

            float4 vert(float4 v : POSITION) : SV_POSITION
            {
                return mul(unity_matrix_mvp, v);
            }
            
            float4 frag() : SV_Target
            {
                return _BaseColor;
            }

            ENDHLSL
        }
    }
}
