#!/usr/bin/env python2

from __future__ import print_function
import rospy
import sys
import copy
import math
import moveit_commander

import moveit_msgs.msg
from moveit_msgs.msg import Constraints, JointConstraint, PositionConstraint, OrientationConstraint, BoundingVolume
from sensor_msgs.msg import JointState
from moveit_msgs.msg import RobotState
import geometry_msgs.msg
from geometry_msgs.msg import Quaternion, Pose
from std_msgs.msg import String
from moveit_commander.conversions import pose_to_list

from interbotix_moveit.srv import MoverService, MoverServiceRequest, MoverServiceResponse


joint_names = ['waist', 'shoulder', 'elbow', 'forearm_roll', 'wrist_angle', 'wrist_rotate']


# Between Melodic and Noetic, the return type of plan() changed. moveit_commander has no __version__ variable, so checking the python version as a proxy
if sys.version_info >= (3, 0):
	def planCompat(plan):
		return plan[1]
else:
	def planCompat(plan):
		return plan


	
"""
	Given the start angles of the robot, plan a trajectory that ends at the destination pose.
"""
def plan_trajectory(move_group, destination_pose, start_joint_angles): 
    print(destination_pose)
    print('1')
    current_joint_state = JointState()
    current_joint_state.name = joint_names
    current_joint_state.position = start_joint_angles
    print('2')
    moveit_robot_state = RobotState()
    moveit_robot_state.joint_state = current_joint_state
    print('3')
    move_group.set_start_state(moveit_robot_state)
    move_group.set_pose_target(destination_pose)
    print('4')
    plan = move_group.plan()
    print('5')
    
    if not plan:
    	exception_str = """
    		Trajectory could not be planned for a destination of {} with starting joint angles {}.
    		Please make sure target and destination are reachable by the robot.
    	""".format(destination_pose, destination_pose)
    	raise Exception(exception_str)

    return planCompat(plan)


"""
	Creates a pick and place plan using the four states below.
	
	1. Pre Grasp - position gripper directly above target object
	2. Grasp - lower gripper so that fingers are on either side of object
	3. Pick Up - raise gripper back to the pre grasp position
	4. Place - move gripper to desired placement position

	Gripper behaviour is handled outside of this trajectory planning.
		- Gripper close occurs after 'grasp' position has been achieved
		- Gripper open occurs after 'place' position has been achieved

	https://github.com/ros-planning/moveit/blob/master/moveit_commander/src/moveit_commander/move_group.py
"""

def plan_pick_and_place(req):
    setup_scene()

    response = MoverServiceResponse()
    #unity unit
    #print(req.joints_input)

    # TODO: Make message type a list instead
    current_robot_joint_configuration = [
        math.radians(req.joints_input.joint_00),
        math.radians(req.joints_input.joint_01),
        math.radians(req.joints_input.joint_02),
        math.radians(req.joints_input.joint_03),
        math.radians(req.joints_input.joint_04),
        math.radians(req.joints_input.joint_05),
    ]
    #ros unity
    #print(current_robot_joint_configuration)

    print('pre grasp')
    # Pre grasp - position gripper directly above target object
    pre_grasp_pose = plan_trajectory(move_group, req.pick_pose, current_robot_joint_configuration) 

    # If the trajectory has no points, planning has failed and we return an empty response
    if not pre_grasp_pose.joint_trajectory.points:
        return response

    previous_ending_joint_angles = pre_grasp_pose.joint_trajectory.points[-1].positions
    
    print('grasp')
	# Grasp - lower gripper so that fingers are on either side of object
    pick_pose = copy.deepcopy(req.pick_pose)
    pick_pose.position.z -= 0.05  # Static value coming from Unity, TODO: pass along with request
    grasp_pose = plan_trajectory(move_group, pick_pose, previous_ending_joint_angles)
	
    if not grasp_pose.joint_trajectory.points:
    	return response

    previous_ending_joint_angles = grasp_pose.joint_trajectory.points[-1].positions
    
    print('Pick')
    # Pick Up - raise gripper back to the pre grasp position
    pick_up_pose = plan_trajectory(move_group, req.pick_pose, previous_ending_joint_angles)
	
    if not pick_up_pose.joint_trajectory.points:
    	return response

    previous_ending_joint_angles = pick_up_pose.joint_trajectory.points[-1].positions
    
    print('place')
	# Place - move gripper to desired placement position
    place_pose = plan_trajectory(move_group, req.place_pose, previous_ending_joint_angles)

    if not place_pose.joint_trajectory.points:
    	return response
        
  
	# If trajectory planning worked for all pick and place stages, add plan to response
    response.trajectories.append(pre_grasp_pose)
    response.trajectories.append(grasp_pose)
    response.trajectories.append(pick_up_pose)
    response.trajectories.append(place_pose)

    move_group.clear_pose_targets()

    return response


def setup_scene():
    global move_group
    robot = moveit_commander.RobotCommander(robot_description="vx300s/robot_description")
    scene = moveit_commander.PlanningSceneInterface(ns="vx300s")
    group_name = "interbotix_arm"
    move_group = moveit_commander.MoveGroupCommander(robot_description="vx300s/robot_description", ns="vx300s", name=group_name)

    rospy.sleep(2)


def moveit_server():
    ##########solve the joint_states problem###############
    joint_state_topic = ['joint_states:=/vx300s/joint_states']
    moveit_commander.roscpp_initialize(joint_state_topic)
    ##########solve the joint_states problem###############

    moveit_commander.roscpp_initialize(sys.argv)
    rospy.init_node('Viper_moveit_server', anonymous=True)

    s = rospy.Service('interbotix_moveit', MoverService, plan_pick_and_place)
    print("Ready to plan")
    rospy.spin()


if __name__ == "__main__":
	moveit_server()
