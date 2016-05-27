Shader "TerrainExtention/Terrian" {
	Properties {
		_MainTex ("SplatMap (RGB)", 2D) = "black" {}
		_TexR("SplatR (RGB)", 2D) = "white" {}
		_TexG("SplatG (RGB)", 2D) = "white" {}
		_TexB("SplatB (RGB)", 2D) = "white" {}
		_TexA("SplatA (RGB)", 2D) = "white" {}
		[MaterialToggle]_NoTileR("NoTileRed",Float)=0
		[MaterialToggle]_NoTileG("NoTileGreen",Float)=0
		[MaterialToggle]_NoTileB("NoTileBlue",Float)=0
		[MaterialToggle]_NoTileA("NoTileAlpha",Float)=0
		
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

		
		fixed4 getPixelFromSplat(float2 uv, sampler2D tex,bool mixUv)
		{
			if(mixUv)//mix uv coordinates
			{
				fixed4 value;
				value.rgb = (tex2D(tex,uv).rgb + tex2D(tex,uv*-0.25).rgb) /2;
				return value;
			}
			else
			{
				fixed4 value;
				value.rgb = tex2D(tex,uv).rgb;
				return value;
			}
		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			float2 uv = float2(IN.worldPos.x,IN.worldPos.z) + IN.uv_MainTex;
			fixed4 splat_control = tex2D (_MainTex, IN.uv_MainTex);
			
			fixed3 col;
			col = splat_control.r * getPixelFromSplat(uv,_TexR,_NoTileR !=0);
			col += splat_control.g * getPixelFromSplat(uv,_TexG,_NoTileG !=0);
			col += splat_control.b * getPixelFromSplat(uv,_TexB,_NoTileB !=0);
			col += splat_control.a * getPixelFromSplat(uv,_TexA,_NoTileA !=0);
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
