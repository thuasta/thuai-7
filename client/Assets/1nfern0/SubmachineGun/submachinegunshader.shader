//#EditorFriendly
//#node13:posx=-463.5:posy=283:title=Texture:title2=Normal:input0=(0,0):input0type=Vector2:
//#node12:posx=-459.5:posy=147:title=Texture:title2=Specular:input0=(0,0):input0type=Vector2:
//#node11:posx=-464.5:posy=-76:title=Texture:title2=Diffuse:input0=(0,0):input0type=Vector2:
//#node10:posx=-192.5:posy=-19:title=Multiply:input0=1:input0type=float:input0linkindexnode=11:input0linkindexoutput=0:input1=1:input1type=float:input1linkindexnode=9:input1linkindexoutput=0:
//#node9:posx=-344.5:posy=3:title=ParamFloat:title2=Ambient:input0=1.2:input0type=float:
//#node8:posx=-342.5:posy=214:title=ParamFloat:title2=Spec_intensity:input0=1.3:input0type=float:
//#node7:posx=-186.5:posy=125:title=Multiply:input0=1:input0type=float:input0linkindexnode=12:input0linkindexoutput=0:input1=1:input1type=float:input1linkindexnode=8:input1linkindexoutput=0:
//#node6:posx=-218.5:posy=286:title=UnpackNormal:input0=0:input0type=float:input0linkindexnode=13:input0linkindexoutput=0:
//#node5:posx=-337.5:posy=82:title=ParamFloat:title2=Gloss:input0=0.25:input0type=float:
//#node4:posx=0:posy=0:title=Lighting:title2=On:
//#node3:posx=0:posy=0:title=DoubleSided:title2=Back:
//#node2:posx=0:posy=0:title=FallbackInfo:title2=Transparent/Cutout/VertexLit:input0=1:input0type=float:
//#node1:posx=0:posy=0:title=LODInfo:title2=LODInfo1:input0=600:input0type=float:
//#masterNode:posx=0:posy=0:title=Master Node:input0linkindexnode=10:input0linkindexoutput=0:input2linkindexnode=7:input2linkindexoutput=0:input3linkindexnode=5:input3linkindexoutput=0:input5linkindexnode=6:input5linkindexoutput=0:
//#sm=3.0
//#blending=Normal
//#ShaderName
Shader "ShaderFusion/submachinegunshader" {
	Properties {
_Color ("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
_SpecColor ("Specular Color", Color) = (1.0, 1.0, 1.0, 1.0)
_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
//#ShaderProperties
_Diffuse ("Diffuse", 2D) = "white" {}
_Ambient ("Ambient", Float) = 1.2
_Specular ("Specular", 2D) = "white" {}
_Spec_intensity ("Spec_intensity", Float) = 1.3
_Gloss ("Gloss", Float) = 0.25
_Normal ("Normal", 2D) = "white" {}
	}
	Category {
		SubShader { 
//#Blend
ZWrite On
//#CatTags
Tags { "RenderType"="Opaque" }
Lighting On
Cull Back
//#LOD
LOD 600
//#GrabPass
		CGPROGRAM
//#LightingModelTag
#pragma surface surf ShaderFusion vertex:vert alphatest:_Cutoff
 //use custom lighting functions
 
 //custom surface output structure
 struct SurfaceShaderFusion {
	half3 Albedo;
	half3 Normal;
	half3 Emission;
	half Specular;
	half3 GlossColor; //Gloss is now three-channel
	half Alpha;
 };
inline fixed4 LightingShaderFusion (SurfaceShaderFusion s, fixed3 lightDir, half3 viewDir, fixed atten)
{
	half3 h = normalize (lightDir + viewDir);
	
	fixed diff = max (0, dot (s.Normal, lightDir));
	
	float nh = max (0, dot (s.Normal, h));
	float3 spec = pow (nh, s.Specular*128.0) * s.GlossColor;
	
	fixed4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
	c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
	return c;
}
inline fixed4 LightingShaderFusion_PrePass (SurfaceShaderFusion s, half4 light)
{
	fixed3 spec = light.a * s.GlossColor;
	
	fixed4 c;
	c.rgb = (s.Albedo * light.rgb + light.rgb * _SpecColor.rgb * spec);
	c.a = s.Alpha + spec * _SpecColor.a;
	return c;
}
inline half4 LightingShaderFusion_DirLightmap (SurfaceShaderFusion s, fixed4 color, fixed4 scale, half3 viewDir, bool surfFuncWritesNormal, out half3 specColor)
{
	UNITY_DIRBASIS
	half3 scalePerBasisVector;
	
	half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector);
	
	half3 lightDir = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
	half3 h = normalize (lightDir + viewDir);
	float nh = max (0, dot (s.Normal, h));
	float spec = pow (nh, s.Specular * 128.0);
	
	// specColor used outside in the forward path, compiled out in prepass
	specColor = lm * _SpecColor.rgb * s.GlossColor * spec;
	
	// spec from the alpha component is used to calculate specular
	// in the Lighting*_Prepass function, it's not used in forward
	return half4(lm, spec);
}
//#TargetSM
#pragma target 3.0
//#UnlitCGDefs
sampler2D _Diffuse;
float _Ambient;
sampler2D _Specular;
float _Spec_intensity;
float _Gloss;
sampler2D _Normal;
float4 _Color;
		struct Input {
//#UVDefs
float2 sfuv1;
		INTERNAL_DATA
		};
		
		void vert (inout appdata_full v, out Input o) {
//#DeferredVertexBody
o.sfuv1 = v.texcoord.xy;
//#DeferredVertexEnd
		}
		void surf (Input IN, inout SurfaceShaderFusion o) {
			float4 normal = float4(0.0,0.0,1.0,0.0);
			float3 emissive = 0.0;
			float3 specular = 1.0;
			float gloss = 1.0;
			float3 diffuse = 1.0;
			float alpha = 1.0;
//#PreFragBody
float4 node11 = tex2D(_Diffuse,IN.sfuv1);
float4 node12 = tex2D(_Specular,IN.sfuv1);
float4 node13 = tex2D(_Normal,IN.sfuv1);
//#FragBody
normal = (float4(float3(UnpackNormal((node13))),0));
gloss = (_Gloss);
specular = ((node12) * (_Spec_intensity));
diffuse = ((node11) * (_Ambient));
			
			o.Albedo = diffuse.rgb*_Color;
			#ifdef SHADER_API_OPENGL
			o.Albedo = max(float3(0,0,0),o.Albedo);
			#endif
			
			o.Emission = emissive*_Color;
			#ifdef SHADER_API_OPENGL
			o.Emission = max(float3(0,0,0),o.Emission);
			#endif
			
			o.GlossColor = specular*_SpecColor;
			#ifdef SHADER_API_OPENGL
			o.GlossColor = max(float3(0,0,0),o.GlossColor);
			#endif
			
			o.Alpha = alpha*_Color.a;
			#ifdef SHADER_API_OPENGL
			o.Alpha = max(float3(0,0,0),o.Alpha);
			#endif
			
			o.Specular = gloss;
			#ifdef SHADER_API_OPENGL
			o.Specular = max(float3(0,0,0),o.Specular);
			#endif
			
			o.Normal = normal;
//#FragEnd
		}
		ENDCG
		}
	}
//#Fallback
Fallback "Transparent/Cutout/VertexLit"
}
