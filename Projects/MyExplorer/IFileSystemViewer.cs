using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyExplorer
{
    public interface IFileSystemViewer
    {
        void AddFSItem(FileSystemItem fsiItem);
        void ShowChildren(List<FileSystemItem> lstChildren, FileSystemItem fsFather);
        string CurrentPath { get; set; }
    }
}
