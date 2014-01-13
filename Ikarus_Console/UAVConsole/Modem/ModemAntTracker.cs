using System;
using System.Collections.Generic;
using System.Text;
using UAVConsole.USBXpress;
using System.Windows.Forms;

namespace UAVConsole.Modem
{
    class ModemAntTracker : ModemAbstract
    {
        Timer timer;
        AntTracker anttracker;
        bool locked;

        public ModemAntTracker(AntTracker antTracker) :base ()
        {
            anttracker = antTracker;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 50;   //ms
            timer.Tick += new EventHandler(timer_Tick);
            timer.Enabled = true;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (anttracker != null)
            {
                if (!locked)
                {
                    locked = true;

                    byte[] buffer = anttracker.ReadModem();
                    ParsePacket(buffer);
                    listeners.Invoke();
                    locked = false;
                }
            }
        }
        
        override public void dispose()
        {
            timer.Enabled = false;
            base.dispose();
        }
    }
}
