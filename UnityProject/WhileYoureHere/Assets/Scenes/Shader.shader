Shader "Custom/FullscreenEdgeDetection_Fixed"
{
    Properties
    {
        _OutlineColor ("outline color", Color) = (0, 0, 0, 1)
        _Threshold ("threshold", Range(0.0, 1.0)) = 0.5
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
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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
                float4 _OutlineColor;
                float _Threshold;
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
                float3 bl = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2(-1,-1)).rgb;
                float3 bm = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2( 0,-1)).rgb;
                float3 br = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2( 1,-1)).rgb;

                float3 ml = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2(-1, 0)).rgb;
                float3 mm = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2( 0, 0)).rgb;
                float3 mr = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2( 1, 0)).rgb;

                float3 tl = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2(-1, 1)).rgb;
                float3 tm = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2( 0, 1)).rgb;
                float3 tr = Unity_Universal_SampleBuffer_BlitSource(screenUV + texel * float2( 1, 1)).rgb;

                // convert to luminance
                float l_bl = Luminance(bl);
                float l_bm = Luminance(bm);
                float l_br = Luminance(br);
                float l_ml = Luminance(ml);
                float l_mr = Luminance(mr);
                float l_tl = Luminance(tl);
                float l_tm = Luminance(tm);
                float l_tr = Luminance(tr);

                // Sobel kernels
                float gx = -l_bl - 2*l_ml - l_tl + l_br + 2*l_mr + l_tr;
                float gy = -l_bl - 2*l_bm - l_br + l_tl + 2*l_tm + l_tr;

                float edge = sqrt(gx*gx + gy*gy);
                
                if (edge > _Threshold) return _OutlineColor;
                return float4(mm, 1);
            }

            ENDHLSL
        }
    }
}
