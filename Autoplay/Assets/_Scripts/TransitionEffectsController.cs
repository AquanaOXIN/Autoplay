using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionEffectsController : MonoBehaviour
{
    // UI Transitions
    public IEnumerator UIFadeIn(GameObject go, float duration)
    {
        float t = 0;
        // float startAlpha = go.GetComponent<CanvasRenderer>().GetAlpha();
        float startAlpha = 0f;

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
        // float startAlpha = go.GetComponent<CanvasRenderer>().GetAlpha();
        float startAlpha = 1f;

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

    public void SetAlphaImmediately(GameObject go, float alpha)
    {
        Color originalColor = go.GetComponent<SpriteRenderer>().color;
        go.GetComponent<SpriteRenderer>().color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
    }

    public void ResetSpriteRenderer(GameObject go)
    {
        go.GetComponent<SpriteRenderer>().sprite = null;
    }

    // Audio Transitions
    public IEnumerator AudioFadeIn(AudioSource source, float duration, float volumeCap)
    {
        float t = 0;
        source.volume = 0f;

        while (t < duration)
        {
            if(source.volume < volumeCap)
            {
                source.volume += volumeCap / (duration / Time.deltaTime);
            }
            else
            {
                source.volume = volumeCap;
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator AudioFadeOut(AudioSource source, float duration)
    {
        float t = 0;
        float startVolume = source.volume;

        while (t < duration)
        {
            if (source.volume > 0)
            {
                source.volume -= startVolume / (duration / Time.deltaTime);
            }
            else
            {
                source.volume = 0;
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    /// Visual Effects
    private IEnumerator NoiseTVEffect(GameObject go, float duration)
    {
        float t = 0;
        float startEdge = 0.25f;
        float endEdge = 0f;
        go.GetComponent<Image>().material.SetFloat("Step Edge", startEdge);

        while (t < duration)
        {
            go.GetComponent<CanvasRenderer>().SetAlpha(Mathf.Lerp(startEdge, endEdge, t / duration));

            t += Time.deltaTime;
            yield return null;
        }
    }
}
