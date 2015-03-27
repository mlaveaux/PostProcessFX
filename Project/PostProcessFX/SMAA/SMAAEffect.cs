using System;

using UnityEngine;
using System.Collections;

namespace PostProcessFX
{
	public class SMAAEffect : MonoBehaviour
	{
		public bool ApplyEffect;
		public int State = 3;
		public int Passes = 8;

		private Texture2D black;
		private Material mat;

		void Start()
		{
			mat = new Material(smaaShaderText);

			black = new Texture2D(1, 1);
			black.SetPixel(0, 0, new Color(0, 0, 0, 0));
			black.Apply();

			//create texture generator
			GameObject obj = new GameObject();
			obj.name = "TextureGenerator";
			obj.AddComponent<AreaTexture>();
			obj.AddComponent<SearchTexture>();

			ApplyEffect = true;
		}

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(black, destination);

			Vector4 metrics = new Vector4(1 / (float)Screen.width, 1 / (float)Screen.height, Screen.width, Screen.height);

			if (this.ApplyEffect)
			{
				if (State == 1)
				{
					Graphics.Blit(source, destination, mat, 0);
				}
				else if (State == 2)
				{
					mat.SetTexture("areaTex", GameObject.Find("TextureGenerator").GetComponent<AreaTexture>().alphaTex);
					mat.SetTexture("luminTex", GameObject.Find("TextureGenerator").GetComponent<AreaTexture>().luminTex);
					mat.SetTexture("searchTex", GameObject.Find("TextureGenerator").GetComponent<SearchTexture>().alphaTex);
					mat.SetVector("SMAA_RT_METRICS", metrics);

					var rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

					Graphics.Blit(source, rt, mat, 0);
					Graphics.Blit(rt, destination, mat, 1);

					rt.Release();
				}
				else if (State == 3)
				{
					mat.SetTexture("areaTex", GameObject.Find("TextureGenerator").GetComponent<AreaTexture>().alphaTex);
					mat.SetTexture("luminTex", GameObject.Find("TextureGenerator").GetComponent<AreaTexture>().luminTex);
					mat.SetTexture("searchTex", GameObject.Find("TextureGenerator").GetComponent<SearchTexture>().alphaTex);
					mat.SetTexture("_SrcTex", source);
					mat.SetVector("SMAA_RT_METRICS", metrics);

					var rt = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
					var rt2 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
					var rt3 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

					Graphics.Blit(source, rt3);
					for (var i = 0; i < Passes; i++)
					{
						Graphics.Blit(black, rt);
						Graphics.Blit(black, rt2);

						Graphics.Blit(rt3, rt, mat, 0);

						Graphics.Blit(rt, rt2, mat, 1);
						Graphics.Blit(rt2, rt3, mat, 2);
					}
					Graphics.Blit(rt3, destination);

					rt.Release();
					rt2.Release();
					rt3.Release();
				}
			}
			else
			{
				Graphics.Blit(source, destination);
			}
		}

		private const String smaaShaderText = @"// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 48.0KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Custom/SMAAshader"" {
Properties {
 _SrcTex (""source texture"", 2D) = ""white"" { }
 _MainTex (""Base (RGB)"", 2D) = ""white"" { }
 areaTex (""area texture"", 2D) = ""white"" { }
 luminTex (""lumin texture"", 2D) = ""white"" { }
 searchTex (""search texture"", 2D) = ""white"" { }
 SMAA_RT_METRICS (""rt metrics"", Vector) = (0,0,0,0)
}
SubShader { 
 LOD 200
 Tags { ""RenderType""=""Opaque"" }


 // Stats for Vertex shader:
 //       d3d11 : 7 math
 //        d3d9 : 9 math
 //      opengl : 36 math, 8 texture, 1 branch
 // Stats for Fragment shader:
 //       d3d11 : 29 math, 7 texture
 //        d3d9 : 29 math, 8 texture
 Pass {
  Tags { ""RenderType""=""Opaque"" }
  ZTest Always
  ZWrite Off
  GpuProgramID 28231
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 36 math, 8 textures, 1 branches
""!!GLSL
#ifdef VERTEX

uniform vec4 SMAA_RT_METRICS;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD1 = ((SMAA_RT_METRICS.xyxy * vec4(-1.0, 0.0, 0.0, -1.0)) + gl_MultiTexCoord0.xyxy);
  xlv_TEXCOORD2 = ((SMAA_RT_METRICS.xyxy * vec4(1.0, 0.0, 0.0, 1.0)) + gl_MultiTexCoord0.xyxy);
  xlv_TEXCOORD3 = ((SMAA_RT_METRICS.xyxy * vec4(-2.0, 0.0, 0.0, -2.0)) + gl_MultiTexCoord0.xyxy);
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
void main ()
{
  vec4 delta_1;
  vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  vec3 tmpvar_3;
  tmpvar_3 = abs((tmpvar_2.xyz - texture2D (_MainTex, xlv_TEXCOORD1.xy).xyz));
  delta_1.x = max (max (tmpvar_3.x, tmpvar_3.y), tmpvar_3.z);
  vec3 tmpvar_4;
  tmpvar_4 = abs((tmpvar_2.xyz - texture2D (_MainTex, xlv_TEXCOORD1.zw).xyz));
  delta_1.y = max (max (tmpvar_4.x, tmpvar_4.y), tmpvar_4.z);
  vec2 tmpvar_5;
  tmpvar_5 = vec2(greaterThanEqual (delta_1.xy, vec2(0.05, 0.05)));
  float tmpvar_6;
  tmpvar_6 = dot (tmpvar_5, vec2(1.0, 1.0));
  if ((tmpvar_6 == 0.0)) {
    discard;
  };
  vec3 tmpvar_7;
  tmpvar_7 = abs((tmpvar_2.xyz - texture2D (_MainTex, xlv_TEXCOORD2.xy).xyz));
  delta_1.z = max (max (tmpvar_7.x, tmpvar_7.y), tmpvar_7.z);
  vec3 tmpvar_8;
  tmpvar_8 = abs((tmpvar_2.xyz - texture2D (_MainTex, xlv_TEXCOORD2.zw).xyz));
  delta_1.w = max (max (tmpvar_8.x, tmpvar_8.y), tmpvar_8.z);
  vec2 tmpvar_9;
  tmpvar_9 = max (delta_1.xy, delta_1.zw);
  vec3 tmpvar_10;
  tmpvar_10 = abs((tmpvar_2.xyz - texture2D (_MainTex, xlv_TEXCOORD3.xy).xyz));
  delta_1.z = max (max (tmpvar_10.x, tmpvar_10.y), tmpvar_10.z);
  vec3 tmpvar_11;
  tmpvar_11 = abs((tmpvar_2.xyz - texture2D (_MainTex, xlv_TEXCOORD3.zw).xyz));
  delta_1.w = max (max (tmpvar_11.x, tmpvar_11.y), tmpvar_11.z);
  vec2 tmpvar_12;
  tmpvar_12 = max (tmpvar_9, delta_1.zw);
  vec4 tmpvar_13;
  tmpvar_13.zw = vec2(0.0, 0.0);
  tmpvar_13.xy = (tmpvar_5 * vec2(greaterThanEqual (delta_1.xy, 
    (0.5 * max (tmpvar_12.xx, tmpvar_12.yy))
  )));
  gl_FragData[0] = tmpvar_13;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 9 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [SMAA_RT_METRICS]
""vs_3_0
def c5, -1, 0, 1, -2
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
mad o2, r0.xyxy, c5.xyyx, v1.xyxy
mad o3, r0.xyxy, c5.zyyz, v1.xyxy
mad o4, r0.xyxy, c5.wyyw, v1.xyxy
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 7 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 112
Vector 96 [SMAA_RT_METRICS]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedoclbmealfinpkkojalhnigdfnockjhfaabaaaaaabmadaaaaadaaaaaa
cmaaaaaakaaaaaaaeaabaaaaejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaafjaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaahaaaaaagaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apadaaaafaepfdejfeejepeoaaeoepfcenebemaafeeffiedepepfceeaaklklkl
epfdeheojiaaaaaaafaaaaaaaiaaaaaaiaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaimaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
imaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapaaaaaaimaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaapaaaaaaimaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaapaaaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfceeaaklklkl
fdeieefcneabaaaaeaaaabaahfaaaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaa
fjaaaaaeegiocaaaabaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaad
dcbabaaaacaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaa
abaaaaaagfaaaaadpccabaaaacaaaaaagfaaaaadpccabaaaadaaaaaagfaaaaad
pccabaaaaeaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaa
aaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaa
egaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaacaaaaaadcaaaaan
pccabaaaacaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaialpaaaaaaaa
aaaaaaaaaaaaialpegbebaaaacaaaaaadcaaaaanpccabaaaadaaaaaaegiecaaa
aaaaaaaaagaaaaaaaceaaaaaaaaaiadpaaaaaaaaaaaaaaaaaaaaiadpegbebaaa
acaaaaaadcaaaaanpccabaaaaeaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaa
aaaaaamaaaaaaaaaaaaaaaaaaaaaaamaegbebaaaacaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 29 math, 8 textures
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c0, -0.0500000007, 1, 0, 0.5
dcl_texcoord v0.xy
dcl_texcoord1 v1
dcl_texcoord2 v2
dcl_texcoord3 v3
dcl_2d s0
texld r0, v0, s0
texld r1, v1, s0
add r1.xyz, r0, -r1
max r0.w, r1_abs.x, r1_abs.y
max r2.x, r0.w, r1_abs.z
texld r1, v1.zwzw, s0
add r1.xyz, r0, -r1
max r0.w, r1_abs.x, r1_abs.y
max r2.y, r0.w, r1_abs.z
add r1.xy, r2, c0.x
cmp r1.xy, r1, c0.y, c0.z
dp2add r0.w, r1, c0.y, c0.z
cmp r3, -r0.w, -c0.y, -c0.z
texkill r3
texld r3, v2, s0
add r3.xyz, r0, -r3
max r0.w, r3_abs.x, r3_abs.y
max r4.x, r0.w, r3_abs.z
texld r3, v2.zwzw, s0
add r3.xyz, r0, -r3
max r0.w, r3_abs.x, r3_abs.y
max r4.y, r0.w, r3_abs.z
max r1.zw, r2.xyxy, r4.xyxy
texld r3, v3, s0
add r3.xyz, r0, -r3
max r0.w, r3_abs.x, r3_abs.y
max r4.x, r0.w, r3_abs.z
texld r3, v3.zwzw, s0
add r0.xyz, r0, -r3
max r2.z, r0_abs.x, r0_abs.y
max r4.y, r2.z, r0_abs.z
max r0.xy, r1.zwzw, r4
max r1.z, r0.x, r0.y
mad r0.xy, r1.z, -c0.w, r2
cmp oC0.xy, r0, r1, c0.z
mov oC0.zw, c0.z

""
}
SubProgram ""d3d11 "" {
// Stats: 29 math, 7 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedlghpakjaccegodkfebnfpogdmopmepdiabaaaaaafiagaaaaadaaaaaa
cmaaaaaammaaaaaaaaabaaaaejfdeheojiaaaaaaafaaaaaaaiaaaaaaiaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaimaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaimaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaaimaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaapapaaaaimaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapapaaaafdfgfpfagphdgjhegjgpgoaa
feeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl
fdeieefcfaafaaaaeaaaaaaafeabaaaafkaaaaadaagabaaaaaaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaadpcbabaaa
acaaaaaagcbaaaadpcbabaaaadaaaaaagcbaaaadpcbabaaaaeaaaaaagfaaaaad
pccabaaaaaaaaaaagiaaaaacafaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaa
egbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaaihcaabaaa
abaaaaaaegacbaaaaaaaaaaaegacbaiaebaaaaaaabaaaaaadeaaaaajicaabaaa
aaaaaaaabkaabaiaibaaaaaaabaaaaaaakaabaiaibaaaaaaabaaaaaadeaaaaai
bcaabaaaabaaaaaackaabaiaibaaaaaaabaaaaaadkaabaaaaaaaaaaaefaaaaaj
pcaabaaaacaaaaaaogbkbaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
aaaaaaaihcaabaaaacaaaaaaegacbaaaaaaaaaaaegacbaiaebaaaaaaacaaaaaa
deaaaaajicaabaaaaaaaaaaabkaabaiaibaaaaaaacaaaaaaakaabaiaibaaaaaa
acaaaaaadeaaaaaiccaabaaaabaaaaaackaabaiaibaaaaaaacaaaaaadkaabaaa
aaaaaaaabnaaaaakmcaabaaaabaaaaaaagaebaaaabaaaaaaaceaaaaaaaaaaaaa
aaaaaaaamnmmemdnmnmmemdnabaaaaakmcaabaaaabaaaaaakgaobaaaabaaaaaa
aceaaaaaaaaaaaaaaaaaaaaaaaaaiadpaaaaiadpapaaaaakicaabaaaaaaaaaaa
ogakbaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaaaaaaaaaaaaabiaaaaah
icaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaaaaaaaaaaanaaaeaddkaabaaa
aaaaaaaaefaaaaajpcaabaaaacaaaaaaegbabaaaadaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaaaaaaaaihcaabaaaacaaaaaaegacbaaaaaaaaaaaegacbaia
ebaaaaaaacaaaaaadeaaaaajicaabaaaaaaaaaaabkaabaiaibaaaaaaacaaaaaa
akaabaiaibaaaaaaacaaaaaadeaaaaaibcaabaaaacaaaaaackaabaiaibaaaaaa
acaaaaaadkaabaaaaaaaaaaaefaaaaajpcaabaaaadaaaaaaogbkbaaaadaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaaihcaabaaaadaaaaaaegacbaaa
aaaaaaaaegacbaiaebaaaaaaadaaaaaadeaaaaajicaabaaaaaaaaaaabkaabaia
ibaaaaaaadaaaaaaakaabaiaibaaaaaaadaaaaaadeaaaaaiccaabaaaacaaaaaa
ckaabaiaibaaaaaaadaaaaaadkaabaaaaaaaaaaadeaaaaahdcaabaaaacaaaaaa
egaabaaaabaaaaaaegaabaaaacaaaaaaefaaaaajpcaabaaaadaaaaaaegbabaaa
aeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaaihcaabaaaadaaaaaa
egacbaaaaaaaaaaaegacbaiaebaaaaaaadaaaaaadeaaaaajicaabaaaaaaaaaaa
bkaabaiaibaaaaaaadaaaaaaakaabaiaibaaaaaaadaaaaaadeaaaaaibcaabaaa
adaaaaaackaabaiaibaaaaaaadaaaaaadkaabaaaaaaaaaaaefaaaaajpcaabaaa
aeaaaaaaogbkbaaaaeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaai
hcaabaaaaaaaaaaaegacbaaaaaaaaaaaegacbaiaebaaaaaaaeaaaaaadeaaaaaj
bcaabaaaaaaaaaaabkaabaiaibaaaaaaaaaaaaaaakaabaiaibaaaaaaaaaaaaaa
deaaaaaiccaabaaaadaaaaaackaabaiaibaaaaaaaaaaaaaaakaabaaaaaaaaaaa
deaaaaahdcaabaaaaaaaaaaaegaabaaaacaaaaaaegaabaaaadaaaaaadeaaaaah
bcaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaaaaaaaaaadiaaaaahbcaabaaa
aaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaaadpbnaaaaahdcaabaaaaaaaaaaa
egaabaaaabaaaaaaagaabaaaaaaaaaaaabaaaaakdcaabaaaaaaaaaaaegaabaaa
aaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaaaaaaaaaaaaadiaaaaahdccabaaa
aaaaaaaaegaabaaaaaaaaaaaogakbaaaabaaaaaadgaaaaaimccabaaaaaaaaaaa
aceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 8 math
 //        d3d9 : 13 math
 //      opengl : 97 math, 17 texture, 10 branch
 // Stats for Fragment shader:
 //       d3d11 : 51 math, 8 branch
 //        d3d9 : 113 math, 34 texture, 24 branch
 Pass {
  Tags { ""RenderType""=""Opaque"" }
  ZTest Always
  ZWrite Off
  GpuProgramID 67362
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 97 math, 17 textures, 10 branches
""!!GLSL
#ifdef VERTEX

uniform vec4 SMAA_RT_METRICS;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
varying vec2 xlv_TEXCOORD4;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = ((SMAA_RT_METRICS.xyxy * vec4(-0.25, -0.125, 1.25, -0.125)) + gl_MultiTexCoord0.xyxy);
  vec4 tmpvar_2;
  tmpvar_2 = ((SMAA_RT_METRICS.xyxy * vec4(-0.125, -0.25, -0.125, 1.25)) + gl_MultiTexCoord0.xyxy);
  vec4 tmpvar_3;
  tmpvar_3.xy = tmpvar_1.xz;
  tmpvar_3.zw = tmpvar_2.yw;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_1;
  xlv_TEXCOORD2 = tmpvar_2;
  xlv_TEXCOORD3 = ((vec4(-64.0, 64.0, -64.0, 64.0) * SMAA_RT_METRICS.xxyy) + tmpvar_3);
  xlv_TEXCOORD4 = (gl_MultiTexCoord0.xy * SMAA_RT_METRICS.zw);
}


#endif
#ifdef FRAGMENT
#extension GL_ARB_shader_texture_lod : enable
uniform sampler2D _MainTex;
uniform sampler2D areaTex;
uniform sampler2D searchTex;
uniform sampler2D luminTex;
uniform vec4 SMAA_RT_METRICS;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
varying vec2 xlv_TEXCOORD4;
void main ()
{
  vec4 weights_1;
  weights_1 = vec4(0.0, 0.0, 0.0, 0.0);
  vec4 tmpvar_2;
  tmpvar_2 = texture2DLod (_MainTex, xlv_TEXCOORD0, 0.0);
  if ((tmpvar_2.y > 0.0)) {
    vec2 coords_3;
    vec2 d_4;
    vec2 texcoord_5;
    texcoord_5 = xlv_TEXCOORD1.xy;
    float end_6;
    end_6 = xlv_TEXCOORD3.x;
    vec2 e_7;
    e_7 = vec2(0.0, 1.0);
    while (true) {
      if (!(((
        (texcoord_5.x > end_6)
       && 
        (e_7.y > 0.8281)
      ) && (e_7.x == 0.0)))) {
        break;
      };
      e_7 = texture2DLod (_MainTex, texcoord_5, 0.0).xy;
      texcoord_5 = ((vec2(-2.0, -0.0) * SMAA_RT_METRICS.xy) + texcoord_5);
    };
    vec4 tmpvar_8;
    tmpvar_8.zw = vec2(0.0, 0.0);
    tmpvar_8.xy = ((vec2(0.515625, -2.0625) * e_7) + vec2(0.0, 2.0625));
    coords_3.x = ((SMAA_RT_METRICS.x * (
      (-2.007874 * texture2DLod (searchTex, tmpvar_8.xy, 0.0).w)
     + 3.25)) + texcoord_5.x);
    coords_3.y = xlv_TEXCOORD2.y;
    d_4.x = coords_3.x;
    float tmpvar_9;
    tmpvar_9 = texture2DLod (_MainTex, coords_3, 0.0).x;
    vec2 texcoord_10;
    texcoord_10 = xlv_TEXCOORD1.zw;
    float end_11;
    end_11 = xlv_TEXCOORD3.y;
    vec2 e_12;
    e_12 = vec2(0.0, 1.0);
    while (true) {
      if (!(((
        (texcoord_10.x < end_11)
       && 
        (e_12.y > 0.8281)
      ) && (e_12.x == 0.0)))) {
        break;
      };
      e_12 = texture2DLod (_MainTex, texcoord_10, 0.0).xy;
      texcoord_10 = ((vec2(2.0, 0.0) * SMAA_RT_METRICS.xy) + texcoord_10);
    };
    vec4 tmpvar_13;
    tmpvar_13.zw = vec2(0.0, 0.0);
    tmpvar_13.xy = ((vec2(0.515625, -2.0625) * e_12) + vec2(0.515625, 2.0625));
    coords_3.x = ((-(SMAA_RT_METRICS.x) * (
      (-2.007874 * texture2DLod (searchTex, tmpvar_13.xy, 0.0).w)
     + 3.25)) + texcoord_10.x);
    d_4.y = coords_3.x;
    vec2 tmpvar_14;
    tmpvar_14 = ((SMAA_RT_METRICS.z * d_4) - xlv_TEXCOORD4.x);
    d_4 = tmpvar_14;
    vec4 tmpvar_15;
    tmpvar_15.zw = vec2(0.0, 0.0);
    tmpvar_15.xy = (coords_3 + (vec2(1.0, 0.0) * SMAA_RT_METRICS.xy));
    vec2 texcoord_16;
    vec2 tmpvar_17;
    tmpvar_17.x = tmpvar_9;
    tmpvar_17.y = texture2DLod (_MainTex, tmpvar_15.xy, 0.0).x;
    texcoord_16 = ((vec2(0.00625, 0.001785714) * (
      (16.0 * floor(((4.0 * tmpvar_17) + vec2(0.5, 0.5))))
     + 
      sqrt(abs(tmpvar_14))
    )) + vec2(0.003125, 0.0008928571));
    vec2 tmpvar_18;
    tmpvar_18.x = texture2DLod (areaTex, texcoord_16, 0.0).w;
    tmpvar_18.y = texture2DLod (luminTex, texcoord_16, 0.0).w;
    weights_1.xy = tmpvar_18;
  };
  if ((tmpvar_2.x > 0.0)) {
    vec2 coords_1_19;
    vec2 d_1_20;
    vec2 texcoord_21;
    texcoord_21 = xlv_TEXCOORD2.xy;
    float end_22;
    end_22 = xlv_TEXCOORD3.z;
    vec2 e_23;
    e_23 = vec2(1.0, 0.0);
    while (true) {
      if (!(((
        (texcoord_21.y > end_22)
       && 
        (e_23.x > 0.8281)
      ) && (e_23.y == 0.0)))) {
        break;
      };
      e_23 = texture2DLod (_MainTex, texcoord_21, 0.0).xy;
      texcoord_21 = ((vec2(-0.0, -2.0) * SMAA_RT_METRICS.xy) + texcoord_21);
    };
    vec4 tmpvar_24;
    tmpvar_24.zw = vec2(0.0, 0.0);
    tmpvar_24.xy = ((vec2(0.515625, -2.0625) * e_23.yx) + vec2(0.0, 2.0625));
    coords_1_19.y = ((SMAA_RT_METRICS.y * (
      (-2.007874 * texture2DLod (searchTex, tmpvar_24.xy, 0.0).w)
     + 3.25)) + texcoord_21.y);
    coords_1_19.x = xlv_TEXCOORD1.x;
    d_1_20.x = coords_1_19.y;
    float tmpvar_25;
    tmpvar_25 = texture2DLod (_MainTex, coords_1_19, 0.0).y;
    vec2 texcoord_26;
    texcoord_26 = xlv_TEXCOORD2.zw;
    float end_27;
    end_27 = xlv_TEXCOORD3.w;
    vec2 e_28;
    e_28 = vec2(1.0, 0.0);
    while (true) {
      if (!(((
        (texcoord_26.y < end_27)
       && 
        (e_28.x > 0.8281)
      ) && (e_28.y == 0.0)))) {
        break;
      };
      e_28 = texture2DLod (_MainTex, texcoord_26, 0.0).xy;
      texcoord_26 = ((vec2(0.0, 2.0) * SMAA_RT_METRICS.xy) + texcoord_26);
    };
    vec4 tmpvar_29;
    tmpvar_29.zw = vec2(0.0, 0.0);
    tmpvar_29.xy = ((vec2(0.515625, -2.0625) * e_28.yx) + vec2(0.515625, 2.0625));
    coords_1_19.y = ((-(SMAA_RT_METRICS.y) * (
      (-2.007874 * texture2DLod (searchTex, tmpvar_29.xy, 0.0).w)
     + 3.25)) + texcoord_26.y);
    d_1_20.y = coords_1_19.y;
    vec2 tmpvar_30;
    tmpvar_30 = ((SMAA_RT_METRICS.w * d_1_20) - xlv_TEXCOORD4.y);
    d_1_20 = tmpvar_30;
    vec4 tmpvar_31;
    tmpvar_31.zw = vec2(0.0, 0.0);
    tmpvar_31.xy = (coords_1_19 + (vec2(0.0, 1.0) * SMAA_RT_METRICS.xy));
    vec2 texcoord_32;
    vec2 tmpvar_33;
    tmpvar_33.x = tmpvar_25;
    tmpvar_33.y = texture2DLod (_MainTex, tmpvar_31.xy, 0.0).y;
    texcoord_32 = ((vec2(0.00625, 0.001785714) * (
      (16.0 * floor(((4.0 * tmpvar_33) + vec2(0.5, 0.5))))
     + 
      sqrt(abs(tmpvar_30))
    )) + vec2(0.003125, 0.0008928571));
    vec2 tmpvar_34;
    tmpvar_34.x = texture2DLod (areaTex, texcoord_32, 0.0).w;
    tmpvar_34.y = texture2DLod (luminTex, texcoord_32, 0.0).w;
    weights_1.zw = tmpvar_34;
  };
  gl_FragData[0] = weights_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 13 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [SMAA_RT_METRICS]
""vs_3_0
def c5, -0.25, -0.125, 1.25, 0
def c6, -64, 64, 0, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2
dcl_texcoord2 o3
dcl_texcoord3 o4
dcl_texcoord4 o5.xy
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mul o5.xy, c4.zwzw, v1
mov r0.xy, c4
mad r1, r0.xxyy, c5.xzyy, v1.xxyy
mov o2, r1.xzyw
mad r2, r0.xyxy, c5.yxyz, v1.xyxy
mov r1.zw, r2.xyyw
mov o3, r2
mad o4, r0.xxyy, c6.xyxy, r1
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 8 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 112
Vector 96 [SMAA_RT_METRICS]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedmjpmlnjdnmmgofkdjhigdblopafochfgabaaaaaajmadaaaaadaaaaaa
cmaaaaaakaaaaaaafiabaaaaejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaafjaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaahaaaaaagaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apadaaaafaepfdejfeejepeoaaeoepfcenebemaafeeffiedepepfceeaaklklkl
epfdeheolaaaaaaaagaaaaaaaiaaaaaajiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaakeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
keaaaaaaaeaaaaaaaaaaaaaaadaaaaaaabaaaaaaamadaaaakeaaaaaaabaaaaaa
aaaaaaaaadaaaaaaacaaaaaaapaaaaaakeaaaaaaacaaaaaaaaaaaaaaadaaaaaa
adaaaaaaapaaaaaakeaaaaaaadaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapaaaaaa
fdfgfpfagphdgjhegjgpgoaafeeffiedepepfceeaaklklklfdeieefcdmacaaaa
eaaaabaaipaaaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafjaaaaaeegiocaaa
abaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaacaaaaaa
ghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaad
mccabaaaabaaaaaagfaaaaadpccabaaaacaaaaaagfaaaaadpccabaaaadaaaaaa
gfaaaaadpccabaaaaeaaaaaagiaaaaacacaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadiaaaaaimccabaaaabaaaaaaagbebaaaacaaaaaa
kgiocaaaaaaaaaaaagaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaacaaaaaa
dcaaaaanpcaabaaaaaaaaaaaagifcaaaaaaaaaaaagaaaaaaaceaaaaaaaaaialo
aaaakadpaaaaaaloaaaaaaloagbfbaaaacaaaaaadgaaaaafpccabaaaacaaaaaa
iganbaaaaaaaaaaadcaaaaanpcaabaaaabaaaaaaegiecaaaaaaaaaaaagaaaaaa
aceaaaaaaaaaaaloaaaaialoaaaaaaloaaaakadpegbebaaaacaaaaaadgaaaaaf
pccabaaaadaaaaaaegaobaaaabaaaaaadgaaaaafmcaabaaaaaaaaaaafganbaaa
abaaaaaadcaaaaanpccabaaaaeaaaaaaagifcaaaaaaaaaaaagaaaaaaaceaaaaa
aaaaiamcaaaaiaecaaaaiamcaaaaiaecegaobaaaaaaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 113 math, 34 textures, 24 branches
Vector 0 [SMAA_RT_METRICS]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [areaTex] 2D 1
SetTexture 2 [searchTex] 2D 2
SetTexture 3 [luminTex] 2D 3
""ps_3_0
def c1, 0.00625000009, 0.0017857143, 0.00312500005, 0.000892857148
def c2, 1, 0, 0, 0.828100026
def c3, 0.515625, -2.0625, 0, 2.0625
def c4, -2, -0, -2.00787401, 3.25
def c5, 4, 0.5, 16, 0
defi i0, 255, 0, 0, 0
dcl_texcoord v0.xy
dcl_texcoord1 v1
dcl_texcoord2 v2
dcl_texcoord3 v3
dcl_texcoord4 v4.xy
dcl_2d s0
dcl_2d s1
dcl_2d s2
dcl_2d s3
mul r0, c2.xxyy, v0.xyxx
texldl r0, r0, s0
if_lt -r0.y, c2.y
mov r1.xy, v1
mov r1.z, c2.x
mov r2.x, c2.y
rep i0
add r0.y, -r1.x, v3.x
add r0.z, -r1.z, c2.w
cmp r0.w, -r2_abs.x, -c2.x, -c2.y
cmp r0.z, r0.z, c2.y, r0.w
cmp r0.y, r0.y, c2.y, r0.z
cmp r0.y, r0.y, c2.x, c2.y
break_ne r0.y, -r0.y
mul r3, r1.xyxx, c2.xxyy
texldl r2, r3, s0
mov r3.xy, c0
mad r1.xy, r3, c4, r1
mov r1.z, r2.y
endrep
mov r2.yz, r1.xxzw
mad r1.xy, r2.xzzw, c3, c3.zwzw
mov r1.zw, c2.y
texldl r1, r1, s2
mad r0.y, r1.w, c4.z, c4.w
mad r1.x, c0.x, r0.y, r2.y
mul r1.yz, c2.xxyw, v2.y
texldl r2, r1.xyzz, s0
mov r3.xy, v1.zwzw
mov r3.z, c2.x
mov r4.x, c2.y
rep i0
add r0.y, r3.x, -v3.y
add r0.z, -r3.z, c2.w
cmp r0.w, -r4_abs.x, -c2.x, -c2.y
cmp r0.z, r0.z, c2.y, r0.w
cmp r0.y, r0.y, c2.y, r0.z
cmp r0.y, r0.y, c2.x, c2.y
break_ne r0.y, -r0.y
mul r5, r3.xyxx, c2.xxyy
texldl r4, r5, s0
mov r5.xy, c0
mad r3.xy, r5, -c4, r3
mov r3.z, r4.y
endrep
mov r4.yz, r3.xxzw
mad r3.xy, r4.xzzw, c3, c3.xwzw
mov r3.zw, c2.y
texldl r3, r3, s2
mad r0.y, r3.w, c4.z, c4.w
mad r1.w, -c0.x, r0.y, r4.y
mad r0.yz, c0.z, r1.xxww, -v4.x
rsq r0.y, r0_abs.y
rsq r0.z, r0_abs.z
rcp r3.x, r0.y
rcp r3.y, r0.z
mov r4.xy, c2
mad r1.xy, c0, r4, r1.wyzw
mov r1.zw, c2.y
texldl r1, r1, s0
mov r2.y, r1.x
mad r0.yz, r2.xxyw, c5.x, c5.y
frc r1.xy, r0.yzzw
add r0.yz, r0, -r1.xxyw
mad r0.yz, r0, c5.z, r3.xxyw
mad r1.xy, r0.yzzw, c1, c1.zwzw
mov r1.zw, c2.y
texldl r2, r1.xyww, s1
texldl r1, r1, s3
mov oC0.x, r2.w
mov oC0.y, r1.w
else
mov oC0.xy, c2.y
endif
if_lt -r0.x, c2.y
mov r0.xy, v2
mov r0.z, c2.x
mov r1.x, c2.y
rep i0
add r0.w, -r0.y, v3.z
add r1.w, -r0.z, c2.w
cmp r2.x, -r1_abs.x, -c2.x, -c2.y
cmp r1.w, r1.w, c2.y, r2.x
cmp r0.w, r0.w, c2.y, r1.w
cmp r0.w, r0.w, c2.x, c2.y
break_ne r0.w, -r0.w
mul r2, r0.xyxx, c2.xxyy
texldl r2, r2, s0
mov r3.xy, c0
mad r0.xy, r3, c4.yxzw, r0
mov r0.z, r2.x
mov r1.x, r2.y
endrep
mov r1.yz, r0
mad r0.xy, r1.xzzw, c3, c3.zwzw
mov r0.zw, c2.y
texldl r0, r0, s2
mad r0.x, r0.w, c4.z, c4.w
mad r0.x, c0.y, r0.x, r1.y
mul r0.yz, c2.xxyw, v1.x
texldl r1, r0.yxzz, s0
mov r2.xy, v2.zwzw
mov r2.z, c2.x
mov r3.x, c2.y
rep i0
add r0.y, r2.y, -v3.w
add r0.z, -r2.z, c2.w
cmp r1.x, -r3_abs.x, -c2.x, -c2.y
cmp r0.z, r0.z, c2.y, r1.x
cmp r0.y, r0.y, c2.y, r0.z
cmp r0.y, r0.y, c2.x, c2.y
break_ne r0.y, -r0.y
mul r4, r2.xyxx, c2.xxyy
texldl r4, r4, s0
mov r5.xy, c0
mad r2.xy, r5, -c4.yxzw, r2
mov r2.z, r4.x
mov r3.x, r4.y
endrep
mov r3.yz, r2
mad r2.xy, r3.xzzw, c3, c3.xwzw
mov r2.zw, c2.y
texldl r2, r2, s2
mad r0.y, r2.w, c4.z, c4.w
mad r0.w, -c0.y, r0.y, r3.y
mad r0.xy, c0.w, r0.xwzw, -v4.y
rsq r0.x, r0_abs.x
rsq r0.y, r0_abs.y
rcp r2.x, r0.x
rcp r2.y, r0.y
mov r0.x, c0.y
add r0.y, r0.x, r0.w
mul r0.zw, c2.xyxy, v1.x
texldl r0, r0.zyww, s0
mov r0.x, r1.y
mad r0.xy, r0, c5.x, c5.y
frc r0.zw, r0.xyxy
add r0.xy, -r0.zwzw, r0
mad r0.xy, r0, c5.z, r2
mad r0.xy, r0, c1, c1.zwzw
mov r0.zw, c2.y
texldl r1, r0.xyww, s1
texldl r0, r0, s3
mov oC0.z, r1.w
mov oC0.w, r0.w
else
mov oC0.zw, c2.y
endif

""
}
SubProgram ""d3d11 "" {
// Stats: 51 math, 8 branches
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [searchTex] 2D 2
SetTexture 2 [areaTex] 2D 1
SetTexture 3 [luminTex] 2D 3
ConstBuffer ""$Globals"" 112
Vector 96 [SMAA_RT_METRICS]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedcjlhgcodijfcmgggnngfkpnjlnhjahikabaaaaaagaapaaaaadaaaaaa
cmaaaaaaoeaaaaaabiabaaaaejfdeheolaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaakeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaakeaaaaaaaeaaaaaaaaaaaaaaadaaaaaaabaaaaaa
amamaaaakeaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapapaaaakeaaaaaa
acaaaaaaaaaaaaaaadaaaaaaadaaaaaaapapaaaakeaaaaaaadaaaaaaaaaaaaaa
adaaaaaaaeaaaaaaapapaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefceaaoaaaa
eaaaaaaajaadaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaa
aaaaaaaafkaaaaadaagabaaaabaaaaaafkaaaaadaagabaaaacaaaaaafkaaaaad
aagabaaaadaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaa
abaaaaaaffffaaaafibiaaaeaahabaaaacaaaaaaffffaaaafibiaaaeaahabaaa
adaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaadmcbabaaaabaaaaaa
gcbaaaadpcbabaaaacaaaaaagcbaaaadpcbabaaaadaaaaaagcbaaaadpcbabaaa
aeaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacafaaaaaaeiaaaaalpcaabaaa
aaaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaa
aaaaaaaadbaaaaakdcaabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
aaaaaaaabgafbaaaaaaaaaaabpaaaeadakaabaaaaaaaaaaadgaaaaafdcaabaaa
abaaaaaaegbabaaaacaaaaaadgaaaaafecaabaaaabaaaaaaabeaaaaaaaaaiadp
dgaaaaafbcaabaaaacaaaaaaabeaaaaaaaaaaaaadaaaaaabdbaaaaahbcaabaaa
aaaaaaaaakbabaaaaeaaaaaaakaabaaaabaaaaaadbaaaaahecaabaaaaaaaaaaa
abeaaaaafnpofddpckaabaaaabaaaaaaabaaaaahbcaabaaaaaaaaaaackaabaaa
aaaaaaaaakaabaaaaaaaaaaabiaaaaahecaabaaaaaaaaaaaakaabaaaacaaaaaa
abeaaaaaaaaaaaaaabaaaaahbcaabaaaaaaaaaaackaabaaaaaaaaaaaakaabaaa
aaaaaaaaadaaaaadakaabaaaaaaaaaaaeiaaaaalpcaabaaaacaaaaaaegaabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadcaaaaan
dcaabaaaabaaaaaaegiacaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaamaaaaaaaia
aaaaaaaaaaaaaaaaegaabaaaabaaaaaadgaaaaafecaabaaaabaaaaaabkaabaaa
acaaaaaabgaaaaabdgaaaaafgcaabaaaacaaaaaaagacbaaaabaaaaaadcaaaaap
fcaabaaaaaaaaaaaagacbaaaacaaaaaaaceaaaaaaaaaaedpaaaaaaaaaaaaaema
aaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaeeaaaaaaaaaeiaaaaalpcaabaaa
abaaaaaaigaabaaaaaaaaaaaeghobaaaabaaaaaaaagabaaaacaaaaaaabeaaaaa
aaaaaaaadcaaaaajbcaabaaaaaaaaaaadkaabaaaabaaaaaaabeaaaaaacibaama
abeaaaaaaaaafaeadcaaaaakbcaabaaaabaaaaaaakiacaaaaaaaaaaaagaaaaaa
akaabaaaaaaaaaaabkaabaaaacaaaaaadgaaaaafccaabaaaabaaaaaabkbabaaa
adaaaaaaeiaaaaalpcaabaaaacaaaaaaegaabaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaabeaaaaaaaaaaaaadgaaaaafdcaabaaaadaaaaaaogbkbaaa
acaaaaaadgaaaaafecaabaaaadaaaaaaabeaaaaaaaaaiadpdgaaaaafbcaabaaa
aeaaaaaaabeaaaaaaaaaaaaadaaaaaabdbaaaaahbcaabaaaaaaaaaaaakaabaaa
adaaaaaabkbabaaaaeaaaaaadbaaaaahecaabaaaaaaaaaaaabeaaaaafnpofddp
ckaabaaaadaaaaaaabaaaaahbcaabaaaaaaaaaaackaabaaaaaaaaaaaakaabaaa
aaaaaaaabiaaaaahecaabaaaaaaaaaaaakaabaaaaeaaaaaaabeaaaaaaaaaaaaa
abaaaaahbcaabaaaaaaaaaaackaabaaaaaaaaaaaakaabaaaaaaaaaaaadaaaaad
akaabaaaaaaaaaaaeiaaaaalpcaabaaaaeaaaaaaegaabaaaadaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadcaaaaandcaabaaaadaaaaaa
egiacaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaeaaaaaaaaaaaaaaaaaaaaaaaaa
egaabaaaadaaaaaadgaaaaafecaabaaaadaaaaaabkaabaaaaeaaaaaabgaaaaab
dgaaaaafgcaabaaaaeaaaaaaagacbaaaadaaaaaadcaaaaapfcaabaaaaaaaaaaa
agacbaaaaeaaaaaaaceaaaaaaaaaaedpaaaaaaaaaaaaaemaaaaaaaaaaceaaaaa
aaaaaedpaaaaaaaaaaaaaeeaaaaaaaaaeiaaaaalpcaabaaaadaaaaaaigaabaaa
aaaaaaaaeghobaaaabaaaaaaaagabaaaacaaaaaaabeaaaaaaaaaaaaadcaaaaaj
bcaabaaaaaaaaaaadkaabaaaadaaaaaaabeaaaaaacibaamaabeaaaaaaaaafaea
dcaaaaalecaabaaaabaaaaaaakiacaiaebaaaaaaaaaaaaaaagaaaaaaakaabaaa
aaaaaaaabkaabaaaaeaaaaaadcaaaaalfcaabaaaaaaaaaaakgikcaaaaaaaaaaa
agaaaaaaagacbaaaabaaaaaakgbkbaiaebaaaaaaabaaaaaaelaaaaagfcaabaaa
aaaaaaaaagacbaiaibaaaaaaaaaaaaaadcaaaaandcaabaaaabaaaaaaegiacaaa
aaaaaaaaagaaaaaaaceaaaaaaaaaiadpaaaaaaaaaaaaaaaaaaaaaaaaggakbaaa
abaaaaaaeiaaaaalpcaabaaaabaaaaaaegaabaaaabaaaaaabghobaaaaaaaaaaa
aagabaaaaaaaaaaaabeaaaaaaaaaaaaadgaaaaafbcaabaaaabaaaaaaakaabaaa
acaaaaaadiaaaaakdcaabaaaabaaaaaaegaabaaaabaaaaaaaceaaaaaaaaaiaea
aaaaiaeaaaaaaaaaaaaaaaaaeaaaaaafdcaabaaaabaaaaaaegaabaaaabaaaaaa
dcaaaaamfcaabaaaaaaaaaaaagabbaaaabaaaaaaaceaaaaaaaaaiaebaaaaaaaa
aaaaiaebaaaaaaaaagacbaaaaaaaaaaadcaaaaapfcaabaaaaaaaaaaaagacbaaa
aaaaaaaaaceaaaaamnmmmmdlaaaaaaaakbaookdkaaaaaaaaaceaaaaamnmmemdl
aaaaaaaakbaogkdkaaaaaaaaeiaaaaalpcaabaaaabaaaaaaigaabaaaaaaaaaaa
eghobaaaacaaaaaaaagabaaaabaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaa
acaaaaaaigaabaaaaaaaaaaaeghobaaaadaaaaaaaagabaaaadaaaaaaabeaaaaa
aaaaaaaadgaaaaafbccabaaaaaaaaaaadkaabaaaabaaaaaadgaaaaafcccabaaa
aaaaaaaadkaabaaaacaaaaaabcaaaaabdgaaaaaidccabaaaaaaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabfaaaaabbpaaaeadbkaabaaaaaaaaaaa
dgaaaaafdcaabaaaaaaaaaaaegbabaaaadaaaaaadgaaaaafecaabaaaaaaaaaaa
abeaaaaaaaaaiadpdgaaaaafbcaabaaaabaaaaaaabeaaaaaaaaaaaaadaaaaaab
dbaaaaahicaabaaaaaaaaaaackbabaaaaeaaaaaabkaabaaaaaaaaaaadbaaaaah
bcaabaaaacaaaaaaabeaaaaafnpofddpckaabaaaaaaaaaaaabaaaaahicaabaaa
aaaaaaaadkaabaaaaaaaaaaaakaabaaaacaaaaaabiaaaaahbcaabaaaacaaaaaa
akaabaaaabaaaaaaabeaaaaaaaaaaaaaabaaaaahicaabaaaaaaaaaaadkaabaaa
aaaaaaaaakaabaaaacaaaaaaadaaaaaddkaabaaaaaaaaaaaeiaaaaalpcaabaaa
abaaaaaaegaabaaaaaaaaaaabghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaa
aaaaaaaadcaaaaandcaabaaaaaaaaaaaegiacaaaaaaaaaaaagaaaaaaaceaaaaa
aaaaaaiaaaaaaamaaaaaaaaaaaaaaaaaegaabaaaaaaaaaaadgaaaaafecaabaaa
aaaaaaaabkaabaaaabaaaaaabgaaaaabdgaaaaafgcaabaaaabaaaaaafgagbaaa
aaaaaaaadcaaaaapdcaabaaaaaaaaaaaigaabaaaabaaaaaaaceaaaaaaaaaaedp
aaaaaemaaaaaaaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaeeaaaaaaaaaaaaaaaaa
eiaaaaalpcaabaaaaaaaaaaaegaabaaaaaaaaaaaeghobaaaabaaaaaaaagabaaa
acaaaaaaabeaaaaaaaaaaaaadcaaaaajbcaabaaaaaaaaaaadkaabaaaaaaaaaaa
abeaaaaaacibaamaabeaaaaaaaaafaeadcaaaaakbcaabaaaaaaaaaaabkiacaaa
aaaaaaaaagaaaaaaakaabaaaaaaaaaaabkaabaaaabaaaaaadgaaaaafccaabaaa
aaaaaaaaakbabaaaacaaaaaaeiaaaaalpcaabaaaabaaaaaabgafbaaaaaaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadgaaaaafdcaabaaa
acaaaaaaogbkbaaaadaaaaaadgaaaaafecaabaaaacaaaaaaabeaaaaaaaaaiadp
dgaaaaafbcaabaaaadaaaaaaabeaaaaaaaaaaaaadaaaaaabdbaaaaahicaabaaa
aaaaaaaabkaabaaaacaaaaaadkbabaaaaeaaaaaadbaaaaahbcaabaaaabaaaaaa
abeaaaaafnpofddpckaabaaaacaaaaaaabaaaaahicaabaaaaaaaaaaadkaabaaa
aaaaaaaaakaabaaaabaaaaaabiaaaaahbcaabaaaabaaaaaaakaabaaaadaaaaaa
abeaaaaaaaaaaaaaabaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaakaabaaa
abaaaaaaadaaaaaddkaabaaaaaaaaaaaeiaaaaalpcaabaaaadaaaaaaegaabaaa
acaaaaaabghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadcaaaaan
dcaabaaaacaaaaaaegiacaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaaaaea
aaaaaaaaaaaaaaaaegaabaaaacaaaaaadgaaaaafecaabaaaacaaaaaabkaabaaa
adaaaaaabgaaaaabdgaaaaafgcaabaaaadaaaaaafgagbaaaacaaaaaadcaaaaap
fcaabaaaabaaaaaaagacbaaaadaaaaaaaceaaaaaaaaaaedpaaaaaaaaaaaaaema
aaaaaaaaaceaaaaaaaaaaedpaaaaaaaaaaaaaeeaaaaaaaaaeiaaaaalpcaabaaa
acaaaaaaigaabaaaabaaaaaaeghobaaaabaaaaaaaagabaaaacaaaaaaabeaaaaa
aaaaaaaadcaaaaajicaabaaaaaaaaaaadkaabaaaacaaaaaaabeaaaaaacibaama
abeaaaaaaaaafaeadcaaaaalecaabaaaaaaaaaaabkiacaiaebaaaaaaaaaaaaaa
agaaaaaadkaabaaaaaaaaaaabkaabaaaadaaaaaadcaaaaaljcaabaaaaaaaaaaa
pgipcaaaaaaaaaaaagaaaaaaagaibaaaaaaaaaaapgbpbaiaebaaaaaaabaaaaaa
elaaaaagjcaabaaaaaaaaaaaagambaiaibaaaaaaaaaaaaaadcaaaaangcaabaaa
aaaaaaaaagibcaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaiadp
aaaaaaaafgagbaaaaaaaaaaaeiaaaaalpcaabaaaacaaaaaajgafbaaaaaaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadgaaaaafbcaabaaa
acaaaaaabkaabaaaabaaaaaadiaaaaakgcaabaaaaaaaaaaaagabbaaaacaaaaaa
aceaaaaaaaaaaaaaaaaaiaeaaaaaiaeaaaaaaaaaeaaaaaafgcaabaaaaaaaaaaa
fgagbaaaaaaaaaaadcaaaaamdcaabaaaaaaaaaaajgafbaaaaaaaaaaaaceaaaaa
aaaaiaebaaaaiaebaaaaaaaaaaaaaaaamgaabaaaaaaaaaaadcaaaaapdcaabaaa
aaaaaaaaegaabaaaaaaaaaaaaceaaaaamnmmmmdlkbaookdkaaaaaaaaaaaaaaaa
aceaaaaamnmmemdlkbaogkdkaaaaaaaaaaaaaaaaeiaaaaalpcaabaaaabaaaaaa
egaabaaaaaaaaaaaeghobaaaacaaaaaaaagabaaaabaaaaaaabeaaaaaaaaaaaaa
eiaaaaalpcaabaaaaaaaaaaaegaabaaaaaaaaaaaeghobaaaadaaaaaaaagabaaa
adaaaaaaabeaaaaaaaaaaaaadgaaaaafeccabaaaaaaaaaaadkaabaaaabaaaaaa
dgaaaaaficcabaaaaaaaaaaadkaabaaaaaaaaaaabcaaaaabdgaaaaaimccabaaa
aaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabfaaaaabdoaaaaab
""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 5 math
 //        d3d9 : 7 math
 //      opengl : 23 math, 6 texture, 7 branch
 // Stats for Fragment shader:
 //       d3d11 : 13 math, 3 branch
 //        d3d9 : 26 math, 12 texture, 2 branch
 Pass {
  Tags { ""RenderType""=""Opaque"" }
  ZTest Always
  ZWrite Off
  GpuProgramID 164660
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 23 math, 6 textures, 7 branches
""!!GLSL
#ifdef VERTEX

uniform vec4 SMAA_RT_METRICS;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD1 = ((SMAA_RT_METRICS.xyxy * vec4(1.0, 0.0, 0.0, 1.0)) + gl_MultiTexCoord0.xyxy);
}


#endif
#ifdef FRAGMENT
#extension GL_ARB_shader_texture_lod : enable
uniform sampler2D _MainTex;
uniform sampler2D _SrcTex;
uniform vec4 SMAA_RT_METRICS;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
void main ()
{
  vec4 tmpvar_1;
  vec4 a_2;
  a_2.x = texture2DLod (_MainTex, xlv_TEXCOORD1.xy, 0.0).w;
  a_2.y = texture2DLod (_MainTex, xlv_TEXCOORD1.zw, 0.0).y;
  a_2.zw = texture2DLod (_MainTex, xlv_TEXCOORD0, 0.0).zx;
  float tmpvar_3;
  tmpvar_3 = dot (a_2, vec4(1.0, 1.0, 1.0, 1.0));
  if ((tmpvar_3 < 1e-05)) {
    tmpvar_1 = texture2DLod (_SrcTex, xlv_TEXCOORD0, 0.0);
  } else {
    bool tmpvar_4;
    tmpvar_4 = (max (a_2.x, a_2.z) > max (a_2.y, a_2.w));
    vec4 tmpvar_5;
    tmpvar_5.xz = vec2(0.0, 0.0);
    tmpvar_5.yw = a_2.yw;
    vec2 tmpvar_6;
    tmpvar_6 = a_2.yw;
    vec4 tmpvar_7;
    tmpvar_7 = vec4(float(tmpvar_4));
    vec4 tmpvar_8;
    tmpvar_8.yw = vec2(0.0, 0.0);
    tmpvar_8.xz = a_2.xz;
    vec4 variable_9;
    variable_9 = tmpvar_5;
    vec2 variable_10;
    variable_10 = tmpvar_5.xy;
    vec2 value_11;
    value_11 = tmpvar_8.xy;
    if (bool(tmpvar_7.x)) {
      variable_10.x = value_11.x;
    };
    if (bool(tmpvar_7.y)) {
      variable_10.y = value_11.y;
    };
    variable_9.xy = variable_10;
    vec2 variable_12;
    variable_12 = variable_9.zw;
    vec2 value_13;
    value_13 = tmpvar_8.zw;
    if (bool(tmpvar_7.z)) {
      variable_12.x = value_13.x;
    };
    if (bool(tmpvar_7.w)) {
      variable_12.y = value_13.y;
    };
    variable_9.zw = variable_12;
    vec2 tmpvar_14;
    tmpvar_14 = vec2(float(tmpvar_4));
    vec2 variable_15;
    variable_15 = tmpvar_6;
    vec2 value_16;
    value_16 = a_2.xz;
    if (bool(tmpvar_14.x)) {
      variable_15.x = value_16.x;
    };
    if (bool(tmpvar_14.y)) {
      variable_15.y = value_16.y;
    };
    vec2 tmpvar_17;
    tmpvar_17 = (variable_15 / dot (variable_15, vec2(1.0, 1.0)));
    vec4 tmpvar_18;
    tmpvar_18.xy = SMAA_RT_METRICS.xy;
    tmpvar_18.zw = -(SMAA_RT_METRICS.xy);
    vec4 tmpvar_19;
    tmpvar_19 = ((variable_9 * tmpvar_18) + xlv_TEXCOORD0.xyxy);
    tmpvar_1 = ((tmpvar_17.x * texture2DLod (_SrcTex, tmpvar_19.xy, 0.0)) + (tmpvar_17.y * texture2DLod (_SrcTex, tmpvar_19.zw, 0.0)));
  };
  gl_FragData[0] = tmpvar_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 7 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [SMAA_RT_METRICS]
""vs_3_0
def c5, 1, 0, 0, 0
dcl_position v0
dcl_texcoord v1
dcl_position o0
dcl_texcoord o1.xy
dcl_texcoord1 o2
dp4 o0.x, c0, v0
dp4 o0.y, c1, v0
dp4 o0.z, c2, v0
dp4 o0.w, c3, v0
mov r0.xy, c4
mad o2, r0.xyxy, c5.xyyx, v1.xyxy
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 5 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 112
Vector 96 [SMAA_RT_METRICS]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefieceddpmcobahmnnpnogomphfkfpkbcdmhmmmabaaaaaagmacaaaaadaaaaaa
cmaaaaaakaaaaaaabaabaaaaejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaafjaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaahaaaaaagaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apadaaaafaepfdejfeejepeoaaeoepfcenebemaafeeffiedepepfceeaaklklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapaaaaaafdfgfpfagphdgjhe
gjgpgoaafeeffiedepepfceeaaklklklfdeieefcfeabaaaaeaaaabaaffaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaacaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaadpccabaaaacaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dgaaaaafdccabaaaabaaaaaaegbabaaaacaaaaaadcaaaaanpccabaaaacaaaaaa
egiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaiadpaaaaaaaaaaaaaaaaaaaaiadp
egbebaaaacaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 26 math, 12 textures, 2 branches
Vector 0 [SMAA_RT_METRICS]
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_SrcTex] 2D 1
""ps_3_0
def c1, 1, 0, 9.99999975e-006, -1
dcl_texcoord v0.xy
dcl_texcoord1 v1
dcl_2d s0
dcl_2d s1
mul r0, c1.xxyy, v1.xyxx
texldl r0, r0, s0
mul r1, c1.xxyy, v1.zwxx
texldl r1, r1, s0
mul r2, c1.xxyy, v0.xyxx
texldl r3, r2, s0
mov r1.x, r0.w
mov r1.zw, r3.xyzx
dp4 r0.x, r1, c1.x
if_lt r0.x, c1.z
texldl oC0, r2, s1
else
max r2.x, r0.w, r1.z
max r0.x, r1.y, r3.x
add r0.x, -r2.x, r0.x
cmp r2.x, r0.x, c1.y, r0.w
cmp r2.y, r0.x, r1.y, c1.y
cmp r2.z, r0.x, c1.y, r1.z
cmp r2.w, r0.x, r3.x, c1.y
cmp r1.x, r0.x, r1.y, r0.w
cmp r1.y, r0.x, r3.x, r1.z
dp2add r0.x, r1, c1.x, c1.y
rcp r0.x, r0.x
mul r0.xy, r0.x, r1
mov r1.xw, c1
mul r1, r1.xxww, c0.xyxy
mad r1, r2, r1, v0.xyxy
mul r2, r1.xyxx, c1.xxyy
texldl r2, r2, s1
mul r1, r1.zwxx, c1.xxyy
texldl r1, r1, s1
mul r1, r0.y, r1
mad oC0, r0.x, r2, r1
endif

""
}
SubProgram ""d3d11 "" {
// Stats: 13 math, 3 branches
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_SrcTex] 2D 1
ConstBuffer ""$Globals"" 112
Vector 96 [SMAA_RT_METRICS]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedladjnmfahanfdppbpndjdhijpignmlenabaaaaaamaaeaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcoiadaaaaeaaaaaaapkaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaad
aagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaa
abaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacadaaaaaaeiaaaaalpcaabaaaaaaaaaaa
egbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaa
eiaaaaalpcaabaaaabaaaaaaogbkbaaaacaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaaabeaaaaaaaaaaaaaeiaaaaalpcaabaaaacaaaaaaegbabaaaabaaaaaa
nghcbaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaadgaaaaafbcaabaaa
acaaaaaadkaabaaaaaaaaaaadgaaaaafccaabaaaacaaaaaabkaabaaaabaaaaaa
bbaaaaakbcaabaaaaaaaaaaaegaobaaaacaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaiadpaaaaiadpdbaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaaabeaaaaa
kmmfchdhbpaaaeadakaabaaaaaaaaaaaeiaaaaalpccabaaaaaaaaaaaegbabaaa
abaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaaabeaaaaaaaaaaaaadoaaaaab
bcaaaaabdeaaaaahbcaabaaaaaaaaaaadkaabaaaaaaaaaaackaabaaaacaaaaaa
deaaaaahccaabaaaaaaaaaaadkaabaaaacaaaaaabkaabaaaacaaaaaadbaaaaah
bcaabaaaaaaaaaaabkaabaaaaaaaaaaaakaabaaaaaaaaaaaabaaaaahbcaabaaa
abaaaaaadkaabaaaaaaaaaaaakaabaaaaaaaaaaaabaaaaahecaabaaaabaaaaaa
ckaabaaaacaaaaaaakaabaaaaaaaaaaadhaaaaamkcaabaaaabaaaaaaagaabaaa
aaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaafganbaaaacaaaaaa
dhaaaaajbcaabaaaacaaaaaaakaabaaaaaaaaaaadkaabaaaaaaaaaaabkaabaaa
acaaaaaadhaaaaajccaabaaaacaaaaaaakaabaaaaaaaaaaackaabaaaacaaaaaa
dkaabaaaacaaaaaaapaaaaakbcaabaaaaaaaaaaaegaabaaaacaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaaaaaaaaaaaaaaoaaaaahdcaabaaaaaaaaaaaegaabaaa
acaaaaaaagaabaaaaaaaaaaadiaaaaalpcaabaaaacaaaaaaegiecaaaaaaaaaaa
agaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaialpaaaaialpdcaaaaajpcaabaaa
abaaaaaaegaobaaaabaaaaaaegaobaaaacaaaaaaegbebaaaabaaaaaaeiaaaaal
pcaabaaaacaaaaaaegaabaaaabaaaaaaeghobaaaabaaaaaaaagabaaaabaaaaaa
abeaaaaaaaaaaaaaeiaaaaalpcaabaaaabaaaaaaogakbaaaabaaaaaaeghobaaa
abaaaaaaaagabaaaabaaaaaaabeaaaaaaaaaaaaadiaaaaahpcaabaaaabaaaaaa
fgafbaaaaaaaaaaaegaobaaaabaaaaaadcaaaaajpccabaaaaaaaaaaaagaabaaa
aaaaaaaaegaobaaaacaaaaaaegaobaaaabaaaaaadoaaaaabbfaaaaabdoaaaaab
""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 7 math
 //        d3d9 : 9 math
 //      opengl : 25 math, 8 texture, 1 branch
 // Stats for Fragment shader:
 //       d3d11 : 21 math
 //        d3d9 : 28 math, 15 texture
 Pass {
  Tags { ""RenderType""=""Opaque"" }
  ZTest Always
  ZWrite Off
  GpuProgramID 260186
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 25 math, 8 textures, 1 branches
""!!GLSL
#ifdef VERTEX

uniform vec4 SMAA_RT_METRICS;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
void main ()
{
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD1 = ((SMAA_RT_METRICS.xyxy * vec4(-1.0, 0.0, 0.0, -1.0)) + gl_MultiTexCoord0.xyxy);
  xlv_TEXCOORD2 = ((SMAA_RT_METRICS.xyxy * vec4(1.0, 0.0, 0.0, 1.0)) + gl_MultiTexCoord0.xyxy);
  xlv_TEXCOORD3 = ((SMAA_RT_METRICS.xyxy * vec4(-2.0, 0.0, 0.0, -2.0)) + gl_MultiTexCoord0.xyxy);
}


#endif
#ifdef FRAGMENT
#extension GL_ARB_shader_texture_lod : enable
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
void main ()
{
  vec4 delta_1;
  float tmpvar_2;
  tmpvar_2 = dot (texture2DLod (_MainTex, xlv_TEXCOORD0, 0.0).xyz, vec3(0.2126, 0.7152, 0.0722));
  float tmpvar_3;
  tmpvar_3 = dot (texture2DLod (_MainTex, xlv_TEXCOORD1.xy, 0.0).xyz, vec3(0.2126, 0.7152, 0.0722));
  float tmpvar_4;
  tmpvar_4 = dot (texture2DLod (_MainTex, xlv_TEXCOORD1.zw, 0.0).xyz, vec3(0.2126, 0.7152, 0.0722));
  vec2 tmpvar_5;
  tmpvar_5.x = tmpvar_3;
  tmpvar_5.y = tmpvar_4;
  delta_1.xy = abs((tmpvar_2 - tmpvar_5));
  vec2 tmpvar_6;
  tmpvar_6 = vec2(greaterThanEqual (delta_1.xy, vec2(0.05, 0.05)));
  float tmpvar_7;
  tmpvar_7 = dot (tmpvar_6, vec2(1.0, 1.0));
  if ((tmpvar_7 == 0.0)) {
    discard;
  };
  vec2 tmpvar_8;
  tmpvar_8.x = dot (texture2DLod (_MainTex, xlv_TEXCOORD2.xy, 0.0).xyz, vec3(0.2126, 0.7152, 0.0722));
  tmpvar_8.y = dot (texture2DLod (_MainTex, xlv_TEXCOORD2.zw, 0.0).xyz, vec3(0.2126, 0.7152, 0.0722));
  delta_1.zw = abs((tmpvar_2 - tmpvar_8));
  vec2 tmpvar_9;
  tmpvar_9 = max (delta_1.xy, delta_1.zw);
  vec2 tmpvar_10;
  tmpvar_10.x = tmpvar_3;
  tmpvar_10.y = tmpvar_4;
  vec2 tmpvar_11;
  tmpvar_11.x = dot (texture2DLod (_MainTex, xlv_TEXCOORD3.xy, 0.0).xyz, vec3(0.2126, 0.7152, 0.0722));
  tmpvar_11.y = dot (texture2DLod (_MainTex, xlv_TEXCOORD3.zw, 0.0).xyz, vec3(0.2126, 0.7152, 0.0722));
  delta_1.zw = abs((tmpvar_10 - tmpvar_11));
  vec2 tmpvar_12;
  tmpvar_12 = max (tmpvar_9, delta_1.zw);
  vec4 tmpvar_13;
  tmpvar_13.zw = vec2(0.0, 0.0);
  tmpvar_13.xy = (tmpvar_6 * vec2(greaterThanEqual (delta_1.xy, 
    (0.5 * max (tmpvar_12.xx, tmpvar_12.yy))
  )));
  gl_FragData[0] = tmpvar_13;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 9 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [SMAA_RT_METRICS]
""vs_3_0
def c5, -1, 0, 1, -2
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
mad o2, r0.xyxy, c5.xyyx, v1.xyxy
mad o3, r0.xyxy, c5.zyyz, v1.xyxy
mad o4, r0.xyxy, c5.wyyw, v1.xyxy
mov o1.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 7 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 112
Vector 96 [SMAA_RT_METRICS]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedoclbmealfinpkkojalhnigdfnockjhfaabaaaaaabmadaaaaadaaaaaa
cmaaaaaakaaaaaaaeaabaaaaejfdeheogmaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaafjaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaahaaaaaagaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apadaaaafaepfdejfeejepeoaaeoepfcenebemaafeeffiedepepfceeaaklklkl
epfdeheojiaaaaaaafaaaaaaaiaaaaaaiaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaimaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
imaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapaaaaaaimaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaapaaaaaaimaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaapaaaaaafdfgfpfagphdgjhegjgpgoaafeeffiedepepfceeaaklklkl
fdeieefcneabaaaaeaaaabaahfaaaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaa
fjaaaaaeegiocaaaabaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaad
dcbabaaaacaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaa
abaaaaaagfaaaaadpccabaaaacaaaaaagfaaaaadpccabaaaadaaaaaagfaaaaad
pccabaaaaeaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaa
aaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaa
aaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaa
dcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaa
egaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaacaaaaaadcaaaaan
pccabaaaacaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaialpaaaaaaaa
aaaaaaaaaaaaialpegbebaaaacaaaaaadcaaaaanpccabaaaadaaaaaaegiecaaa
aaaaaaaaagaaaaaaaceaaaaaaaaaiadpaaaaaaaaaaaaaaaaaaaaiadpegbebaaa
acaaaaaadcaaaaanpccabaaaaeaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaa
aaaaaamaaaaaaaaaaaaaaaaaaaaaaamaegbebaaaacaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 28 math, 15 textures
SetTexture 0 [_MainTex] 2D 0
""ps_3_0
def c0, 0.212599993, 0.715200007, 0.0722000003, 0
def c1, 1, 0, -0.0500000007, 0.5
dcl_texcoord v0.xy
dcl_texcoord1 v1
dcl_texcoord2 v2
dcl_texcoord3 v3
dcl_2d s0
mul r0, c1.xxyy, v0.xyxx
texldl r0, r0, s0
dp3 r0.x, r0, c0
mul r1, c1.xxyy, v1.xyxx
texldl r1, r1, s0
dp3 r1.x, r1, c0
mul r2, c1.xxyy, v1.zwxx
texldl r2, r2, s0
dp3 r1.y, r2, c0
add r0.yz, r0.x, -r1.xxyw
add r1.zw, r0_abs.xyyz, c1.z
cmp r1.zw, r1, c1.x, c1.y
dp2add r0.w, r1.zwzw, c1.x, c1.y
cmp r2, -r0.w, -c1.x, -c1.y
texkill r2
mul r2, c1.xxyy, v2.xyxx
texldl r2, r2, s0
dp3 r2.x, r2, c0
mul r3, c1.xxyy, v2.zwxx
texldl r3, r3, s0
dp3 r2.y, r3, c0
add r0.xw, r0.x, -r2.xyzy
max r2.xy, r0_abs.yzzw, r0_abs.xwzw
mul r3, c1.xxyy, v3.xyxx
texldl r3, r3, s0
dp3 r3.x, r3, c0
mul r4, c1.xxyy, v3.zwxx
texldl r4, r4, s0
dp3 r3.y, r4, c0
add r0.xw, r1.xyzy, -r3.xyzy
max r1.xy, r2, r0_abs.xwzw
max r0.x, r1.x, r1.y
mad r0.xy, r0.x, -c1.w, r0_abs.yzzw
cmp oC0.xy, r0, r1.zwzw, c1.y
mov oC0.zw, c1.y

""
}
SubProgram ""d3d11 "" {
// Stats: 21 math
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedpfppcmelhmfeacobbpgponbhjghgggefabaaaaaameafaaaaadaaaaaa
cmaaaaaammaaaaaaaaabaaaaejfdeheojiaaaaaaafaaaaaaaiaaaaaaiaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaimaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaimaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaaimaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaapapaaaaimaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapapaaaafdfgfpfagphdgjhegjgpgoaa
feeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl
fdeieefclmaeaaaaeaaaaaaacpabaaaafkaaaaadaagabaaaaaaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaadpcbabaaa
acaaaaaagcbaaaadpcbabaaaadaaaaaagcbaaaadpcbabaaaaeaaaaaagfaaaaad
pccabaaaaaaaaaaagiaaaaacaeaaaaaaeiaaaaalpcaabaaaaaaaaaaaegbabaaa
abaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaabaaaaaak
bcaabaaaaaaaaaaaegacbaaaaaaaaaaaaceaaaaanaldfjdofjbhdhdpjinnjddn
aaaaaaaaeiaaaaalpcaabaaaabaaaaaaegbabaaaacaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaabeaaaaaaaaaaaaabaaaaaakbcaabaaaabaaaaaaegacbaaa
abaaaaaaaceaaaaanaldfjdofjbhdhdpjinnjddnaaaaaaaaeiaaaaalpcaabaaa
acaaaaaaogbkbaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaa
aaaaaaaabaaaaaakccaabaaaabaaaaaaegacbaaaacaaaaaaaceaaaaanaldfjdo
fjbhdhdpjinnjddnaaaaaaaaaaaaaaaigcaabaaaaaaaaaaaagaabaaaaaaaaaaa
agabbaiaebaaaaaaabaaaaaabnaaaaalmcaabaaaabaaaaaafgajbaiaibaaaaaa
aaaaaaaaaceaaaaaaaaaaaaaaaaaaaaamnmmemdnmnmmemdnabaaaaakmcaabaaa
abaaaaaakgaobaaaabaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaiadpaaaaiadp
apaaaaakicaabaaaaaaaaaaaogakbaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadp
aaaaaaaaaaaaaaaabiaaaaahicaabaaaaaaaaaaadkaabaaaaaaaaaaaabeaaaaa
aaaaaaaaanaaaeaddkaabaaaaaaaaaaaeiaaaaalpcaabaaaacaaaaaaegbabaaa
adaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaabaaaaaak
bcaabaaaacaaaaaaegacbaaaacaaaaaaaceaaaaanaldfjdofjbhdhdpjinnjddn
aaaaaaaaeiaaaaalpcaabaaaadaaaaaaogbkbaaaadaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaaabeaaaaaaaaaaaaabaaaaaakccaabaaaacaaaaaaegacbaaa
adaaaaaaaceaaaaanaldfjdofjbhdhdpjinnjddnaaaaaaaaaaaaaaaijcaabaaa
aaaaaaaaagaabaaaaaaaaaaaagaebaiaebaaaaaaacaaaaaadeaaaaajjcaabaaa
aaaaaaaaagambaiaibaaaaaaaaaaaaaafgajbaiaibaaaaaaaaaaaaaaeiaaaaal
pcaabaaaacaaaaaaegbabaaaaeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
abeaaaaaaaaaaaaabaaaaaakbcaabaaaacaaaaaaegacbaaaacaaaaaaaceaaaaa
naldfjdofjbhdhdpjinnjddnaaaaaaaaeiaaaaalpcaabaaaadaaaaaaogbkbaaa
aeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaabeaaaaaaaaaaaaabaaaaaak
ccaabaaaacaaaaaaegacbaaaadaaaaaaaceaaaaanaldfjdofjbhdhdpjinnjddn
aaaaaaaaaaaaaaaidcaabaaaabaaaaaaegaabaaaabaaaaaaegaabaiaebaaaaaa
acaaaaaadeaaaaaijcaabaaaaaaaaaaaagambaaaaaaaaaaaagaebaiaibaaaaaa
abaaaaaadeaaaaahbcaabaaaaaaaaaaadkaabaaaaaaaaaaaakaabaaaaaaaaaaa
diaaaaahbcaabaaaaaaaaaaaakaabaaaaaaaaaaaabeaaaaaaaaaaadpbnaaaaai
dcaabaaaaaaaaaaajgafbaiaibaaaaaaaaaaaaaaagaabaaaaaaaaaaaabaaaaak
dcaabaaaaaaaaaaaegaabaaaaaaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaaaaa
aaaaaaaadiaaaaahdccabaaaaaaaaaaaegaabaaaaaaaaaaaogakbaaaabaaaaaa
dgaaaaaimccabaaaaaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
doaaaaab""
}
}
 }
}
Fallback ""Diffuse""
}";
	}
}

