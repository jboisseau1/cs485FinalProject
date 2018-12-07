
#ifndef ROBOT_H
#define ROBOT_H

#include <stdio.h>
#include "protocol.h"

typedef struct {
  char name[STR_MAX];
  char type[STR_MAX];
  char subtype[STR_MAX];
  int   dev_fd;
	FILE *log;
  int packet_id;
  int motor;
  short in_xpos;
  double xpos;
  short in_ypos;
  double ypos;
  short in_thpos;
  double thpos;
  short in_lvel;
  double lvel;
  short in_rvel;
  double rvel;
  char  battery;
  char  lstall;
  int left_stall;
  char  rstall;
  int right_stall;
  short in_control;
  double control;
  unsigned short flags;
  char  compass;
  unsigned short in_sonars[SONAR_CNT];
  double sonars[SONAR_CNT];
  int sonar_updates[SONAR_CNT];
  char  grip_state;
  char  anport;
  char  analog;
  char  digin;
  char  digout;
  short batteryv;
  char  chargestate;
  short rotvel;
  short faultflags;
} robot;

int print_binary(FILE *out, char *str, unsigned char *data, int length);
int open_serial_dev(robot *bot, char *dev, int baud);
int cmd_type(unsigned char cmd);
unsigned short cksum(cbyte *data, unsigned int len);
int cmd(robot *bot, unsigned char cmd, short int_arg, unsigned short uint_arg, char *char_arg);
int init_bot_log(robot *bot, char *logfile);
int init_bot(robot *bot);
int read_packet(robot *bot);
int r_sync(robot *bot);

#endif
