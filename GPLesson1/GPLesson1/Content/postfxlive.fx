﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

Texture2D _MainTex;
sampler2D _MainTexSampler = sampler_state
{
	Texture = <_MainTex>;
};

float4 InvertPS(float2 uv : VPOS) : COLOR
{
	uv = (uv + 0.5) * float2(1.0 / 800, 1.0 / 480.0);
	
	float4 color = tex2D(_MainTexSampler, uv);

	return 1 - color;
}

technique Invert
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL InvertPS();
	}
};


float4 ChromaticAbberationPS(float2 uv : VPOS) : COLOR
{
	uv = (uv + 0.5) * float2(1.0 / 800, 1.0 / 480.0);

	float strength = 5;
	float3 rgbOffset = 1 + float3(0.01, 0.005, 0) * strength;
	float dist = distance(uv, float2(0.5, 0.5));
	float2 dir = uv - float2(0.5, 0.5);

	//Scale rbgOffset & renormalize
	rgbOffset = normalize(rgbOffset * dist);

	//Calculate uvs for each color channel
	float2 uvR = float2(0.5, 0.5) + rgbOffset.r * dir;
	float2 uvG = float2(0.5, 0.5) + rgbOffset.g * dir;
	float2 uvB = float2(0.5, 0.5) + rgbOffset.b * dir;
	
	float4 colorR = tex2D(_MainTexSampler, uvR);
	float4 colorG = tex2D(_MainTexSampler, uvG);
	float4 colorB = tex2D(_MainTexSampler, uvB);


	return float4(colorR.r, colorG.g, colorB.b, 1);
}

technique ChromaticAbberation
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL ChromaticAbberationPS();
	}
};