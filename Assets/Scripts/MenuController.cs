using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MenuController : MonoBehaviour
{
    public GameObject menu;
    public Transform headLocation;
    public float distanceFromCamera = 0.75f;

    private bool menuVisible = false;

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
            TryInitializeControllers();
        }

        // Check both controllers for grip and trigger pressed
        if (IsGripAndTriggerPressed(leftController) || IsGripAndTriggerPressed(rightController))
        {
            if (!menuVisible)
            {
                ShowMenu();
            }
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
    }

    private bool IsGripAndTriggerPressed(InputDevice controller)
    {
        if (!controller.isValid)
            return false;

        bool gripPressed = false;
        bool triggerPressed = false;

        controller.TryGetFeatureValue(CommonUsages.gripButton, out gripPressed);
        controller.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed);

        return gripPressed && triggerPressed;
    }

    public void ShowMenu()
    {
        Vector3 forward = new Vector3(headLocation.forward.x, 0, headLocation.forward.z).normalized;
        menu.transform.position = headLocation.position + forward * distanceFromCamera;
        menu.transform.rotation = Quaternion.LookRotation(forward);
        menu.SetActive(true);
        menuVisible = true;
    }

    public void HideMenu()
    {
        menu.SetActive(false);
        menuVisible = false;
    }
}
