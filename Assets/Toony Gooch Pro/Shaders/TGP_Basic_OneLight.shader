// Toony Gooch Pro+Mobile Shaders
// (c) 2013, Jean Moreno

Shader "Toony Gooch Pro/Normal/OneDirLight/Basic"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		
		//GOOCH
		_Color ("Highlight Color", Color) = (0.8,0.8,0.8,1)
		_SColor ("Shadow Color", Color) = (0.0,0.0,0.0,1)
		
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull off
		
		CGPROGRAM
		
		#include "TGP_Include.cginc"
		
		//nolightmap nodirlightmap		LIGHTMAP
		//noforwardadd					ONLY 1 DIR LIGHT (OTHER LIGHTS AS VERTEX-LIT)
		#pragma surface surf ToonyGooch nolightmap nodirlightmap noforwardadd 
		
		sampler2D _MainTex;
		
		struct Input
		{
			half2 uv_MainTex : TEXCOORD0;
		};
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = c.rgb;
			
		}
		ENDCG
	}
	
	Fallback "VertexLit"
}
