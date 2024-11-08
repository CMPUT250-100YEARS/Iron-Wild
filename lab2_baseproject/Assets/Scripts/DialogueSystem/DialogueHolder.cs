using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace DialogueSystem
{
    
    public class DialogueHolder : MonoBehaviour
    {
        [SerializeField] private string nextSceneName; // The name of the scene to load next

        private void Awake()
        {
            StartCoroutine(dialogueSequence());
           
        }

        private IEnumerator dialogueSequence()
        {
            DeactivateAllLines();

            for (int i=0; i < transform.childCount; i++)
            {
                //***
                GameObject lineObject = transform.GetChild(i).gameObject;
                lineObject.SetActive(true); //current line active

                DialogueBaseClass line = lineObject.GetComponent<DialogueBaseClass>();
                yield return new WaitUntil(() => line.finished);

                lineObject.SetActive(false); //line deactivated when finished

                //transform.GetChild(i).gameObject.SetActive(true);
                //yield return new WaitUntil(() => transform.GetChild(i).GetComponent<DialogueLine>().finished);
            }
            yield return new WaitUntil(() => Input.GetMouseButton(0));
            SceneManager.LoadScene(nextSceneName);
        }

        private void DeactivateAllLines()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

}
