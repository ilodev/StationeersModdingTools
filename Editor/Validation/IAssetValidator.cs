using System.Collections.Generic;
using UnityEngine;

namespace ilodev.stationeers.moddingtools.validation
{
    public interface IAssetValidator
    {
        List<AssetIssue> Validate(string assetPath, Object asset);
    }
}
