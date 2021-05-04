using UnityEngine;
using System.Collections;

using Photon.Realtime;
using Photon.Pun;



namespace Com.MyCompany.MyGame
{
    public class CubeControl : MonoBehaviourPun, IPunObservable
    {
        #region MonoBehaviour Callbacks

        public Rigidbody rb;
        public float speed = 5f;


        float h, v;

        // Use this for initialization
        void Start()
        {
            rb = GetComponent<Rigidbody>();

        }


        // Update is called once per frame
        void Update()
        {




            if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            {
                return;
            }


            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            rb.AddForce(new Vector3(h, 0f, v) * speed);





        }


        #endregion
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

            if (stream.IsWriting)
            {
                // write
                stream.SendNext(h);

            }
            else
            {
                // read
                Debug.Log(stream.ReceiveNext().ToString());


            }



        }
    }
}