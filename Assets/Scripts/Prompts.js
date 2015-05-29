var prompts : Texture2D[];
var url = "http://paws.local/PawsImages/";
var promptCurrentList: int[] = [0,1,2,3,4];
private var pos:int = 0;
private var startDone : boolean = false;
private var countError:int = 0;
var countImages = 0;

public function LoadImages(){
    var www : WWW = null;
    prompts = new Texture2D[countImages];
	for (var i:int = 0; i < countImages;i++)
	{ 
	    var image:Texture2D =  new Texture2D(128,128);
	    www = new WWW (url+(i+1).ToString()+".jpg");
		yield www;
		if (!String.IsNullOrEmpty(www.error)){
	    	Debug.Log(www.error);
	    	countError ++;
	    }
	    else
	    { 
	       image = www.texture;
	       www.Dispose();
	       prompts[i] = image;
	    }
	//	Debug.Log(url+(i+1).ToString()+".jpg");
	
	}
	if ( countImages > 0 ){
		GetComponent(NetworkConnectionIT).PromptReady = true;
	}
	
}

public function GetPrompts(_direction:int){

	for (var i:int = 0; i < 5;i++){
	
		if (_direction == 0){
			promptCurrentList[i] = Mathf.Floor(Random.Range(0, 20));
		}
		else if (_direction == -1 && promptCurrentList[i]-5 >= 0)
		{
			promptCurrentList[i] = promptCurrentList[i]-5;
		}
		else if (_direction == 1 && promptCurrentList[i]+5 <= (countImages - 1))
		{
			promptCurrentList[i] = promptCurrentList[i]+5;
		}
	}

}

public function ResetPrompts(){
	promptCurrentList = [0,1,2,3,4];
}
