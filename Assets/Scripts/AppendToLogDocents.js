import System.IO;
import System;

private var applicationPath:String = "";

function Start (){

	applicationPath = Application.dataPath;
    if (Application.platform == RuntimePlatform.OSXPlayer) {
        applicationPath += "/../../";
    }
    else if (Application.platform == RuntimePlatform.WindowsPlayer) {
        applicationPath += "/../";
    }		
		
}

public function AppendDataToLog(_logLine:String){
   Debug.Log("AppendToLogDocents::Save data in file docents");
  var sw =  File.AppendText(applicationPath+"/A_Mile_In_My_Paws_Log_Docents.txt");
  sw.WriteLine(_logLine);
  sw.Close(); 
}
