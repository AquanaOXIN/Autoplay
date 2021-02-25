using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDisplayController : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> envCharacters = default;
    [SerializeField]
    private int numOnScreen;
    [SerializeField]
    private int defaultSortingOrderBase = 5;

    private float worldWidth = 18f;
    private float intervalDist;
    private float skinWidth = 0.25f;

    private Hashtable alphaStatus = default; // 0 - transparent | 1 - fully | 2 - partial
    private TransitionEffectsController transitionEffects = default;

    private void Awake()
    {
        transitionEffects = this.GetComponent<TransitionEffectsController>();
        envCharacters = new List<GameObject>();
        alphaStatus = new Hashtable();
    }

    private void Start()
    {
        numOnScreen = 0;
        SetDistance();
    }

    public void InitializeEnvCharacterList(GameObject[] goArray)
    {
        foreach(GameObject go in goArray)
        {
            envCharacters.Add(go);
            transitionEffects.SetAlphaImmediately(go, 0);
            alphaStatus.Add(go, 0);
            UpdateNumOnScreen();
        }
    }

    private void UpdateNumOnScreen()
    {
        numOnScreen = 0;
        foreach(GameObject go in envCharacters)
        {
            if((int)alphaStatus[go] > 0 && go.activeInHierarchy)
            {
                numOnScreen++;
            }
        }
    }

    public int GetNumOnScreen()
    {
        return numOnScreen;
    }

    public void IncreaseOneCharacter(int index)
    {
        if(numOnScreen < envCharacters.Count)
        {
            StartCoroutine(CharacterShowing(index));
        }
    }

    public void DecreaseOneCharacter(int index)
    {
        if(numOnScreen > 0)
        {
            StartCoroutine(CharacterLeaving(index));
        }
    }

    private IEnumerator CharacterShowing(int index) // add Y offset
    {
        if((int)alphaStatus[envCharacters[index]] == 0)
        {
            envCharacters[index].SetActive(true);
            StartCoroutine(transitionEffects.FadeIn(envCharacters[index], 1f));
            alphaStatus[envCharacters[index]] = 1;         
            yield return new WaitForSeconds(0.1f);
            UpdateNumOnScreen();
            SetDistance();
            UpdateCharacterPositions();
        }
    }

    private IEnumerator CharacterLeaving(int index)
    {
        if((int)alphaStatus[envCharacters[index]] > 0)
        {
            StartCoroutine(transitionEffects.FadeOut(envCharacters[index], 1f));
            alphaStatus[envCharacters[index]] = 0;
            yield return new WaitForSeconds(1f);
            UpdateNumOnScreen();
            SetDistance();
            UpdateCharacterPositions();
        }
    }

    public void MoveCharacterPositionX(int _cur, int _tar)
    {
        StartCoroutine(SingleCharacterMovePositionX(_cur, _tar));
    }

    private IEnumerator SingleCharacterMovePositionX(int currPos, int tarPos) // int for positions
    {
        if(envCharacters[tarPos].activeInHierarchy && (int)alphaStatus[envCharacters[tarPos]] > 0) // if tarPos already has a character displayed
        {
            envCharacters[currPos].GetComponent<SpriteRenderer>().sortingOrder += 5;
            Vector3 destinate_position = envCharacters[tarPos].transform.position;
            Vector3 current_position = envCharacters[currPos].transform.position;
            Vector3 targetPosition = current_position;
            float destX = destinate_position.x;
            float curX = current_position.x;
            float dir = curX - destX; // > 0 = right | < 0 = left
            if (dir > 0)
            {
                targetPosition = new Vector3(destX + skinWidth, current_position.y, 0);
            }
            else if (dir < 0)
            {
                targetPosition = new Vector3(destX - skinWidth, current_position.y, 0);
            }
            envCharacters[currPos].GetComponent<SingleCharacterDisplayController>().SetTargetPos(targetPosition);
            envCharacters[currPos].GetComponent<SingleCharacterDisplayController>().SetLerpScale(0.5f);
            envCharacters[currPos].GetComponent<SingleCharacterDisplayController>().StartLerping();
            yield return new WaitForSeconds(1f);
            envCharacters[currPos].GetComponent<SingleCharacterDisplayController>().ResetLerpScale();
        }
        else if((int)alphaStatus[envCharacters[tarPos]] == 0)
        {
            // should be changed, sprites should be sign in autoplay manager
            envCharacters[tarPos].GetComponent<SpriteRenderer>().sprite = envCharacters[currPos].GetComponent<SpriteRenderer>().sprite;
            DecreaseOneCharacter(currPos);
            yield return new WaitForSeconds(1f);
            IncreaseOneCharacter(tarPos);
        }
    }

    public void UpdateCharacterPositions()
    {
        if(envCharacters.Count > 0)
        {
            int order = 0;
            for (int i = 0; i < envCharacters.Count ; i++)
            {
                if(envCharacters[i].activeInHierarchy && (int)alphaStatus[envCharacters[i]] > 0) 
                {
                    ResetSortingOrder(i);

                    Vector3 currentPosition = envCharacters[i].transform.position;
                    Vector3 targetPosition = new Vector3(worldWidth / 2 - intervalDist * (order + 1), currentPosition.y, 0);
                    envCharacters[i].GetComponent<SingleCharacterDisplayController>().SetTargetPos(targetPosition);
                    envCharacters[i].GetComponent<SingleCharacterDisplayController>().StartLerping();
                    order++;
                }
            }
        }
    }

    public void ResetDisplayPositions()
    {
        if (envCharacters.Count > 0)
        {
            int order = 0;
            for (int i = 0; i < envCharacters.Count; i++)
            {
                if(envCharacters[i].activeInHierarchy && (int)alphaStatus[envCharacters[i]] > 0)
                {
                    ResetSortingOrder(i);

                    Vector3 currentPosition = envCharacters[i].transform.position;
                    envCharacters[i].transform.position = new Vector3(worldWidth / 2 - intervalDist * (order + 1), currentPosition.y, 0);
                    order++;
                }
            }
        }
    }

    private void ResetSortingOrder(int index)
    {
        if (envCharacters[index].GetComponent<SpriteRenderer>().sortingOrder > defaultSortingOrderBase + 1)
        {
            envCharacters[index].GetComponent<SpriteRenderer>().sortingOrder = defaultSortingOrderBase;
        }
        if (index % 2 == 0)
        {
            envCharacters[index].GetComponent<SpriteRenderer>().sortingOrder = defaultSortingOrderBase + 1;
        }
        else
        {
            envCharacters[index].GetComponent<SpriteRenderer>().sortingOrder = defaultSortingOrderBase - 1;
        }
    }

    public void CharacterSpeaking(int index) // which POSITION's character is speaking
    {
        if(envCharacters[index].activeInHierarchy && (int)alphaStatus[envCharacters[index]] > 0)
        {
            transitionEffects.SetAlphaImmediately(envCharacters[index], 1f);
            alphaStatus[envCharacters[index]] = 1;

            for (int i = 0; i < envCharacters.Count; i++)
            {
                if (envCharacters[i].activeInHierarchy && (int)alphaStatus[envCharacters[i]] > 0 && (int)alphaStatus[envCharacters[i]] != 2) // if displayed and NOT already partially transparent
                {
                    if (i != index)
                    {
                        transitionEffects.SetAlphaImmediately(envCharacters[i], 0.3f);
                        alphaStatus[envCharacters[i]] = 2;
                    }
                }
            }
        }
    }

    public void ResetAlpha(int _alpha) // ONLY 0 or 1
    {
        for (int i = 0; i < envCharacters.Count; i++)
        {
            if (envCharacters[i].activeInHierarchy && (int)alphaStatus[envCharacters[i]] > 0) // if displayed and NOT transparent
            {
                transitionEffects.SetAlphaImmediately(envCharacters[i], _alpha);
                alphaStatus[envCharacters[i]] = _alpha; 
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
            intervalDist = 6f;
        }
    }  
}
