using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace PostProcessFX
{
	class ScreenSpaceReflectionEffect
	{
		private ScreenSpaceReflection reflectionComponent = null;
		private bool lastState = false;

		public ScreenSpaceReflectionEffect() {
			reflectionComponent = Camera.main.GetComponent<ScreenSpaceReflection>();			
		}

		public void drawGUI(EffectConfig config, float x, float y)
		{
			config.reflectionEnabled = GUI.Toggle(new Rect(x, y, 200, 20), config.reflectionEnabled, "enabled");
			y += 25;

			config.reflectionIterations = DrawGUI.drawIntSliderWithLabel(x, y, 1, 300, "iterations", config.reflectionIterations);
			y += 25;

			config.reflectionBinarySearchIterations = DrawGUI.drawIntSliderWithLabel(x, y, 1, 32, "binarySearchIterations", config.reflectionBinarySearchIterations);
			y += 25;

			config.reflectionMaxDistance = DrawGUI.drawSliderWithLabel(x, y, 1.0f, 1000.0f, "distance", config.reflectionMaxDistance);
			y += 25;

			config.reflectionPixelStride = DrawGUI.drawIntSliderWithLabel(x, y, 1, 32, "pixelStride", config.reflectionPixelStride);
			y += 25;

			config.reflectionEyeFadeStart = DrawGUI.drawSliderWithLabel(x, y, 0.0f, 1.0f, "eyeFadeStart", config.reflectionEyeFadeStart);
			y += 25;

			config.reflectionEyeFadeEnd = DrawGUI.drawSliderWithLabel(x, y, 0.0f, 1.0f, "eyeFadeEnd", config.reflectionEyeFadeEnd);
			y += 25;

			config.reflectionScreenEdgeFadeStart = DrawGUI.drawSliderWithLabel(x, y, 0.0f, 1.0f, "edgeFadeStart", config.reflectionScreenEdgeFadeStart);
			y += 25;
		}

		public void applyConfig(EffectConfig config)
		{
			if (config.reflectionEnabled)
			{
				Enable();

				reflectionComponent.binarySearchIterations = config.reflectionBinarySearchIterations;
				reflectionComponent.eyeFadeEnd = config.reflectionEyeFadeEnd;
				reflectionComponent.eyeFadeStart = config.reflectionEyeFadeStart;
				reflectionComponent.iterations = config.reflectionIterations;
				reflectionComponent.screenEdgeFadeStart = config.reflectionScreenEdgeFadeStart;
				reflectionComponent.pixelStride = config.reflectionPixelStride;
				reflectionComponent.maxRayDistance = config.reflectionMaxDistance;
			}
			else
			{
				Disable();
			}
		}

		public void Enable()
		{
			if (reflectionComponent == null)
			{
				reflectionComponent = Camera.main.gameObject.AddComponent<ScreenSpaceReflection>();
				if (reflectionComponent == null)
				{
					Debug.LogError("ScreenSpaceReflectionEffect: Could not add the ScreenSpaceReflection component to the main camera.");
				}
				else
				{
					Material screenSpaceMaterial = new Material(screenSpaceReflectionShaderText);
					Material backfaceMaterial = new Material(backfaceDepthShaderText);
					Material bilateralMaterial = new Material(bilateralBlurShaderText);
					Material ssrBlurCombiner = new Material(ssrBlurCombinerShaderText);
					
					reflectionComponent.screenSpaceReflectionShader = screenSpaceMaterial.shader;
					reflectionComponent.backfaceDepthShader = backfaceMaterial.shader;
					reflectionComponent.bilaturalBlurShader = bilateralMaterial.shader;
					reflectionComponent.ssrBlurCombiner = ssrBlurCombiner.shader;

					reflectionComponent.Start();

					//Camera.main.renderingPath = RenderingPath.DeferredShading;
				}
			}

			if (!lastState)
			{
				lastState = true;
				reflectionComponent.enabled = true;
			}
		}

		public void Disable()
		{
			if (lastState)
			{
				lastState = false;
				reflectionComponent.enabled = false;
				Camera.main.renderingPath = RenderingPath.UsePlayerSettings;
			}
		}

		public void Cleanup()
		{
			if (reflectionComponent != null) 
			{
				MonoBehaviour.Destroy(reflectionComponent);
				reflectionComponent = null;
			}
		}

		private const String bilateralBlurShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 19.7KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""kode80/BilaturalBlur"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = ""white"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 147 math, 22 texture
 // Stats for Fragment shader:
 //       d3d11 : 101 math, 22 texture
 //        d3d9 : 93 math, 22 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 42663
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 147 math, 22 textures
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
uniform vec4 _ProjectionParams;
uniform vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform sampler2D _CameraGBufferTexture1;
uniform sampler2D _CameraGBufferTexture2;
uniform vec3 _TexelOffsetScale;
uniform float _DepthBias;
uniform float _NormalBias;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec3 tmpvar_1;
  tmpvar_1 = ((texture2D (_CameraGBufferTexture2, xlv_TEXCOORD0).xyz * 2.0) - 1.0);
  float tmpvar_2;
  tmpvar_2 = ((1.0/((
    (_ZBufferParams.x * texture2D (_CameraDepthTexture, xlv_TEXCOORD0).x)
   + _ZBufferParams.y))) * _ProjectionParams.z);
  vec2 tmpvar_3;
  tmpvar_3 = (_TexelOffsetScale.xy * (1.0 - texture2D (_CameraGBufferTexture1, xlv_TEXCOORD0).w));
  vec2 tmpvar_4;
  tmpvar_4 = (tmpvar_3 + xlv_TEXCOORD0);
  vec3 tmpvar_5;
  tmpvar_5 = abs(((
    (texture2D (_CameraGBufferTexture2, tmpvar_4).xyz * 2.0)
   - 1.0) - tmpvar_1));
  float tmpvar_6;
  tmpvar_6 = (0.189879 * (float(
    (_NormalBias >= ((tmpvar_5.x + tmpvar_5.y) + tmpvar_5.z))
  ) * float(
    (_DepthBias >= abs(((
      (1.0/(((_ZBufferParams.x * texture2D (_CameraDepthTexture, tmpvar_4).x) + _ZBufferParams.y)))
     * _ProjectionParams.z) - tmpvar_2)))
  )));
  vec2 tmpvar_7;
  tmpvar_7 = ((tmpvar_3 * 2.0) + xlv_TEXCOORD0);
  vec3 tmpvar_8;
  tmpvar_8 = abs(((
    (texture2D (_CameraGBufferTexture2, tmpvar_7).xyz * 2.0)
   - 1.0) - tmpvar_1));
  float tmpvar_9;
  tmpvar_9 = (0.131514 * (float(
    (_NormalBias >= ((tmpvar_8.x + tmpvar_8.y) + tmpvar_8.z))
  ) * float(
    (_DepthBias >= abs(((
      (1.0/(((_ZBufferParams.x * texture2D (_CameraDepthTexture, tmpvar_7).x) + _ZBufferParams.y)))
     * _ProjectionParams.z) - tmpvar_2)))
  )));
  vec2 tmpvar_10;
  tmpvar_10 = ((tmpvar_3 * 3.0) + xlv_TEXCOORD0);
  vec3 tmpvar_11;
  tmpvar_11 = abs(((
    (texture2D (_CameraGBufferTexture2, tmpvar_10).xyz * 2.0)
   - 1.0) - tmpvar_1));
  float tmpvar_12;
  tmpvar_12 = (0.071303 * (float(
    (_NormalBias >= ((tmpvar_11.x + tmpvar_11.y) + tmpvar_11.z))
  ) * float(
    (_DepthBias >= abs(((
      (1.0/(((_ZBufferParams.x * texture2D (_CameraDepthTexture, tmpvar_10).x) + _ZBufferParams.y)))
     * _ProjectionParams.z) - tmpvar_2)))
  )));
  vec2 tmpvar_13;
  tmpvar_13 = (-(tmpvar_3) + xlv_TEXCOORD0);
  vec3 tmpvar_14;
  tmpvar_14 = abs(((
    (texture2D (_CameraGBufferTexture2, tmpvar_13).xyz * 2.0)
   - 1.0) - tmpvar_1));
  float tmpvar_15;
  tmpvar_15 = (0.189879 * (float(
    (_NormalBias >= ((tmpvar_14.x + tmpvar_14.y) + tmpvar_14.z))
  ) * float(
    (_DepthBias >= abs(((
      (1.0/(((_ZBufferParams.x * texture2D (_CameraDepthTexture, tmpvar_13).x) + _ZBufferParams.y)))
     * _ProjectionParams.z) - tmpvar_2)))
  )));
  vec2 tmpvar_16;
  tmpvar_16 = ((tmpvar_3 * -2.0) + xlv_TEXCOORD0);
  vec3 tmpvar_17;
  tmpvar_17 = abs(((
    (texture2D (_CameraGBufferTexture2, tmpvar_16).xyz * 2.0)
   - 1.0) - tmpvar_1));
  float tmpvar_18;
  tmpvar_18 = (0.131514 * (float(
    (_NormalBias >= ((tmpvar_17.x + tmpvar_17.y) + tmpvar_17.z))
  ) * float(
    (_DepthBias >= abs(((
      (1.0/(((_ZBufferParams.x * texture2D (_CameraDepthTexture, tmpvar_16).x) + _ZBufferParams.y)))
     * _ProjectionParams.z) - tmpvar_2)))
  )));
  vec2 tmpvar_19;
  tmpvar_19 = ((tmpvar_3 * -3.0) + xlv_TEXCOORD0);
  vec3 tmpvar_20;
  tmpvar_20 = abs(((
    (texture2D (_CameraGBufferTexture2, tmpvar_19).xyz * 2.0)
   - 1.0) - tmpvar_1));
  float tmpvar_21;
  tmpvar_21 = (0.071303 * (float(
    (_NormalBias >= ((tmpvar_20.x + tmpvar_20.y) + tmpvar_20.z))
  ) * float(
    (_DepthBias >= abs(((
      (1.0/(((_ZBufferParams.x * texture2D (_CameraDepthTexture, tmpvar_19).x) + _ZBufferParams.y)))
     * _ProjectionParams.z) - tmpvar_2)))
  )));
  gl_FragData[0] = (((
    ((((
      (texture2D (_MainTex, xlv_TEXCOORD0) * 0.214607)
     + 
      (texture2D (_MainTex, tmpvar_4) * tmpvar_6)
    ) + (texture2D (_MainTex, tmpvar_7) * tmpvar_9)) + (texture2D (_MainTex, tmpvar_10) * tmpvar_12)) + (texture2D (_MainTex, tmpvar_13) * tmpvar_15))
   + 
    (texture2D (_MainTex, tmpvar_16) * tmpvar_18)
  ) + (texture2D (_MainTex, tmpvar_19) * tmpvar_21)) / ((
    ((((0.214607 + tmpvar_6) + tmpvar_9) + tmpvar_12) + tmpvar_15)
   + tmpvar_18) + tmpvar_21));
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
// Stats: 93 math, 22 textures
Float 6 [_DepthBias]
Float 7 [_NormalBias]
Vector 3 [_ProjectionParams]
Vector 5 [_TexelOffsetScale]
Vector 4 [_ZBufferParams]
Float 0 [weights]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_CameraDepthTexture] 2D 1
SetTexture 2 [_CameraGBufferTexture1] 2D 2
SetTexture 3 [_CameraGBufferTexture2] 2D 3
""ps_3_0
def c8, 0.214607, 3, -2, -3
def c9, 2, -1, 1, 0
dcl_texcoord v0.xy
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
texld r0, v0, s0
texld_pp r1, v0, s2
add r1.x, -r1.w, c9.z
mad r1.yz, c5.xxyw, r1.x, v0.xxyw
texld r2, r1.yzzw, s1
mad r1.w, c4.x, r2.x, c4.y
rcp r1.w, r1.w
texld r2, v0, s1
mad r2.x, c4.x, r2.x, c4.y
rcp r2.x, r2.x
mul r2.x, r2.x, c3.z
mad r1.w, r1.w, c3.z, -r2.x
add r1.w, -r1_abs.w, c6.x
mov r2.w, c9.w
cmp_pp r1.w, r1.w, c2.x, r2.w
texld r3, r1.yzzw, s3
texld r4, r1.yzzw, s0
mad r3.xyz, r3, c9.x, c9.y
texld r5, v0, s3
mad r5.xyz, r5, c9.x, c9.y
add r3.xyz, r3, -r5
add r1.y, r3_abs.y, r3_abs.x
add r1.y, r3_abs.z, r1.y
add r1.y, -r1.y, c7.x
cmp_pp r1.y, r1.y, r1.w, c9.w
mul r3, r1.y, r4
add_pp r1.y, r1.y, c8.x
mad_pp r0, r0, c8.x, r3
mul r1.zw, r1.x, c5.xyxy
mad r2.yz, c5.xxyw, -r1.x, v0.xxyw
mad r3.xy, r1.zwzw, c9.x, v0
texld r4, r3, s1
mad r1.x, c4.x, r4.x, c4.y
rcp r1.x, r1.x
mad r1.x, r1.x, c3.z, -r2.x
add r1.x, -r1_abs.x, c6.x
cmp_pp r1.x, r1.x, c1.x, r2.w
texld r4, r3, s3
texld r3, r3, s0
mad r4.xyz, r4, c9.x, -r5
add r4.xyz, r4, c9.y
add r4.x, r4_abs.y, r4_abs.x
add r4.x, r4_abs.z, r4.x
add r4.x, -r4.x, c7.x
cmp_pp r1.x, r4.x, r1.x, c9.w
mad_pp r0, r3, r1.x, r0
add_pp r1.x, r1.x, r1.y
mad r3, r1.zwzw, c8.yyzz, v0.xyxy
mad r1.yz, r1.xzww, c8.w, v0.xxyw
texld r4, r3, s1
mad r1.w, c4.x, r4.x, c4.y
rcp r1.w, r1.w
mad r1.w, r1.w, c3.z, -r2.x
add r1.w, -r1_abs.w, c6.x
cmp_pp r1.w, r1.w, c0.x, r2.w
texld r4, r3, s3
mad r4.xyz, r4, c9.x, -r5
add r4.xyz, r4, c9.y
add r4.x, r4_abs.y, r4_abs.x
add r4.x, r4_abs.z, r4.x
add r4.x, -r4.x, c7.x
cmp_pp r1.w, r4.x, r1.w, c9.w
texld r4, r3, s0
mad_pp r0, r4, r1.w, r0
add_pp r1.x, r1.w, r1.x
texld r4, r2.yzzw, s1
mad r1.w, c4.x, r4.x, c4.y
rcp r1.w, r1.w
mad r1.w, r1.w, c3.z, -r2.x
add r1.w, -r1_abs.w, c6.x
cmp_pp r1.w, r1.w, c2.x, r2.w
texld r4, r2.yzzw, s3
texld r6, r2.yzzw, s0
mad r4.xyz, r4, c9.x, -r5
add r4.xyz, r4, c9.y
add r2.y, r4_abs.y, r4_abs.x
add r2.y, r4_abs.z, r2.y
add r2.y, -r2.y, c7.x
cmp_pp r1.w, r2.y, r1.w, c9.w
mad_pp r0, r6, r1.w, r0
add_pp r1.x, r1.w, r1.x
texld r4, r3.zwzw, s1
mad r1.w, c4.x, r4.x, c4.y
rcp r1.w, r1.w
mad r1.w, r1.w, c3.z, -r2.x
add r1.w, -r1_abs.w, c6.x
cmp_pp r1.w, r1.w, c1.x, r2.w
texld r4, r3.zwzw, s3
texld r3, r3.zwzw, s0
mad r4.xyz, r4, c9.x, -r5
add r4.xyz, r4, c9.y
add r2.y, r4_abs.y, r4_abs.x
add r2.y, r4_abs.z, r2.y
add r2.y, -r2.y, c7.x
cmp_pp r1.w, r2.y, r1.w, c9.w
mad_pp r0, r3, r1.w, r0
add_pp r1.x, r1.w, r1.x
texld r3, r1.yzzw, s1
mad r1.w, c4.x, r3.x, c4.y
rcp r1.w, r1.w
mad r1.w, r1.w, c3.z, -r2.x
add r1.w, -r1_abs.w, c6.x
cmp_pp r1.w, r1.w, c0.x, r2.w
texld r2, r1.yzzw, s3
texld r3, r1.yzzw, s0
mad r2.xyz, r2, c9.x, -r5
add r2.xyz, r2, c9.y
add r1.y, r2_abs.y, r2_abs.x
add r1.y, r2_abs.z, r1.y
add r1.y, -r1.y, c7.x
cmp_pp r1.y, r1.y, r1.w, c9.w
mad_pp r0, r3, r1.y, r0
add_pp r1.x, r1.y, r1.x
rcp r1.x, r1.x
mul_pp oC0, r0, r1.x

""
}
SubProgram ""d3d11 "" {
// Stats: 101 math, 22 textures
SetTexture 0 [_CameraGBufferTexture1] 2D 2
SetTexture 1 [_CameraGBufferTexture2] 2D 3
SetTexture 2 [_CameraDepthTexture] 2D 1
SetTexture 3 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 176
Vector 96 [_TexelOffsetScale] 3
Float 108 [_DepthBias]
Float 112 [_NormalBias]
Float 128 [weights]
ConstBuffer ""UnityPerCamera"" 144
Vector 80 [_ProjectionParams]
Vector 112 [_ZBufferParams]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerCamera"" 1
""ps_4_0
eefiecedlhnmldbiaefjmnhjpdcjfjcbmedagbflabaaaaaagebdaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefckebcaaaa
eaaaaaaakjaeaaaafjaaaaaeegiocaaaaaaaaaaaalaaaaaafjaaaaaeegiocaaa
abaaaaaaaiaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaa
fkaaaaadaagabaaaacaaaaaafkaaaaadaagabaaaadaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaafibiaaaeaahabaaa
acaaaaaaffffaaaafibiaaaeaahabaaaadaaaaaaffffaaaagcbaaaaddcbabaaa
abaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacahaaaaaaefaaaaajpcaabaaa
aaaaaaaaegbabaaaabaaaaaaeghobaaaadaaaaaaaagabaaaaaaaaaaaefaaaaaj
pcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaacaaaaaa
aaaaaaaibcaabaaaabaaaaaadkaabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadp
dcaaaaakgcaabaaaabaaaaaaagibcaaaaaaaaaaaagaaaaaaagaabaaaabaaaaaa
agbbbaaaabaaaaaaefaaaaajpcaabaaaacaaaaaajgafbaaaabaaaaaaeghobaaa
acaaaaaaaagabaaaabaaaaaadcaaaaalicaabaaaabaaaaaaakiacaaaabaaaaaa
ahaaaaaaakaabaaaacaaaaaabkiacaaaabaaaaaaahaaaaaaaoaaaaakicaabaaa
abaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpdkaabaaaabaaaaaa
efaaaaajpcaabaaaacaaaaaaegbabaaaabaaaaaaeghobaaaacaaaaaaaagabaaa
abaaaaaadcaaaaalbcaabaaaacaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaa
acaaaaaabkiacaaaabaaaaaaahaaaaaaaoaaaaakbcaabaaaacaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaiadpaaaaiadpakaabaaaacaaaaaadiaaaaaibcaabaaa
acaaaaaaakaabaaaacaaaaaackiacaaaabaaaaaaafaaaaaadcaaaaalicaabaaa
abaaaaaadkaabaaaabaaaaaackiacaaaabaaaaaaafaaaaaaakaabaiaebaaaaaa
acaaaaaabnaaaaajicaabaaaabaaaaaadkiacaaaaaaaaaaaagaaaaaadkaabaia
ibaaaaaaabaaaaaaefaaaaajpcaabaaaadaaaaaajgafbaaaabaaaaaaeghobaaa
abaaaaaaaagabaaaadaaaaaaefaaaaajpcaabaaaaeaaaaaajgafbaaaabaaaaaa
eghobaaaadaaaaaaaagabaaaaaaaaaaadcaaaaapocaabaaaacaaaaaaagajbaaa
adaaaaaaaceaaaaaaaaaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaceaaaaaaaaaaaaa
aaaaialpaaaaialpaaaaialpefaaaaajpcaabaaaadaaaaaaegbabaaaabaaaaaa
eghobaaaabaaaaaaaagabaaaadaaaaaadcaaaaaphcaabaaaadaaaaaaegacbaaa
adaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaaaaceaaaaaaaaaialp
aaaaialpaaaaialpaaaaaaaaaaaaaaaiocaabaaaacaaaaaafgaobaaaacaaaaaa
agajbaiaebaaaaaaadaaaaaaaaaaaaajccaabaaaabaaaaaackaabaiaibaaaaaa
acaaaaaabkaabaiaibaaaaaaacaaaaaaaaaaaaaiccaabaaaabaaaaaadkaabaia
ibaaaaaaacaaaaaabkaabaaaabaaaaaabnaaaaaiccaabaaaabaaaaaaakiacaaa
aaaaaaaaahaaaaaabkaabaaaabaaaaaaabaaaaakkcaabaaaabaaaaaafganbaaa
abaaaaaaaceaaaaaaaaaaaaaaaaaiadpaaaaaaaaaaaaiadpdiaaaaahccaabaaa
abaaaaaadkaabaaaabaaaaaabkaabaaaabaaaaaadiaaaaaiecaabaaaabaaaaaa
bkaabaaaabaaaaaaakiacaaaaaaaaaaaakaaaaaadcaaaaakccaabaaaabaaaaaa
akiacaaaaaaaaaaaakaaaaaabkaabaaaabaaaaaaabeaaaaapambfldodiaaaaah
pcaabaaaaeaaaaaakgakbaaaabaaaaaaegaobaaaaeaaaaaadcaaaaampcaabaaa
aaaaaaaaegaobaaaaaaaaaaaaceaaaaapambfldopambfldopambfldopambfldo
egaobaaaaeaaaaaadiaaaaaimcaabaaaabaaaaaaagaabaaaabaaaaaaagiecaaa
aaaaaaaaagaaaaaadcaaaaalgcaabaaaacaaaaaaagibcaiaebaaaaaaaaaaaaaa
agaaaaaaagaabaaaabaaaaaaagbbbaaaabaaaaaadcaaaaamdcaabaaaaeaaaaaa
ogakbaaaabaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaaaaaaaaaaaegbabaaa
abaaaaaaefaaaaajpcaabaaaafaaaaaaegaabaaaaeaaaaaaeghobaaaacaaaaaa
aagabaaaabaaaaaadcaaaaalbcaabaaaabaaaaaaakiacaaaabaaaaaaahaaaaaa
akaabaaaafaaaaaabkiacaaaabaaaaaaahaaaaaaaoaaaaakbcaabaaaabaaaaaa
aceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpakaabaaaabaaaaaadcaaaaal
bcaabaaaabaaaaaaakaabaaaabaaaaaackiacaaaabaaaaaaafaaaaaaakaabaia
ebaaaaaaacaaaaaabnaaaaajbcaabaaaabaaaaaadkiacaaaaaaaaaaaagaaaaaa
akaabaiaibaaaaaaabaaaaaaabaaaaahbcaabaaaabaaaaaaakaabaaaabaaaaaa
abeaaaaaaaaaiadpefaaaaajpcaabaaaafaaaaaaegaabaaaaeaaaaaaeghobaaa
abaaaaaaaagabaaaadaaaaaaefaaaaajpcaabaaaaeaaaaaaegaabaaaaeaaaaaa
eghobaaaadaaaaaaaagabaaaaaaaaaaadcaaaaanhcaabaaaafaaaaaaegacbaaa
afaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaaaegacbaiaebaaaaaa
adaaaaaaaaaaaaakhcaabaaaafaaaaaaegacbaaaafaaaaaaaceaaaaaaaaaialp
aaaaialpaaaaialpaaaaaaaaaaaaaaajicaabaaaacaaaaaabkaabaiaibaaaaaa
afaaaaaaakaabaiaibaaaaaaafaaaaaaaaaaaaaiicaabaaaacaaaaaackaabaia
ibaaaaaaafaaaaaadkaabaaaacaaaaaabnaaaaaiicaabaaaacaaaaaaakiacaaa
aaaaaaaaahaaaaaadkaabaaaacaaaaaaabaaaaahicaabaaaacaaaaaadkaabaaa
acaaaaaaabeaaaaaaaaaiadpdiaaaaahbcaabaaaabaaaaaaakaabaaaabaaaaaa
dkaabaaaacaaaaaadiaaaaaiicaabaaaacaaaaaaakaabaaaabaaaaaaakiacaaa
aaaaaaaaajaaaaaadcaaaaakbcaabaaaabaaaaaaakiacaaaaaaaaaaaajaaaaaa
akaabaaaabaaaaaabkaabaaaabaaaaaadcaaaaajpcaabaaaaaaaaaaaegaobaaa
aeaaaaaapgapbaaaacaaaaaaegaobaaaaaaaaaaadcaaaaampcaabaaaaeaaaaaa
ogaobaaaabaaaaaaaceaaaaaaaaaeaeaaaaaeaeaaaaaaamaaaaaaamaegbebaaa
abaaaaaadcaaaaamgcaabaaaabaaaaaakgalbaaaabaaaaaaaceaaaaaaaaaaaaa
aaaaeamaaaaaeamaaaaaaaaaagbbbaaaabaaaaaaefaaaaajpcaabaaaafaaaaaa
egaabaaaaeaaaaaaeghobaaaacaaaaaaaagabaaaabaaaaaadcaaaaalicaabaaa
abaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaafaaaaaabkiacaaaabaaaaaa
ahaaaaaaaoaaaaakicaabaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaiadpdkaabaaaabaaaaaadcaaaaalicaabaaaabaaaaaadkaabaaaabaaaaaa
ckiacaaaabaaaaaaafaaaaaaakaabaiaebaaaaaaacaaaaaabnaaaaajicaabaaa
abaaaaaadkiacaaaaaaaaaaaagaaaaaadkaabaiaibaaaaaaabaaaaaaabaaaaah
icaabaaaabaaaaaadkaabaaaabaaaaaaabeaaaaaaaaaiadpefaaaaajpcaabaaa
afaaaaaaegaabaaaaeaaaaaaeghobaaaabaaaaaaaagabaaaadaaaaaadcaaaaan
hcaabaaaafaaaaaaegacbaaaafaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaea
aaaaaaaaegacbaiaebaaaaaaadaaaaaaaaaaaaakhcaabaaaafaaaaaaegacbaaa
afaaaaaaaceaaaaaaaaaialpaaaaialpaaaaialpaaaaaaaaaaaaaaajicaabaaa
acaaaaaabkaabaiaibaaaaaaafaaaaaaakaabaiaibaaaaaaafaaaaaaaaaaaaai
icaabaaaacaaaaaackaabaiaibaaaaaaafaaaaaadkaabaaaacaaaaaabnaaaaai
icaabaaaacaaaaaaakiacaaaaaaaaaaaahaaaaaadkaabaaaacaaaaaaabaaaaah
icaabaaaacaaaaaadkaabaaaacaaaaaaabeaaaaaaaaaiadpdiaaaaahicaabaaa
abaaaaaadkaabaaaabaaaaaadkaabaaaacaaaaaadiaaaaaiicaabaaaacaaaaaa
dkaabaaaabaaaaaaakiacaaaaaaaaaaaaiaaaaaadcaaaaakbcaabaaaabaaaaaa
akiacaaaaaaaaaaaaiaaaaaadkaabaaaabaaaaaaakaabaaaabaaaaaaefaaaaaj
pcaabaaaafaaaaaaegaabaaaaeaaaaaaeghobaaaadaaaaaaaagabaaaaaaaaaaa
dcaaaaajpcaabaaaaaaaaaaaegaobaaaafaaaaaapgapbaaaacaaaaaaegaobaaa
aaaaaaaaefaaaaajpcaabaaaafaaaaaajgafbaaaacaaaaaaeghobaaaacaaaaaa
aagabaaaabaaaaaadcaaaaalicaabaaaabaaaaaaakiacaaaabaaaaaaahaaaaaa
akaabaaaafaaaaaabkiacaaaabaaaaaaahaaaaaaaoaaaaakicaabaaaabaaaaaa
aceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpdkaabaaaabaaaaaadcaaaaal
icaabaaaabaaaaaadkaabaaaabaaaaaackiacaaaabaaaaaaafaaaaaaakaabaia
ebaaaaaaacaaaaaabnaaaaajicaabaaaabaaaaaadkiacaaaaaaaaaaaagaaaaaa
dkaabaiaibaaaaaaabaaaaaaabaaaaahicaabaaaabaaaaaadkaabaaaabaaaaaa
abeaaaaaaaaaiadpefaaaaajpcaabaaaafaaaaaajgafbaaaacaaaaaaeghobaaa
abaaaaaaaagabaaaadaaaaaaefaaaaajpcaabaaaagaaaaaajgafbaaaacaaaaaa
eghobaaaadaaaaaaaagabaaaaaaaaaaadcaaaaanocaabaaaacaaaaaaagajbaaa
afaaaaaaaceaaaaaaaaaaaaaaaaaaaeaaaaaaaeaaaaaaaeaagajbaiaebaaaaaa
adaaaaaaaaaaaaakocaabaaaacaaaaaafgaobaaaacaaaaaaaceaaaaaaaaaaaaa
aaaaialpaaaaialpaaaaialpaaaaaaajccaabaaaacaaaaaackaabaiaibaaaaaa
acaaaaaabkaabaiaibaaaaaaacaaaaaaaaaaaaaiccaabaaaacaaaaaadkaabaia
ibaaaaaaacaaaaaabkaabaaaacaaaaaabnaaaaaiccaabaaaacaaaaaaakiacaaa
aaaaaaaaahaaaaaabkaabaaaacaaaaaaabaaaaahccaabaaaacaaaaaabkaabaaa
acaaaaaaabeaaaaaaaaaiadpdiaaaaahicaabaaaabaaaaaadkaabaaaabaaaaaa
bkaabaaaacaaaaaadiaaaaaiccaabaaaacaaaaaadkaabaaaabaaaaaaakiacaaa
aaaaaaaaakaaaaaadcaaaaakbcaabaaaabaaaaaaakiacaaaaaaaaaaaakaaaaaa
dkaabaaaabaaaaaaakaabaaaabaaaaaadcaaaaajpcaabaaaaaaaaaaaegaobaaa
agaaaaaafgafbaaaacaaaaaaegaobaaaaaaaaaaaefaaaaajpcaabaaaafaaaaaa
ogakbaaaaeaaaaaaeghobaaaacaaaaaaaagabaaaabaaaaaadcaaaaalicaabaaa
abaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaafaaaaaabkiacaaaabaaaaaa
ahaaaaaaaoaaaaakicaabaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaiadpdkaabaaaabaaaaaadcaaaaalicaabaaaabaaaaaadkaabaaaabaaaaaa
ckiacaaaabaaaaaaafaaaaaaakaabaiaebaaaaaaacaaaaaabnaaaaajicaabaaa
abaaaaaadkiacaaaaaaaaaaaagaaaaaadkaabaiaibaaaaaaabaaaaaaabaaaaah
icaabaaaabaaaaaadkaabaaaabaaaaaaabeaaaaaaaaaiadpefaaaaajpcaabaaa
afaaaaaaogakbaaaaeaaaaaaeghobaaaabaaaaaaaagabaaaadaaaaaaefaaaaaj
pcaabaaaaeaaaaaaogakbaaaaeaaaaaaeghobaaaadaaaaaaaagabaaaaaaaaaaa
dcaaaaanocaabaaaacaaaaaaagajbaaaafaaaaaaaceaaaaaaaaaaaaaaaaaaaea
aaaaaaeaaaaaaaeaagajbaiaebaaaaaaadaaaaaaaaaaaaakocaabaaaacaaaaaa
fgaobaaaacaaaaaaaceaaaaaaaaaaaaaaaaaialpaaaaialpaaaaialpaaaaaaaj
ccaabaaaacaaaaaackaabaiaibaaaaaaacaaaaaabkaabaiaibaaaaaaacaaaaaa
aaaaaaaiccaabaaaacaaaaaadkaabaiaibaaaaaaacaaaaaabkaabaaaacaaaaaa
bnaaaaaiccaabaaaacaaaaaaakiacaaaaaaaaaaaahaaaaaabkaabaaaacaaaaaa
abaaaaahccaabaaaacaaaaaabkaabaaaacaaaaaaabeaaaaaaaaaiadpdiaaaaah
icaabaaaabaaaaaadkaabaaaabaaaaaabkaabaaaacaaaaaadiaaaaaiccaabaaa
acaaaaaadkaabaaaabaaaaaaakiacaaaaaaaaaaaajaaaaaadcaaaaakbcaabaaa
abaaaaaaakiacaaaaaaaaaaaajaaaaaadkaabaaaabaaaaaaakaabaaaabaaaaaa
dcaaaaajpcaabaaaaaaaaaaaegaobaaaaeaaaaaafgafbaaaacaaaaaaegaobaaa
aaaaaaaaefaaaaajpcaabaaaaeaaaaaajgafbaaaabaaaaaaeghobaaaacaaaaaa
aagabaaaabaaaaaadcaaaaalicaabaaaabaaaaaaakiacaaaabaaaaaaahaaaaaa
akaabaaaaeaaaaaabkiacaaaabaaaaaaahaaaaaaaoaaaaakicaabaaaabaaaaaa
aceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpdkaabaaaabaaaaaadcaaaaal
icaabaaaabaaaaaadkaabaaaabaaaaaackiacaaaabaaaaaaafaaaaaaakaabaia
ebaaaaaaacaaaaaabnaaaaajicaabaaaabaaaaaadkiacaaaaaaaaaaaagaaaaaa
dkaabaiaibaaaaaaabaaaaaaefaaaaajpcaabaaaacaaaaaajgafbaaaabaaaaaa
eghobaaaabaaaaaaaagabaaaadaaaaaaefaaaaajpcaabaaaaeaaaaaajgafbaaa
abaaaaaaeghobaaaadaaaaaaaagabaaaaaaaaaaadcaaaaanhcaabaaaacaaaaaa
egacbaaaacaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaaaegacbaia
ebaaaaaaadaaaaaaaaaaaaakhcaabaaaacaaaaaaegacbaaaacaaaaaaaceaaaaa
aaaaialpaaaaialpaaaaialpaaaaaaaaaaaaaaajccaabaaaabaaaaaabkaabaia
ibaaaaaaacaaaaaaakaabaiaibaaaaaaacaaaaaaaaaaaaaiccaabaaaabaaaaaa
ckaabaiaibaaaaaaacaaaaaabkaabaaaabaaaaaabnaaaaaiccaabaaaabaaaaaa
akiacaaaaaaaaaaaahaaaaaabkaabaaaabaaaaaaabaaaaakkcaabaaaabaaaaaa
fganbaaaabaaaaaaaceaaaaaaaaaaaaaaaaaiadpaaaaaaaaaaaaiadpdiaaaaah
ccaabaaaabaaaaaadkaabaaaabaaaaaabkaabaaaabaaaaaadiaaaaaiecaabaaa
abaaaaaabkaabaaaabaaaaaaakiacaaaaaaaaaaaaiaaaaaadcaaaaakbcaabaaa
abaaaaaaakiacaaaaaaaaaaaaiaaaaaabkaabaaaabaaaaaaakaabaaaabaaaaaa
dcaaaaajpcaabaaaaaaaaaaaegaobaaaaeaaaaaakgakbaaaabaaaaaaegaobaaa
aaaaaaaaaoaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaaagaabaaaabaaaaaa
doaaaaab""
}
}
 }
}
Fallback ""Diffuse""
}";

		private const String ssrBlurCombinerShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 3.8KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""kode80/SSRBlurCombiner"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = ""white"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 5 math, 2 texture
 // Stats for Fragment shader:
 //       d3d11 : 3 math, 2 texture
 //        d3d9 : 3 math, 2 texture
 Pass {
  Tags { ""RenderType""=""Opaque"" }
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 48345
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 5 math, 2 textures
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
uniform sampler2D _BlurTex;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = texture2D (_BlurTex, xlv_TEXCOORD0);
  vec4 tmpvar_2;
  tmpvar_2.w = 1.0;
  tmpvar_2.xyz = ((tmpvar_1.xyz * tmpvar_1.w) + (texture2D (_MainTex, xlv_TEXCOORD0).xyz * (1.0 - tmpvar_1.w)));
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
// Stats: 3 math, 2 textures
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_BlurTex] 2D 1
""ps_3_0
def c0, 1, 0, 0, 0
dcl_texcoord v0.xy
dcl_2d s0
dcl_2d s1
texld_pp r0, v0, s0
texld_pp r1, v0, s1
lrp_pp oC0.xyz, r1.w, r1, r0
mov_pp oC0.w, c0.x

""
}
SubProgram ""d3d11 "" {
// Stats: 3 math, 2 textures
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_BlurTex] 2D 1
""ps_4_0
eefiecedjcnlbglfmbneeomopoklfpocpcaflcbgabaaaaaaoaabaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefccaabaaaa
eaaaaaaaeiaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaa
fibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaa
gcbaaaaddcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaa
efaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaabaaaaaa
aagabaaaabaaaaaaaaaaaaaiicaabaaaaaaaaaaadkaabaiaebaaaaaaabaaaaaa
abeaaaaaaaaaiadpdiaaaaahhcaabaaaaaaaaaaapgapbaaaaaaaaaaaegacbaaa
aaaaaaaadcaaaaajhccabaaaaaaaaaaaegacbaaaabaaaaaapgapbaaaabaaaaaa
egacbaaaaaaaaaaadgaaaaaficcabaaaaaaaaaaaabeaaaaaaaaaiadpdoaaaaab
""
}
}
 }
}
Fallback ""Diffuse""
}";

		private const String backfaceDepthShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 7.0KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""kode80/BackFaceDepth"" {
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 10 math
 //    d3d11_9x : 10 math
 //        d3d9 : 8 math
 //      opengl : 1 math
 // Stats for Fragment shader:
 //        d3d9 : 3 math
 Pass {
  Tags { ""RenderType""=""Opaque"" }
  Cull Front
  GpuProgramID 45628
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 1 math
""!!GLSL
#ifdef VERTEX
uniform vec4 _ProjectionParams;


varying vec4 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.xyw = vec3(0.0, 0.0, 0.0);
  tmpvar_1.z = -(((gl_ModelViewMatrix * gl_Vertex).z * _ProjectionParams.w));
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
varying vec4 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.yzw = vec3(0.0, 0.0, 0.0);
  tmpvar_1.x = xlv_TEXCOORD0.z;
  gl_FragData[0] = tmpvar_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 8 math
Bind ""vertex"" Vertex
Matrix 4 [glstate_matrix_modelview0] 3
Matrix 0 [glstate_matrix_mvp]
Vector 7 [_ProjectionParams]
""vs_2_0
def c8, 0, 0, 0, 0
dcl_position v0
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
dp4 r0.x, c6, v0
mul r0.x, r0.x, c7.w
mov oT0.z, -r0.x
mov oT0.xyw, c8.x

""
}
SubProgram ""d3d11 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
ConstBuffer ""UnityPerCamera"" 144
Vector 80 [_ProjectionParams]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
Matrix 64 [glstate_matrix_modelview0]
BindCB  ""UnityPerCamera"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedaeabnjbodnlolppdblgcbanhfiacfncfabaaaaaaoeacaaaaadaaaaaa
cmaaaaaakaaaaaaapiaaaaaaejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaafjaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaahaaaaaagaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apaaaaaafaepfdejfeejepeoaaeoepfcenebemaafeeffiedepepfceeaaklklkl
epfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaapaaaaaa
fdfgfpfagphdgjhegjgpgoaafeeffiedepepfceeaaklklklfdeieefcoeabaaaa
eaaaabaahjaaaaaafjaaaaaeegiocaaaaaaaaaaaagaaaaaafjaaaaaeegiocaaa
abaaaaaaaiaaaaaafpaaaaadpcbabaaaaaaaaaaaghaaaaaepccabaaaaaaaaaaa
abaaaaaagfaaaaadpccabaaaabaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaa
aaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaa
pgbpbaaaaaaaaaaaegaobaaaaaaaaaaadiaaaaaibcaabaaaaaaaaaaabkbabaaa
aaaaaaaackiacaaaabaaaaaaafaaaaaadcaaaaakbcaabaaaaaaaaaaackiacaaa
abaaaaaaaeaaaaaaakbabaaaaaaaaaaaakaabaaaaaaaaaaadcaaaaakbcaabaaa
aaaaaaaackiacaaaabaaaaaaagaaaaaackbabaaaaaaaaaaaakaabaaaaaaaaaaa
dcaaaaakbcaabaaaaaaaaaaackiacaaaabaaaaaaahaaaaaadkbabaaaaaaaaaaa
akaabaaaaaaaaaaadiaaaaaibcaabaaaaaaaaaaaakaabaaaaaaaaaaadkiacaaa
aaaaaaaaafaaaaaadgaaaaageccabaaaabaaaaaaakaabaiaebaaaaaaaaaaaaaa
dgaaaaailccabaaaabaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
doaaaaab""
}
SubProgram ""d3d11_9x "" {
// Stats: 10 math
Bind ""vertex"" Vertex
ConstBuffer ""UnityPerCamera"" 144
Vector 80 [_ProjectionParams]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
Matrix 64 [glstate_matrix_modelview0]
BindCB  ""UnityPerCamera"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0_level_9_1
eefiecedlcijeknllokcjmchcdjhhieplkgkdajbabaaaaaadmaeaaaaaeaaaaaa
daaaaaaaieabaaaahaadaaaaoeadaaaaebgpgodjemabaaaaemabaaaaaaacpopp
amabaaaaeaaaaaaaacaaceaaaaaadmaaaaaadmaaaaaaceaaabaadmaaaaaaafaa
abaaabaaaaaaaaaaabaaaaaaaiaaacaaaaaaaaaaaaaaaaaaaaacpoppfbaaaaaf
akaaapkaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabpaaaaacafaaaaiaaaaaapja
afaaaaadaaaaabiaaaaaffjaahaakkkaaeaaaaaeaaaaabiaagaakkkaaaaaaaja
aaaaaaiaaeaaaaaeaaaaabiaaiaakkkaaaaakkjaaaaaaaiaaeaaaaaeaaaaabia
ajaakkkaaaaappjaaaaaaaiaafaaaaadaaaaabiaaaaaaaiaabaappkaabaaaaac
aaaaaeoaaaaaaaibafaaaaadaaaaapiaaaaaffjaadaaoekaaeaaaaaeaaaaapia
acaaoekaaaaaaajaaaaaoeiaaeaaaaaeaaaaapiaaeaaoekaaaaakkjaaaaaoeia
aeaaaaaeaaaaapiaafaaoekaaaaappjaaaaaoeiaaeaaaaaeaaaaadmaaaaappia
aaaaoekaaaaaoeiaabaaaaacaaaaammaaaaaoeiaabaaaaacaaaaaloaakaaaaka
ppppaaaafdeieefcoeabaaaaeaaaabaahjaaaaaafjaaaaaeegiocaaaaaaaaaaa
agaaaaaafjaaaaaeegiocaaaabaaaaaaaiaaaaaafpaaaaadpcbabaaaaaaaaaaa
ghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaadpccabaaaabaaaaaagiaaaaac
abaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaa
abaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaa
acaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaa
egiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadiaaaaai
bcaabaaaaaaaaaaabkbabaaaaaaaaaaackiacaaaabaaaaaaafaaaaaadcaaaaak
bcaabaaaaaaaaaaackiacaaaabaaaaaaaeaaaaaaakbabaaaaaaaaaaaakaabaaa
aaaaaaaadcaaaaakbcaabaaaaaaaaaaackiacaaaabaaaaaaagaaaaaackbabaaa
aaaaaaaaakaabaaaaaaaaaaadcaaaaakbcaabaaaaaaaaaaackiacaaaabaaaaaa
ahaaaaaadkbabaaaaaaaaaaaakaabaaaaaaaaaaadiaaaaaibcaabaaaaaaaaaaa
akaabaaaaaaaaaaadkiacaaaaaaaaaaaafaaaaaadgaaaaageccabaaaabaaaaaa
akaabaiaebaaaaaaaaaaaaaadgaaaaailccabaaaabaaaaaaaceaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaadoaaaaabejfdeheogmaaaaaaadaaaaaaaiaaaaaa
faaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaafjaaaaaaaaaaaaaa
aaaaaaaaadaaaaaaabaaaaaaahaaaaaagaaaaaaaaaaaaaaaaaaaaaaaadaaaaaa
acaaaaaaapaaaaaafaepfdejfeejepeoaaeoepfcenebemaafeeffiedepepfcee
aaklklklepfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaaaaaaaaaaabaaaaaa
adaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaa
apaaaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfceeaaklklkl""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 3 math
""ps_2_0
def c0, 0, 0, 0, 0
dcl t0.xyz
mov r0.x, t0.z
mov r0.yzw, c0.x
mov oC0, r0

""
}
SubProgram ""d3d11 "" {
""ps_4_0
eefiecedmbhnjcohmpabgamfbaaliejpniidjkplabaaaaaabiabaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaeaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcfiaaaaaa
eaaaaaaabgaaaaaagcbaaaadecbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaa
dgaaaaafbccabaaaaaaaaaaackbabaaaabaaaaaadgaaaaaioccabaaaaaaaaaaa
aceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadoaaaaab""
}
SubProgram ""d3d11_9x "" {
""ps_4_0_level_9_1
eefiecedgncoonanedmgaiiibkbmpfkakkldjiagabaaaaaajiabaaaaaeaaaaaa
daaaaaaakmaaaaaaamabaaaageabaaaaebgpgodjheaaaaaaheaaaaaaaaacpppp
faaaaaaaceaaaaaaaaaaceaaaaaaceaaaaaaceaaaaaaceaaaaaaceaaaaacpppp
fbaaaaafaaaaapkaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabpaaaaacaaaaaaia
aaaaaplaabaaaaacaaaaabiaaaaakklaabaaaaacaaaaaoiaaaaaaakaabaaaaac
aaaiapiaaaaaoeiappppaaaafdeieefcfiaaaaaaeaaaaaaabgaaaaaagcbaaaad
ecbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaadgaaaaafbccabaaaaaaaaaaa
ckbabaaaabaaaaaadgaaaaaioccabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaa
aaaaaaaaaaaaaaaadoaaaaabejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaapaeaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl""
}
}
 }
}
}";

		private const String screenSpaceReflectionShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 29.6KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""kode80/SSR"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = ""white"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 10 math
 //        d3d9 : 12 math
 //      opengl : 174 math, 8 texture, 15 branch
 // Stats for Fragment shader:
 //       d3d11 : 142 math, 4 texture, 3 branch
 //        d3d9 : 180 math, 12 texture, 11 branch
 Pass {
  Tags { ""RenderType""=""Opaque"" }
  ZTest False
  ZWrite Off
  Cull Off
  GpuProgramID 25699
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 174 math, 8 textures, 15 branches
""!!GLSL
#ifdef VERTEX

uniform mat4 _CameraInverseProjectionMatrix;
varying vec2 xlv_TEXCOORD0;
varying vec3 xlv_TEXCOORD2;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.zw = vec2(1.0, 1.0);
  tmpvar_1.xy = ((gl_MultiTexCoord0.xy * 2.0) - 1.0);
  vec4 tmpvar_2;
  tmpvar_2 = (_CameraInverseProjectionMatrix * tmpvar_1);
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD2 = (tmpvar_2 / tmpvar_2.w).xyz;
}


#endif
#ifdef FRAGMENT
#extension GL_ARB_shader_texture_lod : enable
uniform vec4 _ProjectionParams;
uniform vec4 _ZBufferParams;
uniform sampler2D _MainTex;
uniform sampler2D _CameraDepthTexture;
uniform sampler2D _BackFaceDepthTex;
uniform sampler2D _CameraGBufferTexture1;
uniform sampler2D _CameraDepthNormalsTexture;
uniform mat4 _CameraProjectionMatrix;
uniform float _Iterations;
uniform float _BinarySearchIterations;
uniform float _PixelZSize;
uniform float _PixelStride;
uniform float _PixelStrideZCuttoff;
uniform float _MaxRayDistance;
uniform float _ScreenEdgeFadeStart;
uniform float _EyeFadeStart;
uniform float _EyeFadeEnd;
uniform vec2 _RenderBufferSize;
uniform vec2 _OneDividedByRenderBufferSize;
varying vec2 xlv_TEXCOORD0;
varying vec3 xlv_TEXCOORD2;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = texture2D (_CameraGBufferTexture1, xlv_TEXCOORD0);
  float tmpvar_2;
  tmpvar_2 = max (max (tmpvar_1.x, tmpvar_1.y), tmpvar_1.z);
  vec3 tmpvar_3;
  tmpvar_3 = (xlv_TEXCOORD2 * (1.0/((
    (_ZBufferParams.x * texture2D (_CameraDepthTexture, xlv_TEXCOORD0).x)
   + _ZBufferParams.y))));
  vec3 n_4;
  vec3 tmpvar_5;
  tmpvar_5 = ((texture2D (_CameraDepthNormalsTexture, xlv_TEXCOORD0).xyz * vec3(3.5554, 3.5554, 0.0)) + vec3(-1.7777, -1.7777, 1.0));
  float tmpvar_6;
  tmpvar_6 = (2.0 / dot (tmpvar_5, tmpvar_5));
  n_4.xy = (tmpvar_6 * tmpvar_5.xy);
  n_4.z = (tmpvar_6 - 1.0);
  vec3 tmpvar_7;
  tmpvar_7 = normalize(tmpvar_3);
  vec3 tmpvar_8;
  tmpvar_8 = normalize(n_4);
  vec3 tmpvar_9;
  tmpvar_9 = normalize((tmpvar_7 - (2.0 * 
    (dot (tmpvar_8, tmpvar_7) * tmpvar_8)
  )));
  vec2 tmpvar_10;
  tmpvar_10 = (xlv_TEXCOORD0 * _RenderBufferSize);
  float tmpvar_11;
  tmpvar_11 = ((tmpvar_10.x + tmpvar_10.y) * 0.25);
  float tmpvar_12;
  tmpvar_12 = fract(abs(tmpvar_11));
  float tmpvar_13;
  if ((tmpvar_11 >= 0.0)) {
    tmpvar_13 = tmpvar_12;
  } else {
    tmpvar_13 = -(tmpvar_12);
  };
  vec2 hitPixel_14;
  bool intersect_15;
  vec4 dPQK_16;
  vec4 pqk_17;
  float zB_18;
  float zA_19;
  float i_20;
  bool permute_21;
  vec2 delta_22;
  vec2 P0_23;
  vec3 Q0_24;
  float tmpvar_25;
  if (((tmpvar_3.z + (tmpvar_9.z * _MaxRayDistance)) > -(_ProjectionParams.y))) {
    tmpvar_25 = ((-(_ProjectionParams.y) - tmpvar_3.z) / tmpvar_9.z);
  } else {
    tmpvar_25 = _MaxRayDistance;
  };
  vec3 tmpvar_26;
  tmpvar_26 = (tmpvar_3 + (tmpvar_9 * tmpvar_25));
  vec4 tmpvar_27;
  tmpvar_27.w = 1.0;
  tmpvar_27.xyz = tmpvar_3;
  vec4 tmpvar_28;
  tmpvar_28 = (_CameraProjectionMatrix * tmpvar_27);
  vec4 tmpvar_29;
  tmpvar_29.w = 1.0;
  tmpvar_29.xyz = tmpvar_26;
  vec4 tmpvar_30;
  tmpvar_30 = (_CameraProjectionMatrix * tmpvar_29);
  float tmpvar_31;
  tmpvar_31 = (1.0/(tmpvar_28.w));
  float tmpvar_32;
  tmpvar_32 = (1.0/(tmpvar_30.w));
  vec3 tmpvar_33;
  tmpvar_33 = (tmpvar_3 * tmpvar_31);
  Q0_24 = tmpvar_33;
  vec3 tmpvar_34;
  tmpvar_34 = (tmpvar_26 * tmpvar_32);
  vec2 tmpvar_35;
  tmpvar_35 = (tmpvar_28.xy * tmpvar_31);
  P0_23 = tmpvar_35;
  vec2 tmpvar_36;
  tmpvar_36 = (tmpvar_30.xy * tmpvar_32);
  vec2 tmpvar_37;
  tmpvar_37 = (tmpvar_35 - tmpvar_36);
  float tmpvar_38;
  tmpvar_38 = dot (tmpvar_37, tmpvar_37);
  float tmpvar_39;
  if ((tmpvar_38 < 0.0001)) {
    tmpvar_39 = 0.01;
  } else {
    tmpvar_39 = 0.0;
  };
  vec2 tmpvar_40;
  tmpvar_40 = ((tmpvar_36 + tmpvar_39) - tmpvar_35);
  delta_22 = tmpvar_40;
  permute_21 = bool(0);
  float tmpvar_41;
  tmpvar_41 = abs(tmpvar_40.x);
  float tmpvar_42;
  tmpvar_42 = abs(tmpvar_40.y);
  if ((tmpvar_41 < tmpvar_42)) {
    permute_21 = bool(1);
    delta_22 = tmpvar_40.yx;
    P0_23 = tmpvar_35.yx;
  };
  float tmpvar_43;
  tmpvar_43 = sign(delta_22.x);
  float tmpvar_44;
  tmpvar_44 = (tmpvar_43 / delta_22.x);
  vec2 tmpvar_45;
  tmpvar_45.x = tmpvar_43;
  tmpvar_45.y = (delta_22.y * tmpvar_44);
  float tmpvar_46;
  tmpvar_46 = (1.0 + ((1.0 - 
    min (1.0, (-(tmpvar_3.z) / _PixelStrideZCuttoff))
  ) * _PixelStride));
  vec2 tmpvar_47;
  tmpvar_47 = (tmpvar_45 * tmpvar_46);
  vec3 tmpvar_48;
  tmpvar_48 = (((tmpvar_34 - tmpvar_33) * tmpvar_44) * tmpvar_46);
  float tmpvar_49;
  tmpvar_49 = (((tmpvar_32 - tmpvar_31) * tmpvar_44) * tmpvar_46);
  vec2 tmpvar_50;
  tmpvar_50 = (P0_23 + (tmpvar_47 * tmpvar_13));
  P0_23 = tmpvar_50;
  vec3 tmpvar_51;
  tmpvar_51 = (tmpvar_33 + (tmpvar_48 * tmpvar_13));
  Q0_24 = tmpvar_51;
  zA_19 = 0.0;
  zB_18 = 0.0;
  vec4 tmpvar_52;
  tmpvar_52.xy = tmpvar_50;
  tmpvar_52.z = tmpvar_51.z;
  tmpvar_52.w = (tmpvar_31 + (tmpvar_49 * tmpvar_13));
  pqk_17 = tmpvar_52;
  vec4 tmpvar_53;
  tmpvar_53.xy = tmpvar_47;
  tmpvar_53.z = tmpvar_48.z;
  tmpvar_53.w = tmpvar_49;
  dPQK_16 = tmpvar_53;
  intersect_15 = bool(0);
  i_20 = 0.0;
  for (; ((i_20 < _Iterations) && (intersect_15 == bool(0))); i_20 += 1.0) {
    vec4 tmpvar_54;
    tmpvar_54 = (pqk_17 + dPQK_16);
    pqk_17 = tmpvar_54;
    zA_19 = zB_18;
    float tmpvar_55;
    tmpvar_55 = (((dPQK_16.z * 0.5) + tmpvar_54.z) / ((dPQK_16.w * 0.5) + tmpvar_54.w));
    zB_18 = tmpvar_55;
    float aa_56;
    aa_56 = tmpvar_55;
    float bb_57;
    bb_57 = zA_19;
    if ((tmpvar_55 > zA_19)) {
      aa_56 = zA_19;
      bb_57 = tmpvar_55;
    };
    zB_18 = aa_56;
    zA_19 = bb_57;
    vec2 tmpvar_58;
    if (permute_21) {
      tmpvar_58 = tmpvar_54.yx;
    } else {
      tmpvar_58 = tmpvar_54.xy;
    };
    vec2 tmpvar_59;
    tmpvar_59 = (tmpvar_58 * _OneDividedByRenderBufferSize);
    hitPixel_14 = tmpvar_59;
    float cse_60;
    cse_60 = -(_ProjectionParams.z);
    intersect_15 = ((aa_56 <= (
      (1.0/(((_ZBufferParams.x * texture2DLod (_CameraDepthTexture, tmpvar_59, 0.0).x) + _ZBufferParams.y)))
     * cse_60)) && (bb_57 >= (
      (texture2DLod (_BackFaceDepthTex, tmpvar_59, 0.0).x * cse_60)
     - _PixelZSize)));
  };
  if (((tmpvar_46 > 1.0) && intersect_15)) {
    float stride_62;
    float originalStride_63;
    vec4 tmpvar_64;
    tmpvar_64 = (pqk_17 - tmpvar_53);
    pqk_17 = tmpvar_64;
    dPQK_16 = (tmpvar_53 / tmpvar_46);
    float tmpvar_65;
    tmpvar_65 = (tmpvar_46 * 0.5);
    originalStride_63 = tmpvar_65;
    stride_62 = tmpvar_65;
    float tmpvar_66;
    tmpvar_66 = (tmpvar_64.z / tmpvar_64.w);
    zA_19 = tmpvar_66;
    zB_18 = tmpvar_66;
    for (float j_61; j_61 < _BinarySearchIterations; j_61 += 1.0) {
      vec4 tmpvar_67;
      tmpvar_67 = (pqk_17 + (dPQK_16 * stride_62));
      pqk_17 = tmpvar_67;
      zA_19 = zB_18;
      float tmpvar_68;
      tmpvar_68 = (((dPQK_16.z * -0.5) + tmpvar_67.z) / ((dPQK_16.w * -0.5) + tmpvar_67.w));
      zB_18 = tmpvar_68;
      float aa_69;
      aa_69 = tmpvar_68;
      float bb_70;
      bb_70 = zA_19;
      if ((tmpvar_68 > zA_19)) {
        aa_69 = zA_19;
        bb_70 = tmpvar_68;
      };
      zB_18 = aa_69;
      zA_19 = bb_70;
      vec2 tmpvar_71;
      if (permute_21) {
        tmpvar_71 = tmpvar_67.yx;
      } else {
        tmpvar_71 = tmpvar_67.xy;
      };
      vec2 tmpvar_72;
      tmpvar_72 = (tmpvar_71 * _OneDividedByRenderBufferSize);
      hitPixel_14 = tmpvar_72;
      float tmpvar_73;
      tmpvar_73 = (originalStride_63 * 0.5);
      originalStride_63 = tmpvar_73;
      float tmpvar_74;
      if (((aa_69 <= (
        (1.0/(((_ZBufferParams.x * texture2DLod (_CameraDepthTexture, tmpvar_72, 0.0).x) + _ZBufferParams.y)))
       * 
        -(_ProjectionParams.z)
      )) && (bb_70 >= (
        (texture2DLod (_BackFaceDepthTex, tmpvar_72, 0.0).x * -(_ProjectionParams.z))
       - _PixelZSize)))) {
        tmpvar_74 = -(tmpvar_73);
      } else {
        tmpvar_74 = tmpvar_73;
      };
      stride_62 = tmpvar_74;
    };
  };
  Q0_24.xy = (tmpvar_51.xy + (tmpvar_48.xy * i_20));
  Q0_24.z = pqk_17.z;
  vec3 tmpvar_75;
  tmpvar_75 = (Q0_24 / pqk_17.w);
  vec2 tmpvar_76;
  tmpvar_76 = ((hitPixel_14 * 2.0) - 1.0);
  float tmpvar_77;
  tmpvar_77 = ((min (1.0, tmpvar_2) * (1.0 - 
    (i_20 / _Iterations)
  )) * (1.0 - (
    max (0.0, (min (1.0, max (
      abs(tmpvar_76.x)
    , 
      abs(tmpvar_76.y)
    )) - _ScreenEdgeFadeStart))
   / 
    (1.0 - _ScreenEdgeFadeStart)
  )));
  float aa_78;
  aa_78 = _EyeFadeStart;
  float bb_79;
  bb_79 = _EyeFadeEnd;
  if ((_EyeFadeStart > _EyeFadeEnd)) {
    aa_78 = _EyeFadeEnd;
    bb_79 = _EyeFadeStart;
  };
  vec3 tmpvar_80;
  tmpvar_80 = (tmpvar_3 - tmpvar_75);
  vec4 tmpvar_81;
  tmpvar_81.xyz = texture2D (_MainTex, mix (xlv_TEXCOORD0, hitPixel_14, vec2(float(intersect_15)))).xyz;
  tmpvar_81.w = (((tmpvar_77 * 
    (1.0 - ((clamp (tmpvar_9.z, aa_78, bb_79) - aa_78) / (bb_79 - aa_78)))
  ) * (1.0 - 
    clamp ((sqrt(dot (tmpvar_80, tmpvar_80)) / _MaxRayDistance), 0.0, 1.0)
  )) * float(intersect_15));
  gl_FragData[0] = tmpvar_81;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 12 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 4 [_CameraInverseProjectionMatrix]
Matrix 0 [glstate_matrix_mvp]
""vs_3_0
def c8, 2, 0, -1, 1
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord2 o2.xyz
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mad r0, v1.xyxx, c8.xxyy, c8.zzww
dp4 r1.x, c7, r0
rcp r1.x, r1.x
dp4 r2.x, c4, r0
dp4 r2.y, c5, r0
dp4 r2.z, c6, r0
mul o2.xyz, r1.x, r2
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 352
Matrix 160 [_CameraInverseProjectionMatrix]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedffgccjeikniijepdfdhncelkcmdbdhimabaaaaaapiacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaaahaiaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcaaacaaaaeaaaabaaiaaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaoaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadhccabaaaacaaaaaa
giaaaaacacaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaadcaaaaapdcaabaaaaaaaaaaa
egbabaaaabaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaaaaaaaaaaaaaceaaaaa
aaaaialpaaaaialpaaaaaaaaaaaaaaaadiaaaaaipcaabaaaabaaaaaafgafbaaa
aaaaaaaaegiocaaaaaaaaaaaalaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
aaaaaaaaakaaaaaaagaabaaaaaaaaaaaegaobaaaabaaaaaaaaaaaaaipcaabaaa
aaaaaaaaegaobaaaaaaaaaaaegiocaaaaaaaaaaaamaaaaaaaaaaaaaipcaabaaa
aaaaaaaaegaobaaaaaaaaaaaegiocaaaaaaaaaaaanaaaaaaaoaaaaahhccabaaa
acaaaaaaegacbaaaaaaaaaaapgapbaaaaaaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 180 math, 12 textures, 11 branches
Matrix 0 [_CameraProjectionMatrix]
Float 7 [_BinarySearchIterations]
Float 14 [_EyeFadeEnd]
Float 13 [_EyeFadeStart]
Float 6 [_Iterations]
Float 11 [_MaxRayDistance]
Vector 16 [_OneDividedByRenderBufferSize]
Float 9 [_PixelStride]
Float 10 [_PixelStrideZCuttoff]
Float 8 [_PixelZSize]
Vector 4 [_ProjectionParams]
Vector 15 [_RenderBufferSize]
Float 12 [_ScreenEdgeFadeStart]
Vector 5 [_ZBufferParams]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_CameraDepthTexture] 2D 1
SetTexture 2 [_BackFaceDepthTex] 2D 2
SetTexture 3 [_CameraGBufferTexture1] 2D 3
SetTexture 4 [_CameraDepthNormalsTexture] 2D 4
""ps_3_0
def c17, 3.55539989, 0, -1.77769995, 1
def c18, 2, -1, 0.25, -9.99999975e-005
def c19, 0, 0.00999999978, 0, 0.5
defi i0, 255, 0, 0, 0
dcl_texcoord v0.xy
dcl_texcoord2 v1.xyz
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
dcl_2d s4
texld_pp r0, v0, s3
max_pp r1.x, r0.x, r0.y
max r2.x, r1.x, r0.z
texld r0, v0, s1
mad r0.x, c5.x, r0.x, c5.y
rcp r0.x, r0.x
mul r1.xyz, r0.x, v1
texld r3, v0, s4
mad r0.yzw, r3.xxyz, c17.xxxy, c17.xzzw
dp3 r0.w, r0.yzww, r0.yzww
rcp r0.w, r0.w
add r2.y, r0.w, r0.w
mul r3.xy, r0.yzzw, r2.y
mad r3.z, r0.w, c18.x, c18.y
nrm r4.xyz, r1
nrm r5.xyz, r3
dp3 r0.y, r4, r5
add r0.y, r0.y, r0.y
mad r0.yzw, r5.xxyz, -r0.y, r4.xxyz
nrm r3.xyz, r0.yzww
mul r0.yz, c15.xxyw, v0.xxyw
add r0.y, r0.z, r0.y
mul r0.z, r0.y, c18.z
frc r0.z, r0_abs.z
cmp r0.y, r0.y, r0.z, -r0.z
mad r0.z, r3.z, c11.x, r1.z
add r0.z, -r0.z, -c4.y
mad r0.x, v1.z, -r0.x, -c4.y
rcp r0.w, r3.z
mul r0.x, r0.w, r0.x
cmp r0.x, r0.z, c11.x, r0.x
mad r4.xyz, r3, r0.x, r1
mov r1.w, c17.w
dp4 r3.x, c0, r1
dp4 r3.y, c1, r1
dp4 r0.x, c3, r1
mov r4.w, c17.w
dp4 r0.z, c0, r4
dp4 r0.w, c1, r4
dp4 r1.w, c3, r4
rcp r0.x, r0.x
rcp r1.w, r1.w
mul r2.yzw, r0.x, r1.xxyz
mul r5.xy, r0.x, r3
mul r6.xy, r0.zwzw, r1.w
mad r6.xy, r3, r0.x, -r6
dp2add r3.w, r6, r6, c18.w
cmp r3.w, r3.w, c19.x, c19.y
mad r0.zw, r0, r1.w, r3.w
mad r5.zw, r3.xyxy, -r0.x, r0
add r0.z, -r5_abs.w, r5_abs.z
cmp r5, r0.z, r5, r5.yxwz
cmp r0.w, -r5.z, c17.y, c17.w
cmp r3.x, r5.z, -c17.y, -c17.w
add r3.x, r0.w, r3.x
rcp r0.w, r5.z
mul r0.w, r0.w, r3.x
mad r4.xyz, r4, r1.w, -r2.yzww
mul r4.xyz, r0.w, r4
add r1.w, -r0.x, r1.w
mul r1.w, r0.w, r1.w
mul r3.y, r0.w, r5.w
rcp r0.w, c10.x
mad r0.w, -r1.z, -r0.w, c17.w
mov r3.w, c17.w
mad r4.w, r0.w, c9.x, r3.w
cmp r0.w, r0.w, r4.w, c17.w
mul r6.xy, r0.w, r3
mul r4.xyz, r0.w, r4
mul r6.w, r0.w, r1.w
mad r3.xy, r6, r0.y, r5
mad r2.yzw, r4.xxyz, r0.y, r2
mad r0.x, r6.w, r0.y, r0.x
mov r6.z, r4.z
mov r5.zw, c17.y
mov r5.xy, c17.y
mov r7.xy, r3
mov r0.y, c17.y
mov r8.y, c17.y
mov r7.z, r2.w
mov r7.w, r0.x
mov r1.w, c17.y
rep i0
add r4.z, r0.y, -c6.x
cmp r4.w, -r1.w, -c17.w, -c17.y
cmp r4.z, r4.z, c17.y, r4.w
cmp r4.z, r4.z, c17.w, c17.y
break_ne r4.z, -r4.z
add r7, r6, r7
mad r4.z, r6.z, c19.w, r7.z
mad r4.w, r6.w, c19.w, r7.w
rcp r4.w, r4.w
mul r8.x, r4.w, r4.z
mad r4.z, r4.z, -r4.w, r8.y
cmp r8.xy, r4.z, r8.yxzw, r8
cmp r4.zw, r0.z, r7.xyxy, r7.xyyx
mul r5.xy, r4.zwzw, c16
texldl r9, r5.xyww, s1
mad r4.z, c5.x, r9.x, c5.y
rcp r4.z, r4.z
texldl r9, r5, s2
mad r4.z, r4.z, -c4.z, -r8.y
mov r8.z, c4.z
mad r4.w, r9.x, -r8.z, -c8.x
add r4.w, -r4.w, r8.x
cmp r4.w, r4.w, c17.w, c17.y
cmp r1.w, r4.z, r4.w, c17.y
add r0.y, r0.y, c17.w
endrep
add r0.x, -r0.w, c17.w
cmp r0.x, r0.x, c17.y, r1.w
if_ne r0.x, -r0.x
add r8, -r6, r7
rcp r0.x, r0.w
mul r6, r0.x, r6
mul r0.x, r0.w, c19.w
rcp r0.w, r8.w
mul r0.w, r0.w, r8.z
mov r9.zw, c17.y
mov r9.xy, r5
mov r3.y, r0.w
mov r7, r8
mov r2.w, r0.x
mov r4.z, r0.x
mov r4.w, c17.y
rep i0
break_ge r4.w, c7.x
mad r7, r6, r4.z, r7
mad r5.zw, r6, -c19.w, r7
rcp r5.w, r5.w
mul r3.x, r5.w, r5.z
mad r5.z, r5.z, -r5.w, r3.y
cmp r3.xy, r5.z, r3.yxzw, r3
cmp r5.zw, r0.z, r7.xyxy, r7.xyyx
mul r9.xy, r5.zwzw, c16
mul r2.w, r2.w, c19.w
texldl r10, r9.xyww, s1
mad r5.z, c5.x, r10.x, c5.y
rcp r5.z, r5.z
texldl r10, r9, s2
mad r5.z, r5.z, -c4.z, -r3.y
mov r10.z, c4.z
mad r5.w, r10.x, -r10.z, -c8.x
add r3.x, r3.x, -r5.w
cmp r3.x, r3.x, -c17.w, -c17.y
cmp r3.x, r5.z, r3.x, c17.y
cmp r4.z, r3.x, r2.w, -r2.w
add r4.w, r4.w, c17.w
endrep
mov r5.xy, r9
endif
mad r7.xy, r4, r0.y, r2.yzzw
rcp r0.x, r7.w
min r0.z, r2.x, c17.w
rcp r0.w, c6.x
mad r0.y, r0.y, -r0.w, c17.w
mul r0.y, r0.y, r0.z
mad r0.zw, r5.xyxy, c18.x, c18.y
max r2.x, r0_abs.z, r0_abs.w
min r0.z, r2.x, c17.w
add r0.z, r0.z, -c12.x
max r2.x, r0.z, c17.y
add r0.z, r3.w, -c12.x
rcp r0.z, r0.z
mad r0.z, r2.x, -r0.z, c17.w
mul r0.y, r0.z, r0.y
mov r2.xy, c14.x
add r0.z, r2.x, -c13.x
mov r2.x, c13.x
cmp r0.zw, r0.z, r2.xyxy, r2.xyyx
max r2.x, r3.z, r0.z
min r3.x, r0.w, r2.x
add r2.x, -r0.z, r3.x
add r0.z, -r0.z, r0.w
rcp r0.z, r0.z
mad r0.z, r2.x, -r0.z, c17.w
mul r0.y, r0.z, r0.y
mad r0.xzw, r7.xyyz, -r0.x, r1.xyyz
dp3 r0.x, r0.xzww, r0.xzww
rsq r0.x, r0.x
rcp r0.x, r0.x
rcp r0.z, c11.x
mul_sat r0.x, r0.z, r0.x
add r0.x, -r0.x, c17.w
mul r0.x, r0.x, r0.y
mul_pp oC0.w, r1.w, r0.x
cmp r0.xy, -r1.w, v0, r5
texld_pp r0, r0, s0
mov_pp oC0.xyz, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 142 math, 4 textures, 3 branches
SetTexture 0 [_CameraGBufferTexture1] 2D 3
SetTexture 1 [_CameraDepthTexture] 2D 1
SetTexture 2 [_CameraDepthNormalsTexture] 2D 4
SetTexture 3 [_BackFaceDepthTex] 2D 2
SetTexture 4 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 352
Matrix 96 [_CameraProjectionMatrix]
Float 224 [_Iterations]
Float 228 [_BinarySearchIterations]
Float 232 [_PixelZSize]
Float 236 [_PixelStride]
Float 240 [_PixelStrideZCuttoff]
Float 244 [_MaxRayDistance]
Float 248 [_ScreenEdgeFadeStart]
Float 252 [_EyeFadeStart]
Float 256 [_EyeFadeEnd]
Vector 336 [_RenderBufferSize] 2
Vector 344 [_OneDividedByRenderBufferSize] 2
ConstBuffer ""UnityPerCamera"" 144
Vector 80 [_ProjectionParams]
Vector 112 [_ZBufferParams]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerCamera"" 1
""ps_4_0
eefiecedeccfakohdlneaabfgpahdiimdeojmlpaabaaaaaaoabhaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaacaaaaaaaaaaaaaaadaaaaaaacaaaaaa
ahahaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcaibhaaaaeaaaaaaamcafaaaa
fjaaaaaeegiocaaaaaaaaaaabgaaaaaafjaaaaaeegiocaaaabaaaaaaaiaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafkaaaaadaagabaaa
acaaaaaafkaaaaadaagabaaaadaaaaaafkaaaaadaagabaaaaeaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaafibiaaae
aahabaaaacaaaaaaffffaaaafibiaaaeaahabaaaadaaaaaaffffaaaafibiaaae
aahabaaaaeaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaadhcbabaaa
acaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacakaaaaaaefaaaaajpcaabaaa
aaaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaadaaaaaadeaaaaah
bcaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaaaaaaaaaadeaaaaahbcaabaaa
aaaaaaaackaabaaaaaaaaaaaakaabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaa
egbabaaaabaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaadcaaaaalccaabaaa
aaaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaabaaaaaabkiacaaaabaaaaaa
ahaaaaaaaoaaaaakccaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadp
aaaaiadpbkaabaaaaaaaaaaadiaaaaahhcaabaaaabaaaaaafgafbaaaaaaaaaaa
egbcbaaaacaaaaaaefaaaaajpcaabaaaacaaaaaaegbabaaaabaaaaaaeghobaaa
acaaaaaaaagabaaaaeaaaaaadcaaaaaphcaabaaaacaaaaaaegacbaaaacaaaaaa
aceaaaaakmilgdeakmilgdeaaaaaaaaaaaaaaaaaaceaaaaakmilodlpkmilodlp
aaaaiadpaaaaaaaabaaaaaahecaabaaaaaaaaaaaegacbaaaacaaaaaaegacbaaa
acaaaaaaaoaaaaahecaabaaaaaaaaaaaabeaaaaaaaaaaaeackaabaaaaaaaaaaa
diaaaaahdcaabaaaacaaaaaaegaabaaaacaaaaaakgakbaaaaaaaaaaaaaaaaaah
ecaabaaaacaaaaaackaabaaaaaaaaaaaabeaaaaaaaaaialpbaaaaaahecaabaaa
aaaaaaaaegacbaaaabaaaaaaegacbaaaabaaaaaaeeaaaaafecaabaaaaaaaaaaa
ckaabaaaaaaaaaaadiaaaaahhcaabaaaadaaaaaakgakbaaaaaaaaaaaegacbaaa
abaaaaaabaaaaaahecaabaaaaaaaaaaaegacbaaaacaaaaaaegacbaaaacaaaaaa
eeaaaaafecaabaaaaaaaaaaackaabaaaaaaaaaaadiaaaaahhcaabaaaacaaaaaa
kgakbaaaaaaaaaaaegacbaaaacaaaaaabaaaaaahecaabaaaaaaaaaaaegacbaaa
adaaaaaaegacbaaaacaaaaaaaaaaaaahecaabaaaaaaaaaaackaabaaaaaaaaaaa
ckaabaaaaaaaaaaadcaaaaakhcaabaaaacaaaaaaegacbaaaacaaaaaakgakbaia
ebaaaaaaaaaaaaaaegacbaaaadaaaaaabaaaaaahecaabaaaaaaaaaaaegacbaaa
acaaaaaaegacbaaaacaaaaaaeeaaaaafecaabaaaaaaaaaaackaabaaaaaaaaaaa
diaaaaahhcaabaaaacaaaaaakgakbaaaaaaaaaaaegacbaaaacaaaaaadiaaaaai
mcaabaaaaaaaaaaaagbebaaaabaaaaaaagiecaaaaaaaaaaabfaaaaaaaaaaaaah
ecaabaaaaaaaaaaadkaabaaaaaaaaaaackaabaaaaaaaaaaadiaaaaahecaabaaa
aaaaaaaackaabaaaaaaaaaaaabeaaaaaaaaaiadobnaaaaaiicaabaaaaaaaaaaa
ckaabaaaaaaaaaaackaabaiaebaaaaaaaaaaaaaabkaaaaagecaabaaaaaaaaaaa
ckaabaiaibaaaaaaaaaaaaaadhaaaaakecaabaaaaaaaaaaadkaabaaaaaaaaaaa
ckaabaaaaaaaaaaackaabaiaebaaaaaaaaaaaaaadcaaaaakicaabaaaaaaaaaaa
ckaabaaaacaaaaaabkiacaaaaaaaaaaaapaaaaaackaabaaaabaaaaaadbaaaaaj
icaabaaaaaaaaaaabkiacaiaebaaaaaaabaaaaaaafaaaaaadkaabaaaaaaaaaaa
dcaaaaamicaabaaaabaaaaaackbabaiaebaaaaaaacaaaaaabkaabaaaaaaaaaaa
bkiacaiaebaaaaaaabaaaaaaafaaaaaaaoaaaaahicaabaaaabaaaaaadkaabaaa
abaaaaaackaabaaaacaaaaaadhaaaaakicaabaaaaaaaaaaadkaabaaaaaaaaaaa
dkaabaaaabaaaaaabkiacaaaaaaaaaaaapaaaaaadcaaaaajlcaabaaaacaaaaaa
egaibaaaacaaaaaapgapbaaaaaaaaaaaegaibaaaabaaaaaadiaaaaaihcaabaaa
adaaaaaafgafbaaaabaaaaaaegidcaaaaaaaaaaaahaaaaaadcaaaaakhcaabaaa
adaaaaaaegidcaaaaaaaaaaaagaaaaaaagaabaaaabaaaaaaegacbaaaadaaaaaa
dcaaaaakhcaabaaaadaaaaaaegidcaaaaaaaaaaaaiaaaaaakgakbaaaabaaaaaa
egacbaaaadaaaaaaaaaaaaaihcaabaaaadaaaaaaegacbaaaadaaaaaaegidcaaa
aaaaaaaaajaaaaaadiaaaaaihcaabaaaaeaaaaaafgafbaaaacaaaaaaegidcaaa
aaaaaaaaahaaaaaadcaaaaakhcaabaaaaeaaaaaaegidcaaaaaaaaaaaagaaaaaa
agaabaaaacaaaaaaegacbaaaaeaaaaaadcaaaaakhcaabaaaaeaaaaaaegidcaaa
aaaaaaaaaiaaaaaapgapbaaaacaaaaaaegacbaaaaeaaaaaaaaaaaaaihcaabaaa
aeaaaaaaegacbaaaaeaaaaaaegidcaaaaaaaaaaaajaaaaaaaoaaaaakicaabaaa
aaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpckaabaaaadaaaaaa
aoaaaaakicaabaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadp
ckaabaaaaeaaaaaadiaaaaahhcaabaaaafaaaaaapgapbaaaaaaaaaaaegacbaaa
abaaaaaadiaaaaahdcaabaaaagaaaaaapgapbaaaaaaaaaaabgafbaaaadaaaaaa
diaaaaahdcaabaaaabaaaaaapgapbaaaabaaaaaabgafbaaaaeaaaaaadcaaaaak
dcaabaaaabaaaaaaegaabaaaadaaaaaapgapbaaaaaaaaaaabgafbaiaebaaaaaa
abaaaaaaapaaaaahbcaabaaaabaaaaaaegaabaaaabaaaaaaegaabaaaabaaaaaa
dbaaaaahbcaabaaaabaaaaaaakaabaaaabaaaaaaabeaaaaabhlhnbdiabaaaaah
bcaabaaaabaaaaaaakaabaaaabaaaaaaabeaaaaaaknhcddmdcaaaaajdcaabaaa
abaaaaaabgafbaaaaeaaaaaapgapbaaaabaaaaaaagaabaaaabaaaaaadcaaaaak
mcaabaaaagaaaaaafgabbaiaebaaaaaaadaaaaaapgapbaaaaaaaaaaaagaebaaa
abaaaaaadbaaaaajbcaabaaaabaaaaaadkaabaiaibaaaaaaagaaaaaackaabaia
ibaaaaaaagaaaaaadhaaaaajpcaabaaaadaaaaaaagaabaaaabaaaaaaegaobaaa
agaaaaaabgalbaaaagaaaaaadbaaaaahccaabaaaabaaaaaaabeaaaaaaaaaaaaa
ckaabaaaadaaaaaadbaaaaahbcaabaaaaeaaaaaackaabaaaadaaaaaaabeaaaaa
aaaaaaaaboaaaaaiccaabaaaabaaaaaabkaabaiaebaaaaaaabaaaaaaakaabaaa
aeaaaaaaclaaaaafbcaabaaaaeaaaaaabkaabaaaabaaaaaaaoaaaaahccaabaaa
abaaaaaaakaabaaaaeaaaaaackaabaaaadaaaaaadcaaaaaklcaabaaaacaaaaaa
egambaaaacaaaaaapgapbaaaabaaaaaaegaibaiaebaaaaaaafaaaaaadiaaaaah
lcaabaaaacaaaaaafgafbaaaabaaaaaaegambaaaacaaaaaaaaaaaaaiicaabaaa
abaaaaaadkaabaiaebaaaaaaaaaaaaaadkaabaaaabaaaaaadiaaaaahicaabaaa
abaaaaaabkaabaaaabaaaaaadkaabaaaabaaaaaadiaaaaahccaabaaaaeaaaaaa
bkaabaaaabaaaaaadkaabaaaadaaaaaaaoaaaaajccaabaaaabaaaaaackaabaia
ebaaaaaaabaaaaaaakiacaaaaaaaaaaaapaaaaaaddaaaaahccaabaaaabaaaaaa
bkaabaaaabaaaaaaabeaaaaaaaaaiadpaaaaaaaiccaabaaaabaaaaaabkaabaia
ebaaaaaaabaaaaaaabeaaaaaaaaaiadpdcaaaaakccaabaaaabaaaaaabkaabaaa
abaaaaaadkiacaaaaaaaaaaaaoaaaaaaabeaaaaaaaaaiadpdiaaaaahdcaabaaa
aeaaaaaafgafbaaaabaaaaaaegaabaaaaeaaaaaadiaaaaahlcaabaaaacaaaaaa
fgafbaaaabaaaaaaegambaaaacaaaaaadiaaaaahicaabaaaaeaaaaaabkaabaaa
abaaaaaadkaabaaaabaaaaaadcaaaaajmcaabaaaabaaaaaaagaebaaaaeaaaaaa
kgakbaaaaaaaaaaaagaebaaaadaaaaaadcaaaaajhcaabaaaadaaaaaaegadbaaa
acaaaaaakgakbaaaaaaaaaaaegacbaaaafaaaaaadcaaaaajecaabaaaaaaaaaaa
dkaabaaaaeaaaaaackaabaaaaaaaaaaadkaabaaaaaaaaaaadgaaaaafecaabaaa
aeaaaaaadkaabaaaacaaaaaadgaaaaaidcaabaaaafaaaaaaaceaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaadgaaaaafdcaabaaaagaaaaaaogakbaaaabaaaaaa
dgaaaaaficaabaaaaaaaaaaaabeaaaaaaaaaaaaadgaaaaafbcaabaaaahaaaaaa
abeaaaaaaaaaaaaadgaaaaafecaabaaaagaaaaaackaabaaaadaaaaaadgaaaaaf
icaabaaaagaaaaaackaabaaaaaaaaaaadgaaaaaficaabaaaacaaaaaaabeaaaaa
aaaaaaaadaaaaaabdbaaaaaiicaabaaaadaaaaaadkaabaaaaaaaaaaaakiacaaa
aaaaaaaaaoaaaaaacaaaaaahecaabaaaafaaaaaadkaabaaaacaaaaaaabeaaaaa
aaaaaaaaabaaaaahicaabaaaadaaaaaadkaabaaaadaaaaaackaabaaaafaaaaaa
adaaaaaddkaabaaaadaaaaaaaaaaaaahpcaabaaaagaaaaaaegaobaaaaeaaaaaa
egaobaaaagaaaaaadcaaaaajicaabaaaadaaaaaackaabaaaaeaaaaaaabeaaaaa
aaaaaadpckaabaaaagaaaaaadcaaaaajecaabaaaafaaaaaadkaabaaaaeaaaaaa
abeaaaaaaaaaaadpdkaabaaaagaaaaaaaoaaaaahccaabaaaahaaaaaadkaabaaa
adaaaaaackaabaaaafaaaaaadbaaaaahicaabaaaadaaaaaaakaabaaaahaaaaaa
bkaabaaaahaaaaaadhaaaaajdcaabaaaahaaaaaapgapbaaaadaaaaaaegaabaaa
ahaaaaaabgafbaaaahaaaaaadhaaaaajmcaabaaaafaaaaaaagaabaaaabaaaaaa
fgabbaaaagaaaaaaagaebaaaagaaaaaadiaaaaaidcaabaaaafaaaaaaogakbaaa
afaaaaaaogikcaaaaaaaaaaabfaaaaaaeiaaaaalpcaabaaaaiaaaaaaegaabaaa
afaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaaabeaaaaaaaaaaaaadcaaaaal
icaabaaaadaaaaaaakiacaaaabaaaaaaahaaaaaaakaabaaaaiaaaaaabkiacaaa
abaaaaaaahaaaaaaaoaaaaakicaabaaaadaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaiadpaaaaiadpdkaabaaaadaaaaaadiaaaaajicaabaaaadaaaaaadkaabaaa
adaaaaaackiacaiaebaaaaaaabaaaaaaafaaaaaaeiaaaaalpcaabaaaaiaaaaaa
egaabaaaafaaaaaaeghobaaaadaaaaaaaagabaaaacaaaaaaabeaaaaaaaaaaaaa
bnaaaaahicaabaaaadaaaaaadkaabaaaadaaaaaaakaabaaaahaaaaaadcaaaaan
ecaabaaaafaaaaaaakaabaaaaiaaaaaackiacaiaebaaaaaaabaaaaaaafaaaaaa
ckiacaiaebaaaaaaaaaaaaaaaoaaaaaabnaaaaahecaabaaaafaaaaaabkaabaaa
ahaaaaaackaabaaaafaaaaaaabaaaaahicaabaaaacaaaaaadkaabaaaadaaaaaa
ckaabaaaafaaaaaaaaaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaa
aaaaiadpbgaaaaabdbaaaaahecaabaaaaaaaaaaaabeaaaaaaaaaiadpbkaabaaa
abaaaaaaabaaaaahecaabaaaaaaaaaaadkaabaaaacaaaaaackaabaaaaaaaaaaa
bpaaaeadckaabaaaaaaaaaaaaaaaaaaipcaabaaaahaaaaaaegaobaiaebaaaaaa
aeaaaaaaegaobaaaagaaaaaaaoaaaaahpcaabaaaaeaaaaaaegaobaaaaeaaaaaa
fgafbaaaabaaaaaadiaaaaahecaabaaaaaaaaaaabkaabaaaabaaaaaaabeaaaaa
aaaaaadpaoaaaaahccaabaaaabaaaaaackaabaaaahaaaaaadkaabaaaahaaaaaa
dgaaaaafmcaabaaaabaaaaaaagaebaaaafaaaaaadgaaaaafbcaabaaaaiaaaaaa
bkaabaaaabaaaaaadgaaaaafpcaabaaaagaaaaaaegaobaaaahaaaaaadgaaaaaf
mcaabaaaadaaaaaakgakbaaaaaaaaaaadgaaaaafecaabaaaafaaaaaaabeaaaaa
aaaaaaaadaaaaaabbnaaaaaiicaabaaaafaaaaaackaabaaaafaaaaaabkiacaaa
aaaaaaaaaoaaaaaaadaaaeaddkaabaaaafaaaaaadcaaaaajpcaabaaaagaaaaaa
egaobaaaaeaaaaaapgapbaaaadaaaaaaegaobaaaagaaaaaadcaaaaammcaabaaa
aiaaaaaakgaobaaaaeaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaalpaaaaaalp
kgaobaaaagaaaaaaaoaaaaahccaabaaaaiaaaaaackaabaaaaiaaaaaadkaabaaa
aiaaaaaadbaaaaahicaabaaaafaaaaaaakaabaaaaiaaaaaabkaabaaaaiaaaaaa
dhaaaaajdcaabaaaaiaaaaaapgapbaaaafaaaaaaegaabaaaaiaaaaaabgafbaaa
aiaaaaaadhaaaaajmcaabaaaaiaaaaaaagaabaaaabaaaaaafgabbaaaagaaaaaa
agaebaaaagaaaaaadiaaaaaimcaabaaaabaaaaaakgaobaaaaiaaaaaakgiocaaa
aaaaaaaabfaaaaaadiaaaaahecaabaaaadaaaaaackaabaaaadaaaaaaabeaaaaa
aaaaaadpeiaaaaalpcaabaaaajaaaaaaogakbaaaabaaaaaaeghobaaaabaaaaaa
aagabaaaabaaaaaaabeaaaaaaaaaaaaadcaaaaalicaabaaaafaaaaaaakiacaaa
abaaaaaaahaaaaaaakaabaaaajaaaaaabkiacaaaabaaaaaaahaaaaaaaoaaaaak
icaabaaaafaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpdkaabaaa
afaaaaaadiaaaaajicaabaaaafaaaaaadkaabaaaafaaaaaackiacaiaebaaaaaa
abaaaaaaafaaaaaaeiaaaaalpcaabaaaajaaaaaaogakbaaaabaaaaaaeghobaaa
adaaaaaaaagabaaaacaaaaaaabeaaaaaaaaaaaaabnaaaaahicaabaaaafaaaaaa
dkaabaaaafaaaaaaakaabaaaaiaaaaaadcaaaaanecaabaaaaiaaaaaaakaabaaa
ajaaaaaackiacaiaebaaaaaaabaaaaaaafaaaaaackiacaiaebaaaaaaaaaaaaaa
aoaaaaaabnaaaaahccaabaaaaiaaaaaabkaabaaaaiaaaaaackaabaaaaiaaaaaa
abaaaaahicaabaaaafaaaaaadkaabaaaafaaaaaabkaabaaaaiaaaaaadhaaaaak
icaabaaaadaaaaaadkaabaaaafaaaaaackaabaiaebaaaaaaadaaaaaackaabaaa
adaaaaaaaaaaaaahecaabaaaafaaaaaackaabaaaafaaaaaaabeaaaaaaaaaiadp
bgaaaaabdgaaaaafdcaabaaaafaaaaaaogakbaaaabaaaaaabfaaaaabdcaaaaaj
dcaabaaaagaaaaaaegaabaaaacaaaaaapgapbaaaaaaaaaaaegaabaaaadaaaaaa
aoaaaaahhcaabaaaabaaaaaaegacbaaaagaaaaaapgapbaaaagaaaaaaddaaaaah
bcaabaaaaaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaiadpaoaaaaaiecaabaaa
aaaaaaaadkaabaaaaaaaaaaaakiacaaaaaaaaaaaaoaaaaaaaaaaaaaiecaabaaa
aaaaaaaackaabaiaebaaaaaaaaaaaaaaabeaaaaaaaaaiadpdiaaaaahbcaabaaa
aaaaaaaackaabaaaaaaaaaaaakaabaaaaaaaaaaadcaaaaapmcaabaaaaaaaaaaa
agaebaaaafaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaeaaaaaaaeaaceaaaaa
aaaaaaaaaaaaaaaaaaaaialpaaaaialpdeaaaaajecaabaaaaaaaaaaadkaabaia
ibaaaaaaaaaaaaaackaabaiaibaaaaaaaaaaaaaaddaaaaahecaabaaaaaaaaaaa
ckaabaaaaaaaaaaaabeaaaaaaaaaiadpaaaaaaajecaabaaaaaaaaaaackaabaaa
aaaaaaaackiacaiaebaaaaaaaaaaaaaaapaaaaaadeaaaaahecaabaaaaaaaaaaa
ckaabaaaaaaaaaaaabeaaaaaaaaaaaaaaaaaaaajicaabaaaaaaaaaaackiacaia
ebaaaaaaaaaaaaaaapaaaaaaabeaaaaaaaaaiadpaoaaaaahecaabaaaaaaaaaaa
ckaabaaaaaaaaaaadkaabaaaaaaaaaaaaaaaaaaiecaabaaaaaaaaaaackaabaia
ebaaaaaaaaaaaaaaabeaaaaaaaaaiadpdiaaaaahbcaabaaaaaaaaaaackaabaaa
aaaaaaaaakaabaaaaaaaaaaadbaaaaajecaabaaaaaaaaaaaakiacaaaaaaaaaaa
baaaaaaadkiacaaaaaaaaaaaapaaaaaadgaaaaagbcaabaaaacaaaaaaakiacaaa
aaaaaaaabaaaaaaadgaaaaagccaabaaaacaaaaaadkiacaaaaaaaaaaaapaaaaaa
dhaaaaajmcaabaaaaaaaaaaakgakbaaaaaaaaaaaagaebaaaacaaaaaafgabbaaa
acaaaaaadeaaaaahicaabaaaabaaaaaackaabaaaaaaaaaaackaabaaaacaaaaaa
ddaaaaahicaabaaaabaaaaaadkaabaaaaaaaaaaadkaabaaaabaaaaaaaaaaaaai
icaabaaaabaaaaaackaabaiaebaaaaaaaaaaaaaadkaabaaaabaaaaaaaaaaaaai
ecaabaaaaaaaaaaackaabaiaebaaaaaaaaaaaaaadkaabaaaaaaaaaaaaoaaaaah
ecaabaaaaaaaaaaadkaabaaaabaaaaaackaabaaaaaaaaaaaaaaaaaaiecaabaaa
aaaaaaaackaabaiaebaaaaaaaaaaaaaaabeaaaaaaaaaiadpdiaaaaahbcaabaaa
aaaaaaaackaabaaaaaaaaaaaakaabaaaaaaaaaaadcaaaaakocaabaaaaaaaaaaa
agbjbaaaacaaaaaafgafbaaaaaaaaaaaagajbaiaebaaaaaaabaaaaaabaaaaaah
ccaabaaaaaaaaaaajgahbaaaaaaaaaaajgahbaaaaaaaaaaaelaaaaafccaabaaa
aaaaaaaabkaabaaaaaaaaaaaaocaaaaiccaabaaaaaaaaaaabkaabaaaaaaaaaaa
bkiacaaaaaaaaaaaapaaaaaaaaaaaaaiccaabaaaaaaaaaaabkaabaiaebaaaaaa
aaaaaaaaabeaaaaaaaaaiadpdiaaaaahbcaabaaaaaaaaaaabkaabaaaaaaaaaaa
akaabaaaaaaaaaaaabaaaaahccaabaaaaaaaaaaadkaabaaaacaaaaaaabeaaaaa
aaaaiadpdiaaaaahiccabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaaaaaaaaaa
aaaaaaaifcaabaaaaaaaaaaaagabbaaaafaaaaaaagbbbaiaebaaaaaaabaaaaaa
dcaaaaajdcaabaaaaaaaaaaafgafbaaaaaaaaaaaigaabaaaaaaaaaaaegbabaaa
abaaaaaaefaaaaajpcaabaaaaaaaaaaaegaabaaaaaaaaaaaeghobaaaaeaaaaaa
aagabaaaaaaaaaaadgaaaaafhccabaaaaaaaaaaaegacbaaaaaaaaaaadoaaaaab
""
}
}
 }
}
Fallback ""Diffuse""
}";
	}
}
