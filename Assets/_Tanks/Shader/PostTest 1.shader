Shader "Hidden/Custom/Fisheye"
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

			float4 ffrag(Varyings v) : SV_Target
			{
				float2 screenPos = v.texcoord * 2 - 1;
				float factor = pow(length(screenPos), 2);
				float2 uv = v.texcoord - normalize(screenPos) * factor * _intensity;

				float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);

				return color;
			}

			#pragma vertex Vert
			#pragma fragment ffrag
			ENDHLSL
		}
	}
}
