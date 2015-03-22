using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PostProcessFX
{
	class AntiAliasingEffect : MonoBehaviour
	{
		public AntialiasingAsPostEffect AAComponent;
		
		public AntiAliasingEffect()
		{
			AAComponent = Camera.main.gameObject.AddComponent<AntialiasingAsPostEffect>();
			if (AAComponent == null)
			{
				Debug.LogError("AntiAliasingEffect: Could not add AntialiasingAsPostEffect to Camera.");
			}
			else
			{
				Material dlaaMaterial = new Material(dlaaShaderText);
				Material nfaaMaterial = new Material(nfaaShaderText);
				Material fxaa2Material = new Material(fxaa2ShaderText);
				Material fxaa3ConsoleMaterial = new Material(fxaa3ConsoleShaderText);
				Material fxaaPreset2Material = new Material(fxaaPreset2ShaderText);
				Material fxaaPreset3Material = new Material(fxaaPreset3ShaderText);
				Material ssaaMaterial = new Material(ssaaShaderText);

				AAComponent.mode = AAMode.NFAA;
				AAComponent.nfaaShader = nfaaMaterial.shader;
				AAComponent.dlaaShader = dlaaMaterial.shader;
				AAComponent.shaderFXAAII = fxaa2Material.shader;
				AAComponent.shaderFXAAIII = fxaa3ConsoleMaterial.shader;
				AAComponent.shaderFXAAPreset2 = fxaaPreset2Material.shader;
				AAComponent.shaderFXAAPreset3 = fxaaPreset3Material.shader;
				AAComponent.ssaaShader = ssaaMaterial.shader;
				AAComponent.enabled = true;
			}
		}

		private const String fxaaPreset3ShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 25.3KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/FXAA Preset 3"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = ""white"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 191 math, 12 texture, 26 branch
 // Stats for Fragment shader:
 //       d3d11 : 108 math, 8 branch
 //        d3d9 : 147 math, 24 texture, 12 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 46749
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 191 math, 12 textures, 26 branches
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
#extension GL_ARB_shader_texture_lod : enable
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec2 rcpFrame_1;
  rcpFrame_1 = _MainTex_TexelSize.xy;
  vec3 tmpvar_2;
  bool doneP_4;
  bool doneN_5;
  float lumaEndP_6;
  float lumaEndN_7;
  vec2 offNP_8;
  vec2 posP_9;
  vec2 posN_10;
  float gradientN_11;
  float lengthSign_12;
  float lumaS_13;
  float lumaN_14;
  vec4 tmpvar_15;
  tmpvar_15.zw = vec2(0.0, 0.0);
  tmpvar_15.xy = (xlv_TEXCOORD0 + (vec2(0.0, -1.0) * _MainTex_TexelSize.xy));
  vec4 tmpvar_16;
  tmpvar_16 = texture2DLod (_MainTex, tmpvar_15.xy, 0.0);
  vec4 tmpvar_17;
  tmpvar_17.zw = vec2(0.0, 0.0);
  tmpvar_17.xy = (xlv_TEXCOORD0 + (vec2(-1.0, 0.0) * _MainTex_TexelSize.xy));
  vec4 tmpvar_18;
  tmpvar_18 = texture2DLod (_MainTex, tmpvar_17.xy, 0.0);
  vec4 tmpvar_19;
  tmpvar_19 = texture2DLod (_MainTex, xlv_TEXCOORD0, 0.0);
  vec4 tmpvar_20;
  tmpvar_20.zw = vec2(0.0, 0.0);
  tmpvar_20.xy = (xlv_TEXCOORD0 + (vec2(1.0, 0.0) * _MainTex_TexelSize.xy));
  vec4 tmpvar_21;
  tmpvar_21 = texture2DLod (_MainTex, tmpvar_20.xy, 0.0);
  vec4 tmpvar_22;
  tmpvar_22.zw = vec2(0.0, 0.0);
  tmpvar_22.xy = (xlv_TEXCOORD0 + (vec2(0.0, 1.0) * _MainTex_TexelSize.xy));
  vec4 tmpvar_23;
  tmpvar_23 = texture2DLod (_MainTex, tmpvar_22.xy, 0.0);
  float tmpvar_24;
  tmpvar_24 = ((tmpvar_16.y * 1.963211) + tmpvar_16.x);
  lumaN_14 = tmpvar_24;
  float tmpvar_25;
  tmpvar_25 = ((tmpvar_18.y * 1.963211) + tmpvar_18.x);
  float tmpvar_26;
  tmpvar_26 = ((tmpvar_19.y * 1.963211) + tmpvar_19.x);
  float tmpvar_27;
  tmpvar_27 = ((tmpvar_21.y * 1.963211) + tmpvar_21.x);
  float tmpvar_28;
  tmpvar_28 = ((tmpvar_23.y * 1.963211) + tmpvar_23.x);
  lumaS_13 = tmpvar_28;
  float tmpvar_29;
  tmpvar_29 = max (tmpvar_26, max (max (tmpvar_24, tmpvar_25), max (tmpvar_28, tmpvar_27)));
  float tmpvar_30;
  tmpvar_30 = (tmpvar_29 - min (tmpvar_26, min (
    min (tmpvar_24, tmpvar_25)
  , 
    min (tmpvar_28, tmpvar_27)
  )));
  float tmpvar_31;
  tmpvar_31 = max (0.04166667, (tmpvar_29 * 0.125));
  if ((tmpvar_30 < tmpvar_31)) {
    tmpvar_2 = tmpvar_19.xyz;
  } else {
    float tmpvar_32;
    tmpvar_32 = min (0.75, (max (0.0, 
      ((abs((
        ((((tmpvar_24 + tmpvar_25) + tmpvar_27) + tmpvar_28) * 0.25)
       - tmpvar_26)) / tmpvar_30) - 0.25)
    ) * 1.333333));
    vec4 tmpvar_33;
    tmpvar_33.zw = vec2(0.0, 0.0);
    tmpvar_33.xy = (xlv_TEXCOORD0 - _MainTex_TexelSize.xy);
    vec4 tmpvar_34;
    tmpvar_34 = texture2DLod (_MainTex, tmpvar_33.xy, 0.0);
    vec4 tmpvar_35;
    tmpvar_35.zw = vec2(0.0, 0.0);
    tmpvar_35.xy = (xlv_TEXCOORD0 + (vec2(1.0, -1.0) * _MainTex_TexelSize.xy));
    vec4 tmpvar_36;
    tmpvar_36 = texture2DLod (_MainTex, tmpvar_35.xy, 0.0);
    vec4 tmpvar_37;
    tmpvar_37.zw = vec2(0.0, 0.0);
    tmpvar_37.xy = (xlv_TEXCOORD0 + (vec2(-1.0, 1.0) * _MainTex_TexelSize.xy));
    vec4 tmpvar_38;
    tmpvar_38 = texture2DLod (_MainTex, tmpvar_37.xy, 0.0);
    vec4 tmpvar_39;
    tmpvar_39.zw = vec2(0.0, 0.0);
    tmpvar_39.xy = (xlv_TEXCOORD0 + _MainTex_TexelSize.xy);
    vec4 tmpvar_40;
    tmpvar_40 = texture2DLod (_MainTex, tmpvar_39.xy, 0.0);
    vec3 tmpvar_41;
    tmpvar_41 = (((
      (((tmpvar_16.xyz + tmpvar_18.xyz) + tmpvar_19.xyz) + tmpvar_21.xyz)
     + tmpvar_23.xyz) + (
      ((tmpvar_34.xyz + tmpvar_36.xyz) + tmpvar_38.xyz)
     + tmpvar_40.xyz)) * vec3(0.1111111, 0.1111111, 0.1111111));
    float tmpvar_42;
    tmpvar_42 = ((tmpvar_34.y * 1.963211) + tmpvar_34.x);
    float tmpvar_43;
    tmpvar_43 = ((tmpvar_36.y * 1.963211) + tmpvar_36.x);
    float tmpvar_44;
    tmpvar_44 = ((tmpvar_38.y * 1.963211) + tmpvar_38.x);
    float tmpvar_45;
    tmpvar_45 = ((tmpvar_40.y * 1.963211) + tmpvar_40.x);
    bool tmpvar_46;
    tmpvar_46 = (((
      abs((((0.25 * tmpvar_42) + (-0.5 * tmpvar_25)) + (0.25 * tmpvar_44)))
     + 
      abs((((0.5 * tmpvar_24) - tmpvar_26) + (0.5 * tmpvar_28)))
    ) + abs(
      (((0.25 * tmpvar_43) + (-0.5 * tmpvar_27)) + (0.25 * tmpvar_45))
    )) >= ((
      abs((((0.25 * tmpvar_42) + (-0.5 * tmpvar_24)) + (0.25 * tmpvar_43)))
     + 
      abs((((0.5 * tmpvar_25) - tmpvar_26) + (0.5 * tmpvar_27)))
    ) + abs(
      (((0.25 * tmpvar_44) + (-0.5 * tmpvar_28)) + (0.25 * tmpvar_45))
    )));
    float tmpvar_47;
    if (tmpvar_46) {
      tmpvar_47 = -(_MainTex_TexelSize.y);
    } else {
      tmpvar_47 = -(_MainTex_TexelSize.x);
    };
    lengthSign_12 = tmpvar_47;
    if (!(tmpvar_46)) {
      lumaN_14 = tmpvar_25;
    };
    if (!(tmpvar_46)) {
      lumaS_13 = tmpvar_27;
    };
    float tmpvar_48;
    tmpvar_48 = abs((lumaN_14 - tmpvar_26));
    gradientN_11 = tmpvar_48;
    float tmpvar_49;
    tmpvar_49 = abs((lumaS_13 - tmpvar_26));
    lumaN_14 = ((lumaN_14 + tmpvar_26) * 0.5);
    float tmpvar_50;
    tmpvar_50 = ((lumaS_13 + tmpvar_26) * 0.5);
    lumaS_13 = tmpvar_50;
    bool tmpvar_51;
    tmpvar_51 = (tmpvar_48 >= tmpvar_49);
    if (!(tmpvar_51)) {
      lumaN_14 = tmpvar_50;
    };
    if (!(tmpvar_51)) {
      gradientN_11 = tmpvar_49;
    };
    if (!(tmpvar_51)) {
      lengthSign_12 = -(tmpvar_47);
    };
    float tmpvar_52;
    if (tmpvar_46) {
      tmpvar_52 = 0.0;
    } else {
      tmpvar_52 = (lengthSign_12 * 0.5);
    };
    posN_10.x = (xlv_TEXCOORD0.x + tmpvar_52);
    float tmpvar_53;
    if (tmpvar_46) {
      tmpvar_53 = (lengthSign_12 * 0.5);
    } else {
      tmpvar_53 = 0.0;
    };
    posN_10.y = (xlv_TEXCOORD0.y + tmpvar_53);
    gradientN_11 = (gradientN_11 * 0.25);
    posP_9 = posN_10;
    vec2 tmpvar_54;
    if (tmpvar_46) {
      vec2 tmpvar_55;
      tmpvar_55.y = 0.0;
      tmpvar_55.x = rcpFrame_1.x;
      tmpvar_54 = tmpvar_55;
    } else {
      vec2 tmpvar_56;
      tmpvar_56.x = 0.0;
      tmpvar_56.y = rcpFrame_1.y;
      tmpvar_54 = tmpvar_56;
    };
    offNP_8 = tmpvar_54;
    lumaEndN_7 = lumaN_14;
    lumaEndP_6 = lumaN_14;
    doneN_5 = bool(0);
    doneP_4 = bool(0);
    posN_10 = (posN_10 - tmpvar_54);
    posP_9 = (posP_9 + tmpvar_54);
    for (int i_3 = 0; i_3 < 16; i_3++) {
      if (!(doneN_5)) {
        vec4 tmpvar_57;
        tmpvar_57 = texture2DLod (_MainTex, posN_10, 0.0);
        lumaEndN_7 = ((tmpvar_57.y * 1.963211) + tmpvar_57.x);
      };
      if (!(doneP_4)) {
        vec4 tmpvar_58;
        tmpvar_58 = texture2DLod (_MainTex, posP_9, 0.0);
        lumaEndP_6 = ((tmpvar_58.y * 1.963211) + tmpvar_58.x);
      };
      bool tmpvar_59;
      if (doneN_5) {
        tmpvar_59 = bool(1);
      } else {
        tmpvar_59 = (abs((lumaEndN_7 - lumaN_14)) >= gradientN_11);
      };
      doneN_5 = tmpvar_59;
      bool tmpvar_60;
      if (doneP_4) {
        tmpvar_60 = bool(1);
      } else {
        tmpvar_60 = (abs((lumaEndP_6 - lumaN_14)) >= gradientN_11);
      };
      doneP_4 = tmpvar_60;
      if ((tmpvar_59 && tmpvar_60)) {
        break;
      };
      if (!(tmpvar_59)) {
        posN_10 = (posN_10 - offNP_8);
      };
      if (!(tmpvar_60)) {
        posP_9 = (posP_9 + offNP_8);
      };
    };
    float tmpvar_61;
    if (tmpvar_46) {
      tmpvar_61 = (xlv_TEXCOORD0.x - posN_10.x);
    } else {
      tmpvar_61 = (xlv_TEXCOORD0.y - posN_10.y);
    };
    float tmpvar_62;
    if (tmpvar_46) {
      tmpvar_62 = (posP_9.x - xlv_TEXCOORD0.x);
    } else {
      tmpvar_62 = (posP_9.y - xlv_TEXCOORD0.y);
    };
    bool tmpvar_63;
    tmpvar_63 = (tmpvar_61 < tmpvar_62);
    float tmpvar_64;
    if (tmpvar_63) {
      tmpvar_64 = lumaEndN_7;
    } else {
      tmpvar_64 = lumaEndP_6;
    };
    lumaEndN_7 = tmpvar_64;
    if ((((tmpvar_26 - lumaN_14) < 0.0) == ((tmpvar_64 - lumaN_14) < 0.0))) {
      lengthSign_12 = 0.0;
    };
    float tmpvar_65;
    tmpvar_65 = (tmpvar_62 + tmpvar_61);
    float tmpvar_66;
    if (tmpvar_63) {
      tmpvar_66 = tmpvar_61;
    } else {
      tmpvar_66 = tmpvar_62;
    };
    float tmpvar_67;
    tmpvar_67 = ((0.5 + (tmpvar_66 * 
      (-1.0 / tmpvar_65)
    )) * lengthSign_12);
    float tmpvar_68;
    if (tmpvar_46) {
      tmpvar_68 = 0.0;
    } else {
      tmpvar_68 = tmpvar_67;
    };
    float tmpvar_69;
    if (tmpvar_46) {
      tmpvar_69 = tmpvar_67;
    } else {
      tmpvar_69 = 0.0;
    };
    vec2 tmpvar_70;
    tmpvar_70.x = (xlv_TEXCOORD0.x + tmpvar_68);
    tmpvar_70.y = (xlv_TEXCOORD0.y + tmpvar_69);
    vec4 tmpvar_71;
    tmpvar_71 = texture2DLod (_MainTex, tmpvar_70, 0.0);
    vec3 tmpvar_72;
    tmpvar_72.x = -(tmpvar_32);
    tmpvar_72.y = -(tmpvar_32);
    tmpvar_72.z = -(tmpvar_32);
    tmpvar_2 = ((tmpvar_72 * tmpvar_71.xyz) + ((tmpvar_41 * vec3(tmpvar_32)) + tmpvar_71.xyz));
  };
  vec4 tmpvar_73;
  tmpvar_73.w = 0.0;
  tmpvar_73.xyz = tmpvar_2;
  gl_FragData[0] = tmpvar_73;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 5 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
""vs_3_0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 4 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""UnityPerDraw"" 0
""vs_4_0
eefiecedaffpdldohodkdgpagjklpapmmnbhcfmlabaaaaaaoeabaaaaadaaaaaa
cmaaaaaaiaaaaaaaniaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefcaeabaaaa
eaaaabaaebaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaa
doaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 147 math, 24 textures, 12 branches
Vector 0 [_MainTex_TexelSize]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c1, 0, -1, 1, 1.9632107
def c2, 0.125, -0.0416666679, 0.25, -0.25
def c3, 1.33333337, 0.75, -0.5, 0.5
def c4, 0, 0.111111112, 0, 0
defi i0, 16, 0, 0, 0
dcl_texcoord v0.xy
dcl_2d s0
mov r0.xyz, c1
mad r1, c0.xyxy, r0.xyyx, v0.xyxy
mul r2, r1.xyxx, c1.zzxx
texldl r2, r2, s0
mul r1, r1.zwxx, c1.zzxx
texldl r1, r1, s0
mul r3, c1.zzxx, v0.xyxx
texldl r3, r3, s0
mad r4, c0.xyxy, r0.zxxz, v0.xyxy
mul r5, r4.xyxx, c1.zzxx
texldl r5, r5, s0
mul r4, r4.zwxx, c1.zzxx
texldl r4, r4, s0
mad r0.w, r2.y, c1.w, r2.x
mad r1.w, r1.y, c1.w, r1.x
mad r2.w, r3.y, c1.w, r3.x
mad r3.w, r5.y, c1.w, r5.x
mad r4.w, r4.y, c1.w, r4.x
min r5.w, r1.w, r0.w
min r6.x, r3.w, r4.w
min r7.x, r6.x, r5.w
min r5.w, r7.x, r2.w
max r6.x, r0.w, r1.w
max r6.y, r4.w, r3.w
max r7.x, r6.x, r6.y
max r6.x, r2.w, r7.x
add r5.w, -r5.w, r6.x
mul r6.x, r6.x, c2.x
min r7.x, -r6.x, c2.y
if_lt r5.w, -r7.x
else
add r1.xyz, r1, r2
add r1.xyz, r3, r1
add r1.xyz, r5, r1
add r1.xyz, r4, r1
add r2.x, r0.w, r1.w
add r2.x, r3.w, r2.x
add r2.x, r4.w, r2.x
mad r2.x, r2.x, c2.z, -r2.w
rcp r2.y, r5.w
mad r2.x, r2_abs.x, r2.y, c2.w
mul r2.y, r2.x, c3.x
cmp r2.x, r2.x, r2.y, c1.x
min r4.x, r2.x, c3.y
add r5.xy, -c0, v0
mov r5.zw, c1.x
texldl r5, r5, s0
mad r6, c0.xyxy, r0.zyyz, v0.xyxy
mul r7, r6.xyxx, c1.zzxx
texldl r7, r7, s0
mul r6, r6.zwxx, c1.zzxx
texldl r6, r6, s0
add r8.xy, c0, v0
mov r8.zw, c1.x
texldl r8, r8, s0
add r2.xyz, r5, r7
add r2.xyz, r6, r2
add r2.xyz, r8, r2
add r1.xyz, r1, r2
mul r1.xyz, r4.x, r1
mad r0.y, r5.y, c1.w, r5.x
mad r2.x, r7.y, c1.w, r7.x
mad r2.y, r6.y, c1.w, r6.x
mad r2.z, r8.y, c1.w, r8.x
mul r4.y, r0.w, c3.z
mad r4.y, r0.y, c2.z, r4.y
mad r4.y, r2.x, c2.z, r4.y
mul r4.z, r1.w, c3.z
mad r5.x, r1.w, c3.w, -r2.w
mul r5.y, r3.w, c3.z
mad r5.x, r3.w, c3.w, r5.x
add r4.y, r4_abs.y, r5_abs.x
mul r5.x, r4.w, c3.z
mad r5.x, r2.y, c2.z, r5.x
mad r5.x, r2.z, c2.z, r5.x
add r4.y, r4.y, r5_abs.x
mad r0.y, r0.y, c2.z, r4.z
mad r0.y, r2.y, c2.z, r0.y
mad r2.y, r0.w, c3.w, -r2.w
mad r2.y, r4.w, c3.w, r2.y
add r0.y, r0_abs.y, r2_abs.y
mad r2.x, r2.x, c2.z, r5.y
mad r2.x, r2.z, c2.z, r2.x
add r0.y, r0.y, r2_abs.x
add r0.y, -r4.y, r0.y
cmp r2.x, r0.y, -c0.y, -c0.x
cmp r0.w, r0.y, r0.w, r1.w
cmp r1.w, r0.y, r4.w, r3.w
add r2.y, -r2.w, r0.w
add r2.z, -r2.w, r1.w
add r0.w, r2.w, r0.w
mul r0.w, r0.w, c3.w
add r1.w, r2.w, r1.w
mul r1.w, r1.w, c3.w
add r3.w, -r2_abs.z, r2_abs.y
cmp r0.w, r3.w, r0.w, r1.w
max r1.w, r2_abs.y, r2_abs.z
cmp r2.x, r3.w, r2.x, -r2.x
mul r2.y, r2.x, c3.w
cmp r2.z, r0.y, c1.x, r2.y
cmp r2.y, r0.y, r2.y, c1.x
add r5.xy, r2.zyzw, v0
mul r6, r0.zxxz, c0.xxxy
cmp r0.xz, r0.y, r6.xyyw, r6.zyww
add r2.yz, -r0.xxzw, r5.xxyw
add r4.yz, r0.xxzw, r5.xxyw
mov r5.xy, r2.yzzw
mov r5.zw, r4.xyyz
mov r3.w, r0.w
mov r4.w, r0.w
mov r6.xy, c1.x
rep i0
if_ne r6.x, -r6.x
mov r6.z, r3.w
else
mul r7, r5.xyxx, c1.zzxx
texldl r7, r7, s0
mad r6.z, r7.y, c1.w, r7.x
endif
if_ne r6.y, -r6.y
mov r6.w, r4.w
else
mul r7, r5.zwzz, c1.zzxx
texldl r7, r7, s0
mad r6.w, r7.y, c1.w, r7.x
endif
add r7.xy, -r0.w, r6.zwzw
mad r7.x, r1.w, -c2.z, r7_abs.x
cmp r7.x, r7.x, c1.z, c1.x
mad r7.y, r1.w, -c2.z, r7_abs.y
cmp r7.y, r7.y, c1.z, c1.x
add r7.xy, r6, r7
cmp r6.xy, -r7, c1.x, c1.z
mul r7.z, r6.y, r6.x
if_ne r7.z, -r7.z
mov r3.w, r6.z
mov r4.w, r6.w
break_ne c1.z, -c1.z
endif
add r7.zw, -r0.xyxz, r5.xyxy
cmp r5.xy, -r7.x, r7.zwzw, r5
add r7.xz, r0, r5.zyww
cmp r5.zw, -r7.y, r7.xyxz, r5
mov r3.w, r6.z
mov r4.w, r6.w
endrep
add r0.xz, -r5.xyyw, v0.xyyw
cmp r0.x, r0.y, r0.x, r0.z
add r2.yz, r5.xzww, -v0.xxyw
cmp r0.z, r0.y, r2.y, r2.z
add r1.w, -r0.z, r0.x
cmp r1.w, r1.w, r4.w, r3.w
add r2.y, -r0.w, r2.w
cmp r2.y, r2.y, c1.x, c1.z
add r0.w, -r0.w, r1.w
cmp r0.w, r0.w, -c1.x, -c1.z
add r0.w, r0.w, r2.y
cmp r0.w, -r0_abs.w, c1.x, r2.x
add r1.w, r0.x, r0.z
min r2.x, r0.z, r0.x
rcp r0.x, r1.w
mad r0.x, r2.x, -r0.x, c3.w
mul r0.x, r0.w, r0.x
cmp r0.z, r0.y, c1.x, r0.x
cmp r0.x, r0.y, r0.x, c1.x
add r2.xy, r0.zxzw, v0
mov r2.zw, c1.x
texldl r0, r2, s0
mad r1.xyz, r1, c4.y, r0
mad r3.xyz, -r4.x, r0, r1
endif
mov oC0.xyz, r3
mov oC0.w, c1.x

""
}
SubProgram ""d3d11 "" {
// Stats: 108 math, 8 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 112
Vector 96 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedphmnfpgblokfcicmjihfmaggmalfbhcdabaaaaaaiibeaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcmibdaaaa
eaaaaaaapcaeaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacajaaaaaadcaaaaanpcaabaaaaaaaaaaa
egiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaaialpaaaaialpaaaaaaaa
egbebaaaabaaaaaaeiaaaaalpcaabaaaabaaaaaaegaabaaaaaaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaaaaaaaaaa
ogakbaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaa
eiaaaaalpcaabaaaacaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaadcaaaaanpcaabaaaadaaaaaaegiecaaaaaaaaaaa
agaaaaaaaceaaaaaaaaaiadpaaaaaaaaaaaaaaaaaaaaiadpegbebaaaabaaaaaa
eiaaaaalpcaabaaaaeaaaaaaegaabaaaadaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaaadaaaaaaogakbaaaadaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadcaaaaajicaabaaa
aaaaaaaabkaabaaaabaaaaaaabeaaaaahnekpldpakaabaaaabaaaaaadcaaaaaj
icaabaaaabaaaaaabkaabaaaaaaaaaaaabeaaaaahnekpldpakaabaaaaaaaaaaa
dcaaaaajicaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaahnekpldpakaabaaa
acaaaaaadcaaaaajicaabaaaadaaaaaabkaabaaaaeaaaaaaabeaaaaahnekpldp
akaabaaaaeaaaaaadcaaaaajicaabaaaaeaaaaaabkaabaaaadaaaaaaabeaaaaa
hnekpldpakaabaaaadaaaaaaddaaaaahbcaabaaaafaaaaaadkaabaaaaaaaaaaa
dkaabaaaabaaaaaaddaaaaahccaabaaaafaaaaaadkaabaaaadaaaaaadkaabaaa
aeaaaaaaddaaaaahbcaabaaaafaaaaaabkaabaaaafaaaaaaakaabaaaafaaaaaa
ddaaaaahbcaabaaaafaaaaaadkaabaaaacaaaaaaakaabaaaafaaaaaadeaaaaah
ccaabaaaafaaaaaadkaabaaaaaaaaaaadkaabaaaabaaaaaadeaaaaahecaabaaa
afaaaaaadkaabaaaadaaaaaadkaabaaaaeaaaaaadeaaaaahccaabaaaafaaaaaa
ckaabaaaafaaaaaabkaabaaaafaaaaaadeaaaaahccaabaaaafaaaaaadkaabaaa
acaaaaaabkaabaaaafaaaaaaaaaaaaaibcaabaaaafaaaaaaakaabaiaebaaaaaa
afaaaaaabkaabaaaafaaaaaadiaaaaahccaabaaaafaaaaaabkaabaaaafaaaaaa
abeaaaaaaaaaaadodeaaaaahccaabaaaafaaaaaabkaabaaaafaaaaaaabeaaaaa
klkkckdnbnaaaaahccaabaaaafaaaaaaakaabaaaafaaaaaabkaabaaaafaaaaaa
bpaaaeadbkaabaaaafaaaaaaaaaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaa
egacbaaaabaaaaaaaaaaaaahhcaabaaaaaaaaaaaegacbaaaacaaaaaaegacbaaa
aaaaaaaaaaaaaaahhcaabaaaaaaaaaaaegacbaaaaeaaaaaaegacbaaaaaaaaaaa
aaaaaaahhcaabaaaaaaaaaaaegacbaaaadaaaaaaegacbaaaaaaaaaaaaaaaaaah
bcaabaaaabaaaaaadkaabaaaaaaaaaaadkaabaaaabaaaaaaaaaaaaahbcaabaaa
abaaaaaadkaabaaaadaaaaaaakaabaaaabaaaaaaaaaaaaahbcaabaaaabaaaaaa
dkaabaaaaeaaaaaaakaabaaaabaaaaaadcaaaaakbcaabaaaabaaaaaaakaabaaa
abaaaaaaabeaaaaaaaaaiadodkaabaiaebaaaaaaacaaaaaaaoaaaaaibcaabaaa
abaaaaaaakaabaiaibaaaaaaabaaaaaaakaabaaaafaaaaaaaaaaaaahbcaabaaa
abaaaaaaakaabaaaabaaaaaaabeaaaaaaaaaialodeaaaaahbcaabaaaabaaaaaa
akaabaaaabaaaaaaabeaaaaaaaaaaaaadiaaaaahbcaabaaaabaaaaaaakaabaaa
abaaaaaaabeaaaaaklkkkkdpddaaaaahbcaabaaaabaaaaaaakaabaaaabaaaaaa
abeaaaaaaaaaeadpaaaaaaajgcaabaaaabaaaaaaagbbbaaaabaaaaaaagibcaia
ebaaaaaaaaaaaaaaagaaaaaaeiaaaaalpcaabaaaafaaaaaajgafbaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadcaaaaanpcaabaaa
agaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaiadpaaaaialpaaaaialp
aaaaiadpegbebaaaabaaaaaaeiaaaaalpcaabaaaahaaaaaaegaabaaaagaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaa
agaaaaaaogakbaaaagaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaa
aaaaaaaaaaaaaaaigcaabaaaabaaaaaaagbbbaaaabaaaaaaagibcaaaaaaaaaaa
agaaaaaaeiaaaaalpcaabaaaaiaaaaaajgafbaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaabeaaaaaaaaaaaaaaaaaaaahhcaabaaaadaaaaaaegacbaaa
afaaaaaaegacbaaaahaaaaaaaaaaaaahhcaabaaaadaaaaaaegacbaaaagaaaaaa
egacbaaaadaaaaaaaaaaaaahhcaabaaaadaaaaaaegacbaaaaiaaaaaaegacbaaa
adaaaaaaaaaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaadaaaaaa
diaaaaahhcaabaaaaaaaaaaaagaabaaaabaaaaaaegacbaaaaaaaaaaadcaaaaaj
ccaabaaaabaaaaaabkaabaaaafaaaaaaabeaaaaahnekpldpakaabaaaafaaaaaa
dcaaaaajecaabaaaabaaaaaabkaabaaaahaaaaaaabeaaaaahnekpldpakaabaaa
ahaaaaaadcaaaaajbcaabaaaadaaaaaabkaabaaaagaaaaaaabeaaaaahnekpldp
akaabaaaagaaaaaadcaaaaajccaabaaaadaaaaaabkaabaaaaiaaaaaaabeaaaaa
hnekpldpakaabaaaaiaaaaaadiaaaaahecaabaaaadaaaaaadkaabaaaaaaaaaaa
abeaaaaaaaaaaalpdcaaaaajecaabaaaadaaaaaabkaabaaaabaaaaaaabeaaaaa
aaaaiadockaabaaaadaaaaaadcaaaaajecaabaaaadaaaaaackaabaaaabaaaaaa
abeaaaaaaaaaiadockaabaaaadaaaaaadiaaaaahbcaabaaaaeaaaaaadkaabaaa
abaaaaaaabeaaaaaaaaaaalpdcaaaaakccaabaaaaeaaaaaadkaabaaaabaaaaaa
abeaaaaaaaaaaadpdkaabaiaebaaaaaaacaaaaaadiaaaaahecaabaaaaeaaaaaa
dkaabaaaadaaaaaaabeaaaaaaaaaaalpdcaaaaajccaabaaaaeaaaaaadkaabaaa
adaaaaaaabeaaaaaaaaaaadpbkaabaaaaeaaaaaaaaaaaaajecaabaaaadaaaaaa
ckaabaiaibaaaaaaadaaaaaabkaabaiaibaaaaaaaeaaaaaadiaaaaahccaabaaa
aeaaaaaadkaabaaaaeaaaaaaabeaaaaaaaaaaalpdcaaaaajccaabaaaaeaaaaaa
akaabaaaadaaaaaaabeaaaaaaaaaiadobkaabaaaaeaaaaaadcaaaaajccaabaaa
aeaaaaaabkaabaaaadaaaaaaabeaaaaaaaaaiadobkaabaaaaeaaaaaaaaaaaaai
ecaabaaaadaaaaaackaabaaaadaaaaaabkaabaiaibaaaaaaaeaaaaaadcaaaaaj
ccaabaaaabaaaaaabkaabaaaabaaaaaaabeaaaaaaaaaiadoakaabaaaaeaaaaaa
dcaaaaajccaabaaaabaaaaaaakaabaaaadaaaaaaabeaaaaaaaaaiadobkaabaaa
abaaaaaadcaaaaakbcaabaaaadaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaadp
dkaabaiaebaaaaaaacaaaaaadcaaaaajbcaabaaaadaaaaaadkaabaaaaeaaaaaa
abeaaaaaaaaaaadpakaabaaaadaaaaaaaaaaaaajccaabaaaabaaaaaabkaabaia
ibaaaaaaabaaaaaaakaabaiaibaaaaaaadaaaaaadcaaaaajecaabaaaabaaaaaa
ckaabaaaabaaaaaaabeaaaaaaaaaiadockaabaaaaeaaaaaadcaaaaajecaabaaa
abaaaaaabkaabaaaadaaaaaaabeaaaaaaaaaiadockaabaaaabaaaaaaaaaaaaai
ccaabaaaabaaaaaackaabaiaibaaaaaaabaaaaaabkaabaaaabaaaaaabnaaaaah
ccaabaaaabaaaaaabkaabaaaabaaaaaackaabaaaadaaaaaadhaaaaanecaabaaa
abaaaaaabkaabaaaabaaaaaabkiacaiaebaaaaaaaaaaaaaaagaaaaaaakiacaia
ebaaaaaaaaaaaaaaagaaaaaadhaaaaajicaabaaaaaaaaaaabkaabaaaabaaaaaa
dkaabaaaaaaaaaaadkaabaaaabaaaaaadhaaaaajicaabaaaabaaaaaabkaabaaa
abaaaaaadkaabaaaaeaaaaaadkaabaaaadaaaaaaaaaaaaaibcaabaaaadaaaaaa
dkaabaiaebaaaaaaacaaaaaadkaabaaaaaaaaaaaaaaaaaaiccaabaaaadaaaaaa
dkaabaiaebaaaaaaacaaaaaadkaabaaaabaaaaaaaaaaaaahicaabaaaaaaaaaaa
dkaabaaaacaaaaaadkaabaaaaaaaaaaadiaaaaahicaabaaaaaaaaaaadkaabaaa
aaaaaaaaabeaaaaaaaaaaadpaaaaaaahicaabaaaabaaaaaadkaabaaaacaaaaaa
dkaabaaaabaaaaaadiaaaaahicaabaaaabaaaaaadkaabaaaabaaaaaaabeaaaaa
aaaaaadpbnaaaaajecaabaaaadaaaaaaakaabaiaibaaaaaaadaaaaaabkaabaia
ibaaaaaaadaaaaaadhaaaaajicaabaaaaaaaaaaackaabaaaadaaaaaadkaabaaa
aaaaaaaadkaabaaaabaaaaaadhaaaaalicaabaaaabaaaaaackaabaaaadaaaaaa
akaabaiaibaaaaaaadaaaaaabkaabaiaibaaaaaaadaaaaaadhaaaaakecaabaaa
abaaaaaackaabaaaadaaaaaackaabaaaabaaaaaackaabaiaebaaaaaaabaaaaaa
diaaaaahbcaabaaaadaaaaaackaabaaaabaaaaaaabeaaaaaaaaaaadpdhaaaaaj
ccaabaaaadaaaaaabkaabaaaabaaaaaaabeaaaaaaaaaaaaaakaabaaaadaaaaaa
abaaaaahbcaabaaaadaaaaaabkaabaaaabaaaaaaakaabaaaadaaaaaaaaaaaaah
dcaabaaaaeaaaaaabgafbaaaadaaaaaaegbabaaaabaaaaaadiaaaaahicaabaaa
abaaaaaadkaabaaaabaaaaaaabeaaaaaaaaaiadodgaaaaaigcaabaaaadaaaaaa
aceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadgaaaaagjcaabaaaadaaaaaa
agiecaaaaaaaaaaaagaaaaaadhaaaaajdcaabaaaadaaaaaafgafbaaaabaaaaaa
egaabaaaadaaaaaaogakbaaaadaaaaaaaaaaaaaimcaabaaaadaaaaaaagaebaia
ebaaaaaaadaaaaaaagaebaaaaeaaaaaaaaaaaaahdcaabaaaaeaaaaaaegaabaaa
adaaaaaaegaabaaaaeaaaaaadgaaaaafmcaabaaaaeaaaaaakgaobaaaadaaaaaa
dgaaaaafdcaabaaaafaaaaaaegaabaaaaeaaaaaadgaaaaafmcaabaaaafaaaaaa
pgapbaaaaaaaaaaadgaaaaaihcaabaaaagaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaadaaaaaabcbaaaaahicaabaaaagaaaaaackaabaaaagaaaaaa
abeaaaaabaaaaaaaadaaaeaddkaabaaaagaaaaaabpaaaaadakaabaaaagaaaaaa
eiaaaaalpcaabaaaahaaaaaaogakbaaaaeaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaadcaaaaajicaabaaaagaaaaaabkaabaaaahaaaaaa
abeaaaaahnekpldpakaabaaaahaaaaaabcaaaaabdgaaaaaficaabaaaagaaaaaa
ckaabaaaafaaaaaabfaaaaabbpaaaaadbkaabaaaagaaaaaaeiaaaaalpcaabaaa
ahaaaaaaegaabaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaa
aaaaaaaadcaaaaajbcaabaaaahaaaaaabkaabaaaahaaaaaaabeaaaaahnekpldp
akaabaaaahaaaaaabcaaaaabdgaaaaafbcaabaaaahaaaaaadkaabaaaafaaaaaa
bfaaaaabaaaaaaaiccaabaaaahaaaaaadkaabaiaebaaaaaaaaaaaaaadkaabaaa
agaaaaaabnaaaaaiccaabaaaahaaaaaabkaabaiaibaaaaaaahaaaaaadkaabaaa
abaaaaaadmaaaaahbcaabaaaagaaaaaaakaabaaaagaaaaaabkaabaaaahaaaaaa
aaaaaaaiccaabaaaahaaaaaadkaabaiaebaaaaaaaaaaaaaaakaabaaaahaaaaaa
bnaaaaaiccaabaaaahaaaaaabkaabaiaibaaaaaaahaaaaaadkaabaaaabaaaaaa
dmaaaaahccaabaaaagaaaaaabkaabaaaagaaaaaabkaabaaaahaaaaaaabaaaaah
ccaabaaaahaaaaaabkaabaaaagaaaaaaakaabaaaagaaaaaabpaaaeadbkaabaaa
ahaaaaaadgaaaaafecaabaaaafaaaaaadkaabaaaagaaaaaadgaaaaaficaabaaa
afaaaaaaakaabaaaahaaaaaaacaaaaabbfaaaaabaaaaaaaigcaabaaaahaaaaaa
agabbaiaebaaaaaaadaaaaaakgalbaaaaeaaaaaadhaaaaajmcaabaaaaeaaaaaa
agaabaaaagaaaaaakgaobaaaaeaaaaaafgajbaaaahaaaaaaaaaaaaahgcaabaaa
ahaaaaaaagabbaaaadaaaaaaagabbaaaafaaaaaadhaaaaajdcaabaaaafaaaaaa
fgafbaaaagaaaaaaegaabaaaafaaaaaajgafbaaaahaaaaaaboaaaaahecaabaaa
agaaaaaackaabaaaagaaaaaaabeaaaaaabaaaaaadgaaaaafecaabaaaafaaaaaa
dkaabaaaagaaaaaadgaaaaaficaabaaaafaaaaaaakaabaaaahaaaaaabgaaaaab
aaaaaaaidcaabaaaadaaaaaaogakbaiaebaaaaaaaeaaaaaaegbabaaaabaaaaaa
dhaaaaajicaabaaaabaaaaaabkaabaaaabaaaaaaakaabaaaadaaaaaabkaabaaa
adaaaaaaaaaaaaaidcaabaaaadaaaaaaegaabaaaafaaaaaaegbabaiaebaaaaaa
abaaaaaadhaaaaajbcaabaaaadaaaaaabkaabaaaabaaaaaaakaabaaaadaaaaaa
bkaabaaaadaaaaaadbaaaaahccaabaaaadaaaaaadkaabaaaabaaaaaaakaabaaa
adaaaaaadhaaaaajecaabaaaadaaaaaabkaabaaaadaaaaaackaabaaaafaaaaaa
dkaabaaaafaaaaaaaaaaaaaiicaabaaaacaaaaaadkaabaiaebaaaaaaaaaaaaaa
dkaabaaaacaaaaaadbaaaaahicaabaaaacaaaaaadkaabaaaacaaaaaaabeaaaaa
aaaaaaaaaaaaaaaiicaabaaaaaaaaaaadkaabaiaebaaaaaaaaaaaaaackaabaaa
adaaaaaadbaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaaaa
caaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaaacaaaaaadhaaaaaj
icaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaaaackaabaaaabaaaaaa
aaaaaaahecaabaaaabaaaaaadkaabaaaabaaaaaaakaabaaaadaaaaaadhaaaaaj
icaabaaaabaaaaaabkaabaaaadaaaaaadkaabaaaabaaaaaaakaabaaaadaaaaaa
aoaaaaahecaabaaaabaaaaaaabeaaaaaaaaaialpckaabaaaabaaaaaadcaaaaaj
ecaabaaaabaaaaaadkaabaaaabaaaaaackaabaaaabaaaaaaabeaaaaaaaaaaadp
diaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaackaabaaaabaaaaaadhaaaaaj
ecaabaaaabaaaaaabkaabaaaabaaaaaaabeaaaaaaaaaaaaadkaabaaaaaaaaaaa
aaaaaaahbcaabaaaadaaaaaackaabaaaabaaaaaaakbabaaaabaaaaaaabaaaaah
icaabaaaaaaaaaaadkaabaaaaaaaaaaabkaabaaaabaaaaaaaaaaaaahccaabaaa
adaaaaaadkaabaaaaaaaaaaabkbabaaaabaaaaaaeiaaaaalpcaabaaaadaaaaaa
egaabaaaadaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaa
dcaaaaamhcaabaaaaaaaaaaaegacbaaaaaaaaaaaaceaaaaadjiooddndjiooddn
djiooddnaaaaaaaaegacbaaaadaaaaaadcaaaaakhcaabaaaacaaaaaaagaabaia
ebaaaaaaabaaaaaaegacbaaaadaaaaaaegacbaaaaaaaaaaabfaaaaabdgaaaaaf
hccabaaaaaaaaaaaegacbaaaacaaaaaadgaaaaaficcabaaaaaaaaaaaabeaaaaa
aaaaaaaadoaaaaab""
}
}
 }
}
Fallback ""Hidden/FXAA II""
}";

		private const String fxaaPreset2ShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 25.5KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/FXAA Preset 2"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = ""white"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 194 math, 12 texture, 26 branch
 // Stats for Fragment shader:
 //       d3d11 : 108 math, 8 branch
 //        d3d9 : 145 math, 26 texture, 12 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 26581
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 194 math, 12 textures, 26 branches
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
#extension GL_ARB_shader_texture_lod : enable
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec2 rcpFrame_1;
  rcpFrame_1 = _MainTex_TexelSize.xy;
  vec3 tmpvar_2;
  bool doneP_4;
  bool doneN_5;
  float lumaEndP_6;
  float lumaEndN_7;
  vec2 offNP_8;
  vec2 posP_9;
  vec2 posN_10;
  float gradientN_11;
  float lengthSign_12;
  float lumaS_13;
  float lumaN_14;
  vec4 tmpvar_15;
  tmpvar_15.zw = vec2(0.0, 0.0);
  tmpvar_15.xy = (xlv_TEXCOORD0 + (vec2(0.0, -1.0) * _MainTex_TexelSize.xy));
  vec4 tmpvar_16;
  tmpvar_16 = texture2DLod (_MainTex, tmpvar_15.xy, 0.0);
  vec4 tmpvar_17;
  tmpvar_17.zw = vec2(0.0, 0.0);
  tmpvar_17.xy = (xlv_TEXCOORD0 + (vec2(-1.0, 0.0) * _MainTex_TexelSize.xy));
  vec4 tmpvar_18;
  tmpvar_18 = texture2DLod (_MainTex, tmpvar_17.xy, 0.0);
  vec4 tmpvar_19;
  tmpvar_19 = texture2DLod (_MainTex, xlv_TEXCOORD0, 0.0);
  vec4 tmpvar_20;
  tmpvar_20.zw = vec2(0.0, 0.0);
  tmpvar_20.xy = (xlv_TEXCOORD0 + (vec2(1.0, 0.0) * _MainTex_TexelSize.xy));
  vec4 tmpvar_21;
  tmpvar_21 = texture2DLod (_MainTex, tmpvar_20.xy, 0.0);
  vec4 tmpvar_22;
  tmpvar_22.zw = vec2(0.0, 0.0);
  tmpvar_22.xy = (xlv_TEXCOORD0 + (vec2(0.0, 1.0) * _MainTex_TexelSize.xy));
  vec4 tmpvar_23;
  tmpvar_23 = texture2DLod (_MainTex, tmpvar_22.xy, 0.0);
  float tmpvar_24;
  tmpvar_24 = ((tmpvar_16.y * 1.963211) + tmpvar_16.x);
  lumaN_14 = tmpvar_24;
  float tmpvar_25;
  tmpvar_25 = ((tmpvar_18.y * 1.963211) + tmpvar_18.x);
  float tmpvar_26;
  tmpvar_26 = ((tmpvar_19.y * 1.963211) + tmpvar_19.x);
  float tmpvar_27;
  tmpvar_27 = ((tmpvar_21.y * 1.963211) + tmpvar_21.x);
  float tmpvar_28;
  tmpvar_28 = ((tmpvar_23.y * 1.963211) + tmpvar_23.x);
  lumaS_13 = tmpvar_28;
  float tmpvar_29;
  tmpvar_29 = max (tmpvar_26, max (max (tmpvar_24, tmpvar_25), max (tmpvar_28, tmpvar_27)));
  float tmpvar_30;
  tmpvar_30 = (tmpvar_29 - min (tmpvar_26, min (
    min (tmpvar_24, tmpvar_25)
  , 
    min (tmpvar_28, tmpvar_27)
  )));
  float tmpvar_31;
  tmpvar_31 = max (0.04166667, (tmpvar_29 * 0.125));
  if ((tmpvar_30 < tmpvar_31)) {
    tmpvar_2 = tmpvar_19.xyz;
  } else {
    float tmpvar_32;
    tmpvar_32 = min (0.75, (max (0.0, 
      ((abs((
        ((((tmpvar_24 + tmpvar_25) + tmpvar_27) + tmpvar_28) * 0.25)
       - tmpvar_26)) / tmpvar_30) - 0.25)
    ) * 1.333333));
    vec4 tmpvar_33;
    tmpvar_33.zw = vec2(0.0, 0.0);
    tmpvar_33.xy = (xlv_TEXCOORD0 - _MainTex_TexelSize.xy);
    vec4 tmpvar_34;
    tmpvar_34 = texture2DLod (_MainTex, tmpvar_33.xy, 0.0);
    vec4 tmpvar_35;
    tmpvar_35.zw = vec2(0.0, 0.0);
    tmpvar_35.xy = (xlv_TEXCOORD0 + (vec2(1.0, -1.0) * _MainTex_TexelSize.xy));
    vec4 tmpvar_36;
    tmpvar_36 = texture2DLod (_MainTex, tmpvar_35.xy, 0.0);
    vec4 tmpvar_37;
    tmpvar_37.zw = vec2(0.0, 0.0);
    tmpvar_37.xy = (xlv_TEXCOORD0 + (vec2(-1.0, 1.0) * _MainTex_TexelSize.xy));
    vec4 tmpvar_38;
    tmpvar_38 = texture2DLod (_MainTex, tmpvar_37.xy, 0.0);
    vec4 tmpvar_39;
    tmpvar_39.zw = vec2(0.0, 0.0);
    tmpvar_39.xy = (xlv_TEXCOORD0 + _MainTex_TexelSize.xy);
    vec4 tmpvar_40;
    tmpvar_40 = texture2DLod (_MainTex, tmpvar_39.xy, 0.0);
    vec3 tmpvar_41;
    tmpvar_41 = (((
      (((tmpvar_16.xyz + tmpvar_18.xyz) + tmpvar_19.xyz) + tmpvar_21.xyz)
     + tmpvar_23.xyz) + (
      ((tmpvar_34.xyz + tmpvar_36.xyz) + tmpvar_38.xyz)
     + tmpvar_40.xyz)) * vec3(0.1111111, 0.1111111, 0.1111111));
    float tmpvar_42;
    tmpvar_42 = ((tmpvar_34.y * 1.963211) + tmpvar_34.x);
    float tmpvar_43;
    tmpvar_43 = ((tmpvar_36.y * 1.963211) + tmpvar_36.x);
    float tmpvar_44;
    tmpvar_44 = ((tmpvar_38.y * 1.963211) + tmpvar_38.x);
    float tmpvar_45;
    tmpvar_45 = ((tmpvar_40.y * 1.963211) + tmpvar_40.x);
    bool tmpvar_46;
    tmpvar_46 = (((
      abs((((0.25 * tmpvar_42) + (-0.5 * tmpvar_25)) + (0.25 * tmpvar_44)))
     + 
      abs((((0.5 * tmpvar_24) - tmpvar_26) + (0.5 * tmpvar_28)))
    ) + abs(
      (((0.25 * tmpvar_43) + (-0.5 * tmpvar_27)) + (0.25 * tmpvar_45))
    )) >= ((
      abs((((0.25 * tmpvar_42) + (-0.5 * tmpvar_24)) + (0.25 * tmpvar_43)))
     + 
      abs((((0.5 * tmpvar_25) - tmpvar_26) + (0.5 * tmpvar_27)))
    ) + abs(
      (((0.25 * tmpvar_44) + (-0.5 * tmpvar_28)) + (0.25 * tmpvar_45))
    )));
    float tmpvar_47;
    if (tmpvar_46) {
      tmpvar_47 = -(_MainTex_TexelSize.y);
    } else {
      tmpvar_47 = -(_MainTex_TexelSize.x);
    };
    lengthSign_12 = tmpvar_47;
    if (!(tmpvar_46)) {
      lumaN_14 = tmpvar_25;
    };
    if (!(tmpvar_46)) {
      lumaS_13 = tmpvar_27;
    };
    float tmpvar_48;
    tmpvar_48 = abs((lumaN_14 - tmpvar_26));
    gradientN_11 = tmpvar_48;
    float tmpvar_49;
    tmpvar_49 = abs((lumaS_13 - tmpvar_26));
    lumaN_14 = ((lumaN_14 + tmpvar_26) * 0.5);
    float tmpvar_50;
    tmpvar_50 = ((lumaS_13 + tmpvar_26) * 0.5);
    lumaS_13 = tmpvar_50;
    bool tmpvar_51;
    tmpvar_51 = (tmpvar_48 >= tmpvar_49);
    if (!(tmpvar_51)) {
      lumaN_14 = tmpvar_50;
    };
    if (!(tmpvar_51)) {
      gradientN_11 = tmpvar_49;
    };
    if (!(tmpvar_51)) {
      lengthSign_12 = -(tmpvar_47);
    };
    float tmpvar_52;
    if (tmpvar_46) {
      tmpvar_52 = 0.0;
    } else {
      tmpvar_52 = (lengthSign_12 * 0.5);
    };
    posN_10.x = (xlv_TEXCOORD0.x + tmpvar_52);
    float tmpvar_53;
    if (tmpvar_46) {
      tmpvar_53 = (lengthSign_12 * 0.5);
    } else {
      tmpvar_53 = 0.0;
    };
    posN_10.y = (xlv_TEXCOORD0.y + tmpvar_53);
    gradientN_11 = (gradientN_11 * 0.25);
    posP_9 = posN_10;
    vec2 tmpvar_54;
    if (tmpvar_46) {
      vec2 tmpvar_55;
      tmpvar_55.y = 0.0;
      tmpvar_55.x = rcpFrame_1.x;
      tmpvar_54 = tmpvar_55;
    } else {
      vec2 tmpvar_56;
      tmpvar_56.x = 0.0;
      tmpvar_56.y = rcpFrame_1.y;
      tmpvar_54 = tmpvar_56;
    };
    lumaEndN_7 = lumaN_14;
    lumaEndP_6 = lumaN_14;
    doneN_5 = bool(0);
    doneP_4 = bool(0);
    posN_10 = (posN_10 + (tmpvar_54 * vec2(-1.5, -1.5)));
    posP_9 = (posP_9 + (tmpvar_54 * vec2(1.5, 1.5)));
    offNP_8 = (tmpvar_54 * vec2(2.0, 2.0));
    for (int i_3 = 0; i_3 < 8; i_3++) {
      if (!(doneN_5)) {
        vec4 tmpvar_57;
        tmpvar_57 = texture2DGradARB (_MainTex, posN_10, offNP_8, offNP_8);
        lumaEndN_7 = ((tmpvar_57.y * 1.963211) + tmpvar_57.x);
      };
      if (!(doneP_4)) {
        vec4 tmpvar_58;
        tmpvar_58 = texture2DGradARB (_MainTex, posP_9, offNP_8, offNP_8);
        lumaEndP_6 = ((tmpvar_58.y * 1.963211) + tmpvar_58.x);
      };
      bool tmpvar_59;
      if (doneN_5) {
        tmpvar_59 = bool(1);
      } else {
        tmpvar_59 = (abs((lumaEndN_7 - lumaN_14)) >= gradientN_11);
      };
      doneN_5 = tmpvar_59;
      bool tmpvar_60;
      if (doneP_4) {
        tmpvar_60 = bool(1);
      } else {
        tmpvar_60 = (abs((lumaEndP_6 - lumaN_14)) >= gradientN_11);
      };
      doneP_4 = tmpvar_60;
      if ((tmpvar_59 && tmpvar_60)) {
        break;
      };
      if (!(tmpvar_59)) {
        posN_10 = (posN_10 - offNP_8);
      };
      if (!(tmpvar_60)) {
        posP_9 = (posP_9 + offNP_8);
      };
    };
    float tmpvar_61;
    if (tmpvar_46) {
      tmpvar_61 = (xlv_TEXCOORD0.x - posN_10.x);
    } else {
      tmpvar_61 = (xlv_TEXCOORD0.y - posN_10.y);
    };
    float tmpvar_62;
    if (tmpvar_46) {
      tmpvar_62 = (posP_9.x - xlv_TEXCOORD0.x);
    } else {
      tmpvar_62 = (posP_9.y - xlv_TEXCOORD0.y);
    };
    bool tmpvar_63;
    tmpvar_63 = (tmpvar_61 < tmpvar_62);
    float tmpvar_64;
    if (tmpvar_63) {
      tmpvar_64 = lumaEndN_7;
    } else {
      tmpvar_64 = lumaEndP_6;
    };
    lumaEndN_7 = tmpvar_64;
    if ((((tmpvar_26 - lumaN_14) < 0.0) == ((tmpvar_64 - lumaN_14) < 0.0))) {
      lengthSign_12 = 0.0;
    };
    float tmpvar_65;
    tmpvar_65 = (tmpvar_62 + tmpvar_61);
    float tmpvar_66;
    if (tmpvar_63) {
      tmpvar_66 = tmpvar_61;
    } else {
      tmpvar_66 = tmpvar_62;
    };
    float tmpvar_67;
    tmpvar_67 = ((0.5 + (tmpvar_66 * 
      (-1.0 / tmpvar_65)
    )) * lengthSign_12);
    float tmpvar_68;
    if (tmpvar_46) {
      tmpvar_68 = 0.0;
    } else {
      tmpvar_68 = tmpvar_67;
    };
    float tmpvar_69;
    if (tmpvar_46) {
      tmpvar_69 = tmpvar_67;
    } else {
      tmpvar_69 = 0.0;
    };
    vec2 tmpvar_70;
    tmpvar_70.x = (xlv_TEXCOORD0.x + tmpvar_68);
    tmpvar_70.y = (xlv_TEXCOORD0.y + tmpvar_69);
    vec4 tmpvar_71;
    tmpvar_71 = texture2DLod (_MainTex, tmpvar_70, 0.0);
    vec3 tmpvar_72;
    tmpvar_72.x = -(tmpvar_32);
    tmpvar_72.y = -(tmpvar_32);
    tmpvar_72.z = -(tmpvar_32);
    tmpvar_2 = ((tmpvar_72 * tmpvar_71.xyz) + ((tmpvar_41 * vec3(tmpvar_32)) + tmpvar_71.xyz));
  };
  vec4 tmpvar_73;
  tmpvar_73.w = 0.0;
  tmpvar_73.xyz = tmpvar_2;
  gl_FragData[0] = tmpvar_73;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 5 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
""vs_3_0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 4 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""UnityPerDraw"" 0
""vs_4_0
eefiecedaffpdldohodkdgpagjklpapmmnbhcfmlabaaaaaaoeabaaaaadaaaaaa
cmaaaaaaiaaaaaaaniaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefcaeabaaaa
eaaaabaaebaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaa
doaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 145 math, 26 textures, 12 branches
Vector 0 [_MainTex_TexelSize]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c1, 0, -1, 1, 1.9632107
def c2, 0.125, -0.0416666679, 0.25, -0.25
def c3, 1.33333337, 0.75, -0.5, 0.5
def c4, 0.111111112, 0, 0, 0
def c5, -1.5, 1.5, 0, 2
defi i0, 8, 0, 0, 0
dcl_texcoord v0.xy
dcl_2d s0
mov r0.xyz, c1
mad r1, c0.xyxy, r0.xyyx, v0.xyxy
mul r2, r1.xyxx, c1.zzxx
texldl r2, r2, s0
mul r1, r1.zwxx, c1.zzxx
texldl r1, r1, s0
mul r3, c1.zzxx, v0.xyxx
texldl r3, r3, s0
mad r4, c0.xyxy, r0.zxxz, v0.xyxy
mul r5, r4.xyxx, c1.zzxx
texldl r5, r5, s0
mul r4, r4.zwxx, c1.zzxx
texldl r4, r4, s0
mad r0.w, r2.y, c1.w, r2.x
mad r1.w, r1.y, c1.w, r1.x
mad r2.w, r3.y, c1.w, r3.x
mad r3.w, r5.y, c1.w, r5.x
mad r4.w, r4.y, c1.w, r4.x
min r5.w, r1.w, r0.w
min r6.x, r3.w, r4.w
min r7.x, r6.x, r5.w
min r5.w, r7.x, r2.w
max r6.x, r0.w, r1.w
max r6.y, r4.w, r3.w
max r7.x, r6.x, r6.y
max r6.x, r2.w, r7.x
add r5.w, -r5.w, r6.x
mul r6.x, r6.x, c2.x
min r7.x, -r6.x, c2.y
if_lt r5.w, -r7.x
else
add r1.xyz, r1, r2
add r1.xyz, r3, r1
add r1.xyz, r5, r1
add r1.xyz, r4, r1
add r2.x, r0.w, r1.w
add r2.x, r3.w, r2.x
add r2.x, r4.w, r2.x
mad r2.x, r2.x, c2.z, -r2.w
rcp r2.y, r5.w
mad r2.x, r2_abs.x, r2.y, c2.w
mul r2.y, r2.x, c3.x
cmp r2.x, r2.x, r2.y, c1.x
min r4.x, r2.x, c3.y
add r5.xy, -c0, v0
mov r5.zw, c1.x
texldl r5, r5, s0
mad r6, c0.xyxy, r0.zyyz, v0.xyxy
mul r7, r6.xyxx, c1.zzxx
texldl r7, r7, s0
mul r6, r6.zwxx, c1.zzxx
texldl r6, r6, s0
add r8.xy, c0, v0
mov r8.zw, c1.x
texldl r8, r8, s0
add r2.xyz, r5, r7
add r2.xyz, r6, r2
add r2.xyz, r8, r2
add r1.xyz, r1, r2
mul r1.xyz, r4.x, r1
mad r0.y, r5.y, c1.w, r5.x
mad r2.x, r7.y, c1.w, r7.x
mad r2.y, r6.y, c1.w, r6.x
mad r2.z, r8.y, c1.w, r8.x
mul r4.y, r0.w, c3.z
mad r4.y, r0.y, c2.z, r4.y
mad r4.y, r2.x, c2.z, r4.y
mul r4.z, r1.w, c3.z
mad r5.x, r1.w, c3.w, -r2.w
mul r5.y, r3.w, c3.z
mad r5.x, r3.w, c3.w, r5.x
add r4.y, r4_abs.y, r5_abs.x
mul r5.x, r4.w, c3.z
mad r5.x, r2.y, c2.z, r5.x
mad r5.x, r2.z, c2.z, r5.x
add r4.y, r4.y, r5_abs.x
mad r0.y, r0.y, c2.z, r4.z
mad r0.y, r2.y, c2.z, r0.y
mad r2.y, r0.w, c3.w, -r2.w
mad r2.y, r4.w, c3.w, r2.y
add r0.y, r0_abs.y, r2_abs.y
mad r2.x, r2.x, c2.z, r5.y
mad r2.x, r2.z, c2.z, r2.x
add r0.y, r0.y, r2_abs.x
add r0.y, -r4.y, r0.y
cmp r2.x, r0.y, -c0.y, -c0.x
cmp r0.w, r0.y, r0.w, r1.w
cmp r1.w, r0.y, r4.w, r3.w
add r2.y, -r2.w, r0.w
add r2.z, -r2.w, r1.w
add r0.w, r2.w, r0.w
mul r0.w, r0.w, c3.w
add r1.w, r2.w, r1.w
mul r1.w, r1.w, c3.w
add r3.w, -r2_abs.z, r2_abs.y
cmp r0.w, r3.w, r0.w, r1.w
max r1.w, r2_abs.y, r2_abs.z
cmp r2.x, r3.w, r2.x, -r2.x
mul r2.y, r2.x, c3.w
cmp r2.z, r0.y, c1.x, r2.y
cmp r2.y, r0.y, r2.y, c1.x
add r5.xy, r2.zyzw, v0
mul r6, r0.zxxz, c0.xxxy
cmp r0.xz, r0.y, r6.xyyw, r6.zyww
mad r5, r0.xzxz, c5.xxyy, r5.xyxy
add r2.yz, r0.xxzw, r0.xxzw
mov r4.yz, r5.xxyw
mov r6.xy, r5.zwzw
mov r3.w, r0.w
mov r4.w, r0.w
mov r6.zw, c1.x
rep i0
if_ne r6.z, -r6.z
mov r7.x, r3.w
else
texldd r8, r4.yzzw, s0, r2.yzzw, r2.yzzw
mad r7.x, r8.y, c1.w, r8.x
endif
if_ne r6.w, -r6.w
mov r7.y, r4.w
else
texldd r8, r6, s0, r2.yzzw, r2.yzzw
mad r7.y, r8.y, c1.w, r8.x
endif
add r7.zw, -r0.w, r7.xyxy
mad r7.z, r1.w, -c2.z, r7_abs.z
cmp r7.z, r7.z, c1.z, c1.x
mad r7.w, r1.w, -c2.z, r7_abs.w
cmp r7.w, r7.w, c1.z, c1.x
add r7.zw, r6, r7
cmp r6.zw, -r7, c1.x, c1.z
mul r8.x, r6.w, r6.z
if_ne r8.x, -r8.x
mov r3.w, r7.x
mov r4.w, r7.y
break_ne c1.z, -c1.z
endif
mad r8.xy, r0.xzzw, -c5.w, r4.yzzw
cmp r4.yz, -r7.z, r8.xxyw, r4
mad r8.xy, r0.xzzw, c5.w, r6
cmp r6.xy, -r7.w, r8, r6
mov r3.w, r7.x
mov r4.w, r7.y
endrep
add r0.xz, -r4.yyzw, v0.xyyw
cmp r0.x, r0.y, r0.x, r0.z
add r2.yz, r6.xxyw, -v0.xxyw
cmp r0.z, r0.y, r2.y, r2.z
add r1.w, -r0.z, r0.x
cmp r1.w, r1.w, r4.w, r3.w
add r2.y, -r0.w, r2.w
cmp r2.y, r2.y, c1.x, c1.z
add r0.w, -r0.w, r1.w
cmp r0.w, r0.w, -c1.x, -c1.z
add r0.w, r0.w, r2.y
cmp r0.w, -r0_abs.w, c1.x, r2.x
add r1.w, r0.x, r0.z
min r2.x, r0.z, r0.x
rcp r0.x, r1.w
mad r0.x, r2.x, -r0.x, c3.w
mul r0.x, r0.w, r0.x
cmp r0.z, r0.y, c1.x, r0.x
cmp r0.x, r0.y, r0.x, c1.x
add r2.xy, r0.zxzw, v0
mov r2.zw, c1.x
texldl r0, r2, s0
mad r1.xyz, r1, c4.x, r0
mad r3.xyz, -r4.x, r0, r1
endif
mov oC0.xyz, r3
mov oC0.w, c1.x

""
}
SubProgram ""d3d11 "" {
// Stats: 108 math, 8 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 112
Vector 96 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecededjnknonegacfbfckanhoanpgbkppcnhabaaaaaakibeaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcoibdaaaa
eaaaaaaapkaeaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacajaaaaaadcaaaaanpcaabaaaaaaaaaaa
egiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaaialpaaaaialpaaaaaaaa
egbebaaaabaaaaaaeiaaaaalpcaabaaaabaaaaaaegaabaaaaaaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaaaaaaaaaa
ogakbaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaa
eiaaaaalpcaabaaaacaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaadcaaaaanpcaabaaaadaaaaaaegiecaaaaaaaaaaa
agaaaaaaaceaaaaaaaaaiadpaaaaaaaaaaaaaaaaaaaaiadpegbebaaaabaaaaaa
eiaaaaalpcaabaaaaeaaaaaaegaabaaaadaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaaadaaaaaaogakbaaaadaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadcaaaaajicaabaaa
aaaaaaaabkaabaaaabaaaaaaabeaaaaahnekpldpakaabaaaabaaaaaadcaaaaaj
icaabaaaabaaaaaabkaabaaaaaaaaaaaabeaaaaahnekpldpakaabaaaaaaaaaaa
dcaaaaajicaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaahnekpldpakaabaaa
acaaaaaadcaaaaajicaabaaaadaaaaaabkaabaaaaeaaaaaaabeaaaaahnekpldp
akaabaaaaeaaaaaadcaaaaajicaabaaaaeaaaaaabkaabaaaadaaaaaaabeaaaaa
hnekpldpakaabaaaadaaaaaaddaaaaahbcaabaaaafaaaaaadkaabaaaaaaaaaaa
dkaabaaaabaaaaaaddaaaaahccaabaaaafaaaaaadkaabaaaadaaaaaadkaabaaa
aeaaaaaaddaaaaahbcaabaaaafaaaaaabkaabaaaafaaaaaaakaabaaaafaaaaaa
ddaaaaahbcaabaaaafaaaaaadkaabaaaacaaaaaaakaabaaaafaaaaaadeaaaaah
ccaabaaaafaaaaaadkaabaaaaaaaaaaadkaabaaaabaaaaaadeaaaaahecaabaaa
afaaaaaadkaabaaaadaaaaaadkaabaaaaeaaaaaadeaaaaahccaabaaaafaaaaaa
ckaabaaaafaaaaaabkaabaaaafaaaaaadeaaaaahccaabaaaafaaaaaadkaabaaa
acaaaaaabkaabaaaafaaaaaaaaaaaaaibcaabaaaafaaaaaaakaabaiaebaaaaaa
afaaaaaabkaabaaaafaaaaaadiaaaaahccaabaaaafaaaaaabkaabaaaafaaaaaa
abeaaaaaaaaaaadodeaaaaahccaabaaaafaaaaaabkaabaaaafaaaaaaabeaaaaa
klkkckdnbnaaaaahccaabaaaafaaaaaaakaabaaaafaaaaaabkaabaaaafaaaaaa
bpaaaeadbkaabaaaafaaaaaaaaaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaa
egacbaaaabaaaaaaaaaaaaahhcaabaaaaaaaaaaaegacbaaaacaaaaaaegacbaaa
aaaaaaaaaaaaaaahhcaabaaaaaaaaaaaegacbaaaaeaaaaaaegacbaaaaaaaaaaa
aaaaaaahhcaabaaaaaaaaaaaegacbaaaadaaaaaaegacbaaaaaaaaaaaaaaaaaah
bcaabaaaabaaaaaadkaabaaaaaaaaaaadkaabaaaabaaaaaaaaaaaaahbcaabaaa
abaaaaaadkaabaaaadaaaaaaakaabaaaabaaaaaaaaaaaaahbcaabaaaabaaaaaa
dkaabaaaaeaaaaaaakaabaaaabaaaaaadcaaaaakbcaabaaaabaaaaaaakaabaaa
abaaaaaaabeaaaaaaaaaiadodkaabaiaebaaaaaaacaaaaaaaoaaaaaibcaabaaa
abaaaaaaakaabaiaibaaaaaaabaaaaaaakaabaaaafaaaaaaaaaaaaahbcaabaaa
abaaaaaaakaabaaaabaaaaaaabeaaaaaaaaaialodeaaaaahbcaabaaaabaaaaaa
akaabaaaabaaaaaaabeaaaaaaaaaaaaadiaaaaahbcaabaaaabaaaaaaakaabaaa
abaaaaaaabeaaaaaklkkkkdpddaaaaahbcaabaaaabaaaaaaakaabaaaabaaaaaa
abeaaaaaaaaaeadpaaaaaaajgcaabaaaabaaaaaaagbbbaaaabaaaaaaagibcaia
ebaaaaaaaaaaaaaaagaaaaaaeiaaaaalpcaabaaaafaaaaaajgafbaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadcaaaaanpcaabaaa
agaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaiadpaaaaialpaaaaialp
aaaaiadpegbebaaaabaaaaaaeiaaaaalpcaabaaaahaaaaaaegaabaaaagaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaa
agaaaaaaogakbaaaagaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaa
aaaaaaaaaaaaaaaigcaabaaaabaaaaaaagbbbaaaabaaaaaaagibcaaaaaaaaaaa
agaaaaaaeiaaaaalpcaabaaaaiaaaaaajgafbaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaabeaaaaaaaaaaaaaaaaaaaahhcaabaaaadaaaaaaegacbaaa
afaaaaaaegacbaaaahaaaaaaaaaaaaahhcaabaaaadaaaaaaegacbaaaagaaaaaa
egacbaaaadaaaaaaaaaaaaahhcaabaaaadaaaaaaegacbaaaaiaaaaaaegacbaaa
adaaaaaaaaaaaaahhcaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaadaaaaaa
diaaaaahhcaabaaaaaaaaaaaagaabaaaabaaaaaaegacbaaaaaaaaaaadcaaaaaj
ccaabaaaabaaaaaabkaabaaaafaaaaaaabeaaaaahnekpldpakaabaaaafaaaaaa
dcaaaaajecaabaaaabaaaaaabkaabaaaahaaaaaaabeaaaaahnekpldpakaabaaa
ahaaaaaadcaaaaajbcaabaaaadaaaaaabkaabaaaagaaaaaaabeaaaaahnekpldp
akaabaaaagaaaaaadcaaaaajccaabaaaadaaaaaabkaabaaaaiaaaaaaabeaaaaa
hnekpldpakaabaaaaiaaaaaadiaaaaahecaabaaaadaaaaaadkaabaaaaaaaaaaa
abeaaaaaaaaaaalpdcaaaaajecaabaaaadaaaaaabkaabaaaabaaaaaaabeaaaaa
aaaaiadockaabaaaadaaaaaadcaaaaajecaabaaaadaaaaaackaabaaaabaaaaaa
abeaaaaaaaaaiadockaabaaaadaaaaaadiaaaaahbcaabaaaaeaaaaaadkaabaaa
abaaaaaaabeaaaaaaaaaaalpdcaaaaakccaabaaaaeaaaaaadkaabaaaabaaaaaa
abeaaaaaaaaaaadpdkaabaiaebaaaaaaacaaaaaadiaaaaahecaabaaaaeaaaaaa
dkaabaaaadaaaaaaabeaaaaaaaaaaalpdcaaaaajccaabaaaaeaaaaaadkaabaaa
adaaaaaaabeaaaaaaaaaaadpbkaabaaaaeaaaaaaaaaaaaajecaabaaaadaaaaaa
ckaabaiaibaaaaaaadaaaaaabkaabaiaibaaaaaaaeaaaaaadiaaaaahccaabaaa
aeaaaaaadkaabaaaaeaaaaaaabeaaaaaaaaaaalpdcaaaaajccaabaaaaeaaaaaa
akaabaaaadaaaaaaabeaaaaaaaaaiadobkaabaaaaeaaaaaadcaaaaajccaabaaa
aeaaaaaabkaabaaaadaaaaaaabeaaaaaaaaaiadobkaabaaaaeaaaaaaaaaaaaai
ecaabaaaadaaaaaackaabaaaadaaaaaabkaabaiaibaaaaaaaeaaaaaadcaaaaaj
ccaabaaaabaaaaaabkaabaaaabaaaaaaabeaaaaaaaaaiadoakaabaaaaeaaaaaa
dcaaaaajccaabaaaabaaaaaaakaabaaaadaaaaaaabeaaaaaaaaaiadobkaabaaa
abaaaaaadcaaaaakbcaabaaaadaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaadp
dkaabaiaebaaaaaaacaaaaaadcaaaaajbcaabaaaadaaaaaadkaabaaaaeaaaaaa
abeaaaaaaaaaaadpakaabaaaadaaaaaaaaaaaaajccaabaaaabaaaaaabkaabaia
ibaaaaaaabaaaaaaakaabaiaibaaaaaaadaaaaaadcaaaaajecaabaaaabaaaaaa
ckaabaaaabaaaaaaabeaaaaaaaaaiadockaabaaaaeaaaaaadcaaaaajecaabaaa
abaaaaaabkaabaaaadaaaaaaabeaaaaaaaaaiadockaabaaaabaaaaaaaaaaaaai
ccaabaaaabaaaaaackaabaiaibaaaaaaabaaaaaabkaabaaaabaaaaaabnaaaaah
ccaabaaaabaaaaaabkaabaaaabaaaaaackaabaaaadaaaaaadhaaaaanecaabaaa
abaaaaaabkaabaaaabaaaaaabkiacaiaebaaaaaaaaaaaaaaagaaaaaaakiacaia
ebaaaaaaaaaaaaaaagaaaaaadhaaaaajicaabaaaaaaaaaaabkaabaaaabaaaaaa
dkaabaaaaaaaaaaadkaabaaaabaaaaaadhaaaaajicaabaaaabaaaaaabkaabaaa
abaaaaaadkaabaaaaeaaaaaadkaabaaaadaaaaaaaaaaaaaibcaabaaaadaaaaaa
dkaabaiaebaaaaaaacaaaaaadkaabaaaaaaaaaaaaaaaaaaiccaabaaaadaaaaaa
dkaabaiaebaaaaaaacaaaaaadkaabaaaabaaaaaaaaaaaaahicaabaaaaaaaaaaa
dkaabaaaacaaaaaadkaabaaaaaaaaaaadiaaaaahicaabaaaaaaaaaaadkaabaaa
aaaaaaaaabeaaaaaaaaaaadpaaaaaaahicaabaaaabaaaaaadkaabaaaacaaaaaa
dkaabaaaabaaaaaadiaaaaahicaabaaaabaaaaaadkaabaaaabaaaaaaabeaaaaa
aaaaaadpbnaaaaajecaabaaaadaaaaaaakaabaiaibaaaaaaadaaaaaabkaabaia
ibaaaaaaadaaaaaadhaaaaajicaabaaaaaaaaaaackaabaaaadaaaaaadkaabaaa
aaaaaaaadkaabaaaabaaaaaadhaaaaalicaabaaaabaaaaaackaabaaaadaaaaaa
akaabaiaibaaaaaaadaaaaaabkaabaiaibaaaaaaadaaaaaadhaaaaakecaabaaa
abaaaaaackaabaaaadaaaaaackaabaaaabaaaaaackaabaiaebaaaaaaabaaaaaa
diaaaaahbcaabaaaadaaaaaackaabaaaabaaaaaaabeaaaaaaaaaaadpdhaaaaaj
ccaabaaaadaaaaaabkaabaaaabaaaaaaabeaaaaaaaaaaaaaakaabaaaadaaaaaa
abaaaaahbcaabaaaadaaaaaabkaabaaaabaaaaaaakaabaaaadaaaaaaaaaaaaah
dcaabaaaaeaaaaaabgafbaaaadaaaaaaegbabaaaabaaaaaadiaaaaahicaabaaa
abaaaaaadkaabaaaabaaaaaaabeaaaaaaaaaiadodgaaaaaigcaabaaaadaaaaaa
aceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadgaaaaagjcaabaaaadaaaaaa
agiecaaaaaaaaaaaagaaaaaadhaaaaajdcaabaaaadaaaaaafgafbaaaabaaaaaa
egaabaaaadaaaaaaogakbaaaadaaaaaadcaaaaampcaabaaaaeaaaaaaegaebaaa
adaaaaaaaceaaaaaaaaamalpaaaamalpaaaamadpaaaamadpegaebaaaaeaaaaaa
aaaaaaahmcaabaaaadaaaaaaagaebaaaadaaaaaaagaebaaaadaaaaaadgaaaaaf
pcaabaaaafaaaaaaegaobaaaaeaaaaaadgaaaaafdcaabaaaagaaaaaapgapbaaa
aaaaaaaadgaaaaaimcaabaaaagaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
aaaaaaaadgaaaaafbcaabaaaahaaaaaaabeaaaaaaaaaaaaadaaaaaabcbaaaaah
ccaabaaaahaaaaaaakaabaaaahaaaaaaabeaaaaaaiaaaaaaadaaaeadbkaabaaa
ahaaaaaabpaaaaadckaabaaaagaaaaaaejaaaaanpcaabaaaaiaaaaaaegaabaaa
afaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaogakbaaaadaaaaaaogakbaaa
adaaaaaadcaaaaajccaabaaaahaaaaaabkaabaaaaiaaaaaaabeaaaaahnekpldp
akaabaaaaiaaaaaabcaaaaabdgaaaaafccaabaaaahaaaaaaakaabaaaagaaaaaa
bfaaaaabbpaaaaaddkaabaaaagaaaaaaejaaaaanpcaabaaaaiaaaaaaogakbaaa
afaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaogakbaaaadaaaaaaogakbaaa
adaaaaaadcaaaaajecaabaaaahaaaaaabkaabaaaaiaaaaaaabeaaaaahnekpldp
akaabaaaaiaaaaaabcaaaaabdgaaaaafecaabaaaahaaaaaabkaabaaaagaaaaaa
bfaaaaabaaaaaaaiicaabaaaahaaaaaadkaabaiaebaaaaaaaaaaaaaabkaabaaa
ahaaaaaabnaaaaaiicaabaaaahaaaaaadkaabaiaibaaaaaaahaaaaaadkaabaaa
abaaaaaadmaaaaahecaabaaaagaaaaaackaabaaaagaaaaaadkaabaaaahaaaaaa
aaaaaaaiicaabaaaahaaaaaadkaabaiaebaaaaaaaaaaaaaackaabaaaahaaaaaa
bnaaaaaiicaabaaaahaaaaaadkaabaiaibaaaaaaahaaaaaadkaabaaaabaaaaaa
dmaaaaahicaabaaaagaaaaaadkaabaaaagaaaaaadkaabaaaahaaaaaaabaaaaah
icaabaaaahaaaaaadkaabaaaagaaaaaackaabaaaagaaaaaabpaaaeaddkaabaaa
ahaaaaaadgaaaaafdcaabaaaagaaaaaajgafbaaaahaaaaaaacaaaaabbfaaaaab
dcaaaaandcaabaaaaiaaaaaaegaabaiaebaaaaaaadaaaaaaaceaaaaaaaaaaaea
aaaaaaeaaaaaaaaaaaaaaaaaegaabaaaafaaaaaadhaaaaajdcaabaaaafaaaaaa
kgakbaaaagaaaaaaegaabaaaafaaaaaaegaabaaaaiaaaaaadcaaaaamdcaabaaa
aiaaaaaaegaabaaaadaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaaaaaaaaaaa
ogakbaaaafaaaaaadhaaaaajmcaabaaaafaaaaaapgapbaaaagaaaaaakgaobaaa
afaaaaaaagaebaaaaiaaaaaaboaaaaahbcaabaaaahaaaaaaakaabaaaahaaaaaa
abeaaaaaabaaaaaadgaaaaafdcaabaaaagaaaaaajgafbaaaahaaaaaabgaaaaab
aaaaaaaidcaabaaaadaaaaaaegaabaiaebaaaaaaafaaaaaaegbabaaaabaaaaaa
dhaaaaajicaabaaaabaaaaaabkaabaaaabaaaaaaakaabaaaadaaaaaabkaabaaa
adaaaaaaaaaaaaaidcaabaaaadaaaaaaogakbaaaafaaaaaaegbabaiaebaaaaaa
abaaaaaadhaaaaajbcaabaaaadaaaaaabkaabaaaabaaaaaaakaabaaaadaaaaaa
bkaabaaaadaaaaaadbaaaaahccaabaaaadaaaaaadkaabaaaabaaaaaaakaabaaa
adaaaaaadhaaaaajecaabaaaadaaaaaabkaabaaaadaaaaaaakaabaaaagaaaaaa
bkaabaaaagaaaaaaaaaaaaaiicaabaaaacaaaaaadkaabaiaebaaaaaaaaaaaaaa
dkaabaaaacaaaaaadbaaaaahicaabaaaacaaaaaadkaabaaaacaaaaaaabeaaaaa
aaaaaaaaaaaaaaaiicaabaaaaaaaaaaadkaabaiaebaaaaaaaaaaaaaackaabaaa
adaaaaaadbaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaaaa
caaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaaacaaaaaadhaaaaaj
icaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaaaackaabaaaabaaaaaa
aaaaaaahecaabaaaabaaaaaadkaabaaaabaaaaaaakaabaaaadaaaaaadhaaaaaj
icaabaaaabaaaaaabkaabaaaadaaaaaadkaabaaaabaaaaaaakaabaaaadaaaaaa
aoaaaaahecaabaaaabaaaaaaabeaaaaaaaaaialpckaabaaaabaaaaaadcaaaaaj
ecaabaaaabaaaaaadkaabaaaabaaaaaackaabaaaabaaaaaaabeaaaaaaaaaaadp
diaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaackaabaaaabaaaaaadhaaaaaj
ecaabaaaabaaaaaabkaabaaaabaaaaaaabeaaaaaaaaaaaaadkaabaaaaaaaaaaa
aaaaaaahbcaabaaaadaaaaaackaabaaaabaaaaaaakbabaaaabaaaaaaabaaaaah
icaabaaaaaaaaaaadkaabaaaaaaaaaaabkaabaaaabaaaaaaaaaaaaahccaabaaa
adaaaaaadkaabaaaaaaaaaaabkbabaaaabaaaaaaeiaaaaalpcaabaaaadaaaaaa
egaabaaaadaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaa
dcaaaaamhcaabaaaaaaaaaaaegacbaaaaaaaaaaaaceaaaaadjiooddndjiooddn
djiooddnaaaaaaaaegacbaaaadaaaaaadcaaaaakhcaabaaaacaaaaaaagaabaia
ebaaaaaaabaaaaaaegacbaaaadaaaaaaegacbaaaaaaaaaaabfaaaaabdgaaaaaf
hccabaaaaaaaaaaaegacbaaaacaaaaaadgaaaaaficcabaaaaaaaaaaaabeaaaaa
aaaaaaaadoaaaaab""
}
}
 }
}
Fallback ""Hidden/FXAA II""
}";

		private const String fxaa3ConsoleShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 18.3KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/FXAA III (Console)"" {
Properties {
 _MainTex (""-"", 2D) = ""white"" { }
 _EdgeThresholdMin (""Edge threshold min"", Float) = 0.125
 _EdgeThreshold (""Edge Threshold"", Float) = 0.25
 _EdgeSharpness (""Edge sharpness"", Float) = 4
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 9 math
 //        d3d9 : 12 math
 //      opengl : 109 math, 9 texture, 3 branch
 // Stats for Fragment shader:
 //       d3d11 : 83 math, 1 branch
 //        d3d9 : 108 math, 18 texture, 2 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 40769
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 109 math, 9 textures, 3 branches
""!!GLSL
#ifdef VERTEX

uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
void main ()
{
  vec4 rcpSize_1;
  vec4 extents_2;
  vec4 tmpvar_3;
  vec2 cse_4;
  cse_4 = (_MainTex_TexelSize.xy * 0.5);
  extents_2.xy = (gl_MultiTexCoord0.xy - cse_4);
  extents_2.zw = (gl_MultiTexCoord0.xy + cse_4);
  rcpSize_1.xy = (-(_MainTex_TexelSize.xy) * 0.5);
  rcpSize_1.zw = cse_4;
  tmpvar_3.xy = (rcpSize_1.xy * 4.0);
  tmpvar_3.zw = (cse_4 * 4.0);
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD1 = extents_2;
  xlv_TEXCOORD2 = rcpSize_1;
  xlv_TEXCOORD3 = tmpvar_3;
}


#endif
#ifdef FRAGMENT
#extension GL_ARB_shader_texture_lod : enable
uniform vec4 unity_ColorSpaceLuminance;
uniform sampler2D _MainTex;
uniform float _EdgeThresholdMin;
uniform float _EdgeThreshold;
uniform float _EdgeSharpness;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
void main ()
{
  vec3 tmpvar_1;
  vec2 dir_2;
  float tmpvar_3;
  vec3 tmpvar_4;
  tmpvar_4 = (texture2DLod (_MainTex, xlv_TEXCOORD1.xy, 0.0).xyz * unity_ColorSpaceLuminance.xyz);
  tmpvar_3 = (((tmpvar_4.x + tmpvar_4.y) + tmpvar_4.z) + ((2.0 * 
    sqrt((tmpvar_4.y * (tmpvar_4.x + tmpvar_4.z)))
  ) * unity_ColorSpaceLuminance.w));
  float tmpvar_5;
  vec3 tmpvar_6;
  tmpvar_6 = (texture2DLod (_MainTex, xlv_TEXCOORD1.xw, 0.0).xyz * unity_ColorSpaceLuminance.xyz);
  tmpvar_5 = (((tmpvar_6.x + tmpvar_6.y) + tmpvar_6.z) + ((2.0 * 
    sqrt((tmpvar_6.y * (tmpvar_6.x + tmpvar_6.z)))
  ) * unity_ColorSpaceLuminance.w));
  vec3 tmpvar_7;
  tmpvar_7 = (texture2DLod (_MainTex, xlv_TEXCOORD1.zy, 0.0).xyz * unity_ColorSpaceLuminance.xyz);
  float tmpvar_8;
  vec3 tmpvar_9;
  tmpvar_9 = (texture2DLod (_MainTex, xlv_TEXCOORD1.zw, 0.0).xyz * unity_ColorSpaceLuminance.xyz);
  tmpvar_8 = (((tmpvar_9.x + tmpvar_9.y) + tmpvar_9.z) + ((2.0 * 
    sqrt((tmpvar_9.y * (tmpvar_9.x + tmpvar_9.z)))
  ) * unity_ColorSpaceLuminance.w));
  vec4 tmpvar_10;
  tmpvar_10 = texture2DLod (_MainTex, xlv_TEXCOORD0, 0.0);
  float tmpvar_11;
  vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_10.xyz * unity_ColorSpaceLuminance.xyz);
  tmpvar_11 = (((tmpvar_12.x + tmpvar_12.y) + tmpvar_12.z) + ((2.0 * 
    sqrt((tmpvar_12.y * (tmpvar_12.x + tmpvar_12.z)))
  ) * unity_ColorSpaceLuminance.w));
  float tmpvar_13;
  tmpvar_13 = (((
    (tmpvar_7.x + tmpvar_7.y)
   + tmpvar_7.z) + (
    (2.0 * sqrt((tmpvar_7.y * (tmpvar_7.x + tmpvar_7.z))))
   * unity_ColorSpaceLuminance.w)) + 0.002604167);
  float tmpvar_14;
  tmpvar_14 = max (max (tmpvar_13, tmpvar_8), max (tmpvar_3, tmpvar_5));
  float tmpvar_15;
  tmpvar_15 = min (min (tmpvar_13, tmpvar_8), min (tmpvar_3, tmpvar_5));
  float tmpvar_16;
  tmpvar_16 = max (_EdgeThresholdMin, (tmpvar_14 * _EdgeThreshold));
  float tmpvar_17;
  tmpvar_17 = (tmpvar_5 - tmpvar_13);
  float tmpvar_18;
  tmpvar_18 = (max (tmpvar_14, tmpvar_11) - min (tmpvar_15, tmpvar_11));
  float tmpvar_19;
  tmpvar_19 = (tmpvar_8 - tmpvar_3);
  if ((tmpvar_18 < tmpvar_16)) {
    tmpvar_1 = tmpvar_10.xyz;
  } else {
    dir_2.x = (tmpvar_17 + tmpvar_19);
    dir_2.y = (tmpvar_17 - tmpvar_19);
    vec2 tmpvar_20;
    tmpvar_20 = normalize(dir_2);
    vec4 tmpvar_21;
    tmpvar_21.zw = vec2(0.0, 0.0);
    tmpvar_21.xy = (xlv_TEXCOORD0 - (tmpvar_20 * xlv_TEXCOORD2.zw));
    vec4 tmpvar_22;
    tmpvar_22.zw = vec2(0.0, 0.0);
    tmpvar_22.xy = (xlv_TEXCOORD0 + (tmpvar_20 * xlv_TEXCOORD2.zw));
    vec2 tmpvar_23;
    tmpvar_23 = clamp ((tmpvar_20 / (
      min (abs(tmpvar_20.x), abs(tmpvar_20.y))
     * _EdgeSharpness)), vec2(-2.0, -2.0), vec2(2.0, 2.0));
    dir_2 = tmpvar_23;
    vec4 tmpvar_24;
    tmpvar_24.zw = vec2(0.0, 0.0);
    tmpvar_24.xy = (xlv_TEXCOORD0 - (tmpvar_23 * xlv_TEXCOORD3.zw));
    vec4 tmpvar_25;
    tmpvar_25.zw = vec2(0.0, 0.0);
    tmpvar_25.xy = (xlv_TEXCOORD0 + (tmpvar_23 * xlv_TEXCOORD3.zw));
    vec3 tmpvar_26;
    tmpvar_26 = (texture2DLod (_MainTex, tmpvar_21.xy, 0.0).xyz + texture2DLod (_MainTex, tmpvar_22.xy, 0.0).xyz);
    vec3 tmpvar_27;
    tmpvar_27 = (((texture2DLod (_MainTex, tmpvar_24.xy, 0.0).xyz + texture2DLod (_MainTex, tmpvar_25.xy, 0.0).xyz) * 0.25) + (tmpvar_26 * 0.25));
    float tmpvar_28;
    vec3 tmpvar_29;
    tmpvar_29 = (tmpvar_26 * unity_ColorSpaceLuminance.xyz);
    tmpvar_28 = (((tmpvar_29.x + tmpvar_29.y) + tmpvar_29.z) + ((2.0 * 
      sqrt((tmpvar_29.y * (tmpvar_29.x + tmpvar_29.z)))
    ) * unity_ColorSpaceLuminance.w));
    bool tmpvar_30;
    if ((tmpvar_28 < tmpvar_15)) {
      tmpvar_30 = bool(1);
    } else {
      vec3 tmpvar_31;
      tmpvar_31 = (tmpvar_27 * unity_ColorSpaceLuminance.xyz);
      tmpvar_30 = (((
        (tmpvar_31.x + tmpvar_31.y)
       + tmpvar_31.z) + (
        (2.0 * sqrt((tmpvar_31.y * (tmpvar_31.x + tmpvar_31.z))))
       * unity_ColorSpaceLuminance.w)) > tmpvar_14);
    };
    if (tmpvar_30) {
      tmpvar_1 = (tmpvar_26 * 0.5);
    } else {
      tmpvar_1 = tmpvar_27;
    };
  };
  vec4 tmpvar_32;
  tmpvar_32.w = 1.0;
  tmpvar_32.xyz = tmpvar_1;
  gl_FragData[0] = tmpvar_32;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 12 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, -0.5, 0.5, -2, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2
dcl_texcoord2 o3
dcl_texcoord3 o4
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.xy, c4
mad o2.xy, r0, -c5.y, v1
mad o2.zw, r0.xyxy, c5.y, v1.xyxy
mul o4.xy, r0, c5.z
add o4.zw, c4.xyxy, c4.xyxy
mov o1.xy, v1
mul r0, r0.xyxy, c5.xxyy
mov o3, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 9 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 128
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedcfjfadhnfkmlccjpdfpnpaaehcfpmdjaabaaaaaafmadaaaaadaaaaaa
cmaaaaaaiaaaaaaacaabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheojiaaaaaaafaaaaaaaiaaaaaaiaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaimaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
imaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapaaaaaaimaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaapaaaaaaimaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl
fdeieefcdeacaaaaeaaaabaainaaaaaafjaaaaaeegiocaaaaaaaaaaaaiaaaaaa
fjaaaaaeegiocaaaabaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaad
dcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaa
abaaaaaagfaaaaadpccabaaaacaaaaaagfaaaaadpccabaaaadaaaaaagfaaaaad
pccabaaaaeaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaa
aaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaa
egaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaadcaaaaao
dccabaaaacaaaaaaegiacaiaebaaaaaaaaaaaaaaahaaaaaaaceaaaaaaaaaaadp
aaaaaadpaaaaaaaaaaaaaaaaegbabaaaabaaaaaadcaaaaanmccabaaaacaaaaaa
agiecaaaaaaaaaaaahaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaadpaaaaaadp
agbebaaaabaaaaaadiaaaaalpcaabaaaaaaaaaaaegiecaaaaaaaaaaaahaaaaaa
aceaaaaaaaaaaalpaaaaaalpaaaaaadpaaaaaadpdgaaaaafpccabaaaadaaaaaa
egaobaaaaaaaaaaadiaaaaaldccabaaaaeaaaaaaegiacaaaaaaaaaaaahaaaaaa
aceaaaaaaaaaaamaaaaaaamaaaaaaaaaaaaaaaaaaaaaaaajmccabaaaaeaaaaaa
agiecaaaaaaaaaaaahaaaaaaagiecaaaaaaaaaaaahaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 108 math, 18 textures, 2 branches
Float 3 [_EdgeSharpness]
Float 2 [_EdgeThreshold]
Float 1 [_EdgeThresholdMin]
Vector 0 [unity_ColorSpaceLuminance]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c4, 1, 0, 0.00260416674, -2
def c5, 0.25, 0.5, 0, 0
dcl_texcoord v0.xy
dcl_texcoord1 v1
dcl_texcoord2 v2.zw
dcl_texcoord3 v3.zw
dcl_2d s0
mul r0, c4.xxyy, v1.xyxx
texldl_pp r0, r0, s0
mul_pp r0.xyw, r0.xyzz, c0.xyzz
add_pp r0.xw, r0.yyzw, r0.x
mad_pp r0.x, r0.z, c0.z, r0.x
mul_pp r0.y, r0.w, r0.y
rsq_pp r0.y, r0.y
rcp_pp r0.y, r0.y
dp2add_pp r0.x, c0.w, r0.y, r0.x
mul r1, c4.xxyy, v1.xwxx
texldl_pp r1, r1, s0
mul_pp r0.yzw, r1.xxyz, c0.xxyz
add_pp r0.yw, r0.xzzw, r0.y
mad_pp r0.y, r1.z, c0.z, r0.y
mul_pp r0.z, r0.w, r0.z
rsq_pp r0.z, r0.z
rcp_pp r0.z, r0.z
dp2add_pp r0.y, c0.w, r0.z, r0.y
mul r1, c4.xxyy, v1.zyxx
texldl_pp r1, r1, s0
mul_pp r1.xyw, r1.xyzz, c0.xyzz
add_pp r0.zw, r1.xyyw, r1.x
mad_pp r0.z, r1.z, c0.z, r0.z
mul_pp r0.w, r0.w, r1.y
rsq_pp r0.w, r0.w
rcp_pp r0.w, r0.w
dp2add_pp r0.z, c0.w, r0.w, r0.z
mul r1, c4.xxyy, v1.zwxx
texldl_pp r1, r1, s0
mul_pp r1.xyw, r1.xyzz, c0.xyzz
add_pp r1.xw, r1.yyzw, r1.x
mad_pp r0.w, r1.z, c0.z, r1.x
mul_pp r1.x, r1.w, r1.y
rsq_pp r1.x, r1.x
rcp_pp r1.x, r1.x
dp2add_pp r0.w, c0.w, r1.x, r0.w
mul r1, c4.xxyy, v0.xyxx
texldl_pp r1, r1, s0
mul_pp r2.xyz, r1, c0
add_pp r2.xz, r2.yyzw, r2.x
mad_pp r1.w, r1.z, c0.z, r2.x
mul_pp r2.x, r2.z, r2.y
rsq_pp r2.x, r2.x
rcp_pp r2.x, r2.x
dp2add_pp r1.w, c0.w, r2.x, r1.w
max_pp r2.x, r0.x, r0.y
add_pp r0.z, r0.z, c4.z
min_pp r2.y, r0.y, r0.x
max_pp r2.z, r0.z, r0.w
min_pp r2.w, r0.w, r0.z
max_pp r3.x, r2.z, r2.x
min_pp r3.y, r2.y, r2.w
mul_pp r2.x, r3.x, c2.x
min_pp r2.y, r1.w, r3.y
max_pp r3.z, c1.x, r2.x
max_pp r2.x, r3.x, r1.w
add_pp r1.w, -r2.y, r2.x
if_lt r1.w, r3.z
else
add_pp r0.xy, -r0.xzzw, r0.wyzw
add_pp r2.x, r0.x, r0.y
add_pp r2.y, -r0.x, r0.y
dp2add_pp r0.x, r2, r2, c4.y
rsq_pp r0.x, r0.x
mul_pp r0.xy, r0.x, r2
mov r2.xy, v0
mad r4.xy, r0, -v2.zwzw, r2
mov r4.zw, c4.y
texldl_pp r4, r4, s0
mad r5.xy, r0, v2.zwzw, r2
mov r5.zw, c4.y
texldl_pp r5, r5, s0
min_pp r1.w, r0_abs.y, r0_abs.x
mul_pp r0.z, r1.w, c3.x
rcp r0.z, r0.z
mul_pp r0.xy, r0.z, r0
max_pp r2.zw, r0.xyxy, c4.w
min_pp r0.xy, r2.zwzw, -c4.w
mad r6.xy, r0, -v3.zwzw, r2
mov r6.zw, c4.y
texldl_pp r6, r6, s0
mad r0.xy, r0, v3.zwzw, r2
mov r0.zw, c4.y
texldl_pp r0, r0, s0
add_pp r2.xyz, r4, r5
add_pp r0.xyz, r0, r6
mul_pp r4.xyz, r2, c5.x
mad_pp r0.xyz, r0, c5.x, r4
mul_pp r4.xyz, r2, c0
add_pp r3.zw, r4.xyyz, r4.x
mad_pp r0.w, r2.z, c0.z, r3.z
mul_pp r1.w, r3.w, r4.y
rsq_pp r1.w, r1.w
rcp_pp r1.w, r1.w
dp2add_pp r0.w, c0.w, r1.w, r0.w
add r0.w, -r3.y, r0.w
cmp r0.w, r0.w, c4.y, c4.x
mul_pp r3.yzw, r0.xxyz, c0.xxyz
add_pp r3.yw, r3.xzzw, r3.y
mad_pp r1.w, r0.z, c0.z, r3.y
mul_pp r2.w, r3.w, r3.z
rsq_pp r2.w, r2.w
rcp_pp r2.w, r2.w
dp2add_pp r1.w, c0.w, r2.w, r1.w
add r1.w, -r1.w, r3.x
cmp r1.w, r1.w, c4.y, c4.x
add r0.w, r0.w, r1.w
mul_pp r2.xyz, r2, c5.y
cmp_pp r1.xyz, -r0.w, r0, r2
endif
mov_pp oC0.xyz, r1
mov_pp oC0.w, c4.x

""
}
SubProgram ""d3d11 "" {
// Stats: 83 math, 1 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 128
Vector 48 [unity_ColorSpaceLuminance]
Float 96 [_EdgeThresholdMin]
Float 100 [_EdgeThreshold]
Float 104 [_EdgeSharpness]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedemfmimiekokehcanbhmkaccohdjjlnfgabaaaaaafianaaaaadaaaaaa
cmaaaaaammaaaaaaaaabaaaaejfdeheojiaaaaaaafaaaaaaaiaaaaaaiaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaimaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaimaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaaimaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaapamaaaaimaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapamaaaafdfgfpfaepfdejfeejepeoaa
feeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl
fdeieefcfaamaaaaeaaaaaaabeadaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaagcbaaaadmcbabaaaadaaaaaa
gcbaaaadmcbabaaaaeaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacagaaaaaa
eiaaaaalpcaabaaaaaaaaaaaegbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaadiaaaaailcaabaaaaaaaaaaaegaibaaaaaaaaaaa
egiicaaaaaaaaaaaadaaaaaaaaaaaaahjcaabaaaaaaaaaaafganbaaaaaaaaaaa
agaabaaaaaaaaaaadcaaaaakbcaabaaaaaaaaaaackaabaaaaaaaaaaackiacaaa
aaaaaaaaadaaaaaaakaabaaaaaaaaaaadiaaaaahccaabaaaaaaaaaaadkaabaaa
aaaaaaaabkaabaaaaaaaaaaaelaaaaafccaabaaaaaaaaaaabkaabaaaaaaaaaaa
apaaaaaiccaabaaaaaaaaaaapgipcaaaaaaaaaaaadaaaaaafgafbaaaaaaaaaaa
aaaaaaahbcaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaaaaaaaaaaeiaaaaal
pcaabaaaabaaaaaamgbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
abeaaaaaaaaaaaaadiaaaaaiocaabaaaaaaaaaaaagajbaaaabaaaaaaagijcaaa
aaaaaaaaadaaaaaaaaaaaaahkcaabaaaaaaaaaaakgaobaaaaaaaaaaafgafbaaa
aaaaaaaadcaaaaakccaabaaaaaaaaaaackaabaaaabaaaaaackiacaaaaaaaaaaa
adaaaaaabkaabaaaaaaaaaaadiaaaaahecaabaaaaaaaaaaadkaabaaaaaaaaaaa
ckaabaaaaaaaaaaaelaaaaafecaabaaaaaaaaaaackaabaaaaaaaaaaaapaaaaai
ecaabaaaaaaaaaaapgipcaaaaaaaaaaaadaaaaaakgakbaaaaaaaaaaaaaaaaaah
ccaabaaaaaaaaaaackaabaaaaaaaaaaabkaabaaaaaaaaaaaeiaaaaalpcaabaaa
abaaaaaaggbkbaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaa
aaaaaaaadiaaaaailcaabaaaabaaaaaaegaibaaaabaaaaaaegiicaaaaaaaaaaa
adaaaaaaaaaaaaahmcaabaaaaaaaaaaafganbaaaabaaaaaaagaabaaaabaaaaaa
dcaaaaakecaabaaaaaaaaaaackaabaaaabaaaaaackiacaaaaaaaaaaaadaaaaaa
ckaabaaaaaaaaaaadiaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaabkaabaaa
abaaaaaaelaaaaaficaabaaaaaaaaaaadkaabaaaaaaaaaaaapaaaaaiicaabaaa
aaaaaaaapgipcaaaaaaaaaaaadaaaaaapgapbaaaaaaaaaaaaaaaaaahecaabaaa
aaaaaaaadkaabaaaaaaaaaaackaabaaaaaaaaaaaeiaaaaalpcaabaaaabaaaaaa
ogbkbaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaa
diaaaaailcaabaaaabaaaaaaegaibaaaabaaaaaaegiicaaaaaaaaaaaadaaaaaa
aaaaaaahjcaabaaaabaaaaaafganbaaaabaaaaaaagaabaaaabaaaaaadcaaaaak
icaabaaaaaaaaaaackaabaaaabaaaaaackiacaaaaaaaaaaaadaaaaaaakaabaaa
abaaaaaadiaaaaahbcaabaaaabaaaaaadkaabaaaabaaaaaabkaabaaaabaaaaaa
elaaaaafbcaabaaaabaaaaaaakaabaaaabaaaaaaapaaaaaibcaabaaaabaaaaaa
pgipcaaaaaaaaaaaadaaaaaaagaabaaaabaaaaaaaaaaaaahicaabaaaaaaaaaaa
dkaabaaaaaaaaaaaakaabaaaabaaaaaaeiaaaaalpcaabaaaabaaaaaaegbabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadiaaaaai
hcaabaaaacaaaaaaegacbaaaabaaaaaaegiccaaaaaaaaaaaadaaaaaaaaaaaaah
fcaabaaaacaaaaaafgagbaaaacaaaaaaagaabaaaacaaaaaadcaaaaakicaabaaa
abaaaaaackaabaaaabaaaaaackiacaaaaaaaaaaaadaaaaaaakaabaaaacaaaaaa
diaaaaahbcaabaaaacaaaaaackaabaaaacaaaaaabkaabaaaacaaaaaaelaaaaaf
bcaabaaaacaaaaaaakaabaaaacaaaaaaapaaaaaibcaabaaaacaaaaaapgipcaaa
aaaaaaaaadaaaaaaagaabaaaacaaaaaaaaaaaaahicaabaaaabaaaaaadkaabaaa
abaaaaaaakaabaaaacaaaaaaaaaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaa
abeaaaaaklkkckdldeaaaaahfcaabaaaacaaaaaafgahbaaaaaaaaaaaagacbaaa
aaaaaaaaddaaaaahkcaabaaaacaaaaaafganbaaaaaaaaaaaagaibaaaaaaaaaaa
deaaaaahbcaabaaaacaaaaaaakaabaaaacaaaaaackaabaaaacaaaaaaddaaaaah
ccaabaaaacaaaaaabkaabaaaacaaaaaadkaabaaaacaaaaaadiaaaaaiecaabaaa
acaaaaaaakaabaaaacaaaaaabkiacaaaaaaaaaaaagaaaaaaddaaaaahicaabaaa
acaaaaaadkaabaaaabaaaaaabkaabaaaacaaaaaadeaaaaaiecaabaaaacaaaaaa
ckaabaaaacaaaaaaakiacaaaaaaaaaaaagaaaaaadeaaaaahicaabaaaabaaaaaa
dkaabaaaabaaaaaaakaabaaaacaaaaaaaaaaaaaiicaabaaaabaaaaaadkaabaia
ebaaaaaaacaaaaaadkaabaaaabaaaaaabnaaaaahicaabaaaabaaaaaadkaabaaa
abaaaaaackaabaaaacaaaaaabpaaaeaddkaabaaaabaaaaaaaaaaaaaidcaabaaa
aaaaaaaaigaabaiaebaaaaaaaaaaaaaahgapbaaaaaaaaaaaaaaaaaahbcaabaaa
adaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaaaaaaaaaiccaabaaaadaaaaaa
akaabaiaebaaaaaaaaaaaaaabkaabaaaaaaaaaaaapaaaaahbcaabaaaaaaaaaaa
egaabaaaadaaaaaaegaabaaaadaaaaaaeeaaaaafbcaabaaaaaaaaaaaakaabaaa
aaaaaaaadiaaaaahdcaabaaaaaaaaaaaagaabaaaaaaaaaaaegaabaaaadaaaaaa
dcaaaaakmcaabaaaaaaaaaaaagaebaiaebaaaaaaaaaaaaaakgbobaaaadaaaaaa
agbebaaaabaaaaaaeiaaaaalpcaabaaaadaaaaaaogakbaaaaaaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadcaaaaajmcaabaaaaaaaaaaa
agaebaaaaaaaaaaakgbobaaaadaaaaaaagbebaaaabaaaaaaeiaaaaalpcaabaaa
aeaaaaaaogakbaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaa
aaaaaaaaddaaaaajecaabaaaaaaaaaaabkaabaiaibaaaaaaaaaaaaaaakaabaia
ibaaaaaaaaaaaaaadiaaaaaiecaabaaaaaaaaaaackaabaaaaaaaaaaackiacaaa
aaaaaaaaagaaaaaaaoaaaaahdcaabaaaaaaaaaaaegaabaaaaaaaaaaakgakbaaa
aaaaaaaadeaaaaakdcaabaaaaaaaaaaaegaabaaaaaaaaaaaaceaaaaaaaaaaama
aaaaaamaaaaaaaaaaaaaaaaaddaaaaakdcaabaaaaaaaaaaaegaabaaaaaaaaaaa
aceaaaaaaaaaaaeaaaaaaaeaaaaaaaaaaaaaaaaadcaaaaakmcaabaaaaaaaaaaa
agaebaiaebaaaaaaaaaaaaaakgbobaaaaeaaaaaaagbebaaaabaaaaaaeiaaaaal
pcaabaaaafaaaaaaogakbaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
abeaaaaaaaaaaaaadcaaaaajdcaabaaaaaaaaaaaegaabaaaaaaaaaaaogbkbaaa
aeaaaaaaegbabaaaabaaaaaaeiaaaaalpcaabaaaaaaaaaaaegaabaaaaaaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaaaaaaaaahhcaabaaa
adaaaaaaegacbaaaadaaaaaaegacbaaaaeaaaaaaaaaaaaahhcaabaaaaaaaaaaa
egacbaaaaaaaaaaaegacbaaaafaaaaaadiaaaaakhcaabaaaaeaaaaaaegacbaaa
adaaaaaaaceaaaaaaaaaiadoaaaaiadoaaaaiadoaaaaaaaadcaaaaamhcaabaaa
aaaaaaaaegacbaaaaaaaaaaaaceaaaaaaaaaiadoaaaaiadoaaaaiadoaaaaaaaa
egacbaaaaeaaaaaadiaaaaaihcaabaaaaeaaaaaaegacbaaaadaaaaaaegiccaaa
aaaaaaaaadaaaaaaaaaaaaahmcaabaaaacaaaaaafgajbaaaaeaaaaaaagaabaaa
aeaaaaaadcaaaaakicaabaaaaaaaaaaackaabaaaadaaaaaackiacaaaaaaaaaaa
adaaaaaackaabaaaacaaaaaadiaaaaahicaabaaaabaaaaaadkaabaaaacaaaaaa
bkaabaaaaeaaaaaaelaaaaaficaabaaaabaaaaaadkaabaaaabaaaaaaapaaaaai
icaabaaaabaaaaaapgipcaaaaaaaaaaaadaaaaaapgapbaaaabaaaaaaaaaaaaah
icaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaaabaaaaaadbaaaaahicaabaaa
aaaaaaaadkaabaaaaaaaaaaabkaabaaaacaaaaaadiaaaaaiocaabaaaacaaaaaa
agajbaaaaaaaaaaaagijcaaaaaaaaaaaadaaaaaaaaaaaaahkcaabaaaacaaaaaa
kgaobaaaacaaaaaafgafbaaaacaaaaaadcaaaaakicaabaaaabaaaaaackaabaaa
aaaaaaaackiacaaaaaaaaaaaadaaaaaabkaabaaaacaaaaaadiaaaaahccaabaaa
acaaaaaadkaabaaaacaaaaaackaabaaaacaaaaaaelaaaaafccaabaaaacaaaaaa
bkaabaaaacaaaaaaapaaaaaiccaabaaaacaaaaaapgipcaaaaaaaaaaaadaaaaaa
fgafbaaaacaaaaaaaaaaaaahicaabaaaabaaaaaadkaabaaaabaaaaaabkaabaaa
acaaaaaadbaaaaahicaabaaaabaaaaaaakaabaaaacaaaaaadkaabaaaabaaaaaa
dmaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaaabaaaaaadiaaaaak
hcaabaaaacaaaaaaegacbaaaadaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaadp
aaaaaaaadhaaaaajhcaabaaaabaaaaaapgapbaaaaaaaaaaaegacbaaaacaaaaaa
egacbaaaaaaaaaaabfaaaaabdgaaaaafhccabaaaaaaaaaaaegacbaaaabaaaaaa
dgaaaaaficcabaaaaaaaaaaaabeaaaaaaaaaiadpdoaaaaab""
}
}
 }
}
Fallback Off
}";

		private const String fxaa2ShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 11.9KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/FXAA II"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = ""white"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 7 math
 //        d3d9 : 9 math
 //      opengl : 64 math, 9 texture, 1 branch
 // Stats for Fragment shader:
 //       d3d11 : 43 math
 //        d3d9 : 60 math, 18 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 55512
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 64 math, 9 textures, 1 branches
""!!GLSL
#ifdef VERTEX

uniform vec4 _MainTex_TexelSize;
varying vec4 xlv_TEXCOORD0;
void main ()
{
  vec4 posPos_1;
  posPos_1.xy = (((
    (gl_MultiTexCoord0.xy * 2.0)
   - 1.0) * 0.5) + 0.5);
  posPos_1.zw = (posPos_1.xy - (_MainTex_TexelSize.xy * 0.75));
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = posPos_1;
}


#endif
#ifdef FRAGMENT
#extension GL_ARB_shader_texture_lod : enable
uniform vec4 _MainTex_TexelSize;
uniform sampler2D _MainTex;
varying vec4 xlv_TEXCOORD0;
void main ()
{
  vec3 tmpvar_1;
  vec2 dir_2;
  vec4 tmpvar_3;
  tmpvar_3.zw = vec2(0.0, 0.0);
  tmpvar_3.xy = (xlv_TEXCOORD0.zw + (vec2(1.0, 0.0) * _MainTex_TexelSize.xy));
  vec4 tmpvar_4;
  tmpvar_4.zw = vec2(0.0, 0.0);
  tmpvar_4.xy = (xlv_TEXCOORD0.zw + (vec2(0.0, 1.0) * _MainTex_TexelSize.xy));
  vec4 tmpvar_5;
  tmpvar_5.zw = vec2(0.0, 0.0);
  tmpvar_5.xy = (xlv_TEXCOORD0.zw + _MainTex_TexelSize.xy);
  float tmpvar_6;
  tmpvar_6 = dot (texture2DLod (_MainTex, xlv_TEXCOORD0.zw, 0.0).xyz, vec3(0.299, 0.587, 0.114));
  float tmpvar_7;
  tmpvar_7 = dot (texture2DLod (_MainTex, tmpvar_3.xy, 0.0).xyz, vec3(0.299, 0.587, 0.114));
  float tmpvar_8;
  tmpvar_8 = dot (texture2DLod (_MainTex, tmpvar_4.xy, 0.0).xyz, vec3(0.299, 0.587, 0.114));
  float tmpvar_9;
  tmpvar_9 = dot (texture2DLod (_MainTex, tmpvar_5.xy, 0.0).xyz, vec3(0.299, 0.587, 0.114));
  float tmpvar_10;
  tmpvar_10 = dot (texture2DLod (_MainTex, xlv_TEXCOORD0.xy, 0.0).xyz, vec3(0.299, 0.587, 0.114));
  float tmpvar_11;
  tmpvar_11 = min (tmpvar_10, min (min (tmpvar_6, tmpvar_7), min (tmpvar_8, tmpvar_9)));
  float tmpvar_12;
  tmpvar_12 = max (tmpvar_10, max (max (tmpvar_6, tmpvar_7), max (tmpvar_8, tmpvar_9)));
  dir_2.x = ((tmpvar_8 + tmpvar_9) - (tmpvar_6 + tmpvar_7));
  dir_2.y = ((tmpvar_6 + tmpvar_8) - (tmpvar_7 + tmpvar_9));
  vec2 tmpvar_13;
  tmpvar_13 = (min (vec2(8.0, 8.0), max (vec2(-8.0, -8.0), 
    (dir_2 * (1.0/((min (
      abs(dir_2.x)
    , 
      abs(dir_2.y)
    ) + max (
      ((((tmpvar_6 + tmpvar_7) + tmpvar_8) + tmpvar_9) * 0.03125)
    , 0.0078125)))))
  )) * _MainTex_TexelSize.xy);
  dir_2 = tmpvar_13;
  vec4 tmpvar_14;
  tmpvar_14.zw = vec2(0.0, 0.0);
  tmpvar_14.xy = (xlv_TEXCOORD0.xy + (tmpvar_13 * -0.1666667));
  vec4 tmpvar_15;
  tmpvar_15.zw = vec2(0.0, 0.0);
  tmpvar_15.xy = (xlv_TEXCOORD0.xy + (tmpvar_13 * 0.1666667));
  vec3 tmpvar_16;
  tmpvar_16 = (0.5 * (texture2DLod (_MainTex, tmpvar_14.xy, 0.0).xyz + texture2DLod (_MainTex, tmpvar_15.xy, 0.0).xyz));
  vec4 tmpvar_17;
  tmpvar_17.zw = vec2(0.0, 0.0);
  tmpvar_17.xy = (xlv_TEXCOORD0.xy + (tmpvar_13 * -0.5));
  vec4 tmpvar_18;
  tmpvar_18.zw = vec2(0.0, 0.0);
  tmpvar_18.xy = (xlv_TEXCOORD0.xy + (tmpvar_13 * 0.5));
  vec3 tmpvar_19;
  tmpvar_19 = ((tmpvar_16 * 0.5) + (0.25 * (texture2DLod (_MainTex, tmpvar_17.xy, 0.0).xyz + texture2DLod (_MainTex, tmpvar_18.xy, 0.0).xyz)));
  float tmpvar_20;
  tmpvar_20 = dot (tmpvar_19, vec3(0.299, 0.587, 0.114));
  if (((tmpvar_20 < tmpvar_11) || (tmpvar_20 > tmpvar_12))) {
    tmpvar_1 = tmpvar_16;
  } else {
    tmpvar_1 = tmpvar_19;
  };
  vec4 tmpvar_21;
  tmpvar_21.w = 0.0;
  tmpvar_21.xyz = tmpvar_1;
  gl_FragData[0] = tmpvar_21;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 9 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 2, -1, 0.5, 0.75
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mad r0.xy, v1, c5.x, c5.y
mad r0.xy, r0, c5.z, c5.z
mov r0.w, c5.w
mad o1.zw, c4.xyxy, -r0.w, r0.xyxy
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 7 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 112
Vector 96 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefieceddeehmahafidiplchmhaccbnbejjmjjhbabaaaaaakeacaaaaadaaaaaa
cmaaaaaaiaaaaaaaniaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefcmeabaaaa
eaaaabaahbaaaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafjaaaaaeegiocaaa
abaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaa
ghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaadpccabaaaabaaaaaagiaaaaac
abaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaa
abaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaa
acaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaa
egiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaap
dcaabaaaaaaaaaaaegbabaaaabaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaaa
aaaaaaaaaceaaaaaaaaaialpaaaaialpaaaaaaaaaaaaaaaadcaaaaapdcaabaaa
aaaaaaaaegaabaaaaaaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaa
aceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaadcaaaaaomccabaaaabaaaaaa
agiecaiaebaaaaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaeadp
aaaaeadpagaebaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaa
doaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 60 math, 18 textures
Vector 0 [_MainTex_TexelSize]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c1, 0.298999995, 0.587000012, 0.114, -8
def c2, 1, 0, 0.03125, 0.0078125
def c3, -0.166666672, 0.166666672, 0.5, -0.5
def c4, 0.25, 0, 0, 0
dcl_texcoord v0
dcl_2d s0
mul r0, c2.xxyy, v0.zwzz
texldl r0, r0, s0
dp3 r0.x, r0, c1
mov r1.xy, c2
mad r1, c0.xyxy, r1.xyyx, v0.zwzw
mul r2, r1.xyxx, c2.xxyy
mul r1, r1.zwxx, c2.xxyy
texldl r1, r1, s0
dp3 r0.y, r1, c1
texldl r1, r2, s0
dp3 r0.z, r1, c1
add r0.w, r0.z, r0.x
add r1.xy, c0, v0.zwzw
mov r1.zw, c2.y
texldl r1, r1, s0
dp3 r1.x, r1, c1
add r1.y, r0.y, r1.x
add r1.y, r0.w, -r1.y
add r0.w, r0.y, r0.w
add r0.w, r1.x, r0.w
mul r0.w, r0.w, c2.z
max r1.z, r0.w, c2.w
mov r2.xz, -r1.y
cmp r0.w, r2.z, r2.z, r1.y
add r1.y, r0.z, r1.x
add r1.w, r0.y, r0.x
add r2.yw, -r1.y, r1.w
min r1.y, r2_abs.w, r0.w
add r0.w, r1.z, r1.y
rcp r0.w, r0.w
mul r3, r0.w, r2
mad r2, r2.zwzw, -r0.w, c1.w
cmp r2, r2, c1.w, r3
add r3, -r2.zwzw, -c1.w
cmp r2, r3, r2, -c1.w
mul r2, r2, c0.xyxy
mad r3, r2, c3.wwzz, v0.xyxy
mad r2, r2.zwzw, c3.xxyy, v0.xyxy
mul r4, r3.xyxx, c2.xxyy
mul r3, r3.zwxx, c2.xxyy
texldl r3, r3, s0
texldl r4, r4, s0
add r1.yzw, r3.xxyz, r4.xxyz
mul r1.yzw, r1, c4.x
mul r3, r2.xyxx, c2.xxyy
mul r2, r2.zwxx, c2.xxyy
texldl r2, r2, s0
texldl r3, r3, s0
add r2.xyz, r2, r3
mad r1.yzw, r2.xxyz, c4.x, r1
mul r2.xyz, r2, c3.z
dp3 r0.w, r1.yzww, c1
min r2.w, r1.x, r0.y
max r3.x, r0.y, r1.x
min r1.x, r0.z, r0.x
max r3.y, r0.x, r0.z
max r0.x, r3.y, r3.x
min r0.y, r2.w, r1.x
mul r3, c2.xxyy, v0.xyzz
texldl r3, r3, s0
dp3 r0.z, r3, c1
min r1.x, r0.y, r0.z
max r2.w, r0.z, r0.x
add r0.x, -r0.w, r2.w
add r0.y, r0.w, -r1.x
cmp r0.xy, r0, c2.y, c2.x
add r0.x, r0.x, r0.y
cmp oC0.xyz, -r0.x, r1.yzww, r2
mov oC0.w, c2.y

""
}
SubProgram ""d3d11 "" {
// Stats: 43 math
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 112
Vector 96 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedoomnldopbelmlocmcdppgidipbjccgnnabaaaaaahmaiaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefclmahaaaa
eaaaaaaaopabaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaadpcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacafaaaaaadcaaaaanpcaabaaaaaaaaaaa
egiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaiadpaaaaaaaaaaaaaaaaaaaaiadp
ogbobaaaabaaaaaaeiaaaaalpcaabaaaabaaaaaaegaabaaaaaaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaaaaaaaaaa
ogakbaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaa
baaaaaakbcaabaaaaaaaaaaaegacbaaaaaaaaaaaaceaaaaaihbgjjdokcefbgdp
nfhiojdnaaaaaaaabaaaaaakccaabaaaaaaaaaaaegacbaaaabaaaaaaaceaaaaa
ihbgjjdokcefbgdpnfhiojdnaaaaaaaaaaaaaaaimcaabaaaaaaaaaaakgbobaaa
abaaaaaaagiecaaaaaaaaaaaagaaaaaaeiaaaaalpcaabaaaabaaaaaaogakbaaa
aaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaabaaaaaak
ecaabaaaaaaaaaaaegacbaaaabaaaaaaaceaaaaaihbgjjdokcefbgdpnfhiojdn
aaaaaaaaaaaaaaahicaabaaaaaaaaaaackaabaaaaaaaaaaabkaabaaaaaaaaaaa
eiaaaaalpcaabaaaabaaaaaaogbkbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaabaaaaaakbcaabaaaabaaaaaaegacbaaaabaaaaaa
aceaaaaaihbgjjdokcefbgdpnfhiojdnaaaaaaaaaaaaaaahccaabaaaabaaaaaa
akaabaaaaaaaaaaaakaabaaaabaaaaaaaaaaaaaikcaabaaaacaaaaaapgapbaia
ebaaaaaaaaaaaaaafgafbaaaabaaaaaaaaaaaaahicaabaaaaaaaaaaabkaabaaa
aaaaaaaaakaabaaaabaaaaaaaaaaaaahccaabaaaabaaaaaackaabaaaaaaaaaaa
akaabaaaaaaaaaaaaaaaaaaiccaabaaaabaaaaaadkaabaaaaaaaaaaabkaabaia
ebaaaaaaabaaaaaaaaaaaaahicaabaaaaaaaaaaaakaabaaaaaaaaaaadkaabaaa
aaaaaaaaaaaaaaahicaabaaaaaaaaaaackaabaaaaaaaaaaadkaabaaaaaaaaaaa
diaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaadndeaaaaah
icaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaadmddaaaaajecaabaaa
abaaaaaadkaabaiaibaaaaaaacaaaaaabkaabaiaibaaaaaaabaaaaaadgaaaaag
fcaabaaaacaaaaaafgafbaiaebaaaaaaabaaaaaaaaaaaaahicaabaaaaaaaaaaa
dkaabaaaaaaaaaaackaabaaaabaaaaaaaoaaaaakicaabaaaaaaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaiadpaaaaiadpdkaabaaaaaaaaaaadiaaaaahpcaabaaa
acaaaaaapgapbaaaaaaaaaaaegaobaaaacaaaaaadeaaaaakpcaabaaaacaaaaaa
egaobaaaacaaaaaaaceaaaaaaaaaaambaaaaaambaaaaaambaaaaaambddaaaaak
pcaabaaaacaaaaaaegaobaaaacaaaaaaaceaaaaaaaaaaaebaaaaaaebaaaaaaeb
aaaaaaebdiaaaaaipcaabaaaacaaaaaaegaobaaaacaaaaaaegiecaaaaaaaaaaa
agaaaaaadcaaaaampcaabaaaadaaaaaaegaobaaaacaaaaaaaceaaaaaaaaaaalp
aaaaaalpaaaaaadpaaaaaadpegbebaaaabaaaaaadcaaaaampcaabaaaacaaaaaa
ogaobaaaacaaaaaaaceaaaaaklkkckloklkkckloklkkckdoklkkckdoegbebaaa
abaaaaaaeiaaaaalpcaabaaaaeaaaaaaegaabaaaadaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaaadaaaaaaogakbaaa
adaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaaaaaaaaah
ocaabaaaabaaaaaaagajbaaaadaaaaaaagajbaaaaeaaaaaadiaaaaakocaabaaa
abaaaaaafgaobaaaabaaaaaaaceaaaaaaaaaaaaaaaaaiadoaaaaiadoaaaaiado
eiaaaaalpcaabaaaadaaaaaaegaabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaaacaaaaaaogakbaaaacaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaaaaaaaaahhcaabaaa
acaaaaaaegacbaaaacaaaaaaegacbaaaadaaaaaadcaaaaamocaabaaaabaaaaaa
agajbaaaacaaaaaaaceaaaaaaaaaaaaaaaaaiadoaaaaiadoaaaaiadofgaobaaa
abaaaaaadiaaaaakhcaabaaaacaaaaaaegacbaaaacaaaaaaaceaaaaaaaaaaadp
aaaaaadpaaaaaadpaaaaaaaabaaaaaakicaabaaaaaaaaaaajgahbaaaabaaaaaa
aceaaaaaihbgjjdokcefbgdpnfhiojdnaaaaaaaaddaaaaahicaabaaaacaaaaaa
ckaabaaaaaaaaaaaakaabaaaaaaaaaaadeaaaaahbcaabaaaaaaaaaaackaabaaa
aaaaaaaaakaabaaaaaaaaaaaddaaaaahecaabaaaaaaaaaaabkaabaaaaaaaaaaa
akaabaaaabaaaaaadeaaaaahccaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaa
abaaaaaadeaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaa
ddaaaaahccaabaaaaaaaaaaadkaabaaaacaaaaaackaabaaaaaaaaaaaeiaaaaal
pcaabaaaadaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
abeaaaaaaaaaaaaabaaaaaakecaabaaaaaaaaaaaegacbaaaadaaaaaaaceaaaaa
ihbgjjdokcefbgdpnfhiojdnaaaaaaaaddaaaaahccaabaaaaaaaaaaabkaabaaa
aaaaaaaackaabaaaaaaaaaaadeaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaa
ckaabaaaaaaaaaaadbaaaaahdcaabaaaaaaaaaaamgaabaaaaaaaaaaahgapbaaa
aaaaaaaadmaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaa
dhaaaaajhccabaaaaaaaaaaaagaabaaaaaaaaaaaegacbaaaacaaaaaajgahbaaa
abaaaaaadgaaaaaficcabaaaaaaaaaaaabeaaaaaaaaaaaaadoaaaaab""
}
}
 }
}
Fallback Off
}";

		private const String nfaaShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 28.8KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/NFAA"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = ""white"" { }
 _BlurTex (""Base (RGB)"", 2D) = ""white"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 13 math
 //        d3d9 : 17 math
 //      opengl : 93 math, 13 texture
 // Stats for Fragment shader:
 //       d3d11 : 76 math, 13 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 47188
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 93 math, 13 textures
""!!GLSL
#ifdef VERTEX

uniform vec4 _MainTex_TexelSize;
uniform float _OffsetScale;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
varying vec2 xlv_TEXCOORD0_5;
varying vec2 xlv_TEXCOORD0_6;
varying vec2 xlv_TEXCOORD0_7;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1.x = 0.0;
  tmpvar_1.y = _MainTex_TexelSize.y;
  vec2 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * _OffsetScale);
  vec2 tmpvar_3;
  tmpvar_3.y = 0.0;
  tmpvar_3.x = _MainTex_TexelSize.x;
  vec2 tmpvar_4;
  tmpvar_4 = (tmpvar_3 * _OffsetScale);
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = (gl_MultiTexCoord0.xy + tmpvar_2);
  xlv_TEXCOORD0_1 = (gl_MultiTexCoord0.xy - tmpvar_2);
  xlv_TEXCOORD0_2 = (gl_MultiTexCoord0.xy + tmpvar_4);
  xlv_TEXCOORD0_3 = (gl_MultiTexCoord0.xy - tmpvar_4);
  xlv_TEXCOORD0_4 = ((gl_MultiTexCoord0.xy - tmpvar_4) + tmpvar_2);
  xlv_TEXCOORD0_5 = ((gl_MultiTexCoord0.xy - tmpvar_4) - tmpvar_2);
  xlv_TEXCOORD0_6 = ((gl_MultiTexCoord0.xy + tmpvar_4) + tmpvar_2);
  xlv_TEXCOORD0_7 = ((gl_MultiTexCoord0.xy + tmpvar_4) - tmpvar_2);
}


#endif
#ifdef FRAGMENT
uniform vec4 unity_ColorSpaceLuminance;
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
uniform float _BlurRadius;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
varying vec2 xlv_TEXCOORD0_5;
varying vec2 xlv_TEXCOORD0_6;
varying vec2 xlv_TEXCOORD0_7;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0).xyz * unity_ColorSpaceLuminance.xyz);
  vec3 tmpvar_2;
  tmpvar_2 = (texture2D (_MainTex, xlv_TEXCOORD0_1).xyz * unity_ColorSpaceLuminance.xyz);
  vec3 tmpvar_3;
  tmpvar_3 = (texture2D (_MainTex, xlv_TEXCOORD0_2).xyz * unity_ColorSpaceLuminance.xyz);
  vec3 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex, xlv_TEXCOORD0_3).xyz * unity_ColorSpaceLuminance.xyz);
  float tmpvar_5;
  vec3 tmpvar_6;
  tmpvar_6 = (texture2D (_MainTex, xlv_TEXCOORD0_4).xyz * unity_ColorSpaceLuminance.xyz);
  tmpvar_5 = (((tmpvar_6.x + tmpvar_6.y) + tmpvar_6.z) + ((2.0 * 
    sqrt((tmpvar_6.y * (tmpvar_6.x + tmpvar_6.z)))
  ) * unity_ColorSpaceLuminance.w));
  float tmpvar_7;
  vec3 tmpvar_8;
  tmpvar_8 = (texture2D (_MainTex, xlv_TEXCOORD0_5).xyz * unity_ColorSpaceLuminance.xyz);
  tmpvar_7 = (((tmpvar_8.x + tmpvar_8.y) + tmpvar_8.z) + ((2.0 * 
    sqrt((tmpvar_8.y * (tmpvar_8.x + tmpvar_8.z)))
  ) * unity_ColorSpaceLuminance.w));
  float tmpvar_9;
  vec3 tmpvar_10;
  tmpvar_10 = (texture2D (_MainTex, xlv_TEXCOORD0_6).xyz * unity_ColorSpaceLuminance.xyz);
  tmpvar_9 = (((tmpvar_10.x + tmpvar_10.y) + tmpvar_10.z) + ((2.0 * 
    sqrt((tmpvar_10.y * (tmpvar_10.x + tmpvar_10.z)))
  ) * unity_ColorSpaceLuminance.w));
  float tmpvar_11;
  vec3 tmpvar_12;
  tmpvar_12 = (texture2D (_MainTex, xlv_TEXCOORD0_7).xyz * unity_ColorSpaceLuminance.xyz);
  tmpvar_11 = (((tmpvar_12.x + tmpvar_12.y) + tmpvar_12.z) + ((2.0 * 
    sqrt((tmpvar_12.y * (tmpvar_12.x + tmpvar_12.z)))
  ) * unity_ColorSpaceLuminance.w));
  vec3 tmpvar_13;
  tmpvar_13.x = tmpvar_11;
  tmpvar_13.y = (((tmpvar_2.x + tmpvar_2.y) + tmpvar_2.z) + ((2.0 * 
    sqrt((tmpvar_2.y * (tmpvar_2.x + tmpvar_2.z)))
  ) * unity_ColorSpaceLuminance.w));
  tmpvar_13.z = tmpvar_5;
  vec3 tmpvar_14;
  tmpvar_14.x = tmpvar_7;
  tmpvar_14.y = (((tmpvar_1.x + tmpvar_1.y) + tmpvar_1.z) + ((2.0 * 
    sqrt((tmpvar_1.y * (tmpvar_1.x + tmpvar_1.z)))
  ) * unity_ColorSpaceLuminance.w));
  tmpvar_14.z = tmpvar_9;
  vec3 tmpvar_15;
  tmpvar_15.x = tmpvar_5;
  tmpvar_15.y = (((tmpvar_3.x + tmpvar_3.y) + tmpvar_3.z) + ((2.0 * 
    sqrt((tmpvar_3.y * (tmpvar_3.x + tmpvar_3.z)))
  ) * unity_ColorSpaceLuminance.w));
  tmpvar_15.z = tmpvar_7;
  vec3 tmpvar_16;
  tmpvar_16.x = tmpvar_9;
  tmpvar_16.y = (((tmpvar_4.x + tmpvar_4.y) + tmpvar_4.z) + ((2.0 * 
    sqrt((tmpvar_4.y * (tmpvar_4.x + tmpvar_4.z)))
  ) * unity_ColorSpaceLuminance.w));
  tmpvar_16.z = tmpvar_11;
  vec2 tmpvar_17;
  tmpvar_17.x = (dot (vec3(1.0, 1.0, 1.0), tmpvar_13) - dot (vec3(1.0, 1.0, 1.0), tmpvar_14));
  tmpvar_17.y = (dot (vec3(1.0, 1.0, 1.0), tmpvar_16) - dot (vec3(1.0, 1.0, 1.0), tmpvar_15));
  vec2 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * (_MainTex_TexelSize.xy * _BlurRadius));
  vec2 tmpvar_19;
  tmpvar_19 = ((xlv_TEXCOORD0 + xlv_TEXCOORD0_1) * 0.5);
  vec2 tmpvar_20;
  tmpvar_20.x = tmpvar_18.x;
  tmpvar_20.y = -(tmpvar_18.y);
  vec2 tmpvar_21;
  tmpvar_21.x = tmpvar_18.x;
  tmpvar_21.y = -(tmpvar_18.y);
  gl_FragData[0] = (((
    ((texture2D (_MainTex, tmpvar_19) + texture2D (_MainTex, (tmpvar_19 + tmpvar_18))) + texture2D (_MainTex, (tmpvar_19 - tmpvar_18)))
   + texture2D (_MainTex, 
    (tmpvar_19 + tmpvar_20)
  )) + texture2D (_MainTex, (tmpvar_19 - tmpvar_21))) * 0.2);
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 17 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
Float 5 [_OffsetScale]
""vs_2_0
def c6, 0, 0, 0, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.xy, c4
mul r0.yz, r0.xyxw, c5.x
mov r0.xw, c6.x
add oT0.xy, r0, v1
add oT1.xy, -r0, v1
add r1.xy, -r0.zwzw, v1
add oT4.xy, r0, r1
add oT5.xy, -r0, r1
mov oT3.xy, r1
add r0.zw, r0, v1.xyxy
add oT6.xy, r0, r0.zwzw
add oT7.xy, -r0, r0.zwzw
mov oT2.xy, r0.zwzw

""
}
SubProgram ""d3d11 "" {
// Stats: 13 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 128
Vector 96 [_MainTex_TexelSize]
Float 112 [_OffsetScale]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedaklfghanoeplfcpjpkenfgmnpfgdbhahabaaaaaadiaeaaaaadaaaaaa
cmaaaaaaiaaaaaaaiaabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheopiaaaaaaajaaaaaaaiaaaaaaoaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaomaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
omaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaaomaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaadamaaaaomaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaadamaaaaomaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaaadamaaaa
omaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaaadamaaaaomaaaaaaagaaaaaa
aaaaaaaaadaaaaaaahaaaaaaadamaaaaomaaaaaaahaaaaaaaaaaaaaaadaaaaaa
aiaaaaaaadamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl
fdeieefclaacaaaaeaaaabaakmaaaaaafjaaaaaeegiocaaaaaaaaaaaaiaaaaaa
fjaaaaaeegiocaaaabaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaad
dcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaa
abaaaaaagfaaaaaddccabaaaacaaaaaagfaaaaaddccabaaaadaaaaaagfaaaaad
dccabaaaaeaaaaaagfaaaaaddccabaaaafaaaaaagfaaaaaddccabaaaagaaaaaa
gfaaaaaddccabaaaahaaaaaagfaaaaaddccabaaaaiaaaaaagiaaaaacacaaaaaa
diaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaa
kgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaa
abaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadiaaaaajgcaabaaa
aaaaaaaafgiecaaaaaaaaaaaagaaaaaaagiacaaaaaaaaaaaahaaaaaadgaaaaai
jcaabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaah
dccabaaaabaaaaaaegaabaaaaaaaaaaaegbabaaaabaaaaaaaaaaaaaidccabaaa
acaaaaaaegaabaiaebaaaaaaaaaaaaaaegbabaaaabaaaaaaaaaaaaahdcaabaaa
abaaaaaaogakbaaaaaaaaaaaegbabaaaabaaaaaadgaaaaafdccabaaaadaaaaaa
egaabaaaabaaaaaaaaaaaaaimcaabaaaaaaaaaaakgaobaiaebaaaaaaaaaaaaaa
agbebaaaabaaaaaadgaaaaafdccabaaaaeaaaaaaogakbaaaaaaaaaaaaaaaaaah
dccabaaaafaaaaaaegaabaaaaaaaaaaaogakbaaaaaaaaaaaaaaaaaaidccabaaa
agaaaaaaegaabaiaebaaaaaaaaaaaaaaogakbaaaaaaaaaaaaaaaaaahdccabaaa
ahaaaaaaegaabaaaaaaaaaaaegaabaaaabaaaaaaaaaaaaaidccabaaaaiaaaaaa
egaabaiaebaaaaaaaaaaaaaaegaabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
// Platform d3d9 had shader errors
//   <no keywords>
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d11 "" {
// Stats: 76 math, 13 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 128
Vector 48 [unity_ColorSpaceLuminance]
Vector 96 [_MainTex_TexelSize]
Float 116 [_BlurRadius]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedmmccfhphnflgkdilgmglamjfkmfoangbabaaaaaaiaanaaaaadaaaaaa
cmaaaaaacmabaaaagaabaaaaejfdeheopiaaaaaaajaaaaaaaiaaaaaaoaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaomaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaomaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaaomaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaadadaaaaomaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaadadaaaaomaaaaaaaeaaaaaaaaaaaaaa
adaaaaaaafaaaaaaadadaaaaomaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaa
adadaaaaomaaaaaaagaaaaaaaaaaaaaaadaaaaaaahaaaaaaadadaaaaomaaaaaa
ahaaaaaaaaaaaaaaadaaaaaaaiaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaa
feeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl
fdeieefcbiamaaaaeaaaaaaaagadaaaafjaaaaaeegiocaaaaaaaaaaaaiaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaaddcbabaaaadaaaaaa
gcbaaaaddcbabaaaaeaaaaaagcbaaaaddcbabaaaafaaaaaagcbaaaaddcbabaaa
agaaaaaagcbaaaaddcbabaaaahaaaaaagcbaaaaddcbabaaaaiaaaaaagfaaaaad
pccabaaaaaaaaaaagiaaaaacaeaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaa
adaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaailcaabaaaaaaaaaaa
egaibaaaaaaaaaaaegiicaaaaaaaaaaaadaaaaaaaaaaaaahjcaabaaaaaaaaaaa
fganbaaaaaaaaaaaagaabaaaaaaaaaaadiaaaaahccaabaaaaaaaaaaadkaabaaa
aaaaaaaabkaabaaaaaaaaaaadcaaaaakbcaabaaaaaaaaaaackaabaaaaaaaaaaa
ckiacaaaaaaaaaaaadaaaaaaakaabaaaaaaaaaaaelaaaaafccaabaaaaaaaaaaa
bkaabaaaaaaaaaaaapaaaaaiccaabaaaaaaaaaaapgipcaaaaaaaaaaaadaaaaaa
fgafbaaaaaaaaaaaaaaaaaahccaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaa
aaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaafaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadiaaaaailcaabaaaabaaaaaaegaibaaaabaaaaaaegiicaaa
aaaaaaaaadaaaaaaaaaaaaahjcaabaaaabaaaaaafganbaaaabaaaaaaagaabaaa
abaaaaaadiaaaaahicaabaaaaaaaaaaadkaabaaaabaaaaaabkaabaaaabaaaaaa
dcaaaaakbcaabaaaabaaaaaackaabaaaabaaaaaackiacaaaaaaaaaaaadaaaaaa
akaabaaaabaaaaaaelaaaaaficaabaaaaaaaaaaadkaabaaaaaaaaaaaapaaaaai
icaabaaaaaaaaaaapgipcaaaaaaaaaaaadaaaaaapgapbaaaaaaaaaaaaaaaaaah
ecaabaaaabaaaaaadkaabaaaaaaaaaaaakaabaaaabaaaaaadgaaaaafbcaabaaa
aaaaaaaackaabaaaabaaaaaaefaaaaajpcaabaaaacaaaaaaegbabaaaagaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaailcaabaaaacaaaaaaegaibaaa
acaaaaaaegiicaaaaaaaaaaaadaaaaaaaaaaaaahjcaabaaaacaaaaaafganbaaa
acaaaaaaagaabaaaacaaaaaadiaaaaahicaabaaaaaaaaaaadkaabaaaacaaaaaa
bkaabaaaacaaaaaadcaaaaakicaabaaaabaaaaaackaabaaaacaaaaaackiacaaa
aaaaaaaaadaaaaaaakaabaaaacaaaaaaelaaaaaficaabaaaaaaaaaaadkaabaaa
aaaaaaaaapaaaaaiicaabaaaaaaaaaaapgipcaaaaaaaaaaaadaaaaaapgapbaaa
aaaaaaaaaaaaaaahecaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaaabaaaaaa
baaaaaakicaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaaaaa
egacbaaaaaaaaaaaefaaaaajpcaabaaaacaaaaaaegbabaaaaeaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaadiaaaaailcaabaaaacaaaaaaegaibaaaacaaaaaa
egiicaaaaaaaaaaaadaaaaaaaaaaaaahjcaabaaaacaaaaaafganbaaaacaaaaaa
agaabaaaacaaaaaadiaaaaahicaabaaaabaaaaaadkaabaaaacaaaaaabkaabaaa
acaaaaaadcaaaaakbcaabaaaacaaaaaackaabaaaacaaaaaackiacaaaaaaaaaaa
adaaaaaaakaabaaaacaaaaaaelaaaaaficaabaaaabaaaaaadkaabaaaabaaaaaa
apaaaaaiicaabaaaabaaaaaapgipcaaaaaaaaaaaadaaaaaapgapbaaaabaaaaaa
aaaaaaahccaabaaaacaaaaaadkaabaaaabaaaaaaakaabaaaacaaaaaaefaaaaaj
pcaabaaaadaaaaaaegbabaaaahaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
diaaaaailcaabaaaadaaaaaaegaibaaaadaaaaaaegiicaaaaaaaaaaaadaaaaaa
aaaaaaahjcaabaaaadaaaaaafganbaaaadaaaaaaagaabaaaadaaaaaadiaaaaah
icaabaaaabaaaaaadkaabaaaadaaaaaabkaabaaaadaaaaaadcaaaaakicaabaaa
acaaaaaackaabaaaadaaaaaackiacaaaaaaaaaaaadaaaaaaakaabaaaadaaaaaa
elaaaaaficaabaaaabaaaaaadkaabaaaabaaaaaaapaaaaaiicaabaaaabaaaaaa
pgipcaaaaaaaaaaaadaaaaaapgapbaaaabaaaaaaaaaaaaahccaabaaaaaaaaaaa
dkaabaaaabaaaaaadkaabaaaacaaaaaadgaaaaafbcaabaaaacaaaaaabkaabaaa
aaaaaaaaefaaaaajpcaabaaaadaaaaaaegbabaaaaiaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadiaaaaailcaabaaaadaaaaaaegaibaaaadaaaaaaegiicaaa
aaaaaaaaadaaaaaaaaaaaaahjcaabaaaadaaaaaafganbaaaadaaaaaaagaabaaa
adaaaaaadiaaaaahicaabaaaabaaaaaadkaabaaaadaaaaaabkaabaaaadaaaaaa
dcaaaaakicaabaaaacaaaaaackaabaaaadaaaaaackiacaaaaaaaaaaaadaaaaaa
akaabaaaadaaaaaaelaaaaaficaabaaaabaaaaaadkaabaaaabaaaaaaapaaaaai
icaabaaaabaaaaaapgipcaaaaaaaaaaaadaaaaaapgapbaaaabaaaaaaaaaaaaah
bcaabaaaabaaaaaadkaabaaaabaaaaaadkaabaaaacaaaaaadgaaaaafecaabaaa
acaaaaaaakaabaaaabaaaaaabaaaaaakicaabaaaabaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaiadpaaaaaaaaegacbaaaacaaaaaaaaaaaaaiccaabaaaacaaaaaa
dkaabaiaebaaaaaaaaaaaaaadkaabaaaabaaaaaaefaaaaajpcaabaaaadaaaaaa
egbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaailcaabaaa
adaaaaaaegaibaaaadaaaaaaegiicaaaaaaaaaaaadaaaaaaaaaaaaahmcaabaaa
acaaaaaafganbaaaadaaaaaaagaabaaaadaaaaaadiaaaaahicaabaaaaaaaaaaa
dkaabaaaacaaaaaabkaabaaaadaaaaaadcaaaaakicaabaaaabaaaaaackaabaaa
adaaaaaackiacaaaaaaaaaaaadaaaaaackaabaaaacaaaaaaelaaaaaficaabaaa
aaaaaaaadkaabaaaaaaaaaaaapaaaaaiicaabaaaaaaaaaaapgipcaaaaaaaaaaa
adaaaaaapgapbaaaaaaaaaaaaaaaaaahccaabaaaabaaaaaadkaabaaaaaaaaaaa
dkaabaaaabaaaaaabaaaaaakicaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaiadpaaaaaaaaegacbaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaailcaabaaaabaaaaaa
egaibaaaabaaaaaaegiicaaaaaaaaaaaadaaaaaaaaaaaaahjcaabaaaabaaaaaa
fganbaaaabaaaaaaagaabaaaabaaaaaadiaaaaahccaabaaaabaaaaaadkaabaaa
abaaaaaabkaabaaaabaaaaaadcaaaaakbcaabaaaabaaaaaackaabaaaabaaaaaa
ckiacaaaaaaaaaaaadaaaaaaakaabaaaabaaaaaaelaaaaafccaabaaaabaaaaaa
bkaabaaaabaaaaaaapaaaaaiccaabaaaabaaaaaapgipcaaaaaaaaaaaadaaaaaa
fgafbaaaabaaaaaaaaaaaaahbcaabaaaaaaaaaaabkaabaaaabaaaaaaakaabaaa
abaaaaaabaaaaaakbcaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaaaaaegacbaaaaaaaaaaaaaaaaaaibcaabaaaacaaaaaaakaabaiaebaaaaaa
aaaaaaaadkaabaaaaaaaaaaadiaaaaajdcaabaaaaaaaaaaaegiacaaaaaaaaaaa
agaaaaaafgifcaaaaaaaaaaaahaaaaaadiaaaaahdcaabaaaaaaaaaaaegaabaaa
aaaaaaaaegaabaaaacaaaaaaaaaaaaahdcaabaaaabaaaaaaegbabaaaabaaaaaa
egbabaaaacaaaaaadcaaaaammcaabaaaabaaaaaaagaebaaaabaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaadpaaaaaadpagaebaaaaaaaaaaaefaaaaajpcaabaaa
acaaaaaaogakbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaak
mcaabaaaabaaaaaaagaebaaaabaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaadp
aaaaaadpefaaaaajpcaabaaaadaaaaaaogakbaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaaaaaaaahpcaabaaaacaaaaaaegaobaaaacaaaaaaegaobaaa
adaaaaaadcaaaaanmcaabaaaabaaaaaaagaebaaaabaaaaaaaceaaaaaaaaaaaaa
aaaaaaaaaaaaaadpaaaaaadpagaebaiaebaaaaaaaaaaaaaaefaaaaajpcaabaaa
adaaaaaaogakbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaah
pcaabaaaacaaaaaaegaobaaaacaaaaaaegaobaaaadaaaaaadgaaaaagecaabaaa
aaaaaaaabkaabaiaebaaaaaaaaaaaaaadcaaaaamkcaabaaaaaaaaaaaagaebaaa
abaaaaaaaceaaaaaaaaaaaaaaaaaaadpaaaaaaaaaaaaaadpagaibaaaaaaaaaaa
dcaaaaanfcaabaaaaaaaaaaaagabbaaaabaaaaaaaceaaaaaaaaaaadpaaaaaaaa
aaaaaadpaaaaaaaaagacbaiaebaaaaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaa
igaabaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
aaaaaaaangafbaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaah
pcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaacaaaaaaaaaaaaahpcaabaaa
aaaaaaaaegaobaaaabaaaaaaegaobaaaaaaaaaaadiaaaaakpccabaaaaaaaaaaa
egaobaaaaaaaaaaaaceaaaaamnmmemdomnmmemdomnmmemdomnmmemdodoaaaaab
""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 13 math
 //      opengl : 84 math, 8 texture
 // Stats for Fragment shader:
 //       d3d11 : 68 math, 8 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 70803
Program ""vp"" {
// Platform d3d9 skipped due to earlier errors
// Platform d3d9 skipped due to earlier errors
SubProgram ""opengl "" {
// Stats: 84 math, 8 textures
""!!GLSL
#ifdef VERTEX

uniform vec4 _MainTex_TexelSize;
uniform float _OffsetScale;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
varying vec2 xlv_TEXCOORD0_5;
varying vec2 xlv_TEXCOORD0_6;
varying vec2 xlv_TEXCOORD0_7;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1.x = 0.0;
  tmpvar_1.y = _MainTex_TexelSize.y;
  vec2 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * _OffsetScale);
  vec2 tmpvar_3;
  tmpvar_3.y = 0.0;
  tmpvar_3.x = _MainTex_TexelSize.x;
  vec2 tmpvar_4;
  tmpvar_4 = (tmpvar_3 * _OffsetScale);
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = (gl_MultiTexCoord0.xy + tmpvar_2);
  xlv_TEXCOORD0_1 = (gl_MultiTexCoord0.xy - tmpvar_2);
  xlv_TEXCOORD0_2 = (gl_MultiTexCoord0.xy + tmpvar_4);
  xlv_TEXCOORD0_3 = (gl_MultiTexCoord0.xy - tmpvar_4);
  xlv_TEXCOORD0_4 = ((gl_MultiTexCoord0.xy - tmpvar_4) + tmpvar_2);
  xlv_TEXCOORD0_5 = ((gl_MultiTexCoord0.xy - tmpvar_4) - tmpvar_2);
  xlv_TEXCOORD0_6 = ((gl_MultiTexCoord0.xy + tmpvar_4) + tmpvar_2);
  xlv_TEXCOORD0_7 = ((gl_MultiTexCoord0.xy + tmpvar_4) - tmpvar_2);
}


#endif
#ifdef FRAGMENT
uniform vec4 unity_ColorSpaceLuminance;
uniform sampler2D _MainTex;
uniform float _BlurRadius;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
varying vec2 xlv_TEXCOORD0_5;
varying vec2 xlv_TEXCOORD0_6;
varying vec2 xlv_TEXCOORD0_7;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0).xyz * unity_ColorSpaceLuminance.xyz);
  vec3 tmpvar_2;
  tmpvar_2 = (texture2D (_MainTex, xlv_TEXCOORD0_1).xyz * unity_ColorSpaceLuminance.xyz);
  vec3 tmpvar_3;
  tmpvar_3 = (texture2D (_MainTex, xlv_TEXCOORD0_2).xyz * unity_ColorSpaceLuminance.xyz);
  vec3 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex, xlv_TEXCOORD0_3).xyz * unity_ColorSpaceLuminance.xyz);
  float tmpvar_5;
  vec3 tmpvar_6;
  tmpvar_6 = (texture2D (_MainTex, xlv_TEXCOORD0_4).xyz * unity_ColorSpaceLuminance.xyz);
  tmpvar_5 = (((tmpvar_6.x + tmpvar_6.y) + tmpvar_6.z) + ((2.0 * 
    sqrt((tmpvar_6.y * (tmpvar_6.x + tmpvar_6.z)))
  ) * unity_ColorSpaceLuminance.w));
  float tmpvar_7;
  vec3 tmpvar_8;
  tmpvar_8 = (texture2D (_MainTex, xlv_TEXCOORD0_5).xyz * unity_ColorSpaceLuminance.xyz);
  tmpvar_7 = (((tmpvar_8.x + tmpvar_8.y) + tmpvar_8.z) + ((2.0 * 
    sqrt((tmpvar_8.y * (tmpvar_8.x + tmpvar_8.z)))
  ) * unity_ColorSpaceLuminance.w));
  float tmpvar_9;
  vec3 tmpvar_10;
  tmpvar_10 = (texture2D (_MainTex, xlv_TEXCOORD0_6).xyz * unity_ColorSpaceLuminance.xyz);
  tmpvar_9 = (((tmpvar_10.x + tmpvar_10.y) + tmpvar_10.z) + ((2.0 * 
    sqrt((tmpvar_10.y * (tmpvar_10.x + tmpvar_10.z)))
  ) * unity_ColorSpaceLuminance.w));
  float tmpvar_11;
  vec3 tmpvar_12;
  tmpvar_12 = (texture2D (_MainTex, xlv_TEXCOORD0_7).xyz * unity_ColorSpaceLuminance.xyz);
  tmpvar_11 = (((tmpvar_12.x + tmpvar_12.y) + tmpvar_12.z) + ((2.0 * 
    sqrt((tmpvar_12.y * (tmpvar_12.x + tmpvar_12.z)))
  ) * unity_ColorSpaceLuminance.w));
  vec3 tmpvar_13;
  tmpvar_13.x = tmpvar_11;
  tmpvar_13.y = (((tmpvar_2.x + tmpvar_2.y) + tmpvar_2.z) + ((2.0 * 
    sqrt((tmpvar_2.y * (tmpvar_2.x + tmpvar_2.z)))
  ) * unity_ColorSpaceLuminance.w));
  tmpvar_13.z = tmpvar_5;
  vec3 tmpvar_14;
  tmpvar_14.x = tmpvar_7;
  tmpvar_14.y = (((tmpvar_1.x + tmpvar_1.y) + tmpvar_1.z) + ((2.0 * 
    sqrt((tmpvar_1.y * (tmpvar_1.x + tmpvar_1.z)))
  ) * unity_ColorSpaceLuminance.w));
  tmpvar_14.z = tmpvar_9;
  vec3 tmpvar_15;
  tmpvar_15.x = tmpvar_5;
  tmpvar_15.y = (((tmpvar_3.x + tmpvar_3.y) + tmpvar_3.z) + ((2.0 * 
    sqrt((tmpvar_3.y * (tmpvar_3.x + tmpvar_3.z)))
  ) * unity_ColorSpaceLuminance.w));
  tmpvar_15.z = tmpvar_7;
  vec3 tmpvar_16;
  tmpvar_16.x = tmpvar_9;
  tmpvar_16.y = (((tmpvar_4.x + tmpvar_4.y) + tmpvar_4.z) + ((2.0 * 
    sqrt((tmpvar_4.y * (tmpvar_4.x + tmpvar_4.z)))
  ) * unity_ColorSpaceLuminance.w));
  tmpvar_16.z = tmpvar_11;
  vec2 tmpvar_17;
  tmpvar_17.x = (dot (vec3(1.0, 1.0, 1.0), tmpvar_13) - dot (vec3(1.0, 1.0, 1.0), tmpvar_14));
  tmpvar_17.y = (dot (vec3(1.0, 1.0, 1.0), tmpvar_16) - dot (vec3(1.0, 1.0, 1.0), tmpvar_15));
  vec3 tmpvar_18;
  tmpvar_18.z = 1.0;
  tmpvar_18.xy = (tmpvar_17 * _BlurRadius);
  vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = normalize(((tmpvar_18 * 0.5) + 0.5));
  gl_FragData[0] = tmpvar_19;
}


#endif
""
}
SubProgram ""d3d11 "" {
// Stats: 13 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 128
Vector 96 [_MainTex_TexelSize]
Float 112 [_OffsetScale]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedaklfghanoeplfcpjpkenfgmnpfgdbhahabaaaaaadiaeaaaaadaaaaaa
cmaaaaaaiaaaaaaaiaabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheopiaaaaaaajaaaaaaaiaaaaaaoaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaomaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
omaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaaomaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaadamaaaaomaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaadamaaaaomaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaaadamaaaa
omaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaaadamaaaaomaaaaaaagaaaaaa
aaaaaaaaadaaaaaaahaaaaaaadamaaaaomaaaaaaahaaaaaaaaaaaaaaadaaaaaa
aiaaaaaaadamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl
fdeieefclaacaaaaeaaaabaakmaaaaaafjaaaaaeegiocaaaaaaaaaaaaiaaaaaa
fjaaaaaeegiocaaaabaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaad
dcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaa
abaaaaaagfaaaaaddccabaaaacaaaaaagfaaaaaddccabaaaadaaaaaagfaaaaad
dccabaaaaeaaaaaagfaaaaaddccabaaaafaaaaaagfaaaaaddccabaaaagaaaaaa
gfaaaaaddccabaaaahaaaaaagfaaaaaddccabaaaaiaaaaaagiaaaaacacaaaaaa
diaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaa
kgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaa
abaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadiaaaaajgcaabaaa
aaaaaaaafgiecaaaaaaaaaaaagaaaaaaagiacaaaaaaaaaaaahaaaaaadgaaaaai
jcaabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaah
dccabaaaabaaaaaaegaabaaaaaaaaaaaegbabaaaabaaaaaaaaaaaaaidccabaaa
acaaaaaaegaabaiaebaaaaaaaaaaaaaaegbabaaaabaaaaaaaaaaaaahdcaabaaa
abaaaaaaogakbaaaaaaaaaaaegbabaaaabaaaaaadgaaaaafdccabaaaadaaaaaa
egaabaaaabaaaaaaaaaaaaaimcaabaaaaaaaaaaakgaobaiaebaaaaaaaaaaaaaa
agbebaaaabaaaaaadgaaaaafdccabaaaaeaaaaaaogakbaaaaaaaaaaaaaaaaaah
dccabaaaafaaaaaaegaabaaaaaaaaaaaogakbaaaaaaaaaaaaaaaaaaidccabaaa
agaaaaaaegaabaiaebaaaaaaaaaaaaaaogakbaaaaaaaaaaaaaaaaaahdccabaaa
ahaaaaaaegaabaaaaaaaaaaaegaabaaaabaaaaaaaaaaaaaidccabaaaaiaaaaaa
egaabaiaebaaaaaaaaaaaaaaegaabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
// Platform d3d9 skipped due to earlier errors
// Platform d3d9 had shader errors
//   <no keywords>
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d11 "" {
// Stats: 68 math, 8 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 128
Vector 48 [unity_ColorSpaceLuminance]
Float 116 [_BlurRadius]
BindCB  ""$Globals"" 0
""ps_4_0
eefieceddnalgnfflakooohlgleikpngkihhnhhgabaaaaaalealaaaaadaaaaaa
cmaaaaaacmabaaaagaabaaaaejfdeheopiaaaaaaajaaaaaaaiaaaaaaoaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaomaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaomaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaaomaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaadadaaaaomaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaadadaaaaomaaaaaaaeaaaaaaaaaaaaaa
adaaaaaaafaaaaaaadadaaaaomaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaa
adadaaaaomaaaaaaagaaaaaaaaaaaaaaadaaaaaaahaaaaaaadadaaaaomaaaaaa
ahaaaaaaaaaaaaaaadaaaaaaaiaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaa
feeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl
fdeieefcemakaaaaeaaaaaaajdacaaaafjaaaaaeegiocaaaaaaaaaaaaiaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaaddcbabaaaadaaaaaa
gcbaaaaddcbabaaaaeaaaaaagcbaaaaddcbabaaaafaaaaaagcbaaaaddcbabaaa
agaaaaaagcbaaaaddcbabaaaahaaaaaagcbaaaaddcbabaaaaiaaaaaagfaaaaad
pccabaaaaaaaaaaagiaaaaacaeaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaa
adaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaailcaabaaaaaaaaaaa
egaibaaaaaaaaaaaegiicaaaaaaaaaaaadaaaaaaaaaaaaahjcaabaaaaaaaaaaa
fganbaaaaaaaaaaaagaabaaaaaaaaaaadiaaaaahccaabaaaaaaaaaaadkaabaaa
aaaaaaaabkaabaaaaaaaaaaadcaaaaakbcaabaaaaaaaaaaackaabaaaaaaaaaaa
ckiacaaaaaaaaaaaadaaaaaaakaabaaaaaaaaaaaelaaaaafccaabaaaaaaaaaaa
bkaabaaaaaaaaaaaapaaaaaiccaabaaaaaaaaaaapgipcaaaaaaaaaaaadaaaaaa
fgafbaaaaaaaaaaaaaaaaaahccaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaa
aaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaafaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadiaaaaailcaabaaaabaaaaaaegaibaaaabaaaaaaegiicaaa
aaaaaaaaadaaaaaaaaaaaaahjcaabaaaabaaaaaafganbaaaabaaaaaaagaabaaa
abaaaaaadiaaaaahicaabaaaaaaaaaaadkaabaaaabaaaaaabkaabaaaabaaaaaa
dcaaaaakbcaabaaaabaaaaaackaabaaaabaaaaaackiacaaaaaaaaaaaadaaaaaa
akaabaaaabaaaaaaelaaaaaficaabaaaaaaaaaaadkaabaaaaaaaaaaaapaaaaai
icaabaaaaaaaaaaapgipcaaaaaaaaaaaadaaaaaapgapbaaaaaaaaaaaaaaaaaah
ecaabaaaabaaaaaadkaabaaaaaaaaaaaakaabaaaabaaaaaadgaaaaafbcaabaaa
aaaaaaaackaabaaaabaaaaaaefaaaaajpcaabaaaacaaaaaaegbabaaaagaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaailcaabaaaacaaaaaaegaibaaa
acaaaaaaegiicaaaaaaaaaaaadaaaaaaaaaaaaahjcaabaaaacaaaaaafganbaaa
acaaaaaaagaabaaaacaaaaaadiaaaaahicaabaaaaaaaaaaadkaabaaaacaaaaaa
bkaabaaaacaaaaaadcaaaaakicaabaaaabaaaaaackaabaaaacaaaaaackiacaaa
aaaaaaaaadaaaaaaakaabaaaacaaaaaaelaaaaaficaabaaaaaaaaaaadkaabaaa
aaaaaaaaapaaaaaiicaabaaaaaaaaaaapgipcaaaaaaaaaaaadaaaaaapgapbaaa
aaaaaaaaaaaaaaahecaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaaabaaaaaa
baaaaaakicaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaaaaa
egacbaaaaaaaaaaaefaaaaajpcaabaaaacaaaaaaegbabaaaaeaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaadiaaaaailcaabaaaacaaaaaaegaibaaaacaaaaaa
egiicaaaaaaaaaaaadaaaaaaaaaaaaahjcaabaaaacaaaaaafganbaaaacaaaaaa
agaabaaaacaaaaaadiaaaaahicaabaaaabaaaaaadkaabaaaacaaaaaabkaabaaa
acaaaaaadcaaaaakbcaabaaaacaaaaaackaabaaaacaaaaaackiacaaaaaaaaaaa
adaaaaaaakaabaaaacaaaaaaelaaaaaficaabaaaabaaaaaadkaabaaaabaaaaaa
apaaaaaiicaabaaaabaaaaaapgipcaaaaaaaaaaaadaaaaaapgapbaaaabaaaaaa
aaaaaaahccaabaaaacaaaaaadkaabaaaabaaaaaaakaabaaaacaaaaaaefaaaaaj
pcaabaaaadaaaaaaegbabaaaahaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
diaaaaailcaabaaaadaaaaaaegaibaaaadaaaaaaegiicaaaaaaaaaaaadaaaaaa
aaaaaaahjcaabaaaadaaaaaafganbaaaadaaaaaaagaabaaaadaaaaaadiaaaaah
icaabaaaabaaaaaadkaabaaaadaaaaaabkaabaaaadaaaaaadcaaaaakicaabaaa
acaaaaaackaabaaaadaaaaaackiacaaaaaaaaaaaadaaaaaaakaabaaaadaaaaaa
elaaaaaficaabaaaabaaaaaadkaabaaaabaaaaaaapaaaaaiicaabaaaabaaaaaa
pgipcaaaaaaaaaaaadaaaaaapgapbaaaabaaaaaaaaaaaaahccaabaaaaaaaaaaa
dkaabaaaabaaaaaadkaabaaaacaaaaaadgaaaaafbcaabaaaacaaaaaabkaabaaa
aaaaaaaaefaaaaajpcaabaaaadaaaaaaegbabaaaaiaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadiaaaaailcaabaaaadaaaaaaegaibaaaadaaaaaaegiicaaa
aaaaaaaaadaaaaaaaaaaaaahjcaabaaaadaaaaaafganbaaaadaaaaaaagaabaaa
adaaaaaadiaaaaahicaabaaaabaaaaaadkaabaaaadaaaaaabkaabaaaadaaaaaa
dcaaaaakicaabaaaacaaaaaackaabaaaadaaaaaackiacaaaaaaaaaaaadaaaaaa
akaabaaaadaaaaaaelaaaaaficaabaaaabaaaaaadkaabaaaabaaaaaaapaaaaai
icaabaaaabaaaaaapgipcaaaaaaaaaaaadaaaaaapgapbaaaabaaaaaaaaaaaaah
bcaabaaaabaaaaaadkaabaaaabaaaaaadkaabaaaacaaaaaadgaaaaafecaabaaa
acaaaaaaakaabaaaabaaaaaabaaaaaakicaabaaaabaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaiadpaaaaaaaaegacbaaaacaaaaaaaaaaaaaiccaabaaaacaaaaaa
dkaabaiaebaaaaaaaaaaaaaadkaabaaaabaaaaaaefaaaaajpcaabaaaadaaaaaa
egbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaailcaabaaa
adaaaaaaegaibaaaadaaaaaaegiicaaaaaaaaaaaadaaaaaaaaaaaaahmcaabaaa
acaaaaaafganbaaaadaaaaaaagaabaaaadaaaaaadiaaaaahicaabaaaaaaaaaaa
dkaabaaaacaaaaaabkaabaaaadaaaaaadcaaaaakicaabaaaabaaaaaackaabaaa
adaaaaaackiacaaaaaaaaaaaadaaaaaackaabaaaacaaaaaaelaaaaaficaabaaa
aaaaaaaadkaabaaaaaaaaaaaapaaaaaiicaabaaaaaaaaaaapgipcaaaaaaaaaaa
adaaaaaapgapbaaaaaaaaaaaaaaaaaahccaabaaaabaaaaaadkaabaaaaaaaaaaa
dkaabaaaabaaaaaabaaaaaakicaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaiadpaaaaaaaaegacbaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaailcaabaaaabaaaaaa
egaibaaaabaaaaaaegiicaaaaaaaaaaaadaaaaaaaaaaaaahjcaabaaaabaaaaaa
fganbaaaabaaaaaaagaabaaaabaaaaaadiaaaaahccaabaaaabaaaaaadkaabaaa
abaaaaaabkaabaaaabaaaaaadcaaaaakbcaabaaaabaaaaaackaabaaaabaaaaaa
ckiacaaaaaaaaaaaadaaaaaaakaabaaaabaaaaaaelaaaaafccaabaaaabaaaaaa
bkaabaaaabaaaaaaapaaaaaiccaabaaaabaaaaaapgipcaaaaaaaaaaaadaaaaaa
fgafbaaaabaaaaaaaaaaaaahbcaabaaaaaaaaaaabkaabaaaabaaaaaaakaabaaa
abaaaaaabaaaaaakbcaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaaaaaegacbaaaaaaaaaaaaaaaaaaibcaabaaaacaaaaaaakaabaiaebaaaaaa
aaaaaaaadkaabaaaaaaaaaaadiaaaaaidcaabaaaaaaaaaaaegaabaaaacaaaaaa
fgifcaaaaaaaaaaaahaaaaaadiaaaaakdcaabaaaaaaaaaaaegaabaaaaaaaaaaa
aceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaadgaaaaafecaabaaaaaaaaaaa
abeaaaaaaaaaaadpaaaaaaakhcaabaaaaaaaaaaaegacbaaaaaaaaaaaaceaaaaa
aaaaaadpaaaaaadpaaaaaadpaaaaaaaabaaaaaahicaabaaaaaaaaaaaegacbaaa
aaaaaaaaegacbaaaaaaaaaaaeeaaaaaficaabaaaaaaaaaaadkaabaaaaaaaaaaa
diaaaaahhccabaaaaaaaaaaapgapbaaaaaaaaaaaegacbaaaaaaaaaaadgaaaaaf
iccabaaaaaaaaaaaabeaaaaaaaaaiadpdoaaaaab""
}
}
 }
}
Fallback Off
}";

		private const String dlaaShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 44.9KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/DLAA"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = ""white"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 14 math, 5 texture
 // Stats for Fragment shader:
 //       d3d11 : 9 math, 5 texture
 //        d3d9 : 13 math, 5 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 60294
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 14 math, 5 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  vec4 tmpvar_2;
  tmpvar_2.xyz = tmpvar_1.xyz;
  tmpvar_2.w = dot ((4.0 * abs(
    ((((texture2D (_MainTex, 
      (xlv_TEXCOORD0 - _MainTex_TexelSize.xy)
    ) + texture2D (_MainTex, 
      (xlv_TEXCOORD0 + (vec2(1.0, -1.0) * _MainTex_TexelSize.xy))
    )) + texture2D (_MainTex, (xlv_TEXCOORD0 + 
      (vec2(-1.0, 1.0) * _MainTex_TexelSize.xy)
    ))) + texture2D (_MainTex, (xlv_TEXCOORD0 + _MainTex_TexelSize.xy))) - (4.0 * tmpvar_1))
  )).xyz, vec3(0.33, 0.33, 0.33));
  gl_FragData[0] = tmpvar_2;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 5 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
""vs_2_0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov oT0.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 4 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""UnityPerDraw"" 0
""vs_4_0
eefiecedaffpdldohodkdgpagjklpapmmnbhcfmlabaaaaaaoeabaaaaadaaaaaa
cmaaaaaaiaaaaaaaniaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefcaeabaaaa
eaaaabaaebaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaa
doaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 13 math, 5 textures
Vector 0 [_MainTex_TexelSize]
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
def c1, 0.330000013, 0, 0, 0
def c2, 1, -1, 1, 4
dcl t0.xy
dcl_2d s0
add r0.xy, t0, -c0
mov r1.xyz, c2
mad r2.xy, c0, r1, t0
mad r1.xy, c0, r1.yzxw, t0
add r3.xy, t0, c0
texld r0, r0, s0
texld r2, r2, s0
texld r1, r1, s0
texld r3, r3, s0
texld_pp r4, t0, s0
add r0.xyz, r0, r2
add r0.xyz, r1, r0
add r0.xyz, r3, r0
mad r0.xyz, r4, -c2.w, r0
abs r0.xyz, r0
mul r0.xyz, r0, c2.w
dp3_pp r4.w, r0, c1.x
mov_pp oC0, r4

""
}
SubProgram ""d3d11 "" {
// Stats: 9 math, 5 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 112
Vector 96 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedjlilcehclpnnahgblejpglghpmenpjdjabaaaaaadeadaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcheacaaaa
eaaaaaaajnaaaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacadaaaaaaaaaaaaajdcaabaaaaaaaaaaa
egbabaaaabaaaaaaegiacaiaebaaaaaaaaaaaaaaagaaaaaaefaaaaajpcaabaaa
aaaaaaaaegaabaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaan
pcaabaaaabaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaiadpaaaaialp
aaaaialpaaaaiadpegbebaaaabaaaaaaefaaaaajpcaabaaaacaaaaaaegaabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaa
ogakbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahhcaabaaa
aaaaaaaaegacbaaaaaaaaaaaegacbaaaacaaaaaaaaaaaaahhcaabaaaaaaaaaaa
egacbaaaabaaaaaaegacbaaaaaaaaaaaaaaaaaaidcaabaaaabaaaaaaegbabaaa
abaaaaaaegiacaaaaaaaaaaaagaaaaaaefaaaaajpcaabaaaabaaaaaaegaabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahhcaabaaaaaaaaaaa
egacbaaaaaaaaaaaegacbaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaanhcaabaaaaaaaaaaa
egacbaiaebaaaaaaabaaaaaaaceaaaaaaaaaiaeaaaaaiaeaaaaaiaeaaaaaaaaa
egacbaaaaaaaaaaadgaaaaafhccabaaaaaaaaaaaegacbaaaabaaaaaadiaaaaal
hcaabaaaaaaaaaaaegacbaiaibaaaaaaaaaaaaaaaceaaaaaaaaaiaeaaaaaiaea
aaaaiaeaaaaaaaaabaaaaaakiccabaaaaaaaaaaaegacbaaaaaaaaaaaaceaaaaa
mdpfkidomdpfkidomdpfkidoaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 156 math, 25 texture, 5 branch
 // Stats for Fragment shader:
 //       d3d11 : 93 math, 21 texture
 //        d3d9 : 98 math, 21 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 72943
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 156 math, 25 textures, 5 branches
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 clr_1;
  vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  vec4 tmpvar_3;
  vec2 cse_4;
  cse_4 = (vec2(1.5, 0.0) * _MainTex_TexelSize.xy);
  vec2 cse_5;
  cse_5 = (vec2(-1.5, 0.0) * _MainTex_TexelSize.xy);
  tmpvar_3 = (2.0 * (texture2D (_MainTex, (xlv_TEXCOORD0 + cse_5)) + texture2D (_MainTex, (xlv_TEXCOORD0 + cse_4))));
  vec4 tmpvar_6;
  vec2 cse_7;
  cse_7 = (vec2(0.0, 1.5) * _MainTex_TexelSize.xy);
  vec2 cse_8;
  cse_8 = (vec2(0.0, -1.5) * _MainTex_TexelSize.xy);
  tmpvar_6 = (2.0 * (texture2D (_MainTex, (xlv_TEXCOORD0 + cse_8)) + texture2D (_MainTex, (xlv_TEXCOORD0 + cse_7))));
  vec4 tmpvar_9;
  tmpvar_9 = ((tmpvar_3 + (2.0 * tmpvar_2)) / 6.0);
  vec4 tmpvar_10;
  tmpvar_10 = ((tmpvar_6 + (2.0 * tmpvar_2)) / 6.0);
  vec4 tmpvar_11;
  tmpvar_11 = mix (mix (tmpvar_2, tmpvar_9, vec4(clamp (
    (((3.0 * dot (
      (abs((tmpvar_6 - (4.0 * tmpvar_2))) / 4.0)
    .xyz, vec3(0.33, 0.33, 0.33))) - 0.1) / dot (tmpvar_9.xyz, vec3(0.33, 0.33, 0.33)))
  , 0.0, 1.0))), tmpvar_10, vec4(clamp ((
    ((3.0 * dot ((
      abs((tmpvar_3 - (4.0 * tmpvar_2)))
     / 4.0).xyz, vec3(0.33, 0.33, 0.33))) - 0.1)
   / 
    dot (tmpvar_10.xyz, vec3(0.33, 0.33, 0.33))
  ), 0.0, 1.0)));
  clr_1 = tmpvar_11;
  vec4 tmpvar_12;
  tmpvar_12 = texture2D (_MainTex, (xlv_TEXCOORD0 + cse_4));
  vec4 tmpvar_13;
  tmpvar_13 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(3.5, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_14;
  tmpvar_14 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(5.5, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_15;
  tmpvar_15 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(7.5, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_16;
  tmpvar_16 = texture2D (_MainTex, (xlv_TEXCOORD0 + cse_5));
  vec4 tmpvar_17;
  tmpvar_17 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(-3.5, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_18;
  tmpvar_18 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(-5.5, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_19;
  tmpvar_19 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(-7.5, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_20;
  tmpvar_20 = texture2D (_MainTex, (xlv_TEXCOORD0 + cse_7));
  vec4 tmpvar_21;
  tmpvar_21 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, 3.5) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_22;
  tmpvar_22 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, 5.5) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_23;
  tmpvar_23 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, 7.5) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_24;
  tmpvar_24 = texture2D (_MainTex, (xlv_TEXCOORD0 + cse_8));
  vec4 tmpvar_25;
  tmpvar_25 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, -3.5) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_26;
  tmpvar_26 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, -5.5) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_27;
  tmpvar_27 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, -7.5) * _MainTex_TexelSize.xy)));
  float tmpvar_28;
  tmpvar_28 = clamp (((
    ((((
      ((((tmpvar_12.w + tmpvar_13.w) + tmpvar_14.w) + tmpvar_15.w) + tmpvar_16.w)
     + tmpvar_17.w) + tmpvar_18.w) + tmpvar_19.w) / 8.0)
   * 2.0) - 1.0), 0.0, 1.0);
  float tmpvar_29;
  tmpvar_29 = clamp (((
    ((((
      ((((tmpvar_20.w + tmpvar_21.w) + tmpvar_22.w) + tmpvar_23.w) + tmpvar_24.w)
     + tmpvar_25.w) + tmpvar_26.w) + tmpvar_27.w) / 8.0)
   * 2.0) - 1.0), 0.0, 1.0);
  vec4 tmpvar_30;
  tmpvar_30 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(-1.0, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_31;
  tmpvar_31 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(1.0, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_32;
  tmpvar_32 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, -1.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_33;
  tmpvar_33 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, 1.0) * _MainTex_TexelSize.xy)));
  if (((tmpvar_28 > 0.0) || (tmpvar_29 > 0.0))) {
    float tmpvar_34;
    tmpvar_34 = dot (((
      ((((
        ((tmpvar_12 + tmpvar_13) + tmpvar_14)
       + tmpvar_15) + tmpvar_16) + tmpvar_17) + tmpvar_18)
     + tmpvar_19) / 8.0).xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_35;
    tmpvar_35 = dot (((
      ((((
        ((tmpvar_20 + tmpvar_21) + tmpvar_22)
       + tmpvar_23) + tmpvar_24) + tmpvar_25) + tmpvar_26)
     + tmpvar_27) / 8.0).xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_36;
    tmpvar_36 = dot (tmpvar_2.xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_37;
    tmpvar_37 = dot (tmpvar_30.xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_38;
    tmpvar_38 = dot (tmpvar_31.xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_39;
    tmpvar_39 = dot (tmpvar_32.xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_40;
    tmpvar_40 = dot (tmpvar_33.xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_41;
    if ((tmpvar_36 == tmpvar_39)) {
      tmpvar_41 = 0.0;
    } else {
      tmpvar_41 = clamp (((tmpvar_34 - tmpvar_39) / (tmpvar_36 - tmpvar_39)), 0.0, 1.0);
    };
    float tmpvar_42;
    if ((tmpvar_36 == tmpvar_40)) {
      tmpvar_42 = 0.0;
    } else {
      tmpvar_42 = clamp ((1.0 + (
        (tmpvar_34 - tmpvar_36)
       / 
        (tmpvar_36 - tmpvar_40)
      )), 0.0, 1.0);
    };
    float tmpvar_43;
    if ((tmpvar_36 == tmpvar_37)) {
      tmpvar_43 = 0.0;
    } else {
      tmpvar_43 = clamp (((tmpvar_35 - tmpvar_37) / (tmpvar_36 - tmpvar_37)), 0.0, 1.0);
    };
    float tmpvar_44;
    if ((tmpvar_36 == tmpvar_38)) {
      tmpvar_44 = 0.0;
    } else {
      tmpvar_44 = clamp ((1.0 + (
        (tmpvar_35 - tmpvar_36)
       / 
        (tmpvar_36 - tmpvar_38)
      )), 0.0, 1.0);
    };
    clr_1 = mix (mix (tmpvar_11, mix (tmpvar_31, 
      mix (tmpvar_30, tmpvar_2, vec4(tmpvar_43))
    , vec4(tmpvar_44)), vec4(tmpvar_29)), mix (tmpvar_33, mix (tmpvar_32, tmpvar_2, vec4(tmpvar_41)), vec4(tmpvar_42)), vec4(tmpvar_28));
  };
  gl_FragData[0] = clr_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 5 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
""vs_3_0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 4 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""UnityPerDraw"" 0
""vs_4_0
eefiecedaffpdldohodkdgpagjklpapmmnbhcfmlabaaaaaaoeabaaaaadaaaaaa
cmaaaaaaiaaaaaaaniaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefcaeabaaaa
eaaaabaaebaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaa
doaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 98 math, 21 textures
Vector 0 [_MainTex_TexelSize]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c1, 0.25, 2, 0.166666672, 0.330000013
def c2, 3, -0.100000001, 0.125, 1
def c3, 0.25, -1, 0, 1
def c4, -5.5, 0, -7.5, 0
def c5, 7.5, 0, -3.5, 0
def c6, 3.5, 0, 5.5, 1
def c7, -1.5, 0, 1.5, 4
dcl_texcoord v0.xy
dcl_2d s0
mov r0.xy, c0
mad r1, r0.xyxy, c6.yxyz, v0.xyxy
texld r2, r1.zwzw, s0
texld r1, r1, s0
mad r3, r0.xyxy, c7.yxyz, v0.xyxy
texld r4, r3.zwzw, s0
texld r3, r3, s0
add r1, r1.wxyz, r4.wxyz
add r4, r4, r3
add r1, r2.wxyz, r1
mad r2, r0.xyxy, c5.yxyz, v0.xyxy
texld r5, r2, s0
texld r2, r2.zwzw, s0
add r1, r1, r5.wxyz
add r1, r3.wxyz, r1
add r1, r2.wxyz, r1
mad r2, r0.xyxy, c4.yxyz, v0.xyxy
texld r3, r2, s0
texld r2, r2.zwzw, s0
add r1, r1, r3.wxyz
add r1, r2.wxyz, r1
mad_sat r0.z, r1.x, c3.x, c3.y
mul r1.xyz, r1.yzww, c2.z
dp3 r0.w, r1, c1.w
add r1.xyz, r4, r4
texld r2, v0, s0
mad r1.xyz, r2, -c7.w, r1
mul r1.xyz, r1_abs, c1.x
dp3 r1.x, r1, c1.w
mad r1.x, r1.x, c2.x, c2.y
mad r3, r0.xyxy, c7.xyzy, v0.xyxy
texld r5, r3, s0
texld r3, r3.zwzw, s0
add r6, r3, r5
add r7, r2, r2
mad r8, r6, c1.y, r7
add r1.yzw, r6.xxyz, r6.xxyz
mad r1.yzw, r2.xxyz, -c7.w, r1
mul r1.yzw, r1_abs, c1.x
dp3 r1.y, r1.yzww, c1.w
mad r1.y, r1.y, c2.x, c2.y
mad r4, r4, c1.y, r7
mul r6.xyz, r8, c1.z
mad r7, r8, c1.z, -r2
dp3 r1.z, r6, c1.w
rcp r1.z, r1.z
mul_sat r1.x, r1.z, r1.x
mad r6, r1.x, r7, r2
mad r7, r4, c1.z, -r6
mul r1.xzw, r4.xyyz, c1.z
dp3 r1.x, r1.xzww, c1.w
rcp r1.x, r1.x
mul_sat r1.x, r1.x, r1.y
mad r1, r1.x, r7, r6
mad r4, r0.xyxy, c3.yzwz, v0.xyxy
texld r6, r4, s0
texld r4, r4.zwzw, s0
dp3 r7.x, r6, c1.w
add r7.y, r0.w, -r7.x
dp3 r7.z, r2, c1.w
add r7.x, -r7.x, r7.z
rcp r7.w, r7.x
mul_sat r7.y, r7.w, r7.y
cmp r7.x, -r7_abs.x, c7.y, r7.y
lrp r8, r7.x, r2, r6
add r0.w, r0.w, -r7.z
dp3 r6.x, r4, c1.w
add r6.x, -r6.x, r7.z
rcp r6.y, r6.x
mad_sat r0.w, r0.w, r6.y, c2.w
cmp r0.w, -r6_abs.x, c7.y, r0.w
lrp r6, r0.w, r8, r4
lrp r4, r0.z, r6, r1
mad r6, r0.xyxy, c6.xyzy, v0.xyxy
texld r8, r6, s0
texld r6, r6.zwzw, s0
add r3, r3.wxyz, r8.wxyz
add r3, r6.wxyz, r3
mad r6, r0.xyxy, c5.xyzy, v0.xyxy
texld r8, r6, s0
texld r6, r6.zwzw, s0
add r3, r3, r8.wxyz
add r3, r5.wxyz, r3
add r3, r6.wxyz, r3
mad r5, r0.xyxy, c4.xyzy, v0.xyxy
texld r6, r5, s0
texld r5, r5.zwzw, s0
add r3, r3, r6.wxyz
add r3, r5.wxyz, r3
mad_sat r0.w, r3.x, c3.x, c3.y
mul r3.xyz, r3.yzww, c2.z
dp3 r3.x, r3, c1.w
mad r5, r0.xyxy, c3.zyzw, v0.xyxy
texld r6, r5, s0
texld r5, r5.zwzw, s0
dp3 r0.x, r6, c1.w
add r0.y, -r0.x, r3.x
add r3.x, -r7.z, r3.x
add r0.x, -r0.x, r7.z
rcp r3.y, r0.x
mul_sat r0.y, r0.y, r3.y
cmp r0.x, -r0_abs.x, c7.y, r0.y
lrp r8, r0.x, r2, r6
dp3 r0.x, r5, c1.w
add r0.x, -r0.x, r7.z
rcp r0.y, r0.x
mad_sat r0.y, r3.x, r0.y, c2.w
cmp r0.x, -r0_abs.x, c7.y, r0.y
lrp r2, r0.x, r8, r5
lrp_pp r3, r0.w, r2, r4
cmp r0.xz, -r0.wyzw, c6.y, c6.w
add r0.x, r0.z, r0.x
cmp_pp oC0, -r0.x, r1, r3

""
}
SubProgram ""d3d11 "" {
// Stats: 93 math, 21 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 112
Vector 96 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedmmlkkfehkioelkbdchikpfjmjpebbkbdabaaaaaanabbaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcbabbaaaa
eaaaaaaaeeaeaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacakaaaaaadcaaaaanpcaabaaaaaaaaaaa
egiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaamalpaaaaaaaaaaaamadp
egbebaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegaabaaaaaaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaaaaaaaaaogakbaaaaaaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaacaaaaaaegaobaaa
aaaaaaaaegaobaaaabaaaaaaaaaaaaahhcaabaaaadaaaaaaegacbaaaacaaaaaa
egacbaaaacaaaaaaefaaaaajpcaabaaaaeaaaaaaegbabaaaabaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaadcaaaaanhcaabaaaadaaaaaaegacbaiaebaaaaaa
aeaaaaaaaceaaaaaaaaaiaeaaaaaiaeaaaaaiaeaaaaaaaaaegacbaaaadaaaaaa
diaaaaalhcaabaaaadaaaaaaegacbaiaibaaaaaaadaaaaaaaceaaaaaaaaaiado
aaaaiadoaaaaiadoaaaaaaaabaaaaaakbcaabaaaadaaaaaaegacbaaaadaaaaaa
aceaaaaamdpfkidomdpfkidomdpfkidoaaaaaaaadcaaaaajbcaabaaaadaaaaaa
akaabaaaadaaaaaaabeaaaaaaaaaeaeaabeaaaaamnmmmmlndcaaaaanpcaabaaa
afaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaamalpaaaaaaaaaaaamadp
aaaaaaaaegbebaaaabaaaaaaefaaaaajpcaabaaaagaaaaaaegaabaaaafaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaafaaaaaaogakbaaa
afaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaahaaaaaa
egaobaaaafaaaaaaegaobaaaagaaaaaaaaaaaaahpcaabaaaaiaaaaaaegaobaaa
aeaaaaaaegaobaaaaeaaaaaadcaaaaampcaabaaaajaaaaaaegaobaaaahaaaaaa
aceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaeaegaobaaaaiaaaaaaaaaaaaah
ocaabaaaadaaaaaaagajbaaaahaaaaaaagajbaaaahaaaaaadcaaaaanocaabaaa
adaaaaaaagajbaiaebaaaaaaaeaaaaaaaceaaaaaaaaaaaaaaaaaiaeaaaaaiaea
aaaaiaeafgaobaaaadaaaaaadiaaaaalocaabaaaadaaaaaafgaobaiaibaaaaaa
adaaaaaaaceaaaaaaaaaaaaaaaaaiadoaaaaiadoaaaaiadobaaaaaakccaabaaa
adaaaaaajgahbaaaadaaaaaaaceaaaaamdpfkidomdpfkidomdpfkidoaaaaaaaa
dcaaaaajccaabaaaadaaaaaabkaabaaaadaaaaaaabeaaaaaaaaaeaeaabeaaaaa
mnmmmmlndcaaaaampcaabaaaacaaaaaaegaobaaaacaaaaaaaceaaaaaaaaaaaea
aaaaaaeaaaaaaaeaaaaaaaeaegaobaaaaiaaaaaadiaaaaakhcaabaaaahaaaaaa
egacbaaaajaaaaaaaceaaaaaklkkckdoklkkckdoklkkckdoaaaaaaaadcaaaaan
pcaabaaaaiaaaaaaegaobaaaajaaaaaaaceaaaaaklkkckdoklkkckdoklkkckdo
klkkckdoegaobaiaebaaaaaaaeaaaaaabaaaaaakecaabaaaadaaaaaaegacbaaa
ahaaaaaaaceaaaaamdpfkidomdpfkidomdpfkidoaaaaaaaaaocaaaahbcaabaaa
adaaaaaaakaabaaaadaaaaaackaabaaaadaaaaaadcaaaaajpcaabaaaahaaaaaa
agaabaaaadaaaaaaegaobaaaaiaaaaaaegaobaaaaeaaaaaadcaaaaanpcaabaaa
aiaaaaaaegaobaaaacaaaaaaaceaaaaaklkkckdoklkkckdoklkkckdoklkkckdo
egaobaiaebaaaaaaahaaaaaadiaaaaakhcaabaaaacaaaaaaegacbaaaacaaaaaa
aceaaaaaklkkckdoklkkckdoklkkckdoaaaaaaaabaaaaaakbcaabaaaacaaaaaa
egacbaaaacaaaaaaaceaaaaamdpfkidomdpfkidomdpfkidoaaaaaaaaaocaaaah
bcaabaaaacaaaaaabkaabaaaadaaaaaaakaabaaaacaaaaaadcaaaaajpcaabaaa
acaaaaaaagaabaaaacaaaaaaegaobaaaaiaaaaaaegaobaaaahaaaaaadcaaaaan
pcaabaaaadaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaagaea
aaaaaaaaaaaalaeaegbebaaaabaaaaaaefaaaaajpcaabaaaahaaaaaaegaabaaa
adaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaadaaaaaa
ogakbaaaadaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaa
aaaaaaaadgajbaaaaaaaaaaadgajbaaaahaaaaaaaaaaaaahpcaabaaaaaaaaaaa
dgajbaaaadaaaaaaegaobaaaaaaaaaaadcaaaaanpcaabaaaadaaaaaaegiecaaa
aaaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaapaeaaaaaaaaaaaaagamaegbebaaa
abaaaaaaefaaaaajpcaabaaaahaaaaaaegaabaaaadaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaefaaaaajpcaabaaaadaaaaaaogakbaaaadaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaaegaobaaaaaaaaaaa
dgajbaaaahaaaaaaaaaaaaahpcaabaaaaaaaaaaadgajbaaaabaaaaaaegaobaaa
aaaaaaaaaaaaaaahpcaabaaaaaaaaaaadgajbaaaadaaaaaaegaobaaaaaaaaaaa
dcaaaaanpcaabaaaabaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaaa
aaaalamaaaaaaaaaaaaapamaegbebaaaabaaaaaaefaaaaajpcaabaaaadaaaaaa
egaabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
abaaaaaaogakbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaah
pcaabaaaaaaaaaaaegaobaaaaaaaaaaadgajbaaaadaaaaaaaaaaaaahpcaabaaa
aaaaaaaadgajbaaaabaaaaaaegaobaaaaaaaaaaadiaaaaakocaabaaaaaaaaaaa
fgaobaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaadoaaaaaadoaaaaaadodccaaaaj
bcaabaaaaaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaiadoabeaaaaaaaaaialp
baaaaaakccaabaaaaaaaaaaajgahbaaaaaaaaaaaaceaaaaamdpfkidomdpfkido
mdpfkidoaaaaaaaadcaaaaanpcaabaaaabaaaaaaegiecaaaaaaaaaaaagaaaaaa
aceaaaaaaaaaialpaaaaaaaaaaaaiadpaaaaaaaaegbebaaaabaaaaaaefaaaaaj
pcaabaaaadaaaaaaegaabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
efaaaaajpcaabaaaabaaaaaaogakbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaabaaaaaakecaabaaaaaaaaaaaegacbaaaadaaaaaaaceaaaaamdpfkido
mdpfkidomdpfkidoaaaaaaaaaaaaaaaiicaabaaaaaaaaaaackaabaiaebaaaaaa
aaaaaaaabkaabaaaaaaaaaaabaaaaaakbcaabaaaahaaaaaaegacbaaaaeaaaaaa
aceaaaaamdpfkidomdpfkidomdpfkidoaaaaaaaaaaaaaaaiccaabaaaahaaaaaa
ckaabaiaebaaaaaaaaaaaaaaakaabaaaahaaaaaabiaaaaahecaabaaaaaaaaaaa
ckaabaaaaaaaaaaaakaabaaaahaaaaaaaocaaaahicaabaaaaaaaaaaadkaabaaa
aaaaaaaabkaabaaaahaaaaaadhaaaaajecaabaaaaaaaaaaackaabaaaaaaaaaaa
abeaaaaaaaaaaaaadkaabaaaaaaaaaaaaaaaaaaipcaabaaaaiaaaaaaegaobaia
ebaaaaaaadaaaaaaegaobaaaaeaaaaaadcaaaaajpcaabaaaadaaaaaakgakbaaa
aaaaaaaaegaobaaaaiaaaaaaegaobaaaadaaaaaaaaaaaaaipcaabaaaadaaaaaa
egaobaiaebaaaaaaabaaaaaaegaobaaaadaaaaaaaaaaaaaiccaabaaaaaaaaaaa
bkaabaaaaaaaaaaaakaabaiaebaaaaaaahaaaaaabaaaaaakecaabaaaaaaaaaaa
egacbaaaabaaaaaaaceaaaaamdpfkidomdpfkidomdpfkidoaaaaaaaaaaaaaaai
icaabaaaaaaaaaaackaabaiaebaaaaaaaaaaaaaaakaabaaaahaaaaaabiaaaaah
ecaabaaaaaaaaaaackaabaaaaaaaaaaaakaabaaaahaaaaaaaoaaaaahccaabaaa
aaaaaaaabkaabaaaaaaaaaaadkaabaaaaaaaaaaaaacaaaahccaabaaaaaaaaaaa
bkaabaaaaaaaaaaaabeaaaaaaaaaiadpdhaaaaajccaabaaaaaaaaaaackaabaaa
aaaaaaaaabeaaaaaaaaaaaaabkaabaaaaaaaaaaadcaaaaajpcaabaaaabaaaaaa
fgafbaaaaaaaaaaaegaobaaaadaaaaaaegaobaaaabaaaaaaaaaaaaaipcaabaaa
abaaaaaaegaobaiaebaaaaaaacaaaaaaegaobaaaabaaaaaadcaaaaajpcaabaaa
abaaaaaaagaabaaaaaaaaaaaegaobaaaabaaaaaaegaobaaaacaaaaaadbaaaaah
bcaabaaaaaaaaaaaabeaaaaaaaaaaaaaakaabaaaaaaaaaaadcaaaaanpcaabaaa
adaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaagaeaaaaaaaaaaaaalaea
aaaaaaaaegbebaaaabaaaaaaefaaaaajpcaabaaaaiaaaaaaegaabaaaadaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaadaaaaaaogakbaaa
adaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaafaaaaaa
dgajbaaaafaaaaaadgajbaaaaiaaaaaaaaaaaaahpcaabaaaadaaaaaadgajbaaa
adaaaaaaegaobaaaafaaaaaadcaaaaanpcaabaaaafaaaaaaegiecaaaaaaaaaaa
agaaaaaaaceaaaaaaaaapaeaaaaaaaaaaaaagamaaaaaaaaaegbebaaaabaaaaaa
efaaaaajpcaabaaaaiaaaaaaegaabaaaafaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaefaaaaajpcaabaaaafaaaaaaogakbaaaafaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaaaaaaaahpcaabaaaadaaaaaaegaobaaaadaaaaaadgajbaaa
aiaaaaaaaaaaaaahpcaabaaaadaaaaaadgajbaaaagaaaaaaegaobaaaadaaaaaa
aaaaaaahpcaabaaaadaaaaaadgajbaaaafaaaaaaegaobaaaadaaaaaadcaaaaan
pcaabaaaafaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaalamaaaaaaaaa
aaaapamaaaaaaaaaegbebaaaabaaaaaaefaaaaajpcaabaaaagaaaaaaegaabaaa
afaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaafaaaaaa
ogakbaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaa
adaaaaaaegaobaaaadaaaaaadgajbaaaagaaaaaaaaaaaaahpcaabaaaadaaaaaa
dgajbaaaafaaaaaaegaobaaaadaaaaaadiaaaaakocaabaaaaaaaaaaafgaobaaa
adaaaaaaaceaaaaaaaaaaaaaaaaaaadoaaaaaadoaaaaaadodccaaaajbcaabaaa
adaaaaaaakaabaaaadaaaaaaabeaaaaaaaaaiadoabeaaaaaaaaaialpbaaaaaak
ccaabaaaaaaaaaaajgahbaaaaaaaaaaaaceaaaaamdpfkidomdpfkidomdpfkido
aaaaaaaadcaaaaanpcaabaaaafaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaa
aaaaaaaaaaaaialpaaaaaaaaaaaaiadpegbebaaaabaaaaaaefaaaaajpcaabaaa
agaaaaaaegaabaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaaj
pcaabaaaafaaaaaaogakbaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
baaaaaakecaabaaaaaaaaaaaegacbaaaagaaaaaaaceaaaaamdpfkidomdpfkido
mdpfkidoaaaaaaaaaaaaaaaiicaabaaaaaaaaaaackaabaiaebaaaaaaaaaaaaaa
bkaabaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaaakaabaiaebaaaaaaahaaaaaa
bkaabaaaaaaaaaaaaaaaaaaiccaabaaaadaaaaaackaabaiaebaaaaaaaaaaaaaa
akaabaaaahaaaaaabiaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaaakaabaaa
ahaaaaaaaocaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaabkaabaaaadaaaaaa
dhaaaaajecaabaaaaaaaaaaackaabaaaaaaaaaaaabeaaaaaaaaaaaaadkaabaaa
aaaaaaaaaaaaaaaipcaabaaaaeaaaaaaegaobaaaaeaaaaaaegaobaiaebaaaaaa
agaaaaaadcaaaaajpcaabaaaaeaaaaaakgakbaaaaaaaaaaaegaobaaaaeaaaaaa
egaobaaaagaaaaaaaaaaaaaipcaabaaaaeaaaaaaegaobaiaebaaaaaaafaaaaaa
egaobaaaaeaaaaaabaaaaaakecaabaaaaaaaaaaaegacbaaaafaaaaaaaceaaaaa
mdpfkidomdpfkidomdpfkidoaaaaaaaaaaaaaaaiicaabaaaaaaaaaaackaabaia
ebaaaaaaaaaaaaaaakaabaaaahaaaaaabiaaaaahecaabaaaaaaaaaaackaabaaa
aaaaaaaaakaabaaaahaaaaaaaoaaaaahccaabaaaaaaaaaaabkaabaaaaaaaaaaa
dkaabaaaaaaaaaaaaacaaaahccaabaaaaaaaaaaabkaabaaaaaaaaaaaabeaaaaa
aaaaiadpdhaaaaajccaabaaaaaaaaaaackaabaaaaaaaaaaaabeaaaaaaaaaaaaa
bkaabaaaaaaaaaaadcaaaaajpcaabaaaaeaaaaaafgafbaaaaaaaaaaaegaobaaa
aeaaaaaaegaobaaaafaaaaaaaaaaaaaipcaabaaaaeaaaaaaegaobaiaebaaaaaa
abaaaaaaegaobaaaaeaaaaaadcaaaaajpcaabaaaabaaaaaaagaabaaaadaaaaaa
egaobaaaaeaaaaaaegaobaaaabaaaaaadbaaaaahccaabaaaaaaaaaaaabeaaaaa
aaaaaaaaakaabaaaadaaaaaadmaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaa
bkaabaaaaaaaaaaadhaaaaajpccabaaaaaaaaaaaagaabaaaaaaaaaaaegaobaaa
abaaaaaaegaobaaaacaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 159 math, 25 texture, 5 branch
 // Stats for Fragment shader:
 //       d3d11 : 93 math, 21 texture
 //        d3d9 : 99 math, 21 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 180491
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 159 math, 25 textures, 5 branches
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 clr_1;
  vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(-1.0, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(1.0, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, -1.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, 1.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_7;
  vec2 cse_8;
  cse_8 = (vec2(1.5, 0.0) * _MainTex_TexelSize.xy);
  vec2 cse_9;
  cse_9 = (vec2(-1.5, 0.0) * _MainTex_TexelSize.xy);
  tmpvar_7 = (((2.0 * 
    (texture2D (_MainTex, (xlv_TEXCOORD0 + cse_9)) + texture2D (_MainTex, (xlv_TEXCOORD0 + cse_8)))
  ) + (2.0 * tmpvar_2)) / 6.0);
  vec4 tmpvar_10;
  vec2 cse_11;
  cse_11 = (vec2(0.0, 1.5) * _MainTex_TexelSize.xy);
  vec2 cse_12;
  cse_12 = (vec2(0.0, -1.5) * _MainTex_TexelSize.xy);
  tmpvar_10 = (((2.0 * 
    (texture2D (_MainTex, (xlv_TEXCOORD0 + cse_12)) + texture2D (_MainTex, (xlv_TEXCOORD0 + cse_11)))
  ) + (2.0 * tmpvar_2)) / 6.0);
  vec4 tmpvar_13;
  tmpvar_13 = mix (mix (tmpvar_2, tmpvar_7, vec4(clamp (
    (((3.0 * dot (
      (abs(((tmpvar_5 + tmpvar_6) - (2.0 * tmpvar_2))) / 2.0)
    .xyz, vec3(0.33, 0.33, 0.33))) - 0.1) / dot (tmpvar_7.xyz, vec3(0.33, 0.33, 0.33)))
  , 0.0, 1.0))), tmpvar_10, vec4((clamp (
    (((3.0 * dot (
      (abs(((tmpvar_3 + tmpvar_4) - (2.0 * tmpvar_2))) / 2.0)
    .xyz, vec3(0.33, 0.33, 0.33))) - 0.1) / dot (tmpvar_10.xyz, vec3(0.33, 0.33, 0.33)))
  , 0.0, 1.0) * 0.5)));
  clr_1 = tmpvar_13;
  vec4 tmpvar_14;
  tmpvar_14 = texture2D (_MainTex, (xlv_TEXCOORD0 + cse_8));
  vec4 tmpvar_15;
  tmpvar_15 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(3.5, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_16;
  tmpvar_16 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(5.5, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_17;
  tmpvar_17 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(7.5, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_18;
  tmpvar_18 = texture2D (_MainTex, (xlv_TEXCOORD0 + cse_9));
  vec4 tmpvar_19;
  tmpvar_19 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(-3.5, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_20;
  tmpvar_20 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(-5.5, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_21;
  tmpvar_21 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(-7.5, 0.0) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_22;
  tmpvar_22 = texture2D (_MainTex, (xlv_TEXCOORD0 + cse_11));
  vec4 tmpvar_23;
  tmpvar_23 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, 3.5) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_24;
  tmpvar_24 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, 5.5) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_25;
  tmpvar_25 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, 7.5) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_26;
  tmpvar_26 = texture2D (_MainTex, (xlv_TEXCOORD0 + cse_12));
  vec4 tmpvar_27;
  tmpvar_27 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, -3.5) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_28;
  tmpvar_28 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, -5.5) * _MainTex_TexelSize.xy)));
  vec4 tmpvar_29;
  tmpvar_29 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, -7.5) * _MainTex_TexelSize.xy)));
  float tmpvar_30;
  tmpvar_30 = clamp (((
    ((((
      ((((tmpvar_14.w + tmpvar_15.w) + tmpvar_16.w) + tmpvar_17.w) + tmpvar_18.w)
     + tmpvar_19.w) + tmpvar_20.w) + tmpvar_21.w) / 8.0)
   * 2.0) - 1.0), 0.0, 1.0);
  float tmpvar_31;
  tmpvar_31 = clamp (((
    ((((
      ((((tmpvar_22.w + tmpvar_23.w) + tmpvar_24.w) + tmpvar_25.w) + tmpvar_26.w)
     + tmpvar_27.w) + tmpvar_28.w) + tmpvar_29.w) / 8.0)
   * 2.0) - 1.0), 0.0, 1.0);
  float tmpvar_32;
  tmpvar_32 = abs((tmpvar_30 - tmpvar_31));
  if ((tmpvar_32 > 0.2)) {
    float tmpvar_33;
    tmpvar_33 = dot (((
      ((((
        ((tmpvar_14 + tmpvar_15) + tmpvar_16)
       + tmpvar_17) + tmpvar_18) + tmpvar_19) + tmpvar_20)
     + tmpvar_21) / 8.0).xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_34;
    tmpvar_34 = dot (((
      ((((
        ((tmpvar_22 + tmpvar_23) + tmpvar_24)
       + tmpvar_25) + tmpvar_26) + tmpvar_27) + tmpvar_28)
     + tmpvar_29) / 8.0).xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_35;
    tmpvar_35 = dot (tmpvar_2.xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_36;
    tmpvar_36 = dot (tmpvar_3.xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_37;
    tmpvar_37 = dot (tmpvar_4.xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_38;
    tmpvar_38 = dot (tmpvar_5.xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_39;
    tmpvar_39 = dot (tmpvar_6.xyz, vec3(0.33, 0.33, 0.33));
    float tmpvar_40;
    if ((tmpvar_35 == tmpvar_38)) {
      tmpvar_40 = 0.0;
    } else {
      tmpvar_40 = clamp (((tmpvar_33 - tmpvar_38) / (tmpvar_35 - tmpvar_38)), 0.0, 1.0);
    };
    float tmpvar_41;
    if ((tmpvar_35 == tmpvar_39)) {
      tmpvar_41 = 0.0;
    } else {
      tmpvar_41 = clamp ((1.0 + (
        (tmpvar_33 - tmpvar_35)
       / 
        (tmpvar_35 - tmpvar_39)
      )), 0.0, 1.0);
    };
    float tmpvar_42;
    if ((tmpvar_35 == tmpvar_36)) {
      tmpvar_42 = 0.0;
    } else {
      tmpvar_42 = clamp (((tmpvar_34 - tmpvar_36) / (tmpvar_35 - tmpvar_36)), 0.0, 1.0);
    };
    float tmpvar_43;
    if ((tmpvar_35 == tmpvar_37)) {
      tmpvar_43 = 0.0;
    } else {
      tmpvar_43 = clamp ((1.0 + (
        (tmpvar_34 - tmpvar_35)
       / 
        (tmpvar_35 - tmpvar_37)
      )), 0.0, 1.0);
    };
    clr_1 = mix (mix (tmpvar_13, mix (tmpvar_4, 
      mix (tmpvar_3, tmpvar_2, vec4(tmpvar_42))
    , vec4(tmpvar_43)), vec4(tmpvar_31)), mix (tmpvar_6, mix (tmpvar_5, tmpvar_2, vec4(tmpvar_40)), vec4(tmpvar_41)), vec4(tmpvar_30));
  };
  gl_FragData[0] = clr_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 5 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
""vs_3_0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 4 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""UnityPerDraw"" 0
""vs_4_0
eefiecedaffpdldohodkdgpagjklpapmmnbhcfmlabaaaaaaoeabaaaaadaaaaaa
cmaaaaaaiaaaaaaaniaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefcaeabaaaa
eaaaabaaebaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaa
doaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 99 math, 21 textures
Vector 0 [_MainTex_TexelSize]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c1, -1, 0, 1, 0.5
def c2, 0.166666672, 0.330000013, 3, -0.100000001
def c3, 7.5, 0, -3.5, 0.200000003
def c4, 3.5, 0, 5.5, 0.125
def c5, 0.25, -1, 0, 0
def c6, -5.5, 0, -7.5, 0
def c7, -1.5, 0, 1.5, 2
dcl_texcoord v0.xy
dcl_2d s0
mov r0.xy, c0
mad r1, r0.xyxy, c4.yxyz, v0.xyxy
texld r2, r1.zwzw, s0
texld r1, r1, s0
mad r3, r0.xyxy, c7.yxyz, v0.xyxy
texld r4, r3.zwzw, s0
texld r3, r3, s0
add r1, r1.wxyz, r4.wxyz
add r4, r4, r3
add r1, r2.wxyz, r1
mad r2, r0.xyxy, c3.yxyz, v0.xyxy
texld r5, r2, s0
texld r2, r2.zwzw, s0
add r1, r1, r5.wxyz
add r1, r3.wxyz, r1
add r1, r2.wxyz, r1
mad r2, r0.xyxy, c6.yxyz, v0.xyxy
texld r3, r2, s0
texld r2, r2.zwzw, s0
add r1, r1, r3.wxyz
add r1, r2.wxyz, r1
mad_sat r0.z, r1.x, c5.x, c5.y
mul r1.xyz, r1.yzww, c4.w
dp3 r0.w, r1, c2.y
mad r1, r0.xyxy, c1.xyzy, v0.xyxy
texld r2, r1, s0
texld r1, r1.zwzw, s0
dp3 r3.x, r2, c2.y
add r3.y, r0.w, -r3.x
texld r5, v0, s0
dp3 r3.z, r5, c2.y
add r3.x, -r3.x, r3.z
rcp r3.w, r3.x
mul_sat r3.y, r3.w, r3.y
cmp r3.x, -r3_abs.x, c7.y, r3.y
lrp r6, r3.x, r5, r2
add r2.xyz, r1, r2
mad r2.xyz, r5, -c7.w, r2
mul r2.xyz, r2_abs, c1.w
dp3 r2.x, r2, c2.y
mad r2.x, r2.x, c2.z, c2.w
add r0.w, r0.w, -r3.z
dp3 r2.y, r1, c2.y
add r2.y, -r2.y, r3.z
rcp r2.z, r2.y
mad_sat r0.w, r0.w, r2.z, c1.z
cmp r0.w, -r2_abs.y, c7.y, r0.w
lrp r7, r0.w, r6, r1
mad r1, r0.xyxy, c7.xyzy, v0.xyxy
texld r6, r1, s0
texld r1, r1.zwzw, s0
add r8, r1, r6
add r9, r5, r5
mad r8, r8, c7.w, r9
mad r4, r4, c7.w, r9
mul r2.yzw, r8.xxyz, c2.x
mad r8, r8, c2.x, -r5
dp3 r0.w, r2.yzww, c2.y
rcp r0.w, r0.w
mad r9, r0.xyxy, c1.yxyz, v0.xyxy
texld r10, r9, s0
texld r9, r9.zwzw, s0
add r2.yzw, r9.xxyz, r10.xxyz
mad r2.yzw, r5.xxyz, -c7.w, r2
mul r2.yzw, r2_abs, c1.w
dp3 r2.y, r2.yzww, c2.y
mad r2.y, r2.y, c2.z, c2.w
mul_sat r0.w, r0.w, r2.y
mad r8, r0.w, r8, r5
mad r11, r4, c2.x, -r8
mul r2.yzw, r4.xxyz, c2.x
dp3 r0.w, r2.yzww, c2.y
rcp r0.w, r0.w
mul_sat r0.w, r0.w, r2.x
mul r0.w, r0.w, c1.w
mad r2, r0.w, r11, r8
lrp r4, r0.z, r7, r2
mad r7, r0.xyxy, c4.xyzy, v0.xyxy
texld r8, r7, s0
texld r7, r7.zwzw, s0
add r1, r1.wxyz, r8.wxyz
add r1, r7.wxyz, r1
mad r7, r0.xyxy, c3.xyzy, v0.xyxy
texld r8, r7, s0
texld r7, r7.zwzw, s0
add r1, r1, r8.wxyz
add r1, r6.wxyz, r1
add r1, r7.wxyz, r1
mad r6, r0.xyxy, c6.xyzy, v0.xyxy
texld r7, r6, s0
texld r6, r6.zwzw, s0
add r1, r1, r7.wxyz
add r1, r6.wxyz, r1
mad_sat r0.x, r1.x, c5.x, c5.y
mul r1.xyz, r1.yzww, c4.w
dp3 r0.y, r1, c2.y
dp3 r0.w, r10, c2.y
add r1.x, -r0.w, r0.y
add r0.y, -r3.z, r0.y
add r0.w, -r0.w, r3.z
rcp r1.y, r0.w
mul_sat r1.x, r1.y, r1.x
cmp r0.w, -r0_abs.w, c7.y, r1.x
lrp r1, r0.w, r5, r10
dp3 r0.w, r9, c2.y
add r0.w, -r0.w, r3.z
rcp r3.x, r0.w
mad_sat r0.y, r0.y, r3.x, c1.z
cmp r0.y, -r0_abs.w, c7.y, r0.y
lrp r3, r0.y, r1, r9
lrp_pp r1, r0.x, r3, r4
add r0.x, -r0.z, r0.x
add r0.x, -r0_abs.x, c3.w
cmp_pp oC0, r0.x, r2, r1

""
}
SubProgram ""d3d11 "" {
// Stats: 93 math, 21 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 112
Vector 96 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0
eefieceddefmklaefommapimmhfidlnbockajpdpabaaaaaanibbaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcbibbaaaa
eaaaaaaaegaeaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacalaaaaaadcaaaaanpcaabaaaaaaaaaaa
egiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaagaeaaaaaaaaaaaaalaea
egbebaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaogakbaaaaaaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaaaaaaaaaegaabaaaaaaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaanpcaabaaaacaaaaaaegiecaaa
aaaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaamalpaaaaaaaaaaaamadpegbebaaa
abaaaaaaefaaaaajpcaabaaaadaaaaaaogakbaaaacaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaefaaaaajpcaabaaaacaaaaaaegaabaaaacaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaadgajbaaaaaaaaaaa
dgajbaaaadaaaaaaaaaaaaahpcaabaaaadaaaaaaegaobaaaadaaaaaaegaobaaa
acaaaaaaaaaaaaahpcaabaaaaaaaaaaadgajbaaaabaaaaaaegaobaaaaaaaaaaa
dcaaaaanpcaabaaaabaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaaa
aaaapaeaaaaaaaaaaaaagamaegbebaaaabaaaaaaefaaaaajpcaabaaaaeaaaaaa
egaabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
abaaaaaaogakbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaah
pcaabaaaaaaaaaaaegaobaaaaaaaaaaadgajbaaaaeaaaaaaaaaaaaahpcaabaaa
aaaaaaaadgajbaaaacaaaaaaegaobaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
dgajbaaaabaaaaaaegaobaaaaaaaaaaadcaaaaanpcaabaaaabaaaaaaegiecaaa
aaaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaalamaaaaaaaaaaaaapamaegbebaaa
abaaaaaaefaaaaajpcaabaaaacaaaaaaegaabaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaogakbaaaabaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaaegaobaaaaaaaaaaa
dgajbaaaacaaaaaaaaaaaaahpcaabaaaaaaaaaaadgajbaaaabaaaaaaegaobaaa
aaaaaaaadiaaaaakocaabaaaaaaaaaaafgaobaaaaaaaaaaaaceaaaaaaaaaaaaa
aaaaaadoaaaaaadoaaaaaadodccaaaajbcaabaaaaaaaaaaaakaabaaaaaaaaaaa
abeaaaaaaaaaiadoabeaaaaaaaaaialpbaaaaaakccaabaaaaaaaaaaajgahbaaa
aaaaaaaaaceaaaaamdpfkidomdpfkidomdpfkidoaaaaaaaadcaaaaanpcaabaaa
abaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaialpaaaaaaaaaaaaiadp
aaaaaaaaegbebaaaabaaaaaaefaaaaajpcaabaaaacaaaaaaegaabaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaogakbaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaabaaaaaakecaabaaaaaaaaaaa
egacbaaaacaaaaaaaceaaaaamdpfkidomdpfkidomdpfkidoaaaaaaaaaaaaaaai
icaabaaaaaaaaaaackaabaiaebaaaaaaaaaaaaaabkaabaaaaaaaaaaaefaaaaaj
pcaabaaaaeaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
baaaaaakbcaabaaaafaaaaaaegacbaaaaeaaaaaaaceaaaaamdpfkidomdpfkido
mdpfkidoaaaaaaaaaaaaaaaiccaabaaaafaaaaaackaabaiaebaaaaaaaaaaaaaa
akaabaaaafaaaaaabiaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaaakaabaaa
afaaaaaaaocaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaabkaabaaaafaaaaaa
dhaaaaajecaabaaaaaaaaaaackaabaaaaaaaaaaaabeaaaaaaaaaaaaadkaabaaa
aaaaaaaaaaaaaaaipcaabaaaagaaaaaaegaobaiaebaaaaaaacaaaaaaegaobaaa
aeaaaaaadcaaaaajpcaabaaaagaaaaaakgakbaaaaaaaaaaaegaobaaaagaaaaaa
egaobaaaacaaaaaaaaaaaaahhcaabaaaacaaaaaaegacbaaaabaaaaaaegacbaaa
acaaaaaadcaaaaanhcaabaaaacaaaaaaegacbaiaebaaaaaaaeaaaaaaaceaaaaa
aaaaaaeaaaaaaaeaaaaaaaeaaaaaaaaaegacbaaaacaaaaaadiaaaaalhcaabaaa
acaaaaaaegacbaiaibaaaaaaacaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaadp
aaaaaaaabaaaaaakecaabaaaaaaaaaaaegacbaaaacaaaaaaaceaaaaamdpfkido
mdpfkidomdpfkidoaaaaaaaadcaaaaajecaabaaaaaaaaaaackaabaaaaaaaaaaa
abeaaaaaaaaaeaeaabeaaaaamnmmmmlnaaaaaaaipcaabaaaacaaaaaaegaobaia
ebaaaaaaabaaaaaaegaobaaaagaaaaaaaaaaaaaiccaabaaaaaaaaaaabkaabaaa
aaaaaaaaakaabaiaebaaaaaaafaaaaaabaaaaaakicaabaaaaaaaaaaaegacbaaa
abaaaaaaaceaaaaamdpfkidomdpfkidomdpfkidoaaaaaaaaaaaaaaaiccaabaaa
afaaaaaadkaabaiaebaaaaaaaaaaaaaaakaabaaaafaaaaaabiaaaaahicaabaaa
aaaaaaaadkaabaaaaaaaaaaaakaabaaaafaaaaaaaoaaaaahccaabaaaaaaaaaaa
bkaabaaaaaaaaaaabkaabaaaafaaaaaaaacaaaahccaabaaaaaaaaaaabkaabaaa
aaaaaaaaabeaaaaaaaaaiadpdhaaaaajccaabaaaaaaaaaaadkaabaaaaaaaaaaa
abeaaaaaaaaaaaaabkaabaaaaaaaaaaadcaaaaajpcaabaaaabaaaaaafgafbaaa
aaaaaaaaegaobaaaacaaaaaaegaobaaaabaaaaaaaaaaaaahpcaabaaaacaaaaaa
egaobaaaaeaaaaaaegaobaaaaeaaaaaadcaaaaampcaabaaaadaaaaaaegaobaaa
adaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaeaegaobaaaacaaaaaa
diaaaaakocaabaaaafaaaaaaagajbaaaadaaaaaaaceaaaaaaaaaaaaaklkkckdo
klkkckdoklkkckdobaaaaaakccaabaaaaaaaaaaajgahbaaaafaaaaaaaceaaaaa
mdpfkidomdpfkidomdpfkidoaaaaaaaaaocaaaahccaabaaaaaaaaaaackaabaaa
aaaaaaaabkaabaaaaaaaaaaadiaaaaahccaabaaaaaaaaaaabkaabaaaaaaaaaaa
abeaaaaaaaaaaadpdcaaaaanpcaabaaaagaaaaaaegiecaaaaaaaaaaaagaaaaaa
aceaaaaaaaaamalpaaaaaaaaaaaamadpaaaaaaaaegbebaaaabaaaaaaefaaaaaj
pcaabaaaahaaaaaaegaabaaaagaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
efaaaaajpcaabaaaagaaaaaaogakbaaaagaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaaaaaaaahpcaabaaaaiaaaaaaegaobaaaagaaaaaaegaobaaaahaaaaaa
dcaaaaampcaabaaaacaaaaaaegaobaaaaiaaaaaaaceaaaaaaaaaaaeaaaaaaaea
aaaaaaeaaaaaaaeaegaobaaaacaaaaaadcaaaaanpcaabaaaaiaaaaaaegaobaaa
acaaaaaaaceaaaaaklkkckdoklkkckdoklkkckdoklkkckdoegaobaiaebaaaaaa
aeaaaaaadiaaaaakhcaabaaaacaaaaaaegacbaaaacaaaaaaaceaaaaaklkkckdo
klkkckdoklkkckdoaaaaaaaabaaaaaakecaabaaaaaaaaaaaegacbaaaacaaaaaa
aceaaaaamdpfkidomdpfkidomdpfkidoaaaaaaaadcaaaaanpcaabaaaacaaaaaa
egiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaaialpaaaaaaaaaaaaiadp
egbebaaaabaaaaaaefaaaaajpcaabaaaajaaaaaaegaabaaaacaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaacaaaaaaogakbaaaacaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahocaabaaaafaaaaaaagajbaaa
acaaaaaaagajbaaaajaaaaaadcaaaaanocaabaaaafaaaaaaagajbaiaebaaaaaa
aeaaaaaaaceaaaaaaaaaaaaaaaaaaaeaaaaaaaeaaaaaaaeafgaobaaaafaaaaaa
diaaaaalocaabaaaafaaaaaafgaobaiaibaaaaaaafaaaaaaaceaaaaaaaaaaaaa
aaaaaadpaaaaaadpaaaaaadpbaaaaaakicaabaaaaaaaaaaajgahbaaaafaaaaaa
aceaaaaamdpfkidomdpfkidomdpfkidoaaaaaaaadcaaaaajicaabaaaaaaaaaaa
dkaabaaaaaaaaaaaabeaaaaaaaaaeaeaabeaaaaamnmmmmlnaocaaaahecaabaaa
aaaaaaaadkaabaaaaaaaaaaackaabaaaaaaaaaaadcaaaaajpcaabaaaaiaaaaaa
kgakbaaaaaaaaaaaegaobaaaaiaaaaaaegaobaaaaeaaaaaaaaaaaaaipcaabaaa
aeaaaaaaegaobaaaaeaaaaaaegaobaiaebaaaaaaajaaaaaadcaaaaanpcaabaaa
adaaaaaaegaobaaaadaaaaaaaceaaaaaklkkckdoklkkckdoklkkckdoklkkckdo
egaobaiaebaaaaaaaiaaaaaadcaaaaajpcaabaaaadaaaaaafgafbaaaaaaaaaaa
egaobaaaadaaaaaaegaobaaaaiaaaaaaaaaaaaaipcaabaaaabaaaaaaegaobaaa
abaaaaaaegaobaiaebaaaaaaadaaaaaadcaaaaajpcaabaaaabaaaaaaagaabaaa
aaaaaaaaegaobaaaabaaaaaaegaobaaaadaaaaaadcaaaaanpcaabaaaaiaaaaaa
egiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaagaeaaaaaaaaaaaaalaeaaaaaaaaa
egbebaaaabaaaaaaefaaaaajpcaabaaaakaaaaaaegaabaaaaiaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaaiaaaaaaogakbaaaaiaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaagaaaaaadgajbaaa
agaaaaaadgajbaaaakaaaaaaaaaaaaahpcaabaaaagaaaaaadgajbaaaaiaaaaaa
egaobaaaagaaaaaadcaaaaanpcaabaaaaiaaaaaaegiecaaaaaaaaaaaagaaaaaa
aceaaaaaaaaapaeaaaaaaaaaaaaagamaaaaaaaaaegbebaaaabaaaaaaefaaaaaj
pcaabaaaakaaaaaaegaabaaaaiaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
efaaaaajpcaabaaaaiaaaaaaogakbaaaaiaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaaaaaaaahpcaabaaaagaaaaaaegaobaaaagaaaaaadgajbaaaakaaaaaa
aaaaaaahpcaabaaaagaaaaaadgajbaaaahaaaaaaegaobaaaagaaaaaaaaaaaaah
pcaabaaaagaaaaaadgajbaaaaiaaaaaaegaobaaaagaaaaaadcaaaaanpcaabaaa
ahaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaalamaaaaaaaaaaaaapama
aaaaaaaaegbebaaaabaaaaaaefaaaaajpcaabaaaaiaaaaaaegaabaaaahaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaahaaaaaaogakbaaa
ahaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaagaaaaaa
egaobaaaagaaaaaadgajbaaaaiaaaaaaaaaaaaahpcaabaaaagaaaaaadgajbaaa
ahaaaaaaegaobaaaagaaaaaadiaaaaakocaabaaaaaaaaaaafgaobaaaagaaaaaa
aceaaaaaaaaaaaaaaaaaaadoaaaaaadoaaaaaadodccaaaajccaabaaaafaaaaaa
akaabaaaagaaaaaaabeaaaaaaaaaiadoabeaaaaaaaaaialpbaaaaaakccaabaaa
aaaaaaaajgahbaaaaaaaaaaaaceaaaaamdpfkidomdpfkidomdpfkidoaaaaaaaa
baaaaaakecaabaaaaaaaaaaaegacbaaaajaaaaaaaceaaaaamdpfkidomdpfkido
mdpfkidoaaaaaaaaaaaaaaaiicaabaaaaaaaaaaackaabaiaebaaaaaaaaaaaaaa
bkaabaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaaakaabaiaebaaaaaaafaaaaaa
bkaabaaaaaaaaaaaaaaaaaaiecaabaaaafaaaaaackaabaiaebaaaaaaaaaaaaaa
akaabaaaafaaaaaabiaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaaakaabaaa
afaaaaaaaocaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaackaabaaaafaaaaaa
dhaaaaajecaabaaaaaaaaaaackaabaaaaaaaaaaaabeaaaaaaaaaaaaadkaabaaa
aaaaaaaadcaaaaajpcaabaaaaeaaaaaakgakbaaaaaaaaaaaegaobaaaaeaaaaaa
egaobaaaajaaaaaaaaaaaaaipcaabaaaaeaaaaaaegaobaiaebaaaaaaacaaaaaa
egaobaaaaeaaaaaabaaaaaakecaabaaaaaaaaaaaegacbaaaacaaaaaaaceaaaaa
mdpfkidomdpfkidomdpfkidoaaaaaaaaaaaaaaaiicaabaaaaaaaaaaackaabaia
ebaaaaaaaaaaaaaaakaabaaaafaaaaaabiaaaaahecaabaaaaaaaaaaackaabaaa
aaaaaaaaakaabaaaafaaaaaaaoaaaaahccaabaaaaaaaaaaabkaabaaaaaaaaaaa
dkaabaaaaaaaaaaaaacaaaahccaabaaaaaaaaaaabkaabaaaaaaaaaaaabeaaaaa
aaaaiadpdhaaaaajccaabaaaaaaaaaaackaabaaaaaaaaaaaabeaaaaaaaaaaaaa
bkaabaaaaaaaaaaadcaaaaajpcaabaaaacaaaaaafgafbaaaaaaaaaaaegaobaaa
aeaaaaaaegaobaaaacaaaaaaaaaaaaaipcaabaaaacaaaaaaegaobaiaebaaaaaa
abaaaaaaegaobaaaacaaaaaadcaaaaajpcaabaaaabaaaaaafgafbaaaafaaaaaa
egaobaaaacaaaaaaegaobaaaabaaaaaaaaaaaaaibcaabaaaaaaaaaaaakaabaia
ebaaaaaaaaaaaaaabkaabaaaafaaaaaadbaaaaaibcaabaaaaaaaaaaaabeaaaaa
mnmmemdoakaabaiaibaaaaaaaaaaaaaadhaaaaajpccabaaaaaaaaaaaagaabaaa
aaaaaaaaegaobaaaabaaaaaaegaobaaaadaaaaaadoaaaaab""
}
}
 }
}
Fallback Off
}";

		private const String ssaaShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 19.7KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/SSAA"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = ""white"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 9 math
 //    d3d11_9x : 9 math
 //        d3d9 : 12 math
 //      opengl : 58 math, 10 texture, 1 branch
 // Stats for Fragment shader:
 //       d3d11 : 45 math, 10 texture, 2 branch
 //    d3d11_9x : 21 math, 10 texture, 2 branch
 //        d3d9 : 57 math, 9 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 35699
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 58 math, 10 textures, 1 branches
""!!GLSL
#ifdef VERTEX

uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1.x = 0.0;
  tmpvar_1.y = _MainTex_TexelSize.y;
  vec2 tmpvar_2;
  tmpvar_2 = (tmpvar_1 * 1.75);
  vec2 tmpvar_3;
  tmpvar_3.y = 0.0;
  tmpvar_3.x = _MainTex_TexelSize.x;
  vec2 tmpvar_4;
  tmpvar_4 = (tmpvar_3 * 1.75);
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = (gl_MultiTexCoord0.xy - tmpvar_2);
  xlv_TEXCOORD0_1 = (gl_MultiTexCoord0.xy - tmpvar_4);
  xlv_TEXCOORD0_2 = (gl_MultiTexCoord0.xy + tmpvar_4);
  xlv_TEXCOORD0_3 = (gl_MultiTexCoord0.xy + tmpvar_2);
  xlv_TEXCOORD0_4 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
uniform vec4 unity_ColorSpaceLuminance;
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
void main ()
{
  vec4 outColor_1;
  vec3 tmpvar_2;
  tmpvar_2 = (texture2D (_MainTex, xlv_TEXCOORD0).xyz * unity_ColorSpaceLuminance.xyz);
  vec3 tmpvar_3;
  tmpvar_3 = (texture2D (_MainTex, xlv_TEXCOORD0_1).xyz * unity_ColorSpaceLuminance.xyz);
  vec3 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex, xlv_TEXCOORD0_2).xyz * unity_ColorSpaceLuminance.xyz);
  vec3 tmpvar_5;
  tmpvar_5 = (texture2D (_MainTex, xlv_TEXCOORD0_3).xyz * unity_ColorSpaceLuminance.xyz);
  vec2 tmpvar_6;
  tmpvar_6.x = (((
    (tmpvar_5.x + tmpvar_5.y)
   + tmpvar_5.z) + (
    (2.0 * sqrt((tmpvar_5.y * (tmpvar_5.x + tmpvar_5.z))))
   * unity_ColorSpaceLuminance.w)) - ((
    (tmpvar_2.x + tmpvar_2.y)
   + tmpvar_2.z) + (
    (2.0 * sqrt((tmpvar_2.y * (tmpvar_2.x + tmpvar_2.z))))
   * unity_ColorSpaceLuminance.w)));
  tmpvar_6.y = (((
    (tmpvar_4.x + tmpvar_4.y)
   + tmpvar_4.z) + (
    (2.0 * sqrt((tmpvar_4.y * (tmpvar_4.x + tmpvar_4.z))))
   * unity_ColorSpaceLuminance.w)) - ((
    (tmpvar_3.x + tmpvar_3.y)
   + tmpvar_3.z) + (
    (2.0 * sqrt((tmpvar_3.y * (tmpvar_3.x + tmpvar_3.z))))
   * unity_ColorSpaceLuminance.w)));
  float tmpvar_7;
  tmpvar_7 = sqrt(dot (tmpvar_6, tmpvar_6));
  if ((tmpvar_7 < 0.0625)) {
    outColor_1 = texture2D (_MainTex, xlv_TEXCOORD0_4);
  } else {
    vec2 tmpvar_8;
    tmpvar_8 = (tmpvar_6 * (_MainTex_TexelSize.xy / tmpvar_7));
    outColor_1 = (((
      ((texture2D (_MainTex, xlv_TEXCOORD0_4) + (texture2D (_MainTex, (xlv_TEXCOORD0_4 + 
        (tmpvar_8 * 0.5)
      )) * 0.9)) + (texture2D (_MainTex, (xlv_TEXCOORD0_4 - (tmpvar_8 * 0.5))) * 0.9))
     + 
      (texture2D (_MainTex, (xlv_TEXCOORD0_4 + tmpvar_8)) * 0.75)
    ) + (texture2D (_MainTex, 
      (xlv_TEXCOORD0_4 - tmpvar_8)
    ) * 0.75)) / 4.3);
  };
  gl_FragData[0] = outColor_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 12 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_2_0
def c5, 1.75, 0, 0, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.x, c5.x
mul r0.yz, r0.x, c4.xyxw
mov r0.xw, c5.y
add oT0.xy, -r0, v1
add oT1.xy, -r0.zwzw, v1
add oT2.xy, r0.zwzw, v1
add oT3.xy, r0, v1
mov oT4.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 9 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 112
Vector 96 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedbcbcdikeikndlpnppbolmlnpdkfjiefnabaaaaaaeiadaaaaadaaaaaa
cmaaaaaaiaaaaaaadiabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheolaaaaaaaagaaaaaaaiaaaaaajiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaakeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
keaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaakeaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaadamaaaakeaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaadamaaaakeaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefcaiacaaaa
eaaaabaaicaaaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafjaaaaaeegiocaaa
abaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaa
ghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaad
dccabaaaacaaaaaagfaaaaaddccabaaaadaaaaaagfaaaaaddccabaaaaeaaaaaa
gfaaaaaddccabaaaafaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadiaaaaalgcaabaaaaaaaaaaafgiecaaaaaaaaaaa
agaaaaaaaceaaaaaaaaaaaaaaaaaoadpaaaaoadpaaaaaaaadgaaaaaijcaabaaa
aaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaidccabaaa
abaaaaaaegaabaiaebaaaaaaaaaaaaaaegbabaaaabaaaaaaaaaaaaaidccabaaa
acaaaaaaogakbaiaebaaaaaaaaaaaaaaegbabaaaabaaaaaaaaaaaaahdccabaaa
adaaaaaaogakbaaaaaaaaaaaegbabaaaabaaaaaaaaaaaaahdccabaaaaeaaaaaa
egaabaaaaaaaaaaaegbabaaaabaaaaaadgaaaaafdccabaaaafaaaaaaegbabaaa
abaaaaaadoaaaaab""
}
SubProgram ""d3d11_9x "" {
// Stats: 9 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 112
Vector 96 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0_level_9_1
eefiecedagpmhbdifloabfljadfmejmbaeighgkoabaaaaaakmaeaaaaaeaaaaaa
daaaaaaajaabaaaakaadaaaapeadaaaaebgpgodjfiabaaaafiabaaaaaaacpopp
biabaaaaeaaaaaaaacaaceaaaaaadmaaaaaadmaaaaaaceaaabaadmaaaaaaagaa
abaaabaaaaaaaaaaabaaaaaaaeaaacaaaaaaaaaaaaaaaaaaaaacpoppfbaaaaaf
agaaapkaaaaaoadpaaaaaaaaaaaaaaaaaaaaaaaabpaaaaacafaaaaiaaaaaapja
bpaaaaacafaaabiaabaaapjaabaaaaacaaaaabiaagaaaakaafaaaaadaaaaagia
aaaaaaiaabaamekaabaaaaacaaaaajiaagaaffkaacaaaaadaaaaadoaaaaaoeib
abaaoejaacaaaaadabaaadoaaaaaooibabaaoejaacaaaaadacaaadoaaaaaooia
abaaoejaacaaaaadadaaadoaaaaaoeiaabaaoejaafaaaaadaaaaapiaaaaaffja
adaaoekaaeaaaaaeaaaaapiaacaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapia
aeaaoekaaaaakkjaaaaaoeiaaeaaaaaeaaaaapiaafaaoekaaaaappjaaaaaoeia
aeaaaaaeaaaaadmaaaaappiaaaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeia
abaaaaacaeaaadoaabaaoejappppaaaafdeieefcaiacaaaaeaaaabaaicaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
gfaaaaaddccabaaaadaaaaaagfaaaaaddccabaaaaeaaaaaagfaaaaaddccabaaa
afaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaa
egiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaa
aaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaa
aaaaaaaadiaaaaalgcaabaaaaaaaaaaafgiecaaaaaaaaaaaagaaaaaaaceaaaaa
aaaaaaaaaaaaoadpaaaaoadpaaaaaaaadgaaaaaijcaabaaaaaaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaidccabaaaabaaaaaaegaabaia
ebaaaaaaaaaaaaaaegbabaaaabaaaaaaaaaaaaaidccabaaaacaaaaaaogakbaia
ebaaaaaaaaaaaaaaegbabaaaabaaaaaaaaaaaaahdccabaaaadaaaaaaogakbaaa
aaaaaaaaegbabaaaabaaaaaaaaaaaaahdccabaaaaeaaaaaaegaabaaaaaaaaaaa
egbabaaaabaaaaaadgaaaaafdccabaaaafaaaaaaegbabaaaabaaaaaadoaaaaab
ejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
aaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadadaaaa
faepfdejfeejepeoaafeeffiedepepfceeaaklklepfdeheolaaaaaaaagaaaaaa
aiaaaaaajiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaakeaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaakeaaaaaaabaaaaaaaaaaaaaa
adaaaaaaacaaaaaaadamaaaakeaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaa
adamaaaakeaaaaaaadaaaaaaaaaaaaaaadaaaaaaaeaaaaaaadamaaaakeaaaaaa
aeaaaaaaaaaaaaaaadaaaaaaafaaaaaaadamaaaafdfgfpfaepfdejfeejepeoaa
feeffiedepepfceeaaklklkl""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 57 math, 9 textures
Vector 1 [_MainTex_TexelSize]
Vector 0 [unity_ColorSpaceLuminance]
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
def c2, 2, 0, -0.0625, 0.5
def c3, 0.899999976, 0.75, 0.232558131, 0
dcl t0.xy
dcl t1.xy
dcl t2.xy
dcl t3.xy
dcl t4.xy
dcl_2d s0
texld_pp r0, t0, s0
texld_pp r1, t3, s0
texld_pp r2, t1, s0
texld_pp r3, t2, s0
mul_pp r4.xyz, r0, c0
add_pp r1.w, r4.z, r4.x
mul_pp r1.w, r1.w, r4.y
add_pp r2.w, r4.y, r4.x
mad_pp r2.w, r0.z, c0.z, r2.w
rsq_pp r1.w, r1.w
rcp_pp r1.w, r1.w
mul_pp r1.w, r1.w, c0.w
mad_pp r1.w, r1.w, c2.x, r2.w
mul_pp r0.xyz, r1, c0
add_pp r2.w, r0.z, r0.x
mul_pp r2.w, r0.y, r2.w
add_pp r3.w, r0.y, r0.x
mad_pp r3.w, r1.z, c0.z, r3.w
rsq_pp r2.w, r2.w
rcp_pp r2.w, r2.w
mul_pp r2.w, r2.w, c0.w
mad_pp r2.w, r2.w, c2.x, r3.w
add_pp r2.w, r1.w, -r2.w
mov_pp r0.x, -r2.w
mul_pp r1.xyz, r2, c0
add_pp r3.w, r1.z, r1.x
mul_pp r3.w, r1.y, r3.w
add_pp r0.z, r1.y, r1.x
mad_pp r0.z, r2.z, c0.z, r0.z
rsq_pp r0.w, r3.w
rcp_pp r0.w, r0.w
mul_pp r0.w, r0.w, c0.w
mad_pp r3.w, r0.w, c2.x, r0.z
mul_pp r1.xyz, r3, c0
add_pp r0.z, r1.z, r1.x
mul_pp r0.z, r0.z, r1.y
add_pp r0.w, r1.y, r1.x
mad_pp r0.w, r3.z, c0.z, r0.w
rsq_pp r0.z, r0.z
rcp_pp r0.z, r0.z
mul_pp r0.z, r0.z, c0.w
mad_pp r0.z, r0.z, c2.x, r0.w
add_pp r0.y, -r3.w, r0.z
dp2add_pp r0.z, r0, r0, c2.y
rsq_pp r0.z, r0.z
mul r1.xy, r0.z, c1
rcp r0.z, r0.z
add r0.z, r0.z, c2.z
mad r2.xy, r0, r1, t4
mul_pp r1.zw, r0.wzyx, r1.wzyx
mad r0.xy, r0, -r1, t4
mad r1.xy, r1.wzyx, -c2.w, t4
mad r3.xy, r1.wzyx, c2.w, t4
texld r2, r2, s0
texld r4, r0, s0
texld r3, r3, s0
texld r1, r1, s0
texld_pp r5, t4, s0
mad_pp r3, r3, c3.x, r5
mad_pp r1, r1, c3.x, r3
mad_pp r1, r2, c3.y, r1
mad_pp r1, r4, c3.y, r1
mul_pp r1, r1, c3.z
cmp_pp r0, r0.z, r1, r5
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 45 math, 10 textures, 2 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 112
Vector 48 [unity_ColorSpaceLuminance]
Vector 96 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedgfikoiecnjaaajingcdeighjmkmmcagkabaaaaaanmaiaaaaadaaaaaa
cmaaaaaaoeaaaaaabiabaaaaejfdeheolaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaakeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaakeaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaakeaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaadadaaaakeaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaadadaaaakeaaaaaaaeaaaaaaaaaaaaaa
adaaaaaaafaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefclmahaaaa
eaaaaaaaopabaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gcbaaaaddcbabaaaacaaaaaagcbaaaaddcbabaaaadaaaaaagcbaaaaddcbabaaa
aeaaaaaagcbaaaaddcbabaaaafaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaac
afaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadiaaaaailcaabaaaaaaaaaaaegaibaaaaaaaaaaaegiicaaa
aaaaaaaaadaaaaaaaaaaaaahjcaabaaaaaaaaaaafganbaaaaaaaaaaaagaabaaa
aaaaaaaadcaaaaakbcaabaaaaaaaaaaackaabaaaaaaaaaaackiacaaaaaaaaaaa
adaaaaaaakaabaaaaaaaaaaadiaaaaahccaabaaaaaaaaaaadkaabaaaaaaaaaaa
bkaabaaaaaaaaaaaelaaaaafccaabaaaaaaaaaaabkaabaaaaaaaaaaaapaaaaai
ccaabaaaaaaaaaaapgipcaaaaaaaaaaaadaaaaaafgafbaaaaaaaaaaaaaaaaaah
bcaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaaaaaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaai
ocaabaaaaaaaaaaaagajbaaaabaaaaaaagijcaaaaaaaaaaaadaaaaaaaaaaaaah
kcaabaaaaaaaaaaakgaobaaaaaaaaaaafgafbaaaaaaaaaaadcaaaaakccaabaaa
aaaaaaaackaabaaaabaaaaaackiacaaaaaaaaaaaadaaaaaabkaabaaaaaaaaaaa
diaaaaahecaabaaaaaaaaaaadkaabaaaaaaaaaaackaabaaaaaaaaaaaelaaaaaf
ecaabaaaaaaaaaaackaabaaaaaaaaaaaapaaaaaiecaabaaaaaaaaaaapgipcaaa
aaaaaaaaadaaaaaakgakbaaaaaaaaaaaaaaaaaahccaabaaaaaaaaaaackaabaaa
aaaaaaaabkaabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaadaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaailcaabaaaabaaaaaaegaibaaa
abaaaaaaegiicaaaaaaaaaaaadaaaaaaaaaaaaahmcaabaaaaaaaaaaafganbaaa
abaaaaaaagaabaaaabaaaaaadcaaaaakecaabaaaaaaaaaaackaabaaaabaaaaaa
ckiacaaaaaaaaaaaadaaaaaackaabaaaaaaaaaaadiaaaaahicaabaaaaaaaaaaa
dkaabaaaaaaaaaaabkaabaaaabaaaaaaelaaaaaficaabaaaaaaaaaaadkaabaaa
aaaaaaaaapaaaaaiicaabaaaaaaaaaaapgipcaaaaaaaaaaaadaaaaaapgapbaaa
aaaaaaaaaaaaaaahecaabaaaaaaaaaaadkaabaaaaaaaaaaackaabaaaaaaaaaaa
efaaaaajpcaabaaaabaaaaaaegbabaaaaeaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadiaaaaailcaabaaaabaaaaaaegaibaaaabaaaaaaegiicaaaaaaaaaaa
adaaaaaaaaaaaaahjcaabaaaabaaaaaafganbaaaabaaaaaaagaabaaaabaaaaaa
dcaaaaakicaabaaaaaaaaaaackaabaaaabaaaaaackiacaaaaaaaaaaaadaaaaaa
akaabaaaabaaaaaadiaaaaahbcaabaaaabaaaaaadkaabaaaabaaaaaabkaabaaa
abaaaaaaelaaaaafbcaabaaaabaaaaaaakaabaaaabaaaaaaapaaaaaibcaabaaa
abaaaaaapgipcaaaaaaaaaaaadaaaaaaagaabaaaabaaaaaaaaaaaaahicaabaaa
aaaaaaaadkaabaaaaaaaaaaaakaabaaaabaaaaaaaaaaaaaibcaabaaaaaaaaaaa
dkaabaiaebaaaaaaaaaaaaaaakaabaaaaaaaaaaadgaaaaagbcaabaaaabaaaaaa
akaabaiaebaaaaaaaaaaaaaaaaaaaaaiccaabaaaabaaaaaabkaabaiaebaaaaaa
aaaaaaaackaabaaaaaaaaaaaapaaaaahbcaabaaaaaaaaaaaegaabaaaabaaaaaa
egaabaaaabaaaaaaelaaaaafbcaabaaaaaaaaaaaakaabaaaaaaaaaaadbaaaaah
ccaabaaaaaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaiadnaoaaaaaifcaabaaa
aaaaaaaaagibcaaaaaaaaaaaagaaaaaaagaabaaaaaaaaaaadiaaaaahmcaabaaa
abaaaaaaagaibaaaaaaaaaaaagaebaaaabaaaaaadcaaaaamdcaabaaaacaaaaaa
ogakbaaaabaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaaegbabaaa
afaaaaaaefaaaaajpcaabaaaacaaaaaaegaabaaaacaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadcaaaaanmcaabaaaabaaaaaakgaobaiaebaaaaaaabaaaaaa
aceaaaaaaaaaaaaaaaaaaaaaaaaaaadpaaaaaadpagbebaaaafaaaaaaefaaaaaj
pcaabaaaadaaaaaaogakbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
dcaaaaajmcaabaaaabaaaaaaagaebaaaabaaaaaaagaibaaaaaaaaaaaagbebaaa
afaaaaaaefaaaaajpcaabaaaaeaaaaaaogakbaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadcaaaaakfcaabaaaaaaaaaaaagabbaiaebaaaaaaabaaaaaa
agacbaaaaaaaaaaaagbbbaaaafaaaaaaefaaaaajpcaabaaaabaaaaaaigaabaaa
aaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaabpaaaeadbkaabaaaaaaaaaaa
efaaaaajpccabaaaaaaaaaaaegbabaaaafaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaabcaaaaabefaaaaajpcaabaaaaaaaaaaaegbabaaaafaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaadcaaaaampcaabaaaaaaaaaaaegaobaaaacaaaaaa
aceaaaaaggggggdpggggggdpggggggdpggggggdpegaobaaaaaaaaaaadcaaaaam
pcaabaaaaaaaaaaaegaobaaaadaaaaaaaceaaaaaggggggdpggggggdpggggggdp
ggggggdpegaobaaaaaaaaaaadcaaaaampcaabaaaaaaaaaaaegaobaaaaeaaaaaa
aceaaaaaaaaaeadpaaaaeadpaaaaeadpaaaaeadpegaobaaaaaaaaaaadcaaaaam
pcaabaaaaaaaaaaaegaobaaaabaaaaaaaceaaaaaaaaaeadpaaaaeadpaaaaeadp
aaaaeadpegaobaaaaaaaaaaadiaaaaakpccabaaaaaaaaaaaegaobaaaaaaaaaaa
aceaaaaalicdgodolicdgodolicdgodolicdgodobfaaaaabdoaaaaab""
}
SubProgram ""d3d11_9x "" {
// Stats: 21 math, 10 textures, 2 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 112
Vector 48 [unity_ColorSpaceLuminance]
Vector 96 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0_level_9_1
eefiecedafciodibkhljhilncailoohcghciohamabaaaaaabaajaaaaaeaaaaaa
daaaaaaacaadaaaaceaiaaaanmaiaaaaebgpgodjoiacaaaaoiacaaaaaaacpppp
kiacaaaaeaaaaaaaacaaciaaaaaaeaaaaaaaeaaaabaaceaaaaaaeaaaaaaaaaaa
aaaaadaaabaaaaaaaaaaaaaaaaaaagaaabaaabaaaaaaaaaaaaacppppfbaaaaaf
acaaapkaaaaaaaaaaaaaialnaaaaaadpggggggdpfbaaaaafadaaapkaaaaaeadp
licdgodoaaaaaaaaaaaaaaaabpaaaaacaaaaaaiaaaaaadlabpaaaaacaaaaaaia
abaaadlabpaaaaacaaaaaaiaacaaadlabpaaaaacaaaaaaiaadaaadlabpaaaaac
aaaaaaiaaeaaadlabpaaaaacaaaaaajaaaaiapkaecaaaaadaaaacpiaaaaaoela
aaaioekaecaaaaadabaacpiaadaaoelaaaaioekaecaaaaadacaacpiaabaaoela
aaaioekaecaaaaadadaacpiaacaaoelaaaaioekaaiaaaaadabaaciiaaaaaoeia
aaaaoekaaiaaaaadacaaciiaabaaoeiaaaaaoekaacaaaaadacaaciiaabaappia
acaappibabaaaaacaaaacbiaacaappibaiaaaaadadaaciiaacaaoeiaaaaaoeka
aiaaaaadaaaaceiaadaaoeiaaaaaoekaacaaaaadaaaacciaadaappibaaaakkia
fkaaaaaeaaaaceiaaaaaoeiaaaaaoeiaacaaaakaahaaaaacaaaaceiaaaaakkia
afaaaaadabaaadiaaaaakkiaabaaoekaagaaaaacaaaaaeiaaaaakkiaacaaaaad
aaaaaeiaaaaakkiaacaaffkaafaaaaadabaacmiaaaaabliaabaabliaaeaaaaae
acaaadiaabaabliaacaakkkbaeaaoelaaeaaaaaeadaaadiaabaabliaacaakkka
aeaaoelaaeaaaaaeaeaaadiaaaaaoeiaabaaoeiaaeaaoelaaeaaaaaeaaaaadia
aaaaoeiaabaaoeibaeaaoelaecaaaaadabaaapiaadaaoeiaaaaioekaecaaaaad
acaaapiaacaaoeiaaaaioekaecaaaaadadaacpiaaeaaoelaaaaioekaecaaaaad
afaaapiaaaaaoeiaaaaioekaecaaaaadaeaaapiaaeaaoeiaaaaioekaaeaaaaae
abaacpiaabaaoeiaacaappkaadaaoeiaaeaaaaaeabaacpiaacaaoeiaacaappka
abaaoeiaaeaaaaaeabaacpiaaeaaoeiaadaaaakaabaaoeiaaeaaaaaeabaacpia
afaaoeiaadaaaakaabaaoeiaafaaaaadabaacpiaabaaoeiaadaaffkafiaaaaae
aaaacpiaaaaakkiaabaaoeiaadaaoeiaabaaaaacaaaicpiaaaaaoeiappppaaaa
fdeieefcpmaeaaaaeaaaaaaadpabaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaaddcbabaaaadaaaaaa
gcbaaaaddcbabaaaaeaaaaaagcbaaaaddcbabaaaafaaaaaagfaaaaadpccabaaa
aaaaaaaagiaaaaacafaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaabaaaaaaibcaabaaaaaaaaaaaegacbaaa
aaaaaaaaegiccaaaaaaaaaaaadaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
acaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaabaaaaaaiccaabaaaaaaaaaaa
egacbaaaabaaaaaaegiccaaaaaaaaaaaadaaaaaaefaaaaajpcaabaaaabaaaaaa
egbabaaaadaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaabaaaaaaiecaabaaa
aaaaaaaaegacbaaaabaaaaaaegiccaaaaaaaaaaaadaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaaeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaabaaaaaai
icaabaaaaaaaaaaaegacbaaaabaaaaaaegiccaaaaaaaaaaaadaaaaaaaaaaaaai
bcaabaaaaaaaaaaadkaabaiaebaaaaaaaaaaaaaaakaabaaaaaaaaaaadgaaaaag
bcaabaaaabaaaaaaakaabaiaebaaaaaaaaaaaaaaaaaaaaaiccaabaaaabaaaaaa
bkaabaiaebaaaaaaaaaaaaaackaabaaaaaaaaaaaapaaaaahbcaabaaaaaaaaaaa
egaabaaaabaaaaaaegaabaaaabaaaaaaelaaaaafbcaabaaaaaaaaaaaakaabaaa
aaaaaaaadbaaaaahccaabaaaaaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaiadn
aoaaaaaifcaabaaaaaaaaaaaagibcaaaaaaaaaaaagaaaaaaagaabaaaaaaaaaaa
diaaaaahmcaabaaaabaaaaaaagaibaaaaaaaaaaaagaebaaaabaaaaaadcaaaaam
dcaabaaaacaaaaaaogakbaaaabaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaaaa
aaaaaaaaegbabaaaafaaaaaaefaaaaajpcaabaaaacaaaaaaegaabaaaacaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaanmcaabaaaabaaaaaakgaobaia
ebaaaaaaabaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaadpaaaaaadpagbebaaa
afaaaaaaefaaaaajpcaabaaaadaaaaaaogakbaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadcaaaaajmcaabaaaabaaaaaaagaebaaaabaaaaaaagaibaaa
aaaaaaaaagbebaaaafaaaaaaefaaaaajpcaabaaaaeaaaaaaogakbaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaakfcaabaaaaaaaaaaaagabbaia
ebaaaaaaabaaaaaaagacbaaaaaaaaaaaagbbbaaaafaaaaaaefaaaaajpcaabaaa
abaaaaaaigaabaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaabpaaaead
bkaabaaaaaaaaaaaefaaaaajpccabaaaaaaaaaaaegbabaaaafaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaabcaaaaabefaaaaajpcaabaaaaaaaaaaaegbabaaa
afaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaampcaabaaaaaaaaaaa
egaobaaaacaaaaaaaceaaaaaggggggdpggggggdpggggggdpggggggdpegaobaaa
aaaaaaaadcaaaaampcaabaaaaaaaaaaaegaobaaaadaaaaaaaceaaaaaggggggdp
ggggggdpggggggdpggggggdpegaobaaaaaaaaaaadcaaaaampcaabaaaaaaaaaaa
egaobaaaaeaaaaaaaceaaaaaaaaaeadpaaaaeadpaaaaeadpaaaaeadpegaobaaa
aaaaaaaadcaaaaampcaabaaaaaaaaaaaegaobaaaabaaaaaaaceaaaaaaaaaeadp
aaaaeadpaaaaeadpaaaaeadpegaobaaaaaaaaaaadiaaaaakpccabaaaaaaaaaaa
egaobaaaaaaaaaaaaceaaaaalicdgodolicdgodolicdgodolicdgodobfaaaaab
doaaaaabejfdeheolaaaaaaaagaaaaaaaiaaaaaajiaaaaaaaaaaaaaaabaaaaaa
adaaaaaaaaaaaaaaapaaaaaakeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaa
adadaaaakeaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadadaaaakeaaaaaa
acaaaaaaaaaaaaaaadaaaaaaadaaaaaaadadaaaakeaaaaaaadaaaaaaaaaaaaaa
adaaaaaaaeaaaaaaadadaaaakeaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaa
adadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklkl""
}
}
 }
}
Fallback Off
}";
	}
}
