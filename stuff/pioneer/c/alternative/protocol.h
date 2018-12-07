
#ifndef PROTOCOL_H
#define PROTOCOL_H

/* The general protocol format is:
 * HEADER1
 * HEADER2
 * LENGTH
 * COMMAND
 * ARG_TYPE (opt)
 * ARG_VAL (opt)
 * CHECKSUM_HIGH
 * CHECKSUM_LOW
 */

/* Slightly more convenient */
#define cbyte const unsigned char

/* For some reason our checksum can be off by 3, per byte, from
 * the checksum the robot gets.  This is to handle it. */
#define CKSM_OFFSET 3

#define SONAR_MAX 32
#define SONAR_CNT 16
#define SONAR_RANGE 5000

/* Maximum size a packet and/or a string can be */
#define PKT_MAX 207
#define STR_MAX 200

/* A full packet should never be less than 31 bytes, and that
 * would be with *zero* sonar readings... */
#define PKT_MIN      31

/* Packet header */
#define HEADER cbyte header[2] = { 0xFA, 0xFB }

/* Argument types */
#define ARGTYPE_NONE 0x00
#define ARGTYPE_INT  0x3B
#define ARGTYPE_INDEP 0x01
#define ARGTYPE_NINT 0x1B
#define ARGTYPE_UINT 0x1B
#define ARGTYPE_STR  0x2B

/* The PULSE command is the same as the SYNC0 sequence */
#define PULSE 0x00

/* The OPEN command is the same as the SYNC1 sequence */
#define OPEN  0x01

/* The CLOSE command is the same as the SYNC1 sequence */
#define CLOSE 0x02

/* The rest of the commands */
#define POLLING        0x03
#define ENABLE         0x04
#define SETA           0x05
#define SETV           0x06
#define SETO           0x07
#define MOVE           0x08
#define ROTATE         0x09
#define SETRV          0x0A
#define VEL            0x0B
#define HEAD           0x0C
#define DHEAD          0x0D
#define SAY            0x0F
#define JOYREQUEST     0x11
#define CONFIG         0x12
#define ENCODER        0x13
#define RVEL           0x15
#define DCHEAD         0x16
#define SETRA          0x17
#define SONAR          0x1C
#define STOP           0x1D
#define DIGOUT         0x1E
#define VEL2           0x20
#define GRIPPER        0x21
#define ADSEL          0x23
#define GRIPPERVAL     0x24
#define GRIPREQUEST    0x25
#define GYROCALCW      0x26
#define GYROCALCCW     0x27
#define IOREQUEST      0x28
#define TTY2           0x2A
#define GETAUX         0x2B
#define BUMPSTALL      0x2C
#define TCM2           0x2D
#define JOYDRIVE       0x2F
#define SONARCYCLE     0x30
#define HOSTBAUD       0x32
#define AUX1BAUD       0x33
#define AUX2BAUD       0x34
#define AUX3BAUD       0x35
#define E_STOP         0x37
#define M_STALL        0x38
#define GYROREQUEST    0x3A
#define LCDWRITE       0x3B
#define TTY4           0x3C
#define GETAUX3        0x3D
#define TTY3           0x42
#define GETAUX2        0x43
#define CHARGE         0x44

/* arm-related commands */
#define ARM0           0x46
#define ARM1           0x47
#define ARM2           0x48
#define ARM3           0x49
#define ARM4           0x4A
#define ARM5           0x4B
#define ARM6           0x4C
#define ARM7           0x4D
#define ARM8           0x4E
#define ARM9           0x4F

#define ROTKP          0x52
#define ROTKV          0x53
#define ROTKI          0x54
#define TRANSKP        0x55
#define TRANSKV        0x56
#define TRANSKI        0x57
#define REVCOUNT       0x58
#define DRIFTFACTOR    0x59
#define SOUNDTOG       0x5C
#define TICKSMM        0x5D
#define BATTEST        0xFA
#define RESET          0xFD
#define MAINTENANCE    0xFF

#define SYNC0 PULSE
#define SYNC1 OPEN
#define SYNC2 CLOSE

#define STALL_FLAG    0x80

/* Size of each sonar info */
#define PKT_SONARSIZ  3

/* Packet data offsets */
#define PKT_TYPE      0
#define PKT_XPOS      1
#define PKT_YPOS      3
#define PKT_THPOS     5
#define PKT_LVEL      7
#define PKT_RVEL      9
#define PKT_BATT     11
#define PKT_LSTALL   12
#define PKT_RSTALL   13
#define PKT_CNRTL    14
#define PKT_FLAGS    16
#define PKT_COMPASS  18
#define PKT_SONARCNT 19
#define PKT_SONAROFF 20

/* Individual sonar info offsets */
#define PKT_SNR_NUM   0
#define PKT_SNR_RNGE  1

/* Sonar is of variable length, offsets from end of sonar data */
#define PKT_GRIP      0
#define PKT_ANPORT    1
#define PKT_ANALOG    2
#define PKT_DIGIN     3
#define PKT_DIGOUT    4
#define PKT_BATTV     5
#define PKT_CHARGE    7
#define PKT_ROTVEL    8
#define PKT_FAULT    10


/* Sync sequence */
#define SYNC cbyte sync[3] = { PULSE, OPEN, CLOSE }

/* Motor status info */
#define MOTORS_OFF 0x32
#define MOTORS_ON  0x33

/* Useful */
#define lo(x) (x >> 8)
#define hi(x) (x & 0xFF)

/* Conversion factors */
#define XPOS_CONV    0.5083
#define YPOS_CONV    0.5083
#define THPOS_CONV   0.001534
#define LVEL_CONV    0.6154
#define RVEL_CONV    0.6154
#define CONTROL_CONV 0.001534
#define SONAR_CONV   0.555

#endif
