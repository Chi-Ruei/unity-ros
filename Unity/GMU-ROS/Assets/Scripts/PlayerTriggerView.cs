using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class PlayerTriggerView : MonoBehaviourPunCallbacks, IPunObservable
{

    public PhotonView view;
    public Text text;
    public float triggerVal;


    public void Start()
    {
        view = GetComponentInParent<PhotonView>();
    }

    public void Update()
    {
        if (view.IsMine)
        {
            triggerVal = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
            text.text = "" + triggerVal;
        }
        else
        {
            text.text = "" + triggerVal;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(triggerVal);
        }
        else
        {
            triggerVal = (float)stream.ReceiveNext();
        }
    }

}
