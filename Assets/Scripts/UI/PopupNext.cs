using System.Collections.Generic;
using UnityEngine;

public class PopupNext : MonoBehaviour
{
    public List<CanvasGroup> popupGroups;
    private int currentIndex = 0;

    private void Awake()
    {
        foreach (var group in popupGroups)
        {
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }

    public void ShowNextPopup()
    {
        if (currentIndex < popupGroups.Count && currentIndex > -1)
        {
            currentIndex++;
            // Hide all popups
            foreach (var group in popupGroups)
            {
                group.alpha = 0;
                group.interactable = false;
                group.blocksRaycasts = false;
            }
            // Show the current popup
            var currentGroup = popupGroups[currentIndex];
            currentGroup.alpha = 1;
            currentGroup.interactable = true;
            currentGroup.blocksRaycasts = true;
        }
        else
        {
            Debug.Log("No more popups to show.");
        }
    }
    public void ShowPreviousPopup()
    {
        if (currentIndex < popupGroups.Count && currentIndex > 0)
        {
            currentIndex--;
            // Hide all popups
            foreach (var group in popupGroups)
            {
                group.alpha = 0;
                group.interactable = false;
                group.blocksRaycasts = false;
            }
            // Show the current popup
            var currentGroup = popupGroups[currentIndex];
            currentGroup.alpha = 1;
            currentGroup.interactable = true;
            currentGroup.blocksRaycasts = true;
        }
        else
        {
            Debug.Log("No more popups to show.");
        }
    }
}
