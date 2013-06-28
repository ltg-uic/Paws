import System.IO;
import System;

private var applicationPath:String = "";
// This is for the Lab Study
function Start (){

	applicationPath = Application.dataPath;
    if (Application.platform == RuntimePlatform.OSXPlayer) {
        applicationPath += "/../../";
    }
    else if (Application.platform == RuntimePlatform.WindowsPlayer) {
        applicationPath += "/../";
    }		
		
}

public function AppendDataToLog(_value: String){
  Debug.Log("Logging...");

  var sw =  File.AppendText(applicationPath+"/StudyScene_Log.txt");
  
  var logline = DateTime.Now+_value;
  sw.WriteLine(logline);
  sw.Close(); 
      
}
