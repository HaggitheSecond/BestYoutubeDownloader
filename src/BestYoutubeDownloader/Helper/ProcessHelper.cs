using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestYoutubeDownloader.Helper;
public static class ProcessHelper
{
    public static void OpenDirectoryOrFile(string? directory)
    {
        if(string.IsNullOrEmpty(directory) || (Directory.Exists(directory) is false && File.Exists(directory) is false))
            return;

        Start(directory);
    }

    public static void Start(string path)
    {
        var process = new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        };
        Process.Start(process);
    }
}
