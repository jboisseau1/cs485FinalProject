
#include <string.h>
#include <limits.h>
#include "robot.h"
#include "robot_cmds.h"

int r_pulse       (robot *bot)                          { return cmd(bot,PULSE,0,0,NULL); }
int r_open        (robot *bot)                          { return cmd(bot,OPEN,0,0,NULL); }
int r_close       (robot *bot)                          { return cmd(bot,CLOSE,0,0,NULL); }
int r_polling     (robot *bot, char *str)               { return cmd(bot,POLLING,0,0,str); }
int r_enable      (robot *bot, short int_arg)           { return cmd(bot,ENABLE,int_arg,0,NULL); }
int r_seta        (robot *bot, short int_arg)           { return cmd(bot,SETA,int_arg,0,NULL); }
int r_setv        (robot *bot, short int_arg)           { return cmd(bot,SETV,int_arg,0,NULL); }
int r_seto        (robot *bot)                          { return cmd(bot,SETO,0,0,NULL); }
int r_move        (robot *bot, short int_arg)           { return cmd(bot,MOVE,int_arg,0,NULL); }
int r_rotate      (robot *bot, short int_arg)           { return cmd(bot,ROTATE,int_arg,0,NULL); }
int r_setrv       (robot *bot, short int_arg)           { return cmd(bot,SETRV,int_arg,0,NULL); }
int r_vel         (robot *bot, short int_arg)           { return cmd(bot,VEL,int_arg,0,NULL); }
int r_head        (robot *bot, short int_arg)           { return cmd(bot,HEAD,int_arg,0,NULL); }
int r_dhead       (robot *bot, short int_arg)           { return cmd(bot,DHEAD,int_arg,0,NULL); }
int r_say         (robot *bot, char *str)               { return cmd(bot,SAY,0,0,str); }
int r_joyrequest  (robot *bot, short int_arg)           { return cmd(bot,JOYREQUEST,int_arg,0,NULL); }
int r_config      (robot *bot)                          { return cmd(bot,CONFIG,0,0,NULL); }
int r_encoder     (robot *bot, short int_arg)           { return cmd(bot,ENCODER,int_arg,0,NULL); }
int r_rvel        (robot *bot, short int_arg)           { return cmd(bot,RVEL,int_arg,0,NULL); }
int r_dchead      (robot *bot, short int_arg)           { return cmd(bot,DCHEAD,int_arg,0,NULL); }
int r_setra       (robot *bot, short int_arg)           { return cmd(bot,SETRA,int_arg,0,NULL); }
int r_sonar       (robot *bot, short int_arg)           { return cmd(bot,SONAR,int_arg,0,NULL); }
int r_stop        (robot *bot)                          { return cmd(bot,STOP,0,0,NULL); }
int r_digout      (robot *bot, unsigned short uint_arg) { return cmd(bot,DIGOUT,0,uint_arg,NULL); }
int r_vel2        (robot *bot, unsigned short uint_arg) { return cmd(bot,VEL2,0,uint_arg,NULL); }
int r_gripper     (robot *bot, short int_arg)           { return cmd(bot,GRIPPER,int_arg,0,NULL); }
int r_adsel       (robot *bot, short int_arg)           { return cmd(bot,ADSEL,int_arg,0,NULL); }
int r_gripperval  (robot *bot, short int_arg)           { return cmd(bot,GRIPPERVAL,int_arg,0,NULL); }
int r_griprequest (robot *bot, short int_arg)           { return cmd(bot,GRIPREQUEST,int_arg,0,NULL); }
int r_gyrocalcw   (robot *bot, unsigned short uint_arg) { return cmd(bot,GYROCALCW,0,uint_arg,NULL); }
int r_gyrocalccw  (robot *bot, unsigned short uint_arg) { return cmd(bot,GYROCALCCW,0,uint_arg,NULL); }
int r_iorequest   (robot *bot, short int_arg)           { return cmd(bot,IOREQUEST,int_arg,0,NULL); }
int r_tty2        (robot *bot, char *str)               { return cmd(bot,TTY2,0,0,str); }
int r_getaux      (robot *bot, unsigned short uint_arg) { return cmd(bot,GETAUX,0,uint_arg,NULL); }
int r_bumpstall   (robot *bot, short int_arg)           { return cmd(bot,BUMPSTALL,int_arg,0,NULL); }
int r_tcm2        (robot *bot, short int_arg)           { return cmd(bot,TCM2,int_arg,0,NULL); }
int r_joydrive    (robot *bot, short int_arg)           { return cmd(bot,JOYDRIVE,int_arg,0,NULL); }
int r_sonarcycle  (robot *bot, unsigned short uint_arg) { return cmd(bot,SONARCYCLE,0,uint_arg,NULL); }
int r_hostbaud    (robot *bot, short int_arg)           { return cmd(bot,HOSTBAUD,int_arg,0,NULL); }
int r_aux1band    (robot *bot, short int_arg)           { return cmd(bot,AUX1BAUD,int_arg,0,NULL); }
int r_aux2band    (robot *bot, short int_arg)           { return cmd(bot,AUX2BAUD,int_arg,0,NULL); }
int r_aux3band    (robot *bot, short int_arg)           { return cmd(bot,AUX3BAUD,int_arg,0,NULL); }
int r_e_stop      (robot *bot)                          { return cmd(bot,E_STOP,0,0,NULL); }
int r_m_stall     (robot *bot, short int_arg)           { return cmd(bot,M_STALL,int_arg,0,NULL); }
int r_gyrorequest (robot *bot, short int_arg)           { return cmd(bot,GYROREQUEST,int_arg,0,NULL); }
int r_lcdwrite    (robot *bot, char *str)               { return cmd(bot,LCDWRITE,0,0,str); }
int r_tty4        (robot *bot, char *str)               { return cmd(bot,TTY4,0,0,str); }
int r_getaux3     (robot *bot, short int_arg)           { return cmd(bot,GETAUX3,int_arg,0,NULL); }
int r_tty3        (robot *bot, char *str)               { return cmd(bot,TTY3,0,0,str); }
int r_getaux2     (robot *bot, short int_arg)           { return cmd(bot,GETAUX2,int_arg,0,NULL); }
int r_charge      (robot *bot, short int_arg)           { return cmd(bot,CHARGE,int_arg,0,NULL); }
int r_arm0        (robot *bot, short int_arg)           { return cmd(bot,ARM0,int_arg,0,NULL); }
int r_arm1        (robot *bot, short int_arg)           { return cmd(bot,ARM1,int_arg,0,NULL); }
int r_arm2        (robot *bot, short int_arg)           { return cmd(bot,ARM2,int_arg,0,NULL); }
int r_arm3        (robot *bot, short int_arg)           { return cmd(bot,ARM3,int_arg,0,NULL); }
int r_arm4        (robot *bot, short int_arg)           { return cmd(bot,ARM4,int_arg,0,NULL); }
int r_arm5        (robot *bot, short int_arg)           { return cmd(bot,ARM5,int_arg,0,NULL); }
int r_arm6        (robot *bot, short int_arg)           { return cmd(bot,ARM6,int_arg,0,NULL); }
int r_arm7        (robot *bot, short int_arg)           { return cmd(bot,ARM7,int_arg,0,NULL); }
int r_arm8        (robot *bot, short int_arg)           { return cmd(bot,ARM8,int_arg,0,NULL); }
int r_arm9        (robot *bot, short int_arg)           { return cmd(bot,ARM9,int_arg,0,NULL); }
int r_rotkp       (robot *bot, short int_arg)           { return cmd(bot,ROTKP,int_arg,0,NULL); }
int r_rotkv       (robot *bot, short int_arg)           { return cmd(bot,ROTKV,int_arg,0,NULL); }
int r_rotki       (robot *bot, short int_arg)           { return cmd(bot,ROTKI,int_arg,0,NULL); }
int r_transkp     (robot *bot, short int_arg)           { return cmd(bot,TRANSKP,int_arg,0,NULL); }
int r_transkv     (robot *bot, short int_arg)           { return cmd(bot,TRANSKV,int_arg,0,NULL); }
int r_transki     (robot *bot, short int_arg)           { return cmd(bot,TRANSKI,int_arg,0,NULL); }
int r_revcount    (robot *bot, short int_arg)           { return cmd(bot,REVCOUNT,int_arg,0,NULL); }
int r_driftfactor (robot *bot, short int_arg)           { return cmd(bot,DRIFTFACTOR,int_arg,0,NULL); }
int r_soundtog    (robot *bot, short int_arg)           { return cmd(bot,SOUNDTOG,int_arg,0,NULL); }
int r_ticksmm     (robot *bot, short int_arg)           { return cmd(bot,TICKSMM,int_arg,0,NULL); }
int r_battest     (robot *bot, short int_arg)           { return cmd(bot,BATTEST,int_arg,0,NULL); }
int r_reset       (robot *bot)                          { return cmd(bot,RESET,0,0,NULL); }
int r_maintenance (robot *bot)                          { return cmd(bot,MAINTENANCE,0,0,NULL); }

int r_vel2_indep(robot *bot, char rvel, char lvel) {
  unsigned short tmp;
	unsigned char l = 0, r = 0;

	if (lvel < 0) {
	  l |= 1 << 7;
		l += lvel*-1;
	} else {
	  l = lvel;
	}

	if (rvel < 0) {
	  r |= 1 << 7;
		r += rvel*-1;
	} else {
	  r = rvel;
	}

	tmp = l << 8 | r;

  return cmd(bot,VEL2,0,tmp,NULL); 
}

int r_digout_indep(robot *bot, char lsbits, char msbits) {
  unsigned short tmp;

	if (lsbits < 0) lsbits += UCHAR_MAX+1;
	if (msbits < 0) msbits += UCHAR_MAX+1;
	tmp = lsbits << 8 | msbits;

  return cmd(bot,DIGOUT,0,tmp,NULL); 
}
