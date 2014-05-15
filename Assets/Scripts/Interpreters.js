
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

public function SetInterpreters(){
	interpreterNames.Clear();
    if (interpreters.Length > 0){
	    var interpreterEntries = interpreters.Split('|'[0]);
	    interpreterNames.Add("...");
	    interpreterIDs.Add(-1);
		for (var entry in interpreterEntries)
		{
		   if (entry.Length > 1){
		   // Debug.Log("Interpreters::Received data:"+entry);
		       var interpreterData : String[]= entry.Split(':'[0]);
		       if (interpreterData.Length > 0){		       
			      // Debug.Log("Interpreters::Received data:"+interpreterData[ 0 ]+"  "+interpreterData[ 1 ]);
				   interpreterID = interpreterData[0];
				   interpreterName = interpreterData[1] ;
				   interpreterNames.Add(interpreterName);
				   interpreterIDs.Add(interpreterID);
			   }
		   }
		}
	}
}