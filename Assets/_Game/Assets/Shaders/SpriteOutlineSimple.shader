Shader "Custom/SpriteOutlineSimple"
{
     Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        [Toggle] _OutlineEnabled ("Outline Enabled", Int) = 1
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineEnabled;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;

                if (_OutlineEnabled == true)
                {
                    if (c.a == 0) {
                        const float outline_width = 0.06f;
                        float step = outline_width * 0.1;
                        
                        if (tex2D(_MainTex, IN.texcoord + float2(step, 0)).a > 0 ||
                            tex2D(_MainTex, IN.texcoord - float2(step, 0)).a > 0 ||
                            tex2D(_MainTex, IN.texcoord + float2(0, step)).a > 0 ||
                            tex2D(_MainTex, IN.texcoord - float2(0, step)).a > 0)
                        {
                            c.rgb = _OutlineColor.rgb;
                            c.a = _OutlineColor.a;
                        }
                    }
                }
                
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}
