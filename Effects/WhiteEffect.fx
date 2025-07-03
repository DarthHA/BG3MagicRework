sampler uImage0 : register(s0);
float4 newColor;

float4 PixelShaderFunction(float2 texCoords : TEXCOORD0) : COLOR
{
    if (!any(tex2D(uImage0, texCoords)))
        return float4(0, 0, 0, 0);
    return newColor;

}
technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}