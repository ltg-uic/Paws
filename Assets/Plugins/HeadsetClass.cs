using UnityEngine;
using System.Collections;
using System; 

public class HeadsetClass {

	private Arduinity arduinity;
	private byte[] buffer;
	
	void Start () {
	}
	
	void Update () {
	}
	
	public void init() {
		arduinity = new Arduinity();
		arduinity.setArduinoDevice(0);
		
		buffer = new byte[5];
	}
	
	public void setHeadset( int headset_index) {
		arduinity.setArduinoDevice(headset_index);
	}

	public int getNumHeadsets() {
		return arduinity.getNumArduinos();
	}
	
	public String read()
	{
		String str = arduinity.read();
		Debug.Log("headset class string " + str);
		return str;
	}
	
	public int readOneByte ()
	{
		return arduinity.readOneByte ();
	}
	
	public string getHeadsetName( int device_index ) {
		return arduinity.getArduinoName( device_index );
	}
	
	/*
	public void testFloat () 
	{
		Debug.Log(arduinity.test () );
	}
	*/
	

	/* 
	   Direction should be between 1 and 8 (inclusive)
		1 = N (forhead)
          2 = NW
		3 = W (right ear)
		4 = SW
		5 = S (back of head)
		6 = SE
		7 = E (left ear)
		8 = NE
	   Distances must be between 1 and 125 (inclusive). This is because a 
	   distance of 0 would be interpreted as a null character in the 
	   string I send. The same is true of direction. */
	public void setDistance(int direction, int distance){
		if( direction < 1)
			return;
		if( direction > 8 )
			return;
		if(distance < 1)
			return;
		if( distance > 125)
			return;
				
		/* Build up a null terminated string. */
		buffer[0] = (byte)127u;
		buffer[1] = (byte)direction;
		buffer[2] = (byte)distance;
		buffer[3] = (byte)126u;
		buffer[4] = (byte)0u;

		string str;
		System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
		str = enc.GetString(buffer);

		arduinity.arduinityWrite(str);
		
		/* So, this plugin seems to be most reliable if I send the message twice.
		   It works 90% of the time if you only send the message once, but this
		   gets five nines (or something closer to that). */
		arduinity.arduinityWrite(str);
	}
}
