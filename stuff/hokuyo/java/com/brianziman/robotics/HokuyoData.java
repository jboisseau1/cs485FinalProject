
package com.brianziman.robotics;


import java.net.*;
import java.io.*;
import java.util.*;
//Used to store data


public class HokuyoData{
public static ArrayList<Integer> data = new ArrayList<Integer>();
public static ArrayList<Integer> nonmes = new ArrayList<Integer>();
public static int max, maxindex;
SCIP11 laser = new SCIP11(5005);
  public static void main(String args[]){

    checkData(44,725,10 );
    System.out.println(Arrays.toString(data.toArray()));
  }


  public static void checkData(int start, int end, int cluster) {

	for(int i = 0; i < data.size()-1; i++){
      nonmes.set(i, 0);
    }

    data = laser.doDistanceCommand(start, end, cluster);

    getLongestDist();

  }

  public static void getLongestDist(){
    max = -1;
    maxindex = -1;

    for(int i = 0; i<data.size()-1;i++){
      if(data.get(i) < 20){

        nonmes.set(i, 1);
      }
      else if(data.get(i) > max){
        max = data.get(i);
        maxindex = i;
      }
    }
  }



}
