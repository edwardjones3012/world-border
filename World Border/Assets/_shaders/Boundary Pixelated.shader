Shader "Boundary/Pixelated"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        [Header(Color)]
        [Space(7)]
        _FarColor ("Far Color", Color) = (1,1,1,1) 
        _NearColor ("Near Color", Color) = (1,1,1,1)

        [Header(Distance)]
        [Space(7)]
        [IntRange] _NearPoint ("Near Point", Range(1,25)) = 1
        [IntRange] _MinDistance ("Min Distance", Range(0,25)) = 1
        [IntRange] _MaxDistance ("Max Distance", Range(0,25)) = 5

        [Header(Pattern)]
        [Space(7)]
        [IntRange] _Pixelation ("Pixelation", Range(8,256)) = 64
        [IntRange] _LineWidth ("Line Width", Range(1, 9)) = 5
        [IntRange] _HorizontalDistortion ("Horizontal Distortion", Range(0, 10)) = 0

        [Header(Misc)]
        [Space(10)]
        [IntRange] _Speed ("Speed", Range(0, 10)) = 1
        [KeywordEnum(Up, Down)] _VerticalDirection ("Vertical Direction", Float) = 0
        [KeywordEnum(Left, Right)] _HorizontalDirection ("Horizontal Direction", Float) = 0
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }
        Pass
        {
            Cull Off
            ZWrite Off
            Blend One One

            CGPROGRAM   
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _FarColor;
            float4 _NearColor;
            float _NearPoint;

            float _ColourStart;
            float _ColourEnd;

            float3 _CameraPos;

            float _MinDistance;
            float _MaxDistance;

            float _Pixelation;
            float _LineWidth;
            float _HorizontalDistortion;
            float _Speed;
            float _VerticalDirection;
            float _HorizontalDirection;

            static const float TAU = 6.283158;

            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal: NORMAL;
                float3 camPos : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 camPos : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            float InverseLerp(float a, float b, float v)
            {
                return (v-a)/(b-a);
            }

            v2f vert (MeshData m)
            {
                v2f o;

                // basic
                o.vertex = UnityObjectToClipPos(m.vertex);
                o.uv = TRANSFORM_TEX(m.uv, _MainTex);
                o.normal = m.normal;
                
                // additional
                o.camPos = _WorldSpaceCameraPos.xyz;
                o.worldPos = mul(unity_ObjectToWorld, m.vertex);

                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float horizontalMultipler = -floor((_HorizontalDirection * 1.5) - .1);
                float verticalMultiplier = floor((_VerticalDirection * 1.5) - .1) * horizontalMultipler;
                
                // pixelate
                i.uv = round(i.uv * _Pixelation)/ _Pixelation;

                // sample texture for the uv coordinate
                float4 textureCol = tex2D(_MainTex, i.uv);

                // find distance and use to determine transparency
                float4 distance = length(i.worldPos - i.camPos);
                float multiplier = InverseLerp(_MaxDistance, _MinDistance, distance);
                multiplier = clamp(multiplier, 0, 1);

                // get color (near vs far)
                float blendLength = 5;
                float c = InverseLerp(_NearPoint, _NearPoint + blendLength, distance);
                c = clamp(c, 0, 1);
                float4 col = lerp(_NearColor, _FarColor, c);

                // get pattern
                float yOffset = cos(i.uv.y * TAU * 3) * 0.01 * _HorizontalDistortion;
                float t = (cos (((i.uv.x + i.uv.y * verticalMultiplier) + yOffset + (_Time.y * horizontalMultipler) * (0.1 * _Speed)) * TAU * 1)) *.5 + .5;

                // creates hard edges at a threshold
                float width = _LineWidth/10;
                float lines = step(1-width,t);

                return lines * multiplier * col * textureCol;
            }
            ENDCG
        }
    }
}
