using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> controllerPrefabs; //what controller model inclued
    private InputDevice targetDevices; //what is your vr contoller input
    private GameObject spawnedController; // which wanted to cloning the original object

    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        //InputDevices.GetDevices(devices);
        //InputDeviceCharacteristics rightDeviceCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
        


        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }

        if(devices.Count>0)
        {
            targetDevices = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevices.name);
            if (prefab)
            {
                //instantiate meaning cloneing the original object
                spawnedController = Instantiate(prefab, transform);
            }
            else
            {
                Debug.Log("did not find correspoinding controller model");
                spawnedController = Instantiate(controllerPrefabs[0], transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {   //Primary Button             
        if (targetDevices.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
            Debug.Log("Pressing Primary Button");

        // Trigger       
        if (targetDevices.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue) && triggerValue > 0.1f)
            Debug.Log("Trigger pressed"+ triggerValue);

        //Primary Joystick       
        if (targetDevices.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue) && primary2DAxisValue !=Vector2.zero)
            Debug.Log("Primart Touchpad"+primary2DAxisValue);
        if (targetDevices.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool primary2DAxisClick) &&  primary2DAxisClick)
            Debug.Log("primary2DAxisClick" + primary2DAxisClick);

    }
}
