using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PostProcessFX.Config;
using PostProcessFX.EffectMenu;

using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace PostProcessFX
{
    /**
     * Enables and configures the bloom and lensflare effect using the
     * already added Bloom class of Main Camera.
     */
    class BloomEffect : IEffectMenu
    {
        private Bloom m_bloomComponent = null;
        private LensflareEffect m_lensflare = null;

        private BloomConfig m_activeConfig; // The configuration that is being used.

        private static String configFilename = "PostProcessFX_bloom_config.xml";

        /**
         * Create a new bloom effect menu with or without an existing config.
         */
        public BloomEffect()
        {
            m_bloomComponent = Camera.main.GetComponent<Bloom>();

            m_lensflare = new LensflareEffect();

            m_activeConfig = ConfigUtility.Deserialize<BloomConfig>(configFilename);
            if (m_activeConfig == null)
            {
                m_activeConfig = BloomConfig.getDefaultPreset();
            }
            
            applyConfig();
        }

        public void save()
        {
            ConfigUtility.Serialize<BloomConfig>(configFilename, m_activeConfig);
        }

        public void onGUI(float x, float y)
        {
            if (GUI.Button(new Rect(x, y, 75, 20), "Low"))
            {
                m_activeConfig = BloomConfig.getLowPreset();
            }

            if (GUI.Button(new Rect(x + 75, y, 75, 20), "Medium"))
            {
                m_activeConfig = BloomConfig.getMediumPreset();
            }

            if (GUI.Button(new Rect(x + 150, y, 75, 20), "High"))
            {
                m_activeConfig = BloomConfig.getHighPreset();
            }

            if (GUI.Button(new Rect(x + 225, y, 75, 20), "Load"))
            {
                m_activeConfig = ConfigUtility.Deserialize<BloomConfig>(configFilename);
                if (m_activeConfig == null)
                {
                    m_activeConfig = BloomConfig.getDefaultPreset();
                }
            }

            y += 25;
            m_activeConfig.bloomEnabled = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.bloomEnabled, "enable bloom");
            y += 25;

            m_activeConfig.intensity = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 2.0f, "bloomIntensity", m_activeConfig.intensity);
            y += 25;

            m_activeConfig.threshhold = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 1.0f, "bloomThreshhold", m_activeConfig.threshhold);
            y += 25;

            m_activeConfig.blurSpread = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 5.0f, "bloomSpread", m_activeConfig.blurSpread);
            y += 25;

            m_activeConfig.blurIterations = PPFXUtility.drawIntSliderWithLabel(x, y, 1, 8, "bloomBlurIterations", m_activeConfig.blurIterations);
            y += 25;

            m_activeConfig.lensflareEnabled = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.lensflareEnabled, "enable lensflare.");
            y += 25;

            m_activeConfig.lensflareIntensity = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 2.0f, "lensflareIntensity", m_activeConfig.lensflareIntensity);
            y += 25;

            m_activeConfig.lensflareThreshhold = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 0.25f, "lensflareThreshhold", m_activeConfig.lensflareThreshhold);
            y += 25;

            m_activeConfig.lensflareSaturation = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 1.0f, "lensflareSaturation", m_activeConfig.lensflareSaturation);
            y += 25;

            m_activeConfig.lensflareRotation = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 3.14f, "lensflareRotation", m_activeConfig.lensflareRotation);
            y += 25;

            m_activeConfig.lensflareStretchWidth = PPFXUtility.drawSliderWithLabel(x, y, 0.0f, 2.0f, "lensflareWidth", m_activeConfig.lensflareStretchWidth);
            y += 25;

            m_activeConfig.lensflareBlurIterations = PPFXUtility.drawIntSliderWithLabel(x, y, 1, 8, "lensflareBlurIterations", m_activeConfig.lensflareBlurIterations);
            y += 25;

            m_activeConfig.lensflareGhosting = GUI.Toggle(new Rect(x, y, 200.0f, 20.0f), m_activeConfig.lensflareGhosting, "Enable ghosting lensflare.");
            y += 25;

            applyConfig();
        }

        private void applyConfig()
        {
            if (m_activeConfig.bloomEnabled)
            {
                enable();

                m_bloomComponent.bloomIntensity = m_activeConfig.intensity;
                m_bloomComponent.bloomBlurIterations = m_activeConfig.blurIterations;
                m_bloomComponent.bloomThreshold = m_activeConfig.threshhold;
                //m_bloomComponent.blurWidth = m_activeConfig.blurWidth;
                m_bloomComponent.sepBlurSpread = m_activeConfig.blurSpread;

                m_bloomComponent.flareRotation = m_activeConfig.lensflareRotation;
                m_bloomComponent.hollyStretchWidth = m_activeConfig.lensflareStretchWidth;
                m_bloomComponent.hollywoodFlareBlurIterations = m_activeConfig.lensflareBlurIterations;
                m_bloomComponent.lensflareIntensity = m_activeConfig.lensflareEnabled ? m_activeConfig.lensflareIntensity : 0.0f;
                m_bloomComponent.lensFlareSaturation = m_activeConfig.lensflareSaturation;
                m_bloomComponent.lensflareThreshold = m_activeConfig.lensflareThreshhold;
                m_bloomComponent.lensflareMode = m_activeConfig.lensflareGhosting ? Bloom.LensFlareStyle.Combined : Bloom.LensFlareStyle.Anamorphic;
            }
            else
            {
                disable();
            }

            if (m_activeConfig.lensflareSun)
            {
                m_lensflare.enable();
            }
            else
            {
                m_lensflare.disable();
            }
        }

        public void enable()
        {
            if (m_bloomComponent == null)
            {
                m_bloomComponent = Camera.main.gameObject.AddComponent<Bloom>();
                if (m_bloomComponent == null)
                {
                    PPFXUtility.log("BloomEffect: Could not add component CameraMotionBlur to Camera.");
                }
                else
                {
                    Material lensFlareCreate = new Material(strLensFlareCreate);
                    Material blendForBloom = new Material(strBlendForBloom);
                    Material blurAndFlares = new Material(strBlurAndFlares);
                    Material brightPass2 = new Material(strBrightPassFilter2);

                    m_bloomComponent.lensFlareShader = lensFlareCreate.shader;
                    m_bloomComponent.screenBlendShader = blendForBloom.shader;
                    m_bloomComponent.blurAndFlaresShader = blurAndFlares.shader;
                    m_bloomComponent.brightPassFilterShader = brightPass2.shader;
                }
            }

            m_activeConfig.bloomEnabled = true;
        }

        public void disable()
        {
            if (m_bloomComponent != null)
            {
                MonoBehaviour.DestroyImmediate(m_bloomComponent);
                m_bloomComponent = null;
            }

            m_activeConfig.bloomEnabled = false;
        }

        private const String strLensFlareCreate = @"// Compiled shader for custom platforms, uncompressed size: 5.7KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/LensFlareCreate"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = """" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 9 math
 //        d3d9 : 9 math
 //      opengl : 7 math, 4 texture
 // Stats for Fragment shader:
 //       d3d11 : 4 math, 4 texture
 //        d3d9 : 5 math, 4 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Blend One One
  GpuProgramID 49258
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 7 math, 4 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
void main ()
{
  vec2 cse_1;
  cse_1 = (gl_MultiTexCoord0.xy - 0.5);
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = ((cse_1 * -0.85) + 0.5);
  xlv_TEXCOORD0_1 = ((cse_1 * -1.45) + 0.5);
  xlv_TEXCOORD0_2 = ((cse_1 * -2.55) + 0.5);
  xlv_TEXCOORD0_3 = ((cse_1 * -4.15) + 0.5);
}


#endif
#ifdef FRAGMENT
uniform vec4 colorA;
uniform vec4 colorB;
uniform vec4 colorC;
uniform vec4 colorD;
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
void main ()
{
  gl_FragData[0] = (((
    (texture2D (_MainTex, xlv_TEXCOORD0) * colorA)
   + 
    (texture2D (_MainTex, xlv_TEXCOORD0_1) * colorB)
  ) + (texture2D (_MainTex, xlv_TEXCOORD0_2) * colorC)) + (texture2D (_MainTex, xlv_TEXCOORD0_3) * colorD));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 9 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
""vs_2_0
def c4, -0.5, -0.850000024, 0.5, -1.45000005
def c5, -2.54999995, 0.5, -4.1500001, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
add r0, v1.xyxy, c4.x
mad oT0.xy, r0.zwzw, c4.y, c4.z
mad oT1.xy, r0.zwzw, c4.w, c4.z
mad oT2.xy, r0, c5.x, c5.y
mad oT3.xy, r0.zwzw, c5.z, c5.y

""
}
SubProgram ""d3d11 "" {
// Stats: 9 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""UnityPerDraw"" 0
""vs_4_0
eefiecedigeagmaephhcpmnmbhmckppkdniilbblabaaaaaafeadaaaaadaaaaaa
cmaaaaaaiaaaaaaacaabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheojiaaaaaaafaaaaaaaiaaaaaaiaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaimaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
imaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaaimaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaadamaaaaimaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaadamaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklkl
fdeieefccmacaaaaeaaaabaailaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
gfaaaaaddccabaaaadaaaaaagfaaaaaddccabaaaaeaaaaaagiaaaaacabaaaaaa
diaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaaaaaaaaaabaaaaaa
dcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaaaaaaaaaaagbabaaaaaaaaaaa
egaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaaaaaaaaaacaaaaaa
kgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaa
aaaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaaaaaaaaakpcaabaaa
aaaaaaaaegbebaaaabaaaaaaaceaaaaaaaaaaalpaaaaaalpaaaaaalpaaaaaalp
dcaaaaapdccabaaaabaaaaaaogakbaaaaaaaaaaaaceaaaaajkjjfjlpjkjjfjlp
aaaaaaaaaaaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaadcaaaaap
dccabaaaacaaaaaaogakbaaaaaaaaaaaaceaaaaajkjjljlpjkjjljlpaaaaaaaa
aaaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaadcaaaaapdccabaaa
adaaaaaaegaabaaaaaaaaaaaaceaaaaaddddcdmaddddcdmaaaaaaaaaaaaaaaaa
aceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaadcaaaaapdccabaaaaeaaaaaa
ogakbaaaaaaaaaaaaceaaaaamnmmiemamnmmiemaaaaaaaaaaaaaaaaaaceaaaaa
aaaaaadpaaaaaadpaaaaaaaaaaaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 5 math, 4 textures
Vector 0 [colorA]
Vector 1 [colorB]
Vector 2 [colorC]
Vector 3 [colorD]
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
dcl t0.xy
dcl t1.xy
dcl t2.xy
dcl t3.xy
dcl_2d s0
texld r0, t1, s0
texld r1, t0, s0
texld r2, t2, s0
texld r3, t3, s0
mul r0, r0, c1
mad_pp r0, r1, c0, r0
mad_pp r0, r2, c2, r0
mad_pp r0, r3, c3, r0
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 4 math, 4 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 160
Vector 96 [colorA]
Vector 112 [colorB]
Vector 128 [colorC]
Vector 144 [colorD]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedhmihomiidekbjmihjfkfpgnackhkijgmabaaaaaakmacaaaaadaaaaaa
cmaaaaaammaaaaaaaaabaaaaejfdeheojiaaaaaaafaaaaaaaiaaaaaaiaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaimaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaimaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaaimaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaadadaaaaimaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaa
feeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklkl
fdeieefckeabaaaaeaaaaaaagjaaaaaafjaaaaaeegiocaaaaaaaaaaaakaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaaddcbabaaaadaaaaaa
gcbaaaaddcbabaaaaeaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaa
efaaaaajpcaabaaaaaaaaaaaegbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadiaaaaaipcaabaaaaaaaaaaaegaobaaaaaaaaaaaegiocaaaaaaaaaaa
ahaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegaobaaaabaaaaaaegiocaaa
aaaaaaaaagaaaaaaegaobaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
adaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaa
egaobaaaabaaaaaaegiocaaaaaaaaaaaaiaaaaaaegaobaaaaaaaaaaaefaaaaaj
pcaabaaaabaaaaaaegbabaaaaeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
dcaaaaakpccabaaaaaaaaaaaegaobaaaabaaaaaaegiocaaaaaaaaaaaajaaaaaa
egaobaaaaaaaaaaadoaaaaab""
}
}
 }
}
Fallback Off
}";

        private const String strBlendForBloom = @"// Compiled shader for custom platforms, uncompressed size: 48.1KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/BlendForBloom"" {
Properties {
 _MainTex (""Screen Blended"", 2D) = """" { }
 _ColorBuffer (""Color"", 2D) = """" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 5 math, 2 texture
 // Stats for Fragment shader:
 //       d3d11 : 3 math, 2 texture
 //        d3d9 : 5 math, 2 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 14601
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 5 math, 2 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD0_1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _ColorBuffer;
uniform sampler2D _MainTex;
uniform float _Intensity;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  gl_FragData[0] = (1.0 - ((1.0 - 
    (texture2D (_MainTex, xlv_TEXCOORD0) * _Intensity)
  ) * (1.0 - texture2D (_ColorBuffer, xlv_TEXCOORD0_1))));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ColorBuffer_TexelSize]
""vs_2_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad oT1.y, r0.x, r0.y, v1.y
mov oT0.xy, v1
mov oT1.x, v1.x

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_ColorBuffer_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedbkelefboiinbdbcalmldmmfkkpjmbofcabaaaaaajaacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcjiabaaaaeaaaabaaggaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaadbaaaaaibcaabaaaaaaaaaaa
bkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaa
bkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajcccabaaaacaaaaaa
akaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaafbccabaaa
acaaaaaaakbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 5 math, 2 textures
Float 0 [_Intensity]
SetTexture 0 [_ColorBuffer] 2D 0
SetTexture 1 [_MainTex] 2D 1
""ps_2_0
def c1, 1, 0, 0, 0
dcl t0.xy
dcl t1.xy
dcl_2d s0
dcl_2d s1
texld r0, t0, s1
texld_pp r1, t1, s0
mov r2.w, c1.x
mad_pp r0, r0, -c0.x, r2.w
add_pp r1, -r1, c1.x
mad_pp r0, r0, -r1, c1.x
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 3 math, 2 textures
SetTexture 0 [_MainTex] 2D 1
SetTexture 1 [_ColorBuffer] 2D 0
ConstBuffer ""$Globals"" 144
Float 96 [_Intensity]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedeofoonlfbnfjlifkmbecneecikekbjefabaaaaaadiacaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcgaabaaaaeaaaaaaafiaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaad
aagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaa
abaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaabaaaaaadcaaaaaopcaabaaa
aaaaaaaaegaobaiaebaaaaaaaaaaaaaaagiacaaaaaaaaaaaagaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaiadpaaaaiadpefaaaaajpcaabaaaabaaaaaaegbabaaa
acaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaaaaaaaalpcaabaaaabaaaaaa
egaobaiaebaaaaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadp
dcaaaaanpccabaaaaaaaaaaaegaobaiaebaaaaaaaaaaaaaaegaobaaaabaaaaaa
aceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpdoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 2 math, 2 texture
 // Stats for Fragment shader:
 //       d3d11 : 1 math, 2 texture
 //        d3d9 : 2 math, 2 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 128177
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 2 math, 2 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD0_1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _ColorBuffer;
uniform sampler2D _MainTex;
uniform float _Intensity;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  gl_FragData[0] = ((_Intensity * texture2D (_MainTex, xlv_TEXCOORD0)) + texture2D (_ColorBuffer, xlv_TEXCOORD0_1));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ColorBuffer_TexelSize]
""vs_2_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad oT1.y, r0.x, r0.y, v1.y
mov oT0.xy, v1
mov oT1.x, v1.x

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_ColorBuffer_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedbkelefboiinbdbcalmldmmfkkpjmbofcabaaaaaajaacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcjiabaaaaeaaaabaaggaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaadbaaaaaibcaabaaaaaaaaaaa
bkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaa
bkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajcccabaaaacaaaaaa
akaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaafbccabaaa
acaaaaaaakbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 2 math, 2 textures
Float 0 [_Intensity]
SetTexture 0 [_ColorBuffer] 2D 0
SetTexture 1 [_MainTex] 2D 1
""ps_2_0
dcl t0.xy
dcl t1.xy
dcl_2d s0
dcl_2d s1
texld_pp r0, t0, s1
texld_pp r1, t1, s0
mad_pp r0, c0.x, r0, r1
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 1 math, 2 textures
SetTexture 0 [_MainTex] 2D 1
SetTexture 1 [_ColorBuffer] 2D 0
ConstBuffer ""$Globals"" 144
Float 96 [_Intensity]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedakmkbejlmcigbbjcpohfdiefjpdgdbdmabaaaaaamiabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcpaaaaaaaeaaaaaaadmaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaad
aagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaa
abaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaabaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaacaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaagiacaaaaaaaaaaaagaaaaaaegaobaaaaaaaaaaaegaobaaa
abaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 8 math
 //        d3d9 : 10 math
 //      opengl : 4 math, 5 texture
 // Stats for Fragment shader:
 //       d3d11 : 4 math, 5 texture
 //        d3d9 : 5 math, 5 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 158525
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 4 math, 5 textures
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
  vec2 cse_1;
  cse_1 = (_MainTex_TexelSize.xy * 0.5);
  vec2 cse_2;
  cse_2 = (_MainTex_TexelSize.xy * vec2(1.0, -1.0));
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = (gl_MultiTexCoord0.xy + cse_1);
  xlv_TEXCOORD0_1 = (gl_MultiTexCoord0.xy - cse_1);
  xlv_TEXCOORD0_2 = (gl_MultiTexCoord0.xy - (cse_2 * 0.5));
  xlv_TEXCOORD0_3 = (gl_MultiTexCoord0.xy + (cse_2 * 0.5));
  xlv_TEXCOORD0_4 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
void main ()
{
  gl_FragData[0] = max (max (max (
    max (texture2D (_MainTex, xlv_TEXCOORD0_4), texture2D (_MainTex, xlv_TEXCOORD0))
  , texture2D (_MainTex, xlv_TEXCOORD0_1)), texture2D (_MainTex, xlv_TEXCOORD0_2)), texture2D (_MainTex, xlv_TEXCOORD0_3));
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
""vs_2_0
def c5, 0.5, -0.5, 0, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.xy, c4
mad oT0.xy, r0, c5.x, v1
mad oT1.xy, r0, -c5.x, v1
mad oT2.xy, r0, -c5, v1
mad oT3.xy, r0, c5, v1
mov oT4.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 8 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 128 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedfcocijekpmcclkndiialdcacicloeecgabaaaaaafmadaaaaadaaaaaa
cmaaaaaaiaaaaaaadiabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheolaaaaaaaagaaaaaaaiaaaaaajiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaakeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
keaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaakeaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaadamaaaakeaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaadamaaaakeaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefcbmacaaaa
eaaaabaaihaaaaaafjaaaaaeegiocaaaaaaaaaaaajaaaaaafjaaaaaeegiocaaa
abaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaa
ghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaad
dccabaaaacaaaaaagfaaaaaddccabaaaadaaaaaagfaaaaaddccabaaaaeaaaaaa
gfaaaaaddccabaaaafaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaandccabaaaabaaaaaaegiacaaaaaaaaaaa
aiaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaaegbabaaaabaaaaaa
dcaaaaaodccabaaaacaaaaaaegiacaiaebaaaaaaaaaaaaaaaiaaaaaaaceaaaaa
aaaaaadpaaaaaadpaaaaaaaaaaaaaaaaegbabaaaabaaaaaadcaaaaaodccabaaa
adaaaaaaegiacaiaebaaaaaaaaaaaaaaaiaaaaaaaceaaaaaaaaaaadpaaaaaalp
aaaaaaaaaaaaaaaaegbabaaaabaaaaaadcaaaaandccabaaaaeaaaaaaegiacaaa
aaaaaaaaaiaaaaaaaceaaaaaaaaaaadpaaaaaalpaaaaaaaaaaaaaaaaegbabaaa
abaaaaaadgaaaaafdccabaaaafaaaaaaegbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 5 math, 5 textures
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
dcl t0.xy
dcl t1.xy
dcl t2.xy
dcl t3.xy
dcl t4.xy
dcl_2d s0
texld_pp r0, t4, s0
texld_pp r1, t0, s0
texld_pp r2, t1, s0
texld_pp r3, t2, s0
texld_pp r4, t3, s0
max_pp r5, r0, r1
max_pp r0, r5, r2
max_pp r1, r0, r3
max_pp r0, r1, r4
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 4 math, 5 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedienngckhccmeaipjajkfhlnkfjnkgngeabaaaaaalmacaaaaadaaaaaa
cmaaaaaaoeaaaaaabiabaaaaejfdeheolaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaakeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaakeaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaakeaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaadadaaaakeaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaadadaaaakeaaaaaaaeaaaaaaaaaaaaaa
adaaaaaaafaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcjmabaaaa
eaaaaaaaghaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaa
ffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaad
dcbabaaaadaaaaaagcbaaaaddcbabaaaaeaaaaaagcbaaaaddcbabaaaafaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadeaaaaah
pcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadeaaaaah
pcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaadaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadeaaaaah
pcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaaeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadeaaaaah
pccabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 1 math, 2 texture
 // Stats for Fragment shader:
 //       d3d11 : 1 math, 2 texture
 //        d3d9 : 2 math, 2 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 230541
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 1 math, 2 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD0_1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _ColorBuffer;
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  gl_FragData[0] = (texture2D (_MainTex, xlv_TEXCOORD0) * texture2D (_ColorBuffer, xlv_TEXCOORD0));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ColorBuffer_TexelSize]
""vs_2_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad oT1.y, r0.x, r0.y, v1.y
mov oT0.xy, v1
mov oT1.x, v1.x

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_ColorBuffer_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedbkelefboiinbdbcalmldmmfkkpjmbofcabaaaaaajaacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcjiabaaaaeaaaabaaggaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaadbaaaaaibcaabaaaaaaaaaaa
bkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaa
bkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajcccabaaaacaaaaaa
akaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaafbccabaaa
acaaaaaaakbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 2 math, 2 textures
SetTexture 0 [_ColorBuffer] 2D 0
SetTexture 1 [_MainTex] 2D 1
""ps_2_0
dcl t0.xy
dcl_2d s0
dcl_2d s1
texld r0, t0, s1
texld r1, t0, s0
mul_pp r0, r0, r1
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 1 math, 2 textures
SetTexture 0 [_MainTex] 2D 1
SetTexture 1 [_ColorBuffer] 2D 0
""ps_4_0
eefiecedjciifcnpddgfkikonaggbffgchijoadgabaaaaaakaabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcmiaaaaaaeaaaaaaadcaaaaaa
fkaaaaadaagabaaaaaaaaaaafkaaaaadaagabaaaabaaaaaafibiaaaeaahabaaa
aaaaaaaaffffaaaafibiaaaeaahabaaaabaaaaaaffffaaaagcbaaaaddcbabaaa
abaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaa
aaaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaabaaaaaaefaaaaaj
pcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaa
diaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaadoaaaaab
""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 5 math, 2 texture
 // Stats for Fragment shader:
 //       d3d11 : 3 math, 2 texture
 //        d3d9 : 5 math, 2 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 287605
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 5 math, 2 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD0_1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _ColorBuffer;
uniform sampler2D _MainTex;
uniform float _Intensity;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  gl_FragData[0] = (1.0 - ((1.0 - 
    (texture2D (_MainTex, xlv_TEXCOORD0) * _Intensity)
  ) * (1.0 - texture2D (_ColorBuffer, xlv_TEXCOORD0_1))));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ColorBuffer_TexelSize]
""vs_2_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad oT1.y, r0.x, r0.y, v1.y
mov oT0.xy, v1
mov oT1.x, v1.x

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_ColorBuffer_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedbkelefboiinbdbcalmldmmfkkpjmbofcabaaaaaajaacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcjiabaaaaeaaaabaaggaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaadbaaaaaibcaabaaaaaaaaaaa
bkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaa
bkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajcccabaaaacaaaaaa
akaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaafbccabaaa
acaaaaaaakbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 5 math, 2 textures
Float 0 [_Intensity]
SetTexture 0 [_ColorBuffer] 2D 0
SetTexture 1 [_MainTex] 2D 1
""ps_2_0
def c1, 1, 0, 0, 0
dcl t0.xy
dcl t1.xy
dcl_2d s0
dcl_2d s1
texld r0, t0, s1
texld_pp r1, t1, s0
mov r2.w, c1.x
mad_pp r0, r0, -c0.x, r2.w
add_pp r1, -r1, c1.x
mad_pp r0, r0, -r1, c1.x
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 3 math, 2 textures
SetTexture 0 [_MainTex] 2D 1
SetTexture 1 [_ColorBuffer] 2D 0
ConstBuffer ""$Globals"" 144
Float 96 [_Intensity]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedeofoonlfbnfjlifkmbecneecikekbjefabaaaaaadiacaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcgaabaaaaeaaaaaaafiaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaad
aagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaa
abaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaabaaaaaadcaaaaaopcaabaaa
aaaaaaaaegaobaiaebaaaaaaaaaaaaaaagiacaaaaaaaaaaaagaaaaaaaceaaaaa
aaaaiadpaaaaiadpaaaaiadpaaaaiadpefaaaaajpcaabaaaabaaaaaaegbabaaa
acaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaaaaaaaaalpcaabaaaabaaaaaa
egaobaiaebaaaaaaabaaaaaaaceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadp
dcaaaaanpccabaaaaaaaaaaaegaobaiaebaaaaaaaaaaaaaaegaobaaaabaaaaaa
aceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaiadpdoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 2 math, 2 texture
 // Stats for Fragment shader:
 //       d3d11 : 1 math, 2 texture
 //        d3d9 : 2 math, 2 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 379964
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 2 math, 2 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD0_1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _ColorBuffer;
uniform sampler2D _MainTex;
uniform float _Intensity;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  gl_FragData[0] = ((_Intensity * texture2D (_MainTex, xlv_TEXCOORD0)) + texture2D (_ColorBuffer, xlv_TEXCOORD0_1));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ColorBuffer_TexelSize]
""vs_2_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad oT1.y, r0.x, r0.y, v1.y
mov oT0.xy, v1
mov oT1.x, v1.x

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_ColorBuffer_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedbkelefboiinbdbcalmldmmfkkpjmbofcabaaaaaajaacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcjiabaaaaeaaaabaaggaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaadbaaaaaibcaabaaaaaaaaaaa
bkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaa
bkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajcccabaaaacaaaaaa
akaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaafbccabaaa
acaaaaaaakbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 2 math, 2 textures
Float 0 [_Intensity]
SetTexture 0 [_ColorBuffer] 2D 0
SetTexture 1 [_MainTex] 2D 1
""ps_2_0
dcl t0.xy
dcl t1.xy
dcl_2d s0
dcl_2d s1
texld_pp r0, t0, s1
texld_pp r1, t1, s0
mad_pp r0, c0.x, r0, r1
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 1 math, 2 textures
SetTexture 0 [_MainTex] 2D 1
SetTexture 1 [_ColorBuffer] 2D 0
ConstBuffer ""$Globals"" 144
Float 96 [_Intensity]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedakmkbejlmcigbbjcpohfdiefjpdgdbdmabaaaaaamiabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcpaaaaaaaeaaaaaaadmaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaaaaaaaaaafkaaaaad
aagabaaaabaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaafibiaaaeaahabaaa
abaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaabaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaacaaaaaaeghobaaaabaaaaaaaagabaaaaaaaaaaadcaaaaak
pccabaaaaaaaaaaaagiacaaaaaaaaaaaagaaaaaaegaobaaaaaaaaaaaegaobaaa
abaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 8 math
 //        d3d9 : 10 math
 //      opengl : 4 math, 4 texture
 // Stats for Fragment shader:
 //       d3d11 : 4 math, 4 texture
 //        d3d9 : 5 math, 4 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 450581
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 4 math, 4 textures
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
  vec2 cse_1;
  cse_1 = (_MainTex_TexelSize.xy * 0.5);
  vec2 cse_2;
  cse_2 = (_MainTex_TexelSize.xy * vec2(1.0, -1.0));
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = (gl_MultiTexCoord0.xy + cse_1);
  xlv_TEXCOORD0_1 = (gl_MultiTexCoord0.xy - cse_1);
  xlv_TEXCOORD0_2 = (gl_MultiTexCoord0.xy - (cse_2 * 0.5));
  xlv_TEXCOORD0_3 = (gl_MultiTexCoord0.xy + (cse_2 * 0.5));
  xlv_TEXCOORD0_4 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
void main ()
{
  gl_FragData[0] = (((
    (texture2D (_MainTex, xlv_TEXCOORD0) + texture2D (_MainTex, xlv_TEXCOORD0_1))
   + texture2D (_MainTex, xlv_TEXCOORD0_2)) + texture2D (_MainTex, xlv_TEXCOORD0_3)) / 4.0);
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
""vs_2_0
def c5, 0.5, -0.5, 0, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.xy, c4
mad oT0.xy, r0, c5.x, v1
mad oT1.xy, r0, -c5.x, v1
mad oT2.xy, r0, -c5, v1
mad oT3.xy, r0, c5, v1
mov oT4.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 8 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 128 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedfcocijekpmcclkndiialdcacicloeecgabaaaaaafmadaaaaadaaaaaa
cmaaaaaaiaaaaaaadiabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheolaaaaaaaagaaaaaaaiaaaaaajiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaakeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
keaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaakeaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaadamaaaakeaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaadamaaaakeaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaaadamaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefcbmacaaaa
eaaaabaaihaaaaaafjaaaaaeegiocaaaaaaaaaaaajaaaaaafjaaaaaeegiocaaa
abaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaa
ghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaad
dccabaaaacaaaaaagfaaaaaddccabaaaadaaaaaagfaaaaaddccabaaaaeaaaaaa
gfaaaaaddccabaaaafaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaandccabaaaabaaaaaaegiacaaaaaaaaaaa
aiaaaaaaaceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaaegbabaaaabaaaaaa
dcaaaaaodccabaaaacaaaaaaegiacaiaebaaaaaaaaaaaaaaaiaaaaaaaceaaaaa
aaaaaadpaaaaaadpaaaaaaaaaaaaaaaaegbabaaaabaaaaaadcaaaaaodccabaaa
adaaaaaaegiacaiaebaaaaaaaaaaaaaaaiaaaaaaaceaaaaaaaaaaadpaaaaaalp
aaaaaaaaaaaaaaaaegbabaaaabaaaaaadcaaaaandccabaaaaeaaaaaaegiacaaa
aaaaaaaaaiaaaaaaaceaaaaaaaaaaadpaaaaaalpaaaaaaaaaaaaaaaaegbabaaa
abaaaaaadgaaaaafdccabaaaafaaaaaaegbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 5 math, 4 textures
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
def c0, 0.25, 0, 0, 0
dcl t0.xy
dcl t1.xy
dcl t2.xy
dcl t3.xy
dcl_2d s0
texld_pp r0, t0, s0
texld r1, t1, s0
texld r2, t2, s0
texld r3, t3, s0
add_pp r0, r0, r1
add_pp r0, r2, r0
add_pp r0, r3, r0
mul_pp r0, r0, c0.x
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 4 math, 4 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedopphidddlojmdgiamdoggknhdfcedmmoabaaaaaajiacaaaaadaaaaaa
cmaaaaaaoeaaaaaabiabaaaaejfdeheolaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaakeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaakeaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaakeaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaadadaaaakeaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaadadaaaakeaaaaaaaeaaaaaaaaaaaaaa
adaaaaaaafaaaaaaadaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefchiabaaaa
eaaaaaaafoaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaa
ffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaad
dcbabaaaadaaaaaagcbaaaaddcbabaaaaeaaaaaagfaaaaadpccabaaaaaaaaaaa
giaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaacaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaaegaobaaa
aaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaadaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaaegaobaaa
aaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaaeaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaaegaobaaa
aaaaaaaaegaobaaaabaaaaaadiaaaaakpccabaaaaaaaaaaaegaobaaaaaaaaaaa
aceaaaaaaaaaiadoaaaaiadoaaaaiadoaaaaiadodoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 1 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 0 math, 1 texture
 //        d3d9 : 3 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Blend Zero SrcAlpha
  GpuProgramID 469504
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 1 math, 1 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD0_1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _ColorBuffer;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.xyz = vec3(1.0, 1.0, 1.0);
  tmpvar_1.w = texture2D (_ColorBuffer, xlv_TEXCOORD0).x;
  gl_FragData[0] = tmpvar_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ColorBuffer_TexelSize]
""vs_2_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad oT1.y, r0.x, r0.y, v1.y
mov oT0.xy, v1
mov oT1.x, v1.x

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_ColorBuffer_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedbkelefboiinbdbcalmldmmfkkpjmbofcabaaaaaajaacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcjiabaaaaeaaaabaaggaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaadbaaaaaibcaabaaaaaaaaaaa
bkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaa
bkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajcccabaaaacaaaaaa
akaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaafbccabaaa
acaaaaaaakbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 3 math, 1 textures
SetTexture 0 [_ColorBuffer] 2D 0
""ps_2_0
def c0, 1, 0, 0, 0
dcl t0.xy
dcl_2d s0
texld_pp r0, t0, s0
mov_pp r0.w, r0.x
mov_pp r0.xyz, c0.x
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 0 math, 1 textures
SetTexture 0 [_ColorBuffer] 2D 0
""ps_4_0
eefiecedafdhamafecnhiolhijiimkigflllafebabaaaaaahiabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefckaaaaaaaeaaaaaaaciaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacabaaaaaaefaaaaaj
pcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
dgaaaaaficcabaaaaaaaaaaaakaabaaaaaaaaaaadgaaaaaihccabaaaaaaaaaaa
aceaaaaaaaaaiadpaaaaiadpaaaaiadpaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 1 math
 // Stats for Fragment shader:
 //        d3d9 : 2 math
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 588023
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 1 math
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD0_1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
void main ()
{
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ColorBuffer_TexelSize]
""vs_2_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad oT1.y, r0.x, r0.y, v1.y
mov oT0.xy, v1
mov oT1.x, v1.x

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_ColorBuffer_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedbkelefboiinbdbcalmldmmfkkpjmbofcabaaaaaajaacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcjiabaaaaeaaaabaaggaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaadbaaaaaibcaabaaaaaaaaaaa
bkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaa
bkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajcccabaaaacaaaaaa
akaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaafbccabaaa
acaaaaaaakbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 2 math
""ps_2_0
def c0, 0, 0, 0, 0
mov_pp r0, c0.x
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
""ps_4_0
eefiecedffnadndlblnlpchmplaanbncdlbjfmmlabaaaaaabaabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadaaaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcdiaaaaaaeaaaaaaaaoaaaaaa
gfaaaaadpccabaaaaaaaaaaadgaaaaaipccabaaaaaaaaaaaaceaaaaaaaaaaaaa
aaaaaaaaaaaaaaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 1 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 1 math, 1 texture
 //        d3d9 : 2 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Blend One One
  GpuProgramID 619948
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 1 math, 1 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD0_1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform float _Intensity;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  gl_FragData[0] = (texture2D (_MainTex, xlv_TEXCOORD0) * _Intensity);
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ColorBuffer_TexelSize]
""vs_2_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad oT1.y, r0.x, r0.y, v1.y
mov oT0.xy, v1
mov oT1.x, v1.x

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_ColorBuffer_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedbkelefboiinbdbcalmldmmfkkpjmbofcabaaaaaajaacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcjiabaaaaeaaaabaaggaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaadbaaaaaibcaabaaaaaaaaaaa
bkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaa
bkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajcccabaaaacaaaaaa
akaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaafbccabaaa
acaaaaaaakbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 2 math, 1 textures
Float 0 [_Intensity]
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
dcl t0.xy
dcl_2d s0
texld_pp r0, t0, s0
mul_pp r0, r0, c0.x
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 1 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 144
Float 96 [_Intensity]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedcgfbjobdhjkcjohahbjjaglohjhomjmbabaaaaaaheabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcjmaaaaaaeaaaaaaachaaaaaa
fjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaae
aahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaagfaaaaadpccabaaa
aaaaaaaagiaaaaacabaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaaipccabaaaaaaaaaaaegaobaaa
aaaaaaaaagiacaaaaaaaaaaaagaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 6 math
 //        d3d9 : 10 math
 //      opengl : 0 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 0 math, 1 texture
 //        d3d9 : 1 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Blend One One
 BlendOp Max
  GpuProgramID 657499
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 0 math, 1 textures
""!!GLSL
#ifdef VERTEX

varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
void main ()
{
  vec2 tmpvar_1;
  tmpvar_1 = gl_MultiTexCoord0.xy;
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD0_1 = tmpvar_1;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  gl_FragData[0] = texture2D (_MainTex, xlv_TEXCOORD0);
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_ColorBuffer_TexelSize]
""vs_2_0
def c5, 0, -2, 1, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.x, c5.x
slt r0.x, c4.y, r0.x
mad r0.y, v1.y, c5.y, c5.z
mad oT1.y, r0.x, r0.y, v1.y
mov oT0.xy, v1
mov oT1.x, v1.x

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 144
Vector 112 [_ColorBuffer_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedbkelefboiinbdbcalmldmmfkkpjmbofcabaaaaaajaacaaaaadaaaaaa
cmaaaaaaiaaaaaaapaaaaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
fmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaafdfgfpfaepfdejfe
ejepeoaafeeffiedepepfceeaaklklklfdeieefcjiabaaaaeaaaabaaggaaaaaa
fjaaaaaeegiocaaaaaaaaaaaaiaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaa
fpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaa
aaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaa
giaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaa
abaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaa
agbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaa
abaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaa
aaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaa
dgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaadbaaaaaibcaabaaaaaaaaaaa
bkiacaaaaaaaaaaaahaaaaaaabeaaaaaaaaaaaaaaaaaaaaiccaabaaaaaaaaaaa
bkbabaiaebaaaaaaabaaaaaaabeaaaaaaaaaiadpdhaaaaajcccabaaaacaaaaaa
akaabaaaaaaaaaaabkaabaaaaaaaaaaabkbabaaaabaaaaaadgaaaaafbccabaaa
acaaaaaaakbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 1 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
dcl t0.xy
dcl_2d s0
texld_pp r0, t0, s0
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 0 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedlggkdcacocldgjlekgibdpfooohmeepcabaaaaaadmabaaaaadaaaaaa
cmaaaaaajmaaaaaanaaaaaaaejfdeheogiaaaaaaadaaaaaaaiaaaaaafaaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaafmaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafmaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adaaaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheo
cmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaa
apaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcgeaaaaaaeaaaaaaabjaaaaaa
fkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaad
dcbabaaaabaaaaaagfaaaaadpccabaaaaaaaaaaaefaaaaajpccabaaaaaaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadoaaaaab""
}
}
 }
}
Fallback Off
}";

        private const String strBrightPassFilter2 = @"// Compiled shader for custom platforms, uncompressed size: 7.1KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/BrightPassFilter2"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = """" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 2 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 2 math, 1 texture
 //        d3d9 : 3 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 7772
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 2 math, 1 textures
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
uniform vec4 _Threshhold;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 color_1;
  vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  color_1.w = tmpvar_2.w;
  color_1.xyz = max (vec3(0.0, 0.0, 0.0), (tmpvar_2.xyz - _Threshhold.xxx));
  gl_FragData[0] = color_1;
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
// Stats: 3 math, 1 textures
Vector 0 [_Threshhold]
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
def c1, 0, 0, 0, 0
dcl t0.xy
dcl_2d s0
texld_pp r0, t0, s0
add_pp r1.xyz, r0, -c0.x
max_pp r0.xyz, r1, c1.x
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 2 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 112
Vector 96 [_Threshhold]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedfmflllhkapnejdgmhdofcboihhembingabaaaaaajmabaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcnmaaaaaa
eaaaaaaadhaaaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacabaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaajhcaabaaa
aaaaaaaaegacbaaaaaaaaaaaagiacaiaebaaaaaaaaaaaaaaagaaaaaadgaaaaaf
iccabaaaaaaaaaaadkaabaaaaaaaaaaadeaaaaakhccabaaaaaaaaaaaegacbaaa
aaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 2 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 2 math, 1 texture
 //        d3d9 : 3 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 115521
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 2 math, 1 textures
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
uniform vec4 _Threshhold;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 color_1;
  vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
  color_1.w = tmpvar_2.w;
  color_1.xyz = max (vec3(0.0, 0.0, 0.0), (tmpvar_2.xyz - _Threshhold.xyz));
  gl_FragData[0] = color_1;
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
// Stats: 3 math, 1 textures
Vector 0 [_Threshhold]
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
def c1, 0, 0, 0, 0
dcl t0.xy
dcl_2d s0
texld_pp r0, t0, s0
add_pp r1.xyz, r0, -c0
max_pp r0.xyz, r1, c1.x
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 2 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 112
Vector 96 [_Threshhold]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedcphkpagdjbkkkmhfmjhajdlcjaddhnfkabaaaaaajmabaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcnmaaaaaa
eaaaaaaadhaaaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacabaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaajhcaabaaa
aaaaaaaaegacbaaaaaaaaaaaegiccaiaebaaaaaaaaaaaaaaagaaaaaadgaaaaaf
iccabaaaaaaaaaaadkaabaaaaaaaaaaadeaaaaakhccabaaaaaaaaaaaegacbaaa
aaaaaaaaaceaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadoaaaaab""
}
}
 }
}
Fallback Off
}";

        private const String strBlurAndFlares = @"// Compiled shader for custom platforms, uncompressed size: 36.4KB

// Skipping shader variants that would not be included into build of current scene.

Shader ""Hidden/BlurAndFlares"" {
Properties {
 _MainTex (""Base (RGB)"", 2D) = """" { }
 _NonBlurredTex (""Base (RGB)"", 2D) = """" { }
}
SubShader { 


 // Stats for Vertex shader:
 //       d3d11 : 4 math
 //        d3d9 : 5 math
 //      opengl : 11 math, 1 texture
 // Stats for Fragment shader:
 //       d3d11 : 9 math, 1 texture
 //        d3d9 : 13 math, 1 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 12069
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 11 math, 1 textures
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
uniform vec4 unity_ColorSpaceLuminance;
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = texture2D (_MainTex, xlv_TEXCOORD0);
  vec3 tmpvar_2;
  tmpvar_2 = (tmpvar_1.xyz * unity_ColorSpaceLuminance.xyz);
  gl_FragData[0] = (tmpvar_1 / (1.5 + (
    ((tmpvar_2.x + tmpvar_2.y) + tmpvar_2.z)
   + 
    ((2.0 * sqrt((tmpvar_2.y * 
      (tmpvar_2.x + tmpvar_2.z)
    ))) * unity_ColorSpaceLuminance.w)
  )));
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
// Stats: 13 math, 1 textures
Vector 0 [unity_ColorSpaceLuminance]
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
def c1, 2, 1.5, 0, 0
dcl_pp t0.xy
dcl_2d s0
texld_pp r0, t0, s0
mul_pp r1.xyz, r0, c0
add_pp r1.z, r1.z, r1.x
mul_pp r1.z, r1.z, r1.y
add_pp r1.x, r1.y, r1.x
mad_pp r1.x, r0.z, c0.z, r1.x
rsq_pp r1.y, r1.z
rcp_pp r1.y, r1.y
mul_pp r1.y, r1.y, c0.w
mad_pp r1.x, r1.y, c1.x, r1.x
add_pp r1.x, r1.x, c1.y
rcp r1.x, r1.x
mul_pp r0, r0, r1.x
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 9 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 160
Vector 48 [unity_ColorSpaceLuminance]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedcjgkmbicbjhgbnckghgfmafccbaaiopaabaaaaaaeeacaaaaadaaaaaa
cmaaaaaaieaaaaaaliaaaaaaejfdeheofaaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaeeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcieabaaaa
eaaaaaaagbaaaaaafjaaaaaeegiocaaaaaaaaaaaaeaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaaihcaabaaa
abaaaaaaegacbaaaaaaaaaaaegiccaaaaaaaaaaaadaaaaaaaaaaaaahfcaabaaa
abaaaaaafgagbaaaabaaaaaaagaabaaaabaaaaaadiaaaaahccaabaaaabaaaaaa
ckaabaaaabaaaaaabkaabaaaabaaaaaadcaaaaakbcaabaaaabaaaaaackaabaaa
aaaaaaaackiacaaaaaaaaaaaadaaaaaaakaabaaaabaaaaaaelaaaaafccaabaaa
abaaaaaabkaabaaaabaaaaaaapaaaaaiccaabaaaabaaaaaapgipcaaaaaaaaaaa
adaaaaaafgafbaaaabaaaaaaaaaaaaahbcaabaaaabaaaaaabkaabaaaabaaaaaa
akaabaaaabaaaaaaaaaaaaahbcaabaaaabaaaaaaakaabaaaabaaaaaaabeaaaaa
aaaamadpaoaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaaagaabaaaabaaaaaa
doaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 12 math
 //        d3d9 : 14 math
 //      opengl : 6 math, 7 texture
 // Stats for Fragment shader:
 //       d3d11 : 6 math, 7 texture
 //        d3d9 : 7 math, 7 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 67396
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 6 math, 7 textures
""!!GLSL
#ifdef VERTEX

uniform vec4 _Offsets;
uniform float _StretchWidth;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
varying vec2 xlv_TEXCOORD0_5;
varying vec2 xlv_TEXCOORD0_6;
void main ()
{
  float cse_1;
  cse_1 = (_StretchWidth * 2.0);
  float cse_2;
  cse_2 = (_StretchWidth * 4.0);
  float cse_3;
  cse_3 = (_StretchWidth * 6.0);
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD0_1 = (gl_MultiTexCoord0.xy + (cse_1 * _Offsets.xy));
  xlv_TEXCOORD0_2 = (gl_MultiTexCoord0.xy - (cse_1 * _Offsets.xy));
  xlv_TEXCOORD0_3 = (gl_MultiTexCoord0.xy + (cse_2 * _Offsets.xy));
  xlv_TEXCOORD0_4 = (gl_MultiTexCoord0.xy - (cse_2 * _Offsets.xy));
  xlv_TEXCOORD0_5 = (gl_MultiTexCoord0.xy + (cse_3 * _Offsets.xy));
  xlv_TEXCOORD0_6 = (gl_MultiTexCoord0.xy - (cse_3 * _Offsets.xy));
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
varying vec2 xlv_TEXCOORD0_5;
varying vec2 xlv_TEXCOORD0_6;
void main ()
{
  gl_FragData[0] = max (max (max (
    max (max (max (texture2D (_MainTex, xlv_TEXCOORD0), texture2D (_MainTex, xlv_TEXCOORD0_1)), texture2D (_MainTex, xlv_TEXCOORD0_2)), texture2D (_MainTex, xlv_TEXCOORD0_3))
  , texture2D (_MainTex, xlv_TEXCOORD0_4)), texture2D (_MainTex, xlv_TEXCOORD0_5)), texture2D (_MainTex, xlv_TEXCOORD0_6));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 14 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_Offsets]
Float 5 [_StretchWidth]
""vs_2_0
def c6, 4, 6, 0, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
add r0.x, c5.x, c5.x
mad oT1.xy, r0.x, c4, v1
mad oT2.xy, r0.x, -c4, v1
mov r0.x, c5.x
mul r0, r0.x, c6.xxyy
mad oT3.xy, r0, c4, v1
mad oT4.xy, r0, -c4, v1
mad oT5.xy, r0.zwzw, c4, v1
mad oT6.xy, r0.zwzw, -c4, v1
mov oT0.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 12 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 160
Vector 96 [_Offsets]
Float 128 [_StretchWidth]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedojcempkglghjchdhdinfkmldkkajoincabaaaaaabiaeaaaaadaaaaaa
cmaaaaaaiaaaaaaagiabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheooaaaaaaaaiaaaaaaaiaaaaaamiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaneaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
neaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaaneaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaadamaaaaneaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaadamaaaaneaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaaadamaaaa
neaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaaadamaaaaneaaaaaaagaaaaaa
aaaaaaaaadaaaaaaahaaaaaaadamaaaafdfgfpfaepfdejfeejepeoaafeeffied
epepfceeaaklklklfdeieefckiacaaaaeaaaabaakkaaaaaafjaaaaaeegiocaaa
aaaaaaaaajaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaagfaaaaaddccabaaa
adaaaaaagfaaaaaddccabaaaaeaaaaaagfaaaaaddccabaaaafaaaaaagfaaaaad
dccabaaaagaaaaaagfaaaaaddccabaaaahaaaaaagiaaaaacabaaaaaadiaaaaai
pcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaa
adaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaa
egbabaaaabaaaaaaaaaaaaajbcaabaaaaaaaaaaaakiacaaaaaaaaaaaaiaaaaaa
akiacaaaaaaaaaaaaiaaaaaadcaaaaakdccabaaaacaaaaaaagaabaaaaaaaaaaa
egiacaaaaaaaaaaaagaaaaaaegbabaaaabaaaaaadcaaaaaldccabaaaadaaaaaa
agaabaiaebaaaaaaaaaaaaaaegiacaaaaaaaaaaaagaaaaaaegbabaaaabaaaaaa
diaaaaalpcaabaaaaaaaaaaaagiacaaaaaaaaaaaaiaaaaaaaceaaaaaaaaaiaea
aaaaiaeaaaaamaeaaaaamaeadcaaaaakdccabaaaaeaaaaaaegaabaaaaaaaaaaa
egiacaaaaaaaaaaaagaaaaaaegbabaaaabaaaaaadcaaaaaldccabaaaafaaaaaa
egaabaiaebaaaaaaaaaaaaaaegiacaaaaaaaaaaaagaaaaaaegbabaaaabaaaaaa
dcaaaaakdccabaaaagaaaaaaogakbaaaaaaaaaaaegiacaaaaaaaaaaaagaaaaaa
egbabaaaabaaaaaadcaaaaaldccabaaaahaaaaaaogakbaiaebaaaaaaaaaaaaaa
egiacaaaaaaaaaaaagaaaaaaegbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 7 math, 7 textures
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
dcl_pp t0.xy
dcl_pp t1.xy
dcl_pp t2.xy
dcl_pp t3.xy
dcl_pp t4.xy
dcl_pp t5.xy
dcl_pp t6.xy
dcl_2d s0
texld_pp r0, t0, s0
texld_pp r1, t1, s0
texld_pp r2, t2, s0
texld_pp r3, t3, s0
texld_pp r4, t4, s0
texld_pp r5, t5, s0
texld_pp r6, t6, s0
max_pp r7, r0, r1
max_pp r0, r7, r2
max_pp r1, r0, r3
max_pp r0, r1, r4
max_pp r1, r0, r5
max_pp r0, r1, r6
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 6 math, 7 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedofkpmcojhfehfiopackahfoecfjjhdgpabaaaaaaieadaaaaadaaaaaa
cmaaaaaabeabaaaaeiabaaaaejfdeheooaaaaaaaaiaaaaaaaiaaaaaamiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaneaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaneaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaaneaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaadadaaaaneaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaadadaaaaneaaaaaaaeaaaaaaaaaaaaaa
adaaaaaaafaaaaaaadadaaaaneaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaa
adadaaaaneaaaaaaagaaaaaaaaaaaaaaadaaaaaaahaaaaaaadadaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaa
aiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfe
gbhcghgfheaaklklfdeieefcdeacaaaaeaaaaaaainaaaaaafkaaaaadaagabaaa
aaaaaaaafibiaaaeaahabaaaaaaaaaaaffffaaaagcbaaaaddcbabaaaabaaaaaa
gcbaaaaddcbabaaaacaaaaaagcbaaaaddcbabaaaadaaaaaagcbaaaaddcbabaaa
aeaaaaaagcbaaaaddcbabaaaafaaaaaagcbaaaaddcbabaaaagaaaaaagcbaaaad
dcbabaaaahaaaaaagfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaaj
pcaabaaaaaaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
efaaaaajpcaabaaaabaaaaaaegbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadeaaaaahpcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaa
efaaaaajpcaabaaaabaaaaaaegbabaaaadaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadeaaaaahpcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaa
efaaaaajpcaabaaaabaaaaaaegbabaaaaeaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadeaaaaahpcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaa
efaaaaajpcaabaaaabaaaaaaegbabaaaafaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadeaaaaahpcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaa
efaaaaajpcaabaaaabaaaaaaegbabaaaagaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadeaaaaahpcaabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaa
efaaaaajpcaabaaaabaaaaaaegbabaaaahaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadeaaaaahpccabaaaaaaaaaaaegaobaaaaaaaaaaaegaobaaaabaaaaaa
doaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 11 math
 //        d3d9 : 13 math
 //      opengl : 20 math, 7 texture
 // Stats for Fragment shader:
 //       d3d11 : 18 math, 7 texture
 //        d3d9 : 22 math, 7 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 136774
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 20 math, 7 textures
""!!GLSL
#ifdef VERTEX

uniform vec4 _Offsets;
uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
varying vec2 xlv_TEXCOORD0_5;
varying vec2 xlv_TEXCOORD0_6;
void main ()
{
  vec2 cse_1;
  cse_1 = (0.5 * _MainTex_TexelSize.xy);
  vec2 cse_2;
  cse_2 = (1.5 * _MainTex_TexelSize.xy);
  vec2 cse_3;
  cse_3 = (2.5 * _MainTex_TexelSize.xy);
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD0_1 = (gl_MultiTexCoord0.xy + (cse_1 * _Offsets.xy));
  xlv_TEXCOORD0_2 = (gl_MultiTexCoord0.xy - (cse_1 * _Offsets.xy));
  xlv_TEXCOORD0_3 = (gl_MultiTexCoord0.xy + (cse_2 * _Offsets.xy));
  xlv_TEXCOORD0_4 = (gl_MultiTexCoord0.xy - (cse_2 * _Offsets.xy));
  xlv_TEXCOORD0_5 = (gl_MultiTexCoord0.xy + (cse_3 * _Offsets.xy));
  xlv_TEXCOORD0_6 = (gl_MultiTexCoord0.xy - (cse_3 * _Offsets.xy));
}


#endif
#ifdef FRAGMENT
uniform vec4 unity_ColorSpaceLuminance;
uniform vec4 _TintColor;
uniform vec2 _Threshhold;
uniform float _Saturation;
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
varying vec2 xlv_TEXCOORD0_5;
varying vec2 xlv_TEXCOORD0_6;
void main ()
{
  vec4 color_1;
  vec4 tmpvar_2;
  tmpvar_2 = max (((
    ((((
      ((texture2D (_MainTex, xlv_TEXCOORD0) + texture2D (_MainTex, xlv_TEXCOORD0_1)) + texture2D (_MainTex, xlv_TEXCOORD0_2))
     + texture2D (_MainTex, xlv_TEXCOORD0_3)) + texture2D (_MainTex, xlv_TEXCOORD0_4)) + texture2D (_MainTex, xlv_TEXCOORD0_5)) + texture2D (_MainTex, xlv_TEXCOORD0_6))
   / 7.0) - _Threshhold.xxxx), vec4(0.0, 0.0, 0.0, 0.0));
  color_1.w = tmpvar_2.w;
  vec3 tmpvar_3;
  tmpvar_3 = (tmpvar_2.xyz * unity_ColorSpaceLuminance.xyz);
  color_1.xyz = (mix (vec3((
    ((tmpvar_3.x + tmpvar_3.y) + tmpvar_3.z)
   + 
    ((2.0 * sqrt((tmpvar_3.y * 
      (tmpvar_3.x + tmpvar_3.z)
    ))) * unity_ColorSpaceLuminance.w)
  )), tmpvar_2.xyz, vec3(_Saturation)) * _TintColor.xyz);
  gl_FragData[0] = color_1;
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 13 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 5 [_MainTex_TexelSize]
Vector 4 [_Offsets]
""vs_2_0
def c6, 0.5, 1.5, 2.5, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.xy, c4
mul r0.xy, r0, c5
mad oT1.xy, r0, c6.x, v1
mad oT2.xy, r0, -c6.x, v1
mad oT3.xy, r0, c6.y, v1
mad oT4.xy, r0, -c6.y, v1
mad oT5.xy, r0, c6.z, v1
mad oT6.xy, r0, -c6.z, v1
mov oT0.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 11 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 160
Vector 96 [_Offsets]
Vector 144 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecednmfoedijbcffahcaaajnkhggoddeifnlabaaaaaabmaeaaaaadaaaaaa
cmaaaaaaiaaaaaaagiabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheooaaaaaaaaiaaaaaaaiaaaaaamiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaneaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
neaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaaneaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaadamaaaaneaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaadamaaaaneaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaaadamaaaa
neaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaaadamaaaaneaaaaaaagaaaaaa
aaaaaaaaadaaaaaaahaaaaaaadamaaaafdfgfpfaepfdejfeejepeoaafeeffied
epepfceeaaklklklfdeieefckmacaaaaeaaaabaaklaaaaaafjaaaaaeegiocaaa
aaaaaaaaakaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaagfaaaaaddccabaaa
adaaaaaagfaaaaaddccabaaaaeaaaaaagfaaaaaddccabaaaafaaaaaagfaaaaad
dccabaaaagaaaaaagfaaaaaddccabaaaahaaaaaagiaaaaacabaaaaaadiaaaaai
pcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaa
adaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaa
egbabaaaabaaaaaadiaaaaajdcaabaaaaaaaaaaaegiacaaaaaaaaaaaagaaaaaa
egiacaaaaaaaaaaaajaaaaaadcaaaaamdccabaaaacaaaaaaegaabaaaaaaaaaaa
aceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaaegbabaaaabaaaaaadcaaaaan
dccabaaaadaaaaaaegaabaiaebaaaaaaaaaaaaaaaceaaaaaaaaaaadpaaaaaadp
aaaaaaaaaaaaaaaaegbabaaaabaaaaaadcaaaaamdccabaaaaeaaaaaaegaabaaa
aaaaaaaaaceaaaaaaaaamadpaaaamadpaaaaaaaaaaaaaaaaegbabaaaabaaaaaa
dcaaaaandccabaaaafaaaaaaegaabaiaebaaaaaaaaaaaaaaaceaaaaaaaaamadp
aaaamadpaaaaaaaaaaaaaaaaegbabaaaabaaaaaadcaaaaamdccabaaaagaaaaaa
egaabaaaaaaaaaaaaceaaaaaaaaacaeaaaaacaeaaaaaaaaaaaaaaaaaegbabaaa
abaaaaaadcaaaaandccabaaaahaaaaaaegaabaiaebaaaaaaaaaaaaaaaceaaaaa
aaaacaeaaaaacaeaaaaaaaaaaaaaaaaaegbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 22 math, 7 textures
Float 3 [_Saturation]
Vector 2 [_Threshhold]
Vector 1 [_TintColor]
Vector 0 [unity_ColorSpaceLuminance]
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
def c4, 0.142857149, 0, 2, 0
dcl_pp t0.xy
dcl_pp t1.xy
dcl_pp t2.xy
dcl_pp t3.xy
dcl_pp t4.xy
dcl_pp t5.xy
dcl_pp t6.xy
dcl_2d s0
texld_pp r0, t0, s0
texld r1, t1, s0
texld r2, t2, s0
texld r3, t3, s0
texld r4, t4, s0
texld r5, t5, s0
texld r6, t6, s0
add_pp r0, r0, r1
add_pp r0, r2, r0
add_pp r0, r3, r0
add_pp r0, r4, r0
add_pp r0, r5, r0
add_pp r0, r6, r0
mov r1.x, c4.x
mad_pp r0, r0, r1.x, -c2.x
max_pp r1, r0, c4.y
mul_pp r0.xyz, r1, c0
add_pp r0.z, r0.z, r0.x
mul_pp r0.z, r0.z, r0.y
add_pp r0.x, r0.y, r0.x
mad_pp r0.x, r1.z, c0.z, r0.x
rsq_pp r0.y, r0.z
rcp_pp r0.y, r0.y
mul_pp r0.y, r0.y, c0.w
mad_pp r0.x, r0.y, c4.z, r0.x
lrp_pp r2.xyz, c3.x, r1, r0.x
mul_pp r1.xyz, r2, c1
mov_pp oC0, r1

""
}
SubProgram ""d3d11 "" {
// Stats: 18 math, 7 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 160
Vector 48 [unity_ColorSpaceLuminance]
Vector 112 [_TintColor]
Vector 132 [_Threshhold] 2
Float 140 [_Saturation]
BindCB  ""$Globals"" 0
""ps_4_0
eefieceddajcjfcoedcclcjbolpaihnedeifocdoabaaaaaaeaafaaaaadaaaaaa
cmaaaaaabeabaaaaeiabaaaaejfdeheooaaaaaaaaiaaaaaaaiaaaaaamiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaneaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaneaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaaneaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaadadaaaaneaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaadadaaaaneaaaaaaaeaaaaaaaaaaaaaa
adaaaaaaafaaaaaaadadaaaaneaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaa
adadaaaaneaaaaaaagaaaaaaaaaaaaaaadaaaaaaahaaaaaaadadaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaa
aiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfe
gbhcghgfheaaklklfdeieefcpaadaaaaeaaaaaaapmaaaaaafjaaaaaeegiocaaa
aaaaaaaaajaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaa
ffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaad
dcbabaaaadaaaaaagcbaaaaddcbabaaaaeaaaaaagcbaaaaddcbabaaaafaaaaaa
gcbaaaaddcbabaaaagaaaaaagcbaaaaddcbabaaaahaaaaaagfaaaaadpccabaaa
aaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
acaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
adaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
aeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
afaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
agaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
ahaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaadcaaaaaopcaabaaaaaaaaaaaegaobaaa
aaaaaaaaaceaaaaacfejbcdocfejbcdocfejbcdocfejbcdofgifcaiaebaaaaaa
aaaaaaaaaiaaaaaadeaaaaakpcaabaaaaaaaaaaaegaobaaaaaaaaaaaaceaaaaa
aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaadiaaaaaihcaabaaaabaaaaaaegacbaaa
aaaaaaaaegiccaaaaaaaaaaaadaaaaaaaaaaaaahfcaabaaaabaaaaaafgagbaaa
abaaaaaaagaabaaaabaaaaaadiaaaaahccaabaaaabaaaaaackaabaaaabaaaaaa
bkaabaaaabaaaaaadcaaaaakbcaabaaaabaaaaaackaabaaaaaaaaaaackiacaaa
aaaaaaaaadaaaaaaakaabaaaabaaaaaaelaaaaafccaabaaaabaaaaaabkaabaaa
abaaaaaaapaaaaaiccaabaaaabaaaaaapgipcaaaaaaaaaaaadaaaaaafgafbaaa
abaaaaaaaaaaaaahbcaabaaaabaaaaaabkaabaaaabaaaaaaakaabaaaabaaaaaa
aaaaaaaihcaabaaaaaaaaaaaegacbaaaaaaaaaaaagaabaiaebaaaaaaabaaaaaa
dcaaaaakhcaabaaaaaaaaaaapgipcaaaaaaaaaaaaiaaaaaaegacbaaaaaaaaaaa
agaabaaaabaaaaaadiaaaaaihccabaaaaaaaaaaaegacbaaaaaaaaaaaegiccaaa
aaaaaaaaahaaaaaadgaaaaaficcabaaaaaaaaaaadkaabaaaaaaaaaaadoaaaaab
""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 11 math
 //        d3d9 : 13 math
 //      opengl : 17 math, 7 texture
 // Stats for Fragment shader:
 //       d3d11 : 15 math, 7 texture
 //        d3d9 : 19 math, 7 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 207948
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 17 math, 7 textures
""!!GLSL
#ifdef VERTEX

uniform vec4 _Offsets;
uniform vec4 _MainTex_TexelSize;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
varying vec2 xlv_TEXCOORD0_5;
varying vec2 xlv_TEXCOORD0_6;
void main ()
{
  vec2 cse_1;
  cse_1 = (0.5 * _MainTex_TexelSize.xy);
  vec2 cse_2;
  cse_2 = (1.5 * _MainTex_TexelSize.xy);
  vec2 cse_3;
  cse_3 = (2.5 * _MainTex_TexelSize.xy);
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD0_1 = (gl_MultiTexCoord0.xy + (cse_1 * _Offsets.xy));
  xlv_TEXCOORD0_2 = (gl_MultiTexCoord0.xy - (cse_1 * _Offsets.xy));
  xlv_TEXCOORD0_3 = (gl_MultiTexCoord0.xy + (cse_2 * _Offsets.xy));
  xlv_TEXCOORD0_4 = (gl_MultiTexCoord0.xy - (cse_2 * _Offsets.xy));
  xlv_TEXCOORD0_5 = (gl_MultiTexCoord0.xy + (cse_3 * _Offsets.xy));
  xlv_TEXCOORD0_6 = (gl_MultiTexCoord0.xy - (cse_3 * _Offsets.xy));
}


#endif
#ifdef FRAGMENT
uniform vec4 unity_ColorSpaceLuminance;
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
varying vec2 xlv_TEXCOORD0_1;
varying vec2 xlv_TEXCOORD0_2;
varying vec2 xlv_TEXCOORD0_3;
varying vec2 xlv_TEXCOORD0_4;
varying vec2 xlv_TEXCOORD0_5;
varying vec2 xlv_TEXCOORD0_6;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1 = (((
    (((texture2D (_MainTex, xlv_TEXCOORD0) + texture2D (_MainTex, xlv_TEXCOORD0_1)) + texture2D (_MainTex, xlv_TEXCOORD0_2)) + texture2D (_MainTex, xlv_TEXCOORD0_3))
   + texture2D (_MainTex, xlv_TEXCOORD0_4)) + texture2D (_MainTex, xlv_TEXCOORD0_5)) + texture2D (_MainTex, xlv_TEXCOORD0_6));
  vec3 tmpvar_2;
  tmpvar_2 = (tmpvar_1.xyz * unity_ColorSpaceLuminance.xyz);
  gl_FragData[0] = (tmpvar_1 / (7.5 + (
    ((tmpvar_2.x + tmpvar_2.y) + tmpvar_2.z)
   + 
    ((2.0 * sqrt((tmpvar_2.y * 
      (tmpvar_2.x + tmpvar_2.z)
    ))) * unity_ColorSpaceLuminance.w)
  )));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 13 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 5 [_MainTex_TexelSize]
Vector 4 [_Offsets]
""vs_2_0
def c6, 0.5, 1.5, 2.5, 0
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.xy, c4
mul r0.xy, r0, c5
mad oT1.xy, r0, c6.x, v1
mad oT2.xy, r0, -c6.x, v1
mad oT3.xy, r0, c6.y, v1
mad oT4.xy, r0, -c6.y, v1
mad oT5.xy, r0, c6.z, v1
mad oT6.xy, r0, -c6.z, v1
mov oT0.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 11 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 160
Vector 96 [_Offsets]
Vector 144 [_MainTex_TexelSize]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecednmfoedijbcffahcaaajnkhggoddeifnlabaaaaaabmaeaaaaadaaaaaa
cmaaaaaaiaaaaaaagiabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheooaaaaaaaaiaaaaaaaiaaaaaamiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaaneaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
neaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaadamaaaaneaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaadamaaaaneaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaadamaaaaneaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaaadamaaaa
neaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaaadamaaaaneaaaaaaagaaaaaa
aaaaaaaaadaaaaaaahaaaaaaadamaaaafdfgfpfaepfdejfeejepeoaafeeffied
epepfceeaaklklklfdeieefckmacaaaaeaaaabaaklaaaaaafjaaaaaeegiocaaa
aaaaaaaaakaaaaaafjaaaaaeegiocaaaabaaaaaaaeaaaaaafpaaaaadpcbabaaa
aaaaaaaafpaaaaaddcbabaaaabaaaaaaghaaaaaepccabaaaaaaaaaaaabaaaaaa
gfaaaaaddccabaaaabaaaaaagfaaaaaddccabaaaacaaaaaagfaaaaaddccabaaa
adaaaaaagfaaaaaddccabaaaaeaaaaaagfaaaaaddccabaaaafaaaaaagfaaaaad
dccabaaaagaaaaaagfaaaaaddccabaaaahaaaaaagiaaaaacabaaaaaadiaaaaai
pcaabaaaaaaaaaaafgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaa
aaaaaaaaegaobaaaaaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaa
adaaaaaapgbpbaaaaaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaa
egbabaaaabaaaaaadiaaaaajdcaabaaaaaaaaaaaegiacaaaaaaaaaaaagaaaaaa
egiacaaaaaaaaaaaajaaaaaadcaaaaamdccabaaaacaaaaaaegaabaaaaaaaaaaa
aceaaaaaaaaaaadpaaaaaadpaaaaaaaaaaaaaaaaegbabaaaabaaaaaadcaaaaan
dccabaaaadaaaaaaegaabaiaebaaaaaaaaaaaaaaaceaaaaaaaaaaadpaaaaaadp
aaaaaaaaaaaaaaaaegbabaaaabaaaaaadcaaaaamdccabaaaaeaaaaaaegaabaaa
aaaaaaaaaceaaaaaaaaamadpaaaamadpaaaaaaaaaaaaaaaaegbabaaaabaaaaaa
dcaaaaandccabaaaafaaaaaaegaabaiaebaaaaaaaaaaaaaaaceaaaaaaaaamadp
aaaamadpaaaaaaaaaaaaaaaaegbabaaaabaaaaaadcaaaaamdccabaaaagaaaaaa
egaabaaaaaaaaaaaaceaaaaaaaaacaeaaaaacaeaaaaaaaaaaaaaaaaaegbabaaa
abaaaaaadcaaaaandccabaaaahaaaaaaegaabaiaebaaaaaaaaaaaaaaaceaaaaa
aaaacaeaaaaacaeaaaaaaaaaaaaaaaaaegbabaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 19 math, 7 textures
Vector 0 [unity_ColorSpaceLuminance]
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
def c1, 2, 7.5, 0, 0
dcl_pp t0.xy
dcl_pp t1.xy
dcl_pp t2.xy
dcl_pp t3.xy
dcl_pp t4.xy
dcl_pp t5.xy
dcl_pp t6.xy
dcl_2d s0
texld_pp r0, t0, s0
texld r1, t1, s0
texld r2, t2, s0
texld r3, t3, s0
texld r4, t4, s0
texld r5, t5, s0
texld r6, t6, s0
add_pp r0, r0, r1
add_pp r0, r2, r0
add_pp r0, r3, r0
add_pp r0, r4, r0
add_pp r0, r5, r0
add_pp r0, r6, r0
mul_pp r1.xyz, r0, c0
add_pp r1.z, r1.z, r1.x
mul_pp r1.z, r1.z, r1.y
add_pp r1.x, r1.y, r1.x
mad_pp r1.x, r0.z, c0.z, r1.x
rsq_pp r1.y, r1.z
rcp_pp r1.y, r1.y
mul_pp r1.y, r1.y, c0.w
mad_pp r1.x, r1.y, c1.x, r1.x
add_pp r1.x, r1.x, c1.y
rcp r1.x, r1.x
mul_pp r0, r0, r1.x
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 15 math, 7 textures
SetTexture 0 [_MainTex] 2D 0
ConstBuffer ""$Globals"" 160
Vector 48 [unity_ColorSpaceLuminance]
BindCB  ""$Globals"" 0
""ps_4_0
eefiecedhliefohhlnkdpflkbokkgefepfpgdiiaabaaaaaajmaeaaaaadaaaaaa
cmaaaaaabeabaaaaeiabaaaaejfdeheooaaaaaaaaiaaaaaaaiaaaaaamiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaaneaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaaneaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
adadaaaaneaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaadadaaaaneaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaadadaaaaneaaaaaaaeaaaaaaaaaaaaaa
adaaaaaaafaaaaaaadadaaaaneaaaaaaafaaaaaaaaaaaaaaadaaaaaaagaaaaaa
adadaaaaneaaaaaaagaaaaaaaaaaaaaaadaaaaaaahaaaaaaadadaaaafdfgfpfa
epfdejfeejepeoaafeeffiedepepfceeaaklklklepfdeheocmaaaaaaabaaaaaa
aiaaaaaacaaaaaaaaaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapaaaaaafdfgfpfe
gbhcghgfheaaklklfdeieefcemadaaaaeaaaaaaandaaaaaafjaaaaaeegiocaaa
aaaaaaaaaeaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaa
ffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaaddcbabaaaacaaaaaagcbaaaad
dcbabaaaadaaaaaagcbaaaaddcbabaaaaeaaaaaagcbaaaaddcbabaaaafaaaaaa
gcbaaaaddcbabaaaagaaaaaagcbaaaaddcbabaaaahaaaaaagfaaaaadpccabaaa
aaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaaegbabaaaabaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
acaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
adaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
aeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
afaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
agaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
ahaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaaaaaaaaahpcaabaaaaaaaaaaa
egaobaaaaaaaaaaaegaobaaaabaaaaaadiaaaaaihcaabaaaabaaaaaaegacbaaa
aaaaaaaaegiccaaaaaaaaaaaadaaaaaaaaaaaaahfcaabaaaabaaaaaafgagbaaa
abaaaaaaagaabaaaabaaaaaadiaaaaahccaabaaaabaaaaaackaabaaaabaaaaaa
bkaabaaaabaaaaaadcaaaaakbcaabaaaabaaaaaackaabaaaaaaaaaaackiacaaa
aaaaaaaaadaaaaaaakaabaaaabaaaaaaelaaaaafccaabaaaabaaaaaabkaabaaa
abaaaaaaapaaaaaiccaabaaaabaaaaaapgipcaaaaaaaaaaaadaaaaaafgafbaaa
abaaaaaaaaaaaaahbcaabaaaabaaaaaabkaabaaaabaaaaaaakaabaaaabaaaaaa
aaaaaaahbcaabaaaabaaaaaaakaabaaaabaaaaaaabeaaaaaaaaapaeaaoaaaaah
pccabaaaaaaaaaaaegaobaaaaaaaaaaaagaabaaaabaaaaaadoaaaaab""
}
}
 }


 // Stats for Vertex shader:
 //       d3d11 : 8 math
 //        d3d9 : 10 math
 //      opengl : 17 math, 9 texture
 // Stats for Fragment shader:
 //       d3d11 : 9 math, 9 texture
 //        d3d9 : 18 math, 9 texture
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  GpuProgramID 292851
Program ""vp"" {
SubProgram ""opengl "" {
// Stats: 17 math, 9 textures
""!!GLSL
#ifdef VERTEX

uniform vec4 _Offsets;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
varying vec4 xlv_TEXCOORD4;
void main ()
{
  vec4 cse_1;
  cse_1 = (_Offsets.xyxy * vec4(1.0, 1.0, -1.0, -1.0));
  gl_Position = (gl_ModelViewProjectionMatrix * gl_Vertex);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
  xlv_TEXCOORD1 = (gl_MultiTexCoord0.xyxy + cse_1);
  xlv_TEXCOORD2 = (gl_MultiTexCoord0.xyxy + (cse_1 * 2.0));
  xlv_TEXCOORD3 = (gl_MultiTexCoord0.xyxy + (cse_1 * 3.0));
  xlv_TEXCOORD4 = (gl_MultiTexCoord0.xyxy + (cse_1 * 5.0));
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec2 xlv_TEXCOORD0;
varying vec4 xlv_TEXCOORD1;
varying vec4 xlv_TEXCOORD2;
varying vec4 xlv_TEXCOORD3;
varying vec4 xlv_TEXCOORD4;
void main ()
{
  gl_FragData[0] = (((
    ((((
      ((0.225 * texture2D (_MainTex, xlv_TEXCOORD0)) + (0.15 * texture2D (_MainTex, xlv_TEXCOORD1.xy)))
     + 
      (0.15 * texture2D (_MainTex, xlv_TEXCOORD1.zw))
    ) + (0.11 * texture2D (_MainTex, xlv_TEXCOORD2.xy))) + (0.11 * texture2D (_MainTex, xlv_TEXCOORD2.zw))) + (0.075 * texture2D (_MainTex, xlv_TEXCOORD3.xy)))
   + 
    (0.075 * texture2D (_MainTex, xlv_TEXCOORD3.zw))
  ) + (0.0525 * texture2D (_MainTex, xlv_TEXCOORD4.xy))) + (0.0525 * texture2D (_MainTex, xlv_TEXCOORD4.zw)));
}


#endif
""
}
SubProgram ""d3d9 "" {
// Stats: 10 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
Matrix 0 [glstate_matrix_mvp]
Vector 4 [_Offsets]
""vs_2_0
def c5, 1, -1, 2, -2
def c6, 3, -3, 5, -5
dcl_position v0
dcl_texcoord v1
dp4 oPos.x, c0, v0
dp4 oPos.y, c1, v0
dp4 oPos.z, c2, v0
dp4 oPos.w, c3, v0
mov r0.xy, c4
mad oT1, r0.xyxy, c5.xxyy, v1.xyxy
mad oT2, r0.xyxy, c5.zzww, v1.xyxy
mad oT3, r0.xyxy, c6.xxyy, v1.xyxy
mad oT4, r0.xyxy, c6.zzww, v1.xyxy
mov oT0.xy, v1

""
}
SubProgram ""d3d11 "" {
// Stats: 8 math
Bind ""vertex"" Vertex
Bind ""texcoord"" TexCoord0
ConstBuffer ""$Globals"" 160
Vector 96 [_Offsets]
ConstBuffer ""UnityPerDraw"" 336
Matrix 0 [glstate_matrix_mvp]
BindCB  ""$Globals"" 0
BindCB  ""UnityPerDraw"" 1
""vs_4_0
eefiecedhjcoffdmcdajbfdajdolgeoicmclpmhpabaaaaaafeadaaaaadaaaaaa
cmaaaaaaiaaaaaaadiabaaaaejfdeheoemaaaaaaacaaaaaaaiaaaaaadiaaaaaa
aaaaaaaaaaaaaaaaadaaaaaaaaaaaaaaapapaaaaebaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaafaepfdejfeejepeoaafeeffiedepepfceeaaklkl
epfdeheolaaaaaaaagaaaaaaaiaaaaaajiaaaaaaaaaaaaaaabaaaaaaadaaaaaa
aaaaaaaaapaaaaaakeaaaaaaaaaaaaaaaaaaaaaaadaaaaaaabaaaaaaadamaaaa
keaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaaapaaaaaakeaaaaaaacaaaaaa
aaaaaaaaadaaaaaaadaaaaaaapaaaaaakeaaaaaaadaaaaaaaaaaaaaaadaaaaaa
aeaaaaaaapaaaaaakeaaaaaaaeaaaaaaaaaaaaaaadaaaaaaafaaaaaaapaaaaaa
fdfgfpfaepfdejfeejepeoaafeeffiedepepfceeaaklklklfdeieefcbeacaaaa
eaaaabaaifaaaaaafjaaaaaeegiocaaaaaaaaaaaahaaaaaafjaaaaaeegiocaaa
abaaaaaaaeaaaaaafpaaaaadpcbabaaaaaaaaaaafpaaaaaddcbabaaaabaaaaaa
ghaaaaaepccabaaaaaaaaaaaabaaaaaagfaaaaaddccabaaaabaaaaaagfaaaaad
pccabaaaacaaaaaagfaaaaadpccabaaaadaaaaaagfaaaaadpccabaaaaeaaaaaa
gfaaaaadpccabaaaafaaaaaagiaaaaacabaaaaaadiaaaaaipcaabaaaaaaaaaaa
fgbfbaaaaaaaaaaaegiocaaaabaaaaaaabaaaaaadcaaaaakpcaabaaaaaaaaaaa
egiocaaaabaaaaaaaaaaaaaaagbabaaaaaaaaaaaegaobaaaaaaaaaaadcaaaaak
pcaabaaaaaaaaaaaegiocaaaabaaaaaaacaaaaaakgbkbaaaaaaaaaaaegaobaaa
aaaaaaaadcaaaaakpccabaaaaaaaaaaaegiocaaaabaaaaaaadaaaaaapgbpbaaa
aaaaaaaaegaobaaaaaaaaaaadgaaaaafdccabaaaabaaaaaaegbabaaaabaaaaaa
dcaaaaanpccabaaaacaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaiadp
aaaaiadpaaaaialpaaaaialpegbebaaaabaaaaaadcaaaaanpccabaaaadaaaaaa
egiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaaaaeaaaaaaaeaaaaaaamaaaaaaama
egbebaaaabaaaaaadcaaaaanpccabaaaaeaaaaaaegiecaaaaaaaaaaaagaaaaaa
aceaaaaaaaaaeaeaaaaaeaeaaaaaeamaaaaaeamaegbebaaaabaaaaaadcaaaaan
pccabaaaafaaaaaaegiecaaaaaaaaaaaagaaaaaaaceaaaaaaaaakaeaaaaakaea
aaaakamaaaaakamaegbebaaaabaaaaaadoaaaaab""
}
}
Program ""fp"" {
SubProgram ""opengl "" {
""!!GLSL""
}
SubProgram ""d3d9 "" {
// Stats: 18 math, 9 textures
SetTexture 0 [_MainTex] 2D 0
""ps_2_0
def c0, 0.0524999984, 0, 0, 0
def c1, 0.150000006, 0.224999994, 0.109999999, 0.075000003
dcl_pp t0.xy
dcl_pp t1
dcl_pp t2
dcl_pp t3
dcl_pp t4
dcl_2d s0
mov_pp r0.x, t1.z
mov_pp r0.y, t1.w
mov_pp r1.x, t2.z
mov_pp r1.y, t2.w
mov_pp r2.x, t3.z
mov_pp r2.y, t3.w
mov_pp r3.x, t4.z
mov_pp r3.y, t4.w
texld r4, t1, s0
texld r5, t0, s0
texld r0, r0, s0
texld r6, t2, s0
texld r1, r1, s0
texld r7, t3, s0
texld r2, r2, s0
texld r8, t4, s0
texld r3, r3, s0
mul r4, r4, c1.x
mad_pp r4, r5, c1.y, r4
mad_pp r0, r0, c1.x, r4
mad_pp r0, r6, c1.z, r0
mad_pp r0, r1, c1.z, r0
mad_pp r0, r7, c1.w, r0
mad_pp r0, r2, c1.w, r0
mad_pp r0, r8, c0.x, r0
mad_pp r0, r3, c0.x, r0
mov_pp oC0, r0

""
}
SubProgram ""d3d11 "" {
// Stats: 9 math, 9 textures
SetTexture 0 [_MainTex] 2D 0
""ps_4_0
eefiecedmjmmcfkdlgnfjhkkonhjjkmjpdhmfpfmabaaaaaaieaeaaaaadaaaaaa
cmaaaaaaoeaaaaaabiabaaaaejfdeheolaaaaaaaagaaaaaaaiaaaaaajiaaaaaa
aaaaaaaaabaaaaaaadaaaaaaaaaaaaaaapaaaaaakeaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaabaaaaaaadadaaaakeaaaaaaabaaaaaaaaaaaaaaadaaaaaaacaaaaaa
apapaaaakeaaaaaaacaaaaaaaaaaaaaaadaaaaaaadaaaaaaapapaaaakeaaaaaa
adaaaaaaaaaaaaaaadaaaaaaaeaaaaaaapapaaaakeaaaaaaaeaaaaaaaaaaaaaa
adaaaaaaafaaaaaaapapaaaafdfgfpfaepfdejfeejepeoaafeeffiedepepfcee
aaklklklepfdeheocmaaaaaaabaaaaaaaiaaaaaacaaaaaaaaaaaaaaaaaaaaaaa
adaaaaaaaaaaaaaaapaaaaaafdfgfpfegbhcghgfheaaklklfdeieefcgeadaaaa
eaaaaaaanjaaaaaafkaaaaadaagabaaaaaaaaaaafibiaaaeaahabaaaaaaaaaaa
ffffaaaagcbaaaaddcbabaaaabaaaaaagcbaaaadpcbabaaaacaaaaaagcbaaaad
pcbabaaaadaaaaaagcbaaaadpcbabaaaaeaaaaaagcbaaaadpcbabaaaafaaaaaa
gfaaaaadpccabaaaaaaaaaaagiaaaaacacaaaaaaefaaaaajpcaabaaaaaaaaaaa
egbabaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadiaaaaakpcaabaaa
aaaaaaaaegaobaaaaaaaaaaaaceaaaaajkjjbjdojkjjbjdojkjjbjdojkjjbjdo
efaaaaajpcaabaaaabaaaaaaegbabaaaabaaaaaaeghobaaaaaaaaaaaaagabaaa
aaaaaaaadcaaaaampcaabaaaaaaaaaaaegaobaaaabaaaaaaaceaaaaaggggggdo
ggggggdoggggggdoggggggdoegaobaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaa
ogbkbaaaacaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaampcaabaaa
aaaaaaaaegaobaaaabaaaaaaaceaaaaajkjjbjdojkjjbjdojkjjbjdojkjjbjdo
egaobaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaaadaaaaaaeghobaaa
aaaaaaaaaagabaaaaaaaaaaadcaaaaampcaabaaaaaaaaaaaegaobaaaabaaaaaa
aceaaaaakoehobdnkoehobdnkoehobdnkoehobdnegaobaaaaaaaaaaaefaaaaaj
pcaabaaaabaaaaaaogbkbaaaadaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaa
dcaaaaampcaabaaaaaaaaaaaegaobaaaabaaaaaaaceaaaaakoehobdnkoehobdn
koehobdnkoehobdnegaobaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaegbabaaa
aeaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaampcaabaaaaaaaaaaa
egaobaaaabaaaaaaaceaaaaajkjjjjdnjkjjjjdnjkjjjjdnjkjjjjdnegaobaaa
aaaaaaaaefaaaaajpcaabaaaabaaaaaaogbkbaaaaeaaaaaaeghobaaaaaaaaaaa
aagabaaaaaaaaaaadcaaaaampcaabaaaaaaaaaaaegaobaaaabaaaaaaaceaaaaa
jkjjjjdnjkjjjjdnjkjjjjdnjkjjjjdnegaobaaaaaaaaaaaefaaaaajpcaabaaa
abaaaaaaegbabaaaafaaaaaaeghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaam
pcaabaaaaaaaaaaaegaobaaaabaaaaaaaceaaaaadnakfhdndnakfhdndnakfhdn
dnakfhdnegaobaaaaaaaaaaaefaaaaajpcaabaaaabaaaaaaogbkbaaaafaaaaaa
eghobaaaaaaaaaaaaagabaaaaaaaaaaadcaaaaampccabaaaaaaaaaaaegaobaaa
abaaaaaaaceaaaaadnakfhdndnakfhdndnakfhdndnakfhdnegaobaaaaaaaaaaa
doaaaaab""
}
}
 }
}
Fallback Off
}";
    }
}