using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;
using System.Threading.Tasks;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    private Queue<string> sentences;

    [SerializeField]
    private float animationSpeed = 3;
    [SerializeField]
    private float closedPosition;
    [SerializeField]
    private float openedPosition;
    [Header("ObjectSetup")]
    [SerializeField]
    private GameObject dialogueBox = null;
    [SerializeField]
    private Button button = null;

    [Header("Runtime Variables")]
    [SerializeField]
    private TextMeshProUGUI characterName = null;
    [SerializeField]
    private TextMeshProUGUI text = null;


    public UnityEvent startDialogue;
    public UnityEvent endedDialogue;

    public static DialogueManager Instance{get{return instance;} }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        instance = this;
        sentences = new Queue<string>();      
    }

    public void StartDialogue(Dialogue dialogue)
    {
        OpenDialogBox();
        //GameManager.Instance.pauseGame();
        startDialogue.Invoke();

        //EventSystem.current.SetSelectedGameObject(button.gameObject);
        sentences.Clear();
        characterName.text = dialogue.name;
        foreach (var sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        displayNextSentence();
    }

    public void displayNextSentence()
    {
        //Audio sound - validate
        if (sentences.Count == 0)
        {
            endDialogue();
        }
        else
        {
            string sentence = sentences.Dequeue();
            displaySentence(sentence);
        }
        //TODO: UI controller to call nextsentence
    }

    private async void displaySentence(string sentence)
    {
        string displayed = "";
        foreach (var letter in sentence)
        {
            //Audio - Sound letter typing
            displayed += letter;
            text.text = displayed;
            await Task.Delay(50);
        }   
    }

    private void endDialogue()
    {
        endedDialogue.Invoke();
        EventSystem.current.SetSelectedGameObject(null);
        CloseDialogBox();
    }

    private void OpenDialogBox()
    {
        LeanTween.moveY(dialogueBox
            , openedPosition
            , 1 / animationSpeed).setEase(LeanTweenType.easeInOutBack);
    }
    private void CloseDialogBox()
    {
        LeanTween.moveY(dialogueBox
    , closedPosition
    , 1 / animationSpeed).setEase(LeanTweenType.easeInOutBack);
    }
}
