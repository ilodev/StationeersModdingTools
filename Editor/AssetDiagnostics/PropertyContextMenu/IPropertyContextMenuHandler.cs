using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ilodev.stationeersmods.tools.diagnostics
{
    public interface IPropertyContextMenuHandler
    {
        void Register(PropertyContextMenuRegistry registry);
    }
}
