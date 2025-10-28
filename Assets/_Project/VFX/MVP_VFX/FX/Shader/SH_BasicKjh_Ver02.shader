// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SH_BasicKjh_Ver02"
{
	Properties
	{
		_Emission("Emission", Float) = 1
		_Opacity("Opacity", Float) = 1
		_MainTX("MainTX", 2D) = "white" {}
		_MainOpaPow("MainOpaPow", Float) = 1
		[Toggle(_RDIST_ON)] _Rdist("[R]dist", Float) = 0
		[Toggle(_GSTEP_ON)] _Gstep("[G]step", Float) = 0
		[Toggle(_BMASK_ON)] _Bmask("[B]mask", Float) = 0
		[Toggle(_ACEIL_ON)] _Aceil("[A]ceil", Float) = 0
		_SubTX("SubTX", 2D) = "white" {}
		_XYspeedZWceilStrength("XY=speed, ZW=ceilStrength", Vector) = (0,0,0,0)
		_distpivotrelocation("dist pivot relocation", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.5
		#pragma shader_feature_local _RDIST_ON
		#pragma shader_feature_local _ACEIL_ON
		#pragma shader_feature_local _BMASK_ON
		#pragma shader_feature_local _GSTEP_ON
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float4 uv2_tex4coord2;
		};

		uniform sampler2D _MainTX;
		uniform float4 _MainTX_ST;
		uniform sampler2D _SubTX;
		uniform float4 _XYspeedZWceilStrength;
		uniform float4 _SubTX_ST;
		uniform float _distpivotrelocation;
		uniform float _Emission;
		uniform float _MainOpaPow;
		uniform float _Opacity;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTX = i.uv_texcoord * _MainTX_ST.xy + _MainTX_ST.zw;
			float2 appendResult11 = (float2(i.uv2_tex4coord2.z , i.uv2_tex4coord2.w));
			float2 appendResult40 = (float2(_XYspeedZWceilStrength.x , _XYspeedZWceilStrength.y));
			float2 uv_SubTX = i.uv_texcoord * _SubTX_ST.xy + _SubTX_ST.zw;
			float2 panner41 = ( 1.0 * _Time.y * appendResult40 + uv_SubTX);
			#ifdef _RDIST_ON
				float staticSwitch65 = ( i.uv2_tex4coord2.x * ( tex2D( _SubTX, panner41 ).r - _distpivotrelocation ) );
			#else
				float staticSwitch65 = 0.0;
			#endif
			float clampResult27 = clamp( ( tex2D( _SubTX, panner41 ).a + _XYspeedZWceilStrength.z ) , 0.0 , 1.0 );
			#ifdef _ACEIL_ON
				float staticSwitch71 = ( ceil( ( clampResult27 * _XYspeedZWceilStrength.w ) ) / _XYspeedZWceilStrength.w );
			#else
				float staticSwitch71 = 1.0;
			#endif
			#ifdef _BMASK_ON
				float staticSwitch69 = tex2D( _SubTX, uv_SubTX ).b;
			#else
				float staticSwitch69 = 1.0;
			#endif
			float4 temp_output_58_0 = ( ( tex2D( _MainTX, ( ( uv_MainTX + appendResult11 ) + staticSwitch65 ) ) * staticSwitch71 ) * staticSwitch69 );
			o.Emission = ( float4( (i.vertexColor).rgb , 0.0 ) * ( temp_output_58_0 * _Emission ) ).rgb;
			float4 temp_cast_2 = (_MainOpaPow).xxxx;
			#ifdef _GSTEP_ON
				float staticSwitch67 = step( i.uv2_tex4coord2.y , tex2D( _SubTX, panner41 ).g );
			#else
				float staticSwitch67 = 1.0;
			#endif
			o.Alpha = ( i.vertexColor.a * ( ( (pow( temp_output_58_0 , temp_cast_2 )).r * staticSwitch67 ) * _Opacity ) );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Unlit alpha:fade keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.5
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 customPack2 : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyzw = customInputData.uv2_tex4coord2;
				o.customPack2.xyzw = v.texcoord1;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.uv2_tex4coord2 = IN.customPack2.xyzw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.vertexColor = IN.color;
				SurfaceOutput o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutput, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18707
-22.4;-13.6;2048;1085.4;1760.472;116.7257;1.514942;True;False
Node;AmplifyShaderEditor.Vector4Node;74;-4722.423,1313.046;Inherit;False;Property;_XYspeedZWceilStrength;XY=speed, ZW=ceilStrength;9;0;Create;True;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;40;-4357.793,1251.794;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;33;-4456.773,1493.626;Inherit;False;0;73;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;73;-4755.266,1010.615;Inherit;True;Property;_SubTX;SubTX;8;0;Create;True;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.PannerNode;41;-4011.519,1462.872;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;24;-3406.499,1010.212;Inherit;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;75;-3941.405,1102.45;Inherit;False;Property;_distpivotrelocation;dist pivot relocation;10;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;37;-3785.702,726.2498;Inherit;True;Property;_TextureSample1;Texture Sample 1;5;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-3059.651,1036.053;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;9;-3232.153,272.2994;Inherit;False;1;4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;42;-3434.272,783.5681;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;27;-2886.499,1035.053;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-2693.358,1034.915;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-3092.104,19.35452;Inherit;False;0;7;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-3214.888,759.8856;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;11;-2860.448,277.9387;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;68;-2820.331,538.5604;Inherit;False;Constant;_Float1;Float 1;9;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;65;-2626.017,523.9468;Inherit;False;Property;_Rdist;[R]dist;4;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-2629.812,164.5109;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CeilOpNode;29;-2521.962,1033.542;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;13;-2338.94,314.1893;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;66;-2146.963,790.8671;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;30;-2326.884,1027.437;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-2085.161,112.2752;Inherit;True;Property;_MainTX;MainTX;2;0;Create;True;0;0;False;0;False;-1;None;667d0f17c70a23b4c9185bf0afb82a1b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;71;-1862.708,719.6296;Inherit;False;Property;_Aceil;[A]ceil;7;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;55;-2855.427,1752.528;Inherit;True;Property;_TextureSample3;Texture Sample 3;5;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;70;-1977.745,1459.861;Inherit;False;Constant;_Float2;Float 2;10;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;69;-1786.754,1500.446;Inherit;False;Property;_Bmask;[B]mask;6;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-1518.647,399.2264;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-1201.035,438.9421;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1221.169,853.7595;Inherit;False;Property;_MainOpaPow;MainOpaPow;3;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;45;-3231.517,1384.907;Inherit;True;Property;_TextureSample2;Texture Sample 2;5;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;72;-1925.253,1118.661;Inherit;False;Constant;_Float3;Float 3;11;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;15;-937.5829,723.8076;Inherit;False;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;52;-2122.86,1296.849;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;22;-752.9967,737.9142;Inherit;False;True;False;False;False;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;67;-1714.714,1141.442;Inherit;False;Property;_Gstep;[G]step;5;0;Create;True;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-663.2311,534.766;Inherit;False;Property;_Emission;Emission;0;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;16;-794.0172,8.590806;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;6;-434.119,1153.255;Inherit;False;Property;_Opacity;Opacity;1;0;Create;True;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-490.9695,820.4089;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;19;-455.8172,-13.00917;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-446.437,370.2844;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-178.5994,823.4193;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;321.9759,589.2719;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-38.81714,109.5908;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;974.8192,128.6689;Float;False;True;-1;3;ASEMaterialInspector;0;0;Unlit;SH_BasicKjh_Ver02;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;40;0;74;1
WireConnection;40;1;74;2
WireConnection;41;0;33;0
WireConnection;41;2;40;0
WireConnection;24;0;73;0
WireConnection;24;1;41;0
WireConnection;37;0;73;0
WireConnection;37;1;41;0
WireConnection;26;0;24;4
WireConnection;26;1;74;3
WireConnection;42;0;37;1
WireConnection;42;1;75;0
WireConnection;27;0;26;0
WireConnection;28;0;27;0
WireConnection;28;1;74;4
WireConnection;43;0;9;1
WireConnection;43;1;42;0
WireConnection;11;0;9;3
WireConnection;11;1;9;4
WireConnection;65;1;68;0
WireConnection;65;0;43;0
WireConnection;10;0;3;0
WireConnection;10;1;11;0
WireConnection;29;0;28;0
WireConnection;13;0;10;0
WireConnection;13;1;65;0
WireConnection;30;0;29;0
WireConnection;30;1;74;4
WireConnection;7;1;13;0
WireConnection;71;1;66;0
WireConnection;71;0;30;0
WireConnection;55;0;73;0
WireConnection;69;1;70;0
WireConnection;69;0;55;3
WireConnection;36;0;7;0
WireConnection;36;1;71;0
WireConnection;58;0;36;0
WireConnection;58;1;69;0
WireConnection;45;0;73;0
WireConnection;45;1;41;0
WireConnection;15;0;58;0
WireConnection;15;1;14;0
WireConnection;52;0;9;2
WireConnection;52;1;45;2
WireConnection;22;0;15;0
WireConnection;67;1;72;0
WireConnection;67;0;52;0
WireConnection;54;0;22;0
WireConnection;54;1;67;0
WireConnection;19;0;16;0
WireConnection;20;0;58;0
WireConnection;20;1;4;0
WireConnection;21;0;54;0
WireConnection;21;1;6;0
WireConnection;18;0;16;4
WireConnection;18;1;21;0
WireConnection;17;0;19;0
WireConnection;17;1;20;0
WireConnection;0;2;17;0
WireConnection;0;9;18;0
ASEEND*/
//CHKSM=FA33C55F1D8F67667C2E937AFDEF6F3A9C00032F