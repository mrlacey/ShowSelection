using System;
using System.Text;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;

namespace ShowSelection
{
    class SelectionAdornment
    {
        private SelectionControl _root;
        private IWpfTextView _view;
        private IAdornmentLayer _adornmentLayer;

        public SelectionAdornment(IWpfTextView view)
        {
            _view = view;
            _root = new SelectionControl();

            // Grab a reference to the adornment layer that this adornment should be added to
            _adornmentLayer = view.GetAdornmentLayer("SelectionAdornment");

            // Reposition the adornment whenever the editor window is resized
            _view.ViewportHeightChanged += delegate { OnSizeChange(); };
            _view.ViewportWidthChanged += delegate { OnSizeChange(); };

            _view.Selection.SelectionChanged += delegate(object sender, EventArgs args)
            {
                var content = new StringBuilder();

                foreach (var selectedSpan in view.Selection.SelectedSpans)
                {
                    content.AppendLine($"{selectedSpan.Start.Position}:{selectedSpan.End.Position}");
                }

                _root.DisplayedSelection.Content = content.ToString().TrimEnd();
            };
        }

        public void OnSizeChange()
        {
            //clear the adornment layer of previous adornments
            _adornmentLayer.RemoveAdornment(_root);

            var margin = 5;

            //Place the image in the top right hand corner of the Viewport
            Canvas.SetLeft(_root, _view.ViewportRight - _root.ActualWidth - margin);
            Canvas.SetTop(_root, _view.ViewportTop + margin);

            // Don't show anythign if empty
            if (!string.IsNullOrWhiteSpace(_root.Content.ToString()))
            {
                //add the image to the adornment layer and make it relative to the viewports
                _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _root, null);
            }
        }
    }
}
