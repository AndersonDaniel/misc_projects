using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace MyExplorer
{
    class FileSystemDrive : FileSystemItem
    {
        // TODO: implement
        public override void OnAction() 
        {
            this.ContainingViewer.CurrentPath = this.Name;
        }
        public override void OnExamine() { }

        public FileSystemDrive(DriveInfo diDrive)
        {
            this.Name = diDrive.Name.Substring(0, 2);
            this.m_strFullPath = this.Name + "\\";
            this.m_imgImage = (Bitmap)MyExplorer.Properties.Resources.Drive.Clone();
        }
    }
}
