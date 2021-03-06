﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tesla_Soft {
    class TankMotors {
        /*
         
            Pola     
             
        */
        // martwe pole na środku mouseImage
        private const int deadBoxCursor = 10;
        // mnoznik px myszki aby otrzymac predkosc 
        private const float mouseRatio = 2.0f;
        
        public Motor MotorL { get; set; }
        public Motor MotorR { get; set; }
        // jesli poprzedni stan to Stop
        private bool fromStop = false;
        // mozliwa predkosc maksymalna
        private int _maxSpeed;
        public int maxSpeed {
            get {
                return _maxSpeed;
            }
            set {
               
                if (value > 255) _maxSpeed = 255;
                else if (value < 0) _maxSpeed = 0;
                else _maxSpeed = value;
                MotorL.maxSpeed = _maxSpeed;
                MotorR.maxSpeed = _maxSpeed;
            }
        } // END public int maxSpeed
        
        /*
         
            Metody     
             
        */
        public void Accelerate(int delta, int deltaToGreater, int brakeDelta) {
            fromStop = false;
            int deltaL = delta, deltaR = delta;
           if (MotorL.direction == MotorR.direction) {
                if(MotorL < MotorR) {
                    deltaL = delta + deltaToGreater;
                } else if(MotorL > MotorR) {
                    deltaR = delta + deltaToGreater;  
                }
            } 
            if (delta > 0) {
                if (MotorL.direction == Direction.Backward) MotorL.Accelerate(deltaL+brakeDelta);
                else MotorL.Accelerate(deltaL);
                if(MotorR.direction == Direction.Backward) MotorR.Accelerate(deltaR + brakeDelta);
                else MotorR.Accelerate(deltaR);
            } else if (delta < 0) {
                if(MotorL.direction == Direction.Forward) MotorL.Accelerate(deltaL + brakeDelta);
                else MotorL.Accelerate(deltaL);
                if(MotorR.direction == Direction.Forward) MotorR.Accelerate(deltaR + brakeDelta);
                else MotorR.Accelerate(deltaR);
            }
        } // END public void Accelerate

        public void Brake(int delta, int deltaToLess) {
            MotorL.Brake(delta);
            MotorR.Brake(delta);
            if (MotorL.direction == MotorR.direction) {
                if (MotorL > MotorR) MotorL.Brake(deltaToLess);
                else if(MotorL < MotorR) MotorR.Brake(deltaToLess);
            }
        } // END public void Brake

        public void LBackRFront(int delta) {
            MotorL.Accelerate(-delta);
            MotorR.Accelerate(delta);
        }
        public void Left(int delta, float stopTurningRatio) {
            int stopDelta = (int)(Math.Abs(delta) * stopTurningRatio);
            if(MotorL.deltaAbsSpeed(MotorR) + delta < maxSpeed) {
                switch(MotorL.direction) {
                    case Direction.Stop: {
                        LBackRFront(stopDelta);
                        fromStop = true;
                    }
                    break;
                    case Direction.Forward: {
                        MotorL.Accelerate(-Math.Abs(delta));
                    }
                    break;
                    case Direction.Backward: {
                        if(fromStop.Equals(true)) {
                            LBackRFront(stopDelta);
                        } else MotorL.Accelerate(Math.Abs(delta));
                    }
                    break;
                }
            }
        } // END public void Left
        public void Right(int delta, float stopTurningRatio) {
            int stopDelta = -(int)(Math.Abs(delta) * stopTurningRatio);
            if(MotorR.deltaAbsSpeed(MotorL) + delta < maxSpeed) {
                switch(MotorR.direction) {
                    case Direction.Stop: {
                        LBackRFront(stopDelta);
                        fromStop = true;
                    }
                    break;
                    case Direction.Forward: {
                        MotorR.Accelerate(-Math.Abs(delta));
                    }
                    break;
                    case Direction.Backward: {
                        if(fromStop.Equals(true)) {
                            LBackRFront(stopDelta);
                        } else MotorR.Accelerate(Math.Abs(delta));
                    }
                    break;
                }
            }

        } // END public void Right
        public void MoveFromCursor(Point cursorStart, Point cursorPoint) {
            int dX = cursorPoint.X - cursorStart.X;
            int dY = cursorPoint.Y - cursorStart.Y;
            Point deltaCursor = MovePoint.PointMinusDeadBox(new Point((int)(dX * mouseRatio), -(int)(dY * mouseRatio)), new Point(deadBoxCursor, deadBoxCursor));
            // MotorL.speed = (int)(Math.Abs(dY - deadBoxCursor) * mouseRatio);
            // MotorR.speed = (int)(Math.Abs(dY - deadBoxCursor) * mouseRatio);
            bool bStop = false;
            Console.WriteLine(deltaCursor);
            // skrety
            MotorL.speedDirection = deltaCursor.Y;
            MotorR.speedDirection = deltaCursor.Y;

            if(MotorL.direction != Direction.Stop) {
                MotorL.absSpeed += MovePoint.onlyMinus(deltaCursor.X);

            } //else MotorL.speedDirection = deltaCursor.X;

            if(MotorR.direction != Direction.Stop) {
                MotorR.absSpeed += MovePoint.onlyPlus(deltaCursor.X);
            } //else MotorL.speedDirection = -deltaCursor.X;


            /*while(MotorL.deltaSpeedDirection(MotorR) > maxSpeed) {
                if(MotorL > MotorR) MotorR.speedDirection += 1;
                else if(MotorL < MotorR) MotorL.speedDirection += 1;
            }*/
            /*if (deltaCursor.X < 0) {
                MotorL.speed = Math.Abs(deltaCursor.X);
                MotorL.direction = Direction.Backward;
            } else if (deltaCursor.X > 0) {
                MotorR.speed = Math.Abs(deltaCursor.X);
                MotorR.direction = Direction.Backward;
            }
            */


            // do tylu
            /*if (dY > 0) {
                if (((dY - deadBoxCursor) > 0)) {
                    MotorL.speed = (int)((dY - deadBoxCursor) * mouseRatio);
                    MotorR.speed = (int)((dY - deadBoxCursor) * mouseRatio);
                    MotorL.direction = Direction.Backward;
                    MotorR.direction = Direction.Backward;
                } else {
                    bStop = true;
                }
            // do przodu
            } else if (((dY + deadBoxCursor) < 0)) {
                MotorL.speed = (int)(Math.Abs(dY + deadBoxCursor) * mouseRatio);
                MotorR.speed = (int)(Math.Abs(dY + deadBoxCursor) * mouseRatio);
                MotorL.direction = Direction.Forward;
                MotorR.direction = Direction.Forward;
            } else bStop = true;*/

            if (bStop == true) {
                MotorL.Brake(maxSpeed);
                MotorR.Brake(maxSpeed);
           }
            /*// w prawo
            int newSpeed;
            if (dX > 0) {
                if (((dX - deadBoxCursor) > 0)) {
                    switch (MotorL.direction) {
                        case Direction.Stop: {
                            MotorR.speed = (int)((dX - deadBoxCursor) * mouseRatio);
                            MotorR.direction = Direction.Backward;
                        }
                        break;
                        case Direction.Forward: {
                            newSpeed = MotorR.speed - (int)((dX - deadBoxCursor) * mouseRatio);
                            MotorR.speed = Math.Abs(newSpeed);
                            if (newSpeed > 0) MotorR.direction = Direction.Forward;
                            else MotorR.direction = Direction.Backward;
                        }
                        break;
                        case Direction.Backward: {
                            newSpeed = MotorR.speed + (int)((dX - deadBoxCursor) * mouseRatio);
                            MotorR.speed = Math.Abs(newSpeed);
                            if (newSpeed > 0) MotorR.direction = Direction.Backward;
                            else MotorR.direction = Direction.Forward;
                        }
                        break;
                    }
                }
                // w lewo
            } else if (((dX + deadBoxCursor) < 0)) {
                switch (MotorR.direction) {
                    case Direction.Stop: {
                        MotorL.speed = (int)(Math.Abs(dX + deadBoxCursor) * mouseRatio);
                        MotorL.direction = Direction.Backward;
                    } break;
                    case Direction.Forward: {
                        newSpeed = MotorL.speed - (int)(Math.Abs(dX + deadBoxCursor) * mouseRatio);
                        MotorL.speed = Math.Abs(newSpeed);
                        if (newSpeed > 0) MotorL.direction = Direction.Forward;
                        else MotorL.direction = Direction.Backward;
                    }
                    break;
                    case Direction.Backward: {
                        newSpeed = MotorL.speed + (int)(Math.Abs(dX + deltaCursor) * mouseRatio);
                        MotorL.speed = Math.Abs(newSpeed);
                        if (newSpeed > 0) MotorL.direction = Direction.Backward;
                        else MotorL.direction = Direction.Forward;
                    }
                    break;
                }
            }*/

        } // END public void MoveFromCursor
        /*
         
            Konstruktory     
             
        */
        public TankMotors(int maxSpeed = 255) {
            MotorL = new Motor();
            MotorR = new Motor();
            this.maxSpeed = maxSpeed;
        }
        public TankMotors(Motor MotorL, Motor MotorR, int maxSpeed = 255) {
            this.MotorL = MotorL;
            this.MotorR = MotorR;
            this.maxSpeed = maxSpeed;
        }

    }
}
