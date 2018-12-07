using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

namespace Robot
{
    public interface IRobot
    {
        void initialize(string commSettings);
        bool open();
        void close();

        void enableMotors(bool arg);
        void enableSonar(bool arg);
        void resetDeadReckoning();

        bool isOpenned();
        bool isMoving();

        string getName();
        string getType();
        string getSubType();

        double getCompass();
        double getBattery(); // volts
        double getLeftVelocity(); // m/s
        double getRightVelocity(); // m/s
        PointF getIntegratedPosition(); // m, m
        double getIntegratedTheta(); // degrees
        int getSonarValue(int sonar);

        void sound( short arg );

        void stop();
        void emergencyStop();
        
        void setVelocity(byte left, byte right);
        void setHeadingAbsolute(short arg);
        void setHeadingRelative(short arg);

        double[] getSonarAngles();
        double getSonarMaxDistance();
    }
}
