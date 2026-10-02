using UnityEngine;

[CreateAssetMenu(fileName = "QuizQuestionData", menuName = "Scriptable Objects/QuizQuestionData")]
public class QuizQuestionData : ScriptableObject
{
    public string questionText;
    public string[] options = new string[4];
    [Range(0, 3)]
    public int correctOptionIndex;
}