Shader "Custom/Outline"
{
	Properties
	{
		_Outline("Outline", Float) = 0.1
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		Cull front

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
	
			half _Outline;
			half4 _OutlineColor;

			struct vertexInput
			{
				float4 vertex: POSITION;
				float3 normal : NORMAL;
			};

			struct vertexOutput
			{
				float4 pos: SV_POSITION;
			};

			float4 CreateOutline(float4 vertPos, float Outline)
			{
				float4x4 scaleMat;
				scaleMat[0][0] = 1.0f + Outline;
				scaleMat[0][1] = 0.0f;
				scaleMat[0][2] = 0.0f;
				scaleMat[0][3] = 0.0f;

				scaleMat[1][0] = 0.0f;
				scaleMat[1][1] = 1.0f + Outline;
				scaleMat[1][2] = 0.0f;
				scaleMat[1][3] = 0.0f;
				
				scaleMat[2][0] = 0.0f;
				scaleMat[2][1] = 0.0f;
				scaleMat[2][2] = 1.0f + Outline;
				scaleMat[2][3] = 0.0f;
				
				scaleMat[3][0] = 0.0f;
				scaleMat[3][1] = 0.0f;
				scaleMat[3][2] = 0.0f;
				scaleMat[3][3] = 1.0f;

				return mul(scaleMat, vertPos);
			}

			vertexOutput vert(vertexInput v)
			{
				vertexOutput o;

				o.pos = UnityObjectToClipPos(CreateOutline(v.vertex, _Outline));

				return o;
			}

			half4 frag(vertexOutput i) : COLOR
			{
				return _OutlineColor;
			}

			ENDCG
		}
	}
}
