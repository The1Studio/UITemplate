using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace HeurekaGames.AssetHunterPRO.BaseTreeviewImpl.DependencyGraph
{
    internal class AH_DepGraphHeader : MultiColumnHeader
    {
        private Mode m_Mode;

        public enum Mode
        {
            Treeview,
            SortedList,
        }

        public AH_DepGraphHeader(MultiColumnHeaderState state) : base(state)
        {
            this.mode = Mode.Treeview;
        }

        public Mode mode
        {
            get => this.m_Mode;
            set
            {
                this.m_Mode = value;
                switch (this.m_Mode)
                {
                    case Mode.Treeview:
                        this.canSort = true;
                        this.height  = DefaultGUI.minimumHeight;
                        break;
                    case Mode.SortedList:
                        this.canSort = true;
                        this.height  = DefaultGUI.defaultHeight;
                        break;
                }
            }
        }

        protected override void ColumnHeaderClicked(MultiColumnHeaderState.Column column, int columnIndex)
        {
            if (this.mode == Mode.Treeview) this.mode = Mode.SortedList;

            base.ColumnHeaderClicked(column, columnIndex);
        }
    }
}