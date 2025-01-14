﻿using NAppUpdateR.Common;

namespace NAppUpdateR.Sources
{
    public interface IUpdateSource
    {
        string GetUpdatesFeed(); // TODO: return a the feed as a stream
        bool GetData(string filePath, string basePath, Action<UpdateProgressInfo> onProgress, ref string tempLocation);
    }
}
