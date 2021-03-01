using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public enum SlideType {fromLeft, fromRight, fromTop, fromButtom, toLeft, toRight, toTop, toButtom}

    private Vector3 defaultPosition = Vector3.zero;

    public void UISlide(GameObject _go, Canvas _mainCanvas, SlideType _type, LeanTweenType _easeType)
    {
        defaultPosition = _go.transform.localPosition;
        float ogX = defaultPosition.x;
        float ogY = defaultPosition.y;
        float distX = _mainCanvas.GetComponent<RectTransform>().rect.width / 2f;
        float distY = _mainCanvas.GetComponent<RectTransform>().rect.height / 2f;

        switch (_type)
        {
            case SlideType.fromLeft:
                _go.transform.localPosition += Vector3.right * (-distX);
                LeanTween.moveLocalX(_go, ogX, 1f).setEase(_easeType);
                break;
            case SlideType.fromRight:
                _go.transform.localPosition += Vector3.right * (distX);
                LeanTween.moveLocalX(_go, ogX, 1f).setEase(_easeType);
                break;
            case SlideType.fromTop:
                _go.transform.localPosition += Vector3.up * (distY);
                LeanTween.moveLocalY(_go, ogY, 1f).setEase(_easeType);
                break;
            case SlideType.fromButtom:
                _go.transform.localPosition += Vector3.up * (-distY);
                LeanTween.moveLocalY(_go, ogY, 1f).setEase(_easeType);
                break;
            case SlideType.toLeft:
                LeanTween.moveLocalX(_go, ogX - distX, 1f).setEase(_easeType);
                // StartCoroutine(DelayedResetUIPosition(_go, defaultPosition, 1.1f));
                break;
            case SlideType.toRight:
                LeanTween.moveLocalX(_go, ogX + distX, 1f).setEase(_easeType);
                // StartCoroutine(DelayedResetUIPosition(_go, defaultPosition, 1.1f));
                break;
            case SlideType.toTop:
                LeanTween.moveLocalY(_go, ogY + distY, 1f).setEase(_easeType);
                // StartCoroutine(DelayedResetUIPosition(_go, defaultPosition, 1.1f));
                break;
            case SlideType.toButtom:
                LeanTween.moveLocalY(_go, ogY + distY, 1f).setEase(_easeType);
                // StartCoroutine(DelayedResetUIPosition(_go, defaultPosition, 1.1f));
                break;
            default:
                break;
        }
    }

    public IEnumerator DelayedResetUIPosition(GameObject _go, float _delayTime)
    {
        yield return new WaitForSeconds(_delayTime);
        _go.transform.localPosition = defaultPosition;
        _go.SetActive(false);
    }
}
