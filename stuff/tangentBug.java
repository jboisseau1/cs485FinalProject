
//need to include the other packages still
package gmu.robot.pioneer;
import hokuyo.java.com.brianziman.robotics.*;

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
  }
}
