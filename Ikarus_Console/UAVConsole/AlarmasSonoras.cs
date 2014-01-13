using System;
using System.Collections.Generic;
using System.Text;
using System.Media;
using System.Threading;

namespace UAVConsole
{
    class AlarmasSonoras
    {
        const int Period = 100;
        const int TO_Altitude = 500 / Period; // (2hz)
        const int TO_Baterias = 10000 / Period; // 10000ms/100ms =10 (cada 10 segundos)
        const int TO_Distance = 20000 / Period; // (cada 20 segundos)


        Singleton me = Singleton.GetInstance();
        bool playing = false;

        int timeoutAlarmMotor = 0;
        int timeoutAlarmVideo = 0;
        int timeoutDistance = 0;
        int timeoutAltitude = 0;

        Timer timer;
        
        public AlarmasSonoras()
        {
            
        }
        public void Start()
        {
            timer = new Timer(new TimerCallback(CheckSoundAlarms), null, 1000, 100);
        }

        public void Stop()
        {
            timer.Dispose();
        }

        public void Play(SoundPlayer player)
        {
           playing = true;

            new Thread(new ParameterizedThreadStart(
                delegate
                {
                    player.PlaySync();
                    playing = false;
                }
                )).Start();
        }

        void CheckSoundAlarms(Object obj)
        {
            float distance = 0.0f;

            if (me.AlarmAscenso_enabled && me.planeState.vertSpeed > 0)
                System.Media.SystemSounds.Beep.Play();

            if (!playing && me.planeState!=null)
            {
                if (me.AlarmAltitude_enabled && me.planeState.Alt < me.AlarmAltitude && timeoutAltitude==0)
                {
                    SoundPlayer sound = new SoundPlayer("AlarmAltitude.wav");
                    sound.Play();
                    timeoutAltitude = TO_Altitude;

                }
                /*
            else if (me.AlarmFastDescentRate_enabled && -me.planeState.vertSpeed > me.AlarmFastDescentRate && timeoutAltitude==0)
            {
                SoundPlayer sound = new SoundPlayer("AlarmAltitude.wav");
                sound.Play();
                timeoutAltitude = TO_Altitude;

            }/* */
                else if (me.AlarmCellVoltage_enabled && (me.planeState.v1 / me.cells1) < me.AlarmCellVoltage &&
                    timeoutAlarmMotor ==0)
                {
                    SoundPlayer sound = new SoundPlayer("AlarmBateriaMotor.wav");
                    sound.Play();
                    timeoutAlarmMotor = TO_Baterias;
                }
                else if (me.AlarmCellVoltage_enabled && (me.planeState.v2 / me.cells2) < me.AlarmCellVoltage && 
                    timeoutAlarmVideo ==0)
                {
                    SoundPlayer sound = new SoundPlayer("AlarmBateriaVideo.wav");
                    sound.Play();
                    timeoutAlarmVideo = TO_Baterias;
                }
                else if (me.AlarmDistance_enabled && distance >me.AlarmDistance &&timeoutDistance ==0)
                {
                    SoundPlayer sound = new SoundPlayer("AlarmDistance.wav");
                    sound.Play();
                    timeoutDistance = TO_Distance;
                }
            }

            if (timeoutAlarmVideo > 0)
                timeoutAlarmVideo--;
            if (timeoutAlarmMotor > 0)
                timeoutAlarmMotor--;
            if (timeoutDistance > 0)
                timeoutDistance--;
            if (timeoutAltitude > 0)
                timeoutAltitude--;

        }
    }
}
