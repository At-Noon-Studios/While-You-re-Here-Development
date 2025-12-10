Shader "Custom/FullscreenEdgeDetection_Fixed"
{
    Properties
    {
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalRenderPipeline" 
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }
        Cull Off
        ZWrite Off
        ZTest Always

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_X(_BlitTexture);

            float4 Unity_Universal_SampleBuffer_BlitSource(float2 uv)
            {
                uint2 pixelCoords = uint2(uv * _ScreenSize.xy);
                return LOAD_TEXTURE2D_X_LOD(_BlitTexture, pixelCoords, 0);
            }

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float _Alpha;
            CBUFFER_END

            Varyings vert(uint vertexID : SV_VertexID)
            {
                Varyings output;
                
                float4 positions[3] = 
                {
                    float4(-1.0, -1.0, 0.0, 1.0),
                    float4(-1.0,  3.0, 0.0, 1.0),
                    float4( 3.0, -1.0, 0.0, 1.0)
                };

                output.positionHCS = positions[vertexID];

                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                float2 screenUV = input.positionHCS.xy / _ScreenParams.xy;
     
                float2 texel = 1 / _ScreenParams.xy;

                // sample neighboring pixels
                float3 c00 = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2(-1,-1)).rgb;
                float3 c10 = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2( 0,-1)).rgb;
                float3 c20 = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2( 1,-1)).rgb;

                float3 c01 = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2(-1, 0)).rgb;
                float3 c11 = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2( 0, 0)).rgb;
                float3 c21 = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2( 1, 0)).rgb;

                float3 c02 = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2(-1, 1)).rgb;
                float3 c12 = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2( 0, 1)).rgb;
                float3 c22 = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2( 1, 1)).rgb;

                // convert to luminance
                float l00 = dot(c00, float3(0.299, 0.587, 0.114));
                float l10 = dot(c10, float3(0.299, 0.587, 0.114));
                float l20 = dot(c20, float3(0.299, 0.587, 0.114));
                float l01 = dot(c01, float3(0.299, 0.587, 0.114));
                float l11 = dot(c11, float3(0.299, 0.587, 0.114));
                float l21 = dot(c21, float3(0.299, 0.587, 0.114));
                float l02 = dot(c02, float3(0.299, 0.587, 0.114));
                float l12 = dot(c12, float3(0.299, 0.587, 0.114));
                float l22 = dot(c22, float3(0.299, 0.587, 0.114));

                // Sobel kernels
                float gx = -l00 - 2*l01 - l02 + l20 + 2*l21 + l22;
                float gy = -l00 - 2*l10 - l20 + l02 + 2*l12 + l22;

                float edge = sqrt(gx*gx + gy*gy);

                return float4(edge.xxx, 1);
            }

            ENDHLSL
        }
    }
}
