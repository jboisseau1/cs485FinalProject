
package com.brianziman.robotics;

import java.net.*;
import java.io.*;
import java.util.*;

ArrayList<Integer> data = new ArrayList<Integer>();
ArrayList<Integer> nonmes = new ArrayList<Integer>();
int max, maxindex;

public class Constant_data{

  public Constant_data(int start, int end, int cluster){
    for(int i = 0; i < data.size()-1; i++){
      nonmes.set(i, 0);
    }
      checkData(start, end, cluster);
      System.out.println(max + " " + maxindex);
  }

  public  void checkData(int start, int end, int cluster) {


      data = doDistanceCommand(start, end, cluster);
      getLongestDist();

  }

  public void getLongestDist(){
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
