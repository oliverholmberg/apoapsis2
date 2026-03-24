Shader "Custom/ProceduralBody"
{
    Properties
    {
        // Colors
        _ColorA ("Color A (Primary)", Color) = (0.1, 0.0, 0.4, 1)
        _ColorB ("Color B (Secondary)", Color) = (1.0, 0.0, 0.8, 1)
        _RimColor ("Rim / Atmosphere Color", Color) = (0.5, 0.8, 1.0, 1)

        // Noise
        _NoiseScale ("Noise Scale", Range(1, 20)) = 4.0
        _NoiseOctaves ("Noise Octaves", Range(1, 6)) = 3
        _NoiseLacunarity ("Noise Lacunarity", Range(1, 4)) = 2.0
        _NoisePersistence ("Noise Persistence", Range(0, 1)) = 0.5

        // Domain warping
        _WarpStrength ("Warp Strength", Range(0, 2)) = 0.3
        _WarpScale ("Warp Scale", Range(0.5, 10)) = 3.0

        // Animation
        _DriftSpeed ("Drift Speed", Range(0, 0.5)) = 0.02
        _DriftDirection ("Drift Direction", Vector) = (1, 0.3, 0, 0)

        // Fresnel rim
        _RimPower ("Rim Power", Range(0.5, 8)) = 2.5
        _RimIntensity ("Rim Intensity", Range(0, 3)) = 1.2

        // Emissive
        _EmissiveIntensity ("Emissive Intensity", Range(0, 2)) = 0.3
        _EmissiveColor ("Emissive Color", Color) = (1, 1, 1, 1)
        _EmissiveThreshold ("Emissive Threshold", Range(0, 1)) = 0.6

        // Style modifiers
        _BandStrength ("Band Strength (Gas Giant)", Range(0, 1)) = 0.0
        _VoronoiStrength ("Voronoi Strength (Craters)", Range(0, 1)) = 0.0
        _VoronoiScale ("Voronoi Scale", Range(1, 15)) = 5.0
        _CrackStrength ("Crack Strength (Lava)", Range(0, 1)) = 0.0

        // Blend mode (overridable for off-screen rendering)
        [HideInInspector] _SrcBlend ("Src Blend", Float) = 5  // SrcAlpha
        [HideInInspector] _DstBlend ("Dst Blend", Float) = 10 // OneMinusSrcAlpha
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend [_SrcBlend] [_DstBlend]
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

            // Properties
            float4 _ColorA, _ColorB, _RimColor, _EmissiveColor;
            float _NoiseScale, _NoiseOctaves, _NoiseLacunarity, _NoisePersistence;
            float _WarpStrength, _WarpScale;
            float _DriftSpeed;
            float4 _DriftDirection;
            float _RimPower, _RimIntensity;
            float _EmissiveIntensity, _EmissiveThreshold;
            float _BandStrength, _VoronoiStrength, _VoronoiScale;
            float _CrackStrength;

            // --- Simplex 2D Noise ---
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

            // --- Voronoi ---
            float2 voronoi_hash(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453);
            }

            float voronoi(float2 x, float scale)
            {
                x *= scale;
                float2 n = floor(x);
                float2 f = frac(x);
                float md = 8.0;
                for (int j = -1; j <= 1; j++)
                for (int i = -1; i <= 1; i++)
                {
                    float2 g = float2(i, j);
                    float2 o = voronoi_hash(n + g);
                    float2 r = g + o - f;
                    float d = dot(r, r);
                    if (d < md) md = d;
                }
                return sqrt(md);
            }

            // --- FBM (Fractal Brownian Motion) ---
            // Unrolled to fixed 6 iterations for mobile GPU compatibility
            float fbm(float2 p, float octaves, float lacunarity, float persistence)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;

                value += amplitude * snoise(p * frequency);
                frequency *= lacunarity; amplitude *= persistence;
                if (octaves <= 1.0) return value;

                value += amplitude * snoise(p * frequency);
                frequency *= lacunarity; amplitude *= persistence;
                if (octaves <= 2.0) return value;

                value += amplitude * snoise(p * frequency);
                frequency *= lacunarity; amplitude *= persistence;
                if (octaves <= 3.0) return value;

                value += amplitude * snoise(p * frequency);
                frequency *= lacunarity; amplitude *= persistence;
                if (octaves <= 4.0) return value;

                value += amplitude * snoise(p * frequency);
                frequency *= lacunarity; amplitude *= persistence;
                if (octaves <= 5.0) return value;

                value += amplitude * snoise(p * frequency);
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
                // Center UV, calculate distance from center
                float2 centeredUV = i.uv - 0.5;
                float dist = length(centeredUV) * 2.0; // 0 at center, 1 at edge

                // Circular mask — discard outside sphere
                if (dist > 1.0)
                    return float4(0, 0, 0, 0);

                // Smooth edge anti-aliasing
                float edgeAA = 1.0 - smoothstep(0.95, 1.0, dist);

                // Simulate spherical mapping (fake 3D from 2D)
                float sphereZ = sqrt(max(0.0, 1.0 - dist * dist));
                float2 sphereUV = centeredUV / max(sphereZ * 0.8 + 0.2, 0.001);

                // Time drift
                float t = _Time.y * _DriftSpeed;
                float2 drift = _DriftDirection.xy * t;

                // Domain warping
                float2 warpedUV = sphereUV * _NoiseScale + drift;
                if (_WarpStrength > 0.001)
                {
                    float warpX = snoise(warpedUV * _WarpScale + float2(5.2, 1.3));
                    float warpY = snoise(warpedUV * _WarpScale + float2(9.7, 3.1));
                    warpedUV += float2(warpX, warpY) * _WarpStrength;
                }

                // Band effect for gas giants
                if (_BandStrength > 0.001)
                {
                    float bandNoise = snoise(float2(warpedUV.x * 0.3, warpedUV.y * 3.0));
                    warpedUV.y += bandNoise * _BandStrength * 0.5;
                    warpedUV.x *= lerp(1.0, 0.3, _BandStrength);
                }

                // Main noise
                float n = fbm(warpedUV, _NoiseOctaves, _NoiseLacunarity, _NoisePersistence);
                n = n * 0.5 + 0.5; // Remap -1..1 to 0..1

                // Voronoi craters
                if (_VoronoiStrength > 0.001)
                {
                    float v = voronoi(sphereUV + drift * 0.1, _VoronoiScale);
                    float craterDepth = smoothstep(0.0, 0.3, v);
                    n = lerp(n, n * craterDepth, _VoronoiStrength);
                }

                // Lava cracks
                float crackEmissive = 0.0;
                if (_CrackStrength > 0.001)
                {
                    float v = voronoi(sphereUV * 1.5 + drift * 0.2, _VoronoiScale * 0.7);
                    float crack = 1.0 - smoothstep(0.0, 0.08, v);
                    crackEmissive = crack * _CrackStrength;
                    n = lerp(n, n * 0.3, crack * _CrackStrength * 0.5);
                }

                // Base surface color
                float3 surfaceColor = lerp(_ColorA.rgb, _ColorB.rgb, n);

                // Fresnel rim glow (simulate atmosphere)
                float rim = pow(dist, _RimPower);
                float3 rimContribution = _RimColor.rgb * rim * _RimIntensity;

                // Darken surface near edges (fake lighting)
                float lighting = lerp(1.0, 0.4, pow(dist, 1.5));
                surfaceColor *= lighting;

                // Emissive
                float3 emissive = float3(0, 0, 0);
                if (_EmissiveIntensity > 0.001)
                {
                    float emMask = smoothstep(_EmissiveThreshold, _EmissiveThreshold + 0.2, n);
                    emissive = _EmissiveColor.rgb * emMask * _EmissiveIntensity;
                }
                emissive += _EmissiveColor.rgb * crackEmissive * 2.0;

                // Combine
                float3 finalColor = surfaceColor + rimContribution + emissive;

                // Apply sprite tint
                finalColor *= i.color.rgb;
                float alpha = edgeAA * i.color.a;

                return float4(finalColor, alpha);
            }
            ENDCG
        }
    }
}
