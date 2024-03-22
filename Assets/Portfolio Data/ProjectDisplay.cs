using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProjectDisplay : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI summaryText;

    public GameObject nextArrow;
    public GameObject prevArrow;
    public GameObject navBar;
    public GameObject projectsContainer;
    
    private string websiteURL; 
    public void SetProjectData(ProjectData projectData)
    {
        image.sprite = projectData.image;
        titleText.text = projectData.title;
        summaryText.text = projectData.summary;
        websiteURL = projectData.webSite;
    }

    public void OpenWebsite()
    {
        if (!string.IsNullOrEmpty(websiteURL))
        {
            Application.OpenURL(websiteURL);
        }
    }

    public void BackButton()
    {
        gameObject.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).OnComplete(() => gameObject.SetActive(false));

        projectsContainer.SetActive(true);
        nextArrow.SetActive(true);
        navBar.SetActive(true);
        prevArrow.SetActive(true);

        projectsContainer.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
        nextArrow.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
        prevArrow.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
        navBar.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
        
    }
}
