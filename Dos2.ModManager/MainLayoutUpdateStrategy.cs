using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dos2.ModManager.ViewModels;
using Xceed.Wpf.AvalonDock.Layout;

namespace Dos2.ModManager
{

    // This is temporary.

    /// <summary>
    /// Main strategy for avalon dock for positioning elements to the docking manager.
    /// </summary>
    public class MainLayoutUpdateStrategy : ILayoutUpdateStrategy
    {
        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
            
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
            
        }
    
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            

            // Right Side Anchorables
            var rightSide = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(x => x.Name == "rightPane");
            //var rightSide = layout.RightSide.Descendents().OfType<LayoutAnchorGroup>().FirstOrDefault();
            if (rightSide != null && (anchorableToShow.Content is PropertiesViewModel || anchorableToShow.Content is ConflictsViewModel))
            {
                anchorableToShow.AutoHideWidth = 200;
                rightSide.InsertChildAt(0, anchorableToShow);
                return true;
            }

            // Bottom Anchorables
            var sub = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.Name == "bottomPane");
            if(sub != null && (anchorableToShow.Content is LogViewModel ))
            {
                sub.InsertChildAt(0, anchorableToShow);
                return true;
            }

            // Main Anchorables
            var main = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.Name == "mainPane");
            if (main != null && (anchorableToShow.Content is WorkspaceViewModel))
            {
                main.InsertChildAt(0, anchorableToShow);
                return true;
            }

            if (destinationContainer is LayoutAnchorablePane pane)
            {
                pane.InsertChildAt(0, anchorableToShow);
                return true;
            }

            return false;
        }

        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            if(destinationContainer is LayoutDocumentPane pane)
            {
                var count = pane.Children.Count;
                pane.InsertChildAt(count, anchorableToShow);
                return true;
            }

            return false;
        }
    }
}
