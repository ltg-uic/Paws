using UnityEngine;
using System.Collections;

public class Interpreters : MonoBehaviour {
	
string interpreterName= "";
string interpreterID = "";
string interpreters;

private Rect textrect = new Rect (100, 150, 500, 500);
 
void OnGUI(){
    GUI.TextArea( textrect, interpreterName );
}

void Start () {
    getInterpreters();
}

void Update () {

}

void getInterpreters(){
/*	Debug.Log("Paso");		
		
        interpreters = GetComponent<DatabaseConnection>().GetInterpreters();
        string[] interpreterEntries = interpreters.Split('|');
		foreach (string entry in interpreterEntries)
		{
		   string[] interpreterData = entry.Split(':');
		   interpreterID = interpreterData[ 0 ];
		   interpreterName = interpreterData[ 1 ] ;
		}
    Debug.Log("Paso2");
		 */
}
}