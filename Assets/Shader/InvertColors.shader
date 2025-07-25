Shader "Custom/InvertColors"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Intensity ("Intensity", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { 
            "RenderType" = "Opaque"
            "Queue" = "Overlay"  // 确保在其他渲染之后执行
        }
        LOD 100

        Pass
        {
            ZTest Always  // 禁用深度测试
            Cull Off      // 禁用面剔除
            ZWrite Off    // 禁用深度写入
            
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

            sampler2D _MainTex;
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // 优化：使用saturate确保颜色值在合法范围内
                col.rgb = saturate(lerp(col.rgb, 1.0 - col.rgb, _Intensity));
                return col;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
}