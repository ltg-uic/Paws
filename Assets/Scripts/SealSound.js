var savedTime : int = 0;

function Update () {
	var seconds : int = Time.time;
	var oddeven = (seconds % 6);
	
	if (oddeven)
	{
		Play(seconds);
	}
}

function Play(seconds)
{
	if (seconds!=savedTime)
	{
		audio.Play();
		savedTime = seconds;
	}
}