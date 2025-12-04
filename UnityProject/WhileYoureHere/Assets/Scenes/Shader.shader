Shader "Custom/FullscreenEdgeDetect"
{
    Properties
    {
        _MainTex("Source", 2D) = "white"
        _Thickness("Edge Thickness", Float) = 1
        _Intensity("Edge Intensity", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }

        Pass
        {
            Name "EdgeDetect"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex FullscreenVert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_TexelSize;   // x=1/width, y=1/height
            float _Thickness;
            float _Intensity;

            //------------------------------------
            // Fullscreen triangle vertex shader
            //------------------------------------
            float4 FullscreenVert(uint id : SV_VertexID) : SV_POSITION
            {
                float2 uv = float2((id << 1) & 2, id & 2);
                return float4(uv * 2 - 1, 0, 1);
            }

            //------------------------------------
            // Edge detection fragment shader
            //------------------------------------
            float4 Frag(float4 position : SV_POSITION) : SV_Target
            {
                float2 uv = position.xy * 0.5 + 0.5;

                float2 texel = _MainTex_TexelSize.xy * _Thickness;

                // Sobel kernels:
                float3 Gx =
                    - SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texel.x,  texel.y)).rgb +
                      SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( texel.x,  texel.y)).rgb +
                    -2 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texel.x, 0)).rgb +
                     2 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( texel.x, 0)).rgb +
                    - SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texel.x, -texel.y)).rgb +
                      SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( texel.x, -texel.y)).rgb;

                float3 Gy =
                      SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texel.x,  texel.y)).rgb +
                     2 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0,          texel.y)).rgb +
                      SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( texel.x,  texel.y)).rgb -
                      SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(-texel.x, -texel.y)).rgb -
                     2 * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2(0,         -texel.y)).rgb -
                      SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv + float2( texel.x, -texel.y)).rgb;

                float3 edge = sqrt(Gx * Gx + Gy * Gy) * _Intensity;

                return float4(edge, 1);
            }

            ENDHLSL
        }
    }
}
