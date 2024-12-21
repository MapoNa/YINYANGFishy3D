Shader "Unlit/YinYangFish"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1, 1, 1, 1)
        _BlackWhiteFactor ("Black White Factor", Range(0, 1)) = 0
        _SinMaxValue("SinMax", float) = 0.02
        _Frequency("Frequency", float) = 2
        _Size("Size", float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _BlackWhiteFactor;
            float _SinMaxValue;
            float _Frequency;
            float _Size;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);

                // 添加顶点动画偏移
                float4 offset = float4(0.0, 0.0, 0.0, 0.0);
                offset.y = _SinMaxValue * sin(_Frequency * _Time.y + v.vertex.x * _Size);
                o.vertex = UnityObjectToClipPos(v.vertex + offset);

                // 纹理坐标
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 获取基础颜色
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                // 应用黑白效果
                float luminance = dot(col.rgb, float3(0.299, 0.587, 0.114));
                col.rgb = lerp(col.rgb, float3(luminance, luminance, luminance), _BlackWhiteFactor);

                return col;
            }
            ENDCG
        }
    }
}
