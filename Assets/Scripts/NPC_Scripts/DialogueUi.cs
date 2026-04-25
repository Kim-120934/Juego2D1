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
    [SerializeField] private float fadeDuration = 0.3f;

    private CanvasGroup _canvasGroup;

    private string[] _lines;
    private int _currentLine = 0;
    private bool _isTyping = false;
    private bool _dialogueActive = false;
    public bool IsDialogueActive => _dialogueActive;
    private System.Action _onFinished;

    [Header("HUD")]
    [SerializeField] private CanvasGroup hudCanvasGroup;

    private void Awake()
    {
        instance = this;
        _canvasGroup = dialogueBox.GetComponent<CanvasGroup>();
        dialogueBox.SetActive(false);
    }

    public void StartDialogue(string[] lines, System.Action onFinished = null)
    {
        _lines = lines;
        _currentLine = 0;
        _onFinished = onFinished;
        _dialogueActive = true;
        continueIndicator.SetActive(false);
        StartCoroutine(FadeInAndType());
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
            AudioManager.instance.PlaySFX(AudioManager.instance.dialogueBlipSFX);
            yield return new WaitForSeconds(letterDelay);
        }

        _isTyping = false;
        continueIndicator.SetActive(true);
    }

    private void EndDialogue()
    {
        _dialogueActive = false;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeInAndType()
    {
        dialogueBox.SetActive(true);
        _canvasGroup.alpha = 0f;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = timer / fadeDuration;
            if (hudCanvasGroup != null)
                hudCanvasGroup.alpha = 1f - (timer / fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = 1f;
        if (hudCanvasGroup != null)
            hudCanvasGroup.alpha = 0f;
        StartCoroutine(TypeLine(_lines[_currentLine]));
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            _canvasGroup.alpha = 1f - (timer / fadeDuration);
            if (hudCanvasGroup != null)
                hudCanvasGroup.alpha = timer / fadeDuration;
            yield return null;
        }
        _canvasGroup.alpha = 0f;
        if (hudCanvasGroup != null)
            hudCanvasGroup.alpha = 1f;
        dialogueBox.SetActive(false);
        _onFinished?.Invoke();
    }
}