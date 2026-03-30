Shader "Custom/Aurora"
{
    Properties
    {
        _ColorA ("Color A (Base)", Color) = (0.1, 0.8, 0.4, 1)
        _ColorB ("Color B (Mid)", Color) = (0.2, 0.5, 0.9, 1)
        _ColorC ("Color C (Top)", Color) = (0.6, 0.2, 0.8, 1)
        _Alpha ("Overall Alpha", Range(0, 1)) = 0.12
        _FlowSpeed ("Flow Speed", Range(0, 2)) = 0.3
        _WaveFreq ("Wave Frequency", Range(1, 10)) = 3.0
        _WaveAmp ("Wave Amplitude", Range(0, 0.5)) = 0.15
        _NoiseScale ("Noise Scale", Range(1, 10)) = 4.0
        _Thickness ("Curtain Thickness", Range(0.01, 0.3)) = 0.08
        _Layers ("Layer Count", Range(1, 4)) = 3
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
            float _Alpha, _FlowSpeed, _WaveFreq, _WaveAmp;
            float _NoiseScale, _Thickness, _Layers;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float auroraLayer(float2 uv, float offset, float t)
            {
                // Horizontal wave displacement
                float wave = sin(uv.x * _WaveFreq + t * 2.0 + offset * 5.0) * _WaveAmp;
                wave += sin(uv.x * _WaveFreq * 1.7 + t * 1.3 + offset * 3.0) * _WaveAmp * 0.5;

                // Noise-based displacement for organic shape
                float noiseDisp = snoise(float2(uv.x * _NoiseScale * 0.5 + t * 0.5, offset * 10.0)) * 0.1;

                // Curtain center line (horizontal band)
                float centerY = 0.5 + offset * 0.15 + wave + noiseDisp;

                // Distance from curtain center
                float d = abs(uv.y - centerY);

                // Curtain shape — sharp bright core, soft glow falloff
                float curtain = exp(-d * d / (_Thickness * _Thickness));

                // Noise modulation for variation along the curtain
                float noiseMod = snoise(float2(uv.x * _NoiseScale + t * 0.3, uv.y * 2.0 + offset * 7.0));
                noiseMod = noiseMod * 0.5 + 0.5;

                // Intensity varies along horizontal
                float horizVar = snoise(float2(uv.x * _NoiseScale * 0.3 + t * 0.2, offset * 3.0));
                horizVar = smoothstep(-0.3, 0.7, horizVar);

                return curtain * noiseMod * horizVar;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float t = _Time.y * _FlowSpeed;

                // Smooth fade on all edges to avoid hard quad boundary
                float edgeFade = smoothstep(0.0, 0.25, uv.x) * smoothstep(1.0, 0.75, uv.x)
                               * smoothstep(0.0, 0.25, uv.y) * smoothstep(1.0, 0.75, uv.y);

                // Vertical fade — stronger in upper portion
                float verticalFade = smoothstep(0.0, 0.3, uv.y) * smoothstep(1.0, 0.6, uv.y) * edgeFade;

                float totalDensity = 0.0;
                float3 totalColor = float3(0, 0, 0);

                // Multiple curtain layers
                int layers = (int)_Layers;
                for (int l = 0; l < 4; l++)
                {
                    if (l >= layers) break;

                    float layerOffset = (float)l / (float)layers - 0.5;
                    float density = auroraLayer(uv, layerOffset, t + l * 1.7);

                    // Color varies by height and layer
                    float colorT = uv.y + (float)l * 0.15;
                    float3 layerColor;
                    if (colorT < 0.5)
                        layerColor = lerp(_ColorA.rgb, _ColorB.rgb, colorT * 2.0);
                    else
                        layerColor = lerp(_ColorB.rgb, _ColorC.rgb, (colorT - 0.5) * 2.0);

                    totalDensity += density;
                    totalColor += layerColor * density;
                }

                if (totalDensity < 0.001) return float4(0, 0, 0, 0);

                totalColor /= max(totalDensity, 0.001);

                // Boost brightness in dense regions
                totalColor *= 1.0 + totalDensity * 0.3;

                float alpha = saturate(totalDensity) * verticalFade * _Alpha * i.color.a;

                return float4(totalColor, alpha);
            }
            ENDCG
        }
    }
}
