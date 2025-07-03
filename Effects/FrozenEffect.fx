sampler2D uImage0 : register(s0);
sampler2D uImage1 : register(s1);
float uOpacity;
float uIntensity;
float uTime;
float4 lightColor;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, uv);
    if (color.a < 0.01)
        return color;
    
    float luminance = dot(color.rgb, float3(0.299, 0.587, 0.114));
    float3 iceColor = lerp(float3(0.7, 0.8, 0.9), float3(0.9, 0.95, 1.0), luminance);
    
    float3 finalColor = lerp(color.rgb, iceColor, uIntensity * 0.7);
    
    //边缘高光
    float edge = 1.0 - color.a * color.a;
    edge = smoothstep(0.3, 0.7, edge) * 0.8;
    finalColor = lerp(finalColor, float3(1, 1, 1), edge * uIntensity);
    
    //简单动态反光
    float shine = sin(uTime * 2.0 + uv.x * 10.0 + uv.y * 5.0);
    shine = shine * 0.1 + tex2D(uImage1, float2(uv.x, uv.y)) * 0.9;
    shine = shine * 0.2 + 0.2;
    finalColor += shine * uIntensity * float3(0.8, 0.9, 1.0);
    
    return float4(finalColor, color.a * uOpacity) * lightColor;
}
technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}