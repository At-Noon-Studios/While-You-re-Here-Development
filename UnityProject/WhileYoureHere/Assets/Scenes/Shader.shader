Shader "Custom/FullscreenEdgeDetection"
{
    Properties
    {
        _ColorOutlineColor ("color outline color", Color) = (0, 0, 0, 1)
        _DepthOutlineColor ("depth outline color", Color) = (0, 0, 0, 1)
        _NormalOutlineColor ("normal outline color", Color) = (0, 0, 0, 1)
        _ColorThreshold ("color threshold", Range(0.0, 1.0)) = 0.5
        _DepthThreshold ("depth threshold", Range(0.0, 10.0)) = 0.5
        _NormalThreshold ("normal threshold", Range(0.0, 10.0)) = 0.5
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl"

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
                float4 _ColorOutlineColor;
                float4 _DepthOutlineColor;
                float4 _NormalOutlineColor;
                float _ColorThreshold;
                float _DepthThreshold;
                float _NormalThreshold;
            CBUFFER_END

            static const float4 positions[3] = 
            {
                float4(-1.0, -1.0, 0.0, 1.0),
                float4(-1.0,  3.0, 0.0, 1.0),
                float4( 3.0, -1.0, 0.0, 1.0)
            };

            Varyings vert(uint vertexID : SV_VertexID)
            {
                Varyings output;
                output.positionHCS = positions[vertexID];
                return output;
            }

            float colorBased(float2 screenUV, float2 pixelSize)
            {
                float3 bl = Unity_Universal_SampleBuffer_BlitSource(screenUV + pixelSize * float2(-1,-1)).rgb;
                float3 bm = Unity_Universal_SampleBuffer_BlitSource(screenUV + pixelSize * float2( 0,-1)).rgb;
                float3 br = Unity_Universal_SampleBuffer_BlitSource(screenUV + pixelSize * float2( 1,-1)).rgb;

                float3 ml = Unity_Universal_SampleBuffer_BlitSource(screenUV + pixelSize * float2(-1, 0)).rgb;
                float3 mr = Unity_Universal_SampleBuffer_BlitSource(screenUV + pixelSize * float2( 1, 0)).rgb;

                float3 tl = Unity_Universal_SampleBuffer_BlitSource(screenUV + pixelSize * float2(-1, 1)).rgb;
                float3 tm = Unity_Universal_SampleBuffer_BlitSource(screenUV + pixelSize * float2( 0, 1)).rgb;
                float3 tr = Unity_Universal_SampleBuffer_BlitSource(screenUV + pixelSize * float2( 1, 1)).rgb;
                
                float l_bl = Luminance(bl);
                float l_bm = Luminance(bm);
                float l_br = Luminance(br);
                float l_ml = Luminance(ml);
                float l_mr = Luminance(mr);
                float l_tl = Luminance(tl);
                float l_tm = Luminance(tm);
                float l_tr = Luminance(tr);
                
                float gx = -l_bl - 2*l_ml - l_tl + l_br + 2*l_mr + l_tr;
                float gy = -l_bl - 2*l_bm - l_br + l_tl + 2*l_tm + l_tr;

                return sqrt(gx*gx + gy*gy);
            }

            float depthBased(float2 screenUV, float2 pixelSize)
            {
                float3 raw_bl = SampleSceneDepth(screenUV + pixelSize * float2(-1,-1));
                float3 raw_bm = SampleSceneDepth(screenUV + pixelSize * float2( 0,-1));
                float3 raw_br = SampleSceneDepth(screenUV + pixelSize * float2( 1,-1));

                float3 raw_ml = SampleSceneDepth(screenUV + pixelSize * float2(-1, 0));
                float3 raw_mr = SampleSceneDepth(screenUV + pixelSize * float2( 1, 0));

                float3 raw_tl = SampleSceneDepth(screenUV + pixelSize * float2(-1, 1));
                float3 raw_tm = SampleSceneDepth(screenUV + pixelSize * float2( 0, 1));
                float3 raw_tr = SampleSceneDepth(screenUV + pixelSize * float2( 1, 1));

                float bl = LinearEyeDepth(raw_bl, _ZBufferParams);
                float bm = LinearEyeDepth(raw_bm, _ZBufferParams);
                float br = LinearEyeDepth(raw_br, _ZBufferParams);
                float ml = LinearEyeDepth(raw_ml, _ZBufferParams);
                float mr = LinearEyeDepth(raw_mr, _ZBufferParams);
                float tl = LinearEyeDepth(raw_tl, _ZBufferParams);
                float tm = LinearEyeDepth(raw_tm, _ZBufferParams);
                float tr = LinearEyeDepth(raw_tr, _ZBufferParams);
                
                float gx = -bl - 2*ml - tl + br + 2*mr + tr;
                float gy = -bl - 2*bm - br + tl + 2*tm + tr;

                return sqrt(gx*gx + gy*gy);
            }

            float normalBased(float2 screenUV, float2 pixelSize)
            {
                float3 bl = SampleSceneNormals(screenUV + pixelSize * float2(-1,-1));
                float3 bm = SampleSceneNormals(screenUV + pixelSize * float2( 0,-1));
                float3 br = SampleSceneNormals(screenUV + pixelSize * float2( 1,-1));

                float3 ml = SampleSceneNormals(screenUV + pixelSize * float2(-1, 0));
                float3 mr = SampleSceneNormals(screenUV + pixelSize * float2( 1, 0));

                float3 tl = SampleSceneNormals(screenUV + pixelSize * float2(-1, 1));
                float3 tm = SampleSceneNormals(screenUV + pixelSize * float2( 0, 1));
                float3 tr = SampleSceneNormals(screenUV + pixelSize * float2( 1, 1));
                
                float gx = -bl - 2*ml - tl + br + 2*mr + tr;
                float gy = -bl - 2*bm - br + tl + 2*tm + tr;

                return sqrt(gx*gx + gy*gy);
            }

            float4 frag(Varyings input) : SV_Target
            {
                float2 screenUV = input.positionHCS.xy / _ScreenParams.xy;
     
                float2 pixelSize = 1 / _ScreenParams.xy;

                float colorEdge = colorBased(screenUV, pixelSize);
                float depthEdge = depthBased(screenUV, pixelSize);
                float normalEdge = normalBased(screenUV, pixelSize);
                
                if (_ColorThreshold != 0 && colorEdge > _ColorThreshold ) return _ColorOutlineColor;
                if (_DepthThreshold != 0 && depthEdge > _DepthThreshold) return _DepthOutlineColor;
                if (_NormalThreshold != 0 && normalEdge > _NormalThreshold) return _NormalOutlineColor;
                return Unity_Universal_SampleBuffer_BlitSource(screenUV);
            }

            ENDHLSL
        }
    }
}

