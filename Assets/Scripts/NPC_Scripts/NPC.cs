using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum UnlockType { Projectile, DoubleJump }

    [Header("Interacción")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private UnlockType unlockType;

    private bool _playerInRange = false;
    private bool _dialogueFinished = false;
    private int _currentLine = 0;
    private HollowKnightMovement _player;

    private void Start()
    {
        _player = FindObjectOfType<HollowKnightMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
            _currentLine = 0;
        }
    }

    public void Interact()
    {
        if (!_playerInRange || _dialogueFinished)
            return;

        if (_currentLine < dialogueLines.Length)
        {
            Debug.Log(dialogueLines[_currentLine]);
            _currentLine++;
        }

        if (_currentLine >= dialogueLines.Length)
        {
            _dialogueFinished = true;

            if (unlockType == UnlockType.Projectile)
                _player.UnlockProjectile();
            else if (unlockType == UnlockType.DoubleJump)
                _player.UnlockDoubleJump();
        }
    }
}
