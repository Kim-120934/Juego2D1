using UnityEngine;

public class NPC : MonoBehaviour
{
    public enum UnlockType { Projectile, DoubleJump }

    [Header("Interacción")]
    [SerializeField] private string[] dialogueLines;
    [SerializeField] private UnlockType unlockType;

    private bool _playerInRange = false;
    private bool _dialogueFinished = false;
    private HollowKnightMovement _player;

    private void Start()
    {
        _player = FindFirstObjectByType<HollowKnightMovement>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            _playerInRange = false;
    }

    public void Interact()
    {
        if (!_playerInRange || _dialogueFinished)
            return;

        DialogueUI.instance.StartDialogue(dialogueLines, OnDialogueFinished);
    }

    private void OnDialogueFinished()
    {
        _dialogueFinished = true;

        if (unlockType == UnlockType.Projectile)
            _player.UnlockProjectile();
        else if (unlockType == UnlockType.DoubleJump)
            _player.UnlockDoubleJump();
    }
}