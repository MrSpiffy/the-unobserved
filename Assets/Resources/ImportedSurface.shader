Shader "Unobserved/Imported Surface"
{
 Properties {
  _Color("Facility tint",Color)=(1,1,1,1)
  _MainTex("Source albedo",2D)="white" {}
  _BumpMap("Source normal",2D)="bump" {}
  _ORM("Source occlusion roughness metal",2D)="white" {}
  _UseORM("Use packed map",Float)=0
  _Saturation("Source saturation",Range(0,1))=.35
  _Smoothness("Smoothness",Range(0,1))=.28
  _Metallic("Metal",Range(0,1))=0
  _EmissionColor("Emission",Color)=(0,0,0,0)
 }
 SubShader {
  Tags {"RenderType"="Opaque"}
  CGPROGRAM
  #pragma surface surf Standard fullforwardshadows
  #pragma target 3.0
  sampler2D _MainTex,_BumpMap,_ORM;
  fixed4 _Color,_EmissionColor; half _UseORM,_Saturation,_Smoothness,_Metallic;
  struct Input {float2 uv_MainTex; float2 uv_BumpMap;};
  void surf(Input i,inout SurfaceOutputStandard o) {
   half3 c=tex2D(_MainTex,i.uv_MainTex).rgb; half l=dot(c,half3(.2126,.7152,.0722));
   o.Albedo=lerp(l.xxx,c,_Saturation)*_Color.rgb;
   o.Normal=UnpackNormal(tex2D(_BumpMap,i.uv_BumpMap));
   half3 orm=tex2D(_ORM,i.uv_MainTex).rgb;
   o.Occlusion=lerp(1,orm.r,_UseORM); o.Smoothness=lerp(_Smoothness,(1-orm.g)*.6,_UseORM); o.Metallic=lerp(_Metallic,orm.b*.65,_UseORM);
   o.Emission=_EmissionColor.rgb; o.Alpha=1;
  }
  ENDCG
 }
 Fallback "Standard"
}
