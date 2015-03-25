using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PostProcessFX
{
	class DepthOfFieldEffect
	{
		public DepthOfFieldScatter dofComponent = null;

		private bool lastState = false;

		public DepthOfFieldEffect()
		{
			dofComponent = Camera.main.GetComponent<DepthOfFieldScatter>();
		}

		public void Enable()
		{
			if (dofComponent == null)
			{
				dofComponent = Camera.main.gameObject.AddComponent<DepthOfFieldScatter>();
				if (dofComponent == null)
				{
					Debug.LogError("DepthOfFieldEffect: Could not add the DepthOfField component to the main camera.");
				}
				else
				{
					Material bokehMaterial = new Material(dx11BokehShaderText);
					Material hdrMaterial = new Material(dofHdrShaderText);
					
					dofComponent.dofHdrShader = bokehMaterial.shader;
					dofComponent.dx11BokehShader = hdrMaterial.shader;
				}
			}

			if (!lastState)
			{
				lastState = true;
				dofComponent.enabled = true;
			}
		}

		public void Disable()
		{
			if (lastState)
			{
				lastState = false;
				dofComponent.enabled = false;
			}
		}

		public void Cleanup()
		{
			if (dofComponent != null)
			{
				MonoBehaviour.Destroy(dofComponent);
				dofComponent = null;
			}
		}

		private const String dofHdrShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 196.6KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/Dof/DepthOfFieldHdr"" {
Properties {
 _MainTex (""-"", 2D) = ""black"" { }
 _FgOverlap (""-"", 2D) = ""black"" { }
 _LowRez (""-"", 2D) = ""black"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 12 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 9 math, 1 texture
 //        d3d9 : 10 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  ColorMask A
  GpuProgramID 45374
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 12 math, 1 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform vec4 _ZBufferParams;
uniform sampler2D _CameraDepthTexture;
uniform vec4 _CurveParams;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec4 color_1;
  color_1.xyz = vec3(0.0, 0.0, 0.0);
  float tmpvar_2;
  tmpvar_2 = (1.0/(((_ZBufferParams.x * texture2D (_CameraDepthTexture, xlv_TEXCOORD1).x) + _ZBufferParams.y)));
  color_1.w = ((_CurveParams.z * abs(
    (tmpvar_2 - _CurveParams.w)
  )) / (tmpvar_2 + 1e-05));
  color_1.w = clamp (max (0.0, (color_1.w - _CurveParams.y)), 0.0, _CurveParams.x);
  gl_FragData[0] = color_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 10 math, 1 textures
Vector 1 [_CurveParams]
Vector 0 [_ZBufferParams]
SetTexture 0 [_CameraDepthTexture] 2D 0
""ps_3_0
def c2, 9.99999975e-006, 0, 0, 0
dcl_texcoord1 v0.xy
dcl_2d s0
texld r0, v0, s0
mad r0.x, c0.x, r0.x, c0.y
rcp r0.x, r0.x
add r0.y, r0.x, -c1.w
add r0.x, r0.x, c2.x
rcp r0.x, r0.x
mul r0.y, r0_abs.y, c1.z
mad r0.x, r0.y, r0.x, -c1.y
max r1.x, r0.x, c2.y
min oC0.w, c1.x, r1.x
mov oC0.xyz, c2.y

""
}
SubProgram ""d3d11 "" {
// Stats: 9 math, 1 textures
SetTexture 0 [_CameraDepthTexture] 2D 0
ConstBuffer ""$Globals"" 144
Vector 96 [_CurveParams]
ConstBuffer ""UnityPerCamera"" 144
Vector 112 [_ZBufferParams]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerCamera"" 1
""ps_4_0
eefiecedalgnkjlgfogkacbomenjhmeckoppgaiaabaaaaaaliacaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcoaabaaaaeaaaaaaahiaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafjaaaaaeegiocaaaabaaaaaaaiaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
mcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacabaaaaaaefaaaaaj
pcaabaaaaaaaaaaaogbkbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
dcaaaaalbcaabaaaaaaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaaaaaaaaa
bkiacaaaabaaaaaaahaaaaaaaoaaaaakbcaabaaaaaaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaiadpaaaaiadpakaabaaaaaaaaaaaaaaaaaajccaabaaaaaaaaaaa
akaabaaaaaaaaaaadkiacaiaebaaaaaaaaaaaaaaagaaaaaaaaaaaaahbcaabaaa
aaaaaaaaakaabaaaaaaaaaaaabeaaaaakmmfchdhdiaaaaajccaabaaaaaaaaaaa
bkaabaiaibaaaaaaaaaaaaaackiacaaaaaaaaaaaagaaaaaaaoaaaaahbcaabaaa
aaaaaaaabkaabaaaaaaaaaaaakaabaaaaaaaaaaaaaaaaaajbcaabaaaaaaaaaaa
akaabaaaaaaaaaaabkiacaiaebaaaaaaaaaaaaaaagaaaaaadeaaaaahbcaabaaa
aaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaaaaaddaaaaaiiccabaaaaaaaaaaa
akaabaaaaaaaaaaaakiacaaaaaaaaaaaagaaaaaadgaaaaaihccabaaaaaaaaaaa
aceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 11 math
 //        d3d9 : 14 math
 //      opengl : 43 math, 11 texture
 // Stats for Fragment shader:
 //       d3d11 : 33 math, 11 texture
 //        d3d9 : 34 math, 11 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 84697
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 43 math, 11 textures
""!!GLSL
#ifdef VERTEX

uniform vec4 _MainTex_TexelSize;
uniform vec4 _Offsets;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
varying vec4 xlv_TEXCOORD4;
varying vec4 xlv_TEXCOORD5;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD1 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(1.0, 1.0, -1.0, -1.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
  xlv_TEXCOORD2 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(2.0, 2.0, -2.0, -2.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
  xlv_TEXCOORD3 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(3.0, 3.0, -3.0, -3.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
  xlv_TEXCOORD4 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(4.0, 4.0, -4.0, -4.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
  xlv_TEXCOORD5 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(5.0, 5.0, -5.0, -5.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
varying vec4 xlv_TEXCOORD4;
varying vec4 xlv_TEXCOORD5;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD1.xy);
  vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD1.zw);
  vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD2.xy);
  vec4 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, xlv_TEXCOORD2.zw);
  vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, xlv_TEXCOORD3.xy);
  vec4 tmpvar_7;
  tmpvar_7 = texture2D (_MainTex, xlv_TEXCOORD3.zw);
  vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, xlv_TEXCOORD4.xy);
  vec4 tmpvar_9;
  tmpvar_9 = texture2D (_MainTex, xlv_TEXCOORD4.zw);
  vec4 tmpvar_10;
  tmpvar_10 = texture2D (_MainTex, xlv_TEXCOORD5.xy);
  vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, xlv_TEXCOORD5.zw);
  float tmpvar_12;
  tmpvar_12 = (tmpvar_2.w * 0.8);
  float tmpvar_13;
  tmpvar_13 = (tmpvar_3.w * 0.8);
  float tmpvar_14;
  tmpvar_14 = (tmpvar_4.w * 0.65);
  float tmpvar_15;
  tmpvar_15 = (tmpvar_5.w * 0.65);
  float tmpvar_16;
  tmpvar_16 = (tmpvar_6.w * 0.5);
  float tmpvar_17;
  tmpvar_17 = (tmpvar_7.w * 0.5);
  float tmpvar_18;
  tmpvar_18 = (tmpvar_8.w * 0.4);
  float tmpvar_19;
  tmpvar_19 = (tmpvar_9.w * 0.4);
  float tmpvar_20;
  tmpvar_20 = (tmpvar_10.w * 0.2);
  float tmpvar_21;
  tmpvar_21 = (tmpvar_11.w * 0.2);
  gl_FragData[0] = (((
    ((((
      ((((
        (tmpvar_1 * tmpvar_1.w)
       + 
        (tmpvar_2 * tmpvar_12)
      ) + (tmpvar_3 * tmpvar_13)) + (tmpvar_4 * tmpvar_14)) + (tmpvar_5 * tmpvar_15))
     + 
      (tmpvar_6 * tmpvar_16)
    ) + (tmpvar_7 * tmpvar_17)) + (tmpvar_8 * tmpvar_18)) + (tmpvar_9 * tmpvar_19))
   + 
    (tmpvar_10 * tmpvar_20)
  ) + (tmpvar_11 * tmpvar_21)) / ((
    ((((
      ((((
        (tmpvar_1.w + tmpvar_12)
       + tmpvar_13) + tmpvar_14) + tmpvar_15) + tmpvar_16)
     + tmpvar_17) + tmpvar_18) + tmpvar_19) + tmpvar_20)
   + tmpvar_21) + 0.0001));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 14 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
Vector 5 [_Offsets]
""vs_3_0
def c6, 0, 1, 0.166666672, -0.166666672
def c7, 0.333333343, -0.333333343, 0.5, -0.5
def c8, 0.666666687, -0.666666687, 0.833333373, -0.833333373
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2
dcl_texcoord2 o3
dcl_texcoord3 o4
dcl_texcoord4 o5
dcl_texcoord5 o6
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.xy, c6
mad r0, c4.xxxy, r0.xxyy, r0.yyxx
mul r0, r0, c5.xyxy
mul r0.xy, r0, c4
mad o2, r0, c6.zzww, v1.xyxy
mad o3, r0.zwzw, c7.xxyy, v1.xyxy
mad o4, r0.zwzw, c7.zzww, v1.xyxy
mad o5, r0.zwzw, c8.xxyy, v1.xyxy
mad o6, r0.zwzw, c8.zzww, v1.xyxy
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 11 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
Vector 128 [_Offsets]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedmiljhlgnmpdjagnaapgfpimdaneppfogabaaaaaaaeaeaaaaadaaaaaa
cmaaaaaaiaaaaaaafaabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheomiaaaaaaahaaaaaaaiaaaaaalaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaalmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
lmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapaaaaaalmaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaapaaaaaalmaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaapaaaaaalmaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaaapaaaaaa
lmaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaaapaaaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefckmacaaaaeaaaabaaklaaaaaa
fjaaaaaeegiocaaaaaaaaaaaajaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadpccabaaaacaaaaaa
gfaaaaadpccabaaaadaaaaaagfaaaaadpccabaaaaeaaaaaagfaaaaadpccabaaa
afaaaaaagfaaaaadpccabaaaagaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaa
abaaaaaadgaaaaafbcaabaaaaaaaaaaaabeaaaaaaaaaiadpdgaaaaagmcaabaaa
aaaaaaaaagiecaaaaaaaaaaaahaaaaaadiaaaaaipcaabaaaaaaaaaaaagaobaaa
aaaaaaaaegiecaaaaaaaaaaaaiaaaaaadiaaaaaidcaabaaaaaaaaaaaegaabaaa
aaaaaaaaegiacaaaaaaaaaaaahaaaaaadcaaaaampccabaaaacaaaaaaegaobaaa
aaaaaaaaaceaaaaaklkkckdoklkkckdoklkkckloklkkckloegbebaaaabaaaaaa
dcaaaaampccabaaaadaaaaaaogaobaaaaaaaaaaaaceaaaaaklkkkkdoklkkkkdo
klkkkkloklkkkkloegbebaaaabaaaaaadcaaaaampccabaaaaeaaaaaaogaobaaa
aaaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaalpaaaaaalpegbebaaaabaaaaaa
dcaaaaampccabaaaafaaaaaaogaobaaaaaaaaaaaaceaaaaaklkkckdpklkkckdp
klkkcklpklkkcklpegbebaaaabaaaaaadcaaaaampccabaaaagaaaaaaogaobaaa
aaaaaaaaaceaaaaafgffffdpfgffffdpfgfffflpfgfffflpegbebaaaabaaaaaa
doaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 34 math, 11 textures
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c0, 0.800000012, 0.649999976, 0.5, 0.400000006
def c1, 0.200000003, 9.99999975e-005, 0, 0
dcl_texcoord v0.xy
dcl_texcoord1 v1
dcl_texcoord2 v2
dcl_texcoord3 v3
dcl_texcoord4 v4
dcl_texcoord5 v5
dcl_2d s0
texld r0, v1, s0
mul r1.x, r0.w, c0.x
mul r1, r0, r1.x
texld r2, v0, s0
mad r1, r2, r2.w, r1
mad r0.x, r0.w, c0.x, r2.w
texld r2, v1.zwzw, s0
mul r0.y, r2.w, c0.x
mad r1, r2, r0.y, r1
mad r0.x, r2.w, c0.x, r0.x
texld r2, v2, s0
mul r0.y, r2.w, c0.y
mad r1, r2, r0.y, r1
mad r0.x, r2.w, c0.y, r0.x
texld r2, v2.zwzw, s0
mul r0.y, r2.w, c0.y
mad r1, r2, r0.y, r1
mad r0.x, r2.w, c0.y, r0.x
texld r2, v3, s0
mul r0.y, r2.w, c0.z
mad r1, r2, r0.y, r1
mad r0.x, r2.w, c0.z, r0.x
texld r2, v3.zwzw, s0
mul r0.y, r2.w, c0.z
mad r1, r2, r0.y, r1
mad r0.x, r2.w, c0.z, r0.x
texld r2, v4, s0
mul r0.y, r2.w, c0.w
mad r1, r2, r0.y, r1
mad r0.x, r2.w, c0.w, r0.x
texld r2, v4.zwzw, s0
mul r0.y, r2.w, c0.w
mad r1, r2, r0.y, r1
mad r0.x, r2.w, c0.w, r0.x
texld r2, v5, s0
mul r0.y, r2.w, c1.x
mad r1, r2, r0.y, r1
mad r0.x, r2.w, c1.x, r0.x
texld r2, v5.zwzw, s0
mul r0.y, r2.w, c1.x
mad r1, r2, r0.y, r1
mad r0.x, r2.w, c1.x, r0.x
add r0.x, r0.x, c1.y
rcp r0.x, r0.x
mul oC0, r0.x, r1

""
}
SubProgram ""d3d11 "" {
// Stats: 33 math, 11 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefieceddbccnmigeiaappjnpbhklcakcnlnknfjabaaaaaaieahaaaaadaaaaaa
cmaaaaaapmaaaaaadaabaaaaejfdeheomiaaaaaaahaaaaaaaiaaaaaalaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaalmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaalmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaalmaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaapapaaaalmaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapapaaaalmaaaaaaaeaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapapaaaalmaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaa
apapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcemagaaaaeaaaaaaajdabaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaagcbaaaadpcbabaaaadaaaaaa
gcbaaaadpcbabaaaaeaaaaaagcbaaaadpcbabaaaafaaaaaagcbaaaadpcbabaaa
agaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacadaaaaaaefaaaaajpcaabaaa
aaaaaaaaegbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaah
bcaabaaaabaaaaaadkaabaaaaaaaaaaaabeaaaaamnmmemdpdiaaaaahpcaabaaa
abaaaaaaegaobaaaaaaaaaaaagaabaaaabaaaaaaefaaaaajpcaabaaaacaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaajpcaabaaa
abaaaaaaegaobaaaacaaaaaapgapbaaaacaaaaaaegaobaaaabaaaaaadcaaaaaj
bcaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaamnmmemdpdkaabaaaacaaaaaa
efaaaaajpcaabaaaacaaaaaaogbkbaaaacaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadiaaaaahccaabaaaaaaaaaaadkaabaaaacaaaaaaabeaaaaamnmmemdp
dcaaaaajpcaabaaaabaaaaaaegaobaaaacaaaaaafgafbaaaaaaaaaaaegaobaaa
abaaaaaadcaaaaajbcaabaaaaaaaaaaadkaabaaaacaaaaaaabeaaaaamnmmemdp
akaabaaaaaaaaaaaefaaaaajpcaabaaaacaaaaaaegbabaaaadaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaadiaaaaahccaabaaaaaaaaaaadkaabaaaacaaaaaa
abeaaaaaggggcgdpdcaaaaajpcaabaaaabaaaaaaegaobaaaacaaaaaafgafbaaa
aaaaaaaaegaobaaaabaaaaaadcaaaaajbcaabaaaaaaaaaaadkaabaaaacaaaaaa
abeaaaaaggggcgdpakaabaaaaaaaaaaaefaaaaajpcaabaaaacaaaaaaogbkbaaa
adaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaahccaabaaaaaaaaaaa
dkaabaaaacaaaaaaabeaaaaaggggcgdpdcaaaaajpcaabaaaabaaaaaaegaobaaa
acaaaaaafgafbaaaaaaaaaaaegaobaaaabaaaaaadcaaaaajbcaabaaaaaaaaaaa
dkaabaaaacaaaaaaabeaaaaaggggcgdpakaabaaaaaaaaaaaefaaaaajpcaabaaa
acaaaaaaegbabaaaaeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaah
ccaabaaaaaaaaaaadkaabaaaacaaaaaaabeaaaaaaaaaaadpdcaaaaajpcaabaaa
abaaaaaaegaobaaaacaaaaaafgafbaaaaaaaaaaaegaobaaaabaaaaaadcaaaaaj
bcaabaaaaaaaaaaadkaabaaaacaaaaaaabeaaaaaaaaaaadpakaabaaaaaaaaaaa
efaaaaajpcaabaaaacaaaaaaogbkbaaaaeaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadiaaaaahccaabaaaaaaaaaaadkaabaaaacaaaaaaabeaaaaaaaaaaadp
dcaaaaajpcaabaaaabaaaaaaegaobaaaacaaaaaafgafbaaaaaaaaaaaegaobaaa
abaaaaaadcaaaaajbcaabaaaaaaaaaaadkaabaaaacaaaaaaabeaaaaaaaaaaadp
akaabaaaaaaaaaaaefaaaaajpcaabaaaacaaaaaaegbabaaaafaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaadiaaaaahccaabaaaaaaaaaaadkaabaaaacaaaaaa
abeaaaaamnmmmmdodcaaaaajpcaabaaaabaaaaaaegaobaaaacaaaaaafgafbaaa
aaaaaaaaegaobaaaabaaaaaadcaaaaajbcaabaaaaaaaaaaadkaabaaaacaaaaaa
abeaaaaamnmmmmdoakaabaaaaaaaaaaaefaaaaajpcaabaaaacaaaaaaogbkbaaa
afaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaahccaabaaaaaaaaaaa
dkaabaaaacaaaaaaabeaaaaamnmmmmdodcaaaaajpcaabaaaabaaaaaaegaobaaa
acaaaaaafgafbaaaaaaaaaaaegaobaaaabaaaaaadcaaaaajbcaabaaaaaaaaaaa
dkaabaaaacaaaaaaabeaaaaamnmmmmdoakaabaaaaaaaaaaaefaaaaajpcaabaaa
acaaaaaaegbabaaaagaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaah
ccaabaaaaaaaaaaadkaabaaaacaaaaaaabeaaaaamnmmemdodcaaaaajpcaabaaa
abaaaaaaegaobaaaacaaaaaafgafbaaaaaaaaaaaegaobaaaabaaaaaadcaaaaaj
bcaabaaaaaaaaaaadkaabaaaacaaaaaaabeaaaaamnmmemdoakaabaaaaaaaaaaa
efaaaaajpcaabaaaacaaaaaaogbkbaaaagaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadiaaaaahccaabaaaaaaaaaaadkaabaaaacaaaaaaabeaaaaamnmmemdo
dcaaaaajpcaabaaaabaaaaaaegaobaaaacaaaaaafgafbaaaaaaaaaaaegaobaaa
abaaaaaadcaaaaajbcaabaaaaaaaaaaadkaabaaaacaaaaaaabeaaaaamnmmemdo
akaabaaaaaaaaaaaaaaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaaabeaaaaa
bhlhnbdiaoaaaaahpccabaaaaaaaaaaaegaobaaaabaaaaaaagaabaaaaaaaaaaa
doaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 11 math
 //        d3d9 : 14 math
 //      opengl : 123 math, 11 texture
 // Stats for Fragment shader:
 //       d3d11 : 93 math, 11 texture
 //        d3d9 : 94 math, 11 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 190747
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 123 math, 11 textures
""!!GLSL
#ifdef VERTEX

uniform vec4 _MainTex_TexelSize;
uniform vec4 _Offsets;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
varying vec4 xlv_TEXCOORD4;
varying vec4 xlv_TEXCOORD5;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD1 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(1.0, 1.0, -1.0, -1.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
  xlv_TEXCOORD2 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(2.0, 2.0, -2.0, -2.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
  xlv_TEXCOORD3 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(3.0, 3.0, -3.0, -3.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
  xlv_TEXCOORD4 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(4.0, 4.0, -4.0, -4.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
  xlv_TEXCOORD5 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(5.0, 5.0, -5.0, -5.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
varying vec4 xlv_TEXCOORD4;
varying vec4 xlv_TEXCOORD5;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD1.xy);
  vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD1.zw);
  vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD2.xy);
  vec4 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, xlv_TEXCOORD2.zw);
  vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, xlv_TEXCOORD3.xy);
  vec4 tmpvar_7;
  tmpvar_7 = texture2D (_MainTex, xlv_TEXCOORD3.zw);
  vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, xlv_TEXCOORD4.xy);
  vec4 tmpvar_9;
  tmpvar_9 = texture2D (_MainTex, xlv_TEXCOORD4.zw);
  vec4 tmpvar_10;
  tmpvar_10 = texture2D (_MainTex, xlv_TEXCOORD5.xy);
  vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, xlv_TEXCOORD5.zw);
  float tmpvar_12;
  tmpvar_12 = clamp (((
    (tmpvar_2.w - tmpvar_1.w)
   - -0.5) / 0.5), 0.0, 1.0);
  float tmpvar_13;
  tmpvar_13 = ((tmpvar_12 * (tmpvar_12 * 
    (3.0 - (2.0 * tmpvar_12))
  )) * 0.8);
  float tmpvar_14;
  tmpvar_14 = clamp (((
    (tmpvar_3.w - tmpvar_1.w)
   - -0.5) / 0.5), 0.0, 1.0);
  float tmpvar_15;
  tmpvar_15 = ((tmpvar_14 * (tmpvar_14 * 
    (3.0 - (2.0 * tmpvar_14))
  )) * 0.8);
  float tmpvar_16;
  tmpvar_16 = clamp (((
    (tmpvar_4.w - tmpvar_1.w)
   - -0.5) / 0.5), 0.0, 1.0);
  float tmpvar_17;
  tmpvar_17 = ((tmpvar_16 * (tmpvar_16 * 
    (3.0 - (2.0 * tmpvar_16))
  )) * 0.675);
  float tmpvar_18;
  tmpvar_18 = clamp (((
    (tmpvar_5.w - tmpvar_1.w)
   - -0.5) / 0.5), 0.0, 1.0);
  float tmpvar_19;
  tmpvar_19 = ((tmpvar_18 * (tmpvar_18 * 
    (3.0 - (2.0 * tmpvar_18))
  )) * 0.675);
  float tmpvar_20;
  tmpvar_20 = clamp (((
    (tmpvar_6.w - tmpvar_1.w)
   - -0.5) / 0.5), 0.0, 1.0);
  float tmpvar_21;
  tmpvar_21 = ((tmpvar_20 * (tmpvar_20 * 
    (3.0 - (2.0 * tmpvar_20))
  )) * 0.5);
  float tmpvar_22;
  tmpvar_22 = clamp (((
    (tmpvar_7.w - tmpvar_1.w)
   - -0.5) / 0.5), 0.0, 1.0);
  float tmpvar_23;
  tmpvar_23 = ((tmpvar_22 * (tmpvar_22 * 
    (3.0 - (2.0 * tmpvar_22))
  )) * 0.5);
  float tmpvar_24;
  tmpvar_24 = clamp (((
    (tmpvar_8.w - tmpvar_1.w)
   - -0.5) / 0.5), 0.0, 1.0);
  float tmpvar_25;
  tmpvar_25 = ((tmpvar_24 * (tmpvar_24 * 
    (3.0 - (2.0 * tmpvar_24))
  )) * 0.2);
  float tmpvar_26;
  tmpvar_26 = clamp (((
    (tmpvar_9.w - tmpvar_1.w)
   - -0.5) / 0.5), 0.0, 1.0);
  float tmpvar_27;
  tmpvar_27 = ((tmpvar_26 * (tmpvar_26 * 
    (3.0 - (2.0 * tmpvar_26))
  )) * 0.2);
  float tmpvar_28;
  tmpvar_28 = clamp (((
    (tmpvar_10.w - tmpvar_1.w)
   - -0.5) / 0.5), 0.0, 1.0);
  float tmpvar_29;
  tmpvar_29 = ((tmpvar_28 * (tmpvar_28 * 
    (3.0 - (2.0 * tmpvar_28))
  )) * 0.075);
  float tmpvar_30;
  tmpvar_30 = clamp (((
    (tmpvar_11.w - tmpvar_1.w)
   - -0.5) / 0.5), 0.0, 1.0);
  float tmpvar_31;
  tmpvar_31 = ((tmpvar_30 * (tmpvar_30 * 
    (3.0 - (2.0 * tmpvar_30))
  )) * 0.075);
  gl_FragData[0] = (((
    ((((
      ((((
        (tmpvar_1 * tmpvar_1.w)
       + 
        (tmpvar_2 * tmpvar_13)
      ) + (tmpvar_3 * tmpvar_15)) + (tmpvar_4 * tmpvar_17)) + (tmpvar_5 * tmpvar_19))
     + 
      (tmpvar_6 * tmpvar_21)
    ) + (tmpvar_7 * tmpvar_23)) + (tmpvar_8 * tmpvar_25)) + (tmpvar_9 * tmpvar_27))
   + 
    (tmpvar_10 * tmpvar_29)
  ) + (tmpvar_11 * tmpvar_31)) / ((
    ((((
      ((((
        (tmpvar_1.w + tmpvar_13)
       + tmpvar_15) + tmpvar_17) + tmpvar_19) + tmpvar_21)
     + tmpvar_23) + tmpvar_25) + tmpvar_27) + tmpvar_29)
   + tmpvar_31) + 0.0001));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 14 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
Vector 5 [_Offsets]
""vs_3_0
def c6, 0, 1, 0.166666672, -0.166666672
def c7, 0.333333343, -0.333333343, 0.5, -0.5
def c8, 0.666666687, -0.666666687, 0.833333373, -0.833333373
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2
dcl_texcoord2 o3
dcl_texcoord3 o4
dcl_texcoord4 o5
dcl_texcoord5 o6
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.xy, c6
mad r0, c4.xxxy, r0.xxyy, r0.yyxx
mul r0, r0, c5.xyxy
mul r0.xy, r0, c4
mad o2, r0, c6.zzww, v1.xyxy
mad o3, r0.zwzw, c7.xxyy, v1.xyxy
mad o4, r0.zwzw, c7.zzww, v1.xyxy
mad o5, r0.zwzw, c8.xxyy, v1.xyxy
mad o6, r0.zwzw, c8.zzww, v1.xyxy
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 11 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
Vector 128 [_Offsets]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedmiljhlgnmpdjagnaapgfpimdaneppfogabaaaaaaaeaeaaaaadaaaaaa
cmaaaaaaiaaaaaaafaabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheomiaaaaaaahaaaaaaaiaaaaaalaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaalmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
lmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapaaaaaalmaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaapaaaaaalmaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaapaaaaaalmaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaaapaaaaaa
lmaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaaapaaaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefckmacaaaaeaaaabaaklaaaaaa
fjaaaaaeegiocaaaaaaaaaaaajaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadpccabaaaacaaaaaa
gfaaaaadpccabaaaadaaaaaagfaaaaadpccabaaaaeaaaaaagfaaaaadpccabaaa
afaaaaaagfaaaaadpccabaaaagaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaa
abaaaaaadgaaaaafbcaabaaaaaaaaaaaabeaaaaaaaaaiadpdgaaaaagmcaabaaa
aaaaaaaaagiecaaaaaaaaaaaahaaaaaadiaaaaaipcaabaaaaaaaaaaaagaobaaa
aaaaaaaaegiecaaaaaaaaaaaaiaaaaaadiaaaaaidcaabaaaaaaaaaaaegaabaaa
aaaaaaaaegiacaaaaaaaaaaaahaaaaaadcaaaaampccabaaaacaaaaaaegaobaaa
aaaaaaaaaceaaaaaklkkckdoklkkckdoklkkckloklkkckloegbebaaaabaaaaaa
dcaaaaampccabaaaadaaaaaaogaobaaaaaaaaaaaaceaaaaaklkkkkdoklkkkkdo
klkkkkloklkkkkloegbebaaaabaaaaaadcaaaaampccabaaaaeaaaaaaogaobaaa
aaaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaalpaaaaaalpegbebaaaabaaaaaa
dcaaaaampccabaaaafaaaaaaogaobaaaaaaaaaaaaceaaaaaklkkckdpklkkckdp
klkkcklpklkkcklpegbebaaaabaaaaaadcaaaaampccabaaaagaaaaaaogaobaaa
aaaaaaaaaceaaaaafgffffdpfgffffdpfgfffflpfgfffflpegbebaaaabaaaaaa
doaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 94 math, 11 textures
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c0, 0.5, -2, 3, 0.800000012
def c1, 0.675000012, 0.200000003, 0.075000003, 9.99999975e-005
dcl_texcoord v0.xy
dcl_texcoord1 v1
dcl_texcoord2 v2
dcl_texcoord3 v3
dcl_texcoord4 v4
dcl_texcoord5 v5
dcl_2d s0
texld r0, v1.zwzw, s0
texld r1, v1, s0
texld r2, v0, s0
add r3.x, r1.w, -r2.w
add r3.x, r3.x, c0.x
add_sat r3.x, r3.x, r3.x
mad r3.y, r3.x, c0.y, c0.z
mul r3.x, r3.x, r3.x
mul r3.x, r3.x, r3.y
mul r3.y, r3.x, c0.w
mad r3.x, r3.x, c0.w, r2.w
mul r1, r1, r3.y
mad r1, r2, r2.w, r1
add r2.x, r0.w, -r2.w
add r2.x, r2.x, c0.x
add_sat r2.x, r2.x, r2.x
mad r2.y, r2.x, c0.y, c0.z
mul r2.x, r2.x, r2.x
mul r2.x, r2.x, r2.y
mul r2.y, r2.x, c0.w
mad r2.x, r2.x, c0.w, r3.x
mad r0, r0, r2.y, r1
texld r1, v2, s0
add r2.y, -r2.w, r1.w
add r2.y, r2.y, c0.x
add_sat r2.y, r2.y, r2.y
mad r2.z, r2.y, c0.y, c0.z
mul r2.y, r2.y, r2.y
mul r2.y, r2.y, r2.z
mul r2.z, r2.y, c1.x
mad r2.x, r2.y, c1.x, r2.x
mad r0, r1, r2.z, r0
texld r1, v2.zwzw, s0
add r2.y, -r2.w, r1.w
add r2.y, r2.y, c0.x
add_sat r2.y, r2.y, r2.y
mad r2.z, r2.y, c0.y, c0.z
mul r2.y, r2.y, r2.y
mul r2.y, r2.y, r2.z
mul r2.z, r2.y, c1.x
mad r2.x, r2.y, c1.x, r2.x
mad r0, r1, r2.z, r0
texld r1, v3, s0
add r2.y, -r2.w, r1.w
add r2.y, r2.y, c0.x
add_sat r2.y, r2.y, r2.y
mad r2.z, r2.y, c0.y, c0.z
mul r2.y, r2.y, r2.y
mul r2.y, r2.y, r2.z
mul r2.z, r2.y, c0.x
mad r2.x, r2.y, c0.x, r2.x
mad r0, r1, r2.z, r0
texld r1, v3.zwzw, s0
add r2.y, -r2.w, r1.w
add r2.y, r2.y, c0.x
add_sat r2.y, r2.y, r2.y
mad r2.z, r2.y, c0.y, c0.z
mul r2.y, r2.y, r2.y
mul r2.y, r2.y, r2.z
mul r2.z, r2.y, c0.x
mad r2.x, r2.y, c0.x, r2.x
mad r0, r1, r2.z, r0
texld r1, v4, s0
add r2.y, -r2.w, r1.w
add r2.y, r2.y, c0.x
add_sat r2.y, r2.y, r2.y
mad r2.z, r2.y, c0.y, c0.z
mul r2.y, r2.y, r2.y
mul r2.y, r2.y, r2.z
mul r2.z, r2.y, c1.y
mad r2.x, r2.y, c1.y, r2.x
mad r0, r1, r2.z, r0
texld r1, v4.zwzw, s0
add r2.y, -r2.w, r1.w
add r2.y, r2.y, c0.x
add_sat r2.y, r2.y, r2.y
mad r2.z, r2.y, c0.y, c0.z
mul r2.y, r2.y, r2.y
mul r2.y, r2.y, r2.z
mul r2.z, r2.y, c1.y
mad r2.x, r2.y, c1.y, r2.x
mad r0, r1, r2.z, r0
texld r1, v5, s0
add r2.y, -r2.w, r1.w
add r2.y, r2.y, c0.x
add_sat r2.y, r2.y, r2.y
mad r2.z, r2.y, c0.y, c0.z
mul r2.y, r2.y, r2.y
mul r2.y, r2.y, r2.z
mul r2.z, r2.y, c1.z
mad r2.x, r2.y, c1.z, r2.x
mad r0, r1, r2.z, r0
texld r1, v5.zwzw, s0
add r2.y, -r2.w, r1.w
add r2.y, r2.y, c0.x
add_sat r2.y, r2.y, r2.y
mad r2.z, r2.y, c0.y, c0.z
mul r2.y, r2.y, r2.y
mul r2.y, r2.y, r2.z
mul r2.z, r2.y, c1.z
mad r2.x, r2.y, c1.z, r2.x
add r2.x, r2.x, c1.w
rcp r2.x, r2.x
mad r0, r1, r2.z, r0
mul oC0, r2.x, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 93 math, 11 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedbkgoolmhehnloelhhocfphaclggeeggnabaaaaaaimaoaaaaadaaaaaa
cmaaaaaapmaaaaaadaabaaaaejfdeheomiaaaaaaahaaaaaaaiaaaaaalaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaalmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaalmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaalmaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaapapaaaalmaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapapaaaalmaaaaaaaeaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapapaaaalmaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaa
apapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcfeanaaaaeaaaaaaaffadaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaagcbaaaadpcbabaaaadaaaaaa
gcbaaaadpcbabaaaaeaaaaaagcbaaaadpcbabaaaafaaaaaagcbaaaadpcbabaaa
agaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacaeaaaaaaefaaaaajpcaabaaa
aaaaaaaaogbkbaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaaj
pcaabaaaabaaaaaaegbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
efaaaaajpcaabaaaacaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaaaaaaaaibcaabaaaadaaaaaadkaabaaaabaaaaaadkaabaiaebaaaaaa
acaaaaaaaaaaaaahbcaabaaaadaaaaaaakaabaaaadaaaaaaabeaaaaaaaaaaadp
aacaaaahbcaabaaaadaaaaaaakaabaaaadaaaaaaakaabaaaadaaaaaadcaaaaaj
ccaabaaaadaaaaaaakaabaaaadaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaea
diaaaaahbcaabaaaadaaaaaaakaabaaaadaaaaaaakaabaaaadaaaaaadiaaaaah
bcaabaaaadaaaaaaakaabaaaadaaaaaabkaabaaaadaaaaaadiaaaaahccaabaaa
adaaaaaaakaabaaaadaaaaaaabeaaaaamnmmemdpdcaaaaajbcaabaaaadaaaaaa
akaabaaaadaaaaaaabeaaaaamnmmemdpdkaabaaaacaaaaaadiaaaaahpcaabaaa
abaaaaaaegaobaaaabaaaaaafgafbaaaadaaaaaadcaaaaajpcaabaaaabaaaaaa
egaobaaaacaaaaaapgapbaaaacaaaaaaegaobaaaabaaaaaaaaaaaaaibcaabaaa
acaaaaaadkaabaaaaaaaaaaadkaabaiaebaaaaaaacaaaaaaaaaaaaahbcaabaaa
acaaaaaaakaabaaaacaaaaaaabeaaaaaaaaaaadpaacaaaahbcaabaaaacaaaaaa
akaabaaaacaaaaaaakaabaaaacaaaaaadcaaaaajccaabaaaacaaaaaaakaabaaa
acaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaeadiaaaaahbcaabaaaacaaaaaa
akaabaaaacaaaaaaakaabaaaacaaaaaadiaaaaahbcaabaaaacaaaaaaakaabaaa
acaaaaaabkaabaaaacaaaaaadiaaaaahccaabaaaacaaaaaaakaabaaaacaaaaaa
abeaaaaamnmmemdpdcaaaaajbcaabaaaacaaaaaaakaabaaaacaaaaaaabeaaaaa
mnmmemdpakaabaaaadaaaaaadcaaaaajpcaabaaaaaaaaaaaegaobaaaaaaaaaaa
fgafbaaaacaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
adaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaaiccaabaaaacaaaaaa
dkaabaiaebaaaaaaacaaaaaadkaabaaaabaaaaaaaaaaaaahccaabaaaacaaaaaa
bkaabaaaacaaaaaaabeaaaaaaaaaaadpaacaaaahccaabaaaacaaaaaabkaabaaa
acaaaaaabkaabaaaacaaaaaadcaaaaajecaabaaaacaaaaaabkaabaaaacaaaaaa
abeaaaaaaaaaaamaabeaaaaaaaaaeaeadiaaaaahccaabaaaacaaaaaabkaabaaa
acaaaaaabkaabaaaacaaaaaadiaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaa
ckaabaaaacaaaaaadiaaaaahecaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaa
mnmmcmdpdcaaaaajbcaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaamnmmcmdp
akaabaaaacaaaaaadcaaaaajpcaabaaaaaaaaaaaegaobaaaabaaaaaakgakbaaa
acaaaaaaegaobaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaogbkbaaaadaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaaiccaabaaaacaaaaaadkaabaia
ebaaaaaaacaaaaaadkaabaaaabaaaaaaaaaaaaahccaabaaaacaaaaaabkaabaaa
acaaaaaaabeaaaaaaaaaaadpaacaaaahccaabaaaacaaaaaabkaabaaaacaaaaaa
bkaabaaaacaaaaaadcaaaaajecaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaa
aaaaaamaabeaaaaaaaaaeaeadiaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaa
bkaabaaaacaaaaaadiaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaackaabaaa
acaaaaaadiaaaaahecaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaamnmmcmdp
dcaaaaajbcaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaamnmmcmdpakaabaaa
acaaaaaadcaaaaajpcaabaaaaaaaaaaaegaobaaaabaaaaaakgakbaaaacaaaaaa
egaobaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaaeaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaaaaaaaaiccaabaaaacaaaaaadkaabaiaebaaaaaa
acaaaaaadkaabaaaabaaaaaaaaaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaa
abeaaaaaaaaaaadpaacaaaahccaabaaaacaaaaaabkaabaaaacaaaaaabkaabaaa
acaaaaaadcaaaaajecaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaaama
abeaaaaaaaaaeaeadiaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaabkaabaaa
acaaaaaadiaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaackaabaaaacaaaaaa
diaaaaahecaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaaadpdcaaaaaj
bcaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaaadpakaabaaaacaaaaaa
dcaaaaajpcaabaaaaaaaaaaaegaobaaaabaaaaaakgakbaaaacaaaaaaegaobaaa
aaaaaaaaefaaaaajpcaabaaaabaaaaaaogbkbaaaaeaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaaaaaaaaiccaabaaaacaaaaaadkaabaiaebaaaaaaacaaaaaa
dkaabaaaabaaaaaaaaaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaa
aaaaaadpaacaaaahccaabaaaacaaaaaabkaabaaaacaaaaaabkaabaaaacaaaaaa
dcaaaaajecaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaaamaabeaaaaa
aaaaeaeadiaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaabkaabaaaacaaaaaa
diaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaackaabaaaacaaaaaadiaaaaah
ecaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaaadpdcaaaaajbcaabaaa
acaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaaadpakaabaaaacaaaaaadcaaaaaj
pcaabaaaaaaaaaaaegaobaaaabaaaaaakgakbaaaacaaaaaaegaobaaaaaaaaaaa
efaaaaajpcaabaaaabaaaaaaegbabaaaafaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaaaaaaaaiccaabaaaacaaaaaadkaabaiaebaaaaaaacaaaaaadkaabaaa
abaaaaaaaaaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaaadp
aacaaaahccaabaaaacaaaaaabkaabaaaacaaaaaabkaabaaaacaaaaaadcaaaaaj
ecaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaea
diaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaabkaabaaaacaaaaaadiaaaaah
ccaabaaaacaaaaaabkaabaaaacaaaaaackaabaaaacaaaaaadiaaaaahecaabaaa
acaaaaaabkaabaaaacaaaaaaabeaaaaamnmmemdodcaaaaajbcaabaaaacaaaaaa
bkaabaaaacaaaaaaabeaaaaamnmmemdoakaabaaaacaaaaaadcaaaaajpcaabaaa
aaaaaaaaegaobaaaabaaaaaakgakbaaaacaaaaaaegaobaaaaaaaaaaaefaaaaaj
pcaabaaaabaaaaaaogbkbaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
aaaaaaaiccaabaaaacaaaaaadkaabaiaebaaaaaaacaaaaaadkaabaaaabaaaaaa
aaaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaaadpaacaaaah
ccaabaaaacaaaaaabkaabaaaacaaaaaabkaabaaaacaaaaaadcaaaaajecaabaaa
acaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaeadiaaaaah
ccaabaaaacaaaaaabkaabaaaacaaaaaabkaabaaaacaaaaaadiaaaaahccaabaaa
acaaaaaabkaabaaaacaaaaaackaabaaaacaaaaaadiaaaaahecaabaaaacaaaaaa
bkaabaaaacaaaaaaabeaaaaamnmmemdodcaaaaajbcaabaaaacaaaaaabkaabaaa
acaaaaaaabeaaaaamnmmemdoakaabaaaacaaaaaadcaaaaajpcaabaaaaaaaaaaa
egaobaaaabaaaaaakgakbaaaacaaaaaaegaobaaaaaaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaagaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaai
ccaabaaaacaaaaaadkaabaiaebaaaaaaacaaaaaadkaabaaaabaaaaaaaaaaaaah
ccaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaaadpaacaaaahccaabaaa
acaaaaaabkaabaaaacaaaaaabkaabaaaacaaaaaadcaaaaajecaabaaaacaaaaaa
bkaabaaaacaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaeadiaaaaahccaabaaa
acaaaaaabkaabaaaacaaaaaabkaabaaaacaaaaaadiaaaaahccaabaaaacaaaaaa
bkaabaaaacaaaaaackaabaaaacaaaaaadiaaaaahecaabaaaacaaaaaabkaabaaa
acaaaaaaabeaaaaajkjjjjdndcaaaaajbcaabaaaacaaaaaabkaabaaaacaaaaaa
abeaaaaajkjjjjdnakaabaaaacaaaaaadcaaaaajpcaabaaaaaaaaaaaegaobaaa
abaaaaaakgakbaaaacaaaaaaegaobaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaa
ogbkbaaaagaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaaiccaabaaa
acaaaaaadkaabaiaebaaaaaaacaaaaaadkaabaaaabaaaaaaaaaaaaahccaabaaa
acaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaaadpaacaaaahccaabaaaacaaaaaa
bkaabaaaacaaaaaabkaabaaaacaaaaaadcaaaaajecaabaaaacaaaaaabkaabaaa
acaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaeadiaaaaahccaabaaaacaaaaaa
bkaabaaaacaaaaaabkaabaaaacaaaaaadiaaaaahccaabaaaacaaaaaabkaabaaa
acaaaaaackaabaaaacaaaaaadiaaaaahecaabaaaacaaaaaabkaabaaaacaaaaaa
abeaaaaajkjjjjdndcaaaaajbcaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaa
jkjjjjdnakaabaaaacaaaaaaaaaaaaahbcaabaaaacaaaaaaakaabaaaacaaaaaa
abeaaaaabhlhnbdidcaaaaajpcaabaaaaaaaaaaaegaobaaaabaaaaaakgakbaaa
acaaaaaaegaobaaaaaaaaaaaaoaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaa
agaabaaaacaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 0 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 0 math, 1 texture
 //        d3d9 : 0 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Blend One One
 BlendOp Max
  ColorMask A
  GpuProgramID 206580
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 0 math, 1 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  gl_FragData[0] = texture2D (_MainTex, xlv_TEXCOORD1);
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: -1 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
dcl_texcoord1 v0.xy
dcl_2d s0
texld oC0, v0, s0

""
}
SubProgram ""d3d11 "" {
// Stats: 0 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedoaicnihpfmdhmdmlhpbdbnjidbbladpiabaaaaaadmabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcgeaaaaaaeaaaaaaabjaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
mcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaaefaaaaajpccabaaaaaaaaaaa
ogbkbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 11 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 9 math, 1 texture
 //        d3d9 : 10 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  ColorMask A
  GpuProgramID 302115
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 11 math, 1 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform vec4 _ZBufferParams;
uniform sampler2D _CameraDepthTexture;
uniform vec4 _CurveParams;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec4 color_1;
  color_1.xyz = vec3(0.0, 0.0, 0.0);
  float tmpvar_2;
  tmpvar_2 = (1.0/(((_ZBufferParams.x * texture2D (_CameraDepthTexture, xlv_TEXCOORD1).x) + _ZBufferParams.y)));
  color_1.w = ((_CurveParams.z * (_CurveParams.w - tmpvar_2)) / (tmpvar_2 + 1e-05));
  color_1.w = clamp (max (0.0, (color_1.w - _CurveParams.y)), 0.0, _CurveParams.x);
  gl_FragData[0] = color_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 10 math, 1 textures
Vector 1 [_CurveParams]
Vector 0 [_ZBufferParams]
SetTexture 0 [_CameraDepthTexture] 2D 0
""ps_3_0
def c2, 9.99999975e-006, 0, 0, 0
dcl_texcoord1 v0.xy
dcl_2d s0
texld r0, v0, s0
mad r0.x, c0.x, r0.x, c0.y
rcp r0.x, r0.x
add r0.y, -r0.x, c1.w
add r0.x, r0.x, c2.x
rcp r0.x, r0.x
mul r0.y, r0.y, c1.z
mad r0.x, r0.y, r0.x, -c1.y
max r1.x, r0.x, c2.y
min oC0.w, c1.x, r1.x
mov oC0.xyz, c2.y

""
}
SubProgram ""d3d11 "" {
// Stats: 9 math, 1 textures
SetTexture 0 [_CameraDepthTexture] 2D 0
ConstBuffer ""$Globals"" 144
Vector 96 [_CurveParams]
ConstBuffer ""UnityPerCamera"" 144
Vector 112 [_ZBufferParams]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerCamera"" 1
""ps_4_0
eefiecedegahieimjabhlicaeledmekndjhfipbcabaaaaaaleacaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcnmabaaaaeaaaaaaahhaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafjaaaaaeegiocaaaabaaaaaaaiaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
mcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacabaaaaaaefaaaaaj
pcaabaaaaaaaaaaaogbkbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
dcaaaaalbcaabaaaaaaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaaaaaaaaa
bkiacaaaabaaaaaaahaaaaaaaoaaaaakbcaabaaaaaaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaiadpaaaaiadpakaabaaaaaaaaaaaaaaaaaajccaabaaaaaaaaaaa
akaabaiaebaaaaaaaaaaaaaadkiacaaaaaaaaaaaagaaaaaaaaaaaaahbcaabaaa
aaaaaaaaakaabaaaaaaaaaaaabeaaaaakmmfchdhdiaaaaaiccaabaaaaaaaaaaa
bkaabaaaaaaaaaaackiacaaaaaaaaaaaagaaaaaaaoaaaaahbcaabaaaaaaaaaaa
bkaabaaaaaaaaaaaakaabaaaaaaaaaaaaaaaaaajbcaabaaaaaaaaaaaakaabaaa
aaaaaaaabkiacaiaebaaaaaaaaaaaaaaagaaaaaadeaaaaahbcaabaaaaaaaaaaa
akaabaaaaaaaaaaaabeaaaaaaaaaaaaaddaaaaaiiccabaaaaaaaaaaaakaabaaa
aaaaaaaaakiacaaaaaaaaaaaagaaaaaadgaaaaaihccabaaaaaaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 29 math, 3 texture, 2 branch
 // Stats for Fragment shader:
 //       d3d11 : 21 math, 3 texture, 1 branch
 //        d3d9 : 27 math, 3 texture, 5 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 346543
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 29 math, 3 textures, 2 branches
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
uniform vec4 _Offsets;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  vec4 steps_3;
  vec2 lenStep_4;
  vec4 sum_5;
  float sampleCount_6;
  vec4 tmpvar_7;
  tmpvar_7 = texture2D (_MainTex, xlv_TEXCOORD1);
  sampleCount_6 = tmpvar_7.w;
  sum_5 = (tmpvar_7 * tmpvar_7.w);
  vec2 tmpvar_8;
  tmpvar_8 = (tmpvar_7.ww * 0.09090909);
  lenStep_4 = tmpvar_8;
  steps_3 = (((_Offsets.xyxy * _MainTex_TexelSize.xyxy) * tmpvar_8.xyxy) * vec4(1.0, 1.0, -1.0, -1.0));
  for (int l_2 = 1; l_2 < 12; l_2++) {
    vec4 tmpvar_9;
    tmpvar_9 = (tmpvar_1.xyxy + (steps_3 * float(l_2)));
    vec4 tmpvar_10;
    tmpvar_10 = texture2D (_MainTex, tmpvar_9.xy);
    vec4 tmpvar_11;
    tmpvar_11 = texture2D (_MainTex, tmpvar_9.zw);
    vec2 tmpvar_12;
    tmpvar_12.x = tmpvar_10.w;
    tmpvar_12.y = tmpvar_11.w;
    vec2 tmpvar_13;
    vec2 tmpvar_14;
    tmpvar_14 = clamp (((
      (tmpvar_12 - (lenStep_4.xx * float(l_2)))
     - vec2(-0.4, -0.4)) / vec2(0.4, 0.4)), 0.0, 1.0);
    tmpvar_13 = (tmpvar_14 * (tmpvar_14 * (3.0 - 
      (2.0 * tmpvar_14)
    )));
    sum_5 = (sum_5 + ((tmpvar_10 * tmpvar_13.x) + (tmpvar_11 * tmpvar_13.y)));
    sampleCount_6 = (sampleCount_6 + dot (tmpvar_13, vec2(1.0, 1.0)));
  };
  gl_FragData[0] = (sum_5 / (1e-05 + sampleCount_6));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 27 math, 3 textures, 5 branches
Vector 0 [_MainTex_TexelSize]
Vector 1 [_Offsets]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c2, 0.0909090936, 1, -1, 0
def c3, 0.400000006, 2.5, -2, 3
def c4, 9.99999975e-006, 0, 0, 0
defi i0, 11, 0, 0, 0
dcl_texcoord1 v0.xy
dcl_2d s0
texld r0, v0, s0
mul r1, r0.w, r0
mul r0.x, r0.w, c2.x
mov r2.xy, c1
mul r2, r2.xyxy, c0.xyxy
mul r2, r0.x, r2
mul r2, r2, c2.yyzz
mov r3, r1
mov r0.y, r0.w
mov r0.z, c2.y
rep i0
mad r4, r2, r0.z, v0.xyxy
texld r5, r4, s0
texld r4, r4.zwzw, s0
mov r6.x, r5.w
mov r6.y, r4.w
mad r6.xy, r0.x, -r0.z, r6
add r6.xy, r6, c3.x
mul_sat r6.xy, r6, c3.y
mad r6.zw, r6.xyxy, c3.z, c3.w
mul r6.xy, r6, r6
mul r6.xy, r6, r6.zwzw
mul r4, r4, r6.y
mad r4, r5, r6.x, r4
add r3, r3, r4
dp2add r0.y, r6, c2.y, r0.y
add r0.z, r0.z, c2.y
endrep
add r0.x, r0.y, c4.x
rcp r0.x, r0.x
mul oC0, r0.x, r3

""
}
SubProgram ""d3d11 "" {
// Stats: 21 math, 3 textures, 1 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
Vector 128 [_Offsets]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedlefnjhhkphgeommpbbiaohddaljehmdhabaaaaaaoiaeaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcbaaeaaaaeaaaaaaaaeabaaaa
fjaaaaaeegiocaaaaaaaaaaaajaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaagcbaaaadmcbabaaaabaaaaaagfaaaaadpccabaaa
aaaaaaaagiaaaaacaiaaaaaaefaaaaajpcaabaaaaaaaaaaaogbkbaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaahpcaabaaaabaaaaaapgapbaaa
aaaaaaaaegaobaaaaaaaaaaadiaaaaahbcaabaaaaaaaaaaadkaabaaaaaaaaaaa
abeaaaaaimcolkdndiaaaaajpcaabaaaacaaaaaaegiecaaaaaaaaaaaahaaaaaa
egiecaaaaaaaaaaaaiaaaaaadiaaaaahpcaabaaaacaaaaaaagaabaaaaaaaaaaa
egaobaaaacaaaaaadiaaaaakpcaabaaaacaaaaaaegaobaaaacaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaialpaaaaialpdgaaaaafpcaabaaaadaaaaaaegaobaaa
abaaaaaadgaaaaafccaabaaaaaaaaaaadkaabaaaaaaaaaaadgaaaaafecaabaaa
aaaaaaaaabeaaaaaabaaaaaadaaaaaabcbaaaaahbcaabaaaaeaaaaaackaabaaa
aaaaaaaaabeaaaaaamaaaaaaadaaaeadakaabaaaaeaaaaaaclaaaaafbcaabaaa
aeaaaaaackaabaaaaaaaaaaadcaaaaajpcaabaaaafaaaaaaegaobaaaacaaaaaa
agaabaaaaeaaaaaaogbobaaaabaaaaaaefaaaaajpcaabaaaagaaaaaaegaabaaa
afaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaafaaaaaa
ogakbaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadgaaaaafbcaabaaa
ahaaaaaadkaabaaaagaaaaaadgaaaaafccaabaaaahaaaaaadkaabaaaafaaaaaa
dcaaaaakdcaabaaaaeaaaaaaagaabaiaebaaaaaaaaaaaaaaagaabaaaaeaaaaaa
egaabaaaahaaaaaaaaaaaaakdcaabaaaaeaaaaaaegaabaaaaeaaaaaaaceaaaaa
mnmmmmdomnmmmmdoaaaaaaaaaaaaaaaadicaaaakdcaabaaaaeaaaaaaegaabaaa
aeaaaaaaaceaaaaaaaaacaeaaaaacaeaaaaaaaaaaaaaaaaadcaaaaapmcaabaaa
aeaaaaaaagaebaaaaeaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaamaaaaaaama
aceaaaaaaaaaaaaaaaaaaaaaaaaaeaeaaaaaeaeadiaaaaahdcaabaaaaeaaaaaa
egaabaaaaeaaaaaaegaabaaaaeaaaaaadiaaaaahdcaabaaaaeaaaaaaegaabaaa
aeaaaaaaogakbaaaaeaaaaaadiaaaaahpcaabaaaafaaaaaafgafbaaaaeaaaaaa
egaobaaaafaaaaaadcaaaaajpcaabaaaafaaaaaaegaobaaaagaaaaaaagaabaaa
aeaaaaaaegaobaaaafaaaaaaaaaaaaahpcaabaaaadaaaaaaegaobaaaadaaaaaa
egaobaaaafaaaaaaapaaaaakbcaabaaaaeaaaaaaegaabaaaaeaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaaaaaaaaaaaaaaaaaaaahccaabaaaaaaaaaaabkaabaaa
aaaaaaaaakaabaaaaeaaaaaaboaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaa
abeaaaaaabaaaaaabgaaaaabaaaaaaahbcaabaaaaaaaaaaabkaabaaaaaaaaaaa
abeaaaaakmmfchdhaoaaaaahpccabaaaaaaaaaaaegaobaaaadaaaaaaagaabaaa
aaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 21 math, 5 texture, 1 branch
 // Stats for Fragment shader:
 //       d3d11 : 11 math, 5 texture
 //        d3d9 : 18 math, 5 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 410550
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 21 math, 5 textures, 1 branches
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 outColor_1;
  vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  vec4 tmpvar_3;
  vec2 cse_4;
  cse_4 = (0.75 * _MainTex_TexelSize.xy);
  tmpvar_3 = texture2D (_MainTex, (xlv_TEXCOORD0 + cse_4));
  vec4 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, (xlv_TEXCOORD0 - cse_4));
  vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, (xlv_TEXCOORD0 + (cse_4 * vec2(1.0, -1.0))));
  vec4 tmpvar_7;
  tmpvar_7 = texture2D (_MainTex, (xlv_TEXCOORD0 - (cse_4 * vec2(1.0, -1.0))));
  vec4 tmpvar_8;
  tmpvar_8.x = tmpvar_3.w;
  tmpvar_8.y = tmpvar_5.w;
  tmpvar_8.z = tmpvar_6.w;
  tmpvar_8.w = tmpvar_7.w;
  vec4 tmpvar_9;
  tmpvar_9 = clamp ((10.0 * tmpvar_8), 0.0, 1.0);
  float tmpvar_10;
  tmpvar_10 = dot (tmpvar_9, vec4(1.0, 1.0, 1.0, 1.0));
  vec4 tmpvar_11;
  tmpvar_11 = (((
    (tmpvar_3 * tmpvar_9.x)
   + 
    (tmpvar_5 * tmpvar_9.y)
  ) + (tmpvar_6 * tmpvar_9.z)) + (tmpvar_7 * tmpvar_9.w));
  outColor_1 = tmpvar_2;
  if ((((tmpvar_2.w * tmpvar_10) * 8.0) > 1e-05)) {
    outColor_1.xyz = (tmpvar_11.xyz / tmpvar_10);
  };
  gl_FragData[0] = outColor_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 18 math, 5 textures
Vector 0 [_MainTex_TexelSize]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c1, 1.24999997e-006, 0, 0, 0
def c2, 0.75, -0.75, 10, 1
dcl_texcoord v0.xy
dcl_2d s0
mov r0.xy, c2
mad r1, c0.xyxy, r0.xxxy, v0.xyxy
texld r2, r1, s0
texld r1, r1.zwzw, s0
mov r3.x, r2.w
mad r0, c0.xyxy, -r0.xxxy, v0.xyxy
texld r4, r0, s0
texld r0, r0.zwzw, s0
mov r3.y, r4.w
mov r3.z, r1.w
mov r3.w, r0.w
mul_sat r3, r3, c2.z
mul r4.xyz, r3.y, r4
mad r2.xyz, r2, r3.x, r4
mad r1.xyz, r1, r3.z, r2
mad r0.xyz, r0, r3.w, r1
dp4 r0.w, r3, c2.w
rcp r1.x, r0.w
mul r0.xyz, r0, r1.x
texld r1, v0, s0
mad r0.w, r1.w, -r0.w, c1.x
cmp oC0.xyz, r0.w, r1, r0
mov oC0.w, r1.w

""
}
SubProgram ""d3d11 "" {
// Stats: 11 math, 5 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedokhmgcgipbbfeffjpbgfmpjogigkkpefabaaaaaaaeaeaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefccmadaaaaeaaaaaaamlaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagfaaaaadpccabaaa
aaaaaaaagiaaaaacafaaaaaadcaaaaanpcaabaaaaaaaaaaaegiecaaaaaaaaaaa
ahaaaaaaaceaaaaaaaaaeadpaaaaeadpaaaaeadpaaaaealpegbebaaaabaaaaaa
efaaaaajpcaabaaaabaaaaaaegaabaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaefaaaaajpcaabaaaaaaaaaaaogakbaaaaaaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadgaaaaafbcaabaaaacaaaaaadkaabaaaabaaaaaadcaaaaao
pcaabaaaadaaaaaaegiecaiaebaaaaaaaaaaaaaaahaaaaaaaceaaaaaaaaaeadp
aaaaeadpaaaaeadpaaaaealpegbebaaaabaaaaaaefaaaaajpcaabaaaaeaaaaaa
egaabaaaadaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
adaaaaaaogakbaaaadaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadgaaaaaf
ccaabaaaacaaaaaadkaabaaaaeaaaaaadgaaaaafecaabaaaacaaaaaadkaabaaa
aaaaaaaadgaaaaaficaabaaaacaaaaaadkaabaaaadaaaaaadicaaaakpcaabaaa
acaaaaaaegaobaaaacaaaaaaaceaaaaaaaaacaebaaaacaebaaaacaebaaaacaeb
diaaaaahhcaabaaaaeaaaaaafgafbaaaacaaaaaaegacbaaaaeaaaaaadcaaaaaj
hcaabaaaabaaaaaaegacbaaaabaaaaaaagaabaaaacaaaaaaegacbaaaaeaaaaaa
dcaaaaajhcaabaaaaaaaaaaaegacbaaaaaaaaaaakgakbaaaacaaaaaaegacbaaa
abaaaaaadcaaaaajhcaabaaaaaaaaaaaegacbaaaadaaaaaapgapbaaaacaaaaaa
egacbaaaaaaaaaaabbaaaaakicaabaaaaaaaaaaaegaobaaaacaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaiadpaaaaiadpaoaaaaahhcaabaaaaaaaaaaaegacbaaa
aaaaaaaapgapbaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaahicaabaaaaaaaaaaadkaabaaa
aaaaaaaadkaabaaaabaaaaaadbaaaaahicaabaaaaaaaaaaaabeaaaaakmmfkhdf
dkaabaaaaaaaaaaadhaaaaajhccabaaaaaaaaaaapgapbaaaaaaaaaaaegacbaaa
aaaaaaaaegacbaaaabaaaaaadgaaaaaficcabaaaaaaaaaaadkaabaaaabaaaaaa
doaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 3 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 2 math, 1 texture
 //        d3d9 : 3 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  GpuProgramID 476614
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 3 math, 1 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  vec4 tmpvar_2;
  tmpvar_2.xyz = tmpvar_1.xyz;
  tmpvar_2.w = (1.0 - clamp ((tmpvar_1.w * 5.0), 0.0, 1.0));
  gl_FragData[0] = tmpvar_2;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 3 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c0, 5, 1, 0, 0
dcl_texcoord v0.xy
dcl_2d s0
texld r0, v0, s0
mul_sat r0.w, r0.w, c0.x
mov oC0.xyz, r0
add oC0.w, -r0.w, c0.y

""
}
SubProgram ""d3d11 "" {
// Stats: 2 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedkinhfhlbbodafpmbjoidnfegldffalbiabaaaaaajeabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefclmaaaaaaeaaaaaaacpaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacabaaaaaaefaaaaaj
pcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
dicaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaakaeadgaaaaaf
hccabaaaaaaaaaaaegacbaaaaaaaaaaaaaaaaaaiiccabaaaaaaaaaaadkaabaia
ebaaaaaaaaaaaaaaabeaaaaaaaaaiadpdoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 13 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 11 math, 1 texture
 //        d3d9 : 10 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  ColorMask A
  GpuProgramID 585908
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 13 math, 1 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform vec4 _ZBufferParams;
uniform sampler2D _CameraDepthTexture;
uniform vec4 _CurveParams;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec4 color_1;
  color_1.xyz = vec3(0.0, 0.0, 0.0);
  float tmpvar_2;
  tmpvar_2 = (1.0/(((_ZBufferParams.x * texture2D (_CameraDepthTexture, xlv_TEXCOORD1).x) + _ZBufferParams.y)));
  color_1.w = ((_CurveParams.z * (_CurveParams.w - tmpvar_2)) / (tmpvar_2 + 1e-05));
  color_1.w = clamp (max (0.0, (color_1.w - _CurveParams.y)), 0.0, _CurveParams.x);
  gl_FragData[0] = vec4(float((color_1.w > 0.0)));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 10 math, 1 textures
Vector 1 [_CurveParams]
Vector 0 [_ZBufferParams]
SetTexture 0 [_CameraDepthTexture] 2D 0
""ps_3_0
def c2, 9.99999975e-006, 0, 1, 0
dcl_texcoord1 v0.xy
dcl_2d s0
texld r0, v0, s0
mad r0.x, c0.x, r0.x, c0.y
rcp r0.x, r0.x
add r0.y, -r0.x, c1.w
add r0.x, r0.x, c2.x
rcp r0.x, r0.x
mul r0.y, r0.y, c1.z
mad r0.x, r0.y, r0.x, -c1.y
max r1.x, r0.x, c2.y
min r0.x, c1.x, r1.x
cmp oC0, -r0.x, c2.y, c2.z

""
}
SubProgram ""d3d11 "" {
// Stats: 11 math, 1 textures
SetTexture 0 [_CameraDepthTexture] 2D 0
ConstBuffer ""$Globals"" 144
Vector 96 [_CurveParams]
ConstBuffer ""UnityPerCamera"" 144
Vector 112 [_ZBufferParams]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerCamera"" 1
""ps_4_0
eefiecedldaekheaaglogcppneojgljdcfhameegabaaaaaaniacaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcaaacaaaaeaaaaaaaiaaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafjaaaaaeegiocaaaabaaaaaaaiaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
mcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacabaaaaaaefaaaaaj
pcaabaaaaaaaaaaaogbkbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
dcaaaaalbcaabaaaaaaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaaaaaaaaa
bkiacaaaabaaaaaaahaaaaaaaoaaaaakbcaabaaaaaaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaiadpaaaaiadpakaabaaaaaaaaaaaaaaaaaajccaabaaaaaaaaaaa
akaabaiaebaaaaaaaaaaaaaadkiacaaaaaaaaaaaagaaaaaaaaaaaaahbcaabaaa
aaaaaaaaakaabaaaaaaaaaaaabeaaaaakmmfchdhdiaaaaaiccaabaaaaaaaaaaa
bkaabaaaaaaaaaaackiacaaaaaaaaaaaagaaaaaaaoaaaaahbcaabaaaaaaaaaaa
bkaabaaaaaaaaaaaakaabaaaaaaaaaaaaaaaaaajbcaabaaaaaaaaaaaakaabaaa
aaaaaaaabkiacaiaebaaaaaaaaaaaaaaagaaaaaadeaaaaahbcaabaaaaaaaaaaa
akaabaaaaaaaaaaaabeaaaaaaaaaaaaaddaaaaaibcaabaaaaaaaaaaaakaabaaa
aaaaaaaaakiacaaaaaaaaaaaagaaaaaadbaaaaahbcaabaaaaaaaaaaaabeaaaaa
aaaaaaaaakaabaaaaaaaaaaaabaaaaakpccabaaaaaaaaaaaagaabaaaaaaaaaaa
aceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpdoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 4 math, 3 texture
 // Stats for Fragment shader:
 //       d3d11 : 4 math, 3 texture
 //        d3d9 : 5 math, 3 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 632841
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 4 math, 3 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform sampler2D _FgOverlap;
uniform sampler2D _LowRez;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD1);
  vec4 tmpvar_2;
  tmpvar_2.xyz = mix (tmpvar_1, texture2D (_LowRez, xlv_TEXCOORD1), vec4(clamp ((
    max (tmpvar_1.w, texture2D (_FgOverlap, xlv_TEXCOORD1).w)
   * 8.0), 0.0, 1.0))).xyz;
  tmpvar_2.w = tmpvar_1.w;
  gl_FragData[0] = tmpvar_2;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 5 math, 3 textures
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_FgOverlap] 2D 1
SetTexture 2 [_LowRez] 2D 2
""ps_3_0
def c0, 8, 0, 0, 0
dcl_texcoord1 v0.xy
dcl_2d s0
dcl_2d s1
dcl_2d s2
texld r0, v0, s1
texld r1, v0, s0
max r2.x, r1.w, r0.w
mul_sat r0.x, r2.x, c0.x
texld r2, v0, s2
add r0.yzw, -r1.xxyz, r2.xxyz
mad oC0.xyz, r0.x, r0.yzww, r1
mov oC0.w, r1.w

""
}
SubProgram ""d3d11 "" {
// Stats: 4 math, 3 textures
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_LowRez] 2D 2
SetTexture 2 [_FgOverlap] 2D 1
""ps_4_0
eefiecedmakoefomghlohjgpnppcnmdfcamfljcgabaaaaaafeacaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefchmabaaaaeaaaaaaafpaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafkaaaaadaagabaaa
acaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaa
ffffaaaafibiaaaeaahabaaaacaaaaaaffffaaaagcbaaaadmcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacadaaaaaaefaaaaajpcaabaaaaaaaaaaa
ogbkbaaaabaaaaaaeghobaaaacaaaaaaaagabaaaabaaaaaaefaaaaajpcaabaaa
abaaaaaaogbkbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadeaaaaah
bcaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaaabaaaaaadicaaaahbcaabaaa
aaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaaaebefaaaaajpcaabaaaacaaaaaa
ogbkbaaaabaaaaaaeghobaaaabaaaaaaaagabaaaacaaaaaaaaaaaaaiocaabaaa
aaaaaaaaagajbaiaebaaaaaaabaaaaaaagajbaaaacaaaaaadcaaaaajhccabaaa
aaaaaaaaagaabaaaaaaaaaaajgahbaaaaaaaaaaaegacbaaaabaaaaaadgaaaaaf
iccabaaaaaaaaaaadkaabaaaabaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 14 math, 2 texture
 // Stats for Fragment shader:
 //       d3d11 : 13 math, 2 texture
 //        d3d9 : 14 math, 2 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 682188
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 14 math, 2 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform vec4 _CurveParams;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec4 color_1;
  color_1.xyz = texture2D (_MainTex, xlv_TEXCOORD1).xyz;
  float tmpvar_2;
  tmpvar_2 = (1.0/(((_ZBufferParams.x * texture2D (_CameraDepthTexture, xlv_TEXCOORD1).x) + _ZBufferParams.y)));
  color_1.w = ((_CurveParams.z * abs(
    (tmpvar_2 - _CurveParams.w)
  )) / (tmpvar_2 + 1e-05));
  color_1.w = (clamp (max (0.0, 
    (color_1.w - _CurveParams.y)
  ), 0.0, _CurveParams.x) * sign((tmpvar_2 - _CurveParams.w)));
  gl_FragData[0] = color_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 14 math, 2 textures
Vector 1 [_CurveParams]
Vector 0 [_ZBufferParams]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_CameraDepthTexture] 2D 1
""ps_3_0
def c2, 9.99999975e-006, 0, 1, 0
dcl_texcoord1 v0.xy
dcl_2d s0
dcl_2d s1
texld r0, v0, s1
mad r0.x, c0.x, r0.x, c0.y
rcp r0.x, r0.x
add r0.y, r0.x, c2.x
add r0.x, r0.x, -c1.w
rcp r0.y, r0.y
mul r0.z, r0_abs.x, c1.z
mad r0.y, r0.z, r0.y, -c1.y
max r1.x, r0.y, c2.y
min r0.y, c1.x, r1.x
cmp r0.z, -r0.x, c2.y, c2.z
cmp r0.x, r0.x, -c2.y, -c2.z
add r0.x, r0.x, r0.z
mul oC0.w, r0.x, r0.y
texld r0, v0, s0
mov oC0.xyz, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 13 math, 2 textures
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_CameraDepthTexture] 2D 1
ConstBuffer ""$Globals"" 144
Vector 96 [_CurveParams]
ConstBuffer ""UnityPerCamera"" 144
Vector 112 [_ZBufferParams]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerCamera"" 1
""ps_4_0
eefiecedbafchdpmfiebnbefceofljokaaojeakmabaaaaaaheadaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcjmacaaaaeaaaaaaakhaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafjaaaaaeegiocaaaabaaaaaaaiaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaadmcbabaaa
abaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacabaaaaaaefaaaaajpcaabaaa
aaaaaaaaogbkbaaaabaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaadcaaaaal
bcaabaaaaaaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaaaaaaaaabkiacaaa
abaaaaaaahaaaaaaaoaaaaakbcaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaiadpaaaaiadpakaabaaaaaaaaaaaaaaaaaahccaabaaaaaaaaaaaakaabaaa
aaaaaaaaabeaaaaakmmfchdhaaaaaaajbcaabaaaaaaaaaaaakaabaaaaaaaaaaa
dkiacaiaebaaaaaaaaaaaaaaagaaaaaadiaaaaajecaabaaaaaaaaaaaakaabaia
ibaaaaaaaaaaaaaackiacaaaaaaaaaaaagaaaaaaaoaaaaahccaabaaaaaaaaaaa
ckaabaaaaaaaaaaabkaabaaaaaaaaaaaaaaaaaajccaabaaaaaaaaaaabkaabaaa
aaaaaaaabkiacaiaebaaaaaaaaaaaaaaagaaaaaadeaaaaahccaabaaaaaaaaaaa
bkaabaaaaaaaaaaaabeaaaaaaaaaaaaaddaaaaaiccaabaaaaaaaaaaabkaabaaa
aaaaaaaaakiacaaaaaaaaaaaagaaaaaadbaaaaahecaabaaaaaaaaaaaabeaaaaa
aaaaaaaaakaabaaaaaaaaaaadbaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaa
abeaaaaaaaaaaaaaboaaaaaibcaabaaaaaaaaaaackaabaiaebaaaaaaaaaaaaaa
akaabaaaaaaaaaaaclaaaaafbcaabaaaaaaaaaaaakaabaaaaaaaaaaadiaaaaah
iccabaaaaaaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaaefaaaaajpcaabaaa
aaaaaaaaogbkbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadgaaaaaf
hccabaaaaaaaaaaaegacbaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 52 math, 2 texture, 3 branch
 // Stats for Fragment shader:
 //       d3d11 : 18 math, 2 texture, 2 branch
 //        d3d9 : 274 math, 29 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 722847
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 52 math, 2 textures, 3 branches
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
uniform vec4 _Offsets;
vec3 DiscKernel[28];
varying vec2 xlv_TEXCOORD1;
void main ()
{
  DiscKernel[0] = vec3(0.62463, 0.54337, 0.8279);
  DiscKernel[1] = vec3(-0.13414, -0.94488, 0.95435);
  DiscKernel[2] = vec3(0.38772, -0.43475, 0.58253);
  DiscKernel[3] = vec3(0.12126, -0.19282, 0.22778);
  DiscKernel[4] = vec3(-0.20388, 0.11133, 0.2323);
  DiscKernel[5] = vec3(0.83114, -0.29218, 0.881);
  DiscKernel[6] = vec3(0.10759, -0.57839, 0.58831);
  DiscKernel[7] = vec3(0.28285, 0.79036, 0.83945);
  DiscKernel[8] = vec3(-0.36622, 0.39516, 0.53876);
  DiscKernel[9] = vec3(0.75591, 0.21916, 0.78704);
  DiscKernel[10] = vec3(-0.5261, 0.02386, 0.52664);
  DiscKernel[11] = vec3(-0.88216, -0.24471, 0.91547);
  DiscKernel[12] = vec3(-0.48888, -0.2933, 0.57011);
  DiscKernel[13] = vec3(0.44014, -0.08558, 0.44838);
  DiscKernel[14] = vec3(0.21179, 0.51373, 0.55567);
  DiscKernel[15] = vec3(0.05483, 0.95701, 0.95858);
  DiscKernel[16] = vec3(-0.59001, -0.70509, 0.91938);
  DiscKernel[17] = vec3(-0.80065, 0.24631, 0.83768);
  DiscKernel[18] = vec3(-0.19424, -0.18402, 0.26757);
  DiscKernel[19] = vec3(-0.43667, 0.76751, 0.88304);
  DiscKernel[20] = vec3(0.21666, 0.11602, 0.24577);
  DiscKernel[21] = vec3(0.15696, -0.856, 0.87027);
  DiscKernel[22] = vec3(-0.75821, 0.58363, 0.95682);
  DiscKernel[23] = vec3(0.99284, -0.02904, 0.99327);
  DiscKernel[24] = vec3(-0.22234, -0.57907, 0.62029);
  DiscKernel[25] = vec3(0.55052, -0.66984, 0.86704);
  DiscKernel[26] = vec3(0.46431, 0.28115, 0.5428);
  DiscKernel[27] = vec3(-0.07214, 0.60554, 0.60982);
  vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  vec4 returnValue_2;
  float sampleCount_4;
  vec4 poissonScale_5;
  vec4 sum_6;
  vec4 centerTap_7;
  vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, xlv_TEXCOORD1);
  centerTap_7 = tmpvar_8;
  poissonScale_5 = ((_MainTex_TexelSize.xyxy * tmpvar_8.w) * _Offsets.w);
  float tmpvar_9;
  tmpvar_9 = max ((tmpvar_8.w * 0.25), _Offsets.z);
  sampleCount_4 = tmpvar_9;
  sum_6 = (tmpvar_8 * tmpvar_9);
  for (int l_3 = 0; l_3 < 28; l_3++) {
    vec4 tmpvar_10;
    tmpvar_10 = texture2D (_MainTex, (tmpvar_1 + (DiscKernel[l_3].xy * poissonScale_5.xy)));
    if ((tmpvar_10.w > 0.0)) {
      float tmpvar_11;
      float tmpvar_12;
      tmpvar_12 = clamp (((
        (tmpvar_10.w - (centerTap_7.w * DiscKernel[l_3].z))
       - -0.265) / 0.265), 0.0, 1.0);
      tmpvar_11 = (tmpvar_12 * (tmpvar_12 * (3.0 - 
        (2.0 * tmpvar_12)
      )));
      sum_6 = (sum_6 + (tmpvar_10 * tmpvar_11));
      sampleCount_4 = (sampleCount_4 + tmpvar_11);
    };
  };
  returnValue_2.xyz = (sum_6 / sampleCount_4).xyz;
  returnValue_2.w = tmpvar_8.w;
  gl_FragData[0] = returnValue_2;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 274 math, 29 textures
Vector 0 [_MainTex_TexelSize]
Vector 1 [_Offsets]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c2, 0.25, 0.827899992, 0.264999986, 3.77358508
def c3, -2, 3, 0.954349995, 0.582530022
def c4, 0.227779999, 0.232299998, 0.880999982, 0.588310003
def c5, 0.839450002, 0.538760006, 0.787039995, 0.526639998
def c6, 0.915470004, 0.570110023, 0.448379993, 0.555670023
def c7, 0.958580017, 0.919380009, 0.837679982, 0.267569989
def c8, 0.883040011, 0.245770007, 0.870270014, 0.956820011
def c9, 0.99326998, 0.620289981, 0.867039979, 0.542800009
def c10, 0.609820008, 0, 0, 0
def c11, 0.46430999, 0.281150013, -0.0721400008, 0.605539978
def c12, -0.222340003, -0.579069972, 0.550520003, -0.669839978
def c13, -0.758210003, 0.583630025, 0.992839992, -0.0290399995
def c14, 0.216659993, 0.116020001, 0.156959996, -0.856000006
def c15, -0.194240004, -0.184019998, -0.436670005, 0.767509997
def c16, -0.590009987, -0.705089986, -0.800650001, 0.246309996
def c17, 0.211789995, 0.51372999, 0.0548299998, 0.957009971
def c18, -0.488880008, -0.293300003, 0.440140009, -0.0855799988
def c19, -0.52609998, 0.0238600001, -0.882160008, -0.244709998
def c20, -0.366219997, 0.39515999, 0.755909979, 0.219160005
def c21, 0.107589997, -0.578390002, 0.282849997, 0.790359974
def c22, -0.203879997, 0.111330003, 0.831139982, -0.292180002
def c23, 0.387719989, -0.434749991, 0.121260002, -0.192819998
def c24, 0.624629974, 0.543370008, -0.13414, -0.944880009
dcl_texcoord1 v0.xy
dcl_2d s0
texld r0, v0, s0
mul r1, r0.w, c0.xyxy
mul r1, r1, c1.w
mad r2, r1.zwzw, c24, v0.xyxy
texld r3, r2, s0
texld r2, r2.zwzw, s0
mad r4.x, r0.w, -c2.y, r3.w
add r4.x, r4.x, c2.z
mul_sat r4.x, r4.x, c2.w
mad r4.y, r4.x, c3.x, c3.y
mul r4.x, r4.x, r4.x
mul r4.z, r4.x, r4.y
mul r4.w, r0.w, c2.x
max r5.w, r4.w, c1.z
mul r5.xyz, r0, r5.w
mad r6.xyz, r3, r4.z, r5
mad r6.w, r4.y, r4.x, r5.w
cmp r3, -r3.w, r5, r6
mad r0.x, r0.w, -c3.z, r2.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r3.w
mad r4.xyz, r2, r0.z, r3
cmp r2, -r2.w, r3, r4
mad r3, r1.zwzw, c23, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r0.x, r0.w, -c3.w, r4.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r5.w, r0.y, r0.x, r2.w
mad r5.xyz, r4, r0.z, r2
cmp r2, -r4.w, r2, r5
mad r0.x, r0.w, -c4.x, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
mad r3, r1.zwzw, c22, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r0.x, r0.w, -c4.y, r4.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r5.w, r0.y, r0.x, r2.w
mad r5.xyz, r4, r0.z, r2
cmp r2, -r4.w, r2, r5
mad r0.x, r0.w, -c4.z, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
mad r3, r1.zwzw, c21, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r0.x, r0.w, -c4.w, r4.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r5.w, r0.y, r0.x, r2.w
mad r5.xyz, r4, r0.z, r2
cmp r2, -r4.w, r2, r5
mad r0.x, r0.w, -c5.x, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
mad r3, r1.zwzw, c20, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r0.x, r0.w, -c5.y, r4.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r5.w, r0.y, r0.x, r2.w
mad r5.xyz, r4, r0.z, r2
cmp r2, -r4.w, r2, r5
mad r0.x, r0.w, -c5.z, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
mad r3, r1.zwzw, c19, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r0.x, r0.w, -c5.w, r4.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r5.w, r0.y, r0.x, r2.w
mad r5.xyz, r4, r0.z, r2
cmp r2, -r4.w, r2, r5
mad r0.x, r0.w, -c6.x, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
mad r3, r1.zwzw, c18, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r0.x, r0.w, -c6.y, r4.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r5.w, r0.y, r0.x, r2.w
mad r5.xyz, r4, r0.z, r2
cmp r2, -r4.w, r2, r5
mad r0.x, r0.w, -c6.z, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
mad r3, r1.zwzw, c17, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r0.x, r0.w, -c6.w, r4.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r5.w, r0.y, r0.x, r2.w
mad r5.xyz, r4, r0.z, r2
cmp r2, -r4.w, r2, r5
mad r0.x, r0.w, -c7.x, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
mad r3, r1.zwzw, c16, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r0.x, r0.w, -c7.y, r4.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r5.w, r0.y, r0.x, r2.w
mad r5.xyz, r4, r0.z, r2
cmp r2, -r4.w, r2, r5
mad r0.x, r0.w, -c7.z, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
mad r3, r1.zwzw, c15, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r0.x, r0.w, -c7.w, r4.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r5.w, r0.y, r0.x, r2.w
mad r5.xyz, r4, r0.z, r2
cmp r2, -r4.w, r2, r5
mad r0.x, r0.w, -c8.x, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
mad r3, r1.zwzw, c14, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r0.x, r0.w, -c8.y, r4.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r5.w, r0.y, r0.x, r2.w
mad r5.xyz, r4, r0.z, r2
cmp r2, -r4.w, r2, r5
mad r0.x, r0.w, -c8.z, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
mad r3, r1.zwzw, c13, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r0.x, r0.w, -c8.w, r4.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r5.w, r0.y, r0.x, r2.w
mad r5.xyz, r4, r0.z, r2
cmp r2, -r4.w, r2, r5
mad r0.x, r0.w, -c9.x, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
mad r3, r1.zwzw, c12, v0.xyxy
mad r1, r1, c11, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r0.x, r0.w, -c9.y, r4.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r5.w, r0.y, r0.x, r2.w
mad r5.xyz, r4, r0.z, r2
cmp r2, -r4.w, r2, r5
mad r0.x, r0.w, -c9.z, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
texld r3, r1, s0
texld r1, r1.zwzw, s0
mad r0.x, r0.w, -c9.w, r3.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r4.w, r0.y, r0.x, r2.w
mad r4.xyz, r3, r0.z, r2
cmp r2, -r3.w, r2, r4
mad r0.x, r0.w, -c10.x, r1.w
mov oC0.w, r0.w
add r0.x, r0.x, c2.z
mul_sat r0.x, r0.x, c2.w
mad r0.y, r0.x, c3.x, c3.y
mul r0.x, r0.x, r0.x
mul r0.z, r0.x, r0.y
mad r3.w, r0.y, r0.x, r2.w
mad r3.xyz, r1, r0.z, r2
cmp r0, -r1.w, r2, r3
rcp r0.w, r0.w
mul oC0.xyz, r0.w, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 18 math, 2 textures, 2 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
Vector 128 [_Offsets]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedpdmpmkmjphkjlpkdmanakimiafefohbnabaaaaaaomafaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcbeafaaaaeaaaaaaaefabaaaa
dfbiaaaahcaaaaaamaohbpdpembkaldpebpbfddpaaaaaaaappflajlokiodhblp
eifahedpaaaaaaaadmidmgdoinjhnololacabfdpaaaaaaaackfhpidnjlhceflo
cjdpgjdoaaaaaaaaolmffalopmaaoednanoagndoaaaaaaaajhmffedpjojijflo
dhijgbdpaaaaaaaacffinmdnfobbbelphmjlbgdpaaaaaaaalhnbjadoaiffekdp
dcogfgdpaaaaaaaadaiblllogjfcmkdocnomajdpaaaaaaaafbidebdphlglgado
hehlejdpaaaaaaaahnkoaglpamhgmddmobnbagdpaaaaaaaadnnfgblpecjfhklo
dofmgkdpaaaaaaaahleopkloglcljglollpcbbdpaaaaaaaaaifkobdojbeekpln
bajcofdoaaaaaaaahknpfidompidaddpgeeaaodpaaaaaaaagmjfgadnjlpohedp
iagfhfdpaaaaaaaaofakbhlpmhiadelphnfmgldpaaaaaaaaggphemlpladihmdo
dchcfgdpaaaaaaaankogeglolngpdmlooppoiidoaaaaaaaadgjdnploijhleedp
ojaogcdpaaaaaaaabonmfndoofjlondnccklhldoaaaaaaaabplkcadonbccfllp
aemkfodpaaaaaaaaanbkeclpmhgibfdpcipchedpaaaaaaaamdckhodpelofonlm
pbeghodpaaaaaaaabjkngdlooodnbelpfdmlbodpaaaaaaaaobooamdpkchkcllp
ffpgfndpaaaaaaaaaklkondoofpcipdopbpeakdpaaaaaaaacdlojdlnklaebldp
ckbnbmdpaaaaaaaafjaaaaaeegiocaaaaaaaaaaaajaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaadmcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacafaaaaaaefaaaaajpcaabaaaaaaaaaaa
ogbkbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaaidcaabaaa
abaaaaaapgapbaaaaaaaaaaaegiacaaaaaaaaaaaahaaaaaadiaaaaaidcaabaaa
abaaaaaaegaabaaaabaaaaaapgipcaaaaaaaaaaaaiaaaaaadiaaaaahecaabaaa
abaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaiadodeaaaaaiecaabaaaabaaaaaa
ckaabaaaabaaaaaackiacaaaaaaaaaaaaiaaaaaadiaaaaahhcaabaaaaaaaaaaa
egacbaaaaaaaaaaakgakbaaaabaaaaaadgaaaaafhcaabaaaacaaaaaaegacbaaa
aaaaaaaadgaaaaaficaabaaaabaaaaaackaabaaaabaaaaaadgaaaaaficaabaaa
acaaaaaaabeaaaaaaaaaaaaadaaaaaabcbaaaaahbcaabaaaadaaaaaadkaabaaa
acaaaaaaabeaaaaabmaaaaaaadaaaeadakaabaaaadaaaaaadcaaaaakdcaabaaa
adaaaaaaegjajaaadkaabaaaacaaaaaaegaabaaaabaaaaaaogbkbaaaabaaaaaa
efaaaaajpcaabaaaadaaaaaaegaabaaaadaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadbaaaaahbcaabaaaaeaaaaaaabeaaaaaaaaaaaaadkaabaaaadaaaaaa
bpaaaeadakaabaaaaeaaaaaadcaaaaalicaabaaaadaaaaaadkaabaiaebaaaaaa
aaaaaaaackjajaaadkaabaaaacaaaaaadkaabaaaadaaaaaaaaaaaaahicaabaaa
adaaaaaadkaabaaaadaaaaaaabeaaaaabekoihdodicaaaahicaabaaaadaaaaaa
dkaabaaaadaaaaaaabeaaaaaglichbeadcaaaaajbcaabaaaaeaaaaaadkaabaaa
adaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaeadiaaaaahicaabaaaadaaaaaa
dkaabaaaadaaaaaadkaabaaaadaaaaaadiaaaaahccaabaaaaeaaaaaadkaabaaa
adaaaaaaakaabaaaaeaaaaaadcaaaaajhcaabaaaacaaaaaaegacbaaaadaaaaaa
fgafbaaaaeaaaaaaegacbaaaacaaaaaadcaaaaajicaabaaaabaaaaaaakaabaaa
aeaaaaaadkaabaaaadaaaaaadkaabaaaabaaaaaabfaaaaabboaaaaahicaabaaa
acaaaaaadkaabaaaacaaaaaaabeaaaaaabaaaaaabgaaaaabaoaaaaahhccabaaa
aaaaaaaaegacbaaaacaaaaaapgapbaaaabaaaaaadgaaaaaficcabaaaaaaaaaaa
dkaabaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 49 math, 3 texture, 3 branch
 // Stats for Fragment shader:
 //       d3d11 : 30 math, 3 texture, 1 branch
 //        d3d9 : 119 math, 14 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 838424
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 49 math, 3 textures, 3 branches
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform sampler2D _LowRez;
uniform vec4 _MainTex_TexelSize;
uniform vec4 _Offsets;
vec2 SmallDiscKernel[12];
varying vec2 xlv_TEXCOORD1;
void main ()
{
  SmallDiscKernel[0] = vec2(-0.326212, -0.40581);
  SmallDiscKernel[1] = vec2(-0.840144, -0.07358);
  SmallDiscKernel[2] = vec2(-0.695914, 0.457137);
  SmallDiscKernel[3] = vec2(-0.203345, 0.620716);
  SmallDiscKernel[4] = vec2(0.96234, -0.194983);
  SmallDiscKernel[5] = vec2(0.473434, -0.480026);
  SmallDiscKernel[6] = vec2(0.519456, 0.767022);
  SmallDiscKernel[7] = vec2(0.185461, -0.893124);
  SmallDiscKernel[8] = vec2(0.507431, 0.064425);
  SmallDiscKernel[9] = vec2(0.89642, 0.412458);
  SmallDiscKernel[10] = vec2(-0.32194, -0.932615);
  SmallDiscKernel[11] = vec2(-0.791559, -0.59771);
  vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  float sampleCount_3;
  vec4 poissonScale_4;
  vec4 smallBlur_5;
  vec4 centerTap_6;
  vec4 tmpvar_7;
  tmpvar_7 = texture2D (_LowRez, xlv_TEXCOORD1);
  vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, xlv_TEXCOORD1);
  centerTap_6 = tmpvar_8;
  poissonScale_4 = ((_MainTex_TexelSize.xyxy * tmpvar_8.w) * _Offsets.w);
  float tmpvar_9;
  tmpvar_9 = max ((tmpvar_8.w * 0.25), 0.1);
  sampleCount_3 = tmpvar_9;
  smallBlur_5 = (tmpvar_8 * tmpvar_9);
  for (int l_2 = 0; l_2 < 12; l_2++) {
    vec4 tmpvar_10;
    tmpvar_10 = texture2D (_MainTex, (tmpvar_1 + ((SmallDiscKernel[l_2] * poissonScale_4.xy) * 1.1)));
    vec2 x_11;
    x_11 = (SmallDiscKernel[l_2] * 1.1);
    float tmpvar_12;
    float tmpvar_13;
    tmpvar_13 = clamp (((
      (tmpvar_10.w - (centerTap_6.w * sqrt(dot (x_11, x_11))))
     - -0.265) / 0.265), 0.0, 1.0);
    tmpvar_12 = (tmpvar_13 * (tmpvar_13 * (3.0 - 
      (2.0 * tmpvar_13)
    )));
    smallBlur_5 = (smallBlur_5 + (tmpvar_10 * tmpvar_12));
    sampleCount_3 = (sampleCount_3 + tmpvar_12);
  };
  float tmpvar_14;
  tmpvar_14 = clamp (((tmpvar_8.w - 0.4) / 0.2), 0.0, 1.0);
  vec4 tmpvar_15;
  tmpvar_15 = mix ((smallBlur_5 / (sampleCount_3 + 1e-05)), tmpvar_7, vec4((tmpvar_14 * (tmpvar_14 * 
    (3.0 - (2.0 * tmpvar_14))
  ))));
  smallBlur_5 = tmpvar_15;
  vec4 tmpvar_16;
  if ((tmpvar_8.w < 0.01)) {
    tmpvar_16 = tmpvar_8;
  } else {
    vec4 tmpvar_17;
    tmpvar_17.xyz = tmpvar_15.xyz;
    tmpvar_17.w = tmpvar_8.w;
    tmpvar_16 = tmpvar_17;
  };
  gl_FragData[0] = tmpvar_16;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 119 math, 14 textures
Vector 0 [_MainTex_TexelSize]
Vector 1 [_Offsets]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_LowRez] 2D 1
""ps_3_0
def c2, 0.25, 0.100000001, 0.572735727, 0.264999986
def c3, 3.77358508, -2, 3, 0.92769593
def c4, 0.915891469, 0.718492448, 1.08008385, 0.741635561
def c5, 1.0190047, 1.00339437, 0.562654853, 1.08543336
def c6, 1.08528042, 1.09106636, 9.99999975e-006, 4.99999952
def c7, -0.400000006, -0.00999999978, 0, 0
def c8, -0.358833194, -0.446391016, -0.924158394, -0.0809379965
def c9, -0.765505373, 0.502850711, -0.223679513, 0.682787597
def c10, 1.05857396, -0.214481309, 0.520777404, -0.528028607
def c11, 0.571401656, 0.843724251, 0.204007104, -0.982436419
def c12, 0.558174074, 0.0708675012, 0.98606205, 0.453703821
def c13, -0.354134023, -1.02587652, -0.870714903, -0.657481015
dcl_texcoord1 v0.xy
dcl_2d s0
dcl_2d s1
texld r0, v0, s0
mul r1, r0.w, c0.xyxy
mul r1, r1, c1.w
mad r2, r1.zwzw, c8, v0.xyxy
texld r3, r2, s0
texld r2, r2.zwzw, s0
mad r3.w, r0.w, -c2.z, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mul r3.xyz, r3, r4.y
mul r4.y, r0.w, c2.x
max r5.x, r4.y, c2.y
mad r3.xyz, r0, r5.x, r3
mad r3.w, r4.x, r3.w, r5.x
mad r2.w, r0.w, -c3.w, r2.w
add r2.w, r2.w, c2.w
mul_sat r2.w, r2.w, c3.x
mad r4.x, r2.w, c3.y, c3.z
mul r2.w, r2.w, r2.w
mul r4.y, r2.w, r4.x
mad r2.w, r4.x, r2.w, r3.w
mad r2.xyz, r2, r4.y, r3
mad r3, r1.zwzw, c9, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c4.x, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c4.y, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c10, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c4.z, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c4.w, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c11, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c5.x, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c5.y, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c12, v0.xyxy
mad r1, r1, c13, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c5.z, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c5.w, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
texld r3, r1, s0
texld r1, r1.zwzw, s0
mad r3.w, r0.w, -c6.x, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r1.w, r0.w, -c6.y, r1.w
add r1.w, r1.w, c2.w
mul_sat r1.w, r1.w, c3.x
mad r3.x, r1.w, c3.y, c3.z
mul r1.w, r1.w, r1.w
mul r3.y, r1.w, r3.x
mad r1.w, r3.x, r1.w, r2.w
add r1.w, r1.w, c6.z
rcp r1.w, r1.w
mad r1.xyz, r1, r3.y, r2
texld r2, v0, s1
mad r2.xyz, r1, -r1.w, r2
mul r1.xyz, r1.w, r1
add r3.xy, r0.w, c7
mul_sat r1.w, r3.x, c6.w
mad r2.w, r1.w, c3.y, c3.z
mul r1.w, r1.w, r1.w
mul r1.w, r1.w, r2.w
mad r1.xyz, r1.w, r2, r1
mov r1.w, r0.w
cmp oC0, r3.y, r1, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 30 math, 3 textures, 1 branches
SetTexture 0 [_LowRez] 2D 1
SetTexture 1 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
Vector 128 [_Offsets]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedgibcclijllnccododpnjhgflmgiaipjmabaaaaaalaagaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcniafaaaaeaaaaaaahgabaaaa
dfbiaaaadcaaaaaaecafkhlofemgmploaaaaaaaaaaaaaaaaknbdfhlpbmlbjgln
aaaaaaaaaaaaaaaaglchdclpnmanokdoaaaaaaaaaaaaaaaakmdjfalodoohbodp
aaaaaaaaaaaaaaaaokflhgdpkakjehloaaaaaaaaaaaaaaaapbgfpcdopimfpflo
aaaaaaaaaaaaaaaabcplaedpiofleedpaaaaaaaaaaaaaaaahnojdndomgkdgelp
aaaaaaaaaaaaaaaappogabdpebpbiddnaaaaaaaaaaaaaaaamihlgfdplccnnddo
aaaaaaaaaaaaaaaafcnfkelonllpgolpaaaaaaaaaaaaaaaajmkdeklpigadbjlp
aaaaaaaaaaaaaaaafjaaaaaeegiocaaaaaaaaaaaajaaaaaafkaaaaadaagabaaa
aaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaa
fibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaadmcbabaaaabaaaaaagfaaaaad
pccabaaaaaaaaaaagiaaaaacahaaaaaaefaaaaajpcaabaaaaaaaaaaaogbkbaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaabaaaaaaefaaaaajpcaabaaaabaaaaaa
ogbkbaaaabaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadiaaaaaidcaabaaa
acaaaaaapgapbaaaabaaaaaaegiacaaaaaaaaaaaahaaaaaadiaaaaaidcaabaaa
acaaaaaaegaabaaaacaaaaaapgipcaaaaaaaaaaaaiaaaaaadiaaaaahicaabaaa
aaaaaaaadkaabaaaabaaaaaaabeaaaaaaaaaiadodeaaaaahicaabaaaaaaaaaaa
dkaabaaaaaaaaaaaabeaaaaamnmmmmdndiaaaaahhcaabaaaadaaaaaapgapbaaa
aaaaaaaaegacbaaaabaaaaaadgaaaaafhcaabaaaaeaaaaaaegacbaaaadaaaaaa
dgaaaaafecaabaaaacaaaaaadkaabaaaaaaaaaaadgaaaaaficaabaaaacaaaaaa
abeaaaaaaaaaaaaadaaaaaabcbaaaaahicaabaaaadaaaaaadkaabaaaacaaaaaa
abeaaaaaamaaaaaaadaaaeaddkaabaaaadaaaaaadiaaaaaidcaabaaaafaaaaaa
egaabaaaacaaaaaaegjajaaadkaabaaaacaaaaaadcaaaaamdcaabaaaafaaaaaa
egaabaaaafaaaaaaaceaaaaamnmmimdpmnmmimdpaaaaaaaaaaaaaaaaogbkbaaa
abaaaaaaefaaaaajpcaabaaaafaaaaaaegaabaaaafaaaaaaeghobaaaabaaaaaa
aagabaaaaaaaaaaadiaaaaaldcaabaaaagaaaaaaaceaaaaamnmmimdpmnmmimdp
aaaaaaaaaaaaaaaaegjajaaadkaabaaaacaaaaaaapaaaaahicaabaaaadaaaaaa
egaabaaaagaaaaaaegaabaaaagaaaaaaelaaaaaficaabaaaadaaaaaadkaabaaa
adaaaaaadcaaaaakicaabaaaadaaaaaadkaabaiaebaaaaaaabaaaaaadkaabaaa
adaaaaaadkaabaaaafaaaaaaaaaaaaahicaabaaaadaaaaaadkaabaaaadaaaaaa
abeaaaaabekoihdodicaaaahicaabaaaadaaaaaadkaabaaaadaaaaaaabeaaaaa
glichbeadcaaaaajicaabaaaaeaaaaaadkaabaaaadaaaaaaabeaaaaaaaaaaama
abeaaaaaaaaaeaeadiaaaaahicaabaaaadaaaaaadkaabaaaadaaaaaadkaabaaa
adaaaaaadiaaaaahicaabaaaafaaaaaadkaabaaaadaaaaaadkaabaaaaeaaaaaa
dcaaaaajhcaabaaaaeaaaaaaegacbaaaafaaaaaapgapbaaaafaaaaaaegacbaaa
aeaaaaaadcaaaaajecaabaaaacaaaaaadkaabaaaaeaaaaaadkaabaaaadaaaaaa
ckaabaaaacaaaaaaboaaaaahicaabaaaacaaaaaadkaabaaaacaaaaaaabeaaaaa
abaaaaaabgaaaaabaaaaaaahicaabaaaaaaaaaaackaabaaaacaaaaaaabeaaaaa
kmmfchdhaoaaaaahhcaabaaaacaaaaaaegacbaaaaeaaaaaapgapbaaaaaaaaaaa
aaaaaaahicaabaaaaaaaaaaadkaabaaaabaaaaaaabeaaaaamnmmmmlodicaaaah
icaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaappppjpeadcaaaaajicaabaaa
acaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaeadiaaaaah
icaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaaaaaaaaaadiaaaaahicaabaaa
aaaaaaaadkaabaaaaaaaaaaadkaabaaaacaaaaaaaaaaaaaihcaabaaaaaaaaaaa
egacbaaaaaaaaaaaegacbaiaebaaaaaaacaaaaaadcaaaaajhcaabaaaaaaaaaaa
pgapbaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaacaaaaaadbaaaaahbcaabaaa
acaaaaaadkaabaaaabaaaaaaabeaaaaaaknhcddmdgaaaaaficaabaaaaaaaaaaa
dkaabaaaabaaaaaadhaaaaajpccabaaaaaaaaaaaagaabaaaacaaaaaaegaobaaa
abaaaaaaegaobaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 12 math, 2 texture
 // Stats for Fragment shader:
 //       d3d11 : 10 math, 2 texture
 //        d3d9 : 10 math, 2 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  ColorMask A
  GpuProgramID 912123
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 12 math, 2 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform vec4 _ZBufferParams;
uniform sampler2D _CameraDepthTexture;
uniform sampler2D _FgOverlap;
uniform vec4 _CurveParams;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec4 color_1;
  vec4 tmpvar_2;
  tmpvar_2 = texture2D (_FgOverlap, xlv_TEXCOORD1);
  color_1.xyz = tmpvar_2.xyz;
  float tmpvar_3;
  tmpvar_3 = (1.0/(((_ZBufferParams.x * texture2D (_CameraDepthTexture, xlv_TEXCOORD1).x) + _ZBufferParams.y)));
  color_1.w = ((_CurveParams.z * abs(
    (tmpvar_3 - _CurveParams.w)
  )) / (tmpvar_3 + 1e-05));
  color_1.w = clamp (max (0.0, (color_1.w - _CurveParams.y)), 0.0, _CurveParams.x);
  gl_FragData[0] = max (color_1.wwww, tmpvar_2.wwww);
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 10 math, 2 textures
Vector 1 [_CurveParams]
Vector 0 [_ZBufferParams]
SetTexture 0 [_CameraDepthTexture] 2D 0
SetTexture 1 [_FgOverlap] 2D 1
""ps_3_0
def c2, 9.99999975e-006, 0, 0, 0
dcl_texcoord1 v0.xy
dcl_2d s0
dcl_2d s1
texld r0, v0, s0
mad r0.x, c0.x, r0.x, c0.y
rcp r0.x, r0.x
add r0.y, r0.x, -c1.w
add r0.x, r0.x, c2.x
rcp r0.x, r0.x
mul r0.y, r0_abs.y, c1.z
mad r0.x, r0.y, r0.x, -c1.y
max r1.x, r0.x, c2.y
min r0.x, c1.x, r1.x
texld r1, v0, s1
max oC0, r0.x, r1.w

""
}
SubProgram ""d3d11 "" {
// Stats: 10 math, 2 textures
SetTexture 0 [_FgOverlap] 2D 1
SetTexture 1 [_CameraDepthTexture] 2D 0
ConstBuffer ""$Globals"" 144
Vector 96 [_CurveParams]
ConstBuffer ""UnityPerCamera"" 144
Vector 112 [_ZBufferParams]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerCamera"" 1
""ps_4_0
eefiecedeoffbdfhdemdfbmglnfklbofoikfpedoabaaaaaapeacaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcbmacaaaaeaaaaaaaihaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafjaaaaaeegiocaaaabaaaaaaaiaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaadmcbabaaa
abaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaa
aaaaaaaaogbkbaaaabaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadcaaaaal
bcaabaaaaaaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaaaaaaaaabkiacaaa
abaaaaaaahaaaaaaaoaaaaakbcaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaiadpaaaaiadpakaabaaaaaaaaaaaaaaaaaajccaabaaaaaaaaaaaakaabaaa
aaaaaaaadkiacaiaebaaaaaaaaaaaaaaagaaaaaaaaaaaaahbcaabaaaaaaaaaaa
akaabaaaaaaaaaaaabeaaaaakmmfchdhdiaaaaajccaabaaaaaaaaaaabkaabaia
ibaaaaaaaaaaaaaackiacaaaaaaaaaaaagaaaaaaaoaaaaahbcaabaaaaaaaaaaa
bkaabaaaaaaaaaaaakaabaaaaaaaaaaaaaaaaaajbcaabaaaaaaaaaaaakaabaaa
aaaaaaaabkiacaiaebaaaaaaaaaaaaaaagaaaaaadeaaaaahbcaabaaaaaaaaaaa
akaabaaaaaaaaaaaabeaaaaaaaaaaaaaddaaaaaibcaabaaaaaaaaaaaakaabaaa
aaaaaaaaakiacaaaaaaaaaaaagaaaaaaefaaaaajpcaabaaaabaaaaaaogbkbaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaabaaaaaadeaaaaahpccabaaaaaaaaaaa
agaabaaaaaaaaaaapgapbaaaabaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 5 math, 2 texture
 // Stats for Fragment shader:
 //       d3d11 : 4 math, 2 texture
 //        d3d9 : 4 math, 2 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Blend One One
 BlendOp Max
  ColorMask A
  GpuProgramID 964306
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 5 math, 2 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform sampler2D _FgOverlap;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD1);
  gl_FragData[0] = vec4((float((tmpvar_1.w < 0.01)) * clamp ((texture2D (_FgOverlap, xlv_TEXCOORD1).w - tmpvar_1.w), 0.0, 1.0)));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 4 math, 2 textures
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_FgOverlap] 2D 1
""ps_3_0
def c0, -0.00999999978, 0, 1, 0
dcl_texcoord1 v0.xy
dcl_2d s0
dcl_2d s1
texld r0, v0, s0
add r0.x, r0.w, c0.x
cmp r0.x, r0.x, c0.y, c0.z
texld r1, v0, s1
add_sat r0.y, -r0.w, r1.w
mul oC0, r0.y, r0.x

""
}
SubProgram ""d3d11 "" {
// Stats: 4 math, 2 textures
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_FgOverlap] 2D 1
""ps_4_0
eefiecedpapmgjjipdgaibneanpfifjcaoahicifabaaaaaapiabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefccaabaaaaeaaaaaaaeiaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaadmcbabaaa
abaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaa
aaaaaaaaogbkbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadbaaaaah
bcaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaknhcddmabaaaaahbcaabaaa
aaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaiadpefaaaaajpcaabaaaabaaaaaa
ogbkbaaaabaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaaaacaaaaiccaabaaa
aaaaaaaadkaabaiaebaaaaaaaaaaaaaadkaabaaaabaaaaaadiaaaaahpccabaaa
aaaaaaaafgafbaaaaaaaaaaaagaabaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 11 math, 4 texture
 // Stats for Fragment shader:
 //       d3d11 : 6 math, 4 texture
 //        d3d9 : 7 math, 4 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 1035383
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 11 math, 4 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 cse_1;
  cse_1 = (0.75 * _MainTex_TexelSize.xy);
  gl_FragData[0] = (((
    (texture2D (_MainTex, (xlv_TEXCOORD1 + cse_1)) + texture2D (_MainTex, (xlv_TEXCOORD1 - cse_1)))
   + texture2D (_MainTex, 
    (xlv_TEXCOORD1 + (cse_1 * vec2(1.0, -1.0)))
  )) + texture2D (_MainTex, (xlv_TEXCOORD1 - 
    (cse_1 * vec2(1.0, -1.0))
  ))) / 4.0);
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 7 math, 4 textures
Vector 0 [_MainTex_TexelSize]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c1, 0.75, -0.75, 0.25, 0
dcl_texcoord1 v0.xy
dcl_2d s0
mov r0.xy, c1
mad r1, c0.xyxy, r0.xxxy, v0.xyxy
texld r2, r1, s0
texld r1, r1.zwzw, s0
mad r0, c0.xyxy, -r0.xxxy, v0.xyxy
texld r3, r0, s0
texld r0, r0.zwzw, s0
add r2, r2, r3
add r1, r1, r2
add r0, r0, r1
mul oC0, r0, c1.z

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math, 4 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedkpcgndacpbdnfeaddomjkhghglbgdnobabaaaaaakiacaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcnaabaaaaeaaaaaaaheaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaagcbaaaadmcbabaaaabaaaaaagfaaaaadpccabaaa
aaaaaaaagiaaaaacaeaaaaaadcaaaaanpcaabaaaaaaaaaaaegiecaaaaaaaaaaa
ahaaaaaaaceaaaaaaaaaeadpaaaaeadpaaaaeadpaaaaealpogbobaaaabaaaaaa
efaaaaajpcaabaaaabaaaaaaegaabaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaefaaaaajpcaabaaaaaaaaaaaogakbaaaaaaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadcaaaaaopcaabaaaacaaaaaaegiecaiaebaaaaaaaaaaaaaa
ahaaaaaaaceaaaaaaaaaeadpaaaaeadpaaaaeadpaaaaealpogbobaaaabaaaaaa
efaaaaajpcaabaaaadaaaaaaegaabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaefaaaaajpcaabaaaacaaaaaaogakbaaaacaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaaaaaaaahpcaabaaaabaaaaaaegaobaaaabaaaaaaegaobaaa
adaaaaaaaaaaaaahpcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaa
aaaaaaahpcaabaaaaaaaaaaaegaobaaaacaaaaaaegaobaaaaaaaaaaadiaaaaak
pccabaaaaaaaaaaaegaobaaaaaaaaaaaaceaaaaaaaaaiadoaaaaiadoaaaaiado
aaaaiadodoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 3 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 1 math, 1 texture
 //        d3d9 : 4 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 1061922
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 3 math, 1 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform vec4 _CurveParams;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec4 returnValue_1;
  vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD1);
  returnValue_1.w = tmpvar_2.w;
  returnValue_1.xyz = mix (vec3(0.0, 0.0, 0.0), vec3(1.0, 1.0, 1.0), vec3(clamp ((tmpvar_2.w / _CurveParams.x), 0.0, 1.0)));
  gl_FragData[0] = returnValue_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 4 math, 1 textures
Vector 0 [_CurveParams]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
dcl_texcoord1 v0.xy
dcl_2d s0
rcp r0.x, c0.x
texld r1, v0, s0
mul r0.x, r0.x, r1.w
mov oC0.w, r1.w
mov_sat oC0.xyz, r0.x

""
}
SubProgram ""d3d11 "" {
// Stats: 1 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 144
Vector 96 [_CurveParams]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedeodpphpignhchiognobjagdepjaofeopabaaaaaajmabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcmeaaaaaaeaaaaaaadbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaagcbaaaadmcbabaaaabaaaaaagfaaaaadpccabaaa
aaaaaaaagiaaaaacabaaaaaaefaaaaajpcaabaaaaaaaaaaaogbkbaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaaoaaaaaibcaabaaaaaaaaaaadkaabaaa
aaaaaaaaakiacaaaaaaaaaaaagaaaaaadgaaaaaficcabaaaaaaaaaaadkaabaaa
aaaaaaaadgcaaaafhccabaaaaaaaaaaaagaabaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 60 math, 3 texture, 3 branch
 // Stats for Fragment shader:
 //       d3d11 : 25 math, 3 texture, 2 branch
 //        d3d9 : 456 math, 57 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 1133637
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 60 math, 3 textures, 3 branches
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
uniform vec4 _Offsets;
vec3 DiscKernel[28];
varying vec2 xlv_TEXCOORD1;
void main ()
{
  DiscKernel[0] = vec3(0.62463, 0.54337, 0.8279);
  DiscKernel[1] = vec3(-0.13414, -0.94488, 0.95435);
  DiscKernel[2] = vec3(0.38772, -0.43475, 0.58253);
  DiscKernel[3] = vec3(0.12126, -0.19282, 0.22778);
  DiscKernel[4] = vec3(-0.20388, 0.11133, 0.2323);
  DiscKernel[5] = vec3(0.83114, -0.29218, 0.881);
  DiscKernel[6] = vec3(0.10759, -0.57839, 0.58831);
  DiscKernel[7] = vec3(0.28285, 0.79036, 0.83945);
  DiscKernel[8] = vec3(-0.36622, 0.39516, 0.53876);
  DiscKernel[9] = vec3(0.75591, 0.21916, 0.78704);
  DiscKernel[10] = vec3(-0.5261, 0.02386, 0.52664);
  DiscKernel[11] = vec3(-0.88216, -0.24471, 0.91547);
  DiscKernel[12] = vec3(-0.48888, -0.2933, 0.57011);
  DiscKernel[13] = vec3(0.44014, -0.08558, 0.44838);
  DiscKernel[14] = vec3(0.21179, 0.51373, 0.55567);
  DiscKernel[15] = vec3(0.05483, 0.95701, 0.95858);
  DiscKernel[16] = vec3(-0.59001, -0.70509, 0.91938);
  DiscKernel[17] = vec3(-0.80065, 0.24631, 0.83768);
  DiscKernel[18] = vec3(-0.19424, -0.18402, 0.26757);
  DiscKernel[19] = vec3(-0.43667, 0.76751, 0.88304);
  DiscKernel[20] = vec3(0.21666, 0.11602, 0.24577);
  DiscKernel[21] = vec3(0.15696, -0.856, 0.87027);
  DiscKernel[22] = vec3(-0.75821, 0.58363, 0.95682);
  DiscKernel[23] = vec3(0.99284, -0.02904, 0.99327);
  DiscKernel[24] = vec3(-0.22234, -0.57907, 0.62029);
  DiscKernel[25] = vec3(0.55052, -0.66984, 0.86704);
  DiscKernel[26] = vec3(0.46431, 0.28115, 0.5428);
  DiscKernel[27] = vec3(-0.07214, 0.60554, 0.60982);
  vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  vec4 returnValue_2;
  float sampleCount_4;
  vec4 poissonScale_5;
  vec4 sum_6;
  vec4 centerTap_7;
  vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, xlv_TEXCOORD1);
  centerTap_7 = tmpvar_8;
  poissonScale_5 = ((_MainTex_TexelSize.xyxy * tmpvar_8.w) * _Offsets.w);
  float tmpvar_9;
  tmpvar_9 = max ((tmpvar_8.w * 0.25), _Offsets.z);
  sampleCount_4 = tmpvar_9;
  sum_6 = (tmpvar_8 * tmpvar_9);
  for (int l_3 = 0; l_3 < 28; l_3++) {
    vec4 tmpvar_10;
    tmpvar_10.xy = vec2(1.2, 1.2);
    tmpvar_10.zw = DiscKernel[l_3].zz;
    vec4 tmpvar_11;
    tmpvar_11 = (tmpvar_1.xyxy + ((DiscKernel[l_3].xyxy * poissonScale_5.xyxy) / tmpvar_10));
    vec4 tmpvar_12;
    tmpvar_12 = texture2D (_MainTex, tmpvar_11.xy);
    vec4 tmpvar_13;
    tmpvar_13 = texture2D (_MainTex, tmpvar_11.zw);
    if (((tmpvar_12.w + tmpvar_13.w) > 0.0)) {
      vec2 tmpvar_14;
      tmpvar_14.y = 1.0;
      tmpvar_14.x = (DiscKernel[l_3].z / 1.2);
      vec2 tmpvar_15;
      tmpvar_15.x = tmpvar_12.w;
      tmpvar_15.y = tmpvar_13.w;
      vec2 tmpvar_16;
      vec2 tmpvar_17;
      tmpvar_17 = clamp (((
        (tmpvar_15 - (centerTap_7.ww * tmpvar_14))
       - vec2(-0.265, -0.265)) / vec2(0.265, 0.265)), 0.0, 1.0);
      tmpvar_16 = (tmpvar_17 * (tmpvar_17 * (3.0 - 
        (2.0 * tmpvar_17)
      )));
      sum_6 = (sum_6 + ((tmpvar_12 * tmpvar_16.x) + (tmpvar_13 * tmpvar_16.y)));
      sampleCount_4 = (sampleCount_4 + dot (tmpvar_16, vec2(1.0, 1.0)));
    };
  };
  returnValue_2.xyz = (sum_6 / sampleCount_4).xyz;
  returnValue_2.w = tmpvar_8.w;
  gl_FragData[0] = returnValue_2;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 456 math, 57 textures
Vector 0 [_MainTex_TexelSize]
Vector 1 [_Offsets]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c2, 0.25, 0.68991667, 1, 0.264999986
def c3, 0.795291662, 1, 0.485441685, 0.189816669
def c4, 0.193583325, 1, 0.734166622, 0.490258336
def c5, 0.699541628, 1, 0.448966652, 0.655866623
def c6, 0.438866645, 1, 0.76289165, 0.475091666
def c7, 0.373649985, 1, 0.463058352, 0.798816681
def c8, 3.77358508, -2, 3, 0
def c9, 0.766149998, 1, 0.698066652, 0.222974986
def c10, 0.735866666, 1, 0.204808339, 0.725224972
def c11, 0.797349989, 1, 0.827724993, 0.516908288
def c12, 0.722533286, 1, 0.452333331, 0.5081833
def c13, 0.520524979, 0.452808321, 0.754475176, 0.656323254
def c14, -0.111783333, -0.787400007, -0.140556395, -0.990076959
def c15, 0.323099971, -0.362291664, 0.665579438, -0.746313453
def c16, 0.101049997, -0.160683334, 0.532355785, -0.846518576
def c17, -0.1699, 0.0927750021, -0.877658129, 0.479250968
def c18, 0.692616642, -0.243483335, 0.943405211, -0.331645846
def c19, 0.0896583274, -0.481991649, 0.182879776, -0.983138144
def c20, 0.235708326, 0.658633292, 0.336946815, 0.941521168
def c21, -0.305183321, 0.329299986, -0.679746091, 0.733461976
def c22, 0.629924953, 0.18263334, 0.960446775, 0.278461099
def c23, -0.43841663, 0.0198833328, -0.998974562, 0.0453060903
def c24, -0.73513335, -0.203924999, -0.963614345, -0.267305315
def c25, -0.407400012, -0.244416669, -0.857518733, -0.514462113
def c26, 0.366783321, -0.0713166669, 0.981622756, -0.190864891
def c27, 0.176491663, 0.428108305, 0.381143451, 0.924523473
def c28, 0.0456916653, 0.797508299, 0.0571991876, 0.998362064
def c29, -0.491674989, -0.587574959, -0.641747653, -0.766919017
def c30, -0.667208314, 0.205258325, -0.955794573, 0.294038296
def c31, -0.161866665, -0.153349996, -0.725940943, -0.687745273
def c32, -0.363891661, 0.639591634, -0.494507611, 0.869167864
def c33, 0.180549994, 0.0966833308, 0.881555855, 0.472067386
def c34, 0.130799994, -0.713333309, 0.180357814, -0.983602822
def c35, -0.63184166, 0.486358345, -0.792426944, 0.609968424
def c36, 0.82736665, -0.0241999999, 0.999567091, -0.0292367637
def c37, -0.185283333, -0.48255831, -0.358445257, -0.933547199
def c38, 0.458766669, -0.558199942, 0.634941936, -0.772559524
def c39, 0.386924982, 0.234291673, 0.85539788, 0.517962396
def c40, -0.0601166673, 0.504616618, -0.118297197, 0.992981434
dcl_texcoord1 v0.xy
dcl_2d s0
texld r0, v0, s0
mul r1, r0.w, c0.xyxy
mul r1, r1, c1.w
mad r2, r1.zwzw, c13, v0.xyxy
texld r3, r2, s0
texld r2, r2.zwzw, s0
mov r4.x, r3.w
mov r4.y, r2.w
mad r4.xy, r0.w, -c2.yzzw, r4
add r4.xy, r4, c2.w
mul_sat r4.xy, r4, c8.x
mad r4.zw, r4.xyxy, c8.y, c8.z
mul r4.xy, r4, r4
mul r4.xy, r4, r4.zwzw
mul r2.xyz, r2, r4.y
add r2.w, r2.w, r3.w
mad r2.xyz, r3, r4.x, r2
mul r3.x, r0.w, c2.x
max r5.w, r3.x, c1.z
mad r3.xyz, r0, r5.w, r2
dp2add r3.w, r4, c2.z, r5.w
mul r5.xyz, r0, r5.w
cmp r2, -r2.w, r5, r3
mad r3, r1.zwzw, c14, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c3, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c15, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c3.zyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c16, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c3.wyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c17, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c4, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c18, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c4.zyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c19, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c4.wyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c20, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c5, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c21, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c5.zyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c22, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c5.wyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c23, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c6, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c24, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c6.zyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c25, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c6.wyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c26, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c7, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c27, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c7.zyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c28, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c7.wyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c29, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c9, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c30, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c9.zyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c31, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c9.wyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c32, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c10, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c33, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c10.zyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c34, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c10.wyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c35, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c11, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c36, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c11.zyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c37, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c11.wyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c38, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c12, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
mad r3, r1.zwzw, c39, v0.xyxy
mad r1, r1, c40, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mov r0.x, r4.w
mov r0.y, r3.w
mad r0.xy, r0.w, -c12.zyzw, r0
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r5.xy, r0, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r5
mul r3.xyz, r0.y, r3
add r0.z, r3.w, r4.w
mad r3.xyz, r4, r0.x, r3
dp2add r4.w, r0, c2.z, r2.w
add r4.xyz, r2, r3
cmp r2, -r0.z, r2, r4
texld r3, r1, s0
texld r1, r1.zwzw, s0
mov r0.x, r3.w
mov r0.y, r1.w
mad r0.xy, r0.w, -c12.wyzw, r0
mov oC0.w, r0.w
add r0.xy, r0, c2.w
mul_sat r0.xy, r0, c8.x
mad r0.zw, r0.xyxy, c8.y, c8.z
mul r0.xy, r0, r0
mul r0.xy, r0, r0.zwzw
mul r1.xyz, r0.y, r1
add r0.z, r1.w, r3.w
mad r1.xyz, r3, r0.x, r1
dp2add r3.w, r0, c2.z, r2.w
add r3.xyz, r1, r2
cmp r0, -r0.z, r2, r3
rcp r0.w, r0.w
mul oC0.xyz, r0.w, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 25 math, 3 textures, 2 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
Vector 128 [_Offsets]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedhckoeoimcaamjbicepbgojnlefnoddhpabaaaaaagaahaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefciiagaaaaeaaaaaaakcabaaaa
dfbiaaaahcaaaaaamaohbpdpembkaldpebpbfddpaaaaaaaappflajlokiodhblp
eifahedpaaaaaaaadmidmgdoinjhnololacabfdpaaaaaaaackfhpidnjlhceflo
cjdpgjdoaaaaaaaaolmffalopmaaoednanoagndoaaaaaaaajhmffedpjojijflo
dhijgbdpaaaaaaaacffinmdnfobbbelphmjlbgdpaaaaaaaalhnbjadoaiffekdp
dcogfgdpaaaaaaaadaiblllogjfcmkdocnomajdpaaaaaaaafbidebdphlglgado
hehlejdpaaaaaaaahnkoaglpamhgmddmobnbagdpaaaaaaaadnnfgblpecjfhklo
dofmgkdpaaaaaaaahleopkloglcljglollpcbbdpaaaaaaaaaifkobdojbeekpln
bajcofdoaaaaaaaahknpfidompidaddpgeeaaodpaaaaaaaagmjfgadnjlpohedp
iagfhfdpaaaaaaaaofakbhlpmhiadelphnfmgldpaaaaaaaaggphemlpladihmdo
dchcfgdpaaaaaaaankogeglolngpdmlooppoiidoaaaaaaaadgjdnploijhleedp
ojaogcdpaaaaaaaabonmfndoofjlondnccklhldoaaaaaaaabplkcadonbccfllp
aemkfodpaaaaaaaaanbkeclpmhgibfdpcipchedpaaaaaaaamdckhodpelofonlm
pbeghodpaaaaaaaabjkngdlooodnbelpfdmlbodpaaaaaaaaobooamdpkchkcllp
ffpgfndpaaaaaaaaaklkondoofpcipdopbpeakdpaaaaaaaacdlojdlnklaebldp
ckbnbmdpaaaaaaaafjaaaaaeegiocaaaaaaaaaaaajaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaadmcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacaiaaaaaaefaaaaajpcaabaaaaaaaaaaa
ogbkbaaaabaaaaaamghjbaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaaipcaabaaa
abaaaaaafgafbaaaaaaaaaaaegiecaaaaaaaaaaaahaaaaaadiaaaaaipcaabaaa
abaaaaaaegaobaaaabaaaaaapgipcaaaaaaaaaaaaiaaaaaadiaaaaahbcaabaaa
acaaaaaabkaabaaaaaaaaaaaabeaaaaaaaaaiadodeaaaaaibcaabaaaacaaaaaa
akaabaaaacaaaaaackiacaaaaaaaaaaaaiaaaaaadiaaaaahocaabaaaacaaaaaa
agaobaaaaaaaaaaaagaabaaaacaaaaaadgaaaaafbcaabaaaadaaaaaaabeaaaaa
jkjjjjdpdgaaaaafhcaabaaaaeaaaaaajgahbaaaacaaaaaadgaaaaafecaabaaa
aaaaaaaaakaabaaaacaaaaaadgaaaaaficaabaaaaaaaaaaaabeaaaaaaaaaaaaa
daaaaaabcbaaaaahccaabaaaadaaaaaadkaabaaaaaaaaaaaabeaaaaabmaaaaaa
adaaaeadbkaabaaaadaaaaaadiaaaaaipcaabaaaafaaaaaaegaobaaaabaaaaaa
egjejaaadkaabaaaaaaaaaaadgaaaaagecaabaaaadaaaaaackjajaaadkaabaaa
aaaaaaaaaoaaaaahpcaabaaaafaaaaaaegaobaaaafaaaaaaagakbaaaadaaaaaa
aaaaaaahpcaabaaaafaaaaaaegaobaaaafaaaaaaogbobaaaabaaaaaaefaaaaaj
pcaabaaaagaaaaaaegaabaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
efaaaaajpcaabaaaafaaaaaaogakbaaaafaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaaaaaaaahccaabaaaadaaaaaadkaabaaaafaaaaaadkaabaaaagaaaaaa
dbaaaaahccaabaaaadaaaaaaabeaaaaaaaaaaaaabkaabaaaadaaaaaabpaaaead
bkaabaaaadaaaaaadiaaaaaibcaabaaaaaaaaaaabkaabaaaaaaaaaaackjajaaa
dkaabaaaaaaaaaaadgaaaaafbcaabaaaahaaaaaadkaabaaaagaaaaaadgaaaaaf
ccaabaaaahaaaaaadkaabaaaafaaaaaadcaaaaangcaabaaaadaaaaaaagabbaia
ebaaaaaaaaaaaaaaaceaaaaaaaaaaaaaffffffdpaaaaiadpaaaaaaaaagabbaaa
ahaaaaaaaaaaaaakgcaabaaaadaaaaaafgagbaaaadaaaaaaaceaaaaaaaaaaaaa
bekoihdobekoihdoaaaaaaaadicaaaakgcaabaaaadaaaaaafgagbaaaadaaaaaa
aceaaaaaaaaaaaaaglichbeaglichbeaaaaaaaaadcaaaaapdcaabaaaahaaaaaa
jgafbaaaadaaaaaaaceaaaaaaaaaaamaaaaaaamaaaaaaaaaaaaaaaaaaceaaaaa
aaaaeaeaaaaaeaeaaaaaaaaaaaaaaaaadiaaaaahgcaabaaaadaaaaaafgagbaaa
adaaaaaafgagbaaaadaaaaaadiaaaaahgcaabaaaadaaaaaafgagbaaaadaaaaaa
agabbaaaahaaaaaadiaaaaahhcaabaaaafaaaaaakgakbaaaadaaaaaaegacbaaa
afaaaaaadcaaaaajhcaabaaaafaaaaaaegacbaaaagaaaaaafgafbaaaadaaaaaa
egacbaaaafaaaaaaaaaaaaahhcaabaaaaeaaaaaaegacbaaaaeaaaaaaegacbaaa
afaaaaaaapaaaaakbcaabaaaaaaaaaaajgafbaaaadaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaaaaaaaaaaaaaaaaaaaahecaabaaaaaaaaaaaakaabaaaaaaaaaaa
ckaabaaaaaaaaaaabfaaaaabboaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaa
abeaaaaaabaaaaaabgaaaaabaoaaaaahhccabaaaaaaaaaaaegacbaaaaeaaaaaa
kgakbaaaaaaaaaaadgaaaaaficcabaaaaaaaaaaabkaabaaaaaaaaaaadoaaaaab
""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 61 math, 3 texture, 3 branch
 // Stats for Fragment shader:
 //       d3d11 : 26 math, 3 texture, 1 branch
 //        d3d9 : 255 math, 30 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 1239289
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 61 math, 3 textures, 3 branches
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform sampler2D _LowRez;
uniform vec4 _MainTex_TexelSize;
uniform vec4 _Offsets;
vec3 DiscKernel[28];
varying vec2 xlv_TEXCOORD1;
void main ()
{
  DiscKernel[0] = vec3(0.62463, 0.54337, 0.8279);
  DiscKernel[1] = vec3(-0.13414, -0.94488, 0.95435);
  DiscKernel[2] = vec3(0.38772, -0.43475, 0.58253);
  DiscKernel[3] = vec3(0.12126, -0.19282, 0.22778);
  DiscKernel[4] = vec3(-0.20388, 0.11133, 0.2323);
  DiscKernel[5] = vec3(0.83114, -0.29218, 0.881);
  DiscKernel[6] = vec3(0.10759, -0.57839, 0.58831);
  DiscKernel[7] = vec3(0.28285, 0.79036, 0.83945);
  DiscKernel[8] = vec3(-0.36622, 0.39516, 0.53876);
  DiscKernel[9] = vec3(0.75591, 0.21916, 0.78704);
  DiscKernel[10] = vec3(-0.5261, 0.02386, 0.52664);
  DiscKernel[11] = vec3(-0.88216, -0.24471, 0.91547);
  DiscKernel[12] = vec3(-0.48888, -0.2933, 0.57011);
  DiscKernel[13] = vec3(0.44014, -0.08558, 0.44838);
  DiscKernel[14] = vec3(0.21179, 0.51373, 0.55567);
  DiscKernel[15] = vec3(0.05483, 0.95701, 0.95858);
  DiscKernel[16] = vec3(-0.59001, -0.70509, 0.91938);
  DiscKernel[17] = vec3(-0.80065, 0.24631, 0.83768);
  DiscKernel[18] = vec3(-0.19424, -0.18402, 0.26757);
  DiscKernel[19] = vec3(-0.43667, 0.76751, 0.88304);
  DiscKernel[20] = vec3(0.21666, 0.11602, 0.24577);
  DiscKernel[21] = vec3(0.15696, -0.856, 0.87027);
  DiscKernel[22] = vec3(-0.75821, 0.58363, 0.95682);
  DiscKernel[23] = vec3(0.99284, -0.02904, 0.99327);
  DiscKernel[24] = vec3(-0.22234, -0.57907, 0.62029);
  DiscKernel[25] = vec3(0.55052, -0.66984, 0.86704);
  DiscKernel[26] = vec3(0.46431, 0.28115, 0.5428);
  DiscKernel[27] = vec3(-0.07214, 0.60554, 0.60982);
  vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  float sampleCount_3;
  vec4 poissonScale_4;
  vec4 smallBlur_5;
  vec4 centerTap_6;
  vec4 tmpvar_7;
  tmpvar_7 = texture2D (_LowRez, xlv_TEXCOORD1);
  vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, xlv_TEXCOORD1);
  centerTap_6 = tmpvar_8;
  poissonScale_4 = ((_MainTex_TexelSize.xyxy * tmpvar_8.w) * _Offsets.w);
  float tmpvar_9;
  tmpvar_9 = max ((tmpvar_8.w * 0.25), 0.1);
  sampleCount_3 = tmpvar_9;
  smallBlur_5 = (tmpvar_8 * tmpvar_9);
  for (int l_2 = 0; l_2 < 28; l_2++) {
    vec4 tmpvar_10;
    tmpvar_10 = texture2D (_MainTex, (tmpvar_1 + (DiscKernel[l_2].xy * poissonScale_4.xy)));
    float tmpvar_11;
    float tmpvar_12;
    tmpvar_12 = clamp (((
      (tmpvar_10.w - (centerTap_6.w * DiscKernel[l_2].z))
     - -0.265) / 0.265), 0.0, 1.0);
    tmpvar_11 = (tmpvar_12 * (tmpvar_12 * (3.0 - 
      (2.0 * tmpvar_12)
    )));
    smallBlur_5 = (smallBlur_5 + (tmpvar_10 * tmpvar_11));
    sampleCount_3 = (sampleCount_3 + tmpvar_11);
  };
  float tmpvar_13;
  tmpvar_13 = clamp (((tmpvar_8.w - 0.65) / 0.2), 0.0, 1.0);
  vec4 tmpvar_14;
  tmpvar_14 = mix ((smallBlur_5 / (sampleCount_3 + 1e-05)), tmpvar_7, vec4((tmpvar_13 * (tmpvar_13 * 
    (3.0 - (2.0 * tmpvar_13))
  ))));
  smallBlur_5 = tmpvar_14;
  vec4 tmpvar_15;
  if ((tmpvar_8.w < 0.01)) {
    tmpvar_15 = tmpvar_8;
  } else {
    vec4 tmpvar_16;
    tmpvar_16.xyz = tmpvar_14.xyz;
    tmpvar_16.w = tmpvar_8.w;
    tmpvar_15 = tmpvar_16;
  };
  gl_FragData[0] = tmpvar_15;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 255 math, 30 textures
Vector 0 [_MainTex_TexelSize]
Vector 1 [_Offsets]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_LowRez] 2D 1
""ps_3_0
def c2, 0.25, 0.100000001, 0.827899992, 0.264999986
def c3, 3.77358508, -2, 3, 0.954349995
def c4, 0.582530022, 0.227779999, 0.232299998, 0.880999982
def c5, 0.588310003, 0.839450002, 0.538760006, 0.787039995
def c6, 0.526639998, 0.915470004, 0.570110023, 0.448379993
def c7, 0.555670023, 0.958580017, 0.919380009, 0.837679982
def c8, 0.267569989, 0.883040011, 0.245770007, 0.870270014
def c9, 0.956820011, 0.99326998, 0.620289981, 0.867039979
def c10, 0.542800009, 0.609820008, 9.99999975e-006, 4.99999905
def c11, -0.649999976, -0.00999999978, 0, 0
def c12, 0.46430999, 0.281150013, -0.0721400008, 0.605539978
def c13, -0.222340003, -0.579069972, 0.550520003, -0.669839978
def c14, -0.758210003, 0.583630025, 0.992839992, -0.0290399995
def c15, 0.216659993, 0.116020001, 0.156959996, -0.856000006
def c16, -0.194240004, -0.184019998, -0.436670005, 0.767509997
def c17, -0.590009987, -0.705089986, -0.800650001, 0.246309996
def c18, 0.211789995, 0.51372999, 0.0548299998, 0.957009971
def c19, -0.488880008, -0.293300003, 0.440140009, -0.0855799988
def c20, -0.52609998, 0.0238600001, -0.882160008, -0.244709998
def c21, -0.366219997, 0.39515999, 0.755909979, 0.219160005
def c22, 0.107589997, -0.578390002, 0.282849997, 0.790359974
def c23, -0.203879997, 0.111330003, 0.831139982, -0.292180002
def c24, 0.387719989, -0.434749991, 0.121260002, -0.192819998
def c25, 0.624629974, 0.543370008, -0.13414, -0.944880009
dcl_texcoord1 v0.xy
dcl_2d s0
dcl_2d s1
texld r0, v0, s0
mul r1, r0.w, c0.xyxy
mul r1, r1, c1.w
mad r2, r1.zwzw, c25, v0.xyxy
texld r3, r2, s0
texld r2, r2.zwzw, s0
mad r3.w, r0.w, -c2.z, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mul r3.xyz, r3, r4.y
mul r4.y, r0.w, c2.x
max r5.x, r4.y, c2.y
mad r3.xyz, r0, r5.x, r3
mad r3.w, r4.x, r3.w, r5.x
mad r2.w, r0.w, -c3.w, r2.w
add r2.w, r2.w, c2.w
mul_sat r2.w, r2.w, c3.x
mad r4.x, r2.w, c3.y, c3.z
mul r2.w, r2.w, r2.w
mul r4.y, r2.w, r4.x
mad r2.w, r4.x, r2.w, r3.w
mad r2.xyz, r2, r4.y, r3
mad r3, r1.zwzw, c24, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c4.x, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c4.y, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c23, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c4.z, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c4.w, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c22, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c5.x, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c5.y, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c21, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c5.z, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c5.w, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c20, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c6.x, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c6.y, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c19, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c6.z, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c6.w, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c18, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c7.x, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c7.y, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c17, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c7.z, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c7.w, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c16, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c8.x, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c8.y, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c15, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c8.z, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c8.w, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c14, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c9.x, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c9.y, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r3, r1.zwzw, c13, v0.xyxy
mad r1, r1, c12, v0.xyxy
texld r4, r3, s0
texld r3, r3.zwzw, s0
mad r4.w, r0.w, -c9.z, r4.w
add r4.w, r4.w, c2.w
mul_sat r4.w, r4.w, c3.x
mad r5.x, r4.w, c3.y, c3.z
mul r4.w, r4.w, r4.w
mul r5.y, r4.w, r5.x
mad r2.w, r5.x, r4.w, r2.w
mad r2.xyz, r4, r5.y, r2
mad r3.w, r0.w, -c9.w, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
texld r3, r1, s0
texld r1, r1.zwzw, s0
mad r3.w, r0.w, -c10.x, r3.w
add r3.w, r3.w, c2.w
mul_sat r3.w, r3.w, c3.x
mad r4.x, r3.w, c3.y, c3.z
mul r3.w, r3.w, r3.w
mul r4.y, r3.w, r4.x
mad r2.w, r4.x, r3.w, r2.w
mad r2.xyz, r3, r4.y, r2
mad r1.w, r0.w, -c10.y, r1.w
add r1.w, r1.w, c2.w
mul_sat r1.w, r1.w, c3.x
mad r3.x, r1.w, c3.y, c3.z
mul r1.w, r1.w, r1.w
mul r3.y, r1.w, r3.x
mad r1.w, r3.x, r1.w, r2.w
add r1.w, r1.w, c10.z
rcp r1.w, r1.w
mad r1.xyz, r1, r3.y, r2
texld r2, v0, s1
mad r2.xyz, r1, -r1.w, r2
mul r1.xyz, r1.w, r1
add r3.xy, r0.w, c11
mul_sat r1.w, r3.x, c10.w
mad r2.w, r1.w, c3.y, c3.z
mul r1.w, r1.w, r1.w
mul r1.w, r1.w, r2.w
mad r1.xyz, r1.w, r2, r1
mov r1.w, r0.w
cmp oC0, r3.y, r1, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 26 math, 3 textures, 1 branches
SetTexture 0 [_LowRez] 2D 1
SetTexture 1 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
Vector 128 [_Offsets]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedddalpbffjdgliiijlldblllmhbnenlbcabaaaaaadaahaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcfiagaaaaeaaaaaaajgabaaaa
dfbiaaaahcaaaaaamaohbpdpembkaldpebpbfddpaaaaaaaappflajlokiodhblp
eifahedpaaaaaaaadmidmgdoinjhnololacabfdpaaaaaaaackfhpidnjlhceflo
cjdpgjdoaaaaaaaaolmffalopmaaoednanoagndoaaaaaaaajhmffedpjojijflo
dhijgbdpaaaaaaaacffinmdnfobbbelphmjlbgdpaaaaaaaalhnbjadoaiffekdp
dcogfgdpaaaaaaaadaiblllogjfcmkdocnomajdpaaaaaaaafbidebdphlglgado
hehlejdpaaaaaaaahnkoaglpamhgmddmobnbagdpaaaaaaaadnnfgblpecjfhklo
dofmgkdpaaaaaaaahleopkloglcljglollpcbbdpaaaaaaaaaifkobdojbeekpln
bajcofdoaaaaaaaahknpfidompidaddpgeeaaodpaaaaaaaagmjfgadnjlpohedp
iagfhfdpaaaaaaaaofakbhlpmhiadelphnfmgldpaaaaaaaaggphemlpladihmdo
dchcfgdpaaaaaaaankogeglolngpdmlooppoiidoaaaaaaaadgjdnploijhleedp
ojaogcdpaaaaaaaabonmfndoofjlondnccklhldoaaaaaaaabplkcadonbccfllp
aemkfodpaaaaaaaaanbkeclpmhgibfdpcipchedpaaaaaaaamdckhodpelofonlm
pbeghodpaaaaaaaabjkngdlooodnbelpfdmlbodpaaaaaaaaobooamdpkchkcllp
ffpgfndpaaaaaaaaaklkondoofpcipdopbpeakdpaaaaaaaacdlojdlnklaebldp
ckbnbmdpaaaaaaaafjaaaaaeegiocaaaaaaaaaaaajaaaaaafkaaaaadaagabaaa
aaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaa
fibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaadmcbabaaaabaaaaaagfaaaaad
pccabaaaaaaaaaaagiaaaaacagaaaaaaefaaaaajpcaabaaaaaaaaaaaogbkbaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaabaaaaaaefaaaaajpcaabaaaabaaaaaa
ogbkbaaaabaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadiaaaaaidcaabaaa
acaaaaaapgapbaaaabaaaaaaegiacaaaaaaaaaaaahaaaaaadiaaaaaidcaabaaa
acaaaaaaegaabaaaacaaaaaapgipcaaaaaaaaaaaaiaaaaaadiaaaaahicaabaaa
aaaaaaaadkaabaaaabaaaaaaabeaaaaaaaaaiadodeaaaaahicaabaaaaaaaaaaa
dkaabaaaaaaaaaaaabeaaaaamnmmmmdndiaaaaahhcaabaaaadaaaaaapgapbaaa
aaaaaaaaegacbaaaabaaaaaadgaaaaafhcaabaaaaeaaaaaaegacbaaaadaaaaaa
dgaaaaafecaabaaaacaaaaaadkaabaaaaaaaaaaadgaaaaaficaabaaaacaaaaaa
abeaaaaaaaaaaaaadaaaaaabcbaaaaahicaabaaaadaaaaaadkaabaaaacaaaaaa
abeaaaaabmaaaaaaadaaaeaddkaabaaaadaaaaaadcaaaaakdcaabaaaafaaaaaa
egjajaaadkaabaaaacaaaaaaegaabaaaacaaaaaaogbkbaaaabaaaaaaefaaaaaj
pcaabaaaafaaaaaaegaabaaaafaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaa
dcaaaaalicaabaaaadaaaaaadkaabaiaebaaaaaaabaaaaaackjajaaadkaabaaa
acaaaaaadkaabaaaafaaaaaaaaaaaaahicaabaaaadaaaaaadkaabaaaadaaaaaa
abeaaaaabekoihdodicaaaahicaabaaaadaaaaaadkaabaaaadaaaaaaabeaaaaa
glichbeadcaaaaajicaabaaaaeaaaaaadkaabaaaadaaaaaaabeaaaaaaaaaaama
abeaaaaaaaaaeaeadiaaaaahicaabaaaadaaaaaadkaabaaaadaaaaaadkaabaaa
adaaaaaadiaaaaahicaabaaaafaaaaaadkaabaaaadaaaaaadkaabaaaaeaaaaaa
dcaaaaajhcaabaaaaeaaaaaaegacbaaaafaaaaaapgapbaaaafaaaaaaegacbaaa
aeaaaaaadcaaaaajecaabaaaacaaaaaadkaabaaaaeaaaaaadkaabaaaadaaaaaa
ckaabaaaacaaaaaaboaaaaahicaabaaaacaaaaaadkaabaaaacaaaaaaabeaaaaa
abaaaaaabgaaaaabaaaaaaahicaabaaaaaaaaaaackaabaaaacaaaaaaabeaaaaa
kmmfchdhaoaaaaahhcaabaaaacaaaaaaegacbaaaaeaaaaaapgapbaaaaaaaaaaa
aaaaaaahicaabaaaaaaaaaaadkaabaaaabaaaaaaabeaaaaaggggcglpdicaaaah
icaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaapoppjpeadcaaaaajicaabaaa
acaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaeadiaaaaah
icaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaaaaaaaaaadiaaaaahicaabaaa
aaaaaaaadkaabaaaaaaaaaaadkaabaaaacaaaaaaaaaaaaaihcaabaaaaaaaaaaa
egacbaaaaaaaaaaaegacbaiaebaaaaaaacaaaaaadcaaaaajhcaabaaaaaaaaaaa
pgapbaaaaaaaaaaaegacbaaaaaaaaaaaegacbaaaacaaaaaadbaaaaahbcaabaaa
acaaaaaadkaabaaaabaaaaaaabeaaaaaaknhcddmdgaaaaaficaabaaaaaaaaaaa
dkaabaaaabaaaaaadhaaaaajpccabaaaaaaaaaaaagaabaaaacaaaaaaegaobaaa
abaaaaaaegaobaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 11 math
 //        d3d9 : 14 math
 //      opengl : 64 math, 11 texture, 1 branch
 // Stats for Fragment shader:
 //       d3d11 : 44 math, 11 texture
 //        d3d9 : 47 math, 11 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 1266660
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 64 math, 11 textures, 1 branches
""!!GLSL
#ifdef VERTEX

uniform vec4 _MainTex_TexelSize;
uniform vec4 _Offsets;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
varying vec4 xlv_TEXCOORD4;
varying vec4 xlv_TEXCOORD5;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD1 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(1.0, 1.0, -1.0, -1.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
  xlv_TEXCOORD2 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(2.0, 2.0, -2.0, -2.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
  xlv_TEXCOORD3 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(3.0, 3.0, -3.0, -3.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
  xlv_TEXCOORD4 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(4.0, 4.0, -4.0, -4.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
  xlv_TEXCOORD5 = (gl_MultiTexCoord0.xyxy + ((
    (_Offsets.xyxy * vec4(5.0, 5.0, -5.0, -5.0))
   * _MainTex_TexelSize.xyxy) / 6.0));
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
varying vec4 xlv_TEXCOORD4;
varying vec4 xlv_TEXCOORD5;
void main ()
{
  vec4 sum_1;
  vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD1.xy);
  vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD1.zw);
  vec4 tmpvar_5;
  tmpvar_5 = texture2D (_MainTex, xlv_TEXCOORD2.xy);
  vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, xlv_TEXCOORD2.zw);
  vec4 tmpvar_7;
  tmpvar_7 = texture2D (_MainTex, xlv_TEXCOORD3.xy);
  vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, xlv_TEXCOORD3.zw);
  vec4 tmpvar_9;
  tmpvar_9 = texture2D (_MainTex, xlv_TEXCOORD4.xy);
  vec4 tmpvar_10;
  tmpvar_10 = texture2D (_MainTex, xlv_TEXCOORD4.zw);
  vec4 tmpvar_11;
  tmpvar_11 = texture2D (_MainTex, xlv_TEXCOORD5.xy);
  vec4 tmpvar_12;
  tmpvar_12 = texture2D (_MainTex, xlv_TEXCOORD5.zw);
  float tmpvar_13;
  tmpvar_13 = (clamp ((2.0 * tmpvar_3.w), 0.0, 1.0) * 0.8);
  float tmpvar_14;
  tmpvar_14 = (clamp ((2.0 * tmpvar_4.w), 0.0, 1.0) * 0.8);
  float tmpvar_15;
  tmpvar_15 = (clamp ((2.0 * tmpvar_5.w), 0.0, 1.0) * 0.675);
  float tmpvar_16;
  tmpvar_16 = (clamp ((2.0 * tmpvar_6.w), 0.0, 1.0) * 0.675);
  float tmpvar_17;
  tmpvar_17 = (clamp ((2.0 * tmpvar_7.w), 0.0, 1.0) * 0.5);
  float tmpvar_18;
  tmpvar_18 = (clamp ((2.0 * tmpvar_8.w), 0.0, 1.0) * 0.5);
  float tmpvar_19;
  tmpvar_19 = (clamp ((2.0 * tmpvar_9.w), 0.0, 1.0) * 0.2);
  float tmpvar_20;
  tmpvar_20 = (clamp ((2.0 * tmpvar_10.w), 0.0, 1.0) * 0.2);
  float tmpvar_21;
  tmpvar_21 = (clamp ((2.0 * tmpvar_11.w), 0.0, 1.0) * 0.075);
  float tmpvar_22;
  tmpvar_22 = (clamp ((2.0 * tmpvar_12.w), 0.0, 1.0) * 0.075);
  sum_1.xyz = (((
    ((((
      ((((
        (tmpvar_2 * tmpvar_2.w)
       + 
        (tmpvar_3 * tmpvar_13)
      ) + (tmpvar_4 * tmpvar_14)) + (tmpvar_5 * tmpvar_15)) + (tmpvar_6 * tmpvar_16))
     + 
      (tmpvar_7 * tmpvar_17)
    ) + (tmpvar_8 * tmpvar_18)) + (tmpvar_9 * tmpvar_19)) + (tmpvar_10 * tmpvar_20))
   + 
    (tmpvar_11 * tmpvar_21)
  ) + (tmpvar_12 * tmpvar_22)) / ((
    ((((
      ((((
        (tmpvar_2.w + tmpvar_13)
       + tmpvar_14) + tmpvar_15) + tmpvar_16) + tmpvar_17)
     + tmpvar_18) + tmpvar_19) + tmpvar_20) + tmpvar_21)
   + tmpvar_22) + 0.0001)).xyz;
  sum_1.w = tmpvar_2.w;
  if ((tmpvar_2.w < 0.01)) {
    sum_1.xyz = tmpvar_2.xyz;
  };
  gl_FragData[0] = sum_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 14 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
Vector 5 [_Offsets]
""vs_3_0
def c6, 0, 1, 0.166666672, -0.166666672
def c7, 0.333333343, -0.333333343, 0.5, -0.5
def c8, 0.666666687, -0.666666687, 0.833333373, -0.833333373
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2
dcl_texcoord2 o3
dcl_texcoord3 o4
dcl_texcoord4 o5
dcl_texcoord5 o6
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.xy, c6
mad r0, c4.xxxy, r0.xxyy, r0.yyxx
mul r0, r0, c5.xyxy
mul r0.xy, r0, c4
mad o2, r0, c6.zzww, v1.xyxy
mad o3, r0.zwzw, c7.xxyy, v1.xyxy
mad o4, r0.zwzw, c7.zzww, v1.xyxy
mad o5, r0.zwzw, c8.xxyy, v1.xyxy
mad o6, r0.zwzw, c8.zzww, v1.xyxy
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 11 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
Vector 128 [_Offsets]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedmiljhlgnmpdjagnaapgfpimdaneppfogabaaaaaaaeaeaaaaadaaaaaa
cmaaaaaaiaaaaaaafaabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheomiaaaaaaahaaaaaaaiaaaaaalaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaalmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
lmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapaaaaaalmaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaapaaaaaalmaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaapaaaaaalmaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaaapaaaaaa
lmaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaaapaaaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefckmacaaaaeaaaabaaklaaaaaa
fjaaaaaeegiocaaaaaaaaaaaajaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadpccabaaaacaaaaaa
gfaaaaadpccabaaaadaaaaaagfaaaaadpccabaaaaeaaaaaagfaaaaadpccabaaa
afaaaaaagfaaaaadpccabaaaagaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaa
abaaaaaadgaaaaafbcaabaaaaaaaaaaaabeaaaaaaaaaiadpdgaaaaagmcaabaaa
aaaaaaaaagiecaaaaaaaaaaaahaaaaaadiaaaaaipcaabaaaaaaaaaaaagaobaaa
aaaaaaaaegiecaaaaaaaaaaaaiaaaaaadiaaaaaidcaabaaaaaaaaaaaegaabaaa
aaaaaaaaegiacaaaaaaaaaaaahaaaaaadcaaaaampccabaaaacaaaaaaegaobaaa
aaaaaaaaaceaaaaaklkkckdoklkkckdoklkkckloklkkckloegbebaaaabaaaaaa
dcaaaaampccabaaaadaaaaaaogaobaaaaaaaaaaaaceaaaaaklkkkkdoklkkkkdo
klkkkkloklkkkkloegbebaaaabaaaaaadcaaaaampccabaaaaeaaaaaaogaobaaa
aaaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaalpaaaaaalpegbebaaaabaaaaaa
dcaaaaampccabaaaafaaaaaaogaobaaaaaaaaaaaaceaaaaaklkkckdpklkkckdp
klkkcklpklkkcklpegbebaaaabaaaaaadcaaaaampccabaaaagaaaaaaogaobaaa
aaaaaaaaaceaaaaafgffffdpfgffffdpfgfffflpfgfffflpegbebaaaabaaaaaa
doaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 47 math, 11 textures
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c0, 0.800000012, 0.675000012, 0.5, 0.200000003
def c1, 0.075000003, 9.99999975e-005, -0.00999999978, 0
dcl_texcoord v0.xy
dcl_texcoord1 v1
dcl_texcoord2 v2
dcl_texcoord3 v3
dcl_texcoord4 v4
dcl_texcoord5 v5
dcl_2d s0
texld r0, v1.zwzw, s0
add_sat r0.w, r0.w, r0.w
mul r1.x, r0.w, c0.x
texld r2, v1, s0
add_sat r1.y, r2.w, r2.w
mul r1.z, r1.y, c0.x
mul r2.xyz, r1.z, r2
texld r3, v0, s0
mad r2.xyz, r3, r3.w, r2
mad r0.xyz, r0, r1.x, r2
texld r2, v2, s0
add_sat r1.x, r2.w, r2.w
mul r1.z, r1.x, c0.y
mad r0.xyz, r2, r1.z, r0
texld r2, v2.zwzw, s0
add_sat r1.z, r2.w, r2.w
mul r1.w, r1.z, c0.y
mad r0.xyz, r2, r1.w, r0
texld r2, v3, s0
add_sat r1.w, r2.w, r2.w
mul r2.w, r1.w, c0.z
mad r0.xyz, r2, r2.w, r0
texld r2, v3.zwzw, s0
add_sat r2.w, r2.w, r2.w
mul r4.x, r2.w, c0.z
mad r0.xyz, r2, r4.x, r0
texld r4, v4, s0
add_sat r2.x, r4.w, r4.w
mul r2.y, r2.x, c0.w
mad r0.xyz, r4, r2.y, r0
texld r4, v4.zwzw, s0
add_sat r2.y, r4.w, r4.w
mul r2.z, r2.y, c0.w
mad r0.xyz, r4, r2.z, r0
texld r4, v5, s0
add_sat r2.z, r4.w, r4.w
mul r4.w, r2.z, c1.x
mad r0.xyz, r4, r4.w, r0
texld r4, v5.zwzw, s0
add_sat r4.w, r4.w, r4.w
mul r5.x, r4.w, c1.x
mad r0.xyz, r4, r5.x, r0
mad r1.y, r1.y, c0.x, r3.w
mad r0.w, r0.w, c0.x, r1.y
mad r0.w, r1.x, c0.y, r0.w
mad r0.w, r1.z, c0.y, r0.w
mad r0.w, r1.w, c0.z, r0.w
mad r0.w, r2.w, c0.z, r0.w
mad r0.w, r2.x, c0.w, r0.w
mad r0.w, r2.y, c0.w, r0.w
mad r0.w, r2.z, c1.x, r0.w
mad r0.w, r4.w, c1.x, r0.w
add r0.w, r0.w, c1.y
rcp r0.w, r0.w
mul r0.xyz, r0.w, r0
add r0.w, r3.w, c1.z
cmp oC0.xyz, r0.w, r0, r3
mov oC0.w, r3.w

""
}
SubProgram ""d3d11 "" {
// Stats: 44 math, 11 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedncmbfcdcnkhkojhiemehbliehenchnifabaaaaaapaaiaaaaadaaaaaa
cmaaaaaapmaaaaaadaabaaaaejfdeheomiaaaaaaahaaaaaaaiaaaaaalaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaalmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaalmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaalmaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaapapaaaalmaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapapaaaalmaaaaaaaeaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapapaaaalmaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaa
apapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcliahaaaaeaaaaaaaooabaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaagcbaaaadpcbabaaaadaaaaaa
gcbaaaadpcbabaaaaeaaaaaagcbaaaadpcbabaaaafaaaaaagcbaaaadpcbabaaa
agaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacagaaaaaaefaaaaajpcaabaaa
aaaaaaaaogbkbaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaacaaaah
icaabaaaaaaaaaaadkaabaaaaaaaaaaadkaabaaaaaaaaaaadiaaaaahbcaabaaa
abaaaaaadkaabaaaaaaaaaaaabeaaaaamnmmemdpefaaaaajpcaabaaaacaaaaaa
egbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaacaaaahccaabaaa
abaaaaaadkaabaaaacaaaaaadkaabaaaacaaaaaadiaaaaahecaabaaaabaaaaaa
bkaabaaaabaaaaaaabeaaaaamnmmemdpdiaaaaahhcaabaaaacaaaaaakgakbaaa
abaaaaaaegacbaaaacaaaaaaefaaaaajpcaabaaaadaaaaaaegbabaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaajhcaabaaaacaaaaaaegacbaaa
adaaaaaapgapbaaaadaaaaaaegacbaaaacaaaaaadcaaaaajhcaabaaaaaaaaaaa
egacbaaaaaaaaaaaagaabaaaabaaaaaaegacbaaaacaaaaaaefaaaaajpcaabaaa
acaaaaaaegbabaaaadaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaacaaaah
bcaabaaaabaaaaaadkaabaaaacaaaaaadkaabaaaacaaaaaadiaaaaahecaabaaa
abaaaaaaakaabaaaabaaaaaaabeaaaaamnmmcmdpdcaaaaajhcaabaaaaaaaaaaa
egacbaaaacaaaaaakgakbaaaabaaaaaaegacbaaaaaaaaaaaefaaaaajpcaabaaa
acaaaaaaogbkbaaaadaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaacaaaah
ecaabaaaabaaaaaadkaabaaaacaaaaaadkaabaaaacaaaaaadiaaaaahicaabaaa
abaaaaaackaabaaaabaaaaaaabeaaaaamnmmcmdpdcaaaaajhcaabaaaaaaaaaaa
egacbaaaacaaaaaapgapbaaaabaaaaaaegacbaaaaaaaaaaaefaaaaajpcaabaaa
acaaaaaaegbabaaaaeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaacaaaah
icaabaaaabaaaaaadkaabaaaacaaaaaadkaabaaaacaaaaaadiaaaaahicaabaaa
acaaaaaadkaabaaaabaaaaaaabeaaaaaaaaaaadpdcaaaaajhcaabaaaaaaaaaaa
egacbaaaacaaaaaapgapbaaaacaaaaaaegacbaaaaaaaaaaaefaaaaajpcaabaaa
acaaaaaaogbkbaaaaeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaacaaaah
icaabaaaacaaaaaadkaabaaaacaaaaaadkaabaaaacaaaaaadiaaaaahbcaabaaa
aeaaaaaadkaabaaaacaaaaaaabeaaaaaaaaaaadpdcaaaaajhcaabaaaaaaaaaaa
egacbaaaacaaaaaaagaabaaaaeaaaaaaegacbaaaaaaaaaaaefaaaaajpcaabaaa
aeaaaaaaegbabaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaacaaaah
bcaabaaaacaaaaaadkaabaaaaeaaaaaadkaabaaaaeaaaaaadiaaaaahccaabaaa
acaaaaaaakaabaaaacaaaaaaabeaaaaamnmmemdodcaaaaajhcaabaaaaaaaaaaa
egacbaaaaeaaaaaafgafbaaaacaaaaaaegacbaaaaaaaaaaaefaaaaajpcaabaaa
aeaaaaaaogbkbaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaacaaaah
ccaabaaaacaaaaaadkaabaaaaeaaaaaadkaabaaaaeaaaaaadiaaaaahecaabaaa
acaaaaaabkaabaaaacaaaaaaabeaaaaamnmmemdodcaaaaajhcaabaaaaaaaaaaa
egacbaaaaeaaaaaakgakbaaaacaaaaaaegacbaaaaaaaaaaaefaaaaajpcaabaaa
aeaaaaaaegbabaaaagaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaacaaaah
ecaabaaaacaaaaaadkaabaaaaeaaaaaadkaabaaaaeaaaaaadiaaaaahicaabaaa
aeaaaaaackaabaaaacaaaaaaabeaaaaajkjjjjdndcaaaaajhcaabaaaaaaaaaaa
egacbaaaaeaaaaaapgapbaaaaeaaaaaaegacbaaaaaaaaaaaefaaaaajpcaabaaa
aeaaaaaaogbkbaaaagaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaacaaaah
icaabaaaaeaaaaaadkaabaaaaeaaaaaadkaabaaaaeaaaaaadiaaaaahbcaabaaa
afaaaaaadkaabaaaaeaaaaaaabeaaaaajkjjjjdndcaaaaajhcaabaaaaaaaaaaa
egacbaaaaeaaaaaaagaabaaaafaaaaaaegacbaaaaaaaaaaadcaaaaajccaabaaa
abaaaaaabkaabaaaabaaaaaaabeaaaaamnmmemdpdkaabaaaadaaaaaadcaaaaaj
icaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaamnmmemdpbkaabaaaabaaaaaa
dcaaaaajicaabaaaaaaaaaaaakaabaaaabaaaaaaabeaaaaamnmmcmdpdkaabaaa
aaaaaaaadcaaaaajicaabaaaaaaaaaaackaabaaaabaaaaaaabeaaaaamnmmcmdp
dkaabaaaaaaaaaaadcaaaaajicaabaaaaaaaaaaadkaabaaaabaaaaaaabeaaaaa
aaaaaadpdkaabaaaaaaaaaaadcaaaaajicaabaaaaaaaaaaadkaabaaaacaaaaaa
abeaaaaaaaaaaadpdkaabaaaaaaaaaaadcaaaaajicaabaaaaaaaaaaaakaabaaa
acaaaaaaabeaaaaamnmmemdodkaabaaaaaaaaaaadcaaaaajicaabaaaaaaaaaaa
bkaabaaaacaaaaaaabeaaaaamnmmemdodkaabaaaaaaaaaaadcaaaaajicaabaaa
aaaaaaaackaabaaaacaaaaaaabeaaaaajkjjjjdndkaabaaaaaaaaaaadcaaaaaj
icaabaaaaaaaaaaadkaabaaaaeaaaaaaabeaaaaajkjjjjdndkaabaaaaaaaaaaa
aaaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaabhlhnbdiaoaaaaah
hcaabaaaaaaaaaaaegacbaaaaaaaaaaapgapbaaaaaaaaaaadbaaaaahicaabaaa
aaaaaaaadkaabaaaadaaaaaaabeaaaaaaknhcddmdhaaaaajhccabaaaaaaaaaaa
pgapbaaaaaaaaaaaegacbaaaadaaaaaaegacbaaaaaaaaaaadgaaaaaficcabaaa
aaaaaaaadkaabaaaadaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 2 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 1 math, 1 texture
 //        d3d9 : 2 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 1324880
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 2 math, 1 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec4 c_1;
  vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD1);
  c_1.xyz = tmpvar_2.xyz;
  c_1.w = clamp ((tmpvar_2.w * 100.0), 0.0, 1.0);
  gl_FragData[0] = c_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 2 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c0, 100, 0, 0, 0
dcl_texcoord1 v0.xy
dcl_2d s0
texld r0, v0, s0
mul_sat oC0.w, r0.w, c0.x
mov oC0.xyz, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 1 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedfpeleclkihodjepmliflhpblipbgnaibabaaaaaaheabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcjmaaaaaaeaaaaaaachaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
mcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacabaaaaaaefaaaaaj
pcaabaaaaaaaaaaaogbkbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
dicaaaahiccabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaamiecdgaaaaaf
hccabaaaaaaaaaaaegacbaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 11 math
 //      opengl : 29 math, 3 texture, 2 branch
 // Stats for Fragment shader:
 //       d3d11 : 21 math, 3 texture, 1 branch
 //        d3d9 : 27 math, 3 texture, 5 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Blend DstAlpha OneMinusDstAlpha, Zero One
  GpuProgramID 1399684
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 29 math, 3 textures, 2 branches
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
uniform vec4 _Offsets;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = xlv_TEXCOORD1;
  vec4 steps_3;
  vec2 lenStep_4;
  vec4 sum_5;
  float sampleCount_6;
  vec4 tmpvar_7;
  tmpvar_7 = texture2D (_MainTex, xlv_TEXCOORD1);
  sampleCount_6 = tmpvar_7.w;
  sum_5 = (tmpvar_7 * tmpvar_7.w);
  vec2 tmpvar_8;
  tmpvar_8 = (tmpvar_7.ww * 0.09090909);
  lenStep_4 = tmpvar_8;
  steps_3 = (((_Offsets.xyxy * _MainTex_TexelSize.xyxy) * tmpvar_8.xyxy) * vec4(1.0, 1.0, -1.0, -1.0));
  for (int l_2 = 1; l_2 < 12; l_2++) {
    vec4 tmpvar_9;
    tmpvar_9 = (tmpvar_1.xyxy + (steps_3 * float(l_2)));
    vec4 tmpvar_10;
    tmpvar_10 = texture2D (_MainTex, tmpvar_9.xy);
    vec4 tmpvar_11;
    tmpvar_11 = texture2D (_MainTex, tmpvar_9.zw);
    vec2 tmpvar_12;
    tmpvar_12.x = tmpvar_10.w;
    tmpvar_12.y = tmpvar_11.w;
    vec2 tmpvar_13;
    vec2 tmpvar_14;
    tmpvar_14 = clamp (((
      (tmpvar_12 - (lenStep_4.xx * float(l_2)))
     - vec2(-0.4, -0.4)) / vec2(0.4, 0.4)), 0.0, 1.0);
    tmpvar_13 = (tmpvar_14 * (tmpvar_14 * (3.0 - 
      (2.0 * tmpvar_14)
    )));
    sum_5 = (sum_5 + ((tmpvar_10 * tmpvar_13.x) + (tmpvar_11 * tmpvar_13.y)));
    sampleCount_6 = (sampleCount_6 + dot (tmpvar_13, vec2(1.0, 1.0)));
  };
  gl_FragData[0] = (sum_5 / (1e-05 + sampleCount_6));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 11 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad r0.y, r0.x, r0.y, v1.y
mov r0.x, v1.x
mov o1.xy, r0
mov o2.xy, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedbkdlpgpjbpaooplkpiiapliplhojmdjhabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajkccabaaaabaaaaaaagaabaaaaaaaaaaafgafbaaaaaaaaaaafgbfbaaa
abaaaaaadgaaaaaffccabaaaabaaaaaaagbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 27 math, 3 textures, 5 branches
Vector 0 [_MainTex_TexelSize]
Vector 1 [_Offsets]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c2, 0.0909090936, 1, -1, 0
def c3, 0.400000006, 2.5, -2, 3
def c4, 9.99999975e-006, 0, 0, 0
defi i0, 11, 0, 0, 0
dcl_texcoord1 v0.xy
dcl_2d s0
texld r0, v0, s0
mul r1, r0.w, r0
mul r0.x, r0.w, c2.x
mov r2.xy, c1
mul r2, r2.xyxy, c0.xyxy
mul r2, r0.x, r2
mul r2, r2, c2.yyzz
mov r3, r1
mov r0.y, r0.w
mov r0.z, c2.y
rep i0
mad r4, r2, r0.z, v0.xyxy
texld r5, r4, s0
texld r4, r4.zwzw, s0
mov r6.x, r5.w
mov r6.y, r4.w
mad r6.xy, r0.x, -r0.z, r6
add r6.xy, r6, c3.x
mul_sat r6.xy, r6, c3.y
mad r6.zw, r6.xyxy, c3.z, c3.w
mul r6.xy, r6, r6
mul r6.xy, r6, r6.zwzw
mul r4, r4, r6.y
mad r4, r5, r6.x, r4
add r3, r3, r4
dp2add r0.y, r6, c2.y, r0.y
add r0.z, r0.z, c2.y
endrep
add r0.x, r0.y, c4.x
rcp r0.x, r0.x
mul oC0, r0.x, r3

""
}
SubProgram ""d3d11 "" {
// Stats: 21 math, 3 textures, 1 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
Vector 128 [_Offsets]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedlefnjhhkphgeommpbbiaohddaljehmdhabaaaaaaoiaeaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcbaaeaaaaeaaaaaaaaeabaaaa
fjaaaaaeegiocaaaaaaaaaaaajaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaagcbaaaadmcbabaaaabaaaaaagfaaaaadpccabaaa
aaaaaaaagiaaaaacaiaaaaaaefaaaaajpcaabaaaaaaaaaaaogbkbaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaahpcaabaaaabaaaaaapgapbaaa
aaaaaaaaegaobaaaaaaaaaaadiaaaaahbcaabaaaaaaaaaaadkaabaaaaaaaaaaa
abeaaaaaimcolkdndiaaaaajpcaabaaaacaaaaaaegiecaaaaaaaaaaaahaaaaaa
egiecaaaaaaaaaaaaiaaaaaadiaaaaahpcaabaaaacaaaaaaagaabaaaaaaaaaaa
egaobaaaacaaaaaadiaaaaakpcaabaaaacaaaaaaegaobaaaacaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaialpaaaaialpdgaaaaafpcaabaaaadaaaaaaegaobaaa
abaaaaaadgaaaaafccaabaaaaaaaaaaadkaabaaaaaaaaaaadgaaaaafecaabaaa
aaaaaaaaabeaaaaaabaaaaaadaaaaaabcbaaaaahbcaabaaaaeaaaaaackaabaaa
aaaaaaaaabeaaaaaamaaaaaaadaaaeadakaabaaaaeaaaaaaclaaaaafbcaabaaa
aeaaaaaackaabaaaaaaaaaaadcaaaaajpcaabaaaafaaaaaaegaobaaaacaaaaaa
agaabaaaaeaaaaaaogbobaaaabaaaaaaefaaaaajpcaabaaaagaaaaaaegaabaaa
afaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaafaaaaaa
ogakbaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadgaaaaafbcaabaaa
ahaaaaaadkaabaaaagaaaaaadgaaaaafccaabaaaahaaaaaadkaabaaaafaaaaaa
dcaaaaakdcaabaaaaeaaaaaaagaabaiaebaaaaaaaaaaaaaaagaabaaaaeaaaaaa
egaabaaaahaaaaaaaaaaaaakdcaabaaaaeaaaaaaegaabaaaaeaaaaaaaceaaaaa
mnmmmmdomnmmmmdoaaaaaaaaaaaaaaaadicaaaakdcaabaaaaeaaaaaaegaabaaa
aeaaaaaaaceaaaaaaaaacaeaaaaacaeaaaaaaaaaaaaaaaaadcaaaaapmcaabaaa
aeaaaaaaagaebaaaaeaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaamaaaaaaama
aceaaaaaaaaaaaaaaaaaaaaaaaaaeaeaaaaaeaeadiaaaaahdcaabaaaaeaaaaaa
egaabaaaaeaaaaaaegaabaaaaeaaaaaadiaaaaahdcaabaaaaeaaaaaaegaabaaa
aeaaaaaaogakbaaaaeaaaaaadiaaaaahpcaabaaaafaaaaaafgafbaaaaeaaaaaa
egaobaaaafaaaaaadcaaaaajpcaabaaaafaaaaaaegaobaaaagaaaaaaagaabaaa
aeaaaaaaegaobaaaafaaaaaaaaaaaaahpcaabaaaadaaaaaaegaobaaaadaaaaaa
egaobaaaafaaaaaaapaaaaakbcaabaaaaeaaaaaaegaabaaaaeaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaaaaaaaaaaaaaaaaaaaahccaabaaaaaaaaaaabkaabaaa
aaaaaaaaakaabaaaaeaaaaaaboaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaa
abeaaaaaabaaaaaabgaaaaabaaaaaaahbcaabaaaaaaaaaaabkaabaaaaaaaaaaa
abeaaaaakmmfchdhaoaaaaahpccabaaaaaaaaaaaegaobaaaadaaaaaaagaabaaa
aaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 0 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 0 math, 1 texture
 //        d3d9 : 0 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Blend DstAlpha One, Zero One
  GpuProgramID 1444505
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 0 math, 1 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD1;
void main ()
{
  gl_FragData[0] = texture2D (_MainTex, xlv_TEXCOORD1);
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_MainTex_TexelSize]
""vs_3_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad o1.y, r0.x, r0.y, v1.y
mov o1.x, v1.x
mov o2.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedefgokgdnhdaplmjakiagepdiofpllgicabaaaaaahmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcieabaaaaeaaaabaagbaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaaabaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dbaaaaaibcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: -1 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
dcl_texcoord1 v0.xy
dcl_2d s0
texld oC0, v0, s0

""
}
SubProgram ""d3d11 "" {
// Stats: 0 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedoaicnihpfmdhmdmlhpbdbnjidbbladpiabaaaaaadmabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcgeaaaaaaeaaaaaaabjaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
mcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaaefaaaaajpccabaaaaaaaaaaa
ogbkbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadoaaaaab""
}
}
 }
}
Fallback Off
}";

		private const String dx11BokehShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 19.2KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/Dof/DX11Dof"" {
Properties {
 _MainTex ("""", 2D) = ""white"" { }
 _BlurredColor ("""", 2D) = ""white"" { }
 _FgCocMask ("""", 2D) = ""white"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 // Stats for Fragment shader:
 //       d3d11 : 27 math, 3 texture, 2 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 32027
Program ""vp"" {
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 16 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_5_0
eefiecedbhflbhhegibboomhakpjkjdjjgedglglabaaaaaalmacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieffimeabaaaafaaaabaahbaaaaaa
gkaiaaabfjaaaaaeegiocaaaaaaaaaaaacaaaaaafjaaaaaeegiocaaaabaaaaaa
aeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaae
pccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadmccabaaa
abaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaa
egiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaa
aaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaa
aaaaaaaadgaaaaafnccabaaaaaaaaaaaagaobaaaaaaaaaaadbaaaaaibcaabaaa
aaaaaaaabkiacaaaaaaaaaaaabaaaaaaabeaaaaaaaaaaaaadhaaaaakcccabaaa
aaaaaaaaakaabaaaaaaaaaaabkaabaiaebaaaaaaaaaaaaaabkaabaaaaaaaaaaa
aaaaaaaiccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dhaaaaajcccabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaa
abaaaaaadgaaaaafnccabaaaabaaaaaaagbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""d3d11 "" {
// Stats: 27 math, 3 textures, 2 branches
SetTexture 0 [_MainTex] 2D 1
SetTexture 1 [_BlurredColor] 2D 0
SetTexture 2 [_FgCocMask] 2D 2
ConstBuffer ""$Globals"" 144
Vector 0 [_BokehParams]
Float 44 [_SpawnHeuristic]
Vector 96 [unity_ColorSpaceLuminance]
BindCB  ""$Globals"" 0
""ps_5_0
eefiecedekgdofmefpcbjaojfmnjconnhifidgnkabaaaaaacaagaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieffieiafaaaafaaaaaaafcabaaaa
gkaiaaabfjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaaaaaaaaaa
fkaaaaadaagabaaaabaaaaaafkaaaaadaagabaaaacaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaafibiaaaeaahabaaa
acaaaaaaffffaaaajoaaaaaeaaoabbaaabaaaaaabmaaaaaagcbaaaaddcbabaaa
abaaaaaagcbaaaadmcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaac
aeaaaaaaefaaaailmcaaaaiaedffbfaapcaabaaaaaaaaaaaegbabaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaabaaaaaadiaaaaaihcaabaaaabaaaaaaegacbaaa
aaaaaaaaegiccaaaaaaaaaaaagaaaaaaaaaaaaahfcaabaaaabaaaaaafgagbaaa
abaaaaaaagaabaaaabaaaaaadcaaaaakbcaabaaaabaaaaaackaabaaaaaaaaaaa
ckiacaaaaaaaaaaaagaaaaaaakaabaaaabaaaaaadiaaaaahccaabaaaabaaaaaa
ckaabaaaabaaaaaabkaabaaaabaaaaaaelaaaaafccaabaaaabaaaaaabkaabaaa
abaaaaaaapaaaaaiccaabaaaabaaaaaapgipcaaaaaaaaaaaagaaaaaafgafbaaa
abaaaaaaaaaaaaahbcaabaaaabaaaaaabkaabaaaabaaaaaaakaabaaaabaaaaaa
efaaaailmcaaaaiaedffbfaapcaabaaaacaaaaaaogbkbaaaabaaaaaaeghobaaa
abaaaaaaaagabaaaaaaaaaaadiaaaaaiocaabaaaabaaaaaaagajbaaaacaaaaaa
agijcaaaaaaaaaaaagaaaaaaaaaaaaahkcaabaaaabaaaaaakgaobaaaabaaaaaa
fgafbaaaabaaaaaadcaaaaakccaabaaaabaaaaaackaabaaaacaaaaaackiacaaa
aaaaaaaaagaaaaaabkaabaaaabaaaaaadiaaaaahecaabaaaabaaaaaadkaabaaa
abaaaaaackaabaaaabaaaaaaelaaaaafecaabaaaabaaaaaackaabaaaabaaaaaa
apaaaaaiecaabaaaabaaaaaapgipcaaaaaaaaaaaagaaaaaakgakbaaaabaaaaaa
aaaaaaahccaabaaaabaaaaaackaabaaaabaaaaaabkaabaaaabaaaaaadiaaaaai
ecaabaaaabaaaaaadkaabaaaaaaaaaaadkiacaaaaaaaaaaaaaaaaaaadbaaaaah
ecaabaaaabaaaaaaabeaaaaaaaaaiadpckaabaaaabaaaaaadbaaaaahicaabaaa
abaaaaaaabeaaaaamnmmmmdndkaabaaaacaaaaaaabaaaaahecaabaaaabaaaaaa
dkaabaaaabaaaaaackaabaaaabaaaaaadbaaaaaiicaabaaaabaaaaaackiacaaa
aaaaaaaaaaaaaaaaakaabaaaabaaaaaaabaaaaahecaabaaaabaaaaaadkaabaaa
abaaaaaackaabaaaabaaaaaaaaaaaaaibcaabaaaabaaaaaabkaabaiaebaaaaaa
abaaaaaaakaabaaaabaaaaaadbaaaaajbcaabaaaabaaaaaadkiacaaaaaaaaaaa
acaaaaaaakaabaiaibaaaaaaabaaaaaaabaaaaahbcaabaaaabaaaaaaakaabaaa
abaaaaaackaabaaaabaaaaaabpaaaeadakaabaaaabaaaaaaefaaaailmcaaaaia
edffbfaaecaabaaaabaaaaaaogbkbaaaabaaaaaaeghlbaaaacaaaaaaaagabaaa
acaaaaaadiaaaaahbcaabaaaacaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaiaea
dgcaaaafbcaabaaaacaaaaaaakaabaaaacaaaaaadiaaaaahhcaabaaaacaaaaaa
egacbaaaaaaaaaaaagaabaaaacaaaaaalcaaaaafbcaabaaaadaaaaaaaaoabbaa
abaaaaaadgaaaaafdcaabaaaabaaaaaaogbkbaaaabaaaaaadgaaaaaficaabaaa
abaaaaaaakaabaaaacaaaaaakiaaaaajpcoabbaaabaaaaaaakaabaaaadaaaaaa
abeaaaaaaaaaaaaaegaobaaaabaaaaaadgaaaaaficaabaaaacaaaaaadkaabaaa
aaaaaaaakiaaaaajhcoabbaaabaaaaaaakaabaaaadaaaaaaabeaaaaabaaaaaaa
jgahbaaaacaaaaaadccaaaakbcaabaaaabaaaaaadkaabaiaebaaaaaaaaaaaaaa
abeaaaaaaaaaiaeaabeaaaaaaaaaiadpdiaaaaahhccabaaaaaaaaaaaegacbaaa
aaaaaaaaagaabaaaabaaaaaadgaaaaaficcabaaaaaaaaaaadkaabaaaacaaaaaa
doaaaaabbfaaaaabdgaaaaafpccabaaaaaaaaaaaegaobaaaaaaaaaaadoaaaaab
""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 2 math
 // Stats for Fragment shader:
 //       d3d11 : 11 math, 1 texture
 // Stats for Geometry shader:
 //       d3d11 : 16 math
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Blend One One
  GpuProgramID 76948
Program ""vp"" {
SubProgram ""d3d11 "" {
// Stats: 2 math
SetBuffer 0 [pointBuffer]
""vs_5_0
eefiecedlajfgkponkfgmlieoonfmkjjfbhcfgmkabaaaaaaeeacaaaaadaaaaaa
cmaaaaaagaaaaaaaoiaaaaaaejfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaa
aaaaaaaaagaaaaaaabaaaaaaaaaaaaaaababaaaafdfgfpfggfhchegfhiejeeaa
epfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadapaaaa
heaaaaaaacaaaaaaaaaaaaaaadaaaaaaabaaaaaaaealaaaaheaaaaaaabaaaaaa
aaaaaaaaadaaaaaaacaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaafeeffied
epepfceeaaklklklfdeieffifeabaaaafaaaabaaffaaaaaagkaiaaabkcaaaaae
aahabaaaaaaaaaaabmaaaaaagaaaaaaebcbabaaaaaaaaaaaagaaaaaaghaaaaae
pccabaaaaaaaaaaaabaaaaaagfaaaaadeccabaaaabaaaaaagfaaaaadpccabaaa
acaaaaaagiaaaaacabaaaaaadgaaaaaimccabaaaaaaaaaaaaceaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaiadpkhaaaailacodaaiaidjjbjaahcaabaaaaaaaaaaa
akbabaaaaaaaaaaaabeaaaaaaaaaaaaaeghcbaaaaaaaaaaadcaaaaapdcaabaaa
aaaaaaaaegaabaaaaaaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaaaaaaaaaaa
aceaaaaaaaaaialpaaaaialpaaaaaaaaaaaaaaaadgaaaaafeccabaaaabaaaaaa
ckaabaaaaaaaaaaadgaaaaagcccabaaaaaaaaaaabkaabaiaebaaaaaaaaaaaaaa
dgaaaaafbccabaaaaaaaaaaaakaabaaaaaaaaaaakhaaaailacodaaiaidjjbjaa
pccabaaaacaaaaaaakbabaaaaaaaaaaaabeaaaaaamaaaaaaeghobaaaaaaaaaaa
doaaaaab""
}
}
Program ""fp"" {
SubProgram ""d3d11 "" {
// Stats: 11 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
""ps_5_0
eefiecedkanajcfgdhmnegkbjdkjbdjkcfeejfjmabaaaaaacmadaaaaadaaaaaa
cmaaaaaaleaaaaaaoiaaaaaaejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaahahaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apahaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaa
aiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfe
gbhcghgfheaaklklfdeieffidmacaaaafaaaaaaaipaaaaaagkaiaaabfkaaaaad
aagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaadhcbabaaa
abaaaaaagcbaaaadhcbabaaaacaaaaaagcbaaaaddcbabaaaadaaaaaagfaaaaad
pccabaaaaaaaaaaagiaaaaacadaaaaaaaaaaaaaldcaabaaaaaaaaaaaegbabaia
ebaaaaaaadaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaaaaaaaaaaaaadiaaaaak
dcaabaaaaaaaaaaaegaabaaaaaaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaaaa
aaaaaaaadcaaaaajdcaabaaaaaaaaaaaegbabaaaabaaaaaaegbabaaaadaaaaaa
egaabaaaaaaaaaaadbaaaaakmcaabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaagaebaaaaaaaaaaadbaaaaakdcaabaaaabaaaaaaegaabaaa
aaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaaaaaaaaaaaaaefaaaailmcaaaaia
edffbfaahcaabaaaacaaaaaaegaabaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaabaaaaahbcaabaaaaaaaaaaadkaabaaaaaaaaaaabkaabaaaabaaaaaa
abaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaaakaabaaaabaaaaaaabaaaaah
bcaabaaaaaaaaaaaakaabaaaaaaaaaaackaabaaaaaaaaaaaabaaaaahbcaabaaa
aaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaiadpdgaaaaafhcaabaaaabaaaaaa
egbcbaaaacaaaaaadgaaaaaficaabaaaabaaaaaaabeaaaaaaaaaiadpdgaaaaaf
icaabaaaacaaaaaackbabaaaabaaaaaadiaaaaahpcaabaaaabaaaaaaegaobaaa
abaaaaaaegaobaaaacaaaaaadiaaaaahpccabaaaaaaaaaaaagaabaaaaaaaaaaa
egaobaaaabaaaaaadoaaaaab""
}
}
Program ""gp"" {
SubProgram ""d3d11 "" {
// Stats: 16 math
ConstBuffer ""$Globals"" 144
Vector 0 [_BokehParams]
Vector 32 [_Screen] 3
BindCB  ""$Globals"" 0
""gs_5_0
eefiecedcclfjgoncflmcmfekbdlmlgepomchnmoabaaaaaaoeagaaaaadaaaaaa
cmaaaaaaleaaaaaaemabaaaaejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapapaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaabaaaaaa
aeaeaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapapaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdehdfjaaaaaaaaeaaaaaa
aiaaaaaaaaaaaaaahiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaa
aaaaaaaaieaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaahaiaaaaaaaaaaaa
ieaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapaaaaaaaaaaaaaaieaaaaaa
acaaaaaaaaaaaaaaadaaaaaaadaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaa
feeffiedepepfceeaaklklklfdeieffijaafaaaafaaaacaageabaaaagkaiaaab
fjaaaaaeegiocaaaaaaaaaaaadaaaaaagbaaaaafpcbacaaaabaaaaaaaaaaaaaa
abaaaaaafpaaaaaedcbacaaaabaaaaaaabaaaaaafpaaaaaeecbacaaaabaaaaaa
abaaaaaafpaaaaaepcbacaaaabaaaaaaacaaaaaagiaaaaacaeaaaaaafnaiaaab
ipaaaaadaaaabbaaaaaaaaaafmciaaabghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaadhccabaaaabaaaaaagfaaaaadpccabaaaacaaaaaagfaaaaadpccabaaa
adaaaaaafoaaaaacaeaaaaaadgaaaaaimcaabaaaaaaaaaaaaceaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaadiaaaaajbcaabaaaabaaaaaadkiacaaaaaaaaaaa
aaaaaaaadkbacaaaaaaaaaaaacaaaaaadcaaaaakbcaabaaaabaaaaaaakaabaaa
abaaaaaaakiacaaaaaaaaaaaaaaaaaaaabeaaaaaaaaaaadpebaaaaafccaabaaa
abaaaaaaakaabaaaabaaaaaadcaaaaajbcaabaaaabaaaaaaakaabaaaabaaaaaa
abeaaaaaaaaaaaeaabeaaaaaaaaaiadpdcaaaaajccaabaaaabaaaaaabkaabaaa
abaaaaaaabeaaaaaaaaaaaeaabeaaaaaaaaaeaeadiaaaaaidcaabaaaacaaaaaa
fgafbaaaabaaaaaaegiacaaaaaaaaaaaacaaaaaaaoaaaaahccaabaaaabaaaaaa
bkaabaaaabaaaaaaakaabaaaabaaaaaadiaaaaahbcaabaaaabaaaaaaakaabaaa
abaaaaaaakaabaaaabaaaaaaaoaaaaaibcaabaaaabaaaaaabkiacaaaaaaaaaaa
aaaaaaaaakaabaaaabaaaaaadiaaaaaipcaabaaaadaaaaaaagaabaaaabaaaaaa
egbocaaaaaaaaaaaacaaaaaadiaaaaakdcaabaaaaaaaaaaaegaabaaaacaaaaaa
aceaaaaaaaaaialpaaaaiadpaaaaaaaaaaaaaaaaaaaaaaaipcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegbocaaaaaaaaaaaaaaaaaaadgaaaaafpccabaaaaaaaaaaa
egaobaaaaaaaaaaadgaaaaaidccabaaaabaaaaaaaceaaaaaaaaaaaaaaaaaiadp
aaaaaaaaaaaaaaaadgaaaaageccabaaaabaaaaaackbacaaaaaaaaaaaabaaaaaa
dgaaaaafpccabaaaacaaaaaaegaobaaaadaaaaaadgaaaaafdccabaaaadaaaaaa
fgafbaaaabaaaaaadgaaaaaimccabaaaadaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaahfaaaaadaaaabbaaaaaaaaaadgaaaaafecaabaaaacaaaaaa
abeaaaaaaaaaaaaaaaaaaaaipcaabaaaaaaaaaaaegakbaaaacaaaaaaegbocaaa
aaaaaaaaaaaaaaaadgaaaaafpccabaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaai
dccabaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaaaaaaaaaaaaadgaaaaag
eccabaaaabaaaaaackbacaaaaaaaaaaaabaaaaaadgaaaaafpccabaaaacaaaaaa
egaobaaaadaaaaaadgaaaaafdccabaaaadaaaaaafgafbaaaabaaaaaadgaaaaai
mccabaaaadaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaahfaaaaad
aaaabbaaaaaaaaaadcaaaaanpcaabaaaaaaaaaaaegakbaaaacaaaaaaaceaaaaa
aaaaialpaaaaialpaaaaiadpaaaaiadpegbocaaaaaaaaaaaaaaaaaaadgaaaaaf
pccabaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaaidccabaaaabaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadgaaaaageccabaaaabaaaaaackbacaaa
aaaaaaaaabaaaaaadgaaaaafpccabaaaacaaaaaaegaobaaaadaaaaaadgaaaaaf
dccabaaaadaaaaaafgafbaaaabaaaaaadgaaaaaimccabaaaadaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaahfaaaaadaaaabbaaaaaaaaaadgaaaaag
icaabaaaacaaaaaabkaabaiaebaaaaaaacaaaaaaaaaaaaaipcaabaaaaaaaaaaa
mgakbaaaacaaaaaaegbocaaaaaaaaaaaaaaaaaaadgaaaaafpccabaaaaaaaaaaa
egaobaaaaaaaaaaadgaaaaaidccabaaaabaaaaaaaceaaaaaaaaaiadpaaaaaaaa
aaaaaaaaaaaaaaaadgaaaaageccabaaaabaaaaaackbacaaaaaaaaaaaabaaaaaa
dgaaaaafpccabaaaacaaaaaaegaobaaaadaaaaaadgaaaaafdccabaaaadaaaaaa
fgafbaaaabaaaaaadgaaaaaimccabaaaadaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaahfaaaaadaaaabbaaaaaaaaaahgaaaaadaaaabbaaaaaaaaaa
doaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 2 math
 // Stats for Fragment shader:
 //       d3d11 : 11 math, 1 texture
 // Stats for Geometry shader:
 //       d3d11 : 16 math
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Blend DstAlpha One, Zero One
  GpuProgramID 136572
Program ""vp"" {
SubProgram ""d3d11 "" {
// Stats: 2 math
SetBuffer 0 [pointBuffer]
""vs_5_0
eefiecedlajfgkponkfgmlieoonfmkjjfbhcfgmkabaaaaaaeeacaaaaadaaaaaa
cmaaaaaagaaaaaaaoiaaaaaaejfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaa
aaaaaaaaagaaaaaaabaaaaaaaaaaaaaaababaaaafdfgfpfggfhchegfhiejeeaa
epfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadapaaaa
heaaaaaaacaaaaaaaaaaaaaaadaaaaaaabaaaaaaaealaaaaheaaaaaaabaaaaaa
aaaaaaaaadaaaaaaacaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaafeeffied
epepfceeaaklklklfdeieffifeabaaaafaaaabaaffaaaaaagkaiaaabkcaaaaae
aahabaaaaaaaaaaabmaaaaaagaaaaaaebcbabaaaaaaaaaaaagaaaaaaghaaaaae
pccabaaaaaaaaaaaabaaaaaagfaaaaadeccabaaaabaaaaaagfaaaaadpccabaaa
acaaaaaagiaaaaacabaaaaaadgaaaaaimccabaaaaaaaaaaaaceaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaiadpkhaaaailacodaaiaidjjbjaahcaabaaaaaaaaaaa
akbabaaaaaaaaaaaabeaaaaaaaaaaaaaeghcbaaaaaaaaaaadcaaaaapdcaabaaa
aaaaaaaaegaabaaaaaaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaaaaaaaaaaa
aceaaaaaaaaaialpaaaaialpaaaaaaaaaaaaaaaadgaaaaafeccabaaaabaaaaaa
ckaabaaaaaaaaaaadgaaaaagcccabaaaaaaaaaaabkaabaiaebaaaaaaaaaaaaaa
dgaaaaafbccabaaaaaaaaaaaakaabaaaaaaaaaaakhaaaailacodaaiaidjjbjaa
pccabaaaacaaaaaaakbabaaaaaaaaaaaabeaaaaaamaaaaaaeghobaaaaaaaaaaa
doaaaaab""
}
}
Program ""fp"" {
SubProgram ""d3d11 "" {
// Stats: 11 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
""ps_5_0
eefiecedkanajcfgdhmnegkbjdkjbdjkcfeejfjmabaaaaaacmadaaaaadaaaaaa
cmaaaaaaleaaaaaaoiaaaaaaejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaahahaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apahaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaapadaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaa
aiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfe
gbhcghgfheaaklklfdeieffidmacaaaafaaaaaaaipaaaaaagkaiaaabfkaaaaad
aagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaadhcbabaaa
abaaaaaagcbaaaadhcbabaaaacaaaaaagcbaaaaddcbabaaaadaaaaaagfaaaaad
pccabaaaaaaaaaaagiaaaaacadaaaaaaaaaaaaaldcaabaaaaaaaaaaaegbabaia
ebaaaaaaadaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaaaaaaaaaaaaadiaaaaak
dcaabaaaaaaaaaaaegaabaaaaaaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaaaa
aaaaaaaadcaaaaajdcaabaaaaaaaaaaaegbabaaaabaaaaaaegbabaaaadaaaaaa
egaabaaaaaaaaaaadbaaaaakmcaabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaagaebaaaaaaaaaaadbaaaaakdcaabaaaabaaaaaaegaabaaa
aaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaaaaaaaaaaaaaefaaaailmcaaaaia
edffbfaahcaabaaaacaaaaaaegaabaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaabaaaaahbcaabaaaaaaaaaaadkaabaaaaaaaaaaabkaabaaaabaaaaaa
abaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaaakaabaaaabaaaaaaabaaaaah
bcaabaaaaaaaaaaaakaabaaaaaaaaaaackaabaaaaaaaaaaaabaaaaahbcaabaaa
aaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaiadpdgaaaaafhcaabaaaabaaaaaa
egbcbaaaacaaaaaadgaaaaaficaabaaaabaaaaaaabeaaaaaaaaaiadpdgaaaaaf
icaabaaaacaaaaaackbabaaaabaaaaaadiaaaaahpcaabaaaabaaaaaaegaobaaa
abaaaaaaegaobaaaacaaaaaadiaaaaahpccabaaaaaaaaaaaagaabaaaaaaaaaaa
egaobaaaabaaaaaadoaaaaab""
}
}
Program ""gp"" {
SubProgram ""d3d11 "" {
// Stats: 16 math
ConstBuffer ""$Globals"" 144
Vector 0 [_BokehParams]
Vector 32 [_Screen] 3
BindCB  ""$Globals"" 0
""gs_5_0
eefiecedcclfjgoncflmcmfekbdlmlgepomchnmoabaaaaaaoeagaaaaadaaaaaa
cmaaaaaaleaaaaaaemabaaaaejfdeheoiaaaaaaaaeaaaaaaaiaaaaaagiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapapaaaaheaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaaheaaaaaaacaaaaaaaaaaaaaaadaaaaaaabaaaaaa
aeaeaaaaheaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapapaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdehdfjaaaaaaaaeaaaaaa
aiaaaaaaaaaaaaaahiaaaaaaaaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaa
aaaaaaaaieaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaahaiaaaaaaaaaaaa
ieaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapaaaaaaaaaaaaaaieaaaaaa
acaaaaaaaaaaaaaaadaaaaaaadaaaaaaapaaaaaafdfgfpfaepfdejfeejepeoaa
feeffiedepepfceeaaklklklfdeieffijaafaaaafaaaacaageabaaaagkaiaaab
fjaaaaaeegiocaaaaaaaaaaaadaaaaaagbaaaaafpcbacaaaabaaaaaaaaaaaaaa
abaaaaaafpaaaaaedcbacaaaabaaaaaaabaaaaaafpaaaaaeecbacaaaabaaaaaa
abaaaaaafpaaaaaepcbacaaaabaaaaaaacaaaaaagiaaaaacaeaaaaaafnaiaaab
ipaaaaadaaaabbaaaaaaaaaafmciaaabghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaadhccabaaaabaaaaaagfaaaaadpccabaaaacaaaaaagfaaaaadpccabaaa
adaaaaaafoaaaaacaeaaaaaadgaaaaaimcaabaaaaaaaaaaaaceaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaadiaaaaajbcaabaaaabaaaaaadkiacaaaaaaaaaaa
aaaaaaaadkbacaaaaaaaaaaaacaaaaaadcaaaaakbcaabaaaabaaaaaaakaabaaa
abaaaaaaakiacaaaaaaaaaaaaaaaaaaaabeaaaaaaaaaaadpebaaaaafccaabaaa
abaaaaaaakaabaaaabaaaaaadcaaaaajbcaabaaaabaaaaaaakaabaaaabaaaaaa
abeaaaaaaaaaaaeaabeaaaaaaaaaiadpdcaaaaajccaabaaaabaaaaaabkaabaaa
abaaaaaaabeaaaaaaaaaaaeaabeaaaaaaaaaeaeadiaaaaaidcaabaaaacaaaaaa
fgafbaaaabaaaaaaegiacaaaaaaaaaaaacaaaaaaaoaaaaahccaabaaaabaaaaaa
bkaabaaaabaaaaaaakaabaaaabaaaaaadiaaaaahbcaabaaaabaaaaaaakaabaaa
abaaaaaaakaabaaaabaaaaaaaoaaaaaibcaabaaaabaaaaaabkiacaaaaaaaaaaa
aaaaaaaaakaabaaaabaaaaaadiaaaaaipcaabaaaadaaaaaaagaabaaaabaaaaaa
egbocaaaaaaaaaaaacaaaaaadiaaaaakdcaabaaaaaaaaaaaegaabaaaacaaaaaa
aceaaaaaaaaaialpaaaaiadpaaaaaaaaaaaaaaaaaaaaaaaipcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegbocaaaaaaaaaaaaaaaaaaadgaaaaafpccabaaaaaaaaaaa
egaobaaaaaaaaaaadgaaaaaidccabaaaabaaaaaaaceaaaaaaaaaaaaaaaaaiadp
aaaaaaaaaaaaaaaadgaaaaageccabaaaabaaaaaackbacaaaaaaaaaaaabaaaaaa
dgaaaaafpccabaaaacaaaaaaegaobaaaadaaaaaadgaaaaafdccabaaaadaaaaaa
fgafbaaaabaaaaaadgaaaaaimccabaaaadaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaahfaaaaadaaaabbaaaaaaaaaadgaaaaafecaabaaaacaaaaaa
abeaaaaaaaaaaaaaaaaaaaaipcaabaaaaaaaaaaaegakbaaaacaaaaaaegbocaaa
aaaaaaaaaaaaaaaadgaaaaafpccabaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaai
dccabaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaaaaaaaaaaaaadgaaaaag
eccabaaaabaaaaaackbacaaaaaaaaaaaabaaaaaadgaaaaafpccabaaaacaaaaaa
egaobaaaadaaaaaadgaaaaafdccabaaaadaaaaaafgafbaaaabaaaaaadgaaaaai
mccabaaaadaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaahfaaaaad
aaaabbaaaaaaaaaadcaaaaanpcaabaaaaaaaaaaaegakbaaaacaaaaaaaceaaaaa
aaaaialpaaaaialpaaaaiadpaaaaiadpegbocaaaaaaaaaaaaaaaaaaadgaaaaaf
pccabaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaaidccabaaaabaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadgaaaaageccabaaaabaaaaaackbacaaa
aaaaaaaaabaaaaaadgaaaaafpccabaaaacaaaaaaegaobaaaadaaaaaadgaaaaaf
dccabaaaadaaaaaafgafbaaaabaaaaaadgaaaaaimccabaaaadaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaahfaaaaadaaaabbaaaaaaaaaadgaaaaag
icaabaaaacaaaaaabkaabaiaebaaaaaaacaaaaaaaaaaaaaipcaabaaaaaaaaaaa
mgakbaaaacaaaaaaegbocaaaaaaaaaaaaaaaaaaadgaaaaafpccabaaaaaaaaaaa
egaobaaaaaaaaaaadgaaaaaidccabaaaabaaaaaaaceaaaaaaaaaiadpaaaaaaaa
aaaaaaaaaaaaaaaadgaaaaageccabaaaabaaaaaackbacaaaaaaaaaaaabaaaaaa
dgaaaaafpccabaaaacaaaaaaegaobaaaadaaaaaadgaaaaafdccabaaaadaaaaaa
fgafbaaaabaaaaaadgaaaaaimccabaaaadaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaahfaaaaadaaaabbaaaaaaaaaahgaaaaadaaaabbaaaaaaaaaa
doaaaaab""
}
}
 }
}
Fallback Off
}";
	}
}
