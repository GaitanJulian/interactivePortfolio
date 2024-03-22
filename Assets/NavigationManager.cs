using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationManager : MonoBehaviour
{
    [Header("Menu Position on screen")]
    [SerializeField] private Transform entrancePosition;
    [SerializeField] private Transform aboutMePosition;
    [SerializeField] private Transform followPLayerPosition;
    [SerializeField] private Transform creditsPosition;
    [SerializeField] private Transform projectsPosition;
    [SerializeField] private Transform playgroundPosition;
    [SerializeField] private Transform openMenuPosition;
    private Transform currentLocation;

    [Header("Menu")]
    [SerializeField] private MenuAnimations menuAnimations;
    [SerializeField] GameObject menu;
    [SerializeField] Collider menuCollider;

    // For enabling the character movement
    [HideInInspector] public bool canMove = false;
    [SerializeField] private MeshRenderer[] walls;
    private bool areWallsRendered = true;

    [Header("Animations")]
    [SerializeField] private float menuAnimationDuration = 1f;
    [SerializeField] private Animator cinemachineAnimator;

    // Animation States IDs
    private int animationIDEntrance;
    private int animationIDAboutme;
    private int animationIDFollowPlayer;
    private int animationIDCredits;
    private int animationIDProjects;
    private int animationIDPlayground;

    private void Start()
    {
        currentLocation = entrancePosition;
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs()
    {
        animationIDEntrance = Animator.StringToHash("Entrance Camera");
        animationIDAboutme = Animator.StringToHash("About me Camera");
        animationIDFollowPlayer = Animator.StringToHash("Player Follow Camera");
        animationIDCredits = Animator.StringToHash("Credits Camera");
        animationIDProjects = Animator.StringToHash("Projects Camera");
        animationIDPlayground = Animator.StringToHash("Playground Camera");
    }

    public void OnClickToStart()
    {
        cinemachineAnimator.Play(animationIDEntrance);
        Invoke("EnableMenu", 1.6f);
    }

    private void EnableMenu()
    {
        menu.SetActive(true);
    }

    public void OnAboutMe()
    {
        cinemachineAnimator.Play(animationIDAboutme);
        ChangeLocation(aboutMePosition);
    }

    public void OnProjects()
    {
        cinemachineAnimator.Play(animationIDProjects);
        ChangeLocation(projectsPosition);
    }

    public void OnCredits()
    {
        cinemachineAnimator.Play(animationIDCredits);
        ChangeLocation(creditsPosition);
    }

    public void OnPlayground()
    {
        cinemachineAnimator.Play(animationIDPlayground);
        ChangeLocation(playgroundPosition);
    }

    public void OnExplore()
    {
        cinemachineAnimator.Play(animationIDFollowPlayer);
        ChangeLocation(followPLayerPosition);
        foreach (MeshRenderer mesh in walls)
        {
            mesh.enabled = false;
        }
        areWallsRendered = false;
    }


    private void ChangeLocation(Transform location)
    {
        menuAnimations.CloseMenu();
        menuCollider.enabled = true;
        menu.transform.DOMove(location.position, menuAnimationDuration);
        currentLocation = location;

        if (currentLocation == followPLayerPosition)
        {
            canMove = true;
        } 
        else
        {
            canMove = false;
        }

        if (!areWallsRendered)
        {
            foreach (MeshRenderer mesh in walls)
            {
                mesh.enabled = true;
            }
        }
    }

    public void OnMenuClick()
    {
        menuAnimations.OpenMenu();
        menu.transform.DOMove(openMenuPosition.position, menuAnimationDuration);
        menuCollider.enabled = false;
    }

    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }
}
