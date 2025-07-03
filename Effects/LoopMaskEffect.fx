sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float4 color;
float length;
float progress;

float4 PixelShaderFunction(float2 texCoords : TEXCOORD0) : COLOR
{
    float4 color1 = tex2D(uImage0, texCoords);
    if (!any(color1))
        return color1;
    float x = texCoords.x + progress;
    float y = texCoords.y;
    int n = x / length;
    x -= length * n;
    if (x < 0)
        x += length; 
    x /= length;
    float4 color2 = tex2D(uImage1, float2(x, y));
    return color1 * color2 * color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}