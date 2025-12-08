
Shader "Custom/SobelColor_URP"
{
    Properties
    {
        _BlitTexture("Source", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Name "SobelPass"

            
            HLSLPROGRAM
            #include "UnityCG.cginc"
            
            #pragma vertex Vert
            #pragma fragment Frag

            sampler2D _BlitTexture;
            float4 _BlitTexture_TexelSize;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes v)
            {
                Varyings o = (Varyings)0;
                o.positionHCS = UnityObjectToClipPos(v.positionOS);
                o.uv = v.uv;
                return o;
            }

            float4 Frag(Varyings i) : SV_Target
            {
                float2 texel = _BlitTexture_TexelSize.xy;

                float3 tl = tex2D(_BlitTexture, i.uv + texel * float2(-1,  1)).rgb;
                float3  l = tex2D(_BlitTexture, i.uv + texel * float2(-1,  0)).rgb;
                float3 bl = tex2D(_BlitTexture, i.uv + texel * float2(-1, -1)).rgb;

                float3 tr = tex2D(_BlitTexture, i.uv + texel * float2( 1,  1)).rgb;
                float3  r = tex2D(_BlitTexture, i.uv + texel * float2( 1,  0)).rgb;
                float3 br = tex2D(_BlitTexture, i.uv + texel * float2( 1, -1)).rgb;

                float3 t = tex2D(_BlitTexture, i.uv + texel * float2(0,  1)).rgb;
                float3 b = tex2D(_BlitTexture, i.uv + texel * float2(0, -1)).rgb;

                float3 gx = -tl - 2*l - bl + tr + 2*r + br;
                float3 gy =  tl + 2*t + tr - bl - 2*b - br;

                float3 edge = sqrt(gx * gx + gy * gy);

                return float4(edge, 1);
            }
            ENDHLSL
        }
    }
}
