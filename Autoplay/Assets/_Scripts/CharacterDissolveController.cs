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
        }

        if(isDissolving)
        {
            fade -= Time.deltaTime;
            if(fade <= 0f)
            {
                fade = 0f;
                // GameObject scatterObj = Instantiate(scatterPS, transform.position, Quaternion.identity);
                isDissolving = false;
            }
            material.SetFloat("_Fade", fade);
        }
    }
}
