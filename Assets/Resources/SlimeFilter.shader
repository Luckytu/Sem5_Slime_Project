Shader "Slime/Filter"
{
    // The _BaseMap variable is visible in the Material's Inspector, as a field
    // called Base Map.
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        
        cutoff("Cutoff", Float) = 1.0
        
        brightnessInclude("Brightness Include", Float) = 1.0
        brightnessExclude("Brightness Exclude", Float) = 1.0
        
        saturationInclude("Saturation Include", Float) = 1.0
        saturationExclude("Saturation Exclude", Float) = 1.0
        
        filterColor("Filter Color", Color) = (1, 1, 1, 1)
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
            
            // This macro declares the sampler for the _BaseMap texture.
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                // The following line declares the _BaseMap_ST variable, so that you
                // can use the _BaseMap variable in the fragment shader. The _ST 
                // suffix is necessary for the tiling and offset function to work.
                float4 _BaseMap_ST;

                float cutoff;
            
                float brightnessInclude;
                float brightnessExclude;

                float saturationInclude;
                float saturationExclude;
            
                half4 filterColor;
            
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

            float4x4 contrastMatrix( float contrast )
            {
	            float t = ( 1.0 - contrast ) / 2.0;
                
                return float4x4( contrast, 0, 0, 0,
                             0, contrast, 0, 0,
                             0, 0, contrast, 0,
                             t, t, t, 1 );

            }

            float4x4 saturationMatrix( float saturation )
            {
                float3 luminance = float3( 0.3086, 0.6094, 0.0820 );
                
                float oneMinusSat = 1.0 - saturation;
                
                float3 red = float3( luminance.xxx * oneMinusSat );
                red+= float3( saturation, 0, 0 );
                
                float3 green = float3( luminance.yyy * oneMinusSat );
                green += float3( 0, saturation, 0 );
                
                float3 blue = float3( luminance.zzz * oneMinusSat );
                blue += float3( 0, 0, saturation );
                
                return float4x4( red,     0,
                             green,   0,
                             blue,    0,
                             0, 0, 0, 1 );
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                half4 diff = abs(color - filterColor);
                /*
                if(length(diff) < cutoff)
                {
                    color = mul(color, saturationMatrix(saturationInclude)) * brightnessInclude;
                }
                else
                {
                    color = mul(color, saturationMatrix(saturationExclude)) * brightnessExclude;
                }
                */
                return color * brightnessInclude;
            }
            ENDHLSL
        }
    }
}
