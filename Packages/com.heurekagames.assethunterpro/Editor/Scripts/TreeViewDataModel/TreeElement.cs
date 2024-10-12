using System;
using System.Collections.Generic;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl
{
    [Serializable]
    public class TreeElement
    {
        [SerializeField] internal int               m_ID;
        [SerializeField] internal string            m_Name;
        [SerializeField] internal int               m_Depth;
        [SerializeField] private  bool              m_Enabled;
        [NonSerialized]  private  TreeElement       m_Parent;
        [NonSerialized]  private  List<TreeElement> m_Children;

        public int depth { get => this.m_Depth; set => this.m_Depth = value; }

        public TreeElement parent { get => this.m_Parent; set => this.m_Parent = value; }

        public List<TreeElement> children { get => this.m_Children; set => this.m_Children = value; }

        public bool hasChildren => this.children != null && this.children.Count > 0;

        public string Name { get => this.m_Name; set => this.m_Name = value; }

        public int id { get => this.m_ID; set => this.m_ID = value; }

        public bool Enabled { get => this.m_Enabled; set => this.m_Enabled = value; }

        public TreeElement()
        {
        }

        public TreeElement(string name, int depth, int id)
        {
            this.m_Name  = name;
            this.m_ID    = id;
            this.m_Depth = depth;
        }
    }
}