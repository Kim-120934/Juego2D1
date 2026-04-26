using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SlotMenu : MonoBehaviour
{
    [Header("Name Input")]
    [SerializeField] private GameObject nameInputPanel;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button confirmButton;

    private int _pendingSlot = -1;

    [Header("Panels")]
    [SerializeField] private GameObject slotsPanel;
    [SerializeField] private GameObject mainButtonsPanel;

    [Header("Slot Buttons")]
    [SerializeField] private Button slot1Button;
    [SerializeField] private Button slot2Button;
    [SerializeField] private Button slot3Button;
    [SerializeField] private Button slot4Button;

    [Header("Slot Labels")]
    [SerializeField] private TextMeshProUGUI slot1Text;
    [SerializeField] private TextMeshProUGUI slot2Text;
    [SerializeField] private TextMeshProUGUI slot3Text;
    [SerializeField] private TextMeshProUGUI slot4Text;

    [Header("Delete Mode")]
    [SerializeField] private Button deleteButton;
    private bool _deleteMode = false;

    private int _selectedSlot = -1;

    private void Start()
    {
        Invoke("UpdateSlotLabels", 0.5f);
    }

    private void UpdateSlotLabels()
    {
        UpdateLabel(slot1Text, 1);
        UpdateLabel(slot2Text, 2);
        UpdateLabel(slot3Text, 3);
        UpdateLabel(slot4Text, 4);
    }

    private void UpdateLabel(TextMeshProUGUI label, int slot)
    {
        bool exists = SaveManager.instance != null && SaveManager.instance.SlotExists(slot);
        Debug.Log($"Slot {slot} existe: {exists}");

        if (exists)
        {
            SaveData data = SaveManager.instance.LoadGame(slot);
            Debug.Log($"Slot {slot} nombre: {(data != null ? data.playerName : "NULL")}");
            string name = data != null ? data.playerName : "Jugador";
            label.text = "Slot " + slot + " - " + name;
        }
        else
            label.text = "Slot " + slot + " - Nueva Partida";
    }

    public void OpenSlots()
    {
        slotsPanel.SetActive(true);
        mainButtonsPanel.SetActive(false);
        UpdateSlotLabels();
    }

    public void CloseSlots()
    {
        slotsPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);
    }

    public void SelectSlot(int slot)
    {
        if (_deleteMode)
        {
            SaveManager.instance.DeleteSave(slot);
            UpdateSlotLabels();
            _deleteMode = false;
            if (deleteButton != null)
                deleteButton.GetComponentInChildren<TextMeshProUGUI>().text = "Borrar Partida";
            return;
        }

        if (SaveManager.instance != null && SaveManager.instance.SlotExists(slot))
        {
            LoadGame(slot);
        }
        else
        {
            _pendingSlot = slot;
            nameInputPanel.SetActive(true);
            nameInput.text = "";
            nameInput.Select();
        }
    }
    public void ConfirmName()
    {
        if (string.IsNullOrEmpty(nameInput.text))
            return;

        Debug.Log("Nombre confirmado: " + nameInput.text);
        nameInputPanel.SetActive(false);
        SaveManager.instance.SetPendingName(nameInput.text);
        StartNewGame(_pendingSlot);
    }
    private void StartNewGame(int slot)
    {
        _selectedSlot = slot;
        SetCurrentSlotStatic(slot);
        SceneManager.LoadScene(1);
    }

    private void LoadGame(int slot)
    {
        SaveData data = SaveManager.instance.LoadGame(slot);
        if (data == null) return;

        SetCurrentSlotStatic(slot);
        SceneManager.LoadScene(data.sceneName);
    }

    public void ToggleDeleteMode()
    {
        _deleteMode = !_deleteMode;
        if (deleteButton != null)
            deleteButton.GetComponentInChildren<TextMeshProUGUI>().text =
                _deleteMode ? "Cancelar" : "Borrar Partida";
    }

    private static int _currentSlot = 0;

public static int CurrentSlot 
{ 
    get { return _currentSlot; }
}

public static void SetCurrentSlotStatic(int slot)
{
    _currentSlot = slot;
    Debug.Log("Slot seleccionado: " + _currentSlot);
}

}