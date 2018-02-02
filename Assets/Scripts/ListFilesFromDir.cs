using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ListFilesFromDir {

    // List of files
    public string[] files = null;

    /**
     * @brief List each files of directory
     * 
     * @param[in]   dir     the directory
     */ 
    public ListFilesFromDir(string dir)
    {
        // Get all file from directory
        DirectoryInfo info = new DirectoryInfo(dir);
        FileInfo []fi = info.GetFiles();

        // Initialize array of string
        List<string> files_list = new List<string>();

        // Set names of files
        foreach (FileInfo f in fi)
        {
            if(f.Extension != ".meta")
                files_list.Add(f.Name);
        }

        files = new string[files_list.Count];
        files = files_list.ToArray();
    }
}
