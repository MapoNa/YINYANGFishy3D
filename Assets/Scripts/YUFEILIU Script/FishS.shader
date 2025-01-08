Shader "Unlit/FishS"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SinMaxValue("SinMax", float) = 0.02
        _Frequency("Frequency", float) = 2
        _Size("Size",float)=2
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
            float _SinMaxValue;
            float _Frequency;
            float _Size;

            v2f vert (appdata v)
            {
                v2f o;
                
                // �ȳ�ʼ�� o.vertex
                o.vertex = UnityObjectToClipPos(v.vertex);

                // ����ƫ����
                float4 offset = float4(0.0, 0.0, 0.0, 0.0);
                offset.y = _SinMaxValue * sin(_Frequency * _Time.y + v.vertex.x*_Size);

                // Ӧ��ƫ�Ƶ�����λ��
                o.vertex = UnityObjectToClipPos(v.vertex + offset);

                // ���� UV
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // ��������������ɫ
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
