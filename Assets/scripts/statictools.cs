using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class statictools
{
	
	static public bool has_tag(string targetTag, List<string> tags) {
		foreach(string tag in tags) { 
			if( targetTag == tag ) return true;
		}
		return false;
	}
	
}
