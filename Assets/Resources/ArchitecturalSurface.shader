Shader "Unobserved/Architectural Surface"
{
    Properties
    {
        _Color("Color",Color)=(.5,.5,.5,1)
        _EmissionColor("Emission",Color)=(0,0,0,0)
        _Glossiness("Smoothness",Range(0,1))=.25
        _Pattern("Concrete / tile / carpet / wood / metal",Float)=0
        _BaseMap("Albedo",2D)="white" {}
        _BumpMap("Normal",2D)="bump" {}
        _SurfaceMap("Occlusion / roughness / metal",2D)="white" {}
        _TextureScale("World texture scale",Float)=.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0
        struct Input { float3 worldPos; float3 worldNormal; INTERNAL_DATA };
        fixed4 _Color,_EmissionColor;
        float _Glossiness,_Pattern;
        sampler2D _BaseMap,_BumpMap,_SurfaceMap;
        float _TextureScale;
        float hash(float3 p) { return frac(sin(dot(p,float3(127.1,311.7,74.7)))*43758.5453); }
        void surf(Input i,inout SurfaceOutputStandard o)
        {
            float3 n=abs(WorldNormalVector(i,float3(0,0,1)));
            float2 uv=n.y>.6?i.worldPos.xz:n.x>.6?i.worldPos.zy:i.worldPos.xy;
            uv*=_TextureScale;
            if(_Pattern>4.5 && _Pattern<5.5) uv+=float2(_Time.y*.008,sin(_Time.y*.13)*.012);
            half3 base=tex2D(_BaseMap,uv).rgb;
            half3 surface=tex2D(_SurfaceMap,uv).rgb;
            o.Albedo=_Color.rgb*base*2.2;
            o.Normal=UnpackNormal(tex2D(_BumpMap,uv)); o.Normal.xy*=.5;
            o.Smoothness=saturate((1-surface.g)*.7+_Glossiness*.3); o.Metallic=surface.b;
            o.Occlusion=surface.r;
            o.Emission=_EmissionColor.rgb; o.Alpha=1;
        }
        ENDCG
    }
    Fallback "Standard"
}
