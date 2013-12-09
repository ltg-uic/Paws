var prompts : Texture2D[];
var url = "http://paws.evl.uic.edu/PawsImages/";
var promptCurrentList: int[] = [0,1,2,3,4];
			
function Start () {
	prompts = new Texture2D[20];
	var www : WWW;
	for (var i:int = 0; i < 20;i++)
	{ 
		prompts[i] = new Texture2D(80,60);
	    www = new WWW (url+(i+1).ToString()+".jpg");
		yield www;
		prompts[i] = www.texture;
		Debug.Log(url+(i+1).ToString()+".jpg");
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
	}

}

public function ResetPrompts(){
	promptCurrentList = [0,1,2,3,4];
}
