Shader "Unobserved/Daylight Sky"
{
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct v2f { float4 pos:SV_POSITION; float3 ray:TEXCOORD0; };
            v2f vert(float4 vertex:POSITION) { v2f o; o.pos=UnityObjectToClipPos(vertex); o.ray=vertex.xyz; return o; }
            half4 frag(v2f i):SV_Target
            {
                float3 direction=normalize(i.ray);
                half3 horizon=half3(.68,.78,.77),zenith=half3(.23,.46,.63);
                half3 sky=lerp(horizon,zenith,saturate(direction.y*.9));
                float sun=pow(saturate(dot(direction,normalize(float3(-.4,.45,.7)))),300);
                sky+=half3(1,.8,.48)*sun*.7;
                sky=lerp(half3(.3,.39,.32),sky,smoothstep(-.12,.025,direction.y));
                return half4(sky,1);
            }
            ENDCG
        }
    }
}
