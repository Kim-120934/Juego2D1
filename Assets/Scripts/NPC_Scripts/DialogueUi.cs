using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject continueIndicator;

    [Header("Settings")]
    [SerializeField] private float letterDelay = 0.05f;

    private string[] _lines;
    private int _currentLine = 0;
    private bool _isTyping = false;
    private bool _dialogueActive = false;
    public bool IsDialogueActive => _dialogueActive;
    private System.Action _onFinished;

    private void Awake()
    {
        instance = this;
        dialogueBox.SetActive(false);
    }

    public void StartDialogue(string[] lines, System.Action onFinished = null)
    {
        _lines = lines;
        _currentLine = 0;
        _onFinished = onFinished;
        _dialogueActive = true;
        dialogueBox.SetActive(true);
        continueIndicator.SetActive(false);
        StartCoroutine(TypeLine(_lines[_currentLine]));
    }

    public void NextLine()
    {
        if (!_dialogueActive) return;

        if (_isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = _lines[_currentLine];
            _isTyping = false;
            continueIndicator.SetActive(true);
            return;
        }

        _currentLine++;

        if (_currentLine < _lines.Length)
        {
            continueIndicator.SetActive(false);
            StartCoroutine(TypeLine(_lines[_currentLine]));
        }
        else
        {
            EndDialogue();
        }
    }

    private IEnumerator TypeLine(string line)
    {
        _isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(letterDelay);
        }

        _isTyping = false;
        continueIndicator.SetActive(true);
    }

    private void EndDialogue()
    {
        _dialogueActive = false;
        dialogueBox.SetActive(false);
        _onFinished?.Invoke();
    }
}