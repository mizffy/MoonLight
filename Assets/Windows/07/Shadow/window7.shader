Shader "Custom/window7_ShadowOnly"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        // 表示されないようにする（ColorMask 0）+ 通常描画はスキップ
        Pass
        {
            Name "Invisible"
            Tags { "LightMode" = "ForwardBase" }
            ZWrite Off
            ColorMask 0
        }

        // ShadowCaster パスだけ実行（＝影は落ちる）
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            ZWrite On
            ColorMask 0
        }

        // Shadow only シェーダーには surface shader を書かない
        // もしくは使いたければ `#pragma skiplighting` で削れるが今回は不要
    }
    FallBack Off
}
