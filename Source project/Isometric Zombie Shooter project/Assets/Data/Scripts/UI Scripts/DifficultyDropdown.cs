using UnityEngine;
using UnityEngine.UI;

/// <summary> Links dropdown's UI GameObject with difficulty container </summary>
[RequireComponent(typeof(Dropdown))]
public class DifficultyDropdown : MonoBehaviour
{
    [SerializeField] private Difficulty difficultyContainer = null;

    private Dropdown dropdown;

    private void Awake() 
    {
        dropdown = GetComponent<Dropdown>();

        // display used difficulty on restart
        dropdown.value = difficultyContainer.DifficultyIndex;

        // change difficulty in container when dropdown changed
        dropdown.onValueChanged.AddListener(delegate { ChangeDifficulty(dropdown); });
    }

    /// <summary> Unsubscrive from events </summary>
    private void OnDestroy() 
    {
        dropdown.onValueChanged.RemoveAllListeners();
    }

    /// <summary> Changing difficulty in container </summary>
    public void ChangeDifficulty(Dropdown _dropdown)
    {
        difficultyContainer.DifficultyIndex = _dropdown.value;
    }
}
