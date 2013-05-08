private var arduinityClass : Arduinity;

var selectedDevice : int = 0;
var numberOfDevices :int; 

var arduinoInputData : int[];

function Start () 
{

	
	arduinityClass = new Arduinity();
	
	numberOfDevices = arduinityClass.getNumArduinos ();
	
	if( numberOfDevices < 1 ) {
		print( "ERROR, can't get number of devices.");
	} else {
		print( "There are " + numberOfDevices + " devices connected");
	}		

	
	//arduinityClass.setArduinoDevice(selectedDevice);
	
	print( "Connecting to device : " + arduinityClass.getArduinoName(selectedDevice) );

	/*
	var testValue  = arduinityClass.test();
	
	if ( testValue == 2.0)
	{
		print ("Test successful !");
	}
	else 
	{
		print ("Test failed !");
	}
	*/	
		
}

function readOneByte () : int 
{
	return arduinityClass.readOneByte ();
}

function writeString ( str )
{
	arduinityClass.arduinityWrite(str);
}

function Update () 
{
	//arduinityClass.setArduinoDevice(selectedDevice);
	var onebyte = arduinityClass.readOneByteByte();
	
	var pIndex : int;
	var pData : int;

    if ( (onebyte >= 0) && (onebyte < 255) )
	{
	  
	  arduinityClass.unpackPhysicalByte ( onebyte, pIndex, pData);
	//  Debug.Log(pData);
	
	  if (pIndex==2){
	     SendMessage("setLeftStep",pData);
	  }
	  else if (pIndex ==3){
	     SendMessage("setRightStep",pData);
	  }
	  
	}
	
}