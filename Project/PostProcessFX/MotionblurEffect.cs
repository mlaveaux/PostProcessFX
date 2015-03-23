using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PostProcessFX
{
	class MotionblurEffect
	{
		public CameraMotionBlur motionBlur;

		public MotionblurEffect()
		{
			motionBlur = Camera.main.gameObject.AddComponent<CameraMotionBlur>();
			if (motionBlur == null) 
			{
				Debug.LogError("MotionblurEffect: Could not add component CameraMotionBlur to Camera.");
			}
			else
			{
				Material dx11MotionBlurMaterial = new Material(dx11MotionBlurShaderText);
				Material motionBlurMaterial = new Material(motionBlurShaderText);
				
				motionBlur.dx11MotionBlurShader = dx11MotionBlurMaterial.shader;
				motionBlur.shader = motionBlurMaterial.shader;
			}
		}

		private const String dx11MotionBlurShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 15.0KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/CameraMotionBlurDX11"" {
Properties {
 _MainTex (""-"", 2D) = """" { }
 _NoiseTex (""-"", 2D) = ""grey"" { }
 _VelTex (""-"", 2D) = ""black"" { }
 _NeighbourMaxTex (""-"", 2D) = ""black"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 // Stats for Fragment shader:
 //       d3d11 : 10 math, 1 texture, 2 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 1777
Program ""vp"" {
SubProgram ""d3d11 "" {
// Stats: 4 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""UnityPerDraw"" 0
""vs_5_0
eefiecedbodhicpeacigaoanndeollipijnneebcabaaaaaaoiabaaaaadaaaaaa
cmaaaaaaiaaaaaaaniaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieffiaiabaaaa
faaaabaaecaaaaaagkaiaaabfjaaaaaeegiocaaaaaaaaaaaaeaaaaaafpaaaaad
pcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaa
abaaaaaagfaaaaaddccabaaaabaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaa
abaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""d3d11 "" {
// Stats: 10 math, 1 textures, 2 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 384
Float 96 [_MaxRadiusOrKInPaper]
Vector 112 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_5_0
eefiecedbcnnhbncdkopibmijjmkhclmmcmlgnacabaaaaaalmadaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieffipmacaaaa
faaaaaaalpaaaaaagkaiaaabfjaaaaaeegiocaaaaaaaaaaaaiaaaaaafkaaaaad
aagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaa
abaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacaeaaaaaadiaaaaajdcaabaaa
aaaaaaaaagiacaaaaaaaaaaaagaaaaaaegiacaaaaaaaaaaaahaaaaaadcaaaaan
dcaabaaaaaaaaaaaegaabaiaebaaaaaaaaaaaaaaaceaaaaaaaaaaadpaaaaaadp
aaaaaaaaaaaaaaaaegbabaaaabaaaaaablaaaaagecaabaaaaaaaaaaaakiacaaa
aaaaaaaaagaaaaaadgaaaaaidcaabaaaabaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaadgaaaaaficaabaaaaaaaaaaaabeaaaaaaaaaaaaadaaaaaab
cbaaaaahecaabaaaabaaaaaadkaabaaaaaaaaaaackaabaaaaaaaaaaaadaaaead
ckaabaaaabaaaaaaclaaaaafccaabaaaacaaaaaadkaabaaaaaaaaaaadgaaaaaf
mcaabaaaabaaaaaaagaebaaaabaaaaaadgaaaaafecaabaaaacaaaaaaabeaaaaa
aaaaaaaadaaaaaabcbaaaaahicaabaaaacaaaaaackaabaaaacaaaaaackaabaaa
aaaaaaaaadaaaeaddkaabaaaacaaaaaaclaaaaafbcaabaaaacaaaaaackaabaaa
acaaaaaadcaaaaakjcaabaaaacaaaaaaagaebaaaacaaaaaaagiecaaaaaaaaaaa
ahaaaaaaagaebaaaaaaaaaaaefaaaailmcaaaaiaedffbfaajcaabaaaacaaaaaa
mgaabaaaacaaaaaaighhbaaaaaaaaaaaaagabaaaaaaaaaaaapaaaaahbcaabaaa
adaaaaaaogakbaaaabaaaaaaogakbaaaabaaaaaaapaaaaahccaabaaaadaaaaaa
mgaabaaaacaaaaaamgaabaaaacaaaaaadbaaaaahbcaabaaaadaaaaaabkaabaaa
adaaaaaaakaabaaaadaaaaaadhaaaaajmcaabaaaabaaaaaaagaabaaaadaaaaaa
kgaobaaaabaaaaaaagambaaaacaaaaaaboaaaaahecaabaaaacaaaaaackaabaaa
acaaaaaaabeaaaaaabaaaaaabgaaaaabdgaaaaafdcaabaaaabaaaaaaogakbaaa
abaaaaaaboaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaabaaaaaa
bgaaaaabdgaaaaafdccabaaaaaaaaaaaegaabaaaabaaaaaadgaaaaaimccabaaa
aaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaiadpdoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 // Stats for Fragment shader:
 //       d3d11 : 8 math, 1 texture, 2 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 105698
Program ""vp"" {
SubProgram ""d3d11 "" {
// Stats: 4 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""UnityPerDraw"" 0
""vs_5_0
eefiecedbodhicpeacigaoanndeollipijnneebcabaaaaaaoiabaaaaadaaaaaa
cmaaaaaaiaaaaaaaniaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieffiaiabaaaa
faaaabaaecaaaaaagkaiaaabfjaaaaaeegiocaaaaaaaaaaaaeaaaaaafpaaaaad
pcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaa
abaaaaaagfaaaaaddccabaaaabaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaa
abaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""d3d11 "" {
// Stats: 8 math, 1 textures, 2 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 384
Vector 112 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_5_0
eefiecedokpcakcpnljihpmlnnpjmpebjookammfabaaaaaadiadaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieffihiacaaaa
faaaaaaajoaaaaaagkaiaaabfjaaaaaeegiocaaaaaaaaaaaaiaaaaaafkaaaaad
aagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaa
abaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacadaaaaaadgaaaaaihcaabaaa
aaaaaaaaaceaaaaaaaaaaaaaaaaaaaaappppppppaaaaaaaadaaaaaabccaaaaah
icaabaaaaaaaaaaaabeaaaaaabaaaaaackaabaaaaaaaaaaaadaaaeaddkaabaaa
aaaaaaaaclaaaaafccaabaaaabaaaaaackaabaaaaaaaaaaadgaaaaafmcaabaaa
abaaaaaaagaebaaaaaaaaaaadgaaaaaficaabaaaaaaaaaaaabeaaaaapppppppp
daaaaaabccaaaaahbcaabaaaacaaaaaaabeaaaaaabaaaaaadkaabaaaaaaaaaaa
adaaaeadakaabaaaacaaaaaaclaaaaafbcaabaaaabaaaaaadkaabaaaaaaaaaaa
dcaaaaakdcaabaaaacaaaaaaegaabaaaabaaaaaaegiacaaaaaaaaaaaahaaaaaa
egbabaaaabaaaaaaefaaaailmcaaaaiaedffbfaadcaabaaaacaaaaaaegaabaaa
acaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaapaaaaahbcaabaaaabaaaaaa
ogakbaaaabaaaaaaogakbaaaabaaaaaaapaaaaahecaabaaaacaaaaaaegaabaaa
acaaaaaaegaabaaaacaaaaaadbaaaaahbcaabaaaabaaaaaackaabaaaacaaaaaa
akaabaaaabaaaaaadhaaaaajmcaabaaaabaaaaaaagaabaaaabaaaaaakgaobaaa
abaaaaaaagaebaaaacaaaaaaboaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaa
abeaaaaaabaaaaaabgaaaaabdgaaaaafdcaabaaaaaaaaaaaogakbaaaabaaaaaa
boaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaaabeaaaaaabaaaaaabgaaaaab
dgaaaaafdccabaaaaaaaaaaaegaabaaaaaaaaaaadgaaaaaimccabaaaaaaaaaaa
aceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaiadpdoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 // Stats for Fragment shader:
 //       d3d11 : 67 math, 5 texture, 3 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 151572
Program ""vp"" {
SubProgram ""d3d11 "" {
// Stats: 4 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""UnityPerDraw"" 0
""vs_5_0
eefiecedbodhicpeacigaoanndeollipijnneebcabaaaaaaoiabaaaaadaaaaaa
cmaaaaaaiaaaaaaaniaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieffiaiabaaaa
faaaabaaecaaaaaagkaiaaabfjaaaaaeegiocaaaaaaaaaaaaeaaaaaafpaaaaad
pcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaa
abaaaaaagfaaaaaddccabaaaabaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaaaaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaa
abaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""d3d11 "" {
// Stats: 67 math, 5 textures, 3 branches
SetTexture 0 [_NeighbourMaxTex] 2D 3
SetTexture 1 [_MainTex] 2D 0
SetTexture 2 [_CameraDepthTexture] 2D 1
SetTexture 3 [_VelTex] 2D 2
SetTexture 4 [_NoiseTex] 2D 4
ConstBuffer ""$Globals"" 384
Float 96 [_MaxRadiusOrKInPaper]
Vector 112 [_MainTex_TexelSize]
Float 352 [_Jitter]
Float 368 [_SoftZDistance]
ConstBuffer ""UnityPerCamera"" 144
Vector 112 [_ZBufferParams]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerCamera"" 1
""ps_5_0
eefiecedopdoonnojcjlnhadbfoakmlhfgmgiajpabaaaaaageamaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieffikealaaaa
faaaaaaaojacaaaagkaiaaabfjaaaaaeegiocaaaaaaaaaaabiaaaaaafjaaaaae
egiocaaaabaaaaaaaiaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaa
abaaaaaafkaaaaadaagabaaaacaaaaaafkaaaaadaagabaaaadaaaaaafkaaaaad
aagabaaaaeaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaa
abaaaaaaffffaaaafibiaaaeaahabaaaacaaaaaaffffaaaafibiaaaeaahabaaa
adaaaaaaffffaaaafibiaaaeaahabaaaaeaaaaaaffffaaaagcbaaaaddcbabaaa
abaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacaiaaaaaadbaaaaaibcaabaaa
aaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaa
aaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajccaabaaa
abaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaaf
bcaabaaaabaaaaaaakbabaaaabaaaaaaefaaaailmcaaaaiaedffbfaagcaabaaa
aaaaaaaaegaabaaaabaaaaaacghnbaaaaaaaaaaaaagabaaaadaaaaaaefaaaail
mcaaaaiaedffbfaapcaabaaaacaaaaaaegbabaaaabaaaaaaeghobaaaabaaaaaa
aagabaaaaaaaaaaaefaaaailmcaaaaiaedffbfaaicaabaaaaaaaaaaaegbabaaa
abaaaaaajghdbaaaacaaaaaaaagabaaaabaaaaaadcaaaaalicaabaaaaaaaaaaa
akiacaaaabaaaaaaahaaaaaadkaabaaaaaaaaaaabkiacaaaabaaaaaaahaaaaaa
aoaaaaakicaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadp
dkaabaaaaaaaaaaaefaaaailmcaaaaiaedffbfaadcaabaaaabaaaaaaegaabaaa
abaaaaaaeghobaaaadaaaaaaaagabaaaacaaaaaadiaaaaakmcaabaaaabaaaaaa
agbebaaaabaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaadaebaaaadaebefaaaail
mcaaaaiaedffbfaaecaabaaaabaaaaaaogakbaaaabaaaaaajghmbaaaaeaaaaaa
aagabaaaaeaaaaaadcaaaaajecaabaaaabaaaaaackaabaaaabaaaaaaabeaaaaa
aaaaaaeaabeaaaaaaaaaialpaaaaaaaiicaabaaaabaaaaaaakiacaaaaaaaaaaa
bgaaaaaaabeaaaaaaaaajaebapaaaaahbcaabaaaadaaaaaaegaabaaaabaaaaaa
egaabaaaabaaaaaaeeaaaaafccaabaaaadaaaaaaakaabaaaadaaaaaadiaaaaah
gcaabaaaadaaaaaaagabbaaaabaaaaaafgafbaaaadaaaaaadiaaaaaigcaabaaa
adaaaaaafgagbaaaadaaaaaaagibcaaaaaaaaaaaahaaaaaadiaaaaaigcaabaaa
adaaaaaafgagbaaaadaaaaaaagiacaaaaaaaaaaaagaaaaaaddaaaaahdcaabaaa
abaaaaaaegaabaaaabaaaaaajgafbaaaadaaaaaaaaaaaaaidcaabaaaabaaaaaa
jgafbaiaebaaaaaaaaaaaaaaegaabaaaabaaaaaaelaaaaafbcaabaaaadaaaaaa
akaabaaaadaaaaaadiaaaaahccaabaaaadaaaaaaakaabaaaadaaaaaaabeaaaaa
mimmmmdnaoaaaaakccaabaaaadaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaiadpbkaabaaaadaaaaaadgaaaaafpcaabaaaaeaaaaaaegaobaaaacaaaaaa
dgaaaaaimcaabaaaadaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaiadpaaaaaaaa
daaaaaabcbaaaaahbcaabaaaafaaaaaadkaabaaaadaaaaaaabeaaaaabdaaaaaa
adaaaeadakaabaaaafaaaaaacaaaaaahbcaabaaaafaaaaaadkaabaaaadaaaaaa
abeaaaaaajaaaaaabpaaaeadakaabaaaafaaaaaadgaaaaaficaabaaaadaaaaaa
abeaaaaaakaaaaaaahaaaaabbfaaaaabclaaaaafbcaabaaaafaaaaaadkaabaaa
adaaaaaadcaaaaakbcaabaaaafaaaaaackaabaaaabaaaaaaakiacaaaaaaaaaaa
bgaaaaaaakaabaaaafaaaaaaaoaaaaahbcaabaaaafaaaaaaakaabaaaafaaaaaa
dkaabaaaabaaaaaadcaaaaajbcaabaaaafaaaaaaakaabaaaafaaaaaaabeaaaaa
aaaaaaeaabeaaaaaaaaaialpabaaaaahccaabaaaafaaaaaadkaabaaaadaaaaaa
abeaaaaaabaaaaaadhaaaaajccaabaaaafaaaaaabkaabaaaafaaaaaaabeaaaaa
aaaaaaaaabeaaaaaaaaaiadpdcaaaaajgcaabaaaafaaaaaafgafbaaaafaaaaaa
agabbaaaabaaaaaafgagbaaaaaaaaaaadiaaaaahdcaabaaaagaaaaaaagaabaaa
afaaaaaajgafbaaaafaaaaaadcaaaaajdcaabaaaafaaaaaajgafbaaaafaaaaaa
agaabaaaafaaaaaaegbabaaaabaaaaaaaaaaaaaiicaabaaaafaaaaaabkaabaia
ebaaaaaaafaaaaaaabeaaaaaaaaaiadpdhaaaaajecaabaaaafaaaaaaakaabaaa
aaaaaaaadkaabaaaafaaaaaabkaabaaaafaaaaaaeiaaaainmcaaaaiaedffbfaa
mcaabaaaafaaaaaaigaabaaaafaaaaaaoghebaaaadaaaaaaaagabaaaacaaaaaa
abeaaaaaaaaaaaaaeiaaaainmcaaaaiaedffbfaaecaabaaaagaaaaaaegaabaaa
afaaaaaajghmbaaaacaaaaaaaagabaaaabaaaaaaabeaaaaaaaaaaaaadcaaaaal
ecaabaaaagaaaaaaakiacaaaabaaaaaaahaaaaaackaabaaaagaaaaaabkiacaaa
abaaaaaaahaaaaaaaoaaaaakecaabaaaagaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaiadpaaaaiadpckaabaaaagaaaaaaaaaaaaaiicaabaaaagaaaaaadkaabaia
ebaaaaaaaaaaaaaackaabaaaagaaaaaaaoaaaaaiicaabaaaagaaaaaadkaabaaa
agaaaaaaakiacaaaaaaaaaaabhaaaaaaaaaaaaaiecaabaaaagaaaaaadkaabaaa
aaaaaaaackaabaiaebaaaaaaagaaaaaaaoaaaaaiecaabaaaagaaaaaackaabaaa
agaaaaaaakiacaaaaaaaaaaabhaaaaaaaacaaaalmcaabaaaagaaaaaakgaobaia
ebaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaiadpaaaaiadpapaaaaah
bcaabaaaagaaaaaaegaabaaaagaaaaaaegaabaaaagaaaaaaelaaaaafbcaabaaa
agaaaaaaakaabaaaagaaaaaaapaaaaahecaabaaaafaaaaaaogakbaaaafaaaaaa
ogakbaaaafaaaaaaelaaaaafecaabaaaafaaaaaackaabaaaafaaaaaaaoaaaaah
icaabaaaafaaaaaaakaabaaaagaaaaaackaabaaaafaaaaaaaaaaaaaiicaabaaa
afaaaaaadkaabaiaebaaaaaaafaaaaaaabeaaaaaaaaaiadpdeaaaaahicaabaaa
afaaaaaadkaabaaaafaaaaaaabeaaaaaaaaaaaaaaaaaaaaidcaabaaaahaaaaaa
egaabaiaebaaaaaaafaaaaaaegbabaaaabaaaaaaapaaaaahccaabaaaagaaaaaa
egaabaaaahaaaaaaegaabaaaahaaaaaaelaaaaafccaabaaaagaaaaaabkaabaaa
agaaaaaaaoaaaaahbcaabaaaahaaaaaabkaabaaaagaaaaaaakaabaaaadaaaaaa
aaaaaaaibcaabaaaahaaaaaaakaabaiaebaaaaaaahaaaaaaabeaaaaaaaaaiadp
deaaaaahbcaabaaaahaaaaaaakaabaaaahaaaaaaabeaaaaaaaaaaaaadiaaaaah
ecaabaaaagaaaaaackaabaaaagaaaaaaakaabaaaahaaaaaadcaaaaajicaabaaa
afaaaaaadkaabaaaagaaaaaadkaabaaaafaaaaaackaabaaaagaaaaaadiaaaaah
ecaabaaaagaaaaaackaabaaaafaaaaaaabeaaaaamimmmmdndcaaaaakecaabaaa
afaaaaaackaabaiaebaaaaaaafaaaaaaabeaaaaaddddhddpakaabaaaagaaaaaa
aoaaaaakbcaabaaaagaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadp
ckaabaaaagaaaaaadicaaaahecaabaaaafaaaaaackaabaaaafaaaaaaakaabaaa
agaaaaaadcaaaaajbcaabaaaagaaaaaackaabaaaafaaaaaaabeaaaaaaaaaaama
abeaaaaaaaaaeaeadiaaaaahecaabaaaafaaaaaackaabaaaafaaaaaackaabaaa
afaaaaaadcaaaaakecaabaaaafaaaaaaakaabaiaebaaaaaaagaaaaaackaabaaa
afaaaaaaabeaaaaaaaaaiadpdcaaaaakbcaabaaaagaaaaaaakaabaiaebaaaaaa
adaaaaaaabeaaaaaddddhddpbkaabaaaagaaaaaadicaaaahbcaabaaaagaaaaaa
bkaabaaaadaaaaaaakaabaaaagaaaaaadcaaaaajccaabaaaagaaaaaaakaabaaa
agaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaeadiaaaaahbcaabaaaagaaaaaa
akaabaaaagaaaaaaakaabaaaagaaaaaadcaaaaakbcaabaaaagaaaaaabkaabaia
ebaaaaaaagaaaaaaakaabaaaagaaaaaaabeaaaaaaaaaiadpapaaaaahecaabaaa
afaaaaaakgakbaaaafaaaaaaagaabaaaagaaaaaaaaaaaaahecaabaaaafaaaaaa
ckaabaaaafaaaaaadkaabaaaafaaaaaaeiaaaainmcaaaaiaedffbfaapcaabaaa
agaaaaaaegaabaaaafaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaabeaaaaa
aaaaaaaadcaaaaajpcaabaaaaeaaaaaaegaobaaaagaaaaaakgakbaaaafaaaaaa
egaobaaaaeaaaaaaaaaaaaahecaabaaaadaaaaaackaabaaaadaaaaaackaabaaa
afaaaaaaboaaaaahicaabaaaadaaaaaadkaabaaaadaaaaaaabeaaaaaabaaaaaa
bgaaaaabaoaaaaahpccabaaaaaaaaaaaegaobaaaaeaaaaaakgakbaaaadaaaaaa
doaaaaab""
}
}
 }
}
Fallback Off
}";

		private const String motionBlurShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 71.4KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/CameraMotionBlur"" {
Properties {
 _MainTex (""-"", 2D) = """" { }
 _NoiseTex (""-"", 2D) = ""grey"" { }
 _VelTex (""-"", 2D) = ""black"" { }
 _NeighbourMaxTex (""-"", 2D) = ""black"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 18 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 20 math, 1 texture
 //        d3d9 : 27 math, 1 texture
 Pass {
  ZTest Always
  Cull Off
  GpuProgramID 31911
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 18 math, 1 textures
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
uniform sampler2D _CameraDepthTexture;
uniform vec4 _MainTex_TexelSize;
uniform mat4 _ToPrevViewProjCombined;
uniform float _VelocityScale;
uniform float _MaxVelocity;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 prevClipPos_1;
  vec3 tmpvar_2;
  tmpvar_2.xy = ((xlv_TEXCOORD0 * vec2(2.0, 2.0)) - vec2(1.0, 1.0));
  tmpvar_2.z = texture2D (_CameraDepthTexture, xlv_TEXCOORD0).x;
  vec4 tmpvar_3;
  tmpvar_3.w = 1.0;
  tmpvar_3.xyz = tmpvar_2;
  vec4 tmpvar_4;
  tmpvar_4 = (_ToPrevViewProjCombined * tmpvar_3);
  prevClipPos_1.w = tmpvar_4.w;
  prevClipPos_1.xyz = (tmpvar_4.xyz / tmpvar_4.w);
  vec2 tmpvar_5;
  tmpvar_5 = (((_MainTex_TexelSize.zw * _VelocityScale) * (tmpvar_2.xy - prevClipPos_1.xy)) / 2.0);
  float tmpvar_6;
  tmpvar_6 = sqrt(dot (tmpvar_5, tmpvar_5));
  vec4 tmpvar_7;
  tmpvar_7.zw = vec2(0.0, 0.0);
  tmpvar_7.xy = (((tmpvar_5 * 
    max (0.5, min (tmpvar_6, _MaxVelocity))
  ) / (tmpvar_6 + 0.01)) * _MainTex_TexelSize.xy);
  gl_FragData[0] = tmpvar_7;
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
// Stats: 27 math, 1 textures
Matrix 0 [_ToPrevViewProjCombined]
Vector 4 [_MainTex_TexelSize]
Float 6 [_MaxVelocity]
Float 5 [_VelocityScale]
SetTexture 0 [_CameraDepthTexture] 2D 0
""ps_3_0
def c7, 1, 2, -1, 0
def c8, 0.5, 0.00999999978, 0, 0
dcl_texcoord v0.xy
dcl_2d s0
mad r0.xy, v0, c7.y, c7.z
add r0.z, c7.x, -v0.y
cmp r1.y, c4.y, v0.y, r0.z
mov r1.x, v0.x
texld r1, r1, s0
mov r1.z, r1.x
mad r1.xyw, v0.xyzx, c7.yyzw, c7.zzzx
dp4 r0.z, c3, r1
rcp r0.z, r0.z
dp4 r2.x, c0, r1
dp4 r2.y, c1, r1
mad r0.xy, r2, -r0.z, r0
mov r0.zw, c4
mul r0.zw, r0, c5.x
mul r0.xy, r0, r0.zwzw
mul r0.xy, r0, c8.x
dp2add r0.z, r0, r0, c7.w
rsq r0.z, r0.z
rcp r0.z, r0.z
min r1.x, c6.x, r0.z
add r0.z, r0.z, c8.y
rcp r0.z, r0.z
max r0.w, c8.x, r1.x
mul r0.xy, r0.w, r0
mul r0.xy, r0.z, r0
mul oC0.xy, r0, c4
mov oC0.zw, c7.w

""
}
SubProgram ""d3d11 "" {
// Stats: 20 math, 1 textures
SetTexture 0 [_CameraDepthTexture] 2D 0
ConstBuffer ""$Globals"" 416
Matrix 288 [_ToPrevViewProjCombined]
Vector 112 [_MainTex_TexelSize]
Float 356 [_VelocityScale]
Float 364 [_MaxVelocity]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecediompiddnjmpemhlopcnajpbllbpeohhmabaaaaaaceaeaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcgeadaaaa
eaaaaaaanjaaaaaafjaaaaaeegiocaaaaaaaaaaabhaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaadbaaaaaibcaabaaaaaaaaaaa
bkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaa
bkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajccaabaaaaaaaaaaa
akaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaafbcaabaaa
aaaaaaaaakbabaaaabaaaaaaefaaaaajpcaabaaaaaaaaaaaegaabaaaaaaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaapgcaabaaaaaaaaaaaagbbbaaa
abaaaaaaaceaaaaaaaaaaaaaaaaaaaeaaaaaaaeaaaaaaaaaaceaaaaaaaaaaaaa
aaaaialpaaaaialpaaaaaaaadiaaaaaihcaabaaaabaaaaaakgakbaaaaaaaaaaa
egidcaaaaaaaaaaabdaaaaaadcaaaaakhcaabaaaabaaaaaaegidcaaaaaaaaaaa
bcaaaaaafgafbaaaaaaaaaaaegacbaaaabaaaaaadcaaaaakhcaabaaaabaaaaaa
egidcaaaaaaaaaaabeaaaaaaagaabaaaaaaaaaaaegacbaaaabaaaaaaaaaaaaai
hcaabaaaabaaaaaaegacbaaaabaaaaaaegidcaaaaaaaaaaabfaaaaaaaoaaaaah
jcaabaaaaaaaaaaaagaebaaaabaaaaaakgakbaaaabaaaaaaaaaaaaaidcaabaaa
aaaaaaaamgaabaiaebaaaaaaaaaaaaaajgafbaaaaaaaaaaadiaaaaajmcaabaaa
aaaaaaaakgiocaaaaaaaaaaaahaaaaaafgifcaaaaaaaaaaabgaaaaaadiaaaaah
dcaabaaaaaaaaaaaegaabaaaaaaaaaaaogakbaaaaaaaaaaadiaaaaakdcaabaaa
aaaaaaaaegaabaaaaaaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaa
apaaaaahecaabaaaaaaaaaaaegaabaaaaaaaaaaaegaabaaaaaaaaaaaelaaaaaf
ecaabaaaaaaaaaaackaabaaaaaaaaaaaddaaaaaiicaabaaaaaaaaaaackaabaaa
aaaaaaaadkiacaaaaaaaaaaabgaaaaaaaaaaaaahecaabaaaaaaaaaaackaabaaa
aaaaaaaaabeaaaaaaknhcddmdeaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaa
abeaaaaaaaaaaadpdiaaaaahdcaabaaaaaaaaaaapgapbaaaaaaaaaaaegaabaaa
aaaaaaaaaoaaaaahdcaabaaaaaaaaaaaegaabaaaaaaaaaaakgakbaaaaaaaaaaa
diaaaaaidccabaaaaaaaaaaaegaabaaaaaaaaaaaegiacaaaaaaaaaaaahaaaaaa
dgaaaaaimccabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
doaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 4 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 2 math, 1 texture
 //        d3d9 : 3 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 74517
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 4 math, 1 textures
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
uniform float _DisplayVelocityScale;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  vec4 cse_2;
  cse_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  tmpvar_1.x = cse_2.x;
  tmpvar_1.y = abs(cse_2.y);
  tmpvar_1.zw = -(cse_2.xy);
  gl_FragData[0] = clamp ((tmpvar_1 * _DisplayVelocityScale), 0.0, 1.0);
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
// Stats: 3 math, 1 textures
Float 0 [_DisplayVelocityScale]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c1, 1, -1, 0, 0
dcl_texcoord v0.xy
dcl_2d s0
texld r0, v0, s0
abs r1.y, r0.y
mul r1.xzw, r0.xyxy, c1.xyyy
mul_sat oC0, r1, c0.x

""
}
SubProgram ""d3d11 "" {
// Stats: 2 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 416
Float 360 [_DisplayVelocityScale]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecednjjcgfocolmndnhaeepainhbdhnibpepabaaaaaajmabaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcnmaaaaaa
eaaaaaaadhaaaaaafjaaaaaeegiocaaaaaaaaaaabhaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadgaaaaagccaabaaa
abaaaaaabkaabaiaibaaaaaaaaaaaaaadiaaaaakncaabaaaabaaaaaaagaebaaa
aaaaaaaaaceaaaaaaaaaiadpaaaaaaaaaaaaialpaaaaialpdicaaaaipccabaaa
aaaaaaaaegaobaaaabaaaaaakgikcaaaaaaaaaaabgaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 23 math, 1 texture, 5 branch
 // Stats for Fragment shader:
 //       d3d11 : 10 math, 2 branch
 //        d3d9 : 26 math, 2 texture, 10 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 147757
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 23 math, 1 textures, 5 branches
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
uniform float _MaxRadiusOrKInPaper;
uniform sampler2D _MainTex;
uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 uvScale_2;
  vec4 baseUv_3;
  vec2 maxvel_4;
  maxvel_4 = vec2(0.0, 0.0);
  vec4 tmpvar_5;
  tmpvar_5.zw = vec2(0.0, 0.0);
  tmpvar_5.xy = (xlv_TEXCOORD0 - (_MainTex_TexelSize.xy * (_MaxRadiusOrKInPaper * 0.5)));
  baseUv_3 = tmpvar_5;
  vec4 tmpvar_6;
  tmpvar_6.zw = vec2(0.0, 0.0);
  tmpvar_6.xy = _MainTex_TexelSize.xy;
  uvScale_2 = tmpvar_6;
  for (int l_1; l_1 < int(_MaxRadiusOrKInPaper); l_1++) {
    for (int k_7; k_7 < int(_MaxRadiusOrKInPaper); k_7++) {
      vec4 tmpvar_8;
      tmpvar_8.zw = vec2(0.0, 0.0);
      tmpvar_8.x = float(l_1);
      tmpvar_8.y = float(k_7);
      vec4 coord_9;
      coord_9 = (baseUv_3 + (tmpvar_8 * uvScale_2));
      vec4 tmpvar_10;
      tmpvar_10 = texture2DLod (_MainTex, coord_9.xy, coord_9.w);
      vec2 b_11;
      b_11 = tmpvar_10.xy;
      float tmpvar_12;
      tmpvar_12 = dot (maxvel_4, maxvel_4);
      float tmpvar_13;
      tmpvar_13 = dot (tmpvar_10.xy, tmpvar_10.xy);
      vec2 tmpvar_14;
      if ((tmpvar_12 > tmpvar_13)) {
        tmpvar_14 = maxvel_4;
      } else {
        tmpvar_14 = b_11;
      };
      maxvel_4 = tmpvar_14;
    };
  };
  vec4 tmpvar_15;
  tmpvar_15.zw = vec2(0.0, 1.0);
  tmpvar_15.xy = maxvel_4;
  gl_FragData[0] = tmpvar_15;
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
// Stats: 26 math, 2 textures, 10 branches
Vector 1 [_MainTex_TexelSize]
Float 0 [_MaxRadiusOrKInPaper]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c2, 0.5, 0, 1, 0
defi i0, 255, 0, 0, 0
dcl_texcoord v0.xy
dcl_2d s0
mov r0.xy, c2
mul r0.x, r0.x, c0.x
mad r0.xz, c1.xyyw, -r0.x, v0.xyyw
frc r0.w, c0.x
add r1.x, -r0.w, c0.x
cmp r0.w, -r0.w, c2.y, c2.z
cmp r0.y, c0.x, r0.y, r0.w
add r0.y, r0.y, r1.x
mov r1.zw, c2.y
mov r2.xy, c2.y
mov r3.x, c2.y
rep i0
mov r0.w, r0.y
break_ge r3.x, r0.w
mov r2.zw, r2.xyxy
mov r3.y, c2.y
rep i0
mov r0.w, r0.y
break_ge r3.y, r0.w
mad r1.xy, r3, c1, r0.xzzw
texldl r4, r1, s0
dp2add r0.w, r2.zwzw, r2.zwzw, c2.y
dp2add r0.w, r4, r4, -r0.w
cmp r2.zw, r0.w, r4.xyxy, r2
add r3.y, r3.y, c2.z
endrep
mov r2.xy, r2.zwzw
add r3.x, r3.x, c2.z
endrep
mov oC0.xy, r2
mov oC0.zw, c2.xyyz

""
}
SubProgram ""d3d11 "" {
// Stats: 10 math, 2 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 416
Float 96 [_MaxRadiusOrKInPaper]
Vector 112 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedohcicjapmfangpmnfpmomcchiophgdggabaaaaaakmadaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcomacaaaa
eaaaaaaallaaaaaafjaaaaaeegiocaaaaaaaaaaaaiaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacaeaaaaaadiaaaaaibcaabaaaaaaaaaaa
akiacaaaaaaaaaaaagaaaaaaabeaaaaaaaaaaadpdcaaaaaldcaabaaaaaaaaaaa
egiacaiaebaaaaaaaaaaaaaaahaaaaaaagaabaaaaaaaaaaaegbabaaaabaaaaaa
blaaaaagecaabaaaaaaaaaaaakiacaaaaaaaaaaaagaaaaaadgaaaaaidcaabaaa
abaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadgaaaaaficaabaaa
aaaaaaaaabeaaaaaaaaaaaaadaaaaaabcbaaaaahecaabaaaabaaaaaadkaabaaa
aaaaaaaackaabaaaaaaaaaaaadaaaeadckaabaaaabaaaaaaclaaaaafbcaabaaa
acaaaaaadkaabaaaaaaaaaaadgaaaaafmcaabaaaabaaaaaaagaebaaaabaaaaaa
dgaaaaafecaabaaaacaaaaaaabeaaaaaaaaaaaaadaaaaaabcbaaaaahicaabaaa
acaaaaaackaabaaaacaaaaaackaabaaaaaaaaaaaadaaaeaddkaabaaaacaaaaaa
claaaaafccaabaaaacaaaaaackaabaaaacaaaaaadcaaaaakkcaabaaaacaaaaaa
agaebaaaacaaaaaaagiecaaaaaaaaaaaahaaaaaaagaebaaaaaaaaaaaeiaaaaal
pcaabaaaadaaaaaangafbaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
abeaaaaaaaaaaaaaapaaaaahccaabaaaacaaaaaaogakbaaaabaaaaaaogakbaaa
abaaaaaaapaaaaahicaabaaaacaaaaaaegaabaaaadaaaaaaegaabaaaadaaaaaa
dbaaaaahccaabaaaacaaaaaadkaabaaaacaaaaaabkaabaaaacaaaaaadhaaaaaj
mcaabaaaabaaaaaafgafbaaaacaaaaaakgaobaaaabaaaaaaagaebaaaadaaaaaa
boaaaaahecaabaaaacaaaaaackaabaaaacaaaaaaabeaaaaaabaaaaaabgaaaaab
dgaaaaafdcaabaaaabaaaaaaogakbaaaabaaaaaaboaaaaahicaabaaaaaaaaaaa
dkaabaaaaaaaaaaaabeaaaaaabaaaaaabgaaaaabdgaaaaafdccabaaaaaaaaaaa
egaabaaaabaaaaaadgaaaaaimccabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaiadpdoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 39 math, 9 texture, 8 branch
 // Stats for Fragment shader:
 //       d3d11 : 29 math, 9 texture
 //        d3d9 : 47 math, 9 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 207056
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 39 math, 9 textures, 8 branches
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
  tmpvar_1 = texture2D (_MainTex, (xlv_TEXCOORD0 + _MainTex_TexelSize.xy));
  vec2 tmpvar_2;
  tmpvar_2 = tmpvar_1.xy;
  vec4 tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(1.0, 0.0) * _MainTex_TexelSize.xy)));
  vec2 b_4;
  b_4 = tmpvar_3.xy;
  float tmpvar_5;
  tmpvar_5 = dot (tmpvar_1.xy, tmpvar_1.xy);
  float tmpvar_6;
  tmpvar_6 = dot (tmpvar_3.xy, tmpvar_3.xy);
  vec2 tmpvar_7;
  if ((tmpvar_5 > tmpvar_6)) {
    tmpvar_7 = tmpvar_2;
  } else {
    tmpvar_7 = b_4;
  };
  vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(1.0, -1.0) * _MainTex_TexelSize.xy)));
  vec2 b_9;
  b_9 = tmpvar_8.xy;
  float tmpvar_10;
  tmpvar_10 = dot (tmpvar_7, tmpvar_7);
  float tmpvar_11;
  tmpvar_11 = dot (tmpvar_8.xy, tmpvar_8.xy);
  vec2 tmpvar_12;
  if ((tmpvar_10 > tmpvar_11)) {
    tmpvar_12 = tmpvar_7;
  } else {
    tmpvar_12 = b_9;
  };
  vec4 tmpvar_13;
  tmpvar_13 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, 1.0) * _MainTex_TexelSize.xy)));
  vec2 b_14;
  b_14 = tmpvar_13.xy;
  float tmpvar_15;
  tmpvar_15 = dot (tmpvar_12, tmpvar_12);
  float tmpvar_16;
  tmpvar_16 = dot (tmpvar_13.xy, tmpvar_13.xy);
  vec2 tmpvar_17;
  if ((tmpvar_15 > tmpvar_16)) {
    tmpvar_17 = tmpvar_12;
  } else {
    tmpvar_17 = b_14;
  };
  vec4 tmpvar_18;
  tmpvar_18 = texture2D (_MainTex, xlv_TEXCOORD0);
  vec2 b_19;
  b_19 = tmpvar_18.xy;
  float tmpvar_20;
  tmpvar_20 = dot (tmpvar_17, tmpvar_17);
  float tmpvar_21;
  tmpvar_21 = dot (tmpvar_18.xy, tmpvar_18.xy);
  vec2 tmpvar_22;
  if ((tmpvar_20 > tmpvar_21)) {
    tmpvar_22 = tmpvar_17;
  } else {
    tmpvar_22 = b_19;
  };
  vec4 tmpvar_23;
  tmpvar_23 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(0.0, -1.0) * _MainTex_TexelSize.xy)));
  vec2 b_24;
  b_24 = tmpvar_23.xy;
  float tmpvar_25;
  tmpvar_25 = dot (tmpvar_22, tmpvar_22);
  float tmpvar_26;
  tmpvar_26 = dot (tmpvar_23.xy, tmpvar_23.xy);
  vec2 tmpvar_27;
  if ((tmpvar_25 > tmpvar_26)) {
    tmpvar_27 = tmpvar_22;
  } else {
    tmpvar_27 = b_24;
  };
  vec4 tmpvar_28;
  tmpvar_28 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(-1.0, 1.0) * _MainTex_TexelSize.xy)));
  vec2 b_29;
  b_29 = tmpvar_28.xy;
  float tmpvar_30;
  tmpvar_30 = dot (tmpvar_27, tmpvar_27);
  float tmpvar_31;
  tmpvar_31 = dot (tmpvar_28.xy, tmpvar_28.xy);
  vec2 tmpvar_32;
  if ((tmpvar_30 > tmpvar_31)) {
    tmpvar_32 = tmpvar_27;
  } else {
    tmpvar_32 = b_29;
  };
  vec4 tmpvar_33;
  tmpvar_33 = texture2D (_MainTex, (xlv_TEXCOORD0 + (vec2(-1.0, 0.0) * _MainTex_TexelSize.xy)));
  vec2 b_34;
  b_34 = tmpvar_33.xy;
  float tmpvar_35;
  tmpvar_35 = dot (tmpvar_32, tmpvar_32);
  float tmpvar_36;
  tmpvar_36 = dot (tmpvar_33.xy, tmpvar_33.xy);
  vec2 tmpvar_37;
  if ((tmpvar_35 > tmpvar_36)) {
    tmpvar_37 = tmpvar_32;
  } else {
    tmpvar_37 = b_34;
  };
  vec4 tmpvar_38;
  tmpvar_38 = texture2D (_MainTex, (xlv_TEXCOORD0 - _MainTex_TexelSize.xy));
  vec2 b_39;
  b_39 = tmpvar_38.xy;
  float tmpvar_40;
  tmpvar_40 = dot (tmpvar_37, tmpvar_37);
  float tmpvar_41;
  tmpvar_41 = dot (tmpvar_38.xy, tmpvar_38.xy);
  vec2 tmpvar_42;
  if ((tmpvar_40 > tmpvar_41)) {
    tmpvar_42 = tmpvar_37;
  } else {
    tmpvar_42 = b_39;
  };
  vec4 tmpvar_43;
  tmpvar_43.zw = vec2(0.0, 0.0);
  tmpvar_43.xy = tmpvar_42;
  gl_FragData[0] = tmpvar_43;
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
// Stats: 47 math, 9 textures
Vector 0 [_MainTex_TexelSize]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c1, 1, 0, -1, 0
dcl_texcoord v0.xy
dcl_2d s0
add r0.xy, c0, v0
texld r0, r0, s0
dp2add r0.z, r0, r0, c1.y
mov r1.xyz, c1
mad r2, c0.xyxy, r1.xyxz, v0.xyxy
texld r3, r2, s0
texld r2, r2.zwzw, s0
dp2add r0.z, r3, r3, -r0.z
cmp r0.xy, r0.z, r3, r0
dp2add r0.z, r0, r0, c1.y
dp2add r0.z, r2, r2, -r0.z
cmp r0.xy, r0.z, r2, r0
dp2add r0.z, r0, r0, c1.y
mad r2, c0.xyxy, r1.yxyz, v0.xyxy
texld r3, r2, s0
texld r2, r2.zwzw, s0
dp2add r0.z, r3, r3, -r0.z
cmp r0.xy, r0.z, r3, r0
dp2add r0.z, r0, r0, c1.y
texld r3, v0, s0
dp2add r0.z, r3, r3, -r0.z
cmp r0.xy, r0.z, r3, r0
dp2add r0.z, r0, r0, c1.y
dp2add r0.z, r2, r2, -r0.z
cmp r0.xy, r0.z, r2, r0
dp2add r0.z, r0, r0, c1.y
mad r1, c0.xyxy, r1.zxzy, v0.xyxy
texld r2, r1, s0
texld r1, r1.zwzw, s0
dp2add r0.z, r2, r2, -r0.z
cmp r0.xy, r0.z, r2, r0
dp2add r0.z, r0, r0, c1.y
dp2add r0.z, r1, r1, -r0.z
cmp r0.xy, r0.z, r1, r0
dp2add r0.z, r0, r0, c1.y
add r1.xy, -c0, v0
texld r1, r1, s0
dp2add r0.z, r1, r1, -r0.z
cmp oC0.xy, r0.z, r1, r0
mov oC0.zw, c1.y

""
}
SubProgram ""d3d11 "" {
// Stats: 29 math, 9 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 416
Vector 112 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedomhepcgoplbellpgfnolfmpaillpnhigabaaaaaabmahaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcfmagaaaa
eaaaaaaajhabaaaafjaaaaaeegiocaaaaaaaaaaaaiaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacadaaaaaaaaaaaaaidcaabaaaaaaaaaaa
egbabaaaabaaaaaaegiacaaaaaaaaaaaahaaaaaaefaaaaajpcaabaaaaaaaaaaa
egaabaaaaaaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaapaaaaahecaabaaa
aaaaaaaaegaabaaaaaaaaaaaegaabaaaaaaaaaaadcaaaaanpcaabaaaabaaaaaa
egiecaaaaaaaaaaaahaaaaaaaceaaaaaaaaaiadpaaaaaaaaaaaaiadpaaaaialp
egbebaaaabaaaaaaefaaaaajpcaabaaaacaaaaaaegaabaaaabaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaogakbaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaapaaaaahicaabaaaaaaaaaaaegaabaaa
acaaaaaaegaabaaaacaaaaaadbaaaaahecaabaaaaaaaaaaadkaabaaaaaaaaaaa
ckaabaaaaaaaaaaadhaaaaajdcaabaaaaaaaaaaakgakbaaaaaaaaaaaegaabaaa
aaaaaaaaegaabaaaacaaaaaaapaaaaahecaabaaaaaaaaaaaegaabaaaaaaaaaaa
egaabaaaaaaaaaaaapaaaaahicaabaaaaaaaaaaaegaabaaaabaaaaaaegaabaaa
abaaaaaadbaaaaahecaabaaaaaaaaaaadkaabaaaaaaaaaaackaabaaaaaaaaaaa
dhaaaaajdcaabaaaaaaaaaaakgakbaaaaaaaaaaaegaabaaaaaaaaaaaegaabaaa
abaaaaaaapaaaaahecaabaaaaaaaaaaaegaabaaaaaaaaaaaegaabaaaaaaaaaaa
dcaaaaanpcaabaaaabaaaaaaegiecaaaaaaaaaaaahaaaaaaaceaaaaaaaaaaaaa
aaaaiadpaaaaaaaaaaaaialpegbebaaaabaaaaaaefaaaaajpcaabaaaacaaaaaa
egaabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
abaaaaaaogakbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaapaaaaah
icaabaaaaaaaaaaaegaabaaaacaaaaaaegaabaaaacaaaaaadbaaaaahecaabaaa
aaaaaaaadkaabaaaaaaaaaaackaabaaaaaaaaaaadhaaaaajdcaabaaaaaaaaaaa
kgakbaaaaaaaaaaaegaabaaaaaaaaaaaegaabaaaacaaaaaaapaaaaahecaabaaa
aaaaaaaaegaabaaaaaaaaaaaegaabaaaaaaaaaaaefaaaaajpcaabaaaacaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaapaaaaahicaabaaa
aaaaaaaaegaabaaaacaaaaaaegaabaaaacaaaaaadbaaaaahecaabaaaaaaaaaaa
dkaabaaaaaaaaaaackaabaaaaaaaaaaadhaaaaajdcaabaaaaaaaaaaakgakbaaa
aaaaaaaaegaabaaaaaaaaaaaegaabaaaacaaaaaaapaaaaahecaabaaaaaaaaaaa
egaabaaaaaaaaaaaegaabaaaaaaaaaaaapaaaaahicaabaaaaaaaaaaaegaabaaa
abaaaaaaegaabaaaabaaaaaadbaaaaahecaabaaaaaaaaaaadkaabaaaaaaaaaaa
ckaabaaaaaaaaaaadhaaaaajdcaabaaaaaaaaaaakgakbaaaaaaaaaaaegaabaaa
aaaaaaaaegaabaaaabaaaaaaapaaaaahecaabaaaaaaaaaaaegaabaaaaaaaaaaa
egaabaaaaaaaaaaadcaaaaanpcaabaaaabaaaaaaegiecaaaaaaaaaaaahaaaaaa
aceaaaaaaaaaialpaaaaiadpaaaaialpaaaaaaaaegbebaaaabaaaaaaefaaaaaj
pcaabaaaacaaaaaaegaabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
efaaaaajpcaabaaaabaaaaaaogakbaaaabaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaapaaaaahicaabaaaaaaaaaaaegaabaaaacaaaaaaegaabaaaacaaaaaa
dbaaaaahecaabaaaaaaaaaaadkaabaaaaaaaaaaackaabaaaaaaaaaaadhaaaaaj
dcaabaaaaaaaaaaakgakbaaaaaaaaaaaegaabaaaaaaaaaaaegaabaaaacaaaaaa
apaaaaahecaabaaaaaaaaaaaegaabaaaaaaaaaaaegaabaaaaaaaaaaaapaaaaah
icaabaaaaaaaaaaaegaabaaaabaaaaaaegaabaaaabaaaaaadbaaaaahecaabaaa
aaaaaaaadkaabaaaaaaaaaaackaabaaaaaaaaaaadhaaaaajdcaabaaaaaaaaaaa
kgakbaaaaaaaaaaaegaabaaaaaaaaaaaegaabaaaabaaaaaaapaaaaahecaabaaa
aaaaaaaaegaabaaaaaaaaaaaegaabaaaaaaaaaaaaaaaaaajdcaabaaaabaaaaaa
egbabaaaabaaaaaaegiacaiaebaaaaaaaaaaaaaaahaaaaaaefaaaaajpcaabaaa
abaaaaaaegaabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaapaaaaah
icaabaaaaaaaaaaaegaabaaaabaaaaaaegaabaaaabaaaaaadbaaaaahecaabaaa
aaaaaaaadkaabaaaaaaaaaaackaabaaaaaaaaaaadhaaaaajdccabaaaaaaaaaaa
kgakbaaaaaaaaaaaegaabaaaaaaaaaaaegaabaaaabaaaaaadgaaaaaimccabaaa
aaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 96 math, 8 texture, 3 branch
 // Stats for Fragment shader:
 //       d3d11 : 59 math, 3 branch
 //        d3d9 : 77 math, 16 texture, 5 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 280264
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 96 math, 8 textures, 3 branches
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
uniform vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform sampler2D _VelTex;
uniform sampler2D _NeighbourMaxTex;
uniform sampler2D _NoiseTex;
uniform float _Jitter;
uniform float _SoftZDistance;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 sum_2;
  float weight_3;
  float j_4;
  float zx_5;
  vec2 vx_6;
  vec2 vn_7;
  vec2 x_8;
  x_8 = xlv_TEXCOORD0;
  vn_7 = texture2DLod (_NeighbourMaxTex, xlv_TEXCOORD0, 0.0).xy;
  vx_6 = texture2DLod (_VelTex, xlv_TEXCOORD0, 0.0).xy;
  zx_5 = -((1.0/((
    (_ZBufferParams.x * texture2DLod (_CameraDepthTexture, xlv_TEXCOORD0, 0.0).x)
   + _ZBufferParams.y))));
  vec4 tmpvar_9;
  tmpvar_9.zw = vec2(0.0, 0.0);
  tmpvar_9.xy = xlv_TEXCOORD0;
  vec4 coord_10;
  coord_10 = (tmpvar_9 * 11.0);
  j_4 = (((texture2DLod (_NoiseTex, coord_10.xy, coord_10.w).x * 2.0) - 1.0) * _Jitter);
  weight_3 = 0.75;
  sum_2 = (texture2DLod (_MainTex, xlv_TEXCOORD0, 0.0) * 0.75);
  for (int l_1 = 0; l_1 < 11; l_1++) {
    float contrib_11;
    contrib_11 = 1.0;
    if ((l_1 == 5)) {
      contrib_11 = 0.0;
    };
    vec2 tmpvar_12;
    tmpvar_12 = (x_8 + (vn_7 * mix (-1.0, 1.0, 
      ((float(l_1) + j_4) / (10.0 + _Jitter))
    )));
    vec4 tmpvar_13;
    tmpvar_13 = texture2DLod (_VelTex, tmpvar_12, 0.0);
    float tmpvar_14;
    tmpvar_14 = -((1.0/((
      (_ZBufferParams.x * texture2DLod (_CameraDepthTexture, tmpvar_12, 0.0).x)
     + _ZBufferParams.y))));
    vec2 x_15;
    x_15 = (x_8 - tmpvar_12);
    vec2 x_16;
    x_16 = (tmpvar_12 - x_8);
    float tmpvar_17;
    tmpvar_17 = sqrt(dot (tmpvar_13.xy, tmpvar_13.xy));
    vec2 x_18;
    x_18 = (tmpvar_12 - x_8);
    float edge0_19;
    edge0_19 = (0.95 * tmpvar_17);
    float tmpvar_20;
    tmpvar_20 = clamp (((
      sqrt(dot (x_18, x_18))
     - edge0_19) / (
      (1.05 * tmpvar_17)
     - edge0_19)), 0.0, 1.0);
    float tmpvar_21;
    tmpvar_21 = sqrt(dot (vx_6, vx_6));
    vec2 x_22;
    x_22 = (x_8 - tmpvar_12);
    float edge0_23;
    edge0_23 = (0.95 * tmpvar_21);
    float tmpvar_24;
    tmpvar_24 = clamp (((
      sqrt(dot (x_22, x_22))
     - edge0_23) / (
      (1.05 * tmpvar_21)
     - edge0_23)), 0.0, 1.0);
    float tmpvar_25;
    tmpvar_25 = (((
      clamp ((1.0 - ((tmpvar_14 - zx_5) / _SoftZDistance)), 0.0, 1.0)
     * 
      clamp ((1.0 - (sqrt(
        dot (x_15, x_15)
      ) / sqrt(
        dot (vx_6, vx_6)
      ))), 0.0, 1.0)
    ) + (
      clamp ((1.0 - ((zx_5 - tmpvar_14) / _SoftZDistance)), 0.0, 1.0)
     * 
      clamp ((1.0 - (sqrt(
        dot (x_16, x_16)
      ) / sqrt(
        dot (tmpvar_13.xy, tmpvar_13.xy)
      ))), 0.0, 1.0)
    )) + ((
      (1.0 - (tmpvar_20 * (tmpvar_20 * (3.0 - 
        (2.0 * tmpvar_20)
      ))))
     * 
      (1.0 - (tmpvar_24 * (tmpvar_24 * (3.0 - 
        (2.0 * tmpvar_24)
      ))))
    ) * 2.0));
    sum_2 = (sum_2 + ((texture2DLod (_MainTex, tmpvar_12, 0.0) * tmpvar_25) * contrib_11));
    weight_3 = (weight_3 + (tmpvar_25 * contrib_11));
  };
  vec4 tmpvar_26;
  tmpvar_26 = (sum_2 / weight_3);
  sum_2 = tmpvar_26;
  gl_FragData[0] = tmpvar_26;
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
// Stats: 77 math, 16 textures, 5 branches
Float 2 [_Jitter]
Vector 1 [_MainTex_TexelSize]
Float 3 [_SoftZDistance]
Vector 0 [_ZBufferParams]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_CameraDepthTexture] 2D 1
SetTexture 2 [_VelTex] 2D 2
SetTexture 3 [_NeighbourMaxTex] 2D 3
SetTexture 4 [_NoiseTex] 2D 4
""ps_3_0
def c4, -5, 1, -2, 3
def c5, 1, 0, 11, -2
def c6, 0.75, 10, 0.0999999642, 0.949999988
defi i0, 11, 0, 0, 0
dcl_texcoord v0.xy
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
dcl_2d s4
add r0.x, c5.x, -v0.y
cmp r0.y, c1.y, v0.y, r0.x
mul r0.xzw, c5.xyyy, v0.x
texldl r1, r0.xyww, s3
mul r2, c5.xxyy, v0.xyxx
texldl r3, r2, s0
texldl r0, r0, s2
texldl r4, r2, s1
mad r0.z, c0.x, r4.x, c0.y
rcp r0.z, r0.z
mul r2, r2, c5.z
texldl r2, r2, s4
mad r0.w, r2.x, -c5.w, -c5.x
mul r2, r3, c6.x
mov r3.y, c6.y
add r1.z, r3.y, c2.x
rcp r1.z, r1.z
rcp r1.w, c3.x
dp2add r0.x, r0, r0, c5.y
rsq r0.x, r0.x
rcp r0.y, r0.x
mul r3.x, r0.y, c6.z
rcp r3.x, r3.x
mov r4.w, c5.y
mov r5, r2
mov r3.y, c6.x
mov r3.z, c5.y
rep i0
add r6.xy, r3.z, c4
cmp r3.w, -r6_abs.x, c5.y, c5.x
mad r6.x, r0.w, c2.x, r3.z
dp2add r6.x, r6.x, r1.z, -c5.x
mul r6.zw, r1.xyxy, r6.x
mad r4.xy, r1, r6.x, v0
add r6.x, -r4.y, c5.x
cmp r4.z, c1.y, r4.y, r6.x
texldl r7, r4.xzww, s2
texldl r8, r4.xyww, s1
mad r4.z, c0.x, r8.x, c0.y
rcp r4.z, r4.z
add r6.x, -r0.z, r4.z
mad_sat r6.x, r6.x, -r1.w, c5.x
add r4.z, r0.z, -r4.z
mad_sat r4.z, r4.z, -r1.w, c5.x
add r7.zw, -r4.xyxy, v0.xyxy
dp2add r7.z, r7.zwzw, r7.zwzw, c5.y
rsq r7.z, r7.z
rcp r7.z, r7.z
mad r7.w, r7.z, -r0.x, c5.x
max r8.x, r7.w, c5.y
dp2add r6.z, r6.zwzw, r6.zwzw, c5.y
rsq r6.z, r6.z
rcp r6.z, r6.z
dp2add r6.w, r7, r7, c5.y
rsq r6.w, r6.w
mad r7.x, r6.z, -r6.w, c5.x
max r8.y, r7.x, c5.y
mul r6.x, r6.x, r8.y
mad r4.z, r4.z, r8.x, r6.x
rcp r6.x, r6.w
mul r6.w, r6.x, c6.z
mad r6.x, r6.x, -c6.w, r6.z
rcp r6.z, r6.w
mul_sat r6.x, r6.z, r6.x
mad r6.z, r6.x, c4.z, c4.w
mul r6.x, r6.x, r6.x
mad r6.x, r6.z, -r6.x, c5.x
mad r6.z, r0.y, -c6.w, r7.z
mul_sat r6.z, r3.x, r6.z
mad r6.w, r6.z, c4.z, c4.w
mul r6.z, r6.z, r6.z
mad r6.z, r6.w, -r6.z, c5.x
dp2add r4.z, r6.x, r6.z, r4.z
texldl r7, r4.xyww, s0
mul r7, r4.z, r7
mad r5, r7, r3.w, r5
mad r3.y, r4.z, r3.w, r3.y
mov r3.z, r6.y
endrep
rcp r0.x, r3.y
mul oC0, r0.x, r5

""
}
SubProgram ""d3d11 "" {
// Stats: 59 math, 3 branches
SetTexture 0 [_NeighbourMaxTex] 2D 3
SetTexture 1 [_MainTex] 2D 0
SetTexture 2 [_VelTex] 2D 2
SetTexture 3 [_CameraDepthTexture] 2D 1
SetTexture 4 [_NoiseTex] 2D 4
ConstBuffer ""$Globals"" 416
Vector 112 [_MainTex_TexelSize]
Float 352 [_Jitter]
Float 400 [_SoftZDistance]
ConstBuffer ""UnityPerCamera"" 144
Vector 112 [_ZBufferParams]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerCamera"" 1
""ps_4_0
eefiecedaabaconahoeklapanfadmbkfoedpoiipabaaaaaaemalaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcimakaaaa
eaaaaaaakdacaaaafjaaaaaeegiocaaaaaaaaaaabkaaaaaafjaaaaaeegiocaaa
abaaaaaaaiaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaa
fkaaaaadaagabaaaacaaaaaafkaaaaadaagabaaaadaaaaaafkaaaaadaagabaaa
aeaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaa
ffffaaaafibiaaaeaahabaaaacaaaaaaffffaaaafibiaaaeaahabaaaadaaaaaa
ffffaaaafibiaaaeaahabaaaaeaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacajaaaaaadbaaaaaibcaabaaaaaaaaaaa
bkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaa
bkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajccaabaaaabaaaaaa
akaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaafbcaabaaa
abaaaaaaakbabaaaabaaaaaaeiaaaaalpcaabaaaacaaaaaaegaabaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaadaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaa
adaaaaaaegbabaaaabaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaabeaaaaa
aaaaaaaaeiaaaaalpcaabaaaabaaaaaaegaabaaaabaaaaaaeghobaaaacaaaaaa
aagabaaaacaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaaaeaaaaaaegbabaaa
abaaaaaaeghobaaaadaaaaaaaagabaaaabaaaaaaabeaaaaaaaaaaaaadcaaaaal
ccaabaaaaaaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaaeaaaaaabkiacaaa
abaaaaaaahaaaaaaaoaaaaakccaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaiadpaaaaiadpbkaabaaaaaaaaaaadiaaaaakmcaabaaaaaaaaaaaagbebaaa
abaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaadaebaaaadaebeiaaaaalpcaabaaa
aeaaaaaaogakbaaaaaaaaaaaeghobaaaaeaaaaaaaagabaaaaeaaaaaaabeaaaaa
aaaaaaaadcaaaaajecaabaaaaaaaaaaaakaabaaaaeaaaaaaabeaaaaaaaaaaaea
abeaaaaaaaaaialpdiaaaaakpcaabaaaadaaaaaaegaobaaaadaaaaaaaceaaaaa
aaaaeadpaaaaeadpaaaaeadpaaaaeadpaaaaaaaiicaabaaaaaaaaaaaakiacaaa
aaaaaaaabgaaaaaaabeaaaaaaaaacaebapaaaaahbcaabaaaabaaaaaaegaabaaa
abaaaaaaegaabaaaabaaaaaaelaaaaafbcaabaaaabaaaaaaakaabaaaabaaaaaa
diaaaaahccaabaaaabaaaaaaakaabaaaabaaaaaaabeaaaaamimmmmdnaoaaaaak
ccaabaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpbkaabaaa
abaaaaaadgaaaaafpcaabaaaaeaaaaaaegaobaaaadaaaaaadgaaaaaimcaabaaa
abaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaeadpaaaaaaaadaaaaaabcbaaaaah
ecaabaaaacaaaaaadkaabaaaabaaaaaaabeaaaaaalaaaaaaadaaaeadckaabaaa
acaaaaaacaaaaaahecaabaaaacaaaaaadkaabaaaabaaaaaaabeaaaaaafaaaaaa
bpaaaeadckaabaaaacaaaaaadgaaaaaficaabaaaabaaaaaaabeaaaaaagaaaaaa
ahaaaaabbfaaaaabclaaaaafecaabaaaacaaaaaadkaabaaaabaaaaaadcaaaaak
ecaabaaaacaaaaaackaabaaaaaaaaaaaakiacaaaaaaaaaaabgaaaaaackaabaaa
acaaaaaaaoaaaaahecaabaaaacaaaaaackaabaaaacaaaaaadkaabaaaaaaaaaaa
dcaaaaajecaabaaaacaaaaaackaabaaaacaaaaaaabeaaaaaaaaaaaeaabeaaaaa
aaaaialpdiaaaaahdcaabaaaafaaaaaakgakbaaaacaaaaaaegaabaaaacaaaaaa
dcaaaaajdcaabaaaagaaaaaaegaabaaaacaaaaaakgakbaaaacaaaaaaegbabaaa
abaaaaaaaaaaaaaiecaabaaaacaaaaaabkaabaiaebaaaaaaagaaaaaaabeaaaaa
aaaaiadpdhaaaaajecaabaaaagaaaaaaakaabaaaaaaaaaaackaabaaaacaaaaaa
bkaabaaaagaaaaaaeiaaaaalpcaabaaaahaaaaaaigaabaaaagaaaaaaeghobaaa
acaaaaaaaagabaaaacaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaaaiaaaaaa
egaabaaaagaaaaaaeghobaaaadaaaaaaaagabaaaabaaaaaaabeaaaaaaaaaaaaa
dcaaaaalecaabaaaacaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaaiaaaaaa
bkiacaaaabaaaaaaahaaaaaaaoaaaaakecaabaaaacaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaiadpaaaaiadpckaabaaaacaaaaaaaaaaaaaiicaabaaaacaaaaaa
bkaabaiaebaaaaaaaaaaaaaackaabaaaacaaaaaaaoaaaaaiicaabaaaacaaaaaa
dkaabaaaacaaaaaaakiacaaaaaaaaaaabjaaaaaaaaaaaaaiecaabaaaacaaaaaa
bkaabaaaaaaaaaaackaabaiaebaaaaaaacaaaaaaaoaaaaaiecaabaaaacaaaaaa
ckaabaaaacaaaaaaakiacaaaaaaaaaaabjaaaaaaaacaaaalmcaabaaaacaaaaaa
kgaobaiaebaaaaaaacaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaiadpaaaaiadp
aaaaaaaimcaabaaaafaaaaaaagaebaiaebaaaaaaagaaaaaaagbebaaaabaaaaaa
apaaaaahecaabaaaafaaaaaaogakbaaaafaaaaaaogakbaaaafaaaaaaelaaaaaf
ecaabaaaafaaaaaackaabaaaafaaaaaaaoaaaaahicaabaaaafaaaaaackaabaaa
afaaaaaaakaabaaaabaaaaaaaaaaaaaiicaabaaaafaaaaaadkaabaiaebaaaaaa
afaaaaaaabeaaaaaaaaaiadpdeaaaaahicaabaaaafaaaaaadkaabaaaafaaaaaa
abeaaaaaaaaaaaaaapaaaaahbcaabaaaafaaaaaaegaabaaaafaaaaaaegaabaaa
afaaaaaaapaaaaahccaabaaaafaaaaaaegaabaaaahaaaaaaegaabaaaahaaaaaa
elaaaaafdcaabaaaafaaaaaaegaabaaaafaaaaaaaoaaaaahecaabaaaagaaaaaa
akaabaaaafaaaaaabkaabaaaafaaaaaaaaaaaaaiecaabaaaagaaaaaackaabaia
ebaaaaaaagaaaaaaabeaaaaaaaaaiadpdeaaaaahecaabaaaagaaaaaackaabaaa
agaaaaaaabeaaaaaaaaaaaaadiaaaaahicaabaaaacaaaaaadkaabaaaacaaaaaa
ckaabaaaagaaaaaadcaaaaajecaabaaaacaaaaaackaabaaaacaaaaaadkaabaaa
afaaaaaadkaabaaaacaaaaaadiaaaaahicaabaaaacaaaaaabkaabaaaafaaaaaa
abeaaaaamimmmmdndcaaaaakbcaabaaaafaaaaaabkaabaiaebaaaaaaafaaaaaa
abeaaaaaddddhddpakaabaaaafaaaaaaaoaaaaakicaabaaaacaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaiadpaaaaiadpdkaabaaaacaaaaaadicaaaahicaabaaa
acaaaaaadkaabaaaacaaaaaaakaabaaaafaaaaaadcaaaaajbcaabaaaafaaaaaa
dkaabaaaacaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaeadiaaaaahicaabaaa
acaaaaaadkaabaaaacaaaaaadkaabaaaacaaaaaadcaaaaakicaabaaaacaaaaaa
akaabaiaebaaaaaaafaaaaaadkaabaaaacaaaaaaabeaaaaaaaaaiadpdcaaaaak
bcaabaaaafaaaaaaakaabaiaebaaaaaaabaaaaaaabeaaaaaddddhddpckaabaaa
afaaaaaadicaaaahbcaabaaaafaaaaaabkaabaaaabaaaaaaakaabaaaafaaaaaa
dcaaaaajccaabaaaafaaaaaaakaabaaaafaaaaaaabeaaaaaaaaaaamaabeaaaaa
aaaaeaeadiaaaaahbcaabaaaafaaaaaaakaabaaaafaaaaaaakaabaaaafaaaaaa
dcaaaaakbcaabaaaafaaaaaabkaabaiaebaaaaaaafaaaaaaakaabaaaafaaaaaa
abeaaaaaaaaaiadpapaaaaahicaabaaaacaaaaaapgapbaaaacaaaaaaagaabaaa
afaaaaaaaaaaaaahecaabaaaacaaaaaadkaabaaaacaaaaaackaabaaaacaaaaaa
eiaaaaalpcaabaaaafaaaaaaegaabaaaagaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaadcaaaaajpcaabaaaaeaaaaaaegaobaaaafaaaaaa
kgakbaaaacaaaaaaegaobaaaaeaaaaaaaaaaaaahecaabaaaabaaaaaackaabaaa
abaaaaaackaabaaaacaaaaaaboaaaaahicaabaaaabaaaaaadkaabaaaabaaaaaa
abeaaaaaabaaaaaabgaaaaabaoaaaaahpccabaaaaaaaaaaaegaobaaaaeaaaaaa
kgakbaaaabaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 11 math, 2 texture, 2 branch
 // Stats for Fragment shader:
 //       d3d11 : 8 math, 2 texture, 1 branch
 //        d3d9 : 10 math, 2 texture, 5 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 367485
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 11 math, 2 textures, 2 branches
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
uniform sampler2D _VelTex;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 sum_2;
  vec2 vx_3;
  vec2 x_4;
  x_4 = xlv_TEXCOORD0;
  vx_3 = texture2D (_VelTex, xlv_TEXCOORD0).xy;
  sum_2 = vec4(0.0, 0.0, 0.0, 0.0);
  for (int l_1 = 0; l_1 < 11; l_1++) {
    sum_2 = (sum_2 + texture2D (_MainTex, (x_4 - (vx_3 * 
      ((float(l_1) / 10.0) - 0.5)
    ))));
  };
  vec4 tmpvar_5;
  tmpvar_5 = (sum_2 / 11.0);
  sum_2 = tmpvar_5;
  gl_FragData[0] = tmpvar_5;
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
// Stats: 10 math, 2 textures, 5 branches
Vector 0 [_MainTex_TexelSize]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_VelTex] 2D 1
""ps_3_0
def c1, 0.100000001, -0.5, 0, 0
def c2, 1, 0, 0, 0.0909090936
defi i0, 11, 0, 0, 0
dcl_texcoord v0.xy
dcl_2d s0
dcl_2d s1
add r0.x, c2.x, -v0.y
cmp r0.y, c0.y, v0.y, r0.x
mov r0.x, v0.x
texld r0, r0, s1
mov r1, c2.z
mov r0.z, c2.z
rep i0
mad r0.w, r0.z, c1.x, c1.y
mad r2.xy, r0, -r0.w, v0
texld r2, r2, s0
add r1, r1, r2
add r0.z, r0.z, c2.x
endrep
mul oC0, r1, c2.w

""
}
SubProgram ""d3d11 "" {
// Stats: 8 math, 2 textures, 1 branches
SetTexture 0 [_VelTex] 2D 1
SetTexture 1 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 416
Vector 112 [_MainTex_TexelSize]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedonghhknbmaieccdenplnjnlcmnefdjboabaaaaaabiadaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcfiacaaaa
eaaaaaaajgaaaaaafjaaaaaeegiocaaaaaaaaaaaaiaaaaaafkaaaaadaagabaaa
aaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaa
fibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagfaaaaad
pccabaaaaaaaaaaagiaaaaacadaaaaaadbaaaaaibcaabaaaaaaaaaaabkiacaaa
aaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaabkbabaia
ebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajccaabaaaaaaaaaaaakaabaaa
aaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaafbcaabaaaaaaaaaaa
akbabaaaabaaaaaaefaaaaajpcaabaaaaaaaaaaaegaabaaaaaaaaaaaeghobaaa
aaaaaaaaaagabaaaabaaaaaadgaaaaaipcaabaaaabaaaaaaaceaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaadgaaaaafecaabaaaaaaaaaaaabeaaaaaaaaaaaaa
daaaaaabcbaaaaahicaabaaaaaaaaaaackaabaaaaaaaaaaaabeaaaaaalaaaaaa
adaaaeaddkaabaaaaaaaaaaaclaaaaaficaabaaaaaaaaaaackaabaaaaaaaaaaa
dcaaaaajicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaamnmmmmdnabeaaaaa
aaaaaalpdcaaaaakdcaabaaaacaaaaaaegaabaiaebaaaaaaaaaaaaaapgapbaaa
aaaaaaaaegbabaaaabaaaaaaefaaaaajpcaabaaaacaaaaaaegaabaaaacaaaaaa
eghobaaaabaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaabaaaaaaegaobaaa
abaaaaaaegaobaaaacaaaaaaboaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaa
abeaaaaaabaaaaaabgaaaaabdiaaaaakpccabaaaaaaaaaaaegaobaaaabaaaaaa
aceaaaaaimcolkdnimcolkdnimcolkdnimcolkdndoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 40 math, 2 texture, 3 branch
 // Stats for Fragment shader:
 //       d3d11 : 29 math, 2 texture, 1 branch
 //        d3d9 : 37 math, 2 texture, 5 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 416503
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 40 math, 2 textures, 3 branches
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
uniform float _VelocityScale;
uniform float _MaxVelocity;
uniform float _MinVelocity;
uniform vec4 _BlurDirectionPacked;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 sum_2;
  float velMag_3;
  vec2 blurDir_4;
  vec2 x_5;
  x_5 = xlv_TEXCOORD0;
  vec2 tmpvar_6;
  tmpvar_6.x = 1.0;
  tmpvar_6.y = (_MainTex_TexelSize.w / _MainTex_TexelSize.z);
  vec2 tmpvar_7;
  tmpvar_7 = (((xlv_TEXCOORD0 * 2.0) - 1.0) * tmpvar_6);
  vec2 tmpvar_8;
  tmpvar_8.x = tmpvar_7.y;
  tmpvar_8.y = -(tmpvar_7.x);
  vec2 tmpvar_9;
  tmpvar_9 = (((
    ((_BlurDirectionPacked.x * vec2(0.0, 1.0)) + (_BlurDirectionPacked.y * vec2(1.0, 0.0)))
   + 
    (_BlurDirectionPacked.z * tmpvar_8)
  ) + (_BlurDirectionPacked.w * tmpvar_7)) * _VelocityScale);
  blurDir_4 = tmpvar_9;
  float tmpvar_10;
  tmpvar_10 = sqrt(dot (tmpvar_9, tmpvar_9));
  velMag_3 = tmpvar_10;
  if ((tmpvar_10 > _MaxVelocity)) {
    blurDir_4 = (tmpvar_9 * (_MaxVelocity / tmpvar_10));
    velMag_3 = _MaxVelocity;
  };
  sum_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  float edge0_11;
  edge0_11 = (_MinVelocity * 0.25);
  float tmpvar_12;
  tmpvar_12 = clamp (((velMag_3 - edge0_11) / (
    (_MinVelocity * 2.5)
   - edge0_11)), 0.0, 1.0);
  blurDir_4 = (((blurDir_4 * 
    (tmpvar_12 * (tmpvar_12 * (3.0 - (2.0 * tmpvar_12))))
  ) * _MainTex_TexelSize.xy) / 16.0);
  for (int i_1_1 = 0; i_1_1 < 16; i_1_1++) {
    sum_2 = (sum_2 + texture2D (_MainTex, (x_5 + (
      float(i_1_1)
     * blurDir_4))));
  };
  gl_FragData[0] = (sum_2 / 17.0);
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
// Stats: 37 math, 2 textures, 5 branches
Vector 4 [_BlurDirectionPacked]
Vector 0 [_MainTex_TexelSize]
Float 2 [_MaxVelocity]
Float 3 [_MinVelocity]
Float 1 [_VelocityScale]
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c5, 2, -1, 1, 0
def c6, 2.25, 0.25, -2, 3
def c7, 0.0625, 0, 0.0588235296, 0
defi i0, 16, 0, 0, 0
dcl_texcoord v0.xy
dcl_2d s0
mad r0.xy, v0, c5.x, c5.y
rcp r0.z, c0.z
mul r1.y, r0.z, c0.w
mov r1.xzw, c5.zyzw
mul r0.xy, r0, r1
mul r1, r1.wzzw, c4.xxyy
add r1.xy, r1.zwzw, r1
mov r0.z, -r0.x
mad r0.zw, c4.z, r0.xyyz, r1.xyxy
mad r0.xy, c4.w, r0, r0.zwzw
mul r0.xy, r0, c1.x
dp2add r0.w, r0, r0, c5.w
rsq r0.w, r0.w
rcp r0.z, r0.w
add r1.x, -r0.z, c2.x
mul r0.w, r0.w, c2.x
mul r2.xy, r0.w, r0
mov r2.z, c2.x
cmp r0.xyz, r1.x, r0, r2
texld r1, v0, s0
mov r2.xy, c6
mul r0.w, r2.x, c3.x
mad r0.z, c3.x, -r2.y, r0.z
rcp r0.w, r0.w
mul_sat r0.z, r0.w, r0.z
mad r0.w, r0.z, c6.z, c6.w
mul r0.z, r0.z, r0.z
mul r0.z, r0.z, r0.w
mul r0.xy, r0.z, r0
mul r0.xy, r0, c0
mul r0.xy, r0, c7.x
mov r2, r1
mov r0.z, c5.w
rep i0
mad r3.xy, r0.z, r0, v0
texld r3, r3, s0
add r2, r2, r3
add r0.z, r0.z, c5.z
endrep
mul oC0, r2, c7.z

""
}
SubProgram ""d3d11 "" {
// Stats: 29 math, 2 textures, 1 branches
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 416
Vector 112 [_MainTex_TexelSize]
Float 356 [_VelocityScale]
Float 364 [_MaxVelocity]
Float 368 [_MinVelocity]
Vector 384 [_BlurDirectionPacked]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedmpjakopgopaebhlocnebbgpdmbakjncdabaaaaaamiafaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcaiafaaaa
eaaaaaaaecabaaaafjaaaaaeegiocaaaaaaaaaaabjaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacaeaaaaaadcaaaaapdcaabaaaaaaaaaaa
egbabaaaabaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaaaaaaaaaaaaceaaaaa
aaaaialpaaaaialpaaaaaaaaaaaaaaaaaoaaaaajccaabaaaabaaaaaadkiacaaa
aaaaaaaaahaaaaaackiacaaaaaaaaaaaahaaaaaadgaaaaafbcaabaaaabaaaaaa
abeaaaaaaaaaiadpdiaaaaahdcaabaaaaaaaaaaaegaabaaaaaaaaaaaegaabaaa
abaaaaaadiaaaaalpcaabaaaabaaaaaaagifcaaaaaaaaaaabiaaaaaaaceaaaaa
aaaaaaaaaaaaiadpaaaaiadpaaaaaaaaaaaaaaahdcaabaaaabaaaaaaogakbaaa
abaaaaaaegaabaaaabaaaaaadgaaaaagecaabaaaaaaaaaaaakaabaiaebaaaaaa
aaaaaaaadcaaaaakmcaabaaaaaaaaaaakgikcaaaaaaaaaaabiaaaaaafgajbaaa
aaaaaaaaagaebaaaabaaaaaadcaaaaakdcaabaaaaaaaaaaapgipcaaaaaaaaaaa
biaaaaaaegaabaaaaaaaaaaaogakbaaaaaaaaaaadiaaaaaidcaabaaaaaaaaaaa
egaabaaaaaaaaaaafgifcaaaaaaaaaaabgaaaaaaapaaaaahicaabaaaaaaaaaaa
egaabaaaaaaaaaaaegaabaaaaaaaaaaaelaaaaafecaabaaaaaaaaaaadkaabaaa
aaaaaaaadbaaaaaiicaabaaaaaaaaaaadkiacaaaaaaaaaaabgaaaaaackaabaaa
aaaaaaaaaoaaaaaibcaabaaaabaaaaaadkiacaaaaaaaaaaabgaaaaaackaabaaa
aaaaaaaadiaaaaahdcaabaaaabaaaaaaegaabaaaaaaaaaaaagaabaaaabaaaaaa
dgaaaaagecaabaaaabaaaaaadkiacaaaaaaaaaaabgaaaaaadhaaaaajhcaabaaa
aaaaaaaapgapbaaaaaaaaaaaegacbaaaabaaaaaaegacbaaaaaaaaaaaefaaaaaj
pcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
diaaaaaiicaabaaaaaaaaaaaakiacaaaaaaaaaaabhaaaaaaabeaaaaaaaaabaea
dcaaaaalecaabaaaaaaaaaaaakiacaiaebaaaaaaaaaaaaaabhaaaaaaabeaaaaa
aaaaiadockaabaaaaaaaaaaaaoaaaaakicaabaaaaaaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaiadpaaaaiadpdkaabaaaaaaaaaaadicaaaahecaabaaaaaaaaaaa
dkaabaaaaaaaaaaackaabaaaaaaaaaaadcaaaaajicaabaaaaaaaaaaackaabaaa
aaaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaeadiaaaaahecaabaaaaaaaaaaa
ckaabaaaaaaaaaaackaabaaaaaaaaaaadiaaaaahecaabaaaaaaaaaaackaabaaa
aaaaaaaadkaabaaaaaaaaaaadiaaaaahdcaabaaaaaaaaaaakgakbaaaaaaaaaaa
egaabaaaaaaaaaaadiaaaaaidcaabaaaaaaaaaaaegaabaaaaaaaaaaaegiacaaa
aaaaaaaaahaaaaaadiaaaaakdcaabaaaaaaaaaaaegaabaaaaaaaaaaaaceaaaaa
aaaaiadnaaaaiadnaaaaaaaaaaaaaaaadgaaaaafpcaabaaaacaaaaaaegaobaaa
abaaaaaadgaaaaafecaabaaaaaaaaaaaabeaaaaaaaaaaaaadaaaaaabcbaaaaah
icaabaaaaaaaaaaackaabaaaaaaaaaaaabeaaaaabaaaaaaaadaaaeaddkaabaaa
aaaaaaaaclaaaaaficaabaaaaaaaaaaackaabaaaaaaaaaaadcaaaaajdcaabaaa
adaaaaaapgapbaaaaaaaaaaaegaabaaaaaaaaaaaegbabaaaabaaaaaaefaaaaaj
pcaabaaaadaaaaaaegaabaaaadaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
aaaaaaahpcaabaaaacaaaaaaegaobaaaacaaaaaaegaobaaaadaaaaaaboaaaaah
ecaabaaaaaaaaaaackaabaaaaaaaaaaaabeaaaaaabaaaaaabgaaaaabdiaaaaak
pccabaaaaaaaaaaaegaobaaaacaaaaaaaceaaaaapbpahadnpbpahadnpbpahadn
pbpahadndoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 107 math, 8 texture, 2 branch
 // Stats for Fragment shader:
 //       d3d11 : 99 math, 1 branch
 //        d3d9 : 93 math, 16 texture, 5 branch
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 463209
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 107 math, 8 textures, 2 branches
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
uniform vec4 _ZBufferParams;
vec2 SmallDiscKernel[12];
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform sampler2D _VelTex;
uniform sampler2D _NeighbourMaxTex;
uniform sampler2D _NoiseTex;
uniform vec4 _MainTex_TexelSize;
uniform float _Jitter;
uniform float _MaxVelocity;
uniform float _SoftZDistance;
varying vec2 xlv_TEXCOORD0;
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
  tmpvar_1 = xlv_TEXCOORD0;
  vec4 jitteredDir_3;
  vec4 sum_4;
  float weight_5;
  float zx_6;
  vec2 vx_7;
  vec2 x_8;
  x_8 = xlv_TEXCOORD0;
  vx_7 = texture2DLod (_VelTex, xlv_TEXCOORD0, 0.0).xy;
  vec4 tmpvar_9;
  tmpvar_9.zw = vec2(0.0, 0.0);
  tmpvar_9.xy = xlv_TEXCOORD0;
  vec4 coord_10;
  coord_10 = (tmpvar_9 * 11.0);
  zx_6 = -((1.0/((
    (_ZBufferParams.x * texture2DLod (_CameraDepthTexture, xlv_TEXCOORD0, 0.0).x)
   + _ZBufferParams.y))));
  weight_5 = 1.0;
  sum_4 = texture2DLod (_MainTex, xlv_TEXCOORD0, 0.0);
  vec4 tmpvar_11;
  tmpvar_11 = (texture2DLod (_NeighbourMaxTex, xlv_TEXCOORD0, 0.0).xyxy + ((
    (texture2DLod (_NoiseTex, coord_10.xy, coord_10.w) * 2.0)
   - 1.0) * (_MainTex_TexelSize.xyxy * _Jitter)).xyyz);
  jitteredDir_3 = ((max (
    abs(tmpvar_11.xyxy)
  , 
    ((_MainTex_TexelSize.xyxy * _MaxVelocity) * 0.15)
  ) * sign(tmpvar_11.xyxy)) * vec4(1.0, 1.0, -1.0, -1.0));
  for (int l_2 = 0; l_2 < 12; l_2++) {
    vec4 tmpvar_12;
    tmpvar_12 = (tmpvar_1.xyxy + ((jitteredDir_3.xyxy * SmallDiscKernel[l_2].xyxy) * vec4(1.0, 1.0, -1.0, -1.0)));
    vec4 tmpvar_13;
    tmpvar_13 = texture2DLod (_VelTex, tmpvar_12.xy, 0.0);
    float tmpvar_14;
    tmpvar_14 = -((1.0/((
      (_ZBufferParams.x * texture2DLod (_CameraDepthTexture, tmpvar_12.xy, 0.0).x)
     + _ZBufferParams.y))));
    vec2 x_15;
    x_15 = (x_8 - tmpvar_12.xy);
    vec2 x_16;
    x_16 = (tmpvar_12.xy - x_8);
    float tmpvar_17;
    tmpvar_17 = sqrt(dot (tmpvar_13.xy, tmpvar_13.xy));
    vec2 x_18;
    x_18 = (tmpvar_12.xy - x_8);
    float edge0_19;
    edge0_19 = (0.95 * tmpvar_17);
    float tmpvar_20;
    tmpvar_20 = clamp (((
      sqrt(dot (x_18, x_18))
     - edge0_19) / (
      (1.05 * tmpvar_17)
     - edge0_19)), 0.0, 1.0);
    float tmpvar_21;
    tmpvar_21 = sqrt(dot (vx_7, vx_7));
    vec2 x_22;
    x_22 = (x_8 - tmpvar_12.xy);
    float edge0_23;
    edge0_23 = (0.95 * tmpvar_21);
    float tmpvar_24;
    tmpvar_24 = clamp (((
      sqrt(dot (x_22, x_22))
     - edge0_23) / (
      (1.05 * tmpvar_21)
     - edge0_23)), 0.0, 1.0);
    float tmpvar_25;
    tmpvar_25 = (((
      clamp ((1.0 - ((tmpvar_14 - zx_6) / _SoftZDistance)), 0.0, 1.0)
     * 
      clamp ((1.0 - (sqrt(
        dot (x_15, x_15)
      ) / sqrt(
        dot (vx_7, vx_7)
      ))), 0.0, 1.0)
    ) + (
      clamp ((1.0 - ((zx_6 - tmpvar_14) / _SoftZDistance)), 0.0, 1.0)
     * 
      clamp ((1.0 - (sqrt(
        dot (x_16, x_16)
      ) / sqrt(
        dot (tmpvar_13.xy, tmpvar_13.xy)
      ))), 0.0, 1.0)
    )) + ((
      (1.0 - (tmpvar_20 * (tmpvar_20 * (3.0 - 
        (2.0 * tmpvar_20)
      ))))
     * 
      (1.0 - (tmpvar_24 * (tmpvar_24 * (3.0 - 
        (2.0 * tmpvar_24)
      ))))
    ) * 2.0));
    sum_4 = (sum_4 + (texture2DLod (_MainTex, tmpvar_12.xy, 0.0) * tmpvar_25));
    weight_5 = (weight_5 + tmpvar_25);
  };
  gl_FragData[0] = (sum_4 / weight_5);
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
// Stats: 93 math, 16 textures, 5 branches
Float 2 [_Jitter]
Vector 1 [_MainTex_TexelSize]
Float 3 [_MaxVelocity]
Float 4 [_SoftZDistance]
Vector 0 [_ZBufferParams]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_CameraDepthTexture] 2D 1
SetTexture 2 [_VelTex] 2D 2
SetTexture 3 [_NeighbourMaxTex] 2D 3
SetTexture 4 [_NoiseTex] 2D 4
""ps_3_0
def c5, 1, 0, 11, -2
def c6, 0.150000006, 0.0999999642, 0, 0.949999988
def c7, -0, -1, -2, -3
def c8, -4, -5, -6, -7
def c9, -8, -9, -10, -11
def c10, -0.791558981, -0.597710013, -2, 3
def c11, -0.326211989, -0.405809999, 0, 0
def c12, -0.840143979, -0.0735799968, -0.69591397, 0.457136989
def c13, -0.203345001, 0.620715976, 0.962339997, -0.194983006
def c14, 0.473434001, -0.480026007, 0.519456029, 0.767022014
def c15, 0.185461, -0.893123984, 0.507430971, 0.0644249991
def c16, 0.896420002, 0.412458003, -0.321940005, -0.932614982
defi i0, 12, 0, 0, 0
dcl_texcoord v0.xy
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
dcl_2d s4
add r0.x, c5.x, -v0.y
cmp r0.y, c1.y, v0.y, r0.x
mul r0.xzw, c5.xyyy, v0.x
texldl r1, r0.xyww, s3
mul r2, c5.xxyy, v0.xyxx
texldl r3, r2, s0
texldl r0, r0, s2
mul r4, r2, c5.z
texldl r4, r4, s4
mad r0.zw, r4.xyxy, -c5.w, -c5.x
texldl r2, r2, s1
mad r1.z, c0.x, r2.x, c0.y
rcp r1.z, r1.z
mov r2.xy, c1
mul r2.zw, r2.xyxy, c2.x
mad r0.zw, r0, r2, r1.xyxy
mul r1.xy, r2, c3.x
mul r1.xy, r1, c6.x
max r2.xy, r0_abs.zwzw, r1
cmp r1.xy, -r0.zwzw, c5.y, c5.x
cmp r0.zw, r0, -c5.y, -c5.x
add r0.zw, r0, r1.xyxy
mul r0.zw, r0, r2.xyxy
rcp r1.x, c4.x
dp2add r0.x, r0, r0, c5.y
rsq r0.x, r0.x
rcp r0.y, r0.x
mul r1.y, r0.y, c6.y
rcp r1.y, r1.y
mov r2.w, c5.y
mov r4, r3
mov r1.w, c5.x
mov r5.x, c5.y
rep i0
add r6, r5.x, c7
add r7, r5.x, c8
add r8, r5.x, c9
cmp r5.yz, -r6_abs.x, c11.xxyw, c11.z
cmp r5.yz, -r6_abs.y, c12.xxyw, r5
cmp r5.yz, -r6_abs.z, c12.xzww, r5
cmp r5.yz, -r6_abs.w, c13.xxyw, r5
cmp r5.yz, -r7_abs.x, c13.xzww, r5
cmp r5.yz, -r7_abs.y, c14.xxyw, r5
cmp r5.yz, -r7_abs.z, c14.xzww, r5
cmp r5.yz, -r7_abs.w, c15.xxyw, r5
cmp r5.yz, -r8_abs.x, c15.xzww, r5
cmp r5.yz, -r8_abs.y, c16.xxyw, r5
cmp r5.yz, -r8_abs.z, c16.xzww, r5
cmp r5.yz, -r8_abs.w, c10.xxyw, r5
mul r6.xy, r0.zwzw, r5.yzzw
mad r2.xy, r0.zwzw, r5.yzzw, v0
add r5.y, -r2.y, c5.x
cmp r2.z, c1.y, r2.y, r5.y
texldl r7, r2.xzww, s2
texldl r8, r2.xyww, s1
mad r2.z, c0.x, r8.x, c0.y
rcp r2.z, r2.z
add r5.y, -r1.z, r2.z
mad_sat r5.y, r5.y, -r1.x, c5.x
add r2.z, r1.z, -r2.z
mad_sat r2.z, r2.z, -r1.x, c5.x
add r5.zw, -r2.xyxy, v0.xyxy
dp2add r5.z, r5.zwzw, r5.zwzw, c5.y
rsq r5.z, r5.z
rcp r5.z, r5.z
mad r5.w, r5.z, -r0.x, c5.x
mul r2.z, r2.z, r5.w
cmp r2.z, r5.w, r2.z, c5.y
dp2add r5.w, r6, r6, c5.y
rsq r5.w, r5.w
rcp r5.w, r5.w
dp2add r6.x, r7, r7, c5.y
rsq r6.x, r6.x
mad r6.y, r5.w, -r6.x, c5.x
mul r5.y, r5.y, r6.y
cmp r5.y, r6.y, r5.y, c5.y
add r2.z, r2.z, r5.y
rcp r5.y, r6.x
mul r6.x, r5.y, c6.y
mad r5.y, r5.y, -c6.w, r5.w
rcp r5.w, r6.x
mul_sat r5.y, r5.w, r5.y
mad r5.w, r5.y, c10.z, c10.w
mul r5.y, r5.y, r5.y
mad r5.y, r5.w, -r5.y, c5.x
mad r5.z, r0.y, -c6.w, r5.z
mul_sat r5.z, r1.y, r5.z
mad r5.w, r5.z, c10.z, c10.w
mul r5.z, r5.z, r5.z
mad r5.z, r5.w, -r5.z, c5.x
dp2add r2.z, r5.y, r5.z, r2.z
texldl r6, r2.xyww, s0
mad r4, r6, r2.z, r4
add r1.w, r1.w, r2.z
add r5.x, r5.x, c5.x
endrep
rcp r0.x, r1.w
mul oC0, r0.x, r4

""
}
SubProgram ""d3d11 "" {
// Stats: 99 math, 1 branches
SetTexture 0 [_NeighbourMaxTex] 2D 3
SetTexture 1 [_MainTex] 2D 0
SetTexture 2 [_VelTex] 2D 2
SetTexture 3 [_NoiseTex] 2D 4
SetTexture 4 [_CameraDepthTexture] 2D 1
ConstBuffer ""$Globals"" 416
Vector 112 [_MainTex_TexelSize]
Float 352 [_Jitter]
Float 364 [_MaxVelocity]
Float 400 [_SoftZDistance]
ConstBuffer ""UnityPerCamera"" 144
Vector 112 [_ZBufferParams]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerCamera"" 1
""ps_4_0
eefiecedlegkcjnakmobfhcllipckbkolbhpdfmaabaaaaaanebbaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcbebbaaaa
eaaaaaaaefaeaaaadfbiaaaadcaaaaaaecafkhlofemgmploaaaaaaaaaaaaaaaa
knbdfhlpbmlbjglnaaaaaaaaaaaaaaaaglchdclpnmanokdoaaaaaaaaaaaaaaaa
kmdjfalodoohbodpaaaaaaaaaaaaaaaaokflhgdpkakjehloaaaaaaaaaaaaaaaa
pbgfpcdopimfpfloaaaaaaaaaaaaaaaabcplaedpiofleedpaaaaaaaaaaaaaaaa
hnojdndomgkdgelpaaaaaaaaaaaaaaaappogabdpebpbiddnaaaaaaaaaaaaaaaa
mihlgfdplccnnddoaaaaaaaaaaaaaaaafcnfkelonllpgolpaaaaaaaaaaaaaaaa
jmkdeklpigadbjlpaaaaaaaaaaaaaaaafjaaaaaeegiocaaaaaaaaaaabkaaaaaa
fjaaaaaeegiocaaaabaaaaaaaiaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaad
aagabaaaabaaaaaafkaaaaadaagabaaaacaaaaaafkaaaaadaagabaaaadaaaaaa
fkaaaaadaagabaaaaeaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaae
aahabaaaabaaaaaaffffaaaafibiaaaeaahabaaaacaaaaaaffffaaaafibiaaae
aahabaaaadaaaaaaffffaaaafibiaaaeaahabaaaaeaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacakaaaaaadbaaaaai
bcaabaaaaaaaaaaabkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaai
ccaabaaaaaaaaaaabkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaaj
ccaabaaaabaaaaaaakaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaa
dgaaaaafbcaabaaaabaaaaaaakbabaaaabaaaaaaeiaaaaalpcaabaaaacaaaaaa
egaabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaadaaaaaaabeaaaaaaaaaaaaa
eiaaaaalpcaabaaaadaaaaaaegbabaaaabaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaaabaaaaaaegaabaaaabaaaaaa
eghobaaaacaaaaaaaagabaaaacaaaaaaabeaaaaaaaaaaaaadiaaaaakgcaabaaa
aaaaaaaaagbbbaaaabaaaaaaaceaaaaaaaaaaaaaaaaadaebaaaadaebaaaaaaaa
eiaaaaalpcaabaaaaeaaaaaajgafbaaaaaaaaaaaeghobaaaadaaaaaaaagabaaa
aeaaaaaaabeaaaaaaaaaaaaadcaaaaappcaabaaaaeaaaaaaegaebaaaaeaaaaaa
aceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaeaaceaaaaaaaaaialpaaaaialp
aaaaialpaaaaialpeiaaaaalpcaabaaaafaaaaaaegbabaaaabaaaaaaeghobaaa
aeaaaaaaaagabaaaabaaaaaaabeaaaaaaaaaaaaadcaaaaalccaabaaaaaaaaaaa
akiacaaaabaaaaaaahaaaaaaakaabaaaafaaaaaabkiacaaaabaaaaaaahaaaaaa
aoaaaaakccaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadp
bkaabaaaaaaaaaaadiaaaaajpcaabaaaafaaaaaaegiecaaaaaaaaaaaahaaaaaa
agipcaaaaaaaaaaabgaaaaaadcaaaaajpcaabaaaacaaaaaaegaobaaaaeaaaaaa
egaebaaaafaaaaaaegaebaaaacaaaaaadiaaaaakpcaabaaaaeaaaaaaogaobaaa
afaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaadpaaaaaadpdeaaaaaipcaabaaa
aeaaaaaaogaobaiaibaaaaaaacaaaaaaegaobaaaaeaaaaaadbaaaaakpcaabaaa
afaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaogaobaaaacaaaaaa
dbaaaaakpcaabaaaacaaaaaaegaobaaaacaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaboaaaaaipcaabaaaacaaaaaaegaobaiaebaaaaaaafaaaaaa
egaobaaaacaaaaaaclaaaaafpcaabaaaacaaaaaaegaobaaaacaaaaaadiaaaaah
pcaabaaaacaaaaaaegaobaaaacaaaaaaegaobaaaaeaaaaaaapaaaaahecaabaaa
aaaaaaaaegaabaaaabaaaaaaegaabaaaabaaaaaaelaaaaafecaabaaaaaaaaaaa
ckaabaaaaaaaaaaadiaaaaahicaabaaaaaaaaaaackaabaaaaaaaaaaaabeaaaaa
mimmmmdnaoaaaaakicaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaiadpdkaabaaaaaaaaaaadgaaaaafpcaabaaaabaaaaaaegaobaaaadaaaaaa
dgaaaaaidcaabaaaaeaaaaaaaceaaaaaaaaaiadpaaaaaaaaaaaaaaaaaaaaaaaa
daaaaaabcbaaaaahecaabaaaaeaaaaaabkaabaaaaeaaaaaaabeaaaaaamaaaaaa
adaaaeadckaabaaaaeaaaaaadiaaaaaipcaabaaaafaaaaaaegaobaaaacaaaaaa
egjejaaabkaabaaaaeaaaaaadiaaaaakpcaabaaaagaaaaaaegaobaaaafaaaaaa
aceaaaaaaaaaiadpaaaaiadpaaaaialpaaaaialpdcaaaaampcaabaaaafaaaaaa
egaobaaaafaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaialpaaaaialpegbebaaa
abaaaaaaaaaaaaalmcaabaaaaeaaaaaafganbaiaebaaaaaaafaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaiadpaaaaiadpdhaaaaajdcaabaaaahaaaaaaagaabaaa
aaaaaaaaogakbaaaaeaaaaaangafbaaaafaaaaaadgaaaaafmcaabaaaahaaaaaa
agaibaaaafaaaaaaeiaaaaalpcaabaaaaiaaaaaacgakbaaaahaaaaaaeghobaaa
acaaaaaaaagabaaaacaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaaajaaaaaa
egaabaaaafaaaaaaeghobaaaaeaaaaaaaagabaaaabaaaaaaabeaaaaaaaaaaaaa
dcaaaaalecaabaaaaeaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaajaaaaaa
bkiacaaaabaaaaaaahaaaaaaaoaaaaakecaabaaaaeaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaiadpaaaaiadpckaabaaaaeaaaaaaaaaaaaaiicaabaaaaeaaaaaa
bkaabaiaebaaaaaaaaaaaaaackaabaaaaeaaaaaaaoaaaaaiicaabaaaaeaaaaaa
dkaabaaaaeaaaaaaakiacaaaaaaaaaaabjaaaaaaaaaaaaaiecaabaaaaeaaaaaa
bkaabaaaaaaaaaaackaabaiaebaaaaaaaeaaaaaaaoaaaaaiecaabaaaaeaaaaaa
ckaabaaaaeaaaaaaakiacaaaaaaaaaaabjaaaaaaaacaaaalmcaabaaaaeaaaaaa
kgaobaiaebaaaaaaaeaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaiadpaaaaiadp
aaaaaaaifcaabaaaahaaaaaaagabbaiaebaaaaaaafaaaaaaagbbbaaaabaaaaaa
apaaaaahbcaabaaaahaaaaaaigaabaaaahaaaaaaigaabaaaahaaaaaaelaaaaaf
bcaabaaaahaaaaaaakaabaaaahaaaaaaaoaaaaahecaabaaaahaaaaaaakaabaaa
ahaaaaaackaabaaaaaaaaaaaaaaaaaaiecaabaaaahaaaaaackaabaiaebaaaaaa
ahaaaaaaabeaaaaaaaaaiadpdeaaaaahecaabaaaahaaaaaackaabaaaahaaaaaa
abeaaaaaaaaaaaaaapaaaaahbcaabaaaagaaaaaaegaabaaaagaaaaaaegaabaaa
agaaaaaaapaaaaahccaabaaaagaaaaaaegaabaaaaiaaaaaaegaabaaaaiaaaaaa
elaaaaafdcaabaaaagaaaaaaegaabaaaagaaaaaaaoaaaaahbcaabaaaaiaaaaaa
akaabaaaagaaaaaabkaabaaaagaaaaaaaaaaaaaibcaabaaaaiaaaaaaakaabaia
ebaaaaaaaiaaaaaaabeaaaaaaaaaiadpdeaaaaahbcaabaaaaiaaaaaaakaabaaa
aiaaaaaaabeaaaaaaaaaaaaadiaaaaahicaabaaaaeaaaaaadkaabaaaaeaaaaaa
akaabaaaaiaaaaaadcaaaaajecaabaaaaeaaaaaackaabaaaaeaaaaaackaabaaa
ahaaaaaadkaabaaaaeaaaaaadiaaaaahicaabaaaaeaaaaaabkaabaaaagaaaaaa
abeaaaaamimmmmdndcaaaaakbcaabaaaagaaaaaabkaabaiaebaaaaaaagaaaaaa
abeaaaaaddddhddpakaabaaaagaaaaaaaoaaaaakicaabaaaaeaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaiadpaaaaiadpdkaabaaaaeaaaaaadicaaaahicaabaaa
aeaaaaaadkaabaaaaeaaaaaaakaabaaaagaaaaaadcaaaaajbcaabaaaagaaaaaa
dkaabaaaaeaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaeadiaaaaahicaabaaa
aeaaaaaadkaabaaaaeaaaaaadkaabaaaaeaaaaaadcaaaaakicaabaaaaeaaaaaa
akaabaiaebaaaaaaagaaaaaadkaabaaaaeaaaaaaabeaaaaaaaaaiadpdcaaaaak
bcaabaaaagaaaaaackaabaiaebaaaaaaaaaaaaaaabeaaaaaddddhddpakaabaaa
ahaaaaaadicaaaahbcaabaaaagaaaaaadkaabaaaaaaaaaaaakaabaaaagaaaaaa
dcaaaaajccaabaaaagaaaaaaakaabaaaagaaaaaaabeaaaaaaaaaaamaabeaaaaa
aaaaeaeadiaaaaahbcaabaaaagaaaaaaakaabaaaagaaaaaaakaabaaaagaaaaaa
dcaaaaakbcaabaaaagaaaaaabkaabaiaebaaaaaaagaaaaaaakaabaaaagaaaaaa
abeaaaaaaaaaiadpapaaaaahicaabaaaaeaaaaaapgapbaaaaeaaaaaaagaabaaa
agaaaaaaaaaaaaahecaabaaaaeaaaaaadkaabaaaaeaaaaaackaabaaaaeaaaaaa
eiaaaaalpcaabaaaaiaaaaaaegaabaaaafaaaaaaeghobaaaabaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaadcaaaaajpcaabaaaaiaaaaaaegaobaaaaiaaaaaa
kgakbaaaaeaaaaaaegaobaaaabaaaaaaaaaaaaahecaabaaaaeaaaaaackaabaaa
aeaaaaaaakaabaaaaeaaaaaaeiaaaaalpcaabaaaahaaaaaahgapbaaaahaaaaaa
eghobaaaacaaaaaaaagabaaaacaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaa
ajaaaaaaogakbaaaafaaaaaaeghobaaaaeaaaaaaaagabaaaabaaaaaaabeaaaaa
aaaaaaaadcaaaaalicaabaaaaeaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaa
ajaaaaaabkiacaaaabaaaaaaahaaaaaaaoaaaaakicaabaaaaeaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaiadpaaaaiadpdkaabaaaaeaaaaaaaaaaaaaibcaabaaa
afaaaaaabkaabaiaebaaaaaaaaaaaaaadkaabaaaaeaaaaaaaoaaaaaibcaabaaa
afaaaaaaakaabaaaafaaaaaaakiacaaaaaaaaaaabjaaaaaaaacaaaaibcaabaaa
afaaaaaaakaabaiaebaaaaaaafaaaaaaabeaaaaaaaaaiadpaaaaaaaiicaabaaa
aeaaaaaabkaabaaaaaaaaaaadkaabaiaebaaaaaaaeaaaaaaaoaaaaaiicaabaaa
aeaaaaaadkaabaaaaeaaaaaaakiacaaaaaaaaaaabjaaaaaaaacaaaaiicaabaaa
aeaaaaaadkaabaiaebaaaaaaaeaaaaaaabeaaaaaaaaaiadpaaaaaaaidcaabaaa
agaaaaaaogakbaiaebaaaaaaafaaaaaaegbabaaaabaaaaaaapaaaaahccaabaaa
afaaaaaaegaabaaaagaaaaaaegaabaaaagaaaaaaelaaaaafccaabaaaafaaaaaa
bkaabaaaafaaaaaaaoaaaaahbcaabaaaagaaaaaabkaabaaaafaaaaaackaabaaa
aaaaaaaaaaaaaaaibcaabaaaagaaaaaaakaabaiaebaaaaaaagaaaaaaabeaaaaa
aaaaiadpapaaaaahccaabaaaagaaaaaaogakbaaaagaaaaaaogakbaaaagaaaaaa
apaaaaahecaabaaaagaaaaaaegaabaaaahaaaaaaegaabaaaahaaaaaaelaaaaaf
gcaabaaaagaaaaaafgagbaaaagaaaaaaaoaaaaahicaabaaaagaaaaaabkaabaaa
agaaaaaackaabaaaagaaaaaaaaaaaaaiicaabaaaagaaaaaadkaabaiaebaaaaaa
agaaaaaaabeaaaaaaaaaiadpdeaaaaakjcaabaaaagaaaaaaagambaaaagaaaaaa
aceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadiaaaaahbcaabaaaafaaaaaa
akaabaaaafaaaaaadkaabaaaagaaaaaadcaaaaajicaabaaaaeaaaaaadkaabaaa
aeaaaaaaakaabaaaagaaaaaaakaabaaaafaaaaaadiaaaaahbcaabaaaafaaaaaa
ckaabaaaagaaaaaaabeaaaaamimmmmdndcaaaaakbcaabaaaagaaaaaackaabaia
ebaaaaaaagaaaaaaabeaaaaaddddhddpbkaabaaaagaaaaaaaoaaaaakbcaabaaa
afaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpakaabaaaafaaaaaa
dicaaaahbcaabaaaafaaaaaaakaabaaaafaaaaaaakaabaaaagaaaaaadcaaaaaj
bcaabaaaagaaaaaaakaabaaaafaaaaaaabeaaaaaaaaaaamaabeaaaaaaaaaeaea
diaaaaahbcaabaaaafaaaaaaakaabaaaafaaaaaaakaabaaaafaaaaaadcaaaaak
bcaabaaaafaaaaaaakaabaiaebaaaaaaagaaaaaaakaabaaaafaaaaaaabeaaaaa
aaaaiadpdcaaaaakccaabaaaafaaaaaackaabaiaebaaaaaaaaaaaaaaabeaaaaa
ddddhddpbkaabaaaafaaaaaadicaaaahccaabaaaafaaaaaadkaabaaaaaaaaaaa
bkaabaaaafaaaaaadcaaaaajbcaabaaaagaaaaaabkaabaaaafaaaaaaabeaaaaa
aaaaaamaabeaaaaaaaaaeaeadiaaaaahccaabaaaafaaaaaabkaabaaaafaaaaaa
bkaabaaaafaaaaaadcaaaaakccaabaaaafaaaaaaakaabaiaebaaaaaaagaaaaaa
bkaabaaaafaaaaaaabeaaaaaaaaaiadpapaaaaahbcaabaaaafaaaaaaagaabaaa
afaaaaaafgafbaaaafaaaaaaaaaaaaahicaabaaaaeaaaaaadkaabaaaaeaaaaaa
akaabaaaafaaaaaaeiaaaaalpcaabaaaafaaaaaaogakbaaaafaaaaaaeghobaaa
abaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadcaaaaajpcaabaaaabaaaaaa
egaobaaaafaaaaaapgapbaaaaeaaaaaaegaobaaaaiaaaaaaaaaaaaahbcaabaaa
aeaaaaaadkaabaaaaeaaaaaackaabaaaaeaaaaaaboaaaaahccaabaaaaeaaaaaa
bkaabaaaaeaaaaaaabeaaaaaabaaaaaabgaaaaabaoaaaaahpccabaaaaaaaaaaa
egaobaaaabaaaaaaagaabaaaaeaaaaaadoaaaaab""
}
}
 }
}
Fallback Off
}";
	}
}
