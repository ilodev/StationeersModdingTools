﻿using System;

namespace ilodev.stationeers.moddingtools.exporter
{
    /// <summary>
    /// Flags for different types of content that can be included in a Mod.
    /// </summary>
    [Flags]
    public enum ModContent
    {
        Scenes = 1,
        Assets = 2,
        Code = 4
    }
}