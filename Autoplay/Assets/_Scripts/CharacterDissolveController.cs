using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDissolveController : MonoBehaviour
{
    public GameObject scatterPS = default;

    Material material = default;
    bool isDissolving = false;

    float fade = 1f;
    // float cutOffHeight = 7.8f;

    private GameObject scatterIns = default;
    public Transform startPos = default;

    private void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            isDissolving = true;
            StartCoroutine(DelayedInstantiate(scatterPS, 0.15f));
        }

        if (isDissolving)
        {
            fade -= Time.deltaTime;
            if (fade <= 0f)
            {
                fade = 0f;
                isDissolving = false;
            }
            material.SetFloat("_Fade", fade);
        }

        //if(isDissolving)
        //{
        //    cutOffHeight -= Time.deltaTime;
        //    if(cutOffHeight < -7f)
        //    {
        //        cutOffHeight = -10.0f;
        //        isDissolving = false;
        //        Destroy(scatterIns.gameObject, 0.5f);
        //    }

        //    material.SetFloat("_CutoffHeight", cutOffHeight);
        //    if(scatterIns)
        //    {
        //        scatterIns.transform.position += Vector3.up * Time.deltaTime * 0.65f;
        //    }

        //}
    }

    private IEnumerator DelayedInstantiate(GameObject _go, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        scatterIns = Instantiate(_go, startPos.position, Quaternion.identity);
    }
}
