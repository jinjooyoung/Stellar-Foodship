using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JinJooYoung
{
    public class UIPopup : MonoBehaviour
    {
        [Header("Popup")]
        public CanvasGroup canvasGroup;

        public RectTransform popupBox;

        [Header("Background")]
        public Image dimmedImage;

        [Header("Settings")]
        public float dimmedAlpha = 0.7f;

        public float fadeDuration = 0.25f;

        public float scaleDuration = 0.35f;

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            StartCoroutine(AutoOpen());
        }

        IEnumerator AutoOpen()
        {
            yield return new WaitForSeconds(1f);

            Open();
        }

        public void Initialize()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            popupBox.localScale = Vector3.zero;

            if (dimmedImage != null)
            {
                Color color = dimmedImage.color;
                color.a = 0f;
                dimmedImage.color = color;
            }
        }

        public void Open()
        {
            MyTween.OpenPopup(
                canvasGroup,
                popupBox,
                dimmedImage,
                fadeDuration,
                scaleDuration,
                dimmedAlpha);
        }

        public void Close()
        {
            MyTween.ClosePopup(
                canvasGroup,
                popupBox,
                dimmedImage,
                fadeDuration,
                scaleDuration);
        }
    }
}