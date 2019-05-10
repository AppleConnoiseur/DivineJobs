using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DivineJobs.Core
{
    /// <summary>
    /// Manages the view of a graph.
    /// </summary>
    public class GraphView
    {
        public IEnumerable<double> targetData = null;
        public double maxY, average;
        public int maxX;

        public GraphView()
        {

        }

        public GraphView(IEnumerable<double> targetData)
        {
            this.targetData = targetData;
        }

        public void SetupGraph()
        {
            if(targetData != null)
            {
                Log.Message("SetupGraph()");
                //Find out the mins and maxes. Also average value.
                average = 0d;
                maxX = 0;
                maxY = double.MinValue;
                foreach(double pointY in targetData)
                {
                    maxX++;
                    maxY = Math.Max(maxY, pointY);
                    average += pointY;
                }

                average = average / maxX;
            }
        }

        public void OnGUI(Rect graphRect)
        {
            Widgets.DrawMenuSection(graphRect);
            
            //Columns
            float spaceBetweenColumn = graphRect.width / maxX;
            float columnWidth = spaceBetweenColumn * 0.6f;

            int current = 0;
            foreach (double pointY in targetData)
            {
                {
                    float columnPositionX = (spaceBetweenColumn * 0.5f) + spaceBetweenColumn * current;
                    float columnHeight = (float)(pointY / maxY) * graphRect.height;
                    Rect columnRect = new Rect(graphRect.x + columnPositionX, graphRect.y + graphRect.height - columnHeight, columnWidth / 2f, columnHeight);

                    Widgets.DrawBoxSolid(columnRect, Color.green);
                    Widgets.DrawBox(columnRect);
                    Rect highlightRect = columnRect.ExpandedBy(3f);
                    Widgets.DrawHighlightIfMouseover(highlightRect);

                    if (Mouse.IsOver(highlightRect))
                    {
                        TooltipHandler.TipRegion(highlightRect, $"{current}: {pointY}");
                    }
                }

                current++;
            }
        }
    }
}
