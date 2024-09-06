Shader "Hidden/Shader/Pixelation"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }

        SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct appdata
        {
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        sampler2D _MainTex;

        int _PixelSize;
        float _WidthMod;
        float _HeightMod;

        v2f vert(appdata v)
        {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            o.uv = v.uv;
            return o;
        }

        fixed4 frag(v2f i) : SV_Target
        {
            float2 pixelatedUV = i.uv;
            pixelatedUV.x = floor(pixelatedUV.x * _ScreenParams.x / _PixelSize) * _WidthMod * _PixelSize;
            pixelatedUV.y = floor(pixelatedUV.y * _ScreenParams.y / _PixelSize) * _HeightMod * _PixelSize;
            fixed4 col = tex2D(_MainTex, pixelatedUV);
            return col;
        }

        ENDCG
    } 
        FallBack "Hidden/InternalErrorShader"
}
