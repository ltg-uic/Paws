private var remoteIP: String = "127.0.0.1";
private var remotePort: int = 25000;
private var listenPort: int = 25000;
private var useNAT = false;
private var server: boolean = false;

var NumWalkSteps:int;
var NumSwimSteps:int;

var playerNumber:int;
private var currentYear: String;
private var yearsAgo: String;
private var gameStarted: boolean = false;

function Start(){
	LoadIPServer();	
	currentYear = "2010";
}

function LoadIPServer(){
    if (PlayerPrefs.HasKey("ipServer")){	
		var serverIP = PlayerPrefs.GetString("ipServer");
	  	remoteIP = PlayerPrefs.GetString("ipServer");
  		remotePort = int.Parse(PlayerPrefs.GetString("remotePort"));
  		listenPort = int.Parse(PlayerPrefs.GetString("listenPort"));
  	}else{
  		remoteIP = "127.0.0.1";
  		remotePort = 25000;
  		listenPort = 25000;
  	}
}

function UpdateIPServer(){ 
   PlayerPrefs.SetString("ipServer",remoteIP);
   PlayerPrefs.SetString("remotePort",remotePort.ToString());
   PlayerPrefs.SetString("listenPort",listenPort.ToString());
   PlayerPrefs.Save();
   Debug.Log("NetworkConnectionBear::Updated network settings..");  
}

function OnConnectedToServer () {
    Debug.Log("Connected to Server... ");
    UpdateIPServer();
	// Notify our objects that the level and the network are ready
	for (var go : GameObject in FindObjectsOfType(GameObject))
		go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
}

function OnGUI () {
	if (!Application.loadedLevelName.Equals("StudyScene")){
		// Checking if you are connected to the server or not
		if (Network.peerType == NetworkPeerType.Disconnected){
			// If not connected
	
			if (server){
				// Creating server
				Network.InitializeServer(10, listenPort,useNAT);
				// Notify our objects that the level and the network is ready
				for (var go : GameObject in FindObjectsOfType(GameObject)){
					go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
				}
			}
			else {
				GUILayout.BeginArea (Rect (20,20,120,200));
				remoteIP = GUILayout.TextField(remoteIP,15);
				
				if (GUILayout.Button("Connect to Mobile Interpretation Tool")){
					Network.Connect(remoteIP, remotePort);
				}
				GUILayout.EndArea();
			}		
		}
		else{
		/*
			// Getting your ip address and port
			ipaddress = Network.player.ipAddress;
			port = Network.player.port.ToString();
			GUI.Label(new Rect(140,20,250,40),"IP Adress: "+ipaddress+":"+port);
			if (GUI.Button (new Rect(10,10,100,50),"Disconnect")){
				// Disconnect from the server
				Network.Disconnect(200);
			}*/
	
	
	  
			if (server)
				GUI.Label(new Rect(180,50,250,40), "Received from client "+playerNumber+" : " + NumWalkSteps +", " +NumSwimSteps);
			
		}
	}
}



function OnPlayerConnected(newPlayer: NetworkPlayer){
	//Called on the server only
    playerNumber = int.Parse (newPlayer.ToString());        
}

@RPC
function ReceivedMovementInput (_steps: String)
{
	 //Called on the server only
    Debug.Log("ReceivedMovementInput ");
}
@RPC
function SendEventId (_id: int)
{
	 //Called on the server only
    Debug.Log("ReceivedMovementInput ");
}
@RPC
function ReceivedFinishedLevel(_calories:String)
{
	 //Called on the server only
    Debug.Log("Reach Goal..... ");
}
@RPC
function SaveLogDocent(_logLine:String)
{
	 //Called on the Clients only
    GetComponent(AppendToLogDocents).AppendDataToLog(_logLine);
}
@RPC
function LoadLevelInClient (_level: String ,  info : NetworkMessageInfo)
{
    gameStarted = true;
	var values = _level.Split(":"[0]);
		
	currentYear = values[0];
	yearsAgo = values[1];
	
	 //Called on the Clients only
    Debug.Log("Load level "+ currentYear);  
	SendMessage("LoadYear",currentYear);
    SendMessage("StartGame");
    	  	
}

@RPC
function FinishGame (_calories: float ,  info : NetworkMessageInfo)
{
	 //Called on the Clients
	 if (   gameStarted){
	    gameStarted = false;
	    SendMessage("HideInitialMessage");
	    Debug.Log("Finish "+ currentYear); 
	    SendMessage("GameOver",_calories);
	  }
   
}
@RPC
function StopGame (_calories: float ,  info : NetworkMessageInfo)
{
	 //Called on the Clients
	  if (gameStarted){
	    gameStarted = false;
	    SendMessage("HideInitialMessage");
	    Debug.Log("Gameover "+ currentYear); 
	    SendMessage("GameOver",_calories);
	  }
   
}
@RPC
function StopGameTimeOut (_calories: float ,  info : NetworkMessageInfo)
{
	 //Called on the Clients
	  if (gameStarted){
	    gameStarted = false;
	    SendMessage("HideInitialMessage");
	    Debug.Log("timeout "+ currentYear);
	    SendMessage("GameOver",_calories); 
	    SendMessage("GameOverTimeOut",_calories);
	  }
   
}
@RPC
function SetGoalBearInMap (_xyValues: String, info : NetworkMessageInfo)
{
	 //Called on the server only
    
}
