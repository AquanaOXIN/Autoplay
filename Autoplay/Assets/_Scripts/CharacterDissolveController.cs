using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDissolveController : MonoBehaviour
{
    public GameObject scatterPS = default;

    Material material = default;
    bool isDissolving = false;

    float fade = 1f;

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

        if(isDissolving)
        {
            fade -= Time.deltaTime;
            if(fade <= 0f)
            {
                fade = 0f; 
                isDissolving = false;
            }
            material.SetFloat("_Fade", fade);
        }
    }

    private IEnumerator DelayedInstantiate(GameObject _go, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        GameObject go = Instantiate(_go, transform.position, Quaternion.identity);
    }
}
