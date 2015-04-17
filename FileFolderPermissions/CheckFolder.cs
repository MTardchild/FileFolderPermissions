using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FileFolderPermissions
{
    class CheckFolder
    {
        private string Path;
        private string LogPath;

        public CheckFolder(string path, string logPath) {
            this.Path = path;
            this.LogPath = logPath;
        }

        public bool fileInfo(string path) 
        {
            string[] Files = null;
            FileInfo fi = new FileInfo(Path);
            try
            {
                Files = Directory.GetFiles(@path, "*");
            } 
            catch (IOException) 
            {
                return false;
            }

            foreach (string file in Files)
            {
                writeToLog(file + "<br />", false);
                FileSecurity fs = fi.GetAccessControl(AccessControlSections.Access);

                foreach (FileSystemAccessRule fsar in fs.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)))
                {
                    string userName = fsar.IdentityReference.Value;
                    string userRights = fsar.FileSystemRights.ToString();
                    string userAccessType = fsar.AccessControlType.ToString();
                    string ruleSource = fsar.IsInherited ? "Inherited" : "Explicit";
                    string rulePropagation = fsar.PropagationFlags.ToString();
                    string ruleInheritance = fsar.InheritanceFlags.ToString();

                    writeToLog(userName, userRights, userAccessType, ruleSource, rulePropagation, ruleInheritance);
                }
            }

            return true;
        }

        public bool directoryInfo()
        {
            DirectoryInfo di = new DirectoryInfo(Path);
            writeToLog(Path + "<br />", true);
            fileInfo(Path);
            foreach (DirectoryInfo dir in di.GetDirectories("*", SearchOption.AllDirectories))
            {
                if (!hasAccess(dir.FullName))
                {
                    writeToLog("FATAL: Unable to access directory.", true);
                    return false;
                }
                writeToLog(dir.FullName + "<br />", true);
                DirectorySecurity ds = dir.GetAccessControl(AccessControlSections.Access);
                foreach (FileSystemAccessRule fsar in ds.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)))
                {
                    string userName = fsar.IdentityReference.Value;
                    string userRights = fsar.FileSystemRights.ToString();
                    string userAccessType = fsar.AccessControlType.ToString();
                    string ruleSource = fsar.IsInherited ? "Inherited" : "Explicit";
                    string rulePropagation = fsar.PropagationFlags.ToString();
                    string ruleInheritance = fsar.InheritanceFlags.ToString();

                    writeToLog(userName, userRights, userAccessType, ruleSource, rulePropagation, ruleInheritance);
                }
                fileInfo(dir.FullName);
            }
            return true;
        }

        private bool hasAccess(string path)
        {
            bool readAllow = false;
            bool readDeny = false;
            DirectorySecurity accessControlList = null;
            try
            {
                accessControlList = Directory.GetAccessControl(path);
            }
            catch (UnauthorizedAccessException ex)
            {
                writeToLog(ex.ToString(), false);
                return false;
            }
            var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));

            if (accessControlList == null) return false;
            if (accessRules == null) return false;

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Read & rule.FileSystemRights) != FileSystemRights.Read) continue;

                if (rule.AccessControlType == AccessControlType.Allow)
                    readAllow = true;
                else if (rule.AccessControlType == AccessControlType.Deny)
                    readDeny = true;
            }

            return readAllow && !readDeny;
        }

        private void writeToLog(string path, bool isDirectory)
        {
            if (File.Exists(LogPath + "\\log.html"))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@LogPath + "\\log.html", true))
                {
                    if (isDirectory)
                    {
                        file.WriteLine("<br/><b>" + path + "</b>");
                    }
                    else
                    {
                        file.WriteLine("<br/>" + path);
                    }
                    
                }
            }
            else
            {
                if (isDirectory)
                {
                    System.IO.File.WriteAllText(@LogPath + "\\log.html", ("<br/><b>" + path + "</b>"));
                }
                else
                {
                    System.IO.File.WriteAllText(@LogPath + "\\log.html", ("<br/>" + path));
                }
            }
        }

        private void writeToLog(string userName, string userRights, string userAccessType, string ruleSource, string rulePropagation, string ruleInheritance)
        {
            if (File.Exists(LogPath + "\\log.html"))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@LogPath + "\\log.html", true))
                {
                    if (userName.Contains("NTDOMAIN"))
                    {
                        file.WriteLine(userName + " : " + userAccessType + " : " + userRights + " : " + ruleSource + " : " + rulePropagation + " : " + ruleInheritance + "<br/>");
                    }
                }
            }
            else
            {
                if (userName.Contains("NTDOMAIN"))
                {
                    System.IO.File.WriteAllText(@LogPath + "\\log.html", (userName + " : " + userAccessType + " : " + userRights 
                                                + " : " + ruleSource + " : " + rulePropagation + " : " + ruleInheritance + "<br/>"));
                }
            }
        }
    }
}
