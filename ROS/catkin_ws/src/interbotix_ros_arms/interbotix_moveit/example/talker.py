#!/usr/bin/env python
import rospy
import geometry_msgs.msg
from std_srvs.srv import Trigger, TriggerResponse

def talker():
    pub = rospy.Publisher('Pickpose', geometry_msgs.msg.Pose, queue_size=1)
    s = rospy.Service('add_two_ints', Trigger , print_srv)

    rospy.init_node('Pickpose_publisher', anonymous=True)
    rate = rospy.Rate(10) # 10hz

    while not rospy.is_shutdown():
        pose_goal = geometry_msgs.msg.Pose()
        pose_goal.position.x = 0.5
        pose_goal.position.y = 0.1
        pose_goal.position.z = 0.2

        pose_goal.orientation.w = 0
        pose_goal.orientation.x = 0
        pose_goal.orientation.y = 0
        pose_goal.orientation.z = 0
        
        pub.publish(pose_goal)

def print_srv():
    print("Hello")

        

if __name__ == '__main__':
    try:
        talker()
    except rospy.ROSInterruptException:
        pass