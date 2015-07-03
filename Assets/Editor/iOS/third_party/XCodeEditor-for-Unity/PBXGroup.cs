namespace UnityEditor.XCodeEditor
{
	public class PBXGroup : PBXObject
	{
		protected const string NAME_KEY = "name";
		protected const string CHILDREN_KEY = "children";
		protected const string PATH_KEY = "path";
		protected const string SOURCETREE_KEY = "sourceTree";
		
		#region Constructor
		
		public PBXGroup( string name, string path = null, string tree = "SOURCE_ROOT" )
		{	
			Add( NAME_KEY, name );
			Add( CHILDREN_KEY, new PBXList() );
			
			if( path != null ) {
				Add( PATH_KEY, path );
				Add( SOURCETREE_KEY, tree );
			}
			else {
				Add( SOURCETREE_KEY, "\"<group>\"" );
			}

			internalNewlines = true;
		}
		
		public PBXGroup( string guid, PBXDictionary dictionary ) : base( guid, dictionary )
		{
			internalNewlines = true;
		}
		
		#endregion
		#region Properties
		
		public string name {
			get {
				if( !ContainsKey( NAME_KEY ) ) {
					return null;
				}
				return (string)_data[NAME_KEY];
			}
		}
		
		public PBXList children {
			get {
				if( !ContainsKey( CHILDREN_KEY ) ) {
					Add( CHILDREN_KEY, new PBXList() );
				}
				return (PBXList)_data[CHILDREN_KEY];
			}
		}
		
		public string path {
			get {
				if( !ContainsKey( PATH_KEY ) ) {
					return null;
				}
				return (string)_data[PATH_KEY];
			}
		}
		
		public string sourceTree {
			get {
				return (string)_data[SOURCETREE_KEY];
			}
		}
		
		#endregion
		
		
		public string AddChild( PBXObject child )
		{
			if( child is PBXFileReference || child is PBXGroup ) {
				children.Add( child.guid );
				return child.guid;
			}
				
			return null;
		}
		
		public void RemoveChild( string id )
		{
			if( !IsGuid( id ) )
				return;
			
			children.Remove( id );
		}
		
		public bool HasChild( string id )
		{
			if( !ContainsKey( CHILDREN_KEY ) ) {
				Add( CHILDREN_KEY, new PBXList() );
				return false;
			}
			
			if( !IsGuid( id ) )
				return false;
			
			return ((PBXList)_data[ CHILDREN_KEY ]).Contains( id );
		}
		
		public string GetName()
		{
			return (string)_data[ NAME_KEY ];
		}
		
	}
}
