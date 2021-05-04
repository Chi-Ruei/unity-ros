using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class PlayerNetwork : MonoBehaviour
{
    public PhotonView view;

    public GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        player = this.gameObject;

        if (view == null)
            return;

        if (!view.IsMine)
        {
            Camera[] cams = player.GetComponentsInChildren<Camera>();
            foreach (Camera cam in cams)
            {
                cam.enabled = false;
            }

            AudioListener[] listeners = player.GetComponentsInChildren<AudioListener>();
            foreach (AudioListener lis in listeners)
            {
                lis.enabled = false;
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
