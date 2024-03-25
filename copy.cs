using System.IO;

public static void CopyDirectory(string sourceDirName, string destDirName)
{
    DirectoryInfo dir = new DirectoryInfo(sourceDirName);

    if (!dir.Exists)
    {
        throw new DirectoryNotFoundException(
            "Source directory does not exist or could not be found: "
            + sourceDirName);
    }

    DirectoryInfo[] dirs = dir.GetDirectories();
    
    // If the destination directory doesn't exist, create it. 
    Directory.CreateDirectory(destDirName);        

    // Get the files in the directory and copy them to the new location.
    FileInfo[] files = dir.GetFiles();
    foreach (FileInfo file in files)
    {
        if (file.Extension != ".pdb" && file.Extension != ".xml")
        {
            string tempPath = Path.Combine(destDirName, file.Name);
            file.CopyTo(tempPath, false);
        }
    }

    // If copying subdirectories, copy them and their contents to new location.
    foreach (DirectoryInfo subdir in dirs)
    {
        if (subdir.Name == "wwwroot" || subdir.Name == "runtimes")
        {
            string tempPath = Path.Combine(destDirName, subdir.Name);
            CopyDirectory(subdir.FullName, tempPath);
        }
    }
}