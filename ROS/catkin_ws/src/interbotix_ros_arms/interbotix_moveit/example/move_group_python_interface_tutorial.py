#!/usr/bin/env python

import sys
import copy
import rospy
import moveit_commander
import moveit_msgs.msg
import geometry_msgs.msg
import time
from math import pi
from std_msgs.msg import String
from std_msgs.msg import Float32MultiArray
from interbotix_sdk.msg import SingleCommand
from std_srvs.srv import Trigger, TriggerResponse
from moveit_commander.conversions import pose_to_list
## END_SUB_TUTORIAL

def all_close(goal, actual, tolerance):
  """
  Convenience method for testing if a list of values are within a tolerance of their counterparts in another list
  @param: goal       A list of floats, a Pose or a PoseStamped
  @param: actual     A list of floats, a Pose or a PoseStamped
  @param: tolerance  A float
  @returns: bool
  """
  all_equal = True
  if type(goal) is list:
    for index in range(len(goal)):
      if abs(actual[index] - goal[index]) > tolerance:
        return False

  elif type(goal) is geometry_msgs.msg.PoseStamped:
    return all_close(goal.pose, actual.pose, tolerance)

  elif type(goal) is geometry_msgs.msg.Pose:
    return all_close(pose_to_list(goal), pose_to_list(actual), tolerance)

  return True

class MoveGroupPythonIntefaceTutorial(object):
  """MoveGroupPythonIntefaceTutorial"""
  def __init__(self):
    super(MoveGroupPythonIntefaceTutorial, self).__init__()
    ## BEGIN_SUB_TUTORIAL setup
    ##
    ## First initialize `moveit_commander`_ and a `rospy`_ node:
    self.robot_name = rospy.get_param("~robot_name","vx300s")
    moveit_commander.roscpp_initialize(sys.argv)
    rospy.init_node('move_group_python_interface_tutorial',
                    anonymous=True)

    ## Get the name of the robot - this will be used to properly define the end-effector link when adding a box
    ## Get the dof of the robot - this will make sure that the right number of joints are controlled
    #dof = rospy.get_param("~dof")
    self.dof = 6

    ## Instantiate a `RobotCommander`_ object. This object is the outer-level interface to
    ## the robot:
    robot = moveit_commander.RobotCommander(robot_description=self.robot_name+"/robot_description")####

    ## Instantiate a `PlanningSceneInterface`_ object.  This object is an interface
    ## to the world surrounding the robot:
    print("1")
    scene = moveit_commander.PlanningSceneInterface()####
    print("2")
    ## Instantiate a `MoveGroupCommander`_ object.  This object is an interface
    ## to one group of joints.  In this case the group is the joints in the Interbotix
    ## arm so we set ``group_name = interbotix_arm``. If you are using a different robot,
    ## you should change this value to the name of your robot arm planning group.
    ## This interface can be used to plan and execute motions on the Interbotix Arm:
    group_name = "interbotix_arm"
    group = moveit_commander.MoveGroupCommander(robot_description=self.robot_name+"/robot_description", name=group_name)####


    ## We create a `DisplayTrajectory`_ publisher which is used later to publish
    ## trajectories for RViz to visualize:
    display_trajectory_publisher = rospy.Publisher("move_group/display_planned_path",
                                                   moveit_msgs.msg.DisplayTrajectory,
                                                   queue_size=20)

    ## END_SUB_TUTORIAL

    ## BEGIN_SUB_TUTORIAL basic_info
    ##
    ## Getting Basic Information
    ## ^^^^^^^^^^^^^^^^^^^^^^^^^
    # We can get the name of the reference frame for this robot:
    planning_frame = group.get_planning_frame()
    print "============ Reference frame: %s" % planning_frame

    # We can also print the name of the end-effector link for this group:
    eef_link = group.get_end_effector_link()
    print "============ End effector: %s" % eef_link

    # We can get a list of all the groups in the robot:
    group_names = robot.get_group_names()
    print "============ Robot Groups:", robot.get_group_names()

    # Sometimes for debugging it is useful to print the entire state of the
    # robot:
    print "============ Printing robot state"
    print robot.get_current_state()
    print ""
    ## END_SUB_TUTORIAL

    # Misc variables
    self.box_name = ''
    self.robot = robot
    self.scene = scene
    self.group = group
    self.display_trajectory_publisher = display_trajectory_publisher
    self.planning_frame = planning_frame
    self.eef_link = eef_link
    self.group_names = group_names

    self.point_position_msg = Float32MultiArray()
    self.sub_point_position = rospy.Subscriber("Pickpose", Float32MultiArray, self.point_position_cb)
    #self.pub_joint_command = rospy.Publisher("single_joint/command", SingleCommand, queue_size=1)
    
    self.srv_rest = rospy.Service('arm_rest', Trigger, self.cb_rest_srv)
    #self.srv_ready = rospy.Service('arm_ready', Trigger, self.cb_ready)
    #self.srv_lift = rospy.Service('arm_lift', Trigger, self.cb_lift)

  def go_to_joint_state(self):
    group = self.group
    dof = self.dof
    joint_goal = group.get_current_joint_values()
    joint_goal[0] = 0
    joint_goal[1] = -pi/4
    joint_goal[2] = 0
    joint_goal[3] = -pi/2
    if dof >= 5:
        joint_goal[4] = 0
    if dof >= 6:
        joint_goal[5] = pi/3

    group.go(joint_goal, wait=True)

    group.stop()

    current_joints = self.group.get_current_joint_values()
    return all_close(joint_goal, current_joints, 0.01)

  def go_to_pose_goal(self):
    group = self.group
    robot_name = self.robot_name

    pose_goal = geometry_msgs.msg.Pose()
    pose_goal.orientation.w = 1.0
    if robot_name == "vx300s":
        pose_goal.position.x = 0.1
        pose_goal.position.z = 0.15
    else:
        pose_goal.position.x = 0.25
        pose_goal.position.z = 0.2
    group.set_pose_target(pose_goal)

 
    plan = group.go(wait=True)
    group.stop()
    group.clear_pose_targets()

    current_pose = self.group.get_current_pose().pose
    return all_close(pose_goal, current_pose, 0.01)


  def plan_cartesian_path(self, scale=1):
    group = self.group

    waypoints = []

    wpose = group.get_current_pose().pose
    wpose.position.z += scale * 0.1  # First move up (z)
    waypoints.append(copy.deepcopy(wpose))

    wpose.position.x += scale * 0.1  # Second move forward in (x)
    waypoints.append(copy.deepcopy(wpose))

    wpose.position.z -= scale * 0.1  # Third move down (z)
    waypoints.append(copy.deepcopy(wpose))


    (plan, fraction) = group.compute_cartesian_path(
                                       waypoints,   # waypoints to follow
                                       0.01,        # eef_step
                                       0.0)         # jump_threshold

    # Note: We are just planning, not asking move_group to actually move the robot yet:
    return plan, fraction

    ## END_SUB_TUTORIAL

  def display_trajectory(self, plan):

    robot = self.robot
    display_trajectory_publisher = self.display_trajectory_publisher
    display_trajectory = moveit_msgs.msg.DisplayTrajectory()
    display_trajectory.trajectory_start = robot.get_current_state()
    display_trajectory.trajectory.append(plan)
    # Publish
    display_trajectory_publisher.publish(display_trajectory);


  def execute_plan(self, plan):

    group = self.group
    group.execute(plan, wait=True)


  


def main():
  try:
    print "============ Press `Enter` to begin the tutorial by setting up the moveit_commander (press ctrl-d to exit) ..."
    raw_input()
    tutorial = MoveGroupPythonIntefaceTutorial()

    print "============ Press `Enter` to execute a movement using a joint state goal ..."
    raw_input()
    tutorial.go_to_joint_state()

    print "============ Press `Enter` to execute a movement using a pose goal ..."
    raw_input()
    tutorial.go_to_pose_goal()

    print "============ Press `Enter` to plan and display a Cartesian path ..."
    raw_input()
    cartesian_plan, fraction = tutorial.plan_cartesian_path()

    print "============ Press `Enter` to display a saved trajectory (this will replay the Cartesian path)  ..."
    raw_input()
    tutorial.display_trajectory(cartesian_plan)


  except rospy.ROSInterruptException:
    return
  except KeyboardInterrupt:
    return

if __name__ == '__main__':
  main()