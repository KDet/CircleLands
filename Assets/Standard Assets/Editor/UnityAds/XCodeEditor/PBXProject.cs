namespace UnityEngine.Advertisements.XCodeEditor
{
	public class PBXProject : PBXObject
	{
		protected string MAINGROUP_KEY = "mainGroup";

		public PBXProject()
		{
		}

		public PBXProject(string guid, PBXDictionary dictionary) : base( guid, dictionary )
		{
		}

		public string mainGroupID {
			get {
				return (string)_data[MAINGROUP_KEY];
			}
		}
	}
}
