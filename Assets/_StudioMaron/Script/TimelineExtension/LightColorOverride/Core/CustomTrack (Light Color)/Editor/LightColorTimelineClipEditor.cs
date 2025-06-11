using UnityEngine;
using UnityEngine.Timeline;
using UnityEditor.Timeline;
using System.Collections.Generic;

/// <summary>
/// Timeline clipの見た目をカラーグラデーションにするEditor拡張
/// </summary>
namespace StudioMaron
{
    [CustomTimelineEditor(typeof(LightColorClip))]
    public class LightColorTimelineClipEditor : ClipEditor
    {
        Dictionary<LightColorClip, Texture2D> _textures = new Dictionary<LightColorClip, Texture2D>();

        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            var tex = GetGradientTexture(clip);
            if (tex) GUI.DrawTexture(region.position, tex);
        }

        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            var options = base.GetClipOptions(clip);
            options.highlightColor = Color.clear;
            return options;
        }
        public override void OnClipChanged(TimelineClip clip)
        {
            GetGradientTexture(clip, true);
        }

        Texture2D GetGradientTexture(TimelineClip clip, bool update = false)
        {
            Texture2D tex = Texture2D.whiteTexture;

            var customClip = clip.asset as LightColorClip;
            if (!customClip) return tex;

            // Clipのグラデーションを取得
            var gradient = customClip.behaviour.gradientColor;

            // ClipがUse Referenceモードの場合は黒色にする
            if (customClip.behaviour.operationMode == LightColorParameter.OperationMode.UseReference)
            {
                //色の設定
                var colorKey = new GradientColorKey[2];
                colorKey[0].color = Color.black;
                colorKey[0].time = 0.0f;
                colorKey[1].color = Color.black;
                colorKey[1].time = 1.0f;

                //透明度の設定
                var alphaKey = new GradientAlphaKey[2];
                alphaKey[0].alpha = 0.5f;
                alphaKey[0].time = 0.0f;
                alphaKey[1].alpha = 0.5f;
                alphaKey[1].time = 1.0f;

                gradient = new Gradient();
                gradient.SetKeys(colorKey, alphaKey);
            }

            if (gradient == null) return tex;

            if (update)
            {
                _textures.Remove(customClip);
            }
            else
            {
                _textures.TryGetValue(customClip, out tex);
                if (tex) return tex;
            }

            var bi = (float)(clip.blendInDuration / clip.duration);
            var bo = (float)(clip.blendOutDuration / clip.duration);
            tex = new Texture2D(128, 9);

            for (int j = 0; j < tex.height; ++j)
            {
                for (int i = 0; i < tex.width; ++i)
                {
                    var t = (float)i / tex.width;
                    var color = gradient.Evaluate(t);
                    var p = (color.r + color.g + color.b) * color.a;
                    if (p > 3)
                    {
                        if((j != 1) && (j != 6))
                        {
                            color.r /= 0.5f * p;
                            color.g /= 0.5f * p;
                            color.b /= 0.5f * p;
                        }
                    }
                    if ((2 <= j)&&(j <= 5))
                    {
                        color.r = 0.1f;
                        color.g = 0.1f;
                        color.b = 0.1f;
                    }
                    if (bi > 0f) color.a *= Mathf.Min(t / bi, 1f);
                    if (bo > 0f) color.a *= Mathf.Min((1f - t) / bo, 1f);
                    tex.SetPixel(i, j, color);
                }
            }
            tex.Apply();
            _textures[customClip] = tex;

            return tex;
        }
    }
}
