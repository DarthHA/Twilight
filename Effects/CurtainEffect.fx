sampler uImage0 : register(s0);

float progress;
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float k = coords.x - progress;
    return color * clamp((coords.x - progress) / 0.05f, 0, 1.05f);
}



technique Technique1
{
    pass CurtainEffect
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
