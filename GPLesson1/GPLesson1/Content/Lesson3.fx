#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// External Properties
float4x4 World, View, Projection;

float3 LightPosition, CameraPosition;
float Time;

Texture2D MainTex;
sampler2D MainTextureSampler = sampler_state
{
    Texture = <MainTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

Texture2D NormalTex;
sampler2D NormalTextureSampler = sampler_state
{
    Texture = <NormalTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

Texture2D DayTex;
sampler2D DayTexSampler = sampler_state
{
    Texture = <DayTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

Texture2D NightTex;
sampler2D NightTexSampler = sampler_state
{
    Texture = <NightTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

Texture2D CloudsTex;
sampler2D CloudsTexSampler = sampler_state
{
    Texture = <CloudsTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
    AddressU = WRAP;
};

Texture2D MoonTex;
sampler2D MoonTexSampler = sampler_state
{
    Texture = <MoonTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

TextureCube SkyTex;
samplerCUBE SkyTexSampler = sampler_state
{
    Texture = <SkyTex>;
    MipFilter = POINT;
    MinFilter = ANISOTROPIC;
    MagFilter = ANISOTROPIC;
};

// Getting out vertex data from vertex shader to pixel shader
struct VertexShaderOutput {
    float4 position     : SV_POSITION;
    float4 color        : COLOR0;
    float2 uv           : TEXCOORD0;
    float3 worldPos     : TEXCOORD1;
    float3 worldNormal  : TEXCOORD2;
};

// Vertex shader, receives values directly from semantic channels
VertexShaderOutput MainVS(float4 position : POSITION, float4 color : COLOR0, float2 uv : TEXCOORD, float3 normal : NORMAL )
{
    VertexShaderOutput output = (VertexShaderOutput)0;

    output.position = mul( mul( mul(position, World), View), Projection);
    output.color = color;
    output.uv = uv;
    output.worldPos = mul(position, World).xyz;
    output.worldNormal = mul(normal, World).xyz;

    return output;
}

// Pixel Shader, receives input from vertex shader, and outputs to COLOR semantic
float4 MainPS(VertexShaderOutput input) : COLOR
{
    //textures
    float4 texColor = tex2D(DayTexSampler, input.uv);
    float4 nightColor = tex2D(NightTexSampler, input.uv);
    float4 cloudColor = tex2D(CloudsTexSampler, input.uv + half2(Time * 0.1,0));
    float4 normalColor = tex2D(NormalTextureSampler, input.uv);

    float3 perturbedNormal = input.worldNormal;
    perturbedNormal.rg += (normalColor.rg * 2 - 1);
    perturbedNormal = normalize(perturbedNormal);

    //calculate vectors for lighting and specular
    float3 viewDirection = normalize(input.worldPos - CameraPosition);
    float3 lightDirection = normalize(input.worldPos - LightPosition);

    //calculate specular
    float3 refl = normalize(-reflect(lightDirection, perturbedNormal));
    float spec = pow(max(dot(refl, normalize(viewDirection)), 0.0),8);

    //calculate lighting
    float light = max(dot(input.worldNormal, -lightDirection), 0.0);

    float3 skyColor = float3(0.529, 0.808, 0.992);

    float3 fresnel = pow(dot(input.worldNormal, viewDirection) * 0.5 + 0.5, 3) * 8 * light * skyColor;

    float3 diffuse = lerp(nightColor.rgb, texColor.rgb, light) + (cloudColor.rgb * light);

    return float4 ((max(light, 0.2) + spec) * diffuse.rgb + fresnel, 1);

}

float4 MoonPS(VertexShaderOutput input) : COLOR
{
    //textures
    float4 texColor = tex2D(MoonTexSampler, input.uv);

    //calculate vectors for lighting
    float3 lightDirection = normalize(input.worldPos - LightPosition);

    //calculate lighting
    float light = min(max(dot(input.worldNormal, -lightDirection), 0.0) * 64, 1.0);

    //float light = max(dot(input.worldNormal, -lightDirection), 0.0);

    return float4 ((max(light, 0.1)) * texColor.rgb, 1);
}

float4 Moon2PS(VertexShaderOutput input) : COLOR
{
    //textures
    float4 texColor = tex2D(MoonTexSampler, input.uv);

    //calculate vectors for lighting
    float3 lightDirection = normalize(input.worldPos - LightPosition);

    //calculate lighting
    float light = min(max(dot(input.worldNormal, -lightDirection), 0.0) * 64, 1.0);

    //float light = max(dot(input.worldNormal, -lightDirection), 0.0);

    return float4 ((max(light, 0.1)) * texColor.rgb, 1);
}

float4 SkyPS(VertexShaderOutput input) : COLOR
{
    float3 viewDirection = normalize(input.worldPos - CameraPosition);

    float3 skyColor = texCUBE(SkyTexSampler, viewDirection).rgb;

    return float4(pow(skyColor, 1.75f), 1);
       }

technique Earth
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

technique Moon
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MoonPS();
    }
};

technique Moon2
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL Moon2PS();
    }
};

technique Sky
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL SkyPS();
    }
};