using UnityEngine;
using System.Collections;       
using System.Runtime.InteropServices;  
using System.IO; 
using System.Text.RegularExpressions;
using System; 
//using System.IO.Ports;
/* Notes:
        This class does not derive from MonoBehavior intentionally. 
        Deriving from MonoBehavior caused errors with serial plugin
        initialization.

	   This class can't print because it's not derived from
	   MonoBehaviour.
 
        This class loads a bundle (OS X) or dll (Windows) called Arduinity, 
*/

public class Arduinity{   
	
	/* I've included these two test funtions as a means of testing if Unity<->Plugin communication
	   is working correctly. 
	   testPointer(int* ref): Pass in a pointer, and the int should be set to the value 2u */
	[DllImport("Arduinity")]
		private static extern void testPointer( ref IntPtr in_ptr );
	/* testFunction(): Returns 2.0f; */
	
	[DllImport("Arduinity")]
		private static extern float testFloat();
	/* testFunction(): Returns 2.0f; */
	
	[DllImport("Arduinity")]
		private static extern float testFunction();               
	
	[DllImport ("Arduinity")]
		private static extern int getNumDevices(); 

	[DllImport ("Arduinity")]
		private static extern string getDevice( int device_num );

	[DllImport ("Arduinity")]
		private static extern int setDevice ( int device_num, ref IntPtr serial_ptr );  

	[DllImport ("Arduinity")]
	private static extern void writingInterface( ref IntPtr serial_ptr, String out_buffer);

	[DllImport ("Arduinity")]
	private static extern int getNumberOfBytesToRead ( ref IntPtr serial_ptr);

	[DllImport ("Arduinity")]
	private static extern int readOneByte ( ref IntPtr serial_ptr);
	
	[DllImport ("Arduinity")]
	private static extern byte readOneByteByteOnly ( ref IntPtr serial_ptr);

	[DllImport ("Arduinity")]
	private static extern String arduinityRead( ref IntPtr serial_ptr, ref String in_buffer, ref int error_flag);

	[DllImport ("Arduinity")]
	private static extern void closeDevice( ref IntPtr serial_ptr);
			
	/* This is the serial writer handle created by an open call in C. */								
	private IntPtr device_ptr;
	private IntPtr error_flag; // jc added for testing serial read
	private IntPtr s;

	public Arduinity(){
	    	/* 0 almost always works. */ 
	
		setArduinoDevice(0); 
	}

	~Arduinity(){
		closeArduinoDevice();
	}

	public void Update (){            

	}        	
	   
	/* isConnected returns true if there is a device connected, and false if there isn't. */
	public bool isConnected(){
		if( device_ptr != IntPtr.Zero ){
			return true;
		} 
		else{
			return false;
		}
	}    
	   
	public int getNumArduinos(){
		return getNumDevices();
	}  
	
	public string getArduinoName( int i){
		return getDevice(i);
	}

	/* Allows the program to later change the device, or set the device after the program has started  
	   You should usually be fine with 0, (unless there are multiple FTDI chips ). */
	public void setArduinoDevice( int in_index ){  
		Debug.Log("Entro" + getNumDevices());
		
		setDevice( in_index, ref device_ptr ); 
	}

	/* Write a null terminated string to the Arduino. Make sure it has the right encoding.
	   Use System.Text.ASCIIEncoding */
	public void arduinityWrite( String str ){
		writingInterface( ref device_ptr, str);
	}

	/* This just calls standard C function close on the pointer. */
	public void closeArduinoDevice() {
		closeDevice(ref device_ptr);
	}
	
	public String read() {
		
		
		// THIS DOES NOT WORK 
		
	//	String in_buffer = "";
		String str = "";
		//int error_flag = 0;
		
		int numberOfBytesToRead = getNumberOfBytesToRead ( ref device_ptr );
		Debug.Log ( "number of bytes to read : " + numberOfBytesToRead );
		
		int readByte = readOneByte ( ref device_ptr );
		Debug.Log ( "byte read = " + readByte );
		//str = readingInterface ( ref device_ptr, ref in_buffer);
		//Debug.Log ( "string length " +str );
		//Debug.Log ( "string lenght : " + str.Length );
		
		//Debug.Log ( "read string : " + str );
		//str = arduinityRead ( ref device_ptr, ref in_buffer, ref error_flag );
		//Debug.Log("error flag " + error_flag + " " + "string " + str + "buffer " + in_buffer);
		
		return str;
		
	}
	
	public int readOneByte () 
	{
		// using the below line does not unpack the data
		return readOneByte ( ref device_ptr );
		
	}
	
    public byte readOneByteByte () 
	{
		return readOneByteByteOnly(ref device_ptr);
	}
	
	public void unpackPhysicalByte ( byte physicalByte, ref int physicalInputIndex, ref int physicalInputData )
	{
		// 224 = 11100000
		// 31 = 00011111
		//Debug.Log("Arduinity:" + physicalByte);
		physicalInputIndex = (physicalByte & 224) >> 5;
		physicalInputData = physicalByte & 31;
	    
		//Debug.Log(physicalByte + " " + physicalInputIndex + " " + physicalInputData);
	}
	
	/*	
	public float test () 
	{
		return testFloat ();
	}
	*/
		

}

