Shader "Custom/UIBlur"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 col = fixed4(0,0,0,0);
                int samples = 8;
                float blur = 0.002f; // ‚Ú‚©‚µ‚Ì”ÍˆÍ
                for(int x = -samples; x <= samples; x++)
                {
                    for(int y = -samples; y <= samples; y++)
                    {
                        float2 sampleUV = uv + float2(x, y) * blur;
                        col += tex2D(_MainTex, sampleUV);
                    }
                }
                col /= (samples * 2 + 1) * (samples * 2 + 1);
                return col;
            }
            ENDCG
        }
    }
}
