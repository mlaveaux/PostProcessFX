using PostProcessFX.EffectMenu;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace PostProcessFX
{
	/**
	 * Add and configure screen space reflection effect.
	 * 
	 * @TODO: Only works in the north direction, because depth is not written when looking at the south.
	 * @TODO: Fix deferred shading as imposters are drawn wrongly.
	 */
	class ScreenSpaceReflectionEffect : IEffectMenu
	{
		private ScreenSpaceReflection reflectionComponent = null;

		public ScreenSpaceReflectionEffect(EffectConfig config) {
			reflectionComponent = Camera.main.GetComponent<ScreenSpaceReflection>();
			applyConfig(config);
		}

		~ScreenSpaceReflectionEffect()
		{
			Disable();
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

			applyConfig(config);
		}

		private void applyConfig(EffectConfig config)
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

		private void Enable()
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
				}
			}
			
			reflectionComponent.enabled = true;
		}

		private void Disable()
		{
			if (reflectionComponent != null)
			{
				MonoBehaviour.DestroyImmediate(reflectionComponent);
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

		private const String screenSpaceReflectionShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 29.5KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""kode80/SSR"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = ""white"" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 10 math
 //        d3d9 : 12 math
 //      opengl : 171 math, 8 texture, 15 branch
 // Stats for Fragment shader:
 //       d3d11 : 141 math, 4 texture, 3 branch
 //        d3d9 : 178 math, 12 texture, 11 branch
 Pass {
  Tags { ""RenderType""=""Opaque"" }
  ZTest False
  ZWrite Off
  Cull Off
  GpuProgramID 57439
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 171 math, 8 textures, 15 branches
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
uniform sampler2D _CameraGBufferTexture2;
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
uniform mat4 _NormalMatrix;
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
  mat3 tmpvar_4;
  tmpvar_4[0] = _NormalMatrix[0].xyz;
  tmpvar_4[1] = _NormalMatrix[1].xyz;
  tmpvar_4[2] = _NormalMatrix[2].xyz;
  vec3 tmpvar_5;
  tmpvar_5 = normalize(tmpvar_3);
  vec3 tmpvar_6;
  tmpvar_6 = normalize((tmpvar_4 * (
    (texture2D (_CameraGBufferTexture2, xlv_TEXCOORD0).xyz * 2.0)
   - 1.0)));
  vec3 tmpvar_7;
  tmpvar_7 = normalize((tmpvar_5 - (2.0 * 
    (dot (tmpvar_6, tmpvar_5) * tmpvar_6)
  )));
  vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0 * _RenderBufferSize);
  float tmpvar_9;
  tmpvar_9 = ((tmpvar_8.x + tmpvar_8.y) * 0.25);
  float tmpvar_10;
  tmpvar_10 = fract(abs(tmpvar_9));
  float tmpvar_11;
  if ((tmpvar_9 >= 0.0)) {
    tmpvar_11 = tmpvar_10;
  } else {
    tmpvar_11 = -(tmpvar_10);
  };
  vec2 hitPixel_12;
  bool intersect_13;
  vec4 dPQK_14;
  vec4 pqk_15;
  float zB_16;
  float zA_17;
  float i_18;
  bool permute_19;
  vec2 delta_20;
  vec2 P0_21;
  vec3 Q0_22;
  float tmpvar_23;
  if (((tmpvar_3.z + (tmpvar_7.z * _MaxRayDistance)) > -(_ProjectionParams.y))) {
    tmpvar_23 = ((-(_ProjectionParams.y) - tmpvar_3.z) / tmpvar_7.z);
  } else {
    tmpvar_23 = _MaxRayDistance;
  };
  vec3 tmpvar_24;
  tmpvar_24 = (tmpvar_3 + (tmpvar_7 * tmpvar_23));
  vec4 tmpvar_25;
  tmpvar_25.w = 1.0;
  tmpvar_25.xyz = tmpvar_3;
  vec4 tmpvar_26;
  tmpvar_26 = (_CameraProjectionMatrix * tmpvar_25);
  vec4 tmpvar_27;
  tmpvar_27.w = 1.0;
  tmpvar_27.xyz = tmpvar_24;
  vec4 tmpvar_28;
  tmpvar_28 = (_CameraProjectionMatrix * tmpvar_27);
  float tmpvar_29;
  tmpvar_29 = (1.0/(tmpvar_26.w));
  float tmpvar_30;
  tmpvar_30 = (1.0/(tmpvar_28.w));
  vec3 tmpvar_31;
  tmpvar_31 = (tmpvar_3 * tmpvar_29);
  Q0_22 = tmpvar_31;
  vec3 tmpvar_32;
  tmpvar_32 = (tmpvar_24 * tmpvar_30);
  vec2 tmpvar_33;
  tmpvar_33 = (tmpvar_26.xy * tmpvar_29);
  P0_21 = tmpvar_33;
  vec2 tmpvar_34;
  tmpvar_34 = (tmpvar_28.xy * tmpvar_30);
  vec2 tmpvar_35;
  tmpvar_35 = (tmpvar_33 - tmpvar_34);
  float tmpvar_36;
  tmpvar_36 = dot (tmpvar_35, tmpvar_35);
  float tmpvar_37;
  if ((tmpvar_36 < 0.0001)) {
    tmpvar_37 = 0.01;
  } else {
    tmpvar_37 = 0.0;
  };
  vec2 tmpvar_38;
  tmpvar_38 = ((tmpvar_34 + tmpvar_37) - tmpvar_33);
  delta_20 = tmpvar_38;
  permute_19 = bool(0);
  float tmpvar_39;
  tmpvar_39 = abs(tmpvar_38.x);
  float tmpvar_40;
  tmpvar_40 = abs(tmpvar_38.y);
  if ((tmpvar_39 < tmpvar_40)) {
    permute_19 = bool(1);
    delta_20 = tmpvar_38.yx;
    P0_21 = tmpvar_33.yx;
  };
  float tmpvar_41;
  tmpvar_41 = sign(delta_20.x);
  float tmpvar_42;
  tmpvar_42 = (tmpvar_41 / delta_20.x);
  vec2 tmpvar_43;
  tmpvar_43.x = tmpvar_41;
  tmpvar_43.y = (delta_20.y * tmpvar_42);
  float tmpvar_44;
  tmpvar_44 = (1.0 + ((1.0 - 
    min (1.0, (-(tmpvar_3.z) / _PixelStrideZCuttoff))
  ) * _PixelStride));
  vec2 tmpvar_45;
  tmpvar_45 = (tmpvar_43 * tmpvar_44);
  vec3 tmpvar_46;
  tmpvar_46 = (((tmpvar_32 - tmpvar_31) * tmpvar_42) * tmpvar_44);
  float tmpvar_47;
  tmpvar_47 = (((tmpvar_30 - tmpvar_29) * tmpvar_42) * tmpvar_44);
  vec2 tmpvar_48;
  tmpvar_48 = (P0_21 + (tmpvar_45 * tmpvar_11));
  P0_21 = tmpvar_48;
  vec3 tmpvar_49;
  tmpvar_49 = (tmpvar_31 + (tmpvar_46 * tmpvar_11));
  Q0_22 = tmpvar_49;
  zA_17 = 0.0;
  zB_16 = 0.0;
  vec4 tmpvar_50;
  tmpvar_50.xy = tmpvar_48;
  tmpvar_50.z = tmpvar_49.z;
  tmpvar_50.w = (tmpvar_29 + (tmpvar_47 * tmpvar_11));
  pqk_15 = tmpvar_50;
  vec4 tmpvar_51;
  tmpvar_51.xy = tmpvar_45;
  tmpvar_51.z = tmpvar_46.z;
  tmpvar_51.w = tmpvar_47;
  dPQK_14 = tmpvar_51;
  intersect_13 = bool(0);
  i_18 = 0.0;
  for (; ((i_18 < _Iterations) && (intersect_13 == bool(0))); i_18 += 1.0) {
    vec4 tmpvar_52;
    tmpvar_52 = (pqk_15 + dPQK_14);
    pqk_15 = tmpvar_52;
    zA_17 = zB_16;
    float tmpvar_53;
    tmpvar_53 = (((dPQK_14.z * 0.5) + tmpvar_52.z) / ((dPQK_14.w * 0.5) + tmpvar_52.w));
    zB_16 = tmpvar_53;
    float aa_54;
    aa_54 = tmpvar_53;
    float bb_55;
    bb_55 = zA_17;
    if ((tmpvar_53 > zA_17)) {
      aa_54 = zA_17;
      bb_55 = tmpvar_53;
    };
    zB_16 = aa_54;
    zA_17 = bb_55;
    vec2 tmpvar_56;
    if (permute_19) {
      tmpvar_56 = tmpvar_52.yx;
    } else {
      tmpvar_56 = tmpvar_52.xy;
    };
    vec2 tmpvar_57;
    tmpvar_57 = (tmpvar_56 * _OneDividedByRenderBufferSize);
    hitPixel_12 = tmpvar_57;
    float cse_58;
    cse_58 = -(_ProjectionParams.z);
    intersect_13 = ((aa_54 <= (
      (1.0/(((_ZBufferParams.x * texture2DLod (_CameraDepthTexture, tmpvar_57, 0.0).x) + _ZBufferParams.y)))
     * cse_58)) && (bb_55 >= (
      (texture2DLod (_BackFaceDepthTex, tmpvar_57, 0.0).x * cse_58)
     - _PixelZSize)));
  };
  if (((tmpvar_44 > 1.0) && intersect_13)) {
    float stride_60;
    float originalStride_61;
    vec4 tmpvar_62;
    tmpvar_62 = (pqk_15 - tmpvar_51);
    pqk_15 = tmpvar_62;
    dPQK_14 = (tmpvar_51 / tmpvar_44);
    float tmpvar_63;
    tmpvar_63 = (tmpvar_44 * 0.5);
    originalStride_61 = tmpvar_63;
    stride_60 = tmpvar_63;
    float tmpvar_64;
    tmpvar_64 = (tmpvar_62.z / tmpvar_62.w);
    zA_17 = tmpvar_64;
    zB_16 = tmpvar_64;
    for (float j_59; j_59 < _BinarySearchIterations; j_59 += 1.0) {
      vec4 tmpvar_65;
      tmpvar_65 = (pqk_15 + (dPQK_14 * stride_60));
      pqk_15 = tmpvar_65;
      zA_17 = zB_16;
      float tmpvar_66;
      tmpvar_66 = (((dPQK_14.z * -0.5) + tmpvar_65.z) / ((dPQK_14.w * -0.5) + tmpvar_65.w));
      zB_16 = tmpvar_66;
      float aa_67;
      aa_67 = tmpvar_66;
      float bb_68;
      bb_68 = zA_17;
      if ((tmpvar_66 > zA_17)) {
        aa_67 = zA_17;
        bb_68 = tmpvar_66;
      };
      zB_16 = aa_67;
      zA_17 = bb_68;
      vec2 tmpvar_69;
      if (permute_19) {
        tmpvar_69 = tmpvar_65.yx;
      } else {
        tmpvar_69 = tmpvar_65.xy;
      };
      vec2 tmpvar_70;
      tmpvar_70 = (tmpvar_69 * _OneDividedByRenderBufferSize);
      hitPixel_12 = tmpvar_70;
      float tmpvar_71;
      tmpvar_71 = (originalStride_61 * 0.5);
      originalStride_61 = tmpvar_71;
      float tmpvar_72;
      if (((aa_67 <= (
        (1.0/(((_ZBufferParams.x * texture2DLod (_CameraDepthTexture, tmpvar_70, 0.0).x) + _ZBufferParams.y)))
       * 
        -(_ProjectionParams.z)
      )) && (bb_68 >= (
        (texture2DLod (_BackFaceDepthTex, tmpvar_70, 0.0).x * -(_ProjectionParams.z))
       - _PixelZSize)))) {
        tmpvar_72 = -(tmpvar_71);
      } else {
        tmpvar_72 = tmpvar_71;
      };
      stride_60 = tmpvar_72;
    };
  };
  Q0_22.xy = (tmpvar_49.xy + (tmpvar_46.xy * i_18));
  Q0_22.z = pqk_15.z;
  vec3 tmpvar_73;
  tmpvar_73 = (Q0_22 / pqk_15.w);
  vec2 tmpvar_74;
  tmpvar_74 = ((hitPixel_12 * 2.0) - 1.0);
  float tmpvar_75;
  tmpvar_75 = ((min (1.0, tmpvar_2) * (1.0 - 
    (i_18 / _Iterations)
  )) * (1.0 - (
    max (0.0, (min (1.0, max (
      abs(tmpvar_74.x)
    , 
      abs(tmpvar_74.y)
    )) - _ScreenEdgeFadeStart))
   / 
    (1.0 - _ScreenEdgeFadeStart)
  )));
  float aa_76;
  aa_76 = _EyeFadeStart;
  float bb_77;
  bb_77 = _EyeFadeEnd;
  if ((_EyeFadeStart > _EyeFadeEnd)) {
    aa_76 = _EyeFadeEnd;
    bb_77 = _EyeFadeStart;
  };
  vec3 tmpvar_78;
  tmpvar_78 = (tmpvar_3 - tmpvar_73);
  vec4 tmpvar_79;
  tmpvar_79.xyz = texture2D (_MainTex, mix (xlv_TEXCOORD0, hitPixel_12, vec2(float(intersect_13)))).xyz;
  tmpvar_79.w = (((tmpvar_75 * 
    (1.0 - ((clamp (tmpvar_7.z, aa_76, bb_77) - aa_76) / (bb_77 - aa_76)))
  ) * (1.0 - 
    clamp ((sqrt(dot (tmpvar_78, tmpvar_78)) / _MaxRayDistance), 0.0, 1.0)
  )) * float(intersect_13));
  gl_FragData[0] = tmpvar_79;
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
// Stats: 178 math, 12 textures, 11 branches
Matrix 0 [_CameraProjectionMatrix]
Matrix 4 [_NormalMatrix] 3
Float 10 [_BinarySearchIterations]
Float 17 [_EyeFadeEnd]
Float 16 [_EyeFadeStart]
Float 9 [_Iterations]
Float 14 [_MaxRayDistance]
Vector 19 [_OneDividedByRenderBufferSize]
Float 12 [_PixelStride]
Float 13 [_PixelStrideZCuttoff]
Float 11 [_PixelZSize]
Vector 7 [_ProjectionParams]
Vector 18 [_RenderBufferSize]
Float 15 [_ScreenEdgeFadeStart]
Vector 8 [_ZBufferParams]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_CameraDepthTexture] 2D 1
SetTexture 2 [_BackFaceDepthTex] 2D 2
SetTexture 3 [_CameraGBufferTexture1] 2D 3
SetTexture 4 [_CameraGBufferTexture2] 2D 4
""ps_3_0
def c20, 2, -1, 0.25, 1
def c21, -9.99999975e-005, 0, 0.00999999978, 1
def c22, 0, 0.5, -0.5, 0
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
mad r0.x, c8.x, r0.x, c8.y
rcp r0.x, r0.x
mul r1.xyz, r0.x, v1
texld r3, v0, s4
mad r0.yzw, r3.xxyz, c20.x, c20.y
dp3 r3.x, c4, r0.yzww
dp3 r3.y, c5, r0.yzww
dp3 r3.z, c6, r0.yzww
nrm r4.xyz, r1
nrm r5.xyz, r3
dp3 r0.y, r4, r5
add r0.y, r0.y, r0.y
mad r0.yzw, r5.xxyz, -r0.y, r4.xxyz
nrm r3.xyz, r0.yzww
mul r0.yz, c18.xxyw, v0.xxyw
add r0.y, r0.z, r0.y
mul r0.z, r0.y, c20.z
frc r0.z, r0_abs.z
cmp r0.y, r0.y, r0.z, -r0.z
mad r0.z, r3.z, c14.x, r1.z
add r0.z, -r0.z, -c7.y
mad r0.x, v1.z, -r0.x, -c7.y
rcp r0.w, r3.z
mul r0.x, r0.w, r0.x
cmp r0.x, r0.z, c14.x, r0.x
mad r4.xyz, r3, r0.x, r1
mov r1.w, c20.w
dp4 r3.x, c0, r1
dp4 r3.y, c1, r1
dp4 r0.x, c3, r1
mov r4.w, c20.w
dp4 r0.z, c0, r4
dp4 r0.w, c1, r4
dp4 r1.w, c3, r4
rcp r0.x, r0.x
rcp r1.w, r1.w
mul r2.yzw, r0.x, r1.xxyz
mul r5.xy, r0.x, r3
mul r6.xy, r0.zwzw, r1.w
mad r6.xy, r3, r0.x, -r6
dp2add r3.w, r6, r6, c21.x
cmp r3.w, r3.w, c21.y, c21.z
mad r0.zw, r0, r1.w, r3.w
mad r5.zw, r3.xyxy, -r0.x, r0
add r0.z, -r5_abs.w, r5_abs.z
cmp r5, r0.z, r5, r5.yxwz
cmp r0.w, -r5.z, c21.y, c21.w
cmp r3.x, r5.z, -c21.y, -c21.w
add r3.x, r0.w, r3.x
rcp r0.w, r5.z
mul r0.w, r0.w, r3.x
mad r4.xyz, r4, r1.w, -r2.yzww
mul r4.xyz, r0.w, r4
add r1.w, -r0.x, r1.w
mul r1.w, r0.w, r1.w
mul r3.y, r0.w, r5.w
rcp r0.w, c13.x
mad r0.w, -r1.z, -r0.w, c20.w
mov r3.w, c20.w
mad r4.w, r0.w, c12.x, r3.w
cmp r0.w, r0.w, r4.w, c20.w
mul r6.xy, r0.w, r3
mul r4.xyz, r0.w, r4
mul r6.w, r0.w, r1.w
mad r3.xy, r6, r0.y, r5
mad r2.yzw, r4.xxyz, r0.y, r2
mad r0.x, r6.w, r0.y, r0.x
mov r6.z, r4.z
mov r5.zw, c21.y
mov r5.xy, c21.y
mov r7.xy, r3
mov r0.y, c21.y
mov r8.y, c21.y
mov r7.z, r2.w
mov r7.w, r0.x
mov r1.w, c21.y
rep i0
add r4.z, r0.y, -c9.x
cmp r4.w, -r1.w, -c21.w, -c21.y
cmp r4.z, r4.z, c21.y, r4.w
cmp r4.z, r4.z, c21.w, c21.y
break_ne r4.z, -r4.z
add r7, r6, r7
mad r4.z, r6.z, c22.y, r7.z
mad r4.w, r6.w, c22.y, r7.w
rcp r4.w, r4.w
mul r8.x, r4.w, r4.z
mad r4.z, r4.z, -r4.w, r8.y
cmp r8.xy, r4.z, r8.yxzw, r8
cmp r4.zw, r0.z, r7.xyxy, r7.xyyx
mul r5.xy, r4.zwzw, c19
texldl r9, r5.xyww, s1
mad r4.z, c8.x, r9.x, c8.y
rcp r4.z, r4.z
texldl r9, r5, s2
mad r4.z, r4.z, -c7.z, -r8.y
mov r8.z, c7.z
mad r4.w, r9.x, -r8.z, -c11.x
add r4.w, -r4.w, r8.x
cmp r4.w, r4.w, c21.w, c21.y
cmp r1.w, r4.z, r4.w, c21.y
add r0.y, r0.y, c20.w
endrep
add r0.x, -r0.w, c20.w
cmp r0.x, r0.x, c21.y, r1.w
if_ne r0.x, -r0.x
add r8, -r6, r7
rcp r0.x, r0.w
mul r6, r0.x, r6
mul r0.x, r0.w, c22.y
rcp r0.w, r8.w
mul r0.w, r0.w, r8.z
mov r9.zw, c21.y
mov r9.xy, r5
mov r3.y, r0.w
mov r7, r8
mov r2.w, r0.x
mov r4.z, r0.x
mov r4.w, c21.y
rep i0
break_ge r4.w, c10.x
mad r7, r6, r4.z, r7
mad r5.zw, r6, c22.z, r7
rcp r5.w, r5.w
mul r3.x, r5.w, r5.z
mad r5.z, r5.z, -r5.w, r3.y
cmp r3.xy, r5.z, r3.yxzw, r3
cmp r5.zw, r0.z, r7.xyxy, r7.xyyx
mul r9.xy, r5.zwzw, c19
mul r2.w, r2.w, c22.y
texldl r10, r9.xyww, s1
mad r5.z, c8.x, r10.x, c8.y
rcp r5.z, r5.z
texldl r10, r9, s2
mad r5.z, r5.z, -c7.z, -r3.y
mov r10.z, c7.z
mad r5.w, r10.x, -r10.z, -c11.x
add r3.x, r3.x, -r5.w
cmp r3.x, r3.x, -c21.w, -c21.y
cmp r3.x, r5.z, r3.x, c21.y
cmp r4.z, r3.x, r2.w, -r2.w
add r4.w, r4.w, c20.w
endrep
mov r5.xy, r9
endif
mad r7.xy, r4, r0.y, r2.yzzw
rcp r0.x, r7.w
min r0.z, r2.x, c20.w
rcp r0.w, c9.x
mad r0.y, r0.y, -r0.w, c20.w
mul r0.y, r0.y, r0.z
mad r0.zw, r5.xyxy, c20.x, c20.y
max r2.x, r0_abs.z, r0_abs.w
min r0.z, r2.x, c20.w
add r0.z, r0.z, -c15.x
max r2.x, r0.z, c21.y
add r0.z, r3.w, -c15.x
rcp r0.z, r0.z
mad r0.z, r2.x, -r0.z, c20.w
mul r0.y, r0.z, r0.y
mov r2.xy, c17.x
add r0.z, r2.x, -c16.x
mov r2.x, c16.x
cmp r0.zw, r0.z, r2.xyxy, r2.xyyx
max r2.x, r3.z, r0.z
min r3.x, r0.w, r2.x
add r2.x, -r0.z, r3.x
add r0.z, -r0.z, r0.w
rcp r0.z, r0.z
mad r0.z, r2.x, -r0.z, c20.w
mul r0.y, r0.z, r0.y
mad r0.xzw, r7.xyyz, -r0.x, r1.xyyz
dp3 r0.x, r0.xzww, r0.xzww
rsq r0.x, r0.x
rcp r0.x, r0.x
rcp r0.z, c14.x
mul_sat r0.x, r0.z, r0.x
add r0.x, -r0.x, c20.w
mul r0.x, r0.x, r0.y
mul_pp oC0.w, r1.w, r0.x
cmp r0.xy, -r1.w, v0, r5
texld_pp r0, r0, s0
mov_pp oC0.xyz, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 141 math, 4 textures, 3 branches
SetTexture 0 [_CameraGBufferTexture1] 2D 3
SetTexture 1 [_CameraDepthTexture] 2D 1
SetTexture 2 [_CameraGBufferTexture2] 2D 4
SetTexture 3 [_BackFaceDepthTex] 2D 2
SetTexture 4 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 352
Matrix 96 [_CameraProjectionMatrix]
Matrix 272 [_NormalMatrix]
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
eefiecedngiopjgfladfomfcnnmkjjhcokfgklaaabaaaaaaoabhaaaaadaaaaaa
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
aceaaaaaaaaaaaeaaaaaaaeaaaaaaaeaaaaaaaaaaceaaaaaaaaaialpaaaaialp
aaaaialpaaaaaaaadiaaaaaihcaabaaaadaaaaaafgafbaaaacaaaaaaegiccaaa
aaaaaaaabcaaaaaadcaaaaaklcaabaaaacaaaaaaegiicaaaaaaaaaaabbaaaaaa
agaabaaaacaaaaaaegaibaaaadaaaaaadcaaaaakhcaabaaaacaaaaaaegiccaaa
aaaaaaaabdaaaaaakgakbaaaacaaaaaaegadbaaaacaaaaaabaaaaaahecaabaaa
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
