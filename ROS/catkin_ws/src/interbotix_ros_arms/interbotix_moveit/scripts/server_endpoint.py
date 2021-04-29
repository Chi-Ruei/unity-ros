#!/usr/bin/env python

import rospy

from ros_tcp_endpoint import TcpServer, RosPublisher, RosSubscriber, RosService
from interbotix_moveit.msg import ViperMoveitJoints, ViperTrajectory
from interbotix_moveit.srv import MoverService, MoverServiceRequest


def main():
    ros_node_name = rospy.get_param("/TCP_NODE_NAME", 'TCPServer')
    tcp_server = TcpServer(ros_node_name)
    rospy.init_node(ros_node_name, anonymous=True)

    # Start the Server Endpoint with a ROS communication objects dictionary for routing messages
    tcp_server.start({
        'SourceDestination_input': RosPublisher('SourceDestination', ViperMoveitJoints, queue_size=10),
        'ViperTrajectory': RosSubscriber('ViperTrajectory', ViperTrajectory, tcp_server),
        'interbotix_moveit': RosService('interbotix_moveit', MoverService),

    })
    rospy.spin()


if __name__ == "__main__":
    main()
