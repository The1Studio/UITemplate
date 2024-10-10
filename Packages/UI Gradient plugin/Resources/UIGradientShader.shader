Shader "PolyAndCode/UI/Gradient"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _MainColor ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
        
        [HideInInspector] _AsOverlay("As Overlay", Float) = 0
        [HideInInspector] _GradientTex ("grad Texture", 2D) = "white" {}
        [HideInInspector]  _Scale ("Scale", Float) = 1
        [HideInInspector] _OffsetX ("offsetX", Float) = 0
        [HideInInspector] _OffsetY ("offsetY", Float) = 0
        [HideInInspector] _Opacity ("Opacity", Float) = 1
        [HideInInspector] _Angle ("Angle", Float) = 0
        [HideInInspector] _AspectRatio ("aspect Ratio", Float) = 0    
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
            #pragma multi_compile __ GRADIENT_LINEAR GRADIENT_RADIAL GRADIENT_ANGLE GRADIENT_REFLECTED GRADIENT_DIAMOND
            #pragma multi_compile __ BM_NORMAL BM_DARKEN BM_MULTIPLY BM_LINEARBURN BM_COLORBURN BM_DARKERCOLOR BM_LIGHTEN BM_SCREEN BM_COLORDODGE BM_LINEARDODGE BM_LIGHTENCOLOR BM_OVERLAY BM_SOFTLIGHT BM_HARDLIGHT BM_VIVIDLIGHT BM_LINEARLIGHT BM_PINLIGHT BM_HARDMIX BM_DIFFERENCE BM_EXCLUSION BM_SUBTRACT BM_DIVIDE
            #pragma multi_compile __ USE_UV1

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                //float2 texcoord2 : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float2 gradientUV  : TEXCOORD1;
                float4 worldPosition : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float _AsOverlay;
            fixed4 _MainColor;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            sampler2D _GradientTex;
            float _OffsetX, _OffsetY;
            float _Scale;
            float _Angle;
            float _Opacity;
            float _AspectRatio;

            sampler2D _MainTex;

            #include "BlendModes.cginc"


            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = v.texcoord;
            
                
                #ifdef USE_UV1
                    OUT.gradientUV = v.texcoord1;
                #else
                    OUT.gradientUV = v.texcoord;
                #endif

                //Transformed Gradient UV
                #ifndef GRADIENT_RADIAL
                float sinX = sin ( _Angle * 0.0174533 );
                float cosX = cos ( _Angle * 0.0174533 );
                OUT.gradientUV -= float2(0.5 +_OffsetX, 0.5 + _OffsetY);
                OUT.gradientUV *= float2(1,_AspectRatio);
                OUT.gradientUV.xy = float2(OUT.gradientUV.x * cosX - OUT.gradientUV.y * sinX , OUT.gradientUV.x * sinX + OUT.gradientUV.y * cosX);
                OUT.gradientUV.xy += float2(0.5 + _OffsetX, (0.5 + _OffsetY)*_AspectRatio);

                #endif
    
                OUT.color = v.color;
                return OUT;
            }


            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * _MainColor;
                float alpha = color.w;
                float4 gradColor = IN.color;
                
                #ifdef GRADIENT_LINEAR
                    gradColor = tex2D(_GradientTex, (float2(_OffsetX,_OffsetY) + IN.gradientUV) / _Scale); 
                #elif GRADIENT_RADIAL
                    float t = length( float2(IN.gradientUV.x,IN.gradientUV.y * _AspectRatio) - float2(0.5 + _OffsetX, (0.5 + _OffsetY) * _AspectRatio)) / (_Scale*0.5);
                    gradColor = (tex2D(_GradientTex, t));        
                #elif GRADIENT_DIAMOND
                    IN.gradientUV = float2(0.5 + _OffsetX, (0.5 + _OffsetY)*_AspectRatio)  - IN.gradientUV;
                    float t = abs( IN.gradientUV.x )+ abs(IN.gradientUV.y); 
                    gradColor = tex2D(_GradientTex, t / (_Scale * 0.5) );
                #elif GRADIENT_ANGLE
                    float2 radVector = IN.gradientUV - (float2(0.5, 0.5) + float2(_OffsetX,_OffsetY)) * float2(1,_AspectRatio);
                    float angle = atan2(radVector.y, radVector.x);
                    if(angle < 0 ){
                        angle +=  6.28318;
                    }
                    gradColor = tex2D(_GradientTex, float2(angle/6.28318,0));
                    
                #elif GRADIENT_REFLECTED
                    gradColor = tex2D(_GradientTex, abs(IN.gradientUV.x -   (0.5 +_OffsetX)) * _Scale * 2 );
                #endif

                gradColor.w = lerp(0, gradColor.w , _Opacity);
                color  =  BlendColors(gradColor, color);
                color.a = _AsOverlay == 1 ? alpha : min(alpha, gradColor.w);
                
                #ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                    clip (color.a - 0.001);
                #endif
                return color; 
            }
            ENDCG
        }
    }
}