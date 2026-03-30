Shader "Custom/Nebula"
{
    Properties
    {
        _ColorA ("Color A", Color) = (0.4, 0.1, 0.6, 1)
        _ColorB ("Color B", Color) = (0.1, 0.3, 0.8, 1)
        _ColorC ("Color C (Core)", Color) = (1.0, 0.4, 0.7, 1)
        _Alpha ("Overall Alpha", Range(0, 1)) = 0.15
        _NoiseScale ("Noise Scale", Range(1, 10)) = 3.0
        _WarpStrength ("Warp Strength", Range(0, 3)) = 1.2
        _SpiralStrength ("Spiral Strength", Range(0, 5)) = 2.0
        _DriftSpeed ("Drift Speed", Range(0, 0.1)) = 0.01
        _CoreBrightness ("Core Brightness", Range(0, 3)) = 1.5
        _FalloffPower ("Edge Falloff", Range(0.5, 5)) = 2.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent-1" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            float4 _ColorA, _ColorB, _ColorC;
            float _Alpha, _NoiseScale, _WarpStrength, _SpiralStrength;
            float _DriftSpeed, _CoreBrightness, _FalloffPower;

            // Simplex 2D noise
            float3 mod289_3(float3 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float2 mod289_2(float2 x) { return x - floor(x * (1.0 / 289.0)) * 289.0; }
            float3 permute(float3 x) { return mod289_3(((x * 34.0) + 1.0) * x); }

            float snoise(float2 v)
            {
                const float4 C = float4(0.211324865405187, 0.366025403784439,
                                        -0.577350269189626, 0.024390243902439);
                float2 i = floor(v + dot(v, C.yy));
                float2 x0 = v - i + dot(i, C.xx);
                float2 i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
                float4 x12 = x0.xyxy + C.xxzz;
                x12.xy -= i1;
                i = mod289_2(i);
                float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0)) + i.x + float3(0.0, i1.x, 1.0));
                float3 m = max(0.5 - float3(dot(x0, x0), dot(x12.xy, x12.xy), dot(x12.zw, x12.zw)), 0.0);
                m = m * m;
                m = m * m;
                float3 x_ = 2.0 * frac(p * C.www) - 1.0;
                float3 h = abs(x_) - 0.5;
                float3 ox = floor(x_ + 0.5);
                float3 a0 = x_ - ox;
                m *= 1.79284291400159 - 0.85373472095314 * (a0 * a0 + h * h);
                float3 g;
                g.x = a0.x * x0.x + h.x * x0.y;
                g.yz = a0.yz * x12.xz + h.yz * x12.yw;
                return 130.0 * dot(m, g);
            }

            float fbm(float2 p, int octaves)
            {
                float value = 0.0;
                float amp = 0.5;
                float freq = 1.0;
                for (int i = 0; i < 4; i++)
                {
                    if (i >= octaves) break;
                    value += amp * snoise(p * freq);
                    freq *= 2.0;
                    amp *= 0.5;
                }
                return value;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 centered = i.uv - 0.5;
                float dist = length(centered) * 2.0;

                // Radial falloff — smooth to zero well before quad edge
                float falloff = smoothstep(1.0, 0.4, dist);
                falloff *= falloff;
                if (falloff < 0.001) return float4(0, 0, 0, 0);

                // Polar coordinates for spiral
                float angle = atan2(centered.y, centered.x);
                float r = length(centered);

                // Time
                float t = _Time.y * _DriftSpeed;

                // Spiral UV warp — twist angle based on distance from center
                float spiralAngle = angle + r * _SpiralStrength + t * 3.0;
                float2 spiralUV = float2(cos(spiralAngle), sin(spiralAngle)) * r;

                // Domain warping for organic feel
                float2 noiseUV = spiralUV * _NoiseScale + float2(t, t * 0.7);
                float warpX = snoise(noiseUV + float2(5.2, 1.3));
                float warpY = snoise(noiseUV + float2(9.7, 3.1));
                noiseUV += float2(warpX, warpY) * _WarpStrength;

                // Layered noise
                float n = fbm(noiseUV, 4);
                n = n * 0.5 + 0.5; // remap to 0..1

                // Second layer for arm structure
                float arms = snoise(spiralUV * _NoiseScale * 0.5 + float2(t * 0.5, 0.0));
                arms = smoothstep(-0.2, 0.6, arms);

                float density = n * arms;

                // Color blend: outer → inner
                float3 color = lerp(_ColorA.rgb, _ColorB.rgb, n);
                // Bright core
                float coreMask = (1.0 - saturate(dist * 1.5));
                coreMask = pow(coreMask, 2.0);
                color = lerp(color, _ColorC.rgb, coreMask * _CoreBrightness * density);

                // Emissive boost in dense regions
                color *= 1.0 + density * 0.5;

                float alpha = density * falloff * _Alpha * i.color.a;

                return float4(color, alpha);
            }
            ENDCG
        }
    }
}
