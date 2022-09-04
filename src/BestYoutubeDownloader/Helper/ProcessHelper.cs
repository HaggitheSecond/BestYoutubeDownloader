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
    public static void OpenDirectory(string? directory)
    {
        if(string.IsNullOrEmpty(directory) || Directory.Exists(directory) is false)
            return;

        var process = new ProcessStartInfo
        {
            FileName = directory,
            UseShellExecute = true
        };
        Process.Start(process);
    }
}
