/**
 * SCIP 1.1 Java Interface for Hokuyo URG-04LX Laser Sensor.
 * Copyright 2007 - Brian Ziman [bziman(at)gmu.edu]
 *
 * Distributed under the Academic Free 3.0 License
 */
package com.brianziman.robotics;

/**
 * This runtime exception is thrown when something "bad" happens, that
 * you wouldn't normally expect, such as a broken socket or receiving an
 * EOF.
 */
public class SCIPException extends RuntimeException {
    public SCIPException(String s) {
        super(s);
    }

    public SCIPException(Exception e) {
        super(e);
    }
}
