#pragma strict
 
var _originalPosition: Vector3 = new Vector3(904,15,620);

function  LoadYear(_year :String){

	 var polarBear:GameObject = GameObject.Find("PolarBear(Clone)");
     var water:GameObject = GameObject.FindGameObjectWithTag("Water");
     var seal:GameObject = GameObject.FindGameObjectWithTag("SealModel");
     var shoreLine:GameObject = GameObject.FindGameObjectWithTag("ShoreLine");
	 print("Loading "+_year);
	 if (_year == "1975"){
	     print("Loading "+_year);
	     polarBear.transform.position = _originalPosition;
	     water.transform.position = new Vector3(842,10.25,785);
	     seal.transform.position = new Vector3(903.1138,10.91528,842.7151);
	     shoreLine.transform.position = new Vector3(906.05,10.3,765.21);
	 }
	 else if (_year == "2010"){
	  print("Loading "+_year);
	     polarBear.transform.position = Vector3(_originalPosition.x+2000,_originalPosition.y,_originalPosition.z);
         water.transform.position = new Vector3(2842,10.25,785);
         seal.transform.position = new Vector3(2903,11.1774,843);
         shoreLine.transform.position = new Vector3(2719.7442,9.1,393.95);
	  }  
	 else if(_year == "2045"){
	  print("Loading "+_year);
	    polarBear.transform.position = Vector3(_originalPosition.x+4000,_originalPosition.y,_originalPosition.z);
	     water.transform.position = new Vector3(4842,10.25,785);
	     seal.transform.position = new Vector3(4903,10.86695,841);
	     shoreLine.transform.position = new Vector3(4908,10.3,666.923);
	 }	
     
}