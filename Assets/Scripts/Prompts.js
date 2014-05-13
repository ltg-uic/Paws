var prompts : Texture2D[];
var url = "http://paws.local/PawsImages/";
var promptCurrentList: int[] = [0,1,2,3,4];
private var remainingImages =  new Array();
private var pos:int = 0;
private var startDone : boolean = false;
private var countError:int = 0;
function Start () {
	prompts = new Texture2D[20];
	var www : WWW;

	for (var i:int = 0; i < 20;i++)
	{ 
		prompts[i] = new Texture2D(100,75);
	    www = new WWW (url+(i+1).ToString()+".jpg");
		yield www;
		if (!String.IsNullOrEmpty(www.error)){
	    	Debug.Log(www.error);
	    	countError ++;
	    	remainingImages.Add(i);
	    }
		prompts[i] = www.texture;
		Debug.Log(url+(i+1).ToString()+".jpg");
	}
	if (countError == 0){
		GetComponent(NetworkConnectionServer).PromptReady = true;
	}
	startDone = true;
}

public function LoadImages(){
    var www : WWW;
	if (remainingImages.length > 0 && pos < remainingImages.length){
			prompts[remainingImages[pos]] = new Texture2D(100,75);
			var name: int = remainingImages[pos];
		    www = new WWW (url+(name+1).ToString()+".jpg");
			yield www;
			if (String.IsNullOrEmpty(www.error)){
		      	remainingImages.RemoveAt(pos);
		    }
			prompts[remainingImages[pos]] = www.texture;
			Debug.Log(url+(name+1).ToString()+".jpg");
			pos++;
	 }
	 else if (pos == 0 && countError>0){
		 GetComponent(NetworkConnectionServer).PromptReady = true;
	 }
	 else{	
	  	 pos = 0;	
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
		else if (_direction == 1 && promptCurrentList[i]+5 <= 19)
		{
			promptCurrentList[i] = promptCurrentList[i]+5;
		}
		LoadImages();
	}

}

public function ResetPrompts(){
	promptCurrentList = [0,1,2,3,4];
}
