
#ifndef ROBOT_CMDS_H
#define ROBOT_CMDS_H

#include "robot.h"

int r_pulse       (robot *bot);
int r_open        (robot *bot);
int r_close       (robot *bot);
int r_polling     (robot *bot, char *str);
int r_enable      (robot *bot, short int_arg);
int r_seta        (robot *bot, short int_arg);
int r_setv        (robot *bot, short int_arg);
int r_seto        (robot *bot);
int r_move        (robot *bot, short int_arg);
int r_rotate      (robot *bot, short int_arg);
int r_setrv       (robot *bot, short int_arg);
int r_vel         (robot *bot, short int_arg);
int r_head        (robot *bot, short int_arg);
int r_dhead       (robot *bot, short int_arg);
int r_say         (robot *bot, char *str);
int r_joyrequest  (robot *bot, short int_arg);
int r_config      (robot *bot);
int r_encoder     (robot *bot, short int_arg);
int r_rvel        (robot *bot, short int_arg);
int r_dchead      (robot *bot, short int_arg);
int r_setra       (robot *bot, short int_arg);
int r_sonar       (robot *bot, short int_arg);
int r_stop        (robot *bot);
int r_digout      (robot *bot, unsigned short uint_arg);
int r_vel2        (robot *bot, unsigned short uint_arg);
int r_gripper     (robot *bot, short int_arg);
int r_adsel       (robot *bot, short int_arg);
int r_gripperval  (robot *bot, short int_arg);
int r_griprequest (robot *bot, short int_arg);
int r_gyrocalcw   (robot *bot, unsigned short uint_arg);
int r_gyrocalccw  (robot *bot, unsigned short uint_arg);
int r_iorequest   (robot *bot, short int_arg);
int r_tty2        (robot *bot, char *str);
int r_getaux      (robot *bot, unsigned short uint_arg);
int r_bumpstall   (robot *bot, short int_arg);
int r_tcm2        (robot *bot, short int_arg);
int r_joydrive    (robot *bot, short int_arg);
int r_sonarcycle  (robot *bot, unsigned short uint_arg);
int r_hostbaud    (robot *bot, short int_arg);
int r_aux1band    (robot *bot, short int_arg);
int r_aux2band    (robot *bot, short int_arg);
int r_aux3band    (robot *bot, short int_arg);
int r_e_stop      (robot *bot);
int r_m_stall     (robot *bot, short int_arg);
int r_gyrorequest (robot *bot, short int_arg);
int r_lcdwrite    (robot *bot, char *str);
int r_tty4        (robot *bot, char *str);
int r_getaux3     (robot *bot, short int_arg);
int r_tty3        (robot *bot, char *str);
int r_getaux2     (robot *bot, short int_arg);
int r_charge      (robot *bot, short int_arg);
int r_arm0        (robot *bot, short int_arg);
int r_arm1        (robot *bot, short int_arg);
int r_arm2        (robot *bot, short int_arg);
int r_arm3        (robot *bot, short int_arg);
int r_arm4        (robot *bot, short int_arg);
int r_arm5        (robot *bot, short int_arg);
int r_arm6        (robot *bot, short int_arg);
int r_arm7        (robot *bot, short int_arg);
int r_arm8        (robot *bot, short int_arg);
int r_arm9        (robot *bot, short int_arg);
int r_rotkp       (robot *bot, short int_arg);
int r_rotkv       (robot *bot, short int_arg);
int r_rotki       (robot *bot, short int_arg);
int r_transkp     (robot *bot, short int_arg);
int r_transkv     (robot *bot, short int_arg);
int r_transki     (robot *bot, short int_arg);
int r_revcount    (robot *bot, short int_arg);
int r_driftfactor (robot *bot, short int_arg);
int r_soundtog    (robot *bot, short int_arg);
int r_ticksmm     (robot *bot, short int_arg);
int r_battest     (robot *bot, short int_arg);
int r_reset       (robot *bot);
int r_maintenance (robot *bot);

int r_vel2_indep(robot *bot, char rvel, char lvel);

int r_digout_indep(robot *bot, char lsbits, char msbits);

#endif
