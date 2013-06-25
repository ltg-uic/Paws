
var interpreterName : String = "";
var interpreterID : String = "";
var interpreters: String;
var mySkin: GUISkin;
var selGridInt : int = 0;
var interpreterNames = new ArrayList();
var interpreterIDs = new ArrayList();

function ResetInterpreter(){
	interpreterName = interpreterNames[0];
}

function Start () {
    getInterpreters();
}


function getInterpreters(){
				
    interpreters = GetComponent(DatabaseConnection).GetInterpreters();
    var interpreterEntries = interpreters.Split('|'[0]);
    interpreterNames.Add("...");
    interpreterIDs.Add(-1);
	for (var entry in interpreterEntries)
	{
	   var interpreterData : String[]= entry.Split(':'[0]);
	   interpreterID = interpreterData[ 0 ];
	   interpreterName = interpreterData[ 1 ] ;
	   interpreterNames.Add(interpreterName);
	   interpreterIDs.Add(interpreterID);
	}
}

function updateInterpreter(_pos:int){
	GetComponent(DatabaseConnection).UpdateInterpreter(interpreterIDs[_pos].ToString());
}