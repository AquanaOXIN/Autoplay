using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleCharacterDisplayController : MonoBehaviour
{
    public int displayIndex;

    private float worldWidth = 18f;
    private float intervalDist;
    private int numOnScreen;

    private bool initLerp;
    private bool isLerping;
    private float t;
    [SerializeField]
    private float baseSpeed = 1f, moveCurveScale = 0.01f;
    private float moveSpeed;
    private float defaultMoveCurveScale = 0.01f;
    [SerializeField]
    private AnimationCurve moveCurve = default;
    private Vector3 targetPos;

    private void Start()
    {
        initLerp = false;
        isLerping = false;
        moveCurveScale = 0.01f;
    }

    private void Update()
    {
        if(isLerping)
        {
            LerpToPosition();
        }
    }

    public void SetTargetPos(Vector3 _pos)
    {
        targetPos = _pos;
    }

    public void SetLerpScale(float _scale)
    {
        moveCurveScale = _scale;
    }

    public void ResetLerpScale()
    {
        moveCurveScale = defaultMoveCurveScale;
    }


    public void StartLerping()
    {
        isLerping = true;
    }

    private void LerpToPosition()
    {
        Vector3 startPos = this.transform.position;
        // float dist = Vector3.Distance(targetPos, startPos);
        if (!initLerp)
        {
            t = 0;
            moveSpeed = baseSpeed;
            initLerp = true;
        }

        t += Time.deltaTime * moveSpeed;
        if (t > 1)
        {
            t = 1;
            isLerping = false;
            initLerp = false;
        }
        Vector3 tp = Vector3.Lerp(startPos, targetPos, t);
        float curveY = moveCurve.Evaluate(t);
        curveY *= moveCurveScale;
        tp.y += curveY;
        this.transform.position = tp;
    }
}
