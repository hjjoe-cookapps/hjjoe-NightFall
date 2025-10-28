Shader "Custom/Reddot" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _PulseSpeed("Pulse Speed", Range(0.1, 10)) = 1.0
        _ScaleSpeed("Scale Speed", Range(0, 10)) = 0.2
        _Scale("Scale", Range(0.1, 100)) = 0.5
        
         _StencilComp ("Stencil Comparison", Float) = 8
         _Stencil ("Stencil ID", Float) = 0
         _StencilOp ("Stencil Operation", Float) = 0
         _StencilWriteMask ("Stencil Write Mask", Float) = 255
         _StencilReadMask ("Stencil Read Mask", Float) = 255
         _ColorMask ("Color Mask", Float) = 15
    }

    SubShader{
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        
         Stencil
         {
             Ref [_Stencil]
             Comp [_StencilComp]
             Pass [_StencilOp] 
             ReadMask [_StencilReadMask]
             WriteMask [_StencilWriteMask]
         }
        
        Pass {

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float4 _Color;
            float _PulseSpeed;
            float _PulseAmount;
            float _ScaleSpeed;
            float _Scale;

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //float pulse = sin(_Time.y * _PulseSpeed) + 1.5;
                float pulse = _Time.y % _PulseSpeed / _PulseSpeed;
                float a = _ScaleSpeed * pulse;
                o.uv =  ((v.uv - 0.5) * (_Scale - a)  + 0.5);
                return o;  
            }

            fixed4 frag(v2f i) : SV_Target{
                float4 tex = tex2D(_MainTex, i.uv);
                float4 col = tex * _Color;
               // float pulse = cos (_Time.y * _PulseSpeed * 2) + 1;
                float pulse = (_Time.y % _PulseSpeed) / _PulseSpeed;
                col.a *= 1 - pulse;
                return col;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}