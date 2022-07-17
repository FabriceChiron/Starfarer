/*
    Basically, the shader samples the cracks multiple times (Samples parameter)
    Each sample slightly changes the UV based on tangent space's view direction.
    At the end the shader combines the cracks texture and combined color of all samples.

    This technique is called Parallax Mapping, you can read more about it here:
    https://en.wikipedia.org/wiki/Parallax_mapping

    Code is based on a tutorial by Jettelly https://www.youtube.com/watch?v=_dZthyScKF8
*/


// Unnecessary includes, used only for autocompletion
#ifndef SHADERGRAPH_PREVIEW
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#endif

void CalculateIceDepth_half(
    in UnityTexture2D MainTexture,
    in float2 UV,
    in UnitySamplerState SampleState,
    in float Samples,
    in float Offset,
    in float3 TangentViewDirection,
    in float MainColorIntensity,
    in float DepthIntensity,
    in int TextureLOD,
    out float4 Out)
{

    half4 depthRawColor = 0;
    half uOffset = 0;
    half vOffset = 0;
    
    float uOffsetDelta = Offset * TangentViewDirection.x;
    float vOffsetDelta = Offset * TangentViewDirection.y;

    float2 offsettedUV;
    
    for(int s = 0; s < Samples; s++)
    {
        uOffset += uOffsetDelta;
        vOffset += vOffsetDelta;
        
        offsettedUV = float2(uOffset, vOffset);
        depthRawColor += SAMPLE_TEXTURE2D_LOD(MainTexture, SampleState, UV + offsettedUV, TextureLOD);
    }

    half4 mainColor = SAMPLE_TEXTURE2D(MainTexture, SampleState, UV);
    half4 depthColor = depthRawColor / Samples;
    
    Out = mainColor * MainColorIntensity + depthColor * DepthIntensity;
}