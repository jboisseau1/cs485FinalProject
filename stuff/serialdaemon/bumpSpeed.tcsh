#! /bin/tcsh
./serialdaemon -serial $1 -port $2 -baud 19200 -debug&
echo S115200 | nc localhost $2 
sleep 0.25
kill %1
sleep 0.25
./serialdaemon -serial $1 -port $2 -baud 115200
