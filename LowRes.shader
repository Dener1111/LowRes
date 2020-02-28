Shader "PostEffect/LowRes"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Tags { "RenderPipeline" = "UniversalPipeline"}
        Pass
        {
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            sampler2D _MainTex;
            int _Height;
            int _Width;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f Vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 Frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv *= (_Height, _Width);
                uv = round(uv);
                uv /= (_Height, _Width);
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}