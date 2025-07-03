sampler uImage0 : register(s0);

float4 color;
float2 length;
float2 progress;
float2 offset;

float4 PixelShaderFunction(float2 texCoords : TEXCOORD0) : COLOR
{
    float x = texCoords.x + progress.x;
    float y = texCoords.y + progress.y;
    
    int n = x / length.x;
    x -= length.x * n;
    if (x < 0)
        x += length.x; 
    x /= length.x;
    
    int m = y / length.y;
    y -= length.y * m;
    if (y < 0)
        y += length.y;
    y /= length.y;
    
    float c1 = 1, c2 = 1;
    if (texCoords.x < offset.x)
        c1 = texCoords.x / offset.x;
    if (texCoords.x > 1 - offset.x)
        c1 = 1 - (texCoords.x - 1 + offset.x) / offset.x;
    
    if (texCoords.y < offset.y)
        c2 = texCoords.y / offset.y;
    if (texCoords.y > 1 - offset.y)
        c2 = 1 - (texCoords.y - 1 + offset.y) / offset.y;
    
    return tex2D(uImage0, float2(x, y)) * color * min(c1, c2);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}