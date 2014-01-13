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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UAVConsole.Modem;

namespace UAVConsole
{
	public class MyWebServer 
	{
        Singleton me = Singleton.GetInstance();

        const string DAEfilename = "PlaneModel.dae";

		private TcpListener myListener ;
        private Thread th;
        private bool running=false;
        
        //The constructor which make the TcpListener start listening on the
		//given port. It also calls a Thread on the method StartListen(). 
		public MyWebServer(int port)
		{
            try
			{
				//start listing on the given port
                this.running = true;
				myListener = new TcpListener(port) ;
				myListener.Start();
				Console.WriteLine("Web Server Running... Press ^C to Stop...");
				//start the thread which calls the method 'StartListen'
				th = new Thread(new ThreadStart(StartListen));
                th.IsBackground = true;
				th.Start() ;

			}
			catch(Exception e)
			{
				Console.WriteLine("An Exception Occurred while Listening :" +e.ToString());
			}
		}
        public void Stop()
        {
            this.running = false;
            myListener.Stop();
            th.Abort();
        }

        string GetKml(string filename, string servername)
        {
            
            if (me.planeState == null)
                me.planeState = new PlaneState(); // return null;

            String kml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
            kml += "<kml xmlns=\"http://www.opengis.net/kml/2.2\">\n";

            if (filename == "Plane-Location.kml")
            {
                //<?xml version="1.0" encoding="UTF-8" standalone="no" ?>
                kml += "<Document>";

                kml += "<Placemark id=\"pm123\">";
                kml += "  <name>Model</name>";
                kml += "  <description></description>";
                kml += "<Style id=\"default\"/>";
                kml += "<Model>";
                kml += "<altitudeMode>absolute</altitudeMode>";     // habria que saberlo! ;)
                kml += "<Location>";
                kml += "<latitude>" + me.planeState.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture) + "</latitude>";
                kml += "<longitude>" + me.planeState.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture) + "</longitude>";
                kml += "<altitude>" + me.planeState.Alt.ToString(System.Globalization.CultureInfo.InvariantCulture) + "</altitude>";
                kml += "</Location>";
                kml += "<Orientation>";
                kml += "<heading>" + me.planeState.Rumbo.ToString(System.Globalization.CultureInfo.InvariantCulture) + "</heading>";
                kml += "<tilt>" + (-me.planeState.pitch).ToString(System.Globalization.CultureInfo.InvariantCulture) + "</tilt>";
                kml += "<roll>" + (-me.planeState.roll).ToString(System.Globalization.CultureInfo.InvariantCulture) + "</roll>";
                kml += "</Orientation>";
                kml += "<Scale>";
                kml += "<x>1</x>";
                kml += "<y>1</y>";
                kml += "<z>1</z>";
                kml += "</Scale>";
                kml += "<Link>";
                kml += "<href>http://" + servername + "/models/"+DAEfilename+"</href>";
                kml += "</Link>";
                kml += "</Model>";
                kml += "</Placemark>";
                kml += "</Document>";
            }
            else if (filename == "NetworkLinkControl-Update.kml")
            {

                kml += "<NetworkLinkControl>";
                kml += "  <Update>";
                kml += "    <targetHref>http://" + servername + "/Plane-Location.kml</targetHref>";
                kml += "<Change>";
                kml += "<Placemark targetId=\"pm123\">";

                kml += "<Model>";
                kml += "<Location>";
                kml += "<latitude>" + me.planeState.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture) + "</latitude>";
                kml += "<longitude>" + me.planeState.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture) + "</longitude>";
                kml += "<altitude>" + me.planeState.Alt.ToString(System.Globalization.CultureInfo.InvariantCulture) + "</altitude>";
                kml += "</Location>";
                kml += "<Orientation>";
                kml += "<heading>" + me.planeState.Rumbo.ToString(System.Globalization.CultureInfo.InvariantCulture) + "</heading>";
                kml += "<tilt>" + (-me.planeState.pitch).ToString(System.Globalization.CultureInfo.InvariantCulture) + "</tilt>";
                kml += "<roll>" + (-me.planeState.roll).ToString(System.Globalization.CultureInfo.InvariantCulture) + "</roll>";
                kml += "</Orientation>";

                kml += "</Model>";

                kml += "      </Placemark>";
                kml += "    </Change>";
                kml += "</Update>";
                kml += "</NetworkLinkControl>";
            }
            else
            {
                kml += "<Document>";
                kml += "<NetworkLink>\n";
                kml += "<name>Plane.kml</name>\n";
                kml += "<Link>\n";
                kml += "<href>http://" + servername + "/Plane-Location.kml</href>";

                kml += "</Link>\n";
                kml += "</NetworkLink>\n";

                kml += "<NetworkLink>";
                kml += "  <name>Update</name>";
                kml += "  <Link>";
                kml += "    <href>http://" + servername + "/NetworkLinkControl-Update.kml</href>";
                kml += " <refreshMode>onInterval</refreshMode>";
                kml += " <refreshInterval>1</refreshInterval>";
                kml += "</Link>";
                kml += "</NetworkLink>";
                kml += "</Document>";
            }

            kml += "</kml>\n";

            return kml;
        }

		/// <summary>
		/// This function send the Header Information to the client (Browser)
		/// </summary>
		/// <param name="sHttpVersion">HTTP Version</param>
		/// <param name="sMIMEHeader">Mime Type</param>
		/// <param name="iTotBytes">Total Bytes to be sent in the body</param>
		/// <param name="mySocket">Socket reference</param>
		/// <returns></returns>
		public void SendHeader(string sHttpVersion, string sMIMEHeader, int iTotBytes, string sStatusCode, ref Socket mySocket)
		{

			String sBuffer = "";
			
			// if Mime type is not provided set default to text/html
			if (sMIMEHeader.Length == 0 )
			{
				sMIMEHeader = "text/html";  // Default Mime Type is text/html
			}

			sBuffer = sBuffer + sHttpVersion + sStatusCode + "\r\n";
			sBuffer = sBuffer + "Server: cx1193719-b\r\n";
			sBuffer = sBuffer + "Content-Type: " + sMIMEHeader + "\r\n";
			sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
			sBuffer = sBuffer + "Content-Length: " + iTotBytes + "\r\n\r\n";
			
			Byte[] bSendData = Encoding.ASCII.GetBytes(sBuffer); 

			SendToBrowser( bSendData, ref mySocket);

			Console.WriteLine("Total Bytes : " + iTotBytes.ToString());

		}



		/// <summary>
		/// Overloaded Function, takes string, convert to bytes and calls 
		/// overloaded sendToBrowserFunction.
		/// </summary>
		/// <param name="sData">The data to be sent to the browser(client)</param>
		/// <param name="mySocket">Socket reference</param>
		public void SendToBrowser(String sData, ref Socket mySocket)
		{
			SendToBrowser (Encoding.ASCII.GetBytes(sData), ref mySocket);
		}



		/// <summary>
		/// Sends data to the browser (client)
		/// </summary>
		/// <param name="bSendData">Byte Array</param>
		/// <param name="mySocket">Socket reference</param>
		public void SendToBrowser(Byte[] bSendData, ref Socket mySocket)
		{
			int numBytes = 0;
			
			try
			{
				if (mySocket.Connected)
				{
					if (( numBytes = mySocket.Send(bSendData, bSendData.Length,0)) == -1)
						Console.WriteLine("Socket Error cannot Send Packet");
					else
					{
						Console.WriteLine("No. of bytes send {0}" , numBytes);
					}
				}
				else
					Console.WriteLine("Connection Dropped....");
			}
			catch (Exception  e)
			{
				Console.WriteLine("Error Occurred : {0} ", e );
							
			}
		}


		//This method Accepts new connection and
		//First it receives the welcome massage from the client,
		//Then it sends the Current date time to the Client.
		public void StartListen()
		{

			int iStartPos = 0;
			String sRequest;
			String sDirName;
			String sRequestedFile;
			String sErrorMessage;
			String sResponse = "";
			
			
			while(running)
			{
				//Accept a new connection
				Socket mySocket = myListener.AcceptSocket() ;

				Console.WriteLine ("Socket Type " + 	mySocket.SocketType ); 
				if(mySocket.Connected)
				{
					Console.WriteLine("\nClient Connected!!\n==================\nCLient IP {0}\n", 
						mySocket.RemoteEndPoint) ;

				

					//make a byte array and receive data from the client 
					Byte[] bReceive = new Byte[1024] ;
					int i = mySocket.Receive(bReceive,bReceive.Length,0) ;


					
					//Convert Byte to String
					string sBuffer = Encoding.ASCII.GetString(bReceive);

		
					
					//At present we will only deal with GET type
					if (sBuffer.Substring(0,3) != "GET" )
					{
						Console.WriteLine("Only Get Method is supported..");
						mySocket.Close();
						return;
					}

					
					// Look for HTTP request
					iStartPos = sBuffer.IndexOf("HTTP",1);


					// Get the HTTP text and version e.g. it will return "HTTP/1.1"
					string sHttpVersion = sBuffer.Substring(iStartPos,8);
        
					        					
					// Extract the Requested Type and Requested file/directory
					sRequest = sBuffer.Substring(0,iStartPos - 1);
        
										
					//Replace backslash with Forward Slash, if Any
					sRequest.Replace("\\","/");
                   
					//If file name is not supplied add forward slash to indicate 
					//that it is a directory and then we will look for the 
					//default file name..
					if ((sRequest.IndexOf(".") <1) && (!sRequest.EndsWith("/")))
					{
						sRequest = sRequest + "/"; 
					}


					//Extract the requested file name
					iStartPos = sRequest.LastIndexOf("/") + 1;
					sRequestedFile = sRequest.Substring(iStartPos);
					sDirName = sRequest.Substring(sRequest.IndexOf("/"), sRequest.LastIndexOf("/")-3);
                    String sMimeType = "";

                    if (sDirName == "/")
                    {
                        string tmp = mySocket.LocalEndPoint.ToString();
                        string kml = GetKml(sRequestedFile,tmp);
                        sMimeType = " application/vnd.google-earth.kml+xml";
                        if (kml == null)
                        {
                            sErrorMessage = "<H2>404 Error! File Does Not Exists...</H2>";
                            SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                            SendToBrowser(sErrorMessage, ref mySocket); ;
                        }
                        else
                        {
                            SendHeader(sHttpVersion, sMimeType, kml.Length, " 200 OK", ref mySocket);
                            SendToBrowser(kml, ref mySocket);
                        }
                    }
                    else if (sDirName.StartsWith("/models/"))
                    {
                        string dir = sDirName.Substring("/models/".Length);
                        dir=dir.Replace('/', '\\');
                        string filename = "Webserver\\" +dir+ sRequestedFile;
                        if (File.Exists(filename))
                        {
                            int iTotBytes = 0;
                            sResponse = "";
                            //sMimeType = " application/vnd.google-earth.kml+xml";
                  
                            FileStream fs = new FileStream(filename, FileMode.Open, 	FileAccess.Read, FileShare.Read);
                            //MemoryStream fs = new MemoryStream(Encoding.ASCII.GetBytes(kml));

                            BinaryReader reader = new BinaryReader(fs);
                            byte[] bytes = new byte[fs.Length];
                            int read;
                            while ((read = reader.Read(bytes, 0, bytes.Length)) != 0)
                            {
                                sResponse = sResponse + Encoding.ASCII.GetString(bytes, 0, read);
                                iTotBytes = iTotBytes + read;
                            }
                            reader.Close();
                            fs.Close();

                            SendHeader(sHttpVersion, sMimeType, iTotBytes, " 200 OK", ref mySocket);
                            SendToBrowser(bytes, ref mySocket);

                        }
                        else
                        {
                            sErrorMessage = "<H2>404 Error! File Does Not Exists...</H2>";
                            SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                            SendToBrowser(sErrorMessage, ref mySocket); ;
                        }
                    }
                    else
                    {
                        sErrorMessage = "<H2>404 Error! File Does Not Exists...</H2>";
                        SendHeader(sHttpVersion, "", sErrorMessage.Length, " 404 Not Found", ref mySocket);
                        SendToBrowser(sErrorMessage, ref mySocket); ;
                    }
                  
					mySocket.Close();						
				}
			}
		}
	}
}
