Shader "Hidden/Custom/Grayscale"
{
	SubShader
	{
		Pass
		{
			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/RunTime/Utilities/Blit.hlsl"           
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

			float _intensity;
			float3 _vignetteColor;

			float4 ffrag(Varyings v) : SV_Target
			{
				float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, v.texcoord.xy);

				float vignetteBias = length(v.texcoord.xy * 2 - 1) * 0.5;
				color.rgb = lerp(color.rgb, float3(0,0,0), vignetteBias);

				return color;
			}

			#pragma vertex Vert
			#pragma fragment ffrag
			ENDHLSL
		}
	}
}
