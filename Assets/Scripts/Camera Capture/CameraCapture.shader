Shader "Slime/CameraCapture"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        filterCutoff("Filter Cutoff", Range(0.0, 1.0)) = 0.5

        a11("a11", Float) = 1.0
        a12("a12", Float) = 0.0
        a13("a13", Float) = 0.0

        a21("a21", Float) = 0.0
        a22("a22", Float) = 1.0
        a23("a23", Float) = 0.0

        a31("a31", Float) = 0.0
        a32("a32", Float) = 0.0
    }
    // The SubShader block containing the Shader code.
    SubShader
    {
        // SubShader Tags define when and under which conditions a SubShader block or
        // a pass is executed.
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            // The HLSL code block. Unity SRP uses the HLSL language.
            HLSLPROGRAM
            // This line defines the name of the vertex shader.
            #pragma vertex vert
            // This line defines the name of the fragment shader.
            #pragma fragment frag

            // The Core.hlsl file contains definitions of frequently used HLSL
            // macros and functions, and also contains #include references to other
            // HLSL files (for example, Common.hlsl, SpaceTransforms.hlsl, etc.).
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // The structure definition defines which variables it contains.
            // This example uses the Attributes structure as an input structure in
            // the vertex shader.
            struct Attributes
            {
                // The positionOS variable contains the vertex positions in object
                // space.
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                // The positions in this struct must have the SV_POSITION semantic.
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float filterCutoff;

                float a11;
                float a12;
                float a13;

                float a21;
                float a22;
                float a23;

                float a31;
                float a32;
            CBUFFER_END

            // The vertex shader definition with properties defined in the Varyings
            // structure. The type of the vert function must match the type (struct)
            // that it returns.
            Varyings vert(Attributes IN)
            {
                // Declaring the output object (OUT) with the Varyings struct.
                Varyings OUT;
                // The TransformObjectToHClip function transforms vertex positions
                // from object space to homogenous clip space.
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                // Returning the output.
                return OUT;
            }

            // The fragment shader definition.
            half4 frag(Varyings IN) : SV_Target
            {
                
                //coordinates in the unit square
                float2 uv = float2(IN.uv.x, IN.uv.y);

                //projected homogenous coordinates
                float3 projH = float3(a11 * uv.x + a12 * uv.y + a13,
                                      a21 * uv.x + a22 * uv.y + a23,
                                      a31 * uv.x + a32 * uv.y + 1);
                
                //de-homogenized coordinates
                float2 projected = float2(projH.x / projH.z, projH.y / projH.z);

                // Defining the color variable and returning it.
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, projected.xy);
                
                //HSV value
                half value = max(color.r, max(color.g, color.b));

                if(value <= filterCutoff)
                {
                    return half4(1, 1, 1, 1);
                }
                else
                {
                    return half4(0, 0, 0, 1);
                }
                

                //return color;
                
            }
            ENDHLSL
        }
    }
}