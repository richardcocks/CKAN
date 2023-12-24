using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
#if NET5_0_OR_GREATER
using System.Runtime.Versioning;
#endif

using CKAN.Extensions;
using CKAN.GUI.Attributes;

namespace CKAN.GUI
{
    #if NET5_0_OR_GREATER
    [SupportedOSPlatform("windows")]
    #endif
    public partial class TagsLabelsLinkList : FlowLayoutPanel
    {
        [ForbidGUICalls]
        public void UpdateTagsAndLabels(IEnumerable<ModuleTag>   tags,
                                        IEnumerable<ModuleLabel> labels)
        {
            Util.Invoke(this, () =>
            {
                SuspendLayout();
                Controls.Clear();
                if (tags != null)
                {
                    foreach (ModuleTag tag in tags)
                    {
                        Controls.Add(TagLabelLink(
                            tag.Name, tag,
                            new LinkLabelLinkClickedEventHandler(TagLinkLabel_LinkClicked)));
                    }
                }
                if (labels != null)
                {
                    foreach (ModuleLabel mlbl in labels)
                    {
                        Controls.Add(TagLabelLink(
                            mlbl.Name, mlbl,
                            new LinkLabelLinkClickedEventHandler(LabelLinkLabel_LinkClicked)));
                    }
                }
                ResumeLayout();
            });
        }

        public event Action<SavedSearch, bool> OnChangeFilter;

        private static int LinkLabelBottom(LinkLabel lbl)
            => lbl == null ? 0
                           : lbl.Bottom + lbl.Margin.Bottom + lbl.Padding.Bottom;

        public int TagsHeight
            => LinkLabelBottom(Controls.OfType<LinkLabel>()
                                       .LastOrDefault());

        private LinkLabel TagLabelLink(string name,
                                       object tag,
                                       LinkLabelLinkClickedEventHandler onClick)
        {
            var link = new LinkLabel()
            {
                AutoSize     = true,
                LinkColor    = SystemColors.GrayText,
                LinkBehavior = LinkBehavior.HoverUnderline,
                Margin       = new Padding(0, 2, 4, 2),
                Text         = name,
                Tag          = tag,
            };
            link.LinkClicked += onClick;
            ToolTip.SetToolTip(link, Properties.Resources.FilterLinkToolTip);
            return link;
        }

        private void TagLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = sender as LinkLabel;
            var merge = ModifierKeys.HasAnyFlag(Keys.Control, Keys.Shift);
            OnChangeFilter?.Invoke(
                ModList.FilterToSavedSearch(GUIModFilter.Tag,
                                            link.Tag as ModuleTag, null),
                merge);
        }

        private void LabelLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = sender as LinkLabel;
            var merge = ModifierKeys.HasAnyFlag(Keys.Control, Keys.Shift);
            OnChangeFilter?.Invoke(
                ModList.FilterToSavedSearch(GUIModFilter.CustomLabel, null,
                                            link.Tag as ModuleLabel),
                merge);
        }

        private readonly ToolTip ToolTip = new ToolTip()
        {
            AutoPopDelay = 10000,
            InitialDelay = 250,
            ReshowDelay  = 250,
            ShowAlways   = true,
        };
    }
}
