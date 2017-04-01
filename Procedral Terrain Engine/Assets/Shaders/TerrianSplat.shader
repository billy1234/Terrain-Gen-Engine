Shader "TerrainExtention/TerrianSplat" {
	Properties {
		_MainTex ("SplatMap (RGBA)", 2D) = "red" {}
		_TexR("SplatChannelR", 2D) = "white" {}
		_TexG("SplatChannelG", 2D) = "white" {}
		_TexB("SplatChannelB", 2D) = "white" {}
		_TexA("SplatChannelA)", 2D) = "white" {}
		[MaterialToggle]_NoTileR("NoTileRed",Float)=0
		[MaterialToggle]_NoTileG("NoTileGreen",Float)=0
		[MaterialToggle]_NoTileB("NoTileBlue",Float)=0
		[MaterialToggle]_NoTileA("NoTileAlpha",Float)=0
		
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_WorldSacle("WorldUnitScale",Float)=1
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
		fixed4 _TexR_ST, _TexG_ST, _TexB_ST, _TexA_ST; //scale and offet from the inspector
		half _NoTileR,_NoTileG,_NoTileB,_NoTileA;
		float _WorldScale;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		
		fixed4 getPixelFromSplat(float2 uv, sampler2D tex,bool mixUv)
		{

			fixed4 resultRaw;
			resultRaw.rgb = tex2D(tex, uv).rgb;
			fixed4 resultMixed;
			resultMixed.rgb = (tex2D(tex, uv).rgb + tex2D(tex, uv*-0.25).rgb) / 2;
			fixed4 weight = fixed4(mixUv, mixUv, mixUv, mixUv);
			return lerp(resultRaw,resultMixed, weight) * 0.7;
		}
		

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			float2 uv = float2(IN.worldPos.x, IN.worldPos.z);//+ IN.uv_MainTex;
			fixed4 splat_control = tex2D (_MainTex, IN.uv_MainTex);
			
			fixed4 col;
			col =	splat_control.r * getPixelFromSplat	(TRANSFORM_TEX(uv, _TexR) ,_TexR,_NoTileR);
			col +=	splat_control.g * getPixelFromSplat	(TRANSFORM_TEX(uv, _TexG),_TexG,_NoTileG);
			col +=	splat_control.b * getPixelFromSplat	(TRANSFORM_TEX(uv, _TexB),_TexB,_NoTileB);
			col +=	splat_control.a * getPixelFromSplat	(TRANSFORM_TEX(uv, _TexA),_TexA,_NoTileA);
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
