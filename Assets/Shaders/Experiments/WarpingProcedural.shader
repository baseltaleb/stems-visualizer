Shader "Custom/WarpinProcedural"
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

            static const float2x2 m = transpose(float2x2(0.8, 0.6, -0.6, 0.8));
            float noise(in float2 p)
            {
                return sin(p.x)*sin(p.y);
            }

            float fbm4(float2 p)
            {
                float f = 0.;
                f += 0.5*noise(p);
                p = mul(m,p)*2.02;
                f += 0.25*noise(p);
                p = mul(m,p)*2.03;
                f += 0.125*noise(p);
                p = mul(m,p)*2.01;
                f += 0.0625*noise(p);
                return f/0.9375;
            }

            float fbm6(float2 p)
            {
                float f = 0.;
                f += 0.5*(0.5+0.5*noise(p));
                p = mul(m,p)*2.02;
                f += 0.25*(0.5+0.5*noise(p));
                p = mul(m,p)*2.03;
                f += 0.125*(0.5+0.5*noise(p));
                p = mul(m,p)*2.01;
                f += 0.0625*(0.5+0.5*noise(p));
                p = mul(m,p)*2.04;
                f += 0.03125*(0.5+0.5*noise(p));
                p = mul(m,p)*2.01;
                f += 0.015625*(0.5+0.5*noise(p));
                return f/0.96875;
            }

            float2 fbm4_2(float2 p)
            {
                return float2(fbm4(p), fbm4(p+((float2)7.8)));
            }

            float2 fbm6_2(float2 p)
            {
                return float2(fbm6(p+((float2)16.8)), fbm6(p+((float2)11.5)));
            }

            float func(float2 q, out float4 ron)
            {
                ron = 0;
                q += 0.03*sin(float2(0.27, 0.23)*_Time.y+length(q)*float2(4.1, 4.3));
                float2 o = fbm4_2(0.9*q);
                o += 0.04*sin(float2(0.12, 0.14)*_Time.y+length(o));
                float2 n = fbm6_2(3.*o);
                ron = float4(o, n);
                float f = 0.5+0.5*fbm4(1.8*q+6.*n);
                return lerp(f, f*f*f*3.5, f*abs(n.x));
            }

            float4 frag (v2f __vertex_output) : SV_Target
            {
                vertex_output = __vertex_output;
                float4 fragColor = 0;
                float2 fragCoord = vertex_output.uv * _Resolution;
                float2 p = (2.*fragCoord-iResolution.xy)/iResolution.y;
                float e = 2./iResolution.y;
                float4 on = ((float4)0.);
                float f = func(p, on);
                float3 col = ((float3)0.);
                col = lerp(float3(0.2, 0.1, 0.4), float3(0.3, 0.05, 0.05), f);
                col = lerp(col, float3(0.9, 0.9, 0.9), dot(on.zw, on.zw));
                col = lerp(col, float3(0.4, 0.3, 0.3), 0.2+0.5*on.y*on.y);
                col = lerp(col, float3(0., 0.2, 0.4), 0.5*smoothstep(1.2, 1.3, abs(on.z)+abs(on.w)));
                col = clamp(col*f*2., 0., 1.);
#if 0
                float3 nor = normalize(float3(ddx(f)*iResolution.x, 6., ddy(f)*iResolution.y));
#else
                float4 kk;
                float3 nor = normalize(float3(func(p+float2(e, 0.), kk)-f, 2.*e, func(p+float2(0., e), kk)-f));
#endif
                float3 lig = normalize(float3(0.9, 0.2, -0.4));
                float dif = clamp(0.3+0.7*dot(nor, lig), 0., 1.);
                float3 lin = float3(0.7, 0.9, 0.95)*(nor.y*0.5+0.5)+float3(0.15, 0.1, 0.05)*dif;
                col *= 1.2*lin;
                col = 1.-col;
                col = 1.1*col*col;
                fragColor = float4(col, 1.);
                if (_GammaCorrect) fragColor.rgb = pow(fragColor.rgb, 2.2);
                return fragColor;
            }
            ENDCG
        }
    }
}
