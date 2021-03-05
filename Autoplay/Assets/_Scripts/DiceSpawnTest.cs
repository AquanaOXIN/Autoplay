using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceSpawnTest : MonoBehaviour
{
    public GameObject diePrefab;

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GenDice(diePrefab);
        }
    }

    private void GenDice(GameObject _go)
    {
        Color newColor = new Color(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), Random.Range(0.5f, 1f));
        GameObject dieIns = Instantiate(_go, transform.position, Quaternion.identity);
        dieIns.transform.SetParent(transform);
        dieIns.GetComponent<Image>().color = newColor;
        dieIns.GetComponent<RectTransform>().localScale *= Random.Range(0.001f, 0.01f);
        Destroy(dieIns, 10f);
    }
}
