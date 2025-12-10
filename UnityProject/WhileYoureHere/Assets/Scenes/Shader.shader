Shader "Custom/EdgeDetection"
{
    Properties
    {
        _EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
        _Threshold ("Depth Threshold", Float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "LightMode"="Universal2D" }
        ZTest Always Cull Off ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex fullscreenVert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl" // Contains fullscreenVert
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl" // Access depth

            float4 _EdgeColor;
            float _Threshold;

            float4 frag(VaryingsFullScreen input) : SV_Target
            {
                float2 texelSize = 1.0 / _ScreenParams.xy;
                float2 uv = input.uv;

                // Sample depth at 4 diagonal neighbors
                float depthC = SampleSceneDepth(uv);
                float depthNW = SampleSceneDepth(uv + float2(-1, 1) * texelSize);
                float depthNE = SampleSceneDepth(uv + float2(1, 1) * texelSize);
                float depthSW = SampleSceneDepth(uv + float2(-1, -1) * texelSize);
                float depthSE = SampleSceneDepth(uv + float2(1, -1) * texelSize);

                // Calculate depth differences (Sobel approximation)
                float depthX = depthNE + 2.0 * depthSE - depthNW - 2.0 * depthSW;
                float depthY = depthNW + 2.0 * depthNE - depthSW - 2.0 * depthSE;
                
                float edge = sqrt(depthX * depthX + depthY * depthY);

                // Check if the edge magnitude is above our threshold
                if (edge > _Threshold)
                {
                    // If it is an edge, return the edge color (e.g., black lines)
                    return _EdgeColor;
                }
                else
                {
                    // Otherwise, return the original scene color
                    // We access the original color buffer that we set up in C#
                    return SampleSceneColor(uv);
                }
            }
            ENDHLSL
        }
    }
}
