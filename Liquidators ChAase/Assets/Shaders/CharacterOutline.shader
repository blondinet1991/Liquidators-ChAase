Shader "Unlit/CharactersShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0.0,16.0)) = 3.0
        _BlinkColor ("Blink Color", Color) = (1,1,1,1)
        _Brightness ("Brightness", Range(0,1)) = 1.0
        _BlinkStrength("Blink Intensity", Range(0,1)) = 0
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
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;

            fixed4 _MainColor;
            fixed4 _BlinkColor;
            fixed4 _OutlineColor;

            float _OutlineWidth;
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

                fixed2 size = fixed2(_OutlineWidth,_OutlineWidth) / _MainTex_TexelSize.zw;
                // fixed2 size = fixed2(_OutlineWidth,_OutlineWidth) / fixed2(400, 574);
   
                float alpha = col.a;
                alpha += tex2D(_MainTex, i.uv + fixed2(0.0, -size.y)).a;
                alpha += tex2D(_MainTex, i.uv + fixed2(size.x, -size.y)).a;
                alpha += tex2D(_MainTex, i.uv + fixed2(size.x, 0.0)).a;
                alpha += tex2D(_MainTex, i.uv + fixed2(size.x, size.y)).a;
                alpha += tex2D(_MainTex, i.uv + fixed2(0.0, size.y)).a;
                alpha += tex2D(_MainTex, i.uv + fixed2(-size.x, size.y)).a;
                alpha += tex2D(_MainTex, i.uv + fixed2(-size.x, 0.0)).a;
                alpha += tex2D(_MainTex, i.uv + fixed2(-size.x, -size.y)).a;
            
                fixed3 final_color = lerp(_OutlineColor.rgb, col.rgb, col.a);
                col = fixed4(final_color, clamp(alpha, 0.0, 1.0));
                
                return col;
            }

            ENDCG
        }
    }
}
