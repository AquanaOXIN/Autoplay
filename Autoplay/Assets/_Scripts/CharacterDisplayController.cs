using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDisplayController : MonoBehaviour
{
    public GameObject inputFieldShowObj;
    public GameObject inputFieldHideObj;
    private int inputShow;
    private int inputHide;

    public float skinWidth = 0.25f;

    public List<GameObject> envCharacters = default;
    [SerializeField]
    private int numOnScreen;
    private float worldWidth = 18f;
    private float intervalDist;

    private Hashtable alphaStatus = default; 

    private TransitionEffectsController transitionEffects = default;

    private void Start()
    {
        transitionEffects = this.GetComponent<TransitionEffectsController>();
        envCharacters = new List<GameObject>();
        alphaStatus = new Hashtable();
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
        foreach(GameObject go in envCharacters)
        {
            if((int)alphaStatus[go] == 1 && go.activeInHierarchy)
            {
                numOnScreen++;
            }
        }
    }

    public int GetNumOnScreen()
    {
        return numOnScreen;
    }

    public void IncreaseOneCharacter(int index) // should be (int index)
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

    private IEnumerator CharacterShowing(int index)
    {
        if((int)alphaStatus[envCharacters[index]] == 0)
        {
            numOnScreen++;
            SetDistance();
            envCharacters[index].SetActive(true);
            StartCoroutine(transitionEffects.FadeIn(envCharacters[index], 1f));
            alphaStatus[envCharacters[index]] = 1;
            UpdateCharacterPositions();
            yield return new WaitForSeconds(1f);           
        }
    }

    private IEnumerator CharacterLeaving(int index)
    {
        if((int)alphaStatus[envCharacters[index]] == 1)
        {
            StartCoroutine(transitionEffects.FadeOut(envCharacters[index], 1f));
            alphaStatus[envCharacters[index]] = 0;
            yield return new WaitForSeconds(1f);
            envCharacters[index].SetActive(false); // may need to change 
            numOnScreen--;
            SetDistance();
            UpdateCharacterPositions();
        }
    }

    public void SingleCharacterPositionXSlide(int my_Char, int other_Char)
    {
        Vector3 other_position = envCharacters[other_Char].transform.position;
        Vector3 my_position = envCharacters[my_Char].transform.position;
        Vector3 targetPosition = my_position;
        float otherX = other_position.x;
        float myX = my_position.x;
        float dir = myX - otherX; // > 0 = right | < 0 = left
        if (dir > 0)
        {
            targetPosition = new Vector3(otherX + skinWidth, my_position.y, 0);
        }
        else if (dir < 0)
        {
            targetPosition = new Vector3(otherX - skinWidth, my_position.y, 0);
        }
        envCharacters[my_Char].GetComponent<SingleCharacterDisplayController>().SetTargetPos(targetPosition);
        envCharacters[my_Char].GetComponent<SingleCharacterDisplayController>().StartLerping();
    }

    public void UpdateCharacterPositions()
    {
        if(envCharacters.Count > 0)
        {
            int order = 0;
            for (int i = 0; i < envCharacters.Count ; i++)
            {
                if(envCharacters[i].activeInHierarchy)
                {
                    Vector3 currentPosition = envCharacters[i].transform.position;
                    Vector3 targetPosition = new Vector3(worldWidth / 2 - intervalDist * (order + 1), currentPosition.y, 0);
                    // envCharacters[i].transform.position = new Vector3(worldWidth - intervalDist * i, 0, 0);
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
                if(envCharacters[i].activeInHierarchy)
                {
                    Vector3 currentPosition = envCharacters[i].transform.position;
                    envCharacters[i].transform.position = new Vector3(worldWidth / 2 - intervalDist * (order + 1), currentPosition.y, 0);
                    order++;
                }
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

    public void GetShowInput()
    {
        string inputString = inputFieldShowObj.gameObject.GetComponent<TMPro.TMP_InputField>().text;
        // Debug.Log(inputString);
        int number;
        bool success = int.TryParse(inputString, out number);
        if(success)
        {
            IncreaseOneCharacter(number);
        }
    }

    public void GetHideInput()
    {
        string inputString = inputFieldHideObj.gameObject.GetComponent<TMPro.TMP_InputField>().text;
        // Debug.Log(inputString);
        int number;
        bool success = int.TryParse(inputString, out number);
        if (success)
        {
            DecreaseOneCharacter(number);
        }
    }
}
