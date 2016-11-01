using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace MyExplorer
{
    public abstract class FileSystemItem
    {
        private string m_strName;
        protected Image m_imgImage;
        protected string m_strFullPath;
        private IFileSystemViewer m_ifsvContainer;

        public string Name
        {
            get { return this.m_strName; }
            set { this.m_strName = value; }
        }

        public string FullPath
        {
            get { return this.m_strFullPath; }
        }

        public Image Icon
        {
            get { return this.m_imgImage; }
        }

        public IFileSystemViewer ContainingViewer
        {
            get { return this.m_ifsvContainer; }
            set { this.m_ifsvContainer = value; }
        }

        public abstract void OnAction();

        public abstract void OnExamine();

        // TODO: implement
        public static List<FileSystemItem> GetItems(string strPath)
        {
            if (strPath == "...")
            {
                List<FileSystemItem> lstItems = new List<FileSystemItem>();
                foreach (DriveInfo inf in DriveInfo.GetDrives())
                {
                    lstItems.Add(new FileSystemDrive(inf));
                }
                return (lstItems);
            }
            else
            {
                return null;
            }
        }
    }
}
