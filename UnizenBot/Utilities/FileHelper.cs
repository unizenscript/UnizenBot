using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnizenBot.Utilities
{
    /// <summary>
    /// General helper methods for files.
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// Normalizes file attributes for all items in a directory.
        /// </summary>
        /// <param name="directoryPath">The directory.</param>
        public static void NormalizeAttributes(string directoryPath)
        {
            string[] filePaths = Directory.GetFiles(directoryPath);
            string[] subdirectoryPaths = Directory.GetDirectories(directoryPath);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
            }
            foreach (string subdirectoryPath in subdirectoryPaths)
            {
                NormalizeAttributes(subdirectoryPath);
            }
            File.SetAttributes(directoryPath, FileAttributes.Normal);
        }
    }
}
