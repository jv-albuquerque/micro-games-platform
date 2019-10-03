using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class contain all needed properties to make a power up works
/// </summary>
[System.Serializable]
public class PowerUpProperties
{
    /// <summary>
    /// Store the Image in the HUD
    /// </summary>
    public GameObject ui_image = null;

    /// <summary>
    /// Store the seconds counter on HUD
    /// </summary>
    public GameObject ui_count = null;

    /// <summary>
    /// How many time the power up will be activated
    /// </summary>
    public float time = 15;

    /// <summary>
    /// The text on the UI thats stores the time left
    /// </summary>
    [HideInInspector] public SetText textCount;

    /// <summary>
    /// the cooldown that will count if the power up stills on
    /// </summary>
    [HideInInspector] public Cooldown cd;

    /// <summary>
    /// If the power up is active
    /// </summary>
    [HideInInspector] public bool active = false;


    /// <summary>
    /// hide the UI gameobjects of the power up on the screen
    /// </summary>
    private void DeactivateUI()
    {
        ui_image.SetActive(false);
        ui_count.SetActive(false);
    }

    /// <summary>
    /// show the UI gameobjects of the power up on the screen
    /// </summary>
    private void ActivateUI()
    {
        ui_image.SetActive(true);
        ui_count.SetActive(true);
    }
    /// <summary>
    /// Update the text in sec on the screen
    /// </summary>
    public void UpdateText()
    {
        textCount.Text = (int)cd.TimeLeft + " sec";
    }

    /// <summary>
    /// Start the power up
    /// </summary>
    public void Start()
    {
        active = true;

        ActivateUI();

        cd.Start();

        UpdateText();
    }

    /// <summary>
    /// Stop the power up
    /// </summary>
    public void Stop()
    {
        active = false;
        DeactivateUI();
    }
}
