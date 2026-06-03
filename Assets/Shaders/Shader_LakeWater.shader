Shader "Custom/Shader_LakeWater"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed", Vector) = (0, 0.03, 0, 0)
        _DisplacementHeight ("Displacement Height", Float) = 0.1
        _TimeScale ("Time Scale", Float) = 0.4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Speed;
            float _DisplacementHeight;
            float _TimeScale;

            v2f vert (appdata v)
            {
                v2f o;
                //Convert to world space
                float4 localPos = v.vertex;
                float4 worldPos = mul(UNITY_MATRIX_M, localPos);

                float t = _Time.y * _TimeScale;
                worldPos.y += sin(worldPos.x * 0.5 + t) * _DisplacementHeight;
                worldPos.y += cos(worldPos.z * 0.5 + t * 0.8) * _DisplacementHeight;

                float4 viewPos = mul(UNITY_MATRIX_V, worldPos);
                o.vertex = mul(UNITY_MATRIX_P, viewPos);

                float2 offset = -_Time.y * _Speed.xy * 0.2;

                o.uv = TRANSFORM_TEX(v.uv, _MainTex) + offset;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
