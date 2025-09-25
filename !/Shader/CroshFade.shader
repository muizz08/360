Shader "Custom/CrossFadeShader"
{
    Properties
    {
        _Texture_A ("Texture A", 2D) = "white" {}
        _Texture_B ("Texture B", 2D) = "white" {}
        _Blend ("Blend", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        
        // Memastikan tidak ada bagian yang tersembunyi
        Cull Back 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _Texture_A;
            sampler2D _Texture_B;
            float _Blend;
            float4 _Texture_A_ST;

            // Untuk mengambil UV yang tepat dari tekstur
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Texture_A);
                return o;
            }

            // Fragment shader
            fixed4 frag (v2f i) : SV_Target
            {
                
                fixed4 colA = tex2D(_Texture_A, i.uv);
                fixed4 colB = tex2D(_Texture_B, i.uv);
                
                fixed4 blendedColor = lerp(colA, colB, _Blend);
                return blendedColor;
            }
            ENDCG
        }
    }
}