Shader "TerrainExtention/Terrian" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "black" {}
		_TexR("SplatR (RGB)", 2D) = "white" {}
		_TexG("SplatG (RGB)", 2D) = "white" {}
		_TexB("SplatB (RGB)", 2D) = "white" {}
		_TexA("SplatA (RGB)", 2D) = "white" {}
		_NoTileR("NoTileRed",Range 		(0, 0.000000000000001))=0
		_NoTileG("NoTileGreen",Range 	(0, 0.000000000000001))=0
		_NoTileB("NoTileBlue",Range 	(0, 0.000000000000001))=0
		_NoTileA("NoTileAlpha",Range 	(0, 0.000000000000001))=0
		
		//[MaterialToggle]_NoTileG("TileGreen",Float)=0{}
		//[MaterialToggle]_NoTileB("TileBlue",Float)=0{}
		//[MaterialToggle]_NoTileA("TileAlpha",Float)=0{}
		
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _TexR,_TexG,_TexB,_TexA;
		half _NoTileR,_NoTileG,_NoTileB,_NoTileA;
		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			float2 uv = float2(IN.worldPos.x,IN.worldPos.z) + IN.uv_MainTex;
			fixed4 splat_control = tex2D (_MainTex, IN.uv_MainTex);
			
			fixed3 col;
			if(_NoTileR != 0)
			{
			col  = splat_control.r * (tex2D (_TexR, uv).rgb *tex2D (_TexR, uv *-0.25).rgb * 4);		
			}
			else
			{
			col  = splat_control.r * tex2D (_TexR, uv).rgb;
			}
			
		 	if(_NoTileG != 0)
			{
			col  += splat_control.g * (tex2D (_TexG, uv).rgb *tex2D (_TexG, uv *-0.25).rgb * 4);		
			}
			else
			{
			col  += splat_control.g * tex2D (_TexG, uv).rgb;
			}
			
			if(_NoTileB != 0)
			{
			col  += splat_control.b * (tex2D (_TexB, uv).rgb *tex2D (_TexB, uv *-0.25).rgb * 4);		
			}
			else
			{			
			col  += splat_control.b * tex2D (_TexB, uv).rgb;
			}
			
			if(_NoTileA != 0)
			{
			col  += splat_control.a * (tex2D (_TexA, uv).rgb *tex2D (_TexA, uv *-0.25).rgb * 4);		
			}
			else
			{
			col  += splat_control.a * tex2D (_TexA, uv).rgb;
			}
			//col = col / maxSplat;
			o.Albedo = col.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 0.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
