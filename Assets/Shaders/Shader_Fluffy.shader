Shader "Custom/Shader_Fluffy"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _FurColor ("Fur Color", Color) = (1,1,1,1)

        _FurLength ("Fur Length", Range(0, 0.5)) = 0.2
        _ShellCount ("Shell Count", Range(1, 40)) = 24

        _NoiseScale ("Noise Scale", Range(1, 60)) = 22
        _Density ("Density", Range(0.1, 5)) = 2

        _ClumpStrength ("Clump Strength", Range(0, 1)) = 0.75
        _Softness ("Softness", Range(0.5, 5)) = 1.5

        _DitherStrength ("Dither Strength", Range(0,1)) = 0.35
    }

    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" }

        ZWrite On
        ZTest LEqual
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _FurColor;

            float _FurLength;
            float _ShellCount;
            float _NoiseScale;
            float _Density;
            float _ClumpStrength;
            float _Softness;
            float _DitherStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float shell : TEXCOORD3;
            };

            float hash(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z);
            }

            float noise(float3 p)
            {
                float3 i = floor(p);
                return hash(i);
            }

            float fbm(float3 p)
            {
                float v = 0;
                float a = 0.5;

                for(int i=0;i<3;i++)
                {
                    v += a * noise(p);
                    p *= 2.0;
                    a *= 0.5;
                }
                return v;
            }

            v2f vert(appdata v)
            {
                v2f o;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 dir = normalize(v.normal);

                int shellCount = (int)_ShellCount;

                float fur = 0;

                for (int i = 0; i < 40; i++)
                {
                    if (i >= shellCount) break;

                    float t = (float)i / max(1.0, _ShellCount - 1);

                    float3 p = worldPos * _NoiseScale + i * 13.1;

                    float n = fbm(p);

                    float clump = smoothstep(0.3, 0.85, n);
                    clump = lerp(n, clump, _ClumpStrength);

                    clump *= (1.0 - t);

                    fur += clump * t * _FurLength;
                }

                fur = saturate(fur * _Density);

                v.vertex.xyz += dir * fur;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                float3 biasedNormal = normalize(v.normal + dir * fur * 2.0);
                o.worldNormal = UnityObjectToWorldNormal(biasedNormal);

                o.shell = fur;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float3 N = normalize(i.worldNormal);
                float3 V = normalize(_WorldSpaceCameraPos.xyz - i.worldPos);

                float NdotL = dot(N, L);

                float wrap = 0.5;
                float front = saturate((NdotL + wrap) / (1.0 + wrap));
                front = pow(front, 1.2);

                float sss = saturate(-NdotL * 0.5);
                sss = pow(sss, 2.5) * 0.25;

                float rim = 1.0 - saturate(dot(N, V));
                rim = pow(rim, 2.5) * 0.25;

                float lighting = front + sss + rim;

                float depthFade = lerp(0.25, 1.0, i.shell);
                lighting *= depthFade;

                lighting += 0.2;

                float tex = tex2D(_MainTex, i.uv).r;

                float3 col = _FurColor.rgb * lighting * tex;

                float dither = frac(sin(dot(i.pos.xy, float2(12.9898, 78.233))) * 43758.5453);
                float alphaMask = step(dither, i.shell + _DitherStrength);

                return float4(col, alphaMask);
            }

            ENDCG
        }
    }
}