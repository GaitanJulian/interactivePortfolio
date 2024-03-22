using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortfolioProjectButton : MonoBehaviour
{
    public ProjectData projectData;
    public ProjectDisplay projectDisplay;

    public GameObject nextArrow;
    public GameObject previousArrow;
    public GameObject navBar;
    public GameObject projectsContainer;

    public void OpenProject()
    {

        nextArrow.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).OnComplete(() => nextArrow.SetActive(false)); 
        previousArrow.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).OnComplete(() => previousArrow.SetActive(false));
        navBar.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).OnComplete(() => navBar.SetActive(false));
        projectDisplay.SetProjectData(projectData);
        projectDisplay.gameObject.SetActive(true);
        projectDisplay.GetComponent<CanvasGroup>().alpha = 0f;
        projectDisplay.GetComponent<CanvasGroup>().DOFade(1f, 0.5f);
    }
}
