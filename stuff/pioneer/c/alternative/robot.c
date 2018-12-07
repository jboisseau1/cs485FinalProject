
#include <fcntl.h> 
#include <stdio.h>
#include <errno.h>
#include <string.h>
#include <termios.h> 
#include <sys/time.h>
#include <sys/stat.h> 
#include <sys/types.h>
#include <sys/socket.h>
#include <sys/select.h>
#include <netinet/in.h>
#include <unistd.h>
#include <errno.h>

#include "robot.h"
#include "robot_cmds.h"

int print_binary(FILE *out, char *str, unsigned char *data, int length)
{
  fprintf(out,str);
  fprintf(out," ");
  for (int pos = 0; pos < length; pos++) {
    fprintf(out,"%.2x",data[pos]);
  }
  fprintf(out,"\n");

  return 1;
}
  
/* Calculate the checksum of the data */
unsigned short cksum(cbyte *data, unsigned int len) {
  int i = 0, c = 0;
  unsigned char n;

  n = (unsigned char)len;
  while (n > 1) {
    c += ((unsigned char) data[i] << 8) | (unsigned char) data[i+1];
    c &= 0xFFFF;
    n -= 2;
    i += 2;
  }

  if (n > 0) c = c ^ (int) ((unsigned char) data[i]);

  return c;
}

int cmd_type(unsigned char cmd)
{
  switch(cmd) {
    case PULSE: return(ARGTYPE_NONE);
    case OPEN: return(ARGTYPE_NONE);
    case CLOSE: return(ARGTYPE_NONE);
    case POLLING: return(ARGTYPE_STR);
    case ENABLE: return(ARGTYPE_INT);
    case SETA: return(ARGTYPE_INT);
    case SETV: return(ARGTYPE_INT);
    case SETO: return(ARGTYPE_NONE);
    case MOVE: return(ARGTYPE_INT);
    case ROTATE: return(ARGTYPE_INT);
    case SETRV: return(ARGTYPE_INT);
    case VEL: return(ARGTYPE_INT);
    case HEAD: return(ARGTYPE_INT);
    case DHEAD: return(ARGTYPE_INT);
    case SAY: return(ARGTYPE_STR);
    case JOYREQUEST: return(ARGTYPE_INT);
    case CONFIG: return(ARGTYPE_NONE);
    case ENCODER: return(ARGTYPE_INT);
    case RVEL: return(ARGTYPE_INT);
    case DCHEAD: return(ARGTYPE_INT);
    case SETRA: return(ARGTYPE_INT);
    case SONAR: return(ARGTYPE_INT);
    case STOP: return(ARGTYPE_NONE);
    case DIGOUT: return(ARGTYPE_UINT);
    case VEL2: return(ARGTYPE_INDEP);
    case GRIPPER: return(ARGTYPE_INT);
    case ADSEL: return(ARGTYPE_INT);
    case GRIPPERVAL: return(ARGTYPE_INT);
    case GRIPREQUEST: return(ARGTYPE_INT);
    case GYROCALCW: return(ARGTYPE_UINT);
    case GYROCALCCW: return(ARGTYPE_UINT);
    case IOREQUEST: return(ARGTYPE_INT);
    case TTY2: return(ARGTYPE_STR);
    case GETAUX: return(ARGTYPE_UINT);
    case BUMPSTALL: return(ARGTYPE_INT);
    case TCM2: return(ARGTYPE_INT);
    case JOYDRIVE: return(ARGTYPE_INT);
    case SONARCYCLE: return(ARGTYPE_UINT);
    case HOSTBAUD: return(ARGTYPE_INT);

    case AUX1BAUD: return(ARGTYPE_INT);
    case AUX2BAUD: return(ARGTYPE_INT);
    case AUX3BAUD: return(ARGTYPE_INT);
    case E_STOP: return(ARGTYPE_NONE);
    case M_STALL: return(ARGTYPE_INT);
    case GYROREQUEST: return(ARGTYPE_INT);
    case LCDWRITE: return(ARGTYPE_STR);
    case TTY4: return(ARGTYPE_STR);
    case GETAUX3: return(ARGTYPE_INT);
    case TTY3: return(ARGTYPE_STR);
    case GETAUX2: return(ARGTYPE_INT);
    case CHARGE: return(ARGTYPE_INT);

    case ARM0: return(ARGTYPE_INT);
    case ARM1: return(ARGTYPE_INT);
    case ARM2: return(ARGTYPE_INT);
    case ARM3: return(ARGTYPE_INT);
    case ARM4: return(ARGTYPE_INT);
    case ARM5: return(ARGTYPE_INT);
    case ARM6: return(ARGTYPE_INT);
    case ARM7: return(ARGTYPE_INT);
    case ARM8: return(ARGTYPE_INT);
    case ARM9: return(ARGTYPE_INT);

    case ROTKP: return(ARGTYPE_INT);
    case ROTKV: return(ARGTYPE_INT);
    case ROTKI: return(ARGTYPE_INT);
    case TRANSKP: return(ARGTYPE_INT);
    case TRANSKV: return(ARGTYPE_INT);
    case TRANSKI: return(ARGTYPE_INT);
    case REVCOUNT: return(ARGTYPE_INT);
    case DRIFTFACTOR: return(ARGTYPE_INT);
    case SOUNDTOG: return(ARGTYPE_INT);
    case TICKSMM: return(ARGTYPE_INT);
    case BATTEST: return(ARGTYPE_INT);
    case RESET: return(ARGTYPE_NONE);
    case MAINTENANCE: return(ARGTYPE_NONE);
  }

  return(ARGTYPE_NONE);
}

int cmd(robot *bot, unsigned char cmd, short int_arg, unsigned short uint_arg, char *char_arg)
{
  HEADER;
  int i = 0, length = 1;
  unsigned short checksum;
  unsigned char data[PKT_MAX];

  fprintf(bot->log,"enter cmd(%x,%x,%x,%s)\n",cmd,int_arg,uint_arg,char_arg == NULL ? "NULL" : char_arg);

  switch (cmd_type(cmd)) {
    case ARGTYPE_NONE: length += 0; break;
    case ARGTYPE_INT:  length++; length += sizeof(int_arg); break;
    case ARGTYPE_INDEP: length++; length += sizeof(uint_arg); break;
    case ARGTYPE_UINT: length++; length += sizeof(uint_arg); break;
    case ARGTYPE_STR:  length++; length += strlen(char_arg) <= STR_MAX ? strlen(char_arg) : STR_MAX; break;
  }
  length += sizeof(checksum);

  memcpy(data+i,header,sizeof(header));
  i += sizeof(header);
  data[i++] = length;
  data[i++] = cmd;

  switch (cmd_type(cmd)) {
    case ARGTYPE_NONE: break;
    case ARGTYPE_INT:  if (int_arg >= 0) {
                         data[i++] = ARGTYPE_INT;
                       } else {
                         int_arg *= -1;
                         data[i++] = ARGTYPE_NINT;
                       }
                       memcpy(data+i,&int_arg,sizeof(int_arg));
                       i += sizeof(int_arg);
                       break;
    case ARGTYPE_INDEP: data[i++] = ARGTYPE_INT;
                        memcpy(data+i,&uint_arg,sizeof(uint_arg));
                        i += sizeof(uint_arg);
                        break;
    case ARGTYPE_UINT: data[i++] = ARGTYPE_UINT;
                       memcpy(data+i,&uint_arg,sizeof(uint_arg));
                       i += sizeof(uint_arg);
                       break;
    case ARGTYPE_STR:  data[i++] = ARGTYPE_STR;
                       memcpy(data+i,char_arg,length-1);
                       i += length-1;
                       break;
  }

  print_binary(bot->log,"cmd: cksum call:",data+sizeof(header)+1,length-sizeof(checksum));
  checksum = cksum(data+sizeof(header)+1,length-sizeof(checksum));
  data[i++] = lo(checksum);
  data[i++] = hi(checksum);

  print_binary(bot->log,"cmd: data:",data,i);

  write(bot->dev_fd,data,i);

  return 1;
}

int read_packet(robot *bot)
{
  HEADER;
  unsigned char sonarcnt;
  int attempts;
  int sonar_offset, curr_snr_offset, curr_snr, snr_num, read_length;
  int read_amt = 0, res = 0;
  char *name_ptr, *type_ptr, *subtype_ptr;
  unsigned short checksum;
  unsigned char in_checksum[2];
  unsigned char cmp_checksum[2];
  unsigned char in_header[sizeof(header)];
  unsigned char data[PKT_MAX];
  unsigned char length;

  fprintf(bot->log,"enter read_packet()\n");

  /* Look for a header in the data,
   * This is basically a sliding read where we pull in
   * a byte in the far-right part of the buffer and then
   * move everything down one byte each time around and
   * read in a new byte to fill it. */
  memset(in_header,0,sizeof(in_header));
  while (memcmp(in_header,header,sizeof(header)) != 0) {
    memmove(in_header,in_header+1,sizeof(header));
    read_length = read(bot->dev_fd,in_header+sizeof(header)-1,1);
    attempts = 0;
    while (read_length == -1 && errno == EAGAIN) {
      if (attempts > 5) return(-1);
      sleep(1);
      read_length = read(bot->dev_fd,in_header+sizeof(header)-1,1);
      attempts++;
    }
    read_amt++;
    if (read_length != 1) { fprintf(bot->log,"read_packet: underflow, requested: %d got: %d\n",1,read_length); return(-1); }
  }

  fprintf(bot->log,"read_packet: header found after %d bytes\n",read_amt);

  /* Header found, get the packet size */
  read_length = read(bot->dev_fd,&length,1);
  attempts = 0;
  while (read_length == -1 && errno == EAGAIN) {
    if (attempts > 5) return(-1);
    sleep(1);
    read_length = read(bot->dev_fd,&length,1);
    attempts++;
  }
  if (read_length != 1) { fprintf(bot->log,"read_packet: underflow, requested: %d got: %d\n",1,read_length); return(-1); }
  length -= sizeof(in_checksum);

  fprintf(bot->log,"read_packet: packet length: %d\n",length);

  /* Read in the rest of this packet. */
  res = read(bot->dev_fd,data,length);
  if (res != -1) read_length = res; else read_length = 0;
  attempts = 0;
  while (read_length < length && (res != -1 || (res == -1 && errno == EAGAIN))) {
    if (attempts > 5) return(-1);
    sleep(1);
    fprintf(bot->log,"read_packet(): partial read: request: %d, got: %d, new request: %d\n",length,read_length,length-read_length);
    res = read(bot->dev_fd,data+read_length,length-read_length);
    if (res != -1) read_length += res;
    attempts++;
  }
  if (read_length != length) {
    perror("read");
    fprintf(bot->log,"read_packet: underflow, requested: %d got: %d\n",length,read_length);
    return(-1);
  }

  print_binary(bot->log,"read_packet: data:",data,length);

  /* Read in the checksum */
  read_length = read(bot->dev_fd,in_checksum,sizeof(in_checksum));
  if (read_length != sizeof(in_checksum)) fprintf(bot->log,"read_packet: underflow, requested: %d got: %d\n",(int)sizeof(in_checksum),read_length);
  print_binary(bot->log,"read_packet: checksum read:",in_checksum,sizeof(in_checksum));

  if (bot->packet_id > 0) {
    /* Send back a pulse, just to let the other side know we
     * are still alive and kicking. */
    r_pulse(bot);
  }

  /* Check the checksum */
  checksum = cksum(data,length);
  cmp_checksum[0] = lo(checksum);
  cmp_checksum[1] = hi(checksum);
  if (memcmp(in_checksum,cmp_checksum,2)) {
    print_binary(bot->log,"read_packet: invalid checksum, expected:",cmp_checksum,sizeof(cmp_checksum));
    return(-1);
  }

  /* Check type of packet */
  /* Still trying to sync */
  if (bot->packet_id == -1) {
    switch (data[PKT_TYPE]) {
      case SYNC0: fprintf(bot->log,"got SYNC0\n"); return(SYNC0);
      case SYNC1: fprintf(bot->log,"got SYNC1\n"); return(SYNC1);
      case SYNC2: /* SYNC2 also returns our name/type/subtype */
                  fprintf(bot->log,"got SYNC2\n"); 
                  name_ptr = (char*)data+1;
                  type_ptr = name_ptr+strlen(name_ptr)+1;
                  subtype_ptr = type_ptr+strlen(type_ptr)+1;
                  strncpy(bot->name,name_ptr,STR_MAX);
                  strncpy(bot->type,type_ptr,STR_MAX);
                  strncpy(bot->subtype,subtype_ptr,STR_MAX);
                  return(SYNC2);
    }
    fprintf(bot->log,"unknown packet type\n");
    return(-1);
  }

  /* Should now be synced */
  switch (data[PKT_TYPE]) {
    case MOTORS_OFF: bot->motor = 0; break;
    case MOTORS_ON:  bot->motor = 1; break;
    default: return(-1); break;
      return -1;
  }

  fprintf(bot->log,"read_packet: motor status: %d\n",bot->motor);

  /* Make sure we have a full-length info packet. */
  if (length < PKT_MIN || length > PKT_MAX) {
    fprintf(bot->log,"read_packet: invalid packet length: %u.\n",length);
    return 0;
  }

  /* If we got here then we must have a full-size status packet */
  /* Update the packet ID */
  bot->packet_id++;

  /* Pull the sonar count out so we can handle skipping past it */
  memcpy(&sonarcnt,data+PKT_SONARCNT,sizeof(sonarcnt));

  /* Check for impossible sonar update counts */
  if (sonarcnt >= SONAR_CNT) { fprintf(bot->log,"read_packet: invalid sonar count: %d\n",sonarcnt); return -1; }

  /* Calculate the offset to push us past the sonar data */
  sonar_offset = PKT_SONAROFF+(sonarcnt*PKT_SONARSIZ);
  
  memcpy(&bot->in_xpos,     data+PKT_XPOS,                sizeof(bot->in_xpos));
  memcpy(&bot->in_ypos,     data+PKT_YPOS,                sizeof(bot->in_ypos));
  memcpy(&bot->in_thpos,    data+PKT_THPOS,               sizeof(bot->in_thpos));
  memcpy(&bot->in_lvel,     data+PKT_LVEL,                sizeof(bot->in_lvel));
  memcpy(&bot->in_rvel,     data+PKT_RVEL,                sizeof(bot->in_rvel));
  memcpy(&bot->battery,     data+PKT_BATT,                sizeof(bot->battery));
  memcpy(&bot->lstall,      data+PKT_LSTALL,              sizeof(bot->lstall));
  memcpy(&bot->rstall,      data+PKT_RSTALL,              sizeof(bot->rstall));
  memcpy(&bot->in_control,  data+PKT_CNRTL,               sizeof(bot->in_control));
  memcpy(&bot->flags,       data+PKT_FLAGS,               sizeof(bot->flags));
  memcpy(&bot->compass,     data+PKT_COMPASS,             sizeof(bot->compass));
  /* Sonar data comes here, so we have to adjust */
  memcpy(&bot->grip_state,  data+PKT_GRIP  +sonar_offset, sizeof(bot->grip_state));
  memcpy(&bot->anport,      data+PKT_ANPORT+sonar_offset, sizeof(bot->anport));
  memcpy(&bot->analog,      data+PKT_ANALOG+sonar_offset, sizeof(bot->analog));
  memcpy(&bot->digin,       data+PKT_DIGIN +sonar_offset, sizeof(bot->digin));
  memcpy(&bot->digout,      data+PKT_DIGOUT+sonar_offset, sizeof(bot->digout));
  memcpy(&bot->batteryv,    data+PKT_BATTV +sonar_offset, sizeof(bot->batteryv));
  memcpy(&bot->chargestate, data+PKT_CHARGE+sonar_offset, sizeof(bot->chargestate));
  memcpy(&bot->rotvel,      data+PKT_ROTVEL+sonar_offset, sizeof(bot->rotvel));

  /* Adjust some values for conversion factors */
  bot->xpos    = bot->in_xpos    * XPOS_CONV;
  bot->ypos    = bot->in_ypos    * YPOS_CONV;
  bot->thpos   = bot->in_thpos   * THPOS_CONV;
  bot->lvel    = bot->in_lvel    * LVEL_CONV;
  bot->rvel    = bot->in_rvel    * RVEL_CONV;
  bot->control = bot->in_control * CONTROL_CONV;

  /* Pull out the left/right stall indicators for conveniance */
  bot->left_stall  = bot->lstall & STALL_FLAG;
  bot->right_stall = bot->rstall & STALL_FLAG;

  /* Read in the sonar data */
  for (curr_snr = 0; curr_snr < sonarcnt; curr_snr++) {
    curr_snr_offset = PKT_SONAROFF+(curr_snr*PKT_SONARSIZ);
    snr_num = data[PKT_SNR_NUM+curr_snr_offset];

    /* check for an invalid sonar number */
    if (snr_num < 0 || snr_num >= SONAR_CNT) { fprintf(bot->log,"read_packet: invalid soner index: %d\n",snr_num); continue; }

    memcpy(bot->in_sonars+snr_num,data+PKT_SNR_RNGE+curr_snr_offset,sizeof(bot->in_sonars[snr_num]));

    /* Apply conversion factor for sonar data */
    /* bot->sonars[snr_num] = bot->in_sonars[snr_num] * SONAR_CONV; */
    /* Sonar values tested and appear to be accurate today. */
    bot->sonars[snr_num] = bot->in_sonars[snr_num];

    /* Mark as having been updated by this packet */
    bot->sonar_updates[snr_num] = bot->packet_id;
  }

  return 1;
}

int r_sync(robot *bot)
{
  SYNC;
  int sync_step = 0, ans;

  for (sync_step = 0; sync_step < sizeof(sync); ) {
    cmd(bot,sync[sync_step],0,0,NULL);
    ans = read_packet(bot);
    fprintf(bot->log,"r_sync: step: %d, value: %d\n",sync_step,sync[sync_step]);
    fprintf(bot->log,"r_sync: read_packet result: %d\n",ans);
    if (ans == sync[sync_step]) { sync_step++; } else { sync_step = 0; sleep(1); }
  }

  /* Print out the name, type, subtype. */
  fprintf(bot->log,"Name: %s\n",bot->name);
  fprintf(bot->log,"Type: %s\n",bot->type);
  fprintf(bot->log,"SubType: %s\n",bot->subtype);

  /* After we have sync'd, call open to get things started. */
  r_open(bot);

  bot->packet_id = 0;

  return 1;
}

int init_bot_log(robot *bot, char *logfile) {

  /* Clean up our data structure */
  memset(bot,0,sizeof(robot));

  bot->packet_id = -1;
  bot->dev_fd = -1;

  for (int sonar = 0; sonar < SONAR_CNT; sonar++) {
    bot->sonar_updates[sonar] = -1;
  }

  if (logfile) {
    bot->log = fopen(logfile,"w");
    setlinebuf(bot->log);
  }

  return 1;
}

int init_bot(robot *bot) {
  return (init_bot_log(bot,NULL));
}

int open_serial_dev(robot *bot, char *dev,int in_baud) {
  int baud;
  struct termios newtio; 

  /* Switch to baud rates defined by termios. */
  switch (in_baud) {
    case 115200: baud = B115200; break;
    case 57600:  baud = B57600;  break;
    case 38400:  baud = B38400;  break;
    case 19200:  baud = B19200;  break;
    case 9600:   baud = B9600;   break;
    default: fprintf(stderr,"ERROR!: Unknown baud rate: %d\n",in_baud);
             return -1;
             break;
  }

  /* Attempt to open the device read/write */
  bot->dev_fd = open(dev,O_RDWR|O_NONBLOCK);
  if (bot->dev_fd == -1) {
    perror("Unable to open device");
    return(-1);
  }

  /* Save the current serial dev settings */
  tcgetattr(bot->dev_fd, &newtio);
        
  /* Set the input/output baud rates for this device */
  cfsetispeed(&newtio, baud);
  cfsetospeed(&newtio, baud);

  /* CLOCAL:      Local connection (no modem control) */
  /* CREAD:       Enable the receiver */
  newtio.c_cflag |= (CLOCAL | CREAD);

  /* PARENB:      Use NO parity */
  /* CSTOPB:      Use 1 stop bit */
  /* CSIZE:       Next two constants: */
  /* CS8:         Use 8 data bits */
  newtio.c_cflag &= ~PARENB;
  newtio.c_cflag &= ~CSTOPB;
  newtio.c_cflag &= ~CSIZE;
  newtio.c_cflag |= CS8;

  /* Disable hardware flow control with:
   * newtio.c_cflag &= ~(CRTSCTS);
   * if necessary.
   */

  /* ICANON:      Disable Canonical mode */
  /* ECHO:        Disable echoing of input characters */
  /* ECHOE:       Echo erase characters as BS-SP-BS */
  /* ISIG:        Disable status signals */
  /* Use if necessary:
   * newtio.c_lflag = (ECHOK);
   */

  /* IGNPAR:      Ignore bytes with parity errors */
  /* ICRNL:       Map CR to NL (otherwise a CR input on the other computer will not terminate input) */
  /* Use if necessary:
   * newtio.c_iflag |= (IGNPAR | ICRNL);
   */
  /* do not ignore CRs */
  /* newtio.c_iflag &= ~IGNCR; */
  newtio.c_iflag |= (IGNPAR | IGNBRK); 
      
  /* NO FLAGS AT ALL FOR OUTPUT CONTROL  -- Sean */
  newtio.c_oflag = 0;

  /* IXON:        Disable software flow control (incoming) */
  /* IXOFF:       Disable software flow control (outgoing) */
  /* IXANY:       Disable software flow control (any character can start flow control */
  newtio.c_iflag &= ~(IXON | IXOFF | IXANY);

  /* NO FLAGS AT ALL FOR LFLAGS  -- Sean*/
  newtio.c_lflag = 0;

  /* The following settings are deprecated and we are no longer using them (~Peter)
   * cam_data.newtio.c_lflag &= ~(ICANON && ECHO && ECHOE && ISIG); 
   * cam_data.newtio.c_lflag = (ECHO);
   * cam_data.newtio.c_iflag = (IXON | IXOFF);
   */
  /* Raw output
   * cam_data.newtio.c_oflag &= ~OPOST;
   */

  /* Clean the modem line and activate new dev settings */
  tcflush(bot->dev_fd, TCIOFLUSH);
  tcsetattr(bot->dev_fd, TCSANOW, &newtio);

  return(bot->dev_fd);
}
