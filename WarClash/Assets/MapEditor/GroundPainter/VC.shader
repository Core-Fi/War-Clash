// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VC" {
	Properties{
		_Map("Map", 2D) = "white" {}
		_T1("Tex1", 2D) = "white" {}
		_T2("Tex2", 2D) = "white" {}
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_SizeX("Size X", float) = 8
		_SizeY("Size Y", float) = 8
	}
		SubShader{
		//"RenderQueue" = "Transparent"
		Tags{ "Queue" = "Geometry-100"
		"RenderType" = "Opaque"}
		LOD 300
	//	Blend SrcAlpha OneMinusSrcAlpha
	//	ZWrite Off
		//ZTest Always
		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv_MainTex : TEXCOORD0;
		float4 color:TEXCOORD1;
	};

	sampler2D _Map;
	sampler2D _T1;
	sampler2D _T2;
	float4 _Color;
	float _SizeX;
	float _SizeY;
	v2f vert(appdata_full v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv_MainTex = v.texcoord;//TRANSFORM_TEX(v.texcoord, _MainTex);
		o.color = v.color;
		return o;
	}
	sampler2D _MainTex;

	float2 Caculate(half g)
	{
		float y = 0.75 - floor(fmod(g, 4)) /4;
		float x = floor(g / 4)/8;
		float2 offsetuv = float2(x, y);
		return offsetuv;
	}
	half4 CaculateR(float2 scaledUv, float r, sampler2D t)
	{
		half g = round(r * 256);
		float2 offsetuv = Caculate(g);
		float2 newUv = scaledUv +offsetuv;
		half4 c = tex2D(t, newUv);
		return c;
	}
	
	float4 frag(v2f IN) : COLOR
	{
		half4 fc = half4(1,1,1,1);
		half4 map = tex2D(_Map, IN.uv_MainTex);
		float2 scaledUv = fmod(IN.uv_MainTex, float2(1/_SizeX, 1/_SizeY)) * float2(_SizeX /8, _SizeY/4);
		 half stepvalue = step( 0.001,map.r);
		half4 c = CaculateR(scaledUv, map.r, _T1);
		fc = lerp(fc, lerp(fc, c, c.a), stepvalue);

		stepvalue = step( 0.001,map.g);
		c = CaculateR(scaledUv, map.g, _T2);
		fc = lerp(fc, lerp(fc, c, c.a), stepvalue);
		return fc;
	}
		ENDCG
	}
	}
}
