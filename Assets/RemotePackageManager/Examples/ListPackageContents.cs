using UnityEngine;
using System.Collections;

public class ListPackageContents : MonoBehaviour
{
	[PackageSelector]
	public string package;
	
	private void Start()
	{
		// Load package "MySpehere" (note the hierarchy) and then execute callback for all assets.
		RemotePackageManager.Load( package ).GetAll<Object>( objs => {			
			// For each asset in package...
			foreach( Object o in objs )
			{
				// Prints its information:
				Debug.Log( "Found asset: '" + o.name + "' of type: " + o.GetType().Name );
			}			
		});
    }
}
