using UnityEngine;

namespace ilodev.stationeers.moddingtools.validation
{
    public class AssetIssue
    {
        public UnityEngine.Object asset;
        public string path;
        public string description;
        public string category;
        public System.Action fixAction;
    }
}