using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AboutUIScript : MonoBehaviour
{
    public GameObject[] presentationGO;
    public GameObject[] aboutGO;
    public GameObject[] skillsGO;
    public GameObject[] educationGO;

    private GameObject[] selectedGO;
    private bool isAnimationCompleted = true;
    private void Start()
    {
        selectedGO = presentationGO;
    }

    public void PresentationSelected()
    {
        SwitchSelectedSection(presentationGO);
    }

    public void AboutSelected()
    {
        SwitchSelectedSection(aboutGO);
    }

    public void SkillsSelected()
    {
        SwitchSelectedSection(skillsGO);
    }

    public void EducationSelected()
    {
        SwitchSelectedSection(educationGO);
    }

    private void SwitchSelectedSection(GameObject[] newSection)
    {
        // prevent the UI from bugging
        if (newSection == selectedGO || !isAnimationCompleted)
            return;

        isAnimationCompleted = false;

        // Fade out the currently selected section
        foreach (GameObject go in selectedGO)
        {
            go.SetActive(true);
            go.GetComponent<CanvasGroup>().DOFade(0f, 0.5f).OnComplete(() => go.SetActive(false));
        }

        // Fade in the new section
        foreach (GameObject go in newSection)
        {
            go.SetActive(true);
            go.GetComponent<CanvasGroup>().alpha = 0f;
            go.GetComponent<CanvasGroup>().DOFade(1f, 0.5f).OnComplete(() => isAnimationCompleted = true); ;
        }

        // Update selectedGO to the new section
        selectedGO = newSection;
    }
}
