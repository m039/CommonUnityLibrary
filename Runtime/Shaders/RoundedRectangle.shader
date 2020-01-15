Shader "Unlit/RoundedRectangle"
{
    Properties
    {
		_BorderSize ("Border Size", float) = 0
		_BorderRadius ("Border Radius", float) = 0
		_BorderColor ("Border Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { 
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
		}

        LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert

            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex	: POSITION;
                float2 uv		: TEXCOORD0;
				float4 color    : COLOR;
            };

            struct v2f
            {
                float2 uv		: TEXCOORD0;
                float4 vertex	: SV_POSITION;
				float4 color	: COLOR;
            };

			fixed4 _BorderColor;

			float _BorderSize;

			float _BorderRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				float alpha = 1;
				fixed4 color = i.color;
				fixed2 pAbs = abs(i.uv - 0.5);
				fixed2 pAbsBack = pAbs - _BorderRadius;
				fixed2 rightTopRadius = 0.5 - _BorderRadius;
				fixed2 rightTopBorder = 0.5 - _BorderSize;
				fixed2 rightTopBorderRadius = rightTopBorder - _BorderRadius;

				if (pAbs.x >= rightTopRadius.x &&
					pAbs.y >= rightTopRadius.y &&
					distance(pAbs, rightTopRadius) >= _BorderRadius)
				{
					float alpha = 0;
				}
				else if (pAbs.x >= rightTopBorder.x || 
						pAbs.y >= rightTopBorder.y)
				{
					color = _BorderColor;
				}
				else if (pAbs.x >= rightTopBorderRadius.x &&
					pAbs.y >= rightTopBorderRadius.y &&
					distance(pAbs, rightTopBorderRadius) >= _BorderRadius) {
					color = _BorderColor;
				}

                return fixed4(color.rgb, alpha);
            }
            ENDCG
        }
    }
}
