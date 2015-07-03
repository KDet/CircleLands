using System.Collections.Generic;
using System.IO;

namespace UnityEngine.Advertisements.XCodeEditor
{
	public class PBXFileReference : PBXObject
	{
		protected const string PATH_KEY = "path";
		protected const string NAME_KEY = "name";
		protected const string SOURCETREE_KEY = "sourceTree";
		protected const string EXPLICIT_FILE_TYPE_KEY = "explicitFileType";
		protected const string LASTKNOWN_FILE_TYPE_KEY = "lastKnownFileType";
		protected const string ENCODING_KEY = "fileEncoding";
		public string buildPhase;
		public readonly Dictionary<TreeEnum, string> trees = new Dictionary<TreeEnum, string> {
			{ TreeEnum.ABSOLUTE, "\"<absolute>\"" },
			{ TreeEnum.GROUP, "\"<group>\"" },
			{ TreeEnum.BUILT_PRODUCTS_DIR, "BUILT_PRODUCTS_DIR" },
			{ TreeEnum.DEVELOPER_DIR, "DEVELOPER_DIR" },
			{ TreeEnum.SDKROOT, "SDKROOT" },
			{ TreeEnum.SOURCE_ROOT, "SOURCE_ROOT" }
		};
		public static readonly Dictionary<string, string> typeNames = new Dictionary<string, string> {
			{ ".a", "archive.ar" },
			{ ".app", "wrapper.application" },
			{ ".s", "sourcecode.asm" },
			{ ".c", "sourcecode.c.c" },
			{ ".cpp", "sourcecode.cpp.cpp" },
			{ ".cs", "sourcecode.cpp.cpp" },
			{ ".framework", "wrapper.framework" },
			{ ".h", "sourcecode.c.h" },
			{ ".icns", "image.icns" },
			{ ".m", "sourcecode.c.objc" },
			{ ".mm", "sourcecode.cpp.objcpp" },
			{ ".nib", "wrapper.nib" },
			{ ".plist", "text.plist.xml" },
			{ ".png", "image.png" },
			{ ".rtf", "text.rtf" },
			{ ".tiff", "image.tiff" },
			{ ".txt", "text" },
			{ ".xcodeproj", "wrapper.pb-project" },
			{ ".xib", "file.xib" },
			{ ".strings", "text.plist.strings" },
			{ ".bundle", "wrapper.plug-in" },
			{ ".dylib", "compiled.mach-o.dylib" }
		 };
		public static readonly Dictionary<string, string> typePhases = new Dictionary<string, string> {
			{ ".a", "PBXFrameworksBuildPhase" },
			{ ".app", null },
			{ ".s", "PBXSourcesBuildPhase" },
			{ ".c", "PBXSourcesBuildPhase" },
			{ ".cpp", "PBXSourcesBuildPhase" },
			{ ".cs", null },
			{ ".framework", "PBXFrameworksBuildPhase" },
			{ ".h", null },
			{ ".icns", "PBXResourcesBuildPhase" },
			{ ".m", "PBXSourcesBuildPhase" },
			{ ".mm", "PBXSourcesBuildPhase" },
			{ ".nib", "PBXResourcesBuildPhase" },
			{ ".plist", "PBXResourcesBuildPhase" },
			{ ".png", "PBXResourcesBuildPhase" },
			{ ".rtf", "PBXResourcesBuildPhase" },
			{ ".tiff", "PBXResourcesBuildPhase" },
			{ ".txt", "PBXResourcesBuildPhase" },
			{ ".xcodeproj", null },
			{ ".xib", "PBXResourcesBuildPhase" },
			{ ".strings", "PBXResourcesBuildPhase" },
			{ ".bundle", "PBXResourcesBuildPhase" },
			{ ".dylib", "PBXFrameworksBuildPhase" }
		};

		public PBXFileReference(string guid, PBXDictionary dictionary) : base( guid, dictionary )
		{

		}

		public PBXFileReference(string filePath, TreeEnum tree = TreeEnum.SOURCE_ROOT)
		{
			string temp = "\"" + filePath + "\"";
			Add(PATH_KEY, temp);
			Add(NAME_KEY, Path.GetFileName(filePath));
			Add(SOURCETREE_KEY, Path.IsPathRooted(filePath) ? trees[TreeEnum.ABSOLUTE] : trees[tree]);
			GuessFileType();
		}

		public string name {
			get {
				if(!ContainsKey(NAME_KEY)) {
					return null;
				}
				return (string)_data[NAME_KEY];
			}
		}

		private void GuessFileType()
		{
			Remove(EXPLICIT_FILE_TYPE_KEY);
			Remove(LASTKNOWN_FILE_TYPE_KEY);
			string extension = Path.GetExtension((string)_data[NAME_KEY]);
			if(!typeNames.ContainsKey(extension)) {
				Debug.LogWarning("Unknown file extension: " + extension + "\nPlease add extension and Xcode type to PBXFileReference.types");
				return;
			}

			Add(LASTKNOWN_FILE_TYPE_KEY, typeNames[extension]);
			buildPhase = typePhases[extension];
		}

		private void SetFileType(string fileType)
		{
			Remove(EXPLICIT_FILE_TYPE_KEY);
			Remove(LASTKNOWN_FILE_TYPE_KEY);

			Add(EXPLICIT_FILE_TYPE_KEY, fileType);
		}

	}

	public enum TreeEnum
	{
		ABSOLUTE,
		GROUP,
		BUILT_PRODUCTS_DIR,
		DEVELOPER_DIR,
		SDKROOT,
		SOURCE_ROOT
	}
}
