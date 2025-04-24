using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public sealed class PostTest : MonoBehaviour
{
	public ClampedFloatParameter blend = new ClampedFloatParameter(value: 0.5f, min: 0, max: 1, overrideState: true);

	[SerializeField]
	public Material mat;

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Debug.Log("ihv");
		Graphics.Blit(source, destination, mat);
	}
}
