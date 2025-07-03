sampler uImage0 : register(s0);

float4 color;
float length;
float progress;
float offset;

float4 PixelShaderFunction(float2 texCoords : TEXCOORD0) : COLOR
{
    float x = texCoords.x + progress;
    float y = texCoords.y;
    int n = x / length;
    x -= length * n;
    if (x < 0)
        x += length; 
    x /= length;
    float c = 1;
    if (texCoords.x < offset)
        c = texCoords.x / offset;
    if (texCoords.x > 1 - offset)
        c = 1 - (texCoords.x - 1 + offset) / offset;
    return tex2D(uImage0, float2(x, y)) * color * c;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}