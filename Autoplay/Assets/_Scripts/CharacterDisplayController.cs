using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDisplayController : MonoBehaviour
{
    public List<GameObject> envCharacters = default;
    [SerializeField]
    private int numOnScreen;
    private float worldWidth = 18f;
    private float intervalDist;

    private TransitionEffectsController transitionEffects = default;

    private void Start()
    {
        transitionEffects = this.GetComponent<TransitionEffectsController>();
        // envCharacters = new List<GameObject>();
        numOnScreen = envCharacters.Count;
        SetDistance();
    }

    public int GetNumOnScreen()
    {
        return numOnScreen;
    }

    //public void SetNumOnScreen(int deltaNum)
    //{
    //    numOnScreen += deltaNum;
    //}

    public void IncreaseOneCharacter() // should be (int index)
    {
        if(numOnScreen < envCharacters.Count)
        {
            StartCoroutine(CharacterShowing(numOnScreen));
        }
    }

    public void DecreaseOneCharacter()
    {
        if(numOnScreen > 0)
        {
            StartCoroutine(CharacterLeaving(numOnScreen - 1));
        }
    }

    private IEnumerator CharacterShowing(int index)
    {
        numOnScreen++;
        SetDistance();
        UpdateCharacterPositions();
        StartCoroutine(transitionEffects.FadeIn(envCharacters[index], 1f));
        // envCharacters[numOnScreen].SetActive(true);
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator CharacterLeaving(int index)
    {
        StartCoroutine(transitionEffects.FadeOut(envCharacters[index], 1f));
        // envCharacters[numOnScreen].SetActive(true);
        yield return new WaitForSeconds(1f);
        numOnScreen--;
        SetDistance();
        UpdateCharacterPositions();
    }

    public void UpdateCharacterPositions()
    {
        if(envCharacters.Count > 0)
        {
            for (int i = 0; i < envCharacters.Count; i++)
            {
                Vector3 currentPosition = envCharacters[i].transform.position;
                Vector3 targetPosition = new Vector3(worldWidth / 2 - intervalDist * (i + 1), currentPosition.y, 0);
                // envCharacters[i].transform.position = new Vector3(worldWidth - intervalDist * i, 0, 0);
                envCharacters[i].GetComponent<SingleCharacterDisplayController>().SetTargetPos(targetPosition);
                envCharacters[i].GetComponent<SingleCharacterDisplayController>().StartLerping();
            }
        }
    }

    public void ResetDisplayPositions()
    {
        if (envCharacters.Count > 0)
        {
            for (int i = 0; i < envCharacters.Count; i++)
            {
                Vector3 currentPosition = envCharacters[i].transform.position;
                envCharacters[i].transform.position = new Vector3(worldWidth / 2 - intervalDist * (i + 1), currentPosition.y, 0);
            }
        }
    }

    private void SetDistance()
    {
        if(numOnScreen > 1)
        {
            intervalDist = worldWidth / (numOnScreen + 1);
        }
        else
        {
            intervalDist = 4.75f;
        }
    }  
}
