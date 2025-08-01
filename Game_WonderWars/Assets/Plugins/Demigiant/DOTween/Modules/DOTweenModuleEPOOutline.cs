using UnityEngine;
using UnityEngine.UI;


#if true || EPO_DOTWEEN // MODULE_MARKER

#if EPO_DOTWEEN && EPO_OUTLINE
using EPOOutline;
using DG.Tweening.Plugins.Options;
using DG.Tweening;
using DG.Tweening.Core;

namespace DG.Tweening
{
    public static class DOTweenModuleEPOOutline
    {
        public static int DOKill(this SerializedPass target, bool complete)
        {
            return DOTween.Kill(target, complete);
        }

        public static TweenerCore<float, float, FloatOptions> DOFloat(this SerializedPass target, string propertyName, float endValue, float duration)
        {
            var tweener = DOTween.To(() => target.GetFloat(propertyName), x => target.SetFloat(propertyName, x), endValue, duration);
            tweener.SetOptions(true).SetTarget(target);
            return tweener;
        }

        public static TweenerCore<Color, Color, ColorOptions> DOFade(this SerializedPass target, string propertyName, float endValue, float duration)
        {
            var tweener = DOTween.ToAlpha(() => target.GetColor(propertyName), x => target.SetColor(propertyName, x), endValue, duration);
            tweener.SetOptions(true).SetTarget(target);
            return tweener;
        }

        public static TweenerCore<Color, Color, ColorOptions> DOColor(this SerializedPass target, string propertyName, Color endValue, float duration)
        {
            var tweener = DOTween.To(() => target.GetColor(propertyName), x => target.SetColor(propertyName, x), endValue, duration);
            tweener.SetOptions(false).SetTarget(target);
            return tweener;
        }

        public static TweenerCore<Vector4, Vector4, VectorOptions> DOVector(this SerializedPass target, string propertyName, Vector4 endValue, float duration)
        {
            var tweener = DOTween.To(() => target.GetVector(propertyName), x => target.SetVector(propertyName, x), endValue, duration);
            tweener.SetOptions(false).SetTarget(target);
            return tweener;
        }

        public static TweenerCore<float, float, FloatOptions> DOFloat(this SerializedPass target, int propertyId, float endValue, float duration)
        {
            var tweener = DOTween.To(() => target.GetFloat(propertyId), x => target.SetFloat(propertyId, x), endValue, duration);
            tweener.SetOptions(true).SetTarget(target);
            return tweener;
        }

        public static TweenerCore<Color, Color, ColorOptions> DOFade(this SerializedPass target, int propertyId, float endValue, float duration)
        {
            var tweener = DOTween.ToAlpha(() => target.GetColor(propertyId), x => target.SetColor(propertyId, x), endValue, duration);
            tweener.SetOptions(true).SetTarget(target);
            return tweener;
        }

        public static TweenerCore<Color, Color, ColorOptions> DOColor(this SerializedPass target, int propertyId, Color endValue, float duration)
        {
            var tweener = DOTween.To(() => target.GetColor(propertyId), x => target.SetColor(propertyId, x), endValue, duration);
            tweener.SetOptions(false).SetTarget(target);
            return tweener;
        }

        public static TweenerCore<Vector4, Vector4, VectorOptions> DOVector(this SerializedPass target, int propertyId, Vector4 endValue, float duration)
        {
            var tweener = DOTween.To(() => target.GetVector(propertyId), x => target.SetVector(propertyId, x), endValue, duration);
            tweener.SetOptions(false).SetTarget(target);
            return tweener;
        }

        public static int DOKill(this Outlinable.OutlineProperties target, bool complete = false)
        {
            return DOTween.Kill(target, complete);
        }

        public static int DOKill(this Outliner target, bool complete = false)
        {
            return DOTween.Kill(target, complete);
        }

        /// <summary>
        /// Controls the alpha (transparency) of the outline
        /// </summary>
        public static TweenerCore<Color, Color, ColorOptions> DOFade(this Outlinable.OutlineProperties target, float endValue, float duration)
        {
            var tweener = DOTween.ToAlpha(() => target.Color, x => target.Color = x, endValue, duration);
            tweener.SetOptions(true).SetTarget(target);
            return tweener;
        }

        /// <summary>
        /// Controls the color of the outline
        /// </summary>
        public static TweenerCore<Color, Color, ColorOptions> DOColor(this Outlinable.OutlineProperties target, Color endValue, float duration)
        {
            var tweener = DOTween.To(() => target.Color, x => target.Color = x, endValue, duration);
            tweener.SetOptions(false).SetTarget(target);
            return tweener;
        }

        /// <summary>
        /// Controls the amount of blur applied to the outline
        /// </summary>
        public static TweenerCore<float, float, FloatOptions> DOBlurShift(this Outlinable.OutlineProperties target, float endValue, float duration, bool snapping = false)
        {
            var tweener = DOTween.To(() => target.BlurShift, x => target.BlurShift = x, endValue, duration);
            tweener.SetOptions(snapping).SetTarget(target);
            return tweener;
        }

        /// <summary>
        /// Controls the amount of blur applied to the outline
        /// </summary>
        public static TweenerCore<float, float, FloatOptions> DOBlurShift(this Outliner target, float endValue, float duration, bool snapping = false)
        {
            var tweener = DOTween.To(() => target.BlurShift, x => target.BlurShift = x, endValue, duration);
            tweener.SetOptions(snapping).SetTarget(target);
            return tweener;
        }

        /// <summary>
        /// Controls the amount of dilation applied to the outline
        /// </summary>
        public static TweenerCore<float, float, FloatOptions> DODilateShift(this Outlinable.OutlineProperties target, float endValue, float duration, bool snapping = false)
        {
            var tweener = DOTween.To(() => target.DilateShift, x => target.DilateShift = x, endValue, duration);
            tweener.SetOptions(snapping).SetTarget(target);
            return tweener;
        }

        // Removed DODilateShift for Outline, as Outline does not have a DODilateShift property in UnityEngine.UI
        // If you need to tween a property on Outline, implement it here with the correct property.

    }
}
#endif
#endif
