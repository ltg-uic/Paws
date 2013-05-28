#pragma strict

var interpreterName : String = "";
var interpreterID : String = "";
var interpreters: String;
var mySkin: GUISkin;
var selGridInt : int = 0;
var interpreterNames = new ArrayList();
var cnt: int = 0;
var showInterpreterList = false;

function ResetInterpreter(){
	interpreterName = "...";
}
function OnGUI(){
    GUI.skin = mySkin; //Please Update in the skin in the inspertor box
    GUI.Label( new Rect (100, 150, 200, 200), "Interpreter: "+interpreterName ); //Please Update coordinates
    if (GUI.Button(Rect(300,150,40,30),">")){
    	showInterpreterList = !showInterpreterList;
    } 
    if (showInterpreterList){
	 	for (cnt = 0; cnt < interpreterNames.Count; cnt++){
	 	   if (GUI.Button(Rect(300,150+25*(cnt+1),50,25),"" + interpreterNames[cnt])){
	 	   		interpreterName = interpreterNames[cnt];
	 	   		showInterpreterList = !showInterpreterList;
	 	   		//Update database
	 	   } 
	    }
    }
}

function Start () {
    getInterpreters();
}

function Update () {

}

function getInterpreters(){
				
    interpreters = GetComponent(DatabaseConnection).GetInterpreters();
    var interpreterEntries = interpreters.Split('|'[0]);
    interpreterNames.Add("...");
	for (var entry in interpreterEntries)
	{
	   var interpreterData : String[]= entry.Split(':'[0]);
	   interpreterID = interpreterData[ 0 ];
	   interpreterName = interpreterData[ 1 ] ;
	   interpreterNames.Add(interpreterName);
	}
}
/*
function InstantiatePrimitive() {
    switch (index) {
        case 0:
            break;
        case 1:

            break;
        case 2:

            break;
        default:
            Debug.LogError("Unrecognized Option");
            break;
    }
}*/