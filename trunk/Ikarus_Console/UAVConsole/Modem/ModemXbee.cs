/* 
 * (c) 2010 Rafael Paz <rpaz@atc.us.es>
 *
 *************
 *
 *	This file is part of IKARUS_OSD.
 *
 *  IKARUS_OSD is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  IKARUS_OSD is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with IKARUS_OSD.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace UAVConsole.Modem
{
    public class ModemXbee: ModemAbstract
    {
        SerialPort com;
        int buffer_index=0;
        
        enum Estado{ Wait_A, Wait_T, Wait_C, ReceiveData };
        Estado st = Estado.Wait_A;

        bool locked = false;
        bool captured = false;
        Timer timer;

        public ModemXbee()
            : this(Singleton.GetInstance().commPort, Singleton.GetInstance().commBps)
        {

        }

        public ModemXbee(string portname, int bauds) : base()
        {
            com = new SerialPort(portname, bauds, Parity.None, 8, StopBits.One);
            com.DataReceived += new SerialDataReceivedEventHandler(com_DataReceived);
            com.Open();

            timer = new Timer(new TimerCallback(timer_Tick), null, 0, 100);
            
        }


        void timer_Tick(Object stateObject)
        {
            if (!locked)
            {
                locked = true;
                captured = false;
                while (captured == false)
                    Thread.Sleep(100);

                listeners.Invoke();
                locked = false;
            }
        }

        override public void dispose()
        {
            com.Close();
            base.dispose();
        }

        ~ModemXbee()
        {
            dispose();
        }

        void com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int valor;
            while (com.BytesToRead > 0)
            {
                valor = com.ReadByte();
                if (st == Estado.Wait_A)
                    if (valor == 'A')
                        st = Estado.Wait_T;
                    else
                        st = Estado.Wait_A;
                else if (st == Estado.Wait_T)
                    if (valor == 'T')
                        st = Estado.Wait_C;
                    else
                        st = Estado.Wait_A;
                else if (st == Estado.Wait_C)
                    if (valor == 'C')
                        st = Estado.ReceiveData;
                    else
                        st = Estado.Wait_A;
                else if (st == Estado.ReceiveData)
                {
                    packet[buffer_index++] = (byte)valor;
                    if (buffer_index >= DATALEN)
                    {
                        if (ParsePacket(packet))
                        {
                            captured = true;
                            //listeners.Invoke();
                        }
                        st = Estado.Wait_A;
                        buffer_index = 0;
                    }
                }
            }                        
        }

    }
}
