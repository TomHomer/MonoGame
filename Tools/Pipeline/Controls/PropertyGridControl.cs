﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using System.Reflection;
using System.Linq;
using System.ComponentModel;

namespace MonoGame.Tools.Pipeline
{
    partial class PropertyGridControl
    {
        List<IProjectItem> selectedObjects;

        public PropertyGridControl()
        {
            InitializeComponent();
        }

        public void SetSelectedItems(List<IProjectItem> objects)
        {
            selectedObjects = objects;

            Reload();
        }

        private object CompareVariables(object a, object b)
        {
            if (a == null)
                return a;

            if (a.ToString () == "???" || a.Equals(b))
                return b;

            return null;
        }

        public void Reload()
        {
            propertyTable.Clear();

            if (selectedObjects.Count == 0)
                return;

            var objectType = selectedObjects[0].GetType ();
            var props = objectType.GetProperties (BindingFlags.Instance | BindingFlags.Public);

            foreach (var p in props)
            {
                var attrs = p.GetCustomAttributes(true);
                var browsable = true;
                var category = "Mics";

                foreach (var a in attrs)
                {
                    if (a is BrowsableAttribute)
                        browsable = (a as BrowsableAttribute).Browsable;
                    else if (a is CategoryAttribute)
                        category = (a as CategoryAttribute).Category;
                }

                if (!browsable)
                    continue;

                object value = "???";
                foreach (object o in selectedObjects) 
                    value = CompareVariables (value, p.GetValue (o, null));

                propertyTable.AddEntry(category, p, value);
            }

            propertyTable.Update();
        }
    }
}

