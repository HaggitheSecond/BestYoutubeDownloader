﻿namespace BestYoutubeDownloader.Common
{
    public class Mp3MetaData
    {
        public string? Artist { get; set; }
        public string? Title { get; set; }
        public bool NeedCheck { get; set; }
        public string? CheckReason { get; set; }
        public string? SourceUrl { get; set; }
    }
}