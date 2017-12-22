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
        files = new string[fi.Length];

        // Set names of files
        int i = 0;
        foreach(FileInfo f in fi)        
            files[i++] = f.Name;   
    }
}
