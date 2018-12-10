
package com.brianziman.robotics;

import java.net.*;
import java.io.*;
import java.util.*;
//Used to store data
public static ArrayList<Integer> data = new ArrayList<Integer>();
public static ArrayList<Integer> nonmes = new ArrayList<Integer>();
public static int max, maxindex;

public class Hokuyo{

  public static void checkData(int start, int end, int cluster) {

	for(int i = 0; i < data.size()-1; i++){
      nonmes.set(i, 0);
    }

    data = doDistanceCommand(start, end, cluster);
    getLongestDist();

  }

  public static void getLongestDist(){
    max = -1;
    maxindex = -1;

    for(int i = 0; i<data.size()-1;i++){
      if(data.get(i) < 20){

        nonmes.index(i) = 1;
      }
      else(data.get(i) > max){
        max = data.get(i);
        maxindex = i;
      }
    }
  }



}
