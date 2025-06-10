using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MenuController : MonoBehaviour
{
    public GameObject menu;
    public Transform headLocation;
    public float distanceFromCamera = 0.75f;

    public bool menuVisible = false;

    // XR input feature usages
    private InputDevice leftController;
    private InputDevice rightController;

    void Start()
    {
        TryInitializeControllers();
    }

    void Update()
    {
        if (!leftController.isValid || !rightController.isValid)
        {
            Start();
            TryInitializeControllers();
        }
        TryInitializeControllers();
        // Check both controllers for primary and secondary button pressed
        if ((IsPrimaryPressed(leftController) || IsPrimaryPressed(rightController)) && !menuVisible)
        {
            ShowMenu();
            Debug.Log("Menu shown using primary button.");
        }
    }

    private void TryInitializeControllers()
    {
        var leftHandedControllers = new List<InputDevice>();
        var rightHandedControllers = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, leftHandedControllers);
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandedControllers);

        if (leftHandedControllers.Count > 0)
            leftController = leftHandedControllers[0];
        if (rightHandedControllers.Count > 0)
            rightController = rightHandedControllers[0];
        Debug.Log("Controllers initialized: Left - " + leftController.isValid + ", Right - " + rightController.isValid);
    }

    private bool IsPrimaryPressed(InputDevice controller)
    {
        if (!controller.isValid)
            return false;

        bool primaryPressed = false;

        controller.TryGetFeatureValue(CommonUsages.primaryButton, out primaryPressed);

        return primaryPressed;
    }

    public void ShowMenu()
    {
        Vector3 forward = new Vector3(headLocation.forward.x, 0, headLocation.forward.z).normalized;
        menu.transform.position = headLocation.position + forward * distanceFromCamera;
        menu.transform.rotation = Quaternion.LookRotation(forward);
        menu.SetActive(true);
        menuVisible = true;
        Debug.Log("Menu shown at position: " + menu.transform.position + " with rotation: " + menu.transform.rotation.eulerAngles);
    }

    public void HideMenu()
    {
        menu.SetActive(false);
        menuVisible = false;
        Debug.Log("Menu hidden.");
    }
}