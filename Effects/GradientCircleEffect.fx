sampler uImage0 : register(s0);

float4 color;
float radius;
float offset;

float4 PixelShaderFunction(float2 texCoords : TEXCOORD0) : COLOR
{
    float dist = sqrt((texCoords.x - 0.5) * (texCoords.x - 0.5) + (texCoords.y - 0.5) * (texCoords.y - 0.5)) * 2;
    float k = clamp(1 - (dist - radius) / offset, 0, 1);
    return tex2D(uImage0, texCoords) * color * k;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}