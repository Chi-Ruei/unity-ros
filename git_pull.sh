#! /bin/bash

# echo "password: $2"
BRANCH=master
if [ ! -z "$1" ]; then
    echo "pull branch: $1"
    BRANCH=$1
fi

echo "-----------------------------------------------------------------------"
echo "-------------------------pull unity-ros-------------------------------"
echo "-----------------------------------------------------------------------"
git pull

CONFLICTS=$(git ls-files -u | wc -l)
if [ "$CONFLICTS" -gt 0 ] ; then
   echo "There is conflict in mm-medical. Aborting"
   return 1
fi

echo "-----------------------------------------------------------------------"
echo "-------------------------pull VR-locobot--------------------------------"
echo "-----------------------------------------------------------------------"
cd ~/unity-ros/ROS/catkin_ws/src/VR-locobot
git checkout $BRANCH
git pull

CONFLICTS=$(git ls-files -u | wc -l)
if [ "$CONFLICTS" -gt 0 ] ; then
   echo "There is conflict in oculusVR. Aborting"
   return 1
fi

cd ~/unity-ros
return 0