all:
	gcc -g -Wall -std=gnu99 -fPIC -c -o robot.o robot.c
	gcc -g -Wall -std=gnu99 -fPIC -c -o robot_cmds.o robot_cmds.c
	gcc -g -Wall -std=gnu99 -shared -fPIC -o librobot.so robot.o robot_cmds.o

clean:
	@rm -f *.o *.so

