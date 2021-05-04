using RosMessageTypes.InterbotixMoveit;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;

public class SourceDestinationPublisher : MonoBehaviour
{
    // ROS Connector
    private ROSConnection ros;
    
    // Variables required for ROS communication
    public string topicName = "SourceDestination_input";

    public GameObject vx300s;
    public GameObject target;
    public GameObject targetPlacement;
    
    private int numRobotJoints = 6;
    private readonly Quaternion pickOrientation = Quaternion.Euler(90, 90, 0);
    
    // Articulation Bodies
    private ArticulationBody[] jointArticulationBodies;
    
    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        // Get ROS connection static instance
        ros = ROSConnection.instance;

        jointArticulationBodies = new ArticulationBody[numRobotJoints];
        string shoulder_link = "base_link/shoulder_link";
        jointArticulationBodies[0] = vx300s.transform.Find(shoulder_link).GetComponent<ArticulationBody>();

        string upper_arm_link = shoulder_link + "/upper_arm_link";
        jointArticulationBodies[1] = vx300s.transform.Find(upper_arm_link).GetComponent<ArticulationBody>();
        
        string upper_forearm_link = upper_arm_link + "/upper_forearm_link";
        jointArticulationBodies[2] = vx300s.transform.Find(upper_forearm_link).GetComponent<ArticulationBody>();
        
        string lower_forearm_link = upper_forearm_link + "/lower_forearm_link";
        jointArticulationBodies[3] = vx300s.transform.Find(lower_forearm_link).GetComponent<ArticulationBody>();
        
        string wrist_link = lower_forearm_link + "/wrist_link";
        jointArticulationBodies[4] = vx300s.transform.Find(wrist_link).GetComponent<ArticulationBody>();
        
        string gripper_link = wrist_link + "/gripper_link";
        jointArticulationBodies[5] = vx300s.transform.Find(gripper_link).GetComponent<ArticulationBody>();
    }

    public void Publish()
    {
        ViperMoveitJoints sourceDestinationMessage = new ViperMoveitJoints();

        sourceDestinationMessage.joint_00 = jointArticulationBodies[0].xDrive.target;
        sourceDestinationMessage.joint_01 = jointArticulationBodies[1].xDrive.target;
        sourceDestinationMessage.joint_02 = jointArticulationBodies[2].xDrive.target;
        sourceDestinationMessage.joint_03 = jointArticulationBodies[3].xDrive.target;
        sourceDestinationMessage.joint_04 = jointArticulationBodies[4].xDrive.target;
        sourceDestinationMessage.joint_05 = jointArticulationBodies[5].xDrive.target;

        // Pick Pose
        sourceDestinationMessage.pick_pose = new RosMessageTypes.Geometry.Pose
        {
            position = target.transform.position.To<FLU>(),
            orientation = Quaternion.Euler(90, target.transform.eulerAngles.y, 0).To<FLU>()
        };

        // Place Pose
        sourceDestinationMessage.place_pose = new RosMessageTypes.Geometry.Pose
        {
            position = targetPlacement.transform.position.To<FLU>(),
            orientation = pickOrientation.To<FLU>()
        };

        // Finally send the message to server_endpoint.py running in ROS
        ros.Send(topicName, sourceDestinationMessage);
    }
}
