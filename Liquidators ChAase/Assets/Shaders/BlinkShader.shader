// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/BlinkShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _BlinkColor ("Blink Color", Color) = (1,1,1,1)
        _Brightness ("Brightness", float) = 1.0
        _BlinkStrength("Blink Intensity", Range(0,1)) = 0
        _XFlip("X Flip", Range(0,1)) = 0
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Opaque" }

        LOD 100

        Pass
        {
            ZTest Off
            ZWrite Off
            Lighting Off
            Blend SrcAlpha OneMinusSrcAlpha
            
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

            fixed4 _MainColor;
            fixed4 _BlinkColor;
            float _BlinkStrength;
            float _Brightness;
            float _XFlip;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _MainColor * tex2D(_MainTex, i.uv);
                col = col * fixed4(_Brightness,_Brightness,_Brightness, 1);
                if (col.a <= 0) 
                    col = fixed4(0.0, 0.0, 0.0, 0.0);
                else
                    col = (col * (1.0 - _BlinkStrength)) + fixed4(_BlinkColor.r*_Brightness,_BlinkColor.g*_Brightness,_BlinkColor.b*_Brightness, col.a) * _BlinkStrength;
                return col;
            }

            ENDCG
        }
    }
}