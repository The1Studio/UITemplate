﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeurekaGames.Utils
{
    public class Heureka_PackageDataManager : ScriptableObject
    {
        public Texture2D          icon;
        public string             title;
        public List<PackageLinks> Links = new();

        public Heureka_PackageData[] sections;
        public bool                  loadedLayout;
    }
}