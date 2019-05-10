using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DivineJobs.Core
{
    /// <summary>
    /// A collapsible section in UI.
    /// </summary>
    public class SectionRect
    {
        public Action<Rect> renderAction;
        public Func<float> heightGetter;
        public bool isCollapsed = false;
        public float collapsedHeight = 24f;
        private float cachedHeight = 0f;
        private bool cached = false;

        public float Height
        {
            get
            {
                if(isCollapsed)
                {
                    return collapsedHeight;
                }
                else
                {
                    if(!cached)
                    {
                        cachedHeight = heightGetter();
                        cached = true;
                    }

                    return cachedHeight;
                }
            }
        }

        public SectionRect()
        {

        }

        public SectionRect(Action<Rect> renderAction, Func<float> heightGetter = null)
        {
            this.renderAction = renderAction;
            this.heightGetter = heightGetter;
        }

        public void ResetCache()
        {
            cached = false;
        }
    }
}
