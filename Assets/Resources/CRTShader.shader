Shader "Slime/CRTShader"
{
    // The _BaseMap variable is visible in the Material's Inspector, as a field
    // called Base Map.
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white"
        NoiseMap("Noise Map", 2D) = "white"
        
        distortMultiplier("Distort Multiplier", Float) = 1.0
        speed("speed", Float) = 1.0
        
        brightness("Brightness", Float) = 1.0
        scanLineAmount("Scan Line Amount", Float) = 100.0
        scanLineSharpness("Scan Line Sharpness", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "IgnoreProjector" = "True"
            "UniversalMaterialType" = "Unlit"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            struct Attributes
            {
                float4 positionOS   : POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                // The uv variable contains the UV coordinate on the texture for the
                // given vertex.
                float2 uv           : TEXCOORD0;
            };

            // This macro declares _BaseMap as a Texture2D object.
            TEXTURE2D(_BaseMap);
            TEXTURE2D(NoiseMap);
            
            // This macro declares the sampler for the _BaseMap texture.
            SAMPLER(sampler_BaseMap);
            SAMPLER(sampler_NoiseMap);

            CBUFFER_START(UnityPerMaterial)
                // The following line declares the _BaseMap_ST variable, so that you
                // can use the _BaseMap variable in the fragment shader. The _ST 
                // suffix is necessary for the tiling and offset function to work.
                float4 _BaseMap_ST;
                float4 NoiseMap_ST;

                float distortMultiplier;
                float speed;

                float brightness;
                float scanLineAmount;
                float scanLineSharpness;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                // The TRANSFORM_TEX macro performs the tiling and offset
                // transformation.
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 noiseUV = IN.uv;
                noiseUV.y = (noiseUV.y + _Time * speed) % 1.0;

                half4 noiseColor = SAMPLE_TEXTURE2D(NoiseMap, sampler_NoiseMap, noiseUV);
                const half distortAmount = noiseColor.a * (noiseColor.r - 0.5) * 2 * distortMultiplier * 0.01;

                float2 colorUV = IN.uv;

                const half colorMultiplier = clamp((sin(colorUV.y * scanLineAmount + _Time * speed * scanLineAmount) + 1) * 0.5 * scanLineSharpness, 0, 1) + brightness;

                colorUV.x = colorUV.x + distortAmount;
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, colorUV) * colorMultiplier;
                
                return color;
            }
            ENDHLSL
        }
    }
}
