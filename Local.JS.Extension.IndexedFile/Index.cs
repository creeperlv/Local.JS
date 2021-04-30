﻿using System;
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
        public FileInfo CoreFile { get {
                return new FileInfo(Path.Combine(BasePath, ParentInstallation, RealLocation));
            }
        }
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
        public static Index Get(string PsesudoLocation)
        {

            Index index = null;
            var l = PsesudoLocation.ToUpper();
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
        //public static void StoreRef(string RealFile, string PsesudoLocation)
        //{
        //    StoreRef(new FileInfo(RealFile), PsesudoLocation);
        //}
        //public static void StoreRef(FileInfo fi, string PsesudoLocation)
        //{
        //    var installation = Installation00.GetInstallation();
        //    var fileID = Guid.NewGuid();
        //    if (installation == Installation.Empty)
        //    {
        //        installation = new Installation();
        //        installation.FolderID = Guid.NewGuid().ToString();
        //        installation.Modified = true;
        //        var folder = Path.Combine(BasePath, installation.FolderID);
        //        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        //        Index index = new Index();
        //        index.RealLocation = fi.FullName;
        //        index.PseudoLocation = PsesudoLocation;
        //        installation.Indices.Add(index);
        //        Installation00.PresentingInstallations.Add(installation);
        //        TheArk.Installations.Add(installation.FolderID);
        //        SaveIndeics();
        //    }
        //    else
        //    {
        //        installation.Modified = true;
        //        Index index = new Index();
        //        index.RealLocation = fi.FullName;
        //        index.PseudoLocation = PsesudoLocation;
        //        installation.Indices.Add(index);
        //    }
        //}
        public static void StoreCpy(string RealFile, string PsesudoLocation)
        {
            StoreCpy(new FileInfo(RealFile), PsesudoLocation);
        }
        public static void StoreCpy(FileInfo fi, string PsesudoLocation)
        {
            var installation = Installation00.GetInstallation();
            var fileID = Guid.NewGuid();
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
                index.PseudoLocation = PsesudoLocation;
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
                index.PseudoLocation = PsesudoLocation;
                installation.Indices.Add(index);
            }
        }
        public static void DeleteFile(string PsesudoLocation)
        {
            var index=Get(PsesudoLocation);
            if(index is not null)
            {
                index.CoreFile.Delete();
                var installation = Installation00.FindInstallation(index.ParentInstallation);
                installation.Indices.Remove(index);
                installation.Modified = true;
            }
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