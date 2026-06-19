using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace JinJooYoung
{
    public static class MyTween
    {
        //==============================
        // Fade
        //==============================

        public static Tween Fade(CanvasGroup group, float targetAlpha, float duration, Ease ease = Ease.Linear)
        {
            group.DOKill();

            return group
                .DOFade(targetAlpha, duration)
                .SetEase(ease);
        }

        public static Tween Fade(Image image, float targetAlpha, float duration, Ease ease = Ease.Linear)
        {
            image.DOKill();

            Color color = image.color;
            color.a = targetAlpha;

            return image
                .DOFade(targetAlpha, duration)
                .SetEase(ease);
        }

        //==============================
        // Scale
        //==============================

        public static Tween Scale(Transform target, Vector3 scale, float duration, Ease ease = Ease.OutBack)
        {
            target.DOKill();

            return target
                .DOScale(scale, duration)
                .SetEase(ease);
        }

        public static Tween PunchScale(Transform target, float strength = 0.2f, float duration = 0.3f)
        {
            target.DOKill();

            return target.DOPunchScale(
                Vector3.one * strength,
                duration,
                10,
                1f);
        }

        //==============================
        // Popup
        //==============================

        public static Sequence OpenPopup(
            CanvasGroup canvasGroup,
            RectTransform popupBox,
            Image dimmed,
            float fadeDuration = 0.25f,
            float scaleDuration = 0.35f,
            float dimmedAlpha = 0.7f)
        {
            canvasGroup.DOKill();
            popupBox.DOKill();

            if (dimmed != null)
                dimmed.DOKill();

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            popupBox.localScale = Vector3.zero;

            Sequence seq = DOTween.Sequence();

            if (dimmed != null)
            {
                seq.Join(
                    dimmed.DOFade(
                        dimmedAlpha,
                        fadeDuration));
            }

            seq.Join(
                canvasGroup.DOFade(
                    1f,
                    fadeDuration));

            seq.Join(
                popupBox
                .DOScale(
                    Vector3.one,
                    scaleDuration)
                .SetEase(Ease.OutBack));

            return seq;
        }

        public static Sequence ClosePopup(
            CanvasGroup canvasGroup,
            RectTransform popupBox,
            Image dimmed,
            float fadeDuration = 0.2f,
            float scaleDuration = 0.2f)
        {
            canvasGroup.DOKill();
            popupBox.DOKill();

            if (dimmed != null)
                dimmed.DOKill();

            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            Sequence seq = DOTween.Sequence();

            if (dimmed != null)
            {
                seq.Join(
                    dimmed.DOFade(
                        0f,
                        fadeDuration));
            }

            seq.Join(
                canvasGroup.DOFade(
                    0f,
                    fadeDuration));

            seq.Join(
                popupBox
                .DOScale(
                    Vector3.zero,
                    scaleDuration)
                .SetEase(Ease.InBack));

            return seq;
        }

        //==============================
        // Slide
        //==============================

        public static Tween Slide(RectTransform target, Vector2 startPos, Vector2 endPos, float duration, Ease ease = Ease.OutCubic)
        {
            target.DOKill();

            target.anchoredPosition = startPos;

            return target
                .DOAnchorPos(endPos, duration)
                .SetEase(ease);
        }

        //==============================
        // Move
        //==============================

        public static Tween MoveTo(RectTransform rect, Vector2 targetPos, float duration, Ease ease = Ease.OutCubic)
        {
            rect.DOKill();

            return rect
                .DOAnchorPos(targetPos, duration)
                .SetEase(ease);
        }

        public static Tween RotateTo(RectTransform rect, float targetZ, float duration, Ease ease = Ease.OutCubic)
        {
            rect.DOKill();

            return rect
                .DOLocalRotate(
                    new Vector3(0, 0, targetZ),
                    duration)
                .SetEase(ease);
        }

        //==============================
        // Dimmed
        //==============================

        public static Tween Dimmed(Image image, float targetAlpha, float duration)
        {
            image.DOKill();

            return image
                .DOFade(targetAlpha, duration)
                .SetEase(Ease.Linear);
        }
    }
}