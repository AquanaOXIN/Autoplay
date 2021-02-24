using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionEffectsController : MonoBehaviour
{
    // UI Transitions
    public IEnumerator UIFadeIn(GameObject go, float duration)
    {
        float t = 0;
        float startAlpha = go.GetComponent<CanvasRenderer>().GetAlpha();

        while (t < duration)
        {
            go.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(startAlpha, 1f, t / duration));

            t += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator UIFadeOut(GameObject go, float duration)
    {
        float t = 0;
        float startAlpha = go.GetComponent<CanvasRenderer>().GetAlpha();

        while (t < duration)
        {
            go.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(startAlpha, 0f, t / duration));

            t += Time.deltaTime;
            yield return null;
        }
    }

    // Environment Transitions
    public IEnumerator FadeIn(GameObject go, float duration)
    {
        float t = 0;
        Color startColor = go.GetComponent<SpriteRenderer>().color;
        float startAlpha = go.GetComponent<SpriteRenderer>().color.a;

        while (t < duration)
        {
            float tmpA = Mathf.Lerp(startAlpha, 1f, t / duration);
            go.GetComponent<SpriteRenderer>().color = new Color(startColor.r, startColor.g, startColor.b, tmpA);
            t += Time.deltaTime;
            yield return null;
            go.GetComponent<SpriteRenderer>().color = new Color(startColor.r, startColor.g, startColor.b, 1f);
        }
    }

    public IEnumerator FadeOut(GameObject go, float duration)
    {
        float t = 0;
        Color startColor = go.GetComponent<SpriteRenderer>().color;
        float startAlpha = go.GetComponent<SpriteRenderer>().color.a;

        while (t < duration)
        {
            float tmpA = Mathf.Lerp(startAlpha, 0f, t / duration);
            go.GetComponent<SpriteRenderer>().color = new Color(startColor.r, startColor.g, startColor.b, tmpA);
            t += Time.deltaTime;
            yield return null;
            go.GetComponent<SpriteRenderer>().color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        }
    }
}
