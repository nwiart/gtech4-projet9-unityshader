Shader "Hidden/Custom/Grayscale"
{
	SubShader
	{
		Pass {
			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
           #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
		              #include "Packages/com.unity.render-pipelines.core/RunTime/Utilities/Blit.hlsl"           
           #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

	  float _Blend;

	  float4 ffrag(Varyings v) : SV_Target
	  {
		  float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, v.texcoord.xy);
		  float luminance = dot(color.rgb, float3(0.2126729, 0.7151522, 0.0721750));
		  color.rgb = lerp(color.rgb, luminance.xxx, 1.0F);
		  return color;
	  }

	   #pragma vertex Vert
			  #pragma fragment ffrag
  ENDHLSL
			}
		}
 
}
