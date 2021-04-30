using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

namespace Autohand.Demo{
    public class OVRHandControllerLink : MonoBehaviour{
        public Hand hand;
        public OVRInput.Controller controller;
        public OVRInput.Axis1D grabAxis;
        public OVRInput.Button grabButton;
        public OVRInput.Button squeezeButton;

        public void Update() {
            if(OVRInput.GetDown(grabButton, controller)) {
                hand.Grab();
                hand.gripOffset += 1;
            }
            if(OVRInput.GetUp(grabButton, controller)) {
                hand.Release();
                hand.gripOffset -= 1;
            }
            if(OVRInput.GetDown(squeezeButton, controller)) {
                hand.Squeeze();
            }
            if(OVRInput.GetUp(squeezeButton, controller)) {
                hand.Unsqueeze();
            }

            //Debug.Log(OVRInput.Get(grabAxis, controller));
            hand.SetGrip(OVRInput.Get(grabAxis, controller));
        }

        public float GetAxis(OVRInput.Axis1D axis) {
            return OVRInput.Get(axis, controller);
        }

        public Vector2 GetAxis2D(OVRInput.Axis2D axis) {
            return OVRInput.Get(axis, controller);
        }
        
        public bool ButtonPressed(OVRInput.Button button) {
            return OVRInput.Get(button, controller);
        }
        
        public bool ButtonPressed(OVRInput.Touch button) {
            return OVRInput.Get(button, controller);
        }
        
        public bool ButtonTouched(OVRInput.Touch button) {
            return OVRInput.Get(button, controller);
        }
    }
}
