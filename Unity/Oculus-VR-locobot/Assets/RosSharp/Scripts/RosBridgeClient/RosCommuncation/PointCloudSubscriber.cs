using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RosSharp.RosBridgeClient;
using std_msgs = RosSharp.RosBridgeClient.MessageTypes.Std;
using sensor_msgs = RosSharp.RosBridgeClient.MessageTypes.Sensor;
//using RosSharp.RosBridgeClient.Protocols;


using System.IO;

using Object = UnityEngine.Object;

[RequireComponent(typeof(RosConnector))]
[RequireComponent(typeof(MeshFilter))]
public class PointCloudSubscriber : MonoBehaviour
{

    public string uri = "ws://10.42.0.2:9090";
    private RosSocket rosSocket;
    string subscriptionId = "";

    public RgbPoint3[] Points;

    //Mesh components
    private Mesh mesh;
    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    //Vector3[] newverts;

    private bool update_mesh = false;


    // Start is called before the first frame update
    void Start()
    {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        //CreateShape();
        //UpdateMesh();

        //mesh.vertices = newVertices;
        //mesh.uv = newUV;
        //mesh.triangles = newTriangles;


        Debug.Log("RosSocket Initialization!!!");
        //RosSocket rosSocket = new RosSocket("ws://147.229.14.150:9090");
        rosSocket = new RosSocket(new RosSharp.RosBridgeClient.Protocols.WebSocketNetProtocol(uri)); // 10.189.42.225:9090
        //Subscribe("/cloud");
        //Subscribe("/zed/rtabmap/cloud_map");
        Subscribe("/camera/depth_registered/points");


    }



    void Update()
    {


        ////string publication_id = rosSocket.Advertise<std_msgs.String>("/talker");

        ////std_msgs.String message = new std_msgs.String
        ////{
        ////    data = "hello"
        ////};
        //RosSocket.Publish(publication_id, message);
        //RosSocket.Unadvertise(publication_id);
        //Thread.Sleep(100);
        //Assert.IsTrue(true);



        ////rosSocket.Publish(publication_id, message);

        //Subscribe("/zed/rtabmap/cloud_map");

        if (update_mesh)
        {
            update_mesh = false;

            //CreateShape();
            UpdateMesh();
        }

    }



    void CreateShape()
    {
        Debug.Log("received one");
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                vertices[i] = new Vector3(x, 0, z);
                i++;
            }
        }
        Debug.Log("received two");
    }

    void UpdateMesh()
    {
        Debug.Log("received three");

        try
        {
            mesh.Clear();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        Debug.Log("received four");

        mesh.vertices = vertices;


    }

    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0.2F, 0, 0.5F);
        if (vertices == null)
            return;
        for (int i = 0; i < vertices.Length; i++)
        {
            //Gizmos.DrawSphere(vertices[i], .1f);
            Gizmos.DrawCube(vertices[i], new Vector3(0.1f, 0.1f, 0.1f));
        }

    }

    /*public void OnDrawGizmos()
    {
        if(vertices == null)
            return;
        for(int i =0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
            //Gizmos.DrawWireCube(vertices[i], .1f);
        }
    }*/

    public void Subscribe(string id)
    {
        //subscriptionId  = rosSocket.Subscribe<std_msgs.String>(id, SubscriptionHandler);
        subscriptionId = rosSocket.Subscribe<sensor_msgs.PointCloud2>(id, SubscriptionHandler);
        //StartCoroutine(WaitForKey());    

        // Subscription:
        //subscription_id = rosSocket.Subscribe<std_msgs.String>("/subscription_test", SubscriptionHandler);
        //subscription_id = rosSocket.Subscribe<std_msgs.String>("/subscription_test", SubscriptionHandler);

    }

    private IEnumerator WaitForKey()
    {
        Debug.Log("Press any key to close...");

        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        Debug.Log("Closed");
        // rosSocket.Close();
    }

    public void SubscriptionHandler(sensor_msgs.PointCloud2 message)
    {

        Debug.Log("Message received!");
        long I = message.data.Length / message.point_step;
        Debug.Log("Long I   " + I);
        RgbPoint3[] Points = new RgbPoint3[I];
        byte[] byteSlice = new byte[message.point_step];

        for (long i = 0; i < I; i++)
        {
            Array.Copy(message.data, i * message.point_step, byteSlice, 0, message.point_step);
            Points[i] = new RgbPoint3(byteSlice, message.fields);

            //Debug.Log("X   " + Points[i].x);
            //Debug.Log("Y   " + Points[i].y);
            //Debug.Log("Z   " + Points[i].z);


        }


        //Vector3[] newverts = new Vector3[I];
        vertices = new Vector3[I];
        //Debug.Log("newvertes_X   " + newverts[0].x ...
    }

}