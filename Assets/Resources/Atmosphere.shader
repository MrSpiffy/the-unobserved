Shader "Unobserved/Atmosphere"
{
    Properties { _MainTex ("Source", 2D) = "white" {} }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex,_Bloom;
            fixed4 frag(v2f_img i) : SV_Target
            {
                float3 color=tex2D(_MainTex,i.uv).rgb+tex2D(_Bloom,i.uv).rgb*.35;
                color=color/(1+color*.13);
                float2 p=i.uv*2-1;
                float vignette=1-.16*dot(p,p);
                float grain=frac(sin(dot(i.uv*_ScreenParams.xy,float2(12.9898,78.233)))*43758.5453)-.5;
                color=color*vignette+grain*.006;
                return float4(color,1);
            }
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            half4 frag(v2f_img i):SV_Target { half3 c=tex2D(_MainTex,i.uv).rgb; return half4(max(0,c-.75),1); }
            ENDCG
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"
            sampler2D _MainTex;
            float2 _BlurDirection;
            half4 frag(v2f_img i):SV_Target
            {
                half3 c=tex2D(_MainTex,i.uv).rgb*.227;
                c+=(tex2D(_MainTex,i.uv+_BlurDirection*1.385).rgb+tex2D(_MainTex,i.uv-_BlurDirection*1.385).rgb)*.316;
                c+=(tex2D(_MainTex,i.uv+_BlurDirection*3.23).rgb+tex2D(_MainTex,i.uv-_BlurDirection*3.23).rgb)*.0705;
                return half4(c,1);
            }
            ENDCG
        }
    }
}
