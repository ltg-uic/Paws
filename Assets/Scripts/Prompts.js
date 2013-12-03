var _prompts : GUITexture[];
var url = "http://paws.evl.uic.edu/PawsImages/clock_3min.png";
			
function Start () {
	_prompts = new GUITexture[5];
	var www : WWW = new WWW (url);
	yield www;
	
	_prompt = new GameObject("Prompt");
    _prompt.transform.position = Vector3(0.2,0.2,1);
    _prompt.AddComponent('GUITexture');  
    _prompt.transform.localScale = Vector3(0.1f,0.1f,0.1f);
    _prompt.guiTexture.texture = www.texture;
	_prompt.guiTexture.enabled= false;
	
	_prompt = new GameObject("Prompt");
    _prompt.transform.position = Vector3(0.3,0.2,1);
    _prompt.AddComponent('GUITexture');  
    _prompt.transform.localScale = Vector3(0.1f,0.1f,0.1f);
    _prompt.guiTexture.texture = www.texture;
	_prompt.guiTexture.enabled= false;
	
}

function Update () {
	 

}

function ShowPrompts(_value: boolean){

    for(var _prompt : GameObject in GameObject.FindObjectsOfType(GameObject))
	{
	    if(_prompt.name == "Prompt")
	    {
	        _prompt.guiTexture.enabled = _value;  
	    }
	    
	}
			
}
