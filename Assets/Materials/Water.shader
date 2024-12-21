Shader "Unlit/Water"
{
    Properties
    {
        _NoiseTex ("NoiseTex", 2D) = "white" {}
        _BumpTex ("BumpTex", 2D) = "white" {}
        _DeepColor("DeepColor", Color) = (0,0,0,0)
        _ShallowColor("ShallowColor", Color) = (0,0,0,0)
        _RampDistance("RampDistance", float) = 0
        _Scale("TexScale",Vector)=(0,0,0,0)
        _NoiseCutOff("NoiceCutOff",float)=0
        _FoamThickness("FoamThickness",float)=0
        _FoamColor("FoamColor",Color)=(0,0,0,0)
        _Distortion("Distortion",float)=0
        _Visility("Visility",Range(0,1))=0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        GrabPass{"_GrabTexture"}
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float3 worldPos : TEXCOORD1; // 添加语义标记
            };
             
            sampler2D _NoiseTex, _CameraDepthTexture, _BumpTex,_GrabTexture;
            float4 _MainTex_ST, _DeepColor, _ShallowColor, _Scale,_FoamColor;
            float _RampDistance, _NoiseCutOff, _FoamThickness,_Distortion,_Visility;

            uniform sampler2D _RenderTexture;
            uniform float _OrthographicSize;
            uniform float4 _Position;



            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex); 
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; // 正确的世界坐标转换
                return o;
            }
            fixed4 BlendColor(fixed4 top, fixed4 bottom)
            {
                float3 col = top.rgb*top.a+bottom.rgb*(1-top.a);
                float a= top.a+bottom.a*(1-top.a);
                return fixed4(col,a);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 rtUv = i.worldPos.xz-_Position.xz;
                rtUv/=_OrthographicSize*2;
                rtUv+=0.5;
                float ripples = tex2D(_RenderTexture,rtUv).b;
                ripples = step(.7,ripples);
                
                float4 noise = tex2D(_NoiseTex,float2(i.worldPos.x*_Scale.x,i.worldPos.z) + _Time.x)+
                               tex2D(_NoiseTex,float2(i.worldPos.x,i.worldPos.z*_Scale.y) + _Time.x);
                float2 uv = i.screenPos.xy / i.screenPos.w;
                float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, uv));
                float waterDepthDifference = saturate((depth - i.screenPos.w) / _RampDistance);

                float foamDepthDifference = saturate((depth-i.screenPos.w)/ _FoamThickness);
                foamDepthDifference *= _NoiseCutOff;
                noise+=ripples;
                float foam = noise> foamDepthDifference ? 1 : 0;
                fixed4 foamColor = foam*_FoamColor;

                fixed3 bump= UnpackNormal(tex2D(_BumpTex,i.worldPos.xz+_Time.x));
                float2 offset = bump.xy*_Distortion;
                float4 screenPos = float4(i.screenPos.xy+offset, i.screenPos.zw);
                uv=screenPos.xy/screenPos.w;
                depth = LinearEyeDepth(tex2D(_CameraDepthTexture, uv));
                waterDepthDifference = saturate(depth - screenPos.w);
                offset *=  waterDepthDifference;
                screenPos = float4(i.screenPos.xy+offset, i.screenPos.zw);
                uv=screenPos.xy/screenPos.w;
                depth = LinearEyeDepth(tex2D(_CameraDepthTexture, uv));
                waterDepthDifference = saturate(depth - screenPos.w);
                fixed4 refrColor = tex2D(_GrabTexture,uv);
                fixed4 waterColor = lerp(_ShallowColor,_DeepColor,waterDepthDifference);
           
                waterColor=BlendColor(fixed4(waterColor.rgb,_Visility),refrColor);
                return  BlendColor(foamColor,waterColor);
            }
            ENDCG
        }
    }
}
