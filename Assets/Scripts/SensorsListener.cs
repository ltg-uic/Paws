using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class SensorsListener : MonoBehaviour {

	// receiving Thread
    Thread receiveThreadR; 
	Thread receiveThreadL; 

    // udpclient object
    UdpClient clientL;
    UdpClient clientR;	

    // public string IP = "127.0.0.1"; default local

    public int portR = 5555; // define > init
	public int portL = 5556; // define > init

    // infos

    public string lastReceivedUDPPacketL="";
    public string allReceivedUDPPacketsL=""; // clean up this from time to time!
    public string lastReceivedUDPPacketR="";
    public string allReceivedUDPPacketsR=""; // clean up this from time to time!
    
	private string[] valuesL;
	private string[] valuesR;
 	private float accYL = 0.00f;
	private float accZL = 0.00f;   
	private float accYR = 0.00f;
	private float accZR = 0.00f;   
	double prevAccYL;
	double prevAccYR;

    private static void Main() 

    {
        SensorsListener receiveObj=new SensorsListener();

        receiveObj.init();
        string text="";

        do
        {
             text = Console.ReadLine();
        }

        while(!text.Equals("exit"));

    }

    // start from unity3d

    public void Start()

    {
       init();
    }
	
    void Update(){
	
		//Debug.Log("Received "+lastReceivedUDPPacket);
		
	    if (lastReceivedUDPPacketL.Length > 0){
			valuesL = lastReceivedUDPPacketL.Split(","[0]);
		      
			if (Math.Abs(Math.Abs(double.Parse(valuesL[3])) - Math.Abs(prevAccYL)) > 0.03){
				accYL = Mathf.Round(float.Parse(valuesL[3])*100)/100;
			    accZL = Mathf.Round(float.Parse(valuesL[4])*100)/100;
		
				SendMessage("yValueL",accYL);
				SendMessage("zValueL",accZL);
				prevAccYL = accYL;
			}
		}
		 if (lastReceivedUDPPacketR.Length > 0){
			valuesR = lastReceivedUDPPacketR.Split(","[0]);
		      
			if (Math.Abs(Math.Abs(double.Parse(valuesR[3])) - Math.Abs(prevAccYR)) > 0.03){
				accYR = Mathf.Round(float.Parse(valuesR[3])*100)/100;
			    accZR = Mathf.Round(float.Parse(valuesR[4])*100)/100;
		
				SendMessage("yValueR",accYR);
				SendMessage("zValueR",accZR);
				prevAccYR = accYR;
			}
		}
	}
	
	
    // init

    private void init()

    {

        // Endpunkt definieren, von dem die Nachrichten gesendet werden.

        print("UDPSend.init()");
        print("Sending to 127.0.0.1 Right: "+portR);

        receiveThreadR = new Thread(

            new ThreadStart(ReceiveDataR));

        receiveThreadR.IsBackground = true;

        receiveThreadR.Start();

        print("Sending to 127.0.0.1 Left: "+portL);

        receiveThreadL = new Thread(

            new ThreadStart(ReceiveDataL));

        receiveThreadL.IsBackground = true;

        receiveThreadL.Start();

    }
    // receive thread 

    private  void ReceiveDataL()
    {
        clientL = new UdpClient(portL);		
        while (true) 
        {
          try 
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

                byte[] data = clientL.Receive(ref anyIP);

                string text = Encoding.UTF8.GetString(data);

                // latest UDPpacket

                lastReceivedUDPPacketL = text;
		        allReceivedUDPPacketsL = allReceivedUDPPacketsL+text;

            }
            catch (Exception err) 
            {
                print(err.ToString());
            }
        }
    }
	
	private  void ReceiveDataR()
    {
        clientR = new UdpClient(portR);		
        while (true) 
        {
          try 
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

                byte[] data = clientR.Receive(ref anyIP);

                string text = Encoding.UTF8.GetString(data);

                // latest UDPpacket

                lastReceivedUDPPacketR = text;
		        allReceivedUDPPacketsR = allReceivedUDPPacketsR+text;

            }
            catch (Exception err) 
            {
                print(err.ToString());
            }
        }
    }
    // getLatestUDPPacket
    // cleans up the rest
    public string getLatestUDPPacketL()
	{
        allReceivedUDPPacketsL ="";
        return lastReceivedUDPPacketL;
    }
	
    public string getLatestUDPPacketR()
	{
        allReceivedUDPPacketsR ="";
        return lastReceivedUDPPacketR;
    }
	
	void OnDisable(){
		if (receiveThreadL!=null){
			receiveThreadL.Abort();
			clientL.Close();
		}
	    if (receiveThreadR!=null){
			receiveThreadR.Abort();
			clientR.Close();
		}
	}
}
