using UnityEngine;

/// <summary> Stores the difficulty index, allows to change it after restarting </summary>
[CreateAssetMenu(fileName = "New Difficulty Container", menuName = "Custom Options/Difficulty")]
public class Difficulty : ScriptableObject
{
    [SerializeField] private int difficultyIndex = 0;

    /// <summary> Return difficulty index, that can be used in Scenario </summary>
    public int DifficultyIndex
    {
        get
        {
            return difficultyIndex;
        }
        set
        {
            difficultyIndex = value;
        }
    }
}
