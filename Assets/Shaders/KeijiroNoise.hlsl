#ifndef KEIJIRO_NOISE_SIMPLE_INCLUDED
#define KEIJIRO_NOISE_SIMPLE_INCLUDED

#include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise2D.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/ClassicNoise3D.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise2D.hlsl"
#include "Packages/jp.keijiro.noiseshader/Shader/SimplexNoise3D.hlsl"

#define NOISE_FUNC(coord, period) PeriodicNoise(coord, period)

void KeijiroNoiseSimple_float(float2 UV, float Time, out float Out)
{
    const float epsilon = 0.0001;

    float2 uv = UV * 4 + float2(0.2, 1) * Time;

    float o = 0.5;
    float s = 1;
    float w = 0.5;

    for (int i = 0; i < 6; i++)
    {
        float3 coord = float3(uv * s, Time);
        float3 period = float3(s, s, 1.0) * 2.0;
        o += NOISE_FUNC(coord, period) * w;
        s *= 2.0;
        w *= 0.5;
    }
    
    Out = o;
}

#endif