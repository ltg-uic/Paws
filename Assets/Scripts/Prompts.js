var _prompts : Texture2D[];
var url = "http://paws.evl.uic.edu/PawsImages/";
var promptCurrentList: int[] = [0,3,2,5,1];
			
function Start () {
	_prompts = new Texture2D[6];
	var www : WWW;
	for (var i:int = 0; i < 6;i++)
	{ 
		_prompts[i] = new Texture2D(80,60);
	    www = new WWW (url+(i+1).ToString()+".png");
	    Debug.Log(url+(i+1).ToString()+".png");
		yield www;
		_prompts[i] = www.texture;
	}
}
