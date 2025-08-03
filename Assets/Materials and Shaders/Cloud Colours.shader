Shader "Custom/Cloud Colors" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color1 ("From Color 1", Color) = (1,1,1,1)
        _Replace1 ("To Color 1", Color) = (1,0,0,1)

        _Color2 ("From Color 2", Color) = (0.5,0.5,0.5,1)
        _Replace2 ("To Color 2", Color) = (0,1,0,1)

        _Color3 ("From Color 3", Color) = (0.2,0.2,0.2,1)
        _Replace3 ("To Color 3", Color) = (0,0,1,1)

        _Color4 ("From Color 4", Color) = (0.2,0.2,0.2,1)
        _Replace4 ("To Color 4", Color) = (0,0,1,1)
    }

    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _Color1, _Replace1;
            float4 _Color2, _Replace2;
            float4 _Color3, _Replace3;
            float4 _Color4, _Replace4;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);

                if (distance(col.rgb, _Color1.rgb) < 0.1)
                    col.rgb = _Replace1.rgb;
                else if (distance(col.rgb, _Color2.rgb) < 0.1)
                    col.rgb = _Replace2.rgb;
                else if (distance(col.rgb, _Color3.rgb) < 0.1)
                    col.rgb = _Replace3.rgb;
                else if (distance(col.rgb, _Color4.rgb) < 0.1)
                    col.rgb = _Replace4.rgb;

                return col;
            }
            ENDCG
        }
    }
}
