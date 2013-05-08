function Start(){
   	var  _IceMaterial : Material = Resources.Load("Mat/Ice_02", typeof(Material));
	
	var CurrentIce: GameObject = GameObject.Find("/Ice/Shore_Ice");
	
	if (CurrentIce != null){
		var children : Renderer[] = CurrentIce.GetComponentsInChildren.<Renderer>();
		var childrenRenderers : Renderer[] = CurrentIce.GetComponentsInChildren.<Renderer>();
		var i = 1;
		for (var childRenderer: Renderer in childrenRenderers){
		   childRenderer.material = _IceMaterial;
		   i++;
		}
	}
}
