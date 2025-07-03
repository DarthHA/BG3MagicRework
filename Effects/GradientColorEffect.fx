sampler uImage0 : register(s0);

float4 color;

float4 PixelShaderFunction(float2 texCoords : TEXCOORD0) : COLOR
{
    return tex2D(uImage0, float2(texCoords.x, texCoords.y)) * color * (1 - texCoords.x);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}