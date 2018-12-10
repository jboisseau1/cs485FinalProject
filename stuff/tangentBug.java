
//need to include the other packages still
package gmu.robot.pioneer;
package com.brianziman.robotics;

import java.net.*;
import java.io.*;
import java.util.*;


public class tangentBug {
  static long time = 0;
  public static boolean current_time(long millis)
  {
    long current = System.currentTimeMillis();
    if(time + millis < current)
    {
      time = current;
      return true;
    }
    else return false;
  }


  public static void main(String []args) throws Exception
  {
      if(args.length < 1)
      {
        
      }
    
      //the main logic of tangentBugAlgorithm
      boolean running = true;
      boolean hitObstacle = false;
      while(running){
        //scan for obstacle
        //set a distance to run till, i.e. check size of robot to allow for turning
        /*if(hit obstacle){
          hitObstacle = true;
        }
        //follow the obstacle and draw the line
        while(hitObstacle){
          //draw the line for the obstacle

        }
      
        if(reached goal){
          running = false;
        }
        */
      }
  }
}
