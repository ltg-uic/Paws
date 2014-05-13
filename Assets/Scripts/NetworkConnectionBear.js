private var remoteIP: String = "127.0.0.1";
private var remotePort: int = 25000;
private var listenPort: int = 25000;
private var useNAT = false;

var NumWalkSteps:int;
var NumSwimSteps:int;
var gameDuration:int;
var playerNumber:int;

var yearList : int[] = [1975,2010,2045];

private var currentYear: String;
private var yearsAgo: String;
private var gameStarted: boolean = false;


function Start(){
	LoadIPServer();	
	currentYear = "1";
}
function Update(){
	//ConnectToServer();
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
	ConnectToServer();
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
function ConnectToServer(){
  if (Network.peerType == NetworkPeerType.Disconnected){	
//	yield WaitForSeconds(15.0);
	Network.Connect(remoteIP, remotePort);
	Debug.Log("After 15 secs... ");

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
	gameDuration = parseInt(values[2]);
	
	 //Called on the Clients only
    Debug.Log("gameDUration "+ gameDuration);  
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
