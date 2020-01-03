Shader "Unlit/Circle"
{
    Properties
    {
    	// Nothing
    	_BorderSize ("Border Size", float) = 0
    	[Toggle(USE_SOLID)] _Solid ("Solid", float)  = 0
    }
    SubShader
    {
        Tags {
        	"Queue"="Transparent" 
        	"RenderType"="Transparent" 
    	}


        LOD 100


        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature USE_SOLID
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex 	: POSITION;
                float2 uv 		: TEXCOORD0;
                float4 color    : COLOR;
            };

            struct v2f
            {
                float2 uv 		: TEXCOORD0;
                float4 vertex 	: SV_POSITION;
                float4 color	: COLOR;
            };

            float _BorderSize;
                        
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float alpha = 1;
				float2 c = float2(0.0, 0.0);
				float2 p = float2(i.uv.x - 0.5, i.uv.y - 0.5);
				float d = distance(c, p);
				
                if (d > 0.5) {
                	alpha = 0;
                } else if (d > 0.5 - _BorderSize) {
                	alpha = 1;
                } else {
                #ifndef USE_SOLID
                	alpha = 0;
                #endif
                }

                return fixed4(i.color.rgb * i.color.a * alpha, alpha * i.color.a);
            }
            ENDCG
        }
    }
}
