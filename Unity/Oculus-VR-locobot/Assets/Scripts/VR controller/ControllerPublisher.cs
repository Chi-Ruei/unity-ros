using System.Collections;
using System.Collections.Generic;
using RosSharp.RosBridgeClient;
using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;
using sensor_msgs = RosSharp.RosBridgeClient.MessageTypes.Sensor;
using UnityEngine;
using UnityEngine.XR;


public class ControllerPublisher : MonoBehaviour
    {
        RosSocket rosSocket;
        string publication_test; //Pub_test
        string primary_button;
        string scondary_button;
        string grip;
        string trigger;
        string joystick;

        //VR Device
        public InputDeviceCharacteristics controllerCharacteristics;
        private InputDevice targetDevices; //what is your vr contoller input

        public string FrameId = "Unity";

    void Start()
    {
        //VR Device
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
            if (devices.Count > 0)
            {
            targetDevices = devices[0];
            Debug.Log(devices[0]);
            }

        //RosSocket
        rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol("ws://10.42.0.2:9090"));
        Debug.Log("Established connection with ros");

        //Topic name
        publication_test = rosSocket.Advertise<std_msgs.String>("publication_test"); //Pub_test
        primary_button = rosSocket.Advertise<std_msgs.String>("vr/primarybutton");
        scondary_button = rosSocket.Advertise<std_msgs.String>("vr/secondarybutton");
        grip = rosSocket.Advertise<std_msgs.String>("vr/grip");
        trigger = rosSocket.Advertise<std_msgs.String>("vr/trigger");
        joystick = rosSocket.Advertise<std_msgs.String>("vr/joystick");

    }


    void Update()
     {
        //------------------Pub_test------------------------------//
        std_msgs.String message_test = new std_msgs.String
        { 
             data = "Message sent from unity"
        };
        rosSocket.Publish(publication_test, message_test);
        //Debug.Log("A message was sent to ROS");
        //------------------Pub_test------------------------------//

        //------------------Pub_Primary Buttom------------------------------//
        targetDevices.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
        //Debug.Log("Primary Button" + primaryButtonValue);
        if (primaryButtonValue)
        {
           std_msgs.String message_p = new std_msgs.String
            {
                data = "1"
            };

            rosSocket.Publish(primary_button, message_p);
            }

        //------------------Pub_Secondary Buttom------------------------------//
        targetDevices.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryButtonValue);
        //Debug.Log("Secondy Button" + secondaryButtonValue);
        if (secondaryButtonValue)
        {
            std_msgs.String message_s = new std_msgs.String
            {
                data = "1"
            };

            rosSocket.Publish(scondary_button, message_s);
        }

        //------------------Pub_Grip------------------------------//
        targetDevices.TryGetFeatureValue(CommonUsages.grip, out float gripValue);
        //Debug.Log("Grip Value" + gripValue);
        if(gripValue > 0.1f)
        {
            string temp_grip = gripValue.ToString("0.000");
            std_msgs.String message_grip = new std_msgs.String
            {
                data = temp_grip
            };

            rosSocket.Publish(grip, message_grip);
        }

        //------------------Pub_Trigger------------------------------//
        targetDevices.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        //Debug.Log("Trigger Value" + triggerValue);
        if (gripValue > 0.1f)
        {
            string temp_trigger = triggerValue.ToString("0.000");
            std_msgs.String message_trigger = new std_msgs.String
            {
                data = temp_trigger
            };

            rosSocket.Publish(trigger, message_trigger);
        }

        //------------------Pub_Joystick------------------------------//
        targetDevices.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joyValue);
        Debug.Log("Joy Value" + joyValue);
        if (joyValue != Vector2.zero)
        {

            //float temp_joy[] = new float temp_joy[];
            //temp_joy[0] = joyValue.x.ToString("0.000");

            float temp_joy_x = joyValue.x;
            float temp_joy_y = joyValue.y;

            float[] temp_joy = new float[]
            {
                temp_joy_x,
                temp_joy_y
            };
            //Debug.Log("temp_x" + temp_joy_x);
            //Debug.Log("temp_y" + temp_joy_y);
            sensor_msgs.Joy message_joy = new sensor_msgs.Joy
            {
                axes = temp_joy

            };
            message_joy.header.frame_id = FrameId;
            //rosSocket.Publish(joystick, message_joy);
        }

    }

}
    


