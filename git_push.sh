#!/bin/bash

if [ ! "$1" ]; then
    echo "commit detail please"
    return
fi
echo "commit: $1"

COMMIT=$1
BRANCH=master

if [ ! -z "$2" ]; then
    echo "operator on branch: $2"
    BRANCH=$2
fi

source git_pull.sh $BRANCH
PULLSTAT=$?
if [ "$PULLSTAT" -gt 0 ] ; then
   echo "There is conflict. Aborting"
   cd ~/unity-ros/
   return
fi
echo "-------------------------pull success----------------------------------"

# push core
echo "-----------------------------------------------------------------------"
echo "-------------------------push VR-locobot------------------------------"
echo "-----------------------------------------------------------------------"
cd ~/unity-ros/ROS/catkin_ws/src/VR-locobot
git add -A
git commit -m "$1 on oculusVR"
git push

# push unity-ros
echo "-----------------------------------------------------------------------"
echo "-------------------------push unity-ros----------------------------------"
echo "-----------------------------------------------------------------------"
cd ~/unity-ros/
git add -A
git commit -m "$1"
git push

