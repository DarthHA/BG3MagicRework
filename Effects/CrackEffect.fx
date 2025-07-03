sampler uImage0 : register(s1);
sampler uImage1 : register(s2);

float2 xy1;
float2 xy2;
float2 size1;
float2 size2;
float4 colorCrack;

float4 PixelShaderFunction(float2 texCoords : TEXCOORD0) : COLOR
{
    // 采样箱子纹理
    float2 uv1 = float2(
        xy1.x + size1.x * texCoords.x,
        xy1.y + size1.y * texCoords.y
    );
    
    if (!any(tex2D(uImage0, uv1)))
        return tex2D(uImage0, uv1);

    // 采样裂纹纹理
    float2 uv2 = float2(
        xy2.x + size2.x * texCoords.x,
        xy2.y + size2.y * texCoords.y
    );
    float4 crackColor = tex2D(uImage1, uv2) * colorCrack;
    
    if (!any(tex2D(uImage1, uv2)))
        return float4(0, 0, 0, 0);
    return crackColor;

}
technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}