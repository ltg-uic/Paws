Shader "SnowInt"
{
	Properties 
	{
_MainTex("_MainTex", 2D) = "black" {}
_SnowTex("_SnowTex", 2D) = "black" {}
_MainNorm("_MainNorm", 2D) = "black" {}
_SnowNorm("_SnowNorm", 2D) = "black" {}
_Accumulation("_Accumulation", Float) = 1

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="False"
"RenderType"="Opaque"

		}

		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Fog{
}


		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 2.0


sampler2D _MainTex;
sampler2D _SnowTex;
sampler2D _MainNorm;
sampler2D _SnowNorm;
float _Accumulation;

			struct EditorSurfaceOutput {
				half3 Albedo;
				half3 Normal;
				half3 Emission;
				half3 Gloss;
				half Specular;
				half Alpha;
				half4 Custom;
			};
			
			inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

			}

			inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
			{
				half3 h = normalize (lightDir + viewDir);
				
				half diff = max (0, dot ( lightDir, s.Normal ));
				
				float nh = max (0, dot (s.Normal, h));
				float spec = pow (nh, s.Specular*128.0);
				
				half4 res;
				res.rgb = _LightColor0.rgb * diff;
				res.w = spec * Luminance (_LightColor0.rgb);
				res *= atten * 2.0;

				return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
			struct Input {
				float2 uv_MainTex;
float2 uv_SnowTex;
float3 sWorldNormal;
float2 uv_MainNorm;
float2 uv_SnowNorm;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);

o.sWorldNormal = mul((float3x3)_Object2World, SCALED_NORMAL);

			}
			

			void surf (Input IN, inout EditorSurfaceOutput o) {
				o.Normal = float3(0.0,0.0,1.0);
				o.Alpha = 1.0;
				o.Albedo = 0.0;
				o.Emission = 0.0;
				o.Gloss = 0.0;
				o.Specular = 0.0;
				o.Custom = 0.0;
				
float4 Sampled2D0=tex2D(_MainTex,IN.uv_MainTex.xy);
float4 Sampled2D1=tex2D(_SnowTex,IN.uv_SnowTex.xy);
float4 Dot0=dot( float4( 0,1,0,0).xyz, float4( IN.sWorldNormal.x, IN.sWorldNormal.y,IN.sWorldNormal.z,1.0 ).xyz ).xxxx;
float4 Multiply0=Dot0 * _Accumulation.xxxx;
float4 Step0=step(Sampled2D1.aaaa,Multiply0);
float4 Lerp1=lerp(Sampled2D0,Sampled2D1,Step0);
float4 Tex2DNormal0=float4(UnpackNormal( tex2D(_MainNorm,(IN.uv_MainNorm.xyxy).xy)).xyz, 1.0 );
float4 Tex2DNormal1=float4(UnpackNormal( tex2D(_SnowNorm,(IN.uv_SnowNorm.xyxy).xy)).xyz, 1.0 );
float4 Lerp0=lerp(Tex2DNormal0,Tex2DNormal1,Step0);
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_3_NoInput = float4(0,0,0,0);
float4 Master0_4_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Lerp1;
o.Normal = Lerp0;

				o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback "Diffuse"
}