#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_4_0_level_9_1
    #define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix MatrixTransform;

// Solid color to paint the silhouette (set by C# code before drawing)
float4 GhostColor;

sampler2D TextureSampler : register(s0);

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color    : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput MainVS(float4 position : POSITION, float4 color : COLOR0, float2 texCoord : TEXCOORD0)
{
    VertexShaderOutput output;
    output.Position = mul(position, MatrixTransform);
    output.Color    = color;
    output.TexCoord = texCoord;
    return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float4 tex = tex2D(TextureSampler, input.TexCoord);
    // Replace all RGB with GhostColor; preserve sprite alpha × draw alpha
    return float4(GhostColor.rgb, tex.a * input.Color.a * GhostColor.a);
}

technique SpriteDrawing
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader  = compile PS_SHADERMODEL MainPS();
    }
}
