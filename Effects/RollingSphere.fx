sampler uImage0 : register(s0);


float4 color1;
float4 color2;
float3 circleCenter;
float radiusOfCircle;
float progress;

/*
* 使用方法：	uTransform		国际惯例是顶点坐标变换矩阵，只会影响两个三角形的变换，也就是教程中的屏幕
*			circleCenter	球心的三维坐标，注意这个坐标的z分量需要是负数才能显示出来，因为我用的是另一个坐标系
*			radiusOfCircle	球的半径
* 如有需要可以自己加参数
*/

float4 PixelShaderFunction(float2 texCoords : TEXCOORD0) : COLOR
{
    float2 coord = texCoords;
	
    float3 dir = float3(coord.x, coord.y, -1);
    float3 P = -circleCenter;
    float A = dot(dir, dir);
    float B = 2 * dot(dir, P);
    float C = dot(P, P) - radiusOfCircle * radiusOfCircle;
	
	// 解方程
    float det = B * B - 4 * A * C;
    if (det < 0)
        return float4(0, 0, 0, 0);
    float sqdet = sqrt(det);
    float t1 = (-B + sqdet) / (2 * A);
    float t2 = (-B - sqdet) / (2 * A);
    float t = t1 < t2 ? t1 : t2;
	
	// 求 theta 和 phi ，对应 x, y
    float3 hitpos = dir * t - circleCenter;
    float3 N = normalize(hitpos);
    float x = atan2(hitpos.z, hitpos.x) / 3.14159 + 1.0;
    float y = hitpos.y / radiusOfCircle + 1.0;
	
	// 因为我坐标是 [-1, 1] 这个区间的，所以先要翻正再取模
    float xx = fmod(x + 1, 1.0);
    float yy = fmod(y + 1, 1.0);
    xx += progress;
    xx -= (int) xx;
    if (xx < 0)
        xx++;
    return lerp(color1, color2, tex2D(uImage0, float2(xx, yy)));
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
