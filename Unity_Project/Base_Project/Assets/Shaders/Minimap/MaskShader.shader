Shader "Custom/MaskShader" {
	Properties {
		//_Color ("Color", Color) = (1,1,1,1)
		//_MainTex ("Albedo (RGB)", 2D) = "white" {}
		//_Glossiness ("Smoothness", Range(0,1)) = 0.5
		//_Metallic ("Metallic", Range(0,1)) = 0.0
		_MainTex ("Main Texture", 2D) = "white" {}
		_Mask ("Mask Texture", 2D) = "white" {}
	}
	
	SubShader {
		Tags { "Queue"="Transparent" }
		Lighting On
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{

			SetTexture [_Mask] {combine texture}
			SetTexture [_MainTex] {combine texture, previous}

		}
	} 
}