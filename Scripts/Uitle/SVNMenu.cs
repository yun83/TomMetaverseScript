using UnityEngine;
using System.IO;
using System.Collections;
using UnityEditor;

public class SVNMenu 
{
#if UNITY_EDITOR
	[MenuItem("adtnMenu/SVN/Update Assets", false, 1)]

	static void svnUpdateAssets ()
	{
		tortoiseProc("update", "Assets");
	}

	[MenuItem("adtnMenu/SVN/Commit Assets", false, 2)]

	static void svnCommitAssets ()
	{
		tortoiseProc("commit", "Assets");
	}
	
	[MenuItem("adtnMenu/SVN/Revert Assets", false, 3)]

	static void svnRevertAssets ()
	{
		tortoiseProc("revert", "Assets");
	}

	static void tortoiseProc( string command, string file )
	{
		string path = Path.Combine(getProjectPath(), file );
		string args = string.Format ("/command:{0} /path:\"{1}\"", command, path);

		System.Diagnostics.Process.Start ("TortoiseProc", args);
	}

	static string getProjectPath ()
	{
		string path = Path.GetFullPath(".");
		return path;
	}


	[MenuItem("adtnMenu/Save/Delete All Save File", false, 11)]
	static void RemoveSaveFile()
    {
		PlayerPrefs.DeleteAll();
	}
	[MenuItem("adtnMenu/Save/Delete Event Day File", false, 12)]
	static void RemoveDayFile()
	{
		PlayerPrefs.DeleteKey("DayEvent");
	}
#endif
}