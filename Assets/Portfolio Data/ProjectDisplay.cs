using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProjectDisplay : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI summaryText;

    public void SetProjectData(ProjectData projectData)
    {
        image.sprite = projectData.image;
        titleText.text = projectData.title;
        summaryText.text = projectData.summary;
    }
}
