using System.Collections;

namespace UnityEditor.XCodeEditor
{
	public class PBXList : ArrayList
	{
		public bool internalNewlines;
		public PBXList()
		{
			internalNewlines=true;
		}
		
		public PBXList( object firstValue )
		{
			Add( firstValue );
			internalNewlines=true;
		}
	}
	
}
