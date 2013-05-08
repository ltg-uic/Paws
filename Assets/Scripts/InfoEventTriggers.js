
private var _currentYear: int;
private var _trigger:int;
private var _prevTrigger:int = 0;
private var _eventId: int;

public function SetCurrentYear(_value:int){
	_currentYear = _value;
	_prevTrigger = 0;
}

public function SetTrigger(_value:int){ // receive bear's z position
	_trigger = _value; 
    _eventId = 0;
	if (_currentYear == 1975){
	Debug.Log(_currentYear);
			if (_trigger >= 619 && _trigger <= 621)
			{
				_trigger = 620;
				_eventId = 1;
			}
			else if (_trigger >= 629 && _trigger <= 631)
			{
				_trigger = 630;
				_eventId = 4;
			}
			else if (_trigger >= 656 && _trigger <= 658)
			{
				_trigger = 657;
				_eventId = 3;
			}
			else if (_trigger >= 704 && _trigger <= 706)
			{
				_trigger = 705;
				_eventId = 18;
			}
			else if (_trigger >= 739 && _trigger <= 741)
			{
				_trigger = 740;
				_eventId = 14;
			}
			else if (_trigger >= 780 && _trigger <= 782)
			{
				_trigger = 781;
				_eventId = 13;
			}
	}
	else if (_currentYear == 2010){
			if (_trigger >= 619 && _trigger <= 621)
			{
				_trigger = 620;
				_eventId = 5;
			}
			else if (_trigger >= 637 && _trigger <= 639)
			{
				_trigger = 638;
				_eventId = 2;
			}
			else if (_trigger >= 679 && _trigger <= 681)
			{
				_trigger = 680;
				_eventId = 7;
			}
			else if (_trigger >= 723 && _trigger <= 725)
			{
				_trigger = 724;
				_eventId = 8;
			}
			else if (_trigger >= 759 && _trigger <= 761)
			{
				_trigger = 760;
				_eventId = 6;
			}
			else if (_trigger >= 815 && _trigger <= 817)
			{
				_trigger = 816;
				_eventId = 15;
			}
	}
	else if (_currentYear == 2045){
			if (_trigger >= 619 && _trigger <= 621)
			{
				_trigger = 620;
				_eventId = 9;
			}
			else if (_trigger >= 632 && _trigger <= 634)
			{
				_trigger = 633;
				_eventId = 11;
			}
			else if (_trigger >= 681 && _trigger <= 683)
			{
				_trigger = 682;
				_eventId = 12;
			}
			else if (_trigger >= 712 && _trigger <= 714)
			{
				_trigger = 713;
				_eventId = 10;
			}
			else if (_trigger >= 743 && _trigger <= 745)
			{
				_trigger = 744;
				_eventId = 17;
			}
			else if (_trigger >= 776 && _trigger <= 778)
			{
				_trigger = 777;
				_eventId = 16;
			}
	}
    Debug.Log("Checking trigger " + _eventId + " " + _prevTrigger + " " +_trigger);
	if (_trigger != _prevTrigger && _eventId > 0){
	    Debug.Log("Send trigger " + _eventId);
		_prevTrigger = _trigger;	   
		networkView.RPC ("SendEventId", RPCMode.Server, _eventId);
	}
}
	