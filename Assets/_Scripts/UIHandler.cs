using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public GameObject workerMenu = null;

    private void Update()
    {
        if (InputHandler.instance.selectedUnits.Count == 1)
        {   // Only allow the build menu to be open when there is a single worker selected.
            workerMenu.SetActive(true);
        }
        else
        {
            workerMenu.SetActive(false);
        }
    }
}
