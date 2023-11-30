using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonStyle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Text buttonText; // 按钮的文字
    public RectTransform rectangleTransform;
    public Image image;
    public float expandTime = 0.1f; // 展开所需的时间
    private Color normalColor = Color.gray;
    private Color hoverColor = Color.white;
    private Color clickedColor = new Color(0.9568628f, 0.5686275f, 0.1647059f, 1f);

    private bool isTransitioning = false;

    void Start()
    {
        buttonText.color = normalColor;
        rectangleTransform.localScale = new Vector3(1f, 0f, 1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonText.color = hoverColor;
        StartCoroutine(ExpandRectangle(1f, expandTime, normalColor, hoverColor));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonText.color = normalColor;
        StartCoroutine(ExpandRectangle(0f, expandTime, hoverColor, normalColor)); // 收缩长方形
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        buttonText.color = clickedColor;
        image.color = clickedColor;
        if (!isTransitioning)
        {
            StartCoroutine(ChangeColorCoroutine());
        }
    }

    IEnumerator ChangeColorCoroutine()
    {
        float transitionTime = 0.1f;
        isTransitioning = true;
        yield return new WaitForSeconds(0.1f);

        float t = 0;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            image.color = Color.Lerp(clickedColor, hoverColor, t / transitionTime);
            buttonText.color = Color.Lerp(clickedColor, hoverColor, t / transitionTime);
            yield return null;
        }

        isTransitioning = false;
    }

    IEnumerator ExpandRectangle(float scale, float duration, Color fromColor, Color toColor)
    {
        float time = 0;
        float initialWidth = 1f - scale;
        float fromBright = fromColor.r;
        float toBright = toColor.r;
        while (time < duration)
        {
            time += Time.deltaTime;
            float width = Mathf.Lerp(initialWidth, scale, time / duration);
            float bright = Mathf.Lerp(fromBright, toBright, time / duration);
            rectangleTransform.localScale = new Vector3(1f, width, 1f);
            image.color = new Color(bright, bright, bright, 1f);
            buttonText.color = new Color(bright, bright, bright, 1f);
            yield return null;
        }
        rectangleTransform.localScale = new Vector3(1f, scale, 1f);
        image.color = toColor;
        buttonText.color = toColor;
    }
}
