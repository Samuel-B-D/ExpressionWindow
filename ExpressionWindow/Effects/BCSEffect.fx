// Brightness / Contrast / Saturation Shader 

// Object Declarations

sampler2D implicitInput : register(s0);
float Brightness : register(c0);
float Contrast : register(c1);
float Saturation : register(c2);

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = tex2D(implicitInput, uv);

	color.rgb /= color.a;

	// Apply saturation
	float grey = (color.r + color.g + color.b) / 3;
	color.r = lerp(color.r, grey, Saturation);
	color.g = lerp(color.g, grey, Saturation);
	color.b = lerp(color.b, grey, Saturation);

	// Apply contrast.
	color.rgb = ((color.rgb - 0.5f) * max(Contrast, 0)) + 0.5f;

	// Apply brightness.
	color.rgb += Brightness;

	// Return final pixel color.
	color.rgb *= color.a;


	return color;
}