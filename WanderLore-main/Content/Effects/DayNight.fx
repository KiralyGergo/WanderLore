// Lightweight day/night color grading for the world sprites.
Texture2D Texture;
SamplerState TextureSampler
{
    Texture = <Texture>;
    Filter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

float TimeOfDay = 0.28;

float4 DayNightPixelShader(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_Target0
{
    float4 pixel = Texture.Sample(TextureSampler, texCoord) * color;
    float phase = TimeOfDay * 6.2831853;
    float daylight = saturate(sin(phase - 1.5707963) * 0.5 + 0.5);
    float twilight = 1.0 - saturate(abs(daylight - 0.5) * 3.0);

    float3 nightTint = float3(0.42, 0.52, 0.78);
    float3 dayTint = float3(1.0, 1.0, 0.94);
    float3 dawnTint = float3(1.0, 0.62, 0.38);
    float3 tint = lerp(nightTint, dayTint, daylight);
    tint = lerp(tint, dawnTint, twilight * 0.30);

    float brightness = lerp(0.48, 1.0, daylight);
    pixel.rgb *= tint * brightness;
    return pixel;
}

technique DayNight
{
    pass P0
    {
        PixelShader = compile ps_6_0 DayNightPixelShader();
    }
}
