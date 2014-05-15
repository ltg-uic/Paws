var infoWidth: int = 0;
var infoHeight: int = 0;

var posX: int = 0;
var posY: int = 0;

var mySkin: GUISkin;
var infoImages: Texture2D[];
var closeImage: Texture2D;

private var _currentInfo: int = 0; 
private var _showInfo = false;
private var _logLine : String;

function Start(){
	_currentInfo = 0;
	_showInfo = false;
    
}
    
function OnGUI() {
	GUI.skin = mySkin;
	if (_showInfo){
	 	GUILayout.BeginArea(new Rect(posX, posY, infoWidth, infoHeight));
	  
		 if (GetComponent(NetworkConnectionIT).isPlaying && _showInfo){
		 	 // Dsiplay bear information on the map
	        if (infoImages[_currentInfo] != null)
				GUI.DrawTexture(new Rect(0, 0, infoWidth, infoHeight), infoImages[_currentInfo]);
	     }
	    
		//GUILayout.BeginArea(Rect (infoWidth*.927, infoHeight*0.015, infoWidth*0.07, infoHeight*0.75));
		GUILayout.BeginArea(Rect (infoWidth*.927, infoHeight*0.015, infoWidth*0.07, infoHeight*0.75));		
	    if (GUILayout.Button(closeImage,"ImageButton")){
	       _showInfo = false;
	       _logLine = "TestB|Close Info|"+GetComponent(NetworkConnectionIT).playerName()+"|"+GetComponent(NetworkConnectionIT).currentYear()+
              "|"+(_currentInfo+1)+"|"+DateTime.Now;
           networkView.RPC ("SaveLogDocent", RPCMode.Others,_logLine); 
		}
		GUILayout.EndArea();
		GUILayout.EndArea();
		
    }
}

public function SetCurrentInfo(_value: int) {
   _currentInfo = _value-1;	
   _showInfo = true;
   _logLine = "TestB|Show Info|"+GetComponent(NetworkConnectionIT).playerName()+"|"+GetComponent(NetworkConnectionIT).currentYear()+
              "|"+_value+"|"+DateTime.Now;
   networkView.RPC ("SaveLogDocent", RPCMode.Others,_logLine);                         
}

public function ShowInfo(_value: boolean){
   _showInfo = _value;
}
