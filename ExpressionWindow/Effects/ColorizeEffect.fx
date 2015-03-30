// Brightness / Contrast / Saturation Shader 

// Object Declarations

sampler2D implicitInput : register(s0);
float4 ColorMix : register(c0);

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = tex2D(implicitInput, uv);

	color.rgb /= color.a;

	//Apply color
	color.rgb = saturate(color * ColorMix);

	// Return final pixel color.
	color.rgb *= color.a;


	return color;
}