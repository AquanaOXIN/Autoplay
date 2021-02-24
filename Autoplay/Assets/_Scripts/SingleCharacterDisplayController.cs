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
    private float baseSpeed = 5.0f;
    private float moveSpeed;

    private Vector3 targetPos;

    private void Start()
    {
        initLerp = false;
        isLerping = false;
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

    public void StartLerping()
    {
        isLerping = true;
    }

    private void LerpToPosition()
    {
        Vector3 startPos = this.transform.position;
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
        this.transform.position = tp;
    }
}
