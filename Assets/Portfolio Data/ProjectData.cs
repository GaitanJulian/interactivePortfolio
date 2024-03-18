using UnityEngine;

[CreateAssetMenu(fileName = "New Project", menuName = "Portfolio/Project")]
public class ProjectData : ScriptableObject
{
    public Sprite image;
    public string title;
    [TextArea(3, 10)]
    public string summary;
}
