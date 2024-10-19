Shader "Custom/Hexagons"
{
    Properties
    {
        _MainTex ("iChannel0", 2D) = "white" {}
        _SecondTex ("iChannel1", 2D) = "white" {}
        _ThirdTex ("iChannel2", 2D) = "white" {}
        _FourthTex ("iChannel3", 2D) = "white" {}
        _Mouse ("Mouse", Vector) = (0.5, 0.5, 0.5, 0.5)
        [ToggleUI] _GammaCorrect ("Gamma Correction", Float) = 1
        _Resolution ("Resolution (Change if AA is bad)", Range(1, 1024)) = 1
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Built-in properties
            sampler2D _MainTex;   float4 _MainTex_TexelSize;
            sampler2D _SecondTex; float4 _SecondTex_TexelSize;
            sampler2D _ThirdTex;  float4 _ThirdTex_TexelSize;
            sampler2D _FourthTex; float4 _FourthTex_TexelSize;
            float4 _Mouse;
            float _GammaCorrect;
            float _Resolution;

            // GLSL Compatability macros
            #define glsl_mod(x,y) (((x)-(y)*floor((x)/(y))))
            #define texelFetch(ch, uv, lod) tex2Dlod(ch, float4((uv).xy * ch##_TexelSize.xy + ch##_TexelSize.xy * 0.5, 0, lod))
            #define textureLod(ch, uv, lod) tex2Dlod(ch, float4(uv, 0, lod))
            #define iResolution float3(_Resolution, _Resolution, _Resolution)
            #define iFrame (floor(_Time.y / 60))
            #define iChannelTime float4(_Time.y, _Time.y, _Time.y, _Time.y)
            #define iDate float4(2020, 6, 18, 30)
            #define iSampleRate (44100)
            #define iChannelResolution float4x4(                      \
                _MainTex_TexelSize.z,   _MainTex_TexelSize.w,   0, 0, \
                _SecondTex_TexelSize.z, _SecondTex_TexelSize.w, 0, 0, \
                _ThirdTex_TexelSize.z,  _ThirdTex_TexelSize.w,  0, 0, \
                _FourthTex_TexelSize.z, _FourthTex_TexelSize.w, 0, 0)

            // Global access to uv data
            static v2f vertex_output;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv =  v.uv;
                return o;
            }

#define AA 2
            float4 hexagon(float2 p)
            {
                float2 q = float2(p.x*2.*0.5773503, p.y+p.x*0.5773503);
                float2 pi = floor(q);
                float2 pf = frac(q);
                float v = glsl_mod(pi.x+pi.y, 3.);
                float ca = step(1., v);
                float cb = step(2., v);
                float2 ma = step(pf.xy, pf.yx);
                float e = dot(ma, 1.-pf.yx+ca*(pf.x+pf.y-1.)+cb*(pf.yx-2.*pf.xy));
                p = float2(q.x+floor(0.5+p.y/1.5), 4.*p.y/3.)*0.5+0.5;
                float f = length((frac(p)-0.5)*float2(1., 0.85));
                return float4(pi+ca-cb*ma, e, f);
            }

            float hash1(float2 p)
            {
                float n = dot(p, float2(127.1, 311.7));
                return frac(sin(n)*43758.547);
            }

            float noise(in float3 x)
            {
                float3 p = floor(x);
                float3 f = frac(x);
                f = f*f*(3.-2.*f);
                float2 uv = p.xy+float2(37., 17.)*p.z+f.xy;
                float2 rg = textureLod(_MainTex, (uv+0.5)/256., 0.).yx;
                return lerp(rg.x, rg.y, f.z);
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float3 tot = ((float3)0.);
                for (int mm = 0;mm<AA; mm++)
                for (int nn = 0;nn<AA; nn++)
                {
                    float2 off = float2(mm, nn)/float(AA);
                    float2 uv = (fragCoord+off)/iResolution.xy;
                    float2 pos = (-iResolution.xy+2.*(fragCoord+off))/iResolution.y;
                    pos *= 1.2+0.15*length(pos);
                    float4 h = hexagon(8.*pos+0.5*_Time.y);
                    float n = noise(float3(0.3*h.xy+_Time.y*0.1, _Time.y));
                    float3 col = 0.15+0.15*hash1(h.xy+1.2)*((float3)1.);
                    col *= smoothstep(0.1, 0.11, h.z);
                    col *= smoothstep(0.1, 0.11, h.w);
                    col *= 1.+0.15*sin(40.*h.z);
                    col *= 0.75+0.5*h.z*n;
                    h = hexagon(6.*(pos+0.1*float2(-1.3, 1.))+0.6*_Time.y);
                    col *= 1.-0.8*smoothstep(0.45, 0.451, noise(float3(0.3*h.xy+_Time.y*0.1, 0.5*_Time.y)));
                    h = hexagon(6.*pos+0.6*_Time.y);
                    n = noise(float3(0.3*h.xy+_Time.y*0.1, 0.5*_Time.y));
                    float3 colb = 0.9+0.8*sin(hash1(h.xy)*1.5+2.+float3(0.1, 1., 1.1));
                    colb *= smoothstep(0.1, 0.11, h.z);
                    colb *= 1.+0.15*sin(40.*h.z);
                    col = lerp(col, colb, smoothstep(0.45, 0.451, n));
                    col *= 2.5/(2.+col);
                    col *= pow(16.*uv.x*(1.-uv.x)*uv.y*(1.-uv.y), 0.1);
                    tot += col;
                }
                tot /= float(AA*AA);
                fragColor = float4(tot, 1.);
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
