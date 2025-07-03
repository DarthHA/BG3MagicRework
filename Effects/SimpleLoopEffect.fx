sampler uImage0 : register(s0);

float4 color;
float length;
float progress;

float4 PixelShaderFunction(float2 texCoords : TEXCOORD0) : COLOR
{
    float x = texCoords.x + progress;
    float y = texCoords.y;
    int n = x / length;
    x -= length * n;
    if (x < 0)
        x += length; 
    x /= length;
    return tex2D(uImage0, float2(x, y)) * color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}