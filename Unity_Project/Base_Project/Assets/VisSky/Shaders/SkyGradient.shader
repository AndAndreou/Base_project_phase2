Shader "VisSky/SkyGradient"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Pass
        {
            Fog { Mode Off }
            Tags {"Queue" = "Opaque" }
            Lighting Off
            // ZWrite Off

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            float  _CloudDensity;
            float3 _SunDirection;
            float4 _SunColor;
            float4 _SunDisk;

            float4    _Color;
            sampler2D _MainTex;

            struct VertexOutput
            {
                float4 position : POSITION;
                float2 uvCoords : TEXCOORD0;
                float3 viewDir  : TEXCOORD1;
            };

            VertexOutput vert(appdata_full IN)
            {
                VertexOutput OUT;

                OUT.position = mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.uvCoords = IN.texcoord;
                OUT.viewDir  = IN.vertex.xyz;

                return OUT;
            }

            float CalcSunIntensity(float cosTheta, float height)
            {
                // x: (1-g)^2
                // y: 1+g^2
				// z: 2*g
                // w: power
                
                return _SunDisk.x / pow(_SunDisk.y - _SunDisk.z * cosTheta, _SunDisk.w); // * pow(1.05f - height, 0.2f);
            }

            float4 frag(VertexOutput IN) : COLOR
            {
                float3 viewDir = normalize(IN.viewDir);

                float cosTheta  = saturate(dot(-_SunDirection, viewDir));
                float sunIntens = CalcSunIntensity(cosTheta, viewDir.y); // * _SunColor.a;

                float3 skyColor = tex2D(_MainTex, float2(_CloudDensity, IN.uvCoords.y)).rgb * _Color.rgb;
                float3 sunColor = sunIntens * _SunColor.rgb;

                return float4(saturate(skyColor + sunColor), 1.0f);
            }

            ENDCG
        }
    }

    Fallback "None"
}
