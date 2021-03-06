using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Local.JS.Extension.IndexedFile
{
    /// <summary>
    /// The Sacred Icon. Parasites, humans - no matter. The Icon must be found.
    /// </summary>
    [Serializable]
    public class Index
    {
        public static int MaxIndexices = int.MaxValue;
        internal static string BasePath;
        internal static string Installation00Path;
        internal static Installation00 TheArk;

        public string PseudoLocation;
        public string RealLocation;
        [JsonIgnore]
        public string ParentInstallation;
        [JsonIgnore]
        public FileInfo CoreFile
        {
            get
            {
                if (File.Exists(RealLocation))
                {
                    return new FileInfo(RealLocation);
                }
                else
                    return new FileInfo(Path.Combine(BasePath, ParentInstallation, RealLocation));
            }
        }
        /// <summary>
        /// Init IndexedFile with target manifest.
        /// </summary>
        /// <param name="Manifest"></param>
        public static void Init(string Manifest)
        {
            FileInfo ManifestFile = new FileInfo(Manifest);
            Installation00Path = ManifestFile.FullName;
            if (!ManifestFile.Directory.Exists) ManifestFile.Directory.Create();
            BasePath = ManifestFile.Directory.FullName;
            if (!ManifestFile.Exists)
            {
                TheArk = new Installation00();
                var content = JsonConvert.SerializeObject(TheArk);
                var file = Path.Combine(BasePath, Installation00Path);
                if (File.Exists(file)) File.Delete(file);
                File.WriteAllText(file, content);
            }
            else
                TheArk = JsonConvert.DeserializeObject<Installation00>(File.ReadAllText(ManifestFile.FullName));
            foreach (var item in TheArk.Installations)
            {
                FileInfo InstallationFile = new FileInfo(Path.Combine(BasePath, "InstallationIndeices", item));
                var installation = JsonConvert.DeserializeObject<Installation>(File.ReadAllText(InstallationFile.FullName));

                Installation00.PresentingInstallations.Add(installation);
            }
        }
        /// <summary>
        /// Validate the existence of local path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ValidateLocalPath(string path)
        {
            var RealFile = path;

            if (RealFile.StartsWith("~"))
            {
                RealFile.Substring(1);
                RealFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + RealFile;
            }
            return File.Exists(RealFile);
        }
        /// <summary>
        /// List all indices whose pseudo location is start with given path.
        /// </summary>
        /// <param name="PseudoLocation"></param>
        /// <returns></returns>
        public static List<Index> List(string PseudoLocation)
        {
            List<Index> indices = new List<Index>();
            PseudoLocation = UnifyPseudoLocation(PseudoLocation);
            PseudoLocation = DirecotrizeLocation(PseudoLocation);
            foreach (var item in Installation00.PresentingInstallations)
            {
                foreach (var _index in item.Indices)
                {
                    if (_index.PseudoLocation.ToUpper().StartsWith(PseudoLocation.ToUpper()))
                    {
                        _index.ParentInstallation = item.FolderID;
                        indices.Add(_index);
                    }
                }
            }
            return indices;
        }
        /// <summary>
        /// Get a file index by specifying 
        /// </summary>
        /// <param name="PseudoLocation"></param>
        /// <returns></returns>
        public static Index Get(string PseudoLocation)
        {

            Index index = null;
            var l = UnifyPseudoLocation(PseudoLocation).ToUpper();
            foreach (var item in Installation00.PresentingInstallations)
            {
                foreach (var _index in item.Indices)
                {
                    if (_index.PseudoLocation.ToUpper() == l)
                    {
                        _index.ParentInstallation = item.FolderID;
                        return _index;
                    }
                }
            }
            return index;
        }
        public static string DirecotrizeLocation(string Location)
        {
            if (Location.EndsWith('/')) return Location; else return (Location + "/");
        }
        public static string UnifyPseudoLocation(string Location)
        {
            var item = Location.Replace("\\", "/");
            if (!item.StartsWith("/")) item = "/" + item;
            return item;
        }
        /// <summary>
        /// Store a reference to given file.
        /// </summary>
        /// <param name="RealFile"></param>
        /// <param name="PseudoLocation"></param>
        public static void StoreRef(string RealFile, string PseudoLocation)
        {
            if (RealFile.StartsWith("~"))
            {
                RealFile.Substring(1);
                RealFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + RealFile;
            }
            StoreRef(new FileInfo(RealFile), PseudoLocation);
        }
        /// <summary>
        /// Store a reference to given file.
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="PseudoLocation"></param>
        public static void StoreRef(FileInfo fi, string PseudoLocation)
        {
            var installation = Installation00.GetInstallation();
            var fileID = Guid.NewGuid();
            PseudoLocation = UnifyPseudoLocation(PseudoLocation);
            var i = Get(PseudoLocation);
            if (i is not null)
            {
                i.RealLocation = fi.FullName;
            }
            else
            if (installation == Installation.Empty)
            {
                installation = new Installation();
                installation.FolderID = Guid.NewGuid().ToString();
                installation.Modified = true;
                var folder = Path.Combine(BasePath, installation.FolderID);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                Index index = new Index();
                index.RealLocation = fi.FullName;
                index.PseudoLocation = PseudoLocation;
                installation.Indices.Add(index);
                Installation00.PresentingInstallations.Add(installation);
                TheArk.Installations.Add(installation.FolderID);
                SaveIndeics();
            }
            else
            {
                installation.Modified = true;
                Index index = new Index();
                index.RealLocation = fi.FullName;
                index.PseudoLocation = PseudoLocation;
                installation.Indices.Add(index);
            }
        }
        /// <summary>
        /// Store a copy of given file.
        /// </summary>
        /// <param name="RealFile"></param>
        /// <param name="PseudoLocation"></param>
        public static void StoreCpy(string RealFile, string PseudoLocation)
        {
            if (RealFile.StartsWith("~"))
            {
                RealFile.Substring(1);
                RealFile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + RealFile;
            }
            StoreCpy(new FileInfo(RealFile), PseudoLocation);
        }
        /// <summary>
        /// Store a copy of given file.
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="PseudoLocation"></param>
        public static void StoreCpy(FileInfo fi, string PseudoLocation)
        {
            PseudoLocation = UnifyPseudoLocation(PseudoLocation);
            var installation = Installation00.GetInstallation();
            var fileID = Guid.NewGuid();
            {
                var index = Get(PseudoLocation);
                if (index != null)
                {
                    File.Copy(fi.FullName, index.RealLocation, true);
                    return;
                }
            }
            if (installation == Installation.Empty)
            {
                installation = new Installation();
                installation.FolderID = Guid.NewGuid().ToString();
                installation.Modified = true;
                var folder = Path.Combine(BasePath, installation.FolderID);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                var f = fi.CopyTo(Path.Combine(folder, fileID.ToString()));
                Index index = new Index();
                index.RealLocation = f.Name;
                index.PseudoLocation = PseudoLocation;
                installation.Indices.Add(index);
                Installation00.PresentingInstallations.Add(installation);
                TheArk.Installations.Add(installation.FolderID);
                SaveIndeics();
            }
            else
            {
                installation.Modified = true;
                var folder = Path.Combine(BasePath, installation.FolderID);
                var f = fi.CopyTo(Path.Combine(folder, fileID.ToString()));
                Index index = new Index();
                index.RealLocation = f.Name;
                index.PseudoLocation = PseudoLocation;
                installation.Indices.Add(index);
            }
        }
        public static void DeleteFile(string PseudoLocation)
        {
            var index = Get(PseudoLocation);
            if (index is not null)
            {
                index.CoreFile.Delete();
                var installation = Installation00.FindInstallation(index.ParentInstallation);
                installation.Indices.Remove(index);
                installation.Modified = true;
            }
        }
        /// <summary>
        /// Rename an index with given pseudo location. Note: must manually call SaveIndeics() to take effect.
        /// </summary>
        /// <param name="OldPseudoLocation"></param>
        /// <param name="NewPseudoLocation"></param>
        public static void Rename(string OldPseudoLocation, string NewPseudoLocation)
        {

            var N = UnifyPseudoLocation(NewPseudoLocation).ToUpper();
            var i = Get(OldPseudoLocation);
            i.PseudoLocation = N;
        }
        /// <summary>
        /// Rename the index with given pseudo location. Note: must manually call SaveIndeics() to take effect.
        /// </summary>
        /// <param name="NewPseudoLocation"></param>
        public void Rename(string NewPseudoLocation)
        {

            var N = UnifyPseudoLocation(NewPseudoLocation).ToUpper();
            PseudoLocation = N;
        }
        public static void SaveIndeics()
        {
            var Folder = Path.Combine(BasePath, "InstallationIndeices");
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
            foreach (var item in Installation00.PresentingInstallations)
            {
                if (item.Modified == true)
                {
                    item.Modified = false;
                    var content = JsonConvert.SerializeObject(item);
                    var file = Path.Combine(BasePath, "InstallationIndeices", item.FolderID);
                    if (File.Exists(file)) File.Delete(file);
                    File.WriteAllText(file, content);
                }
            }
            {

                var content = JsonConvert.SerializeObject(TheArk);
                var file = Path.Combine(BasePath, Installation00Path);
                if (File.Exists(file)) File.Delete(file);
                File.WriteAllText(file, content);
            }
        }
    }
    /// <summary>
    /// Sacred Ring. The ring is not a natural formation. Someone built it, so it must lead somewhere.
    /// </summary>
    [Serializable]
    public class Installation
    {
        public readonly static Installation Empty = new Installation();
        [JsonIgnore]
        public string FileName;
        [JsonIgnore]
        public bool Modified;
        public string FolderID;
        public List<Index> Indices = new();
    }
    /// <summary>
    /// The Ark. 
    /// </summary>
    [Serializable]
    public class Installation00
    {
        public static List<Installation> PresentingInstallations = new();
        internal static Installation FindInstallation(string ID)
        {
            foreach (var item in PresentingInstallations)
            {
                if (item.FolderID == ID) return item;
            }
            return null;
        }
        public static Installation GetInstallation()
        {
            Installation r = Installation.Empty;
            foreach (var item in PresentingInstallations)
            {
                if (item.Indices.Count < Index.MaxIndexices && item.Indices.Count > r.Indices.Count)
                {
                    r = item;
                }
            }
            return r;
        }
        public List<string> Installations = new();
    }
}
