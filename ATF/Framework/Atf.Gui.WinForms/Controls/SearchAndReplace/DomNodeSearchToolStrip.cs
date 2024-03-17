//Copyright © 2014 Sony Computer Entertainment America LLC. See License.txt.

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Sce.Atf.Applications;

namespace Sce.Atf.Dom
{
    /// <summary>
    /// ToolStrip GUI for specifying a search on DomNodes</summary>
    public class DomNodeSearchToolStrip : SearchToolStrip
    {
        /// <summary>
        /// Constructor</summary>
        public DomNodeSearchToolStrip()
        {
            // Define query tree, which defines the layout of the search toolstrip GUI
            m_rootNode = new DomNodeQueryRoot();
            m_rootNode.AddLabel("Find node(s)");

            // Add option to search DomNodes on either their name, or parameters
            QueryOption searchNameOrParam = m_rootNode.AddOption();
            searchNameOrParam.AddOptionItem("whose name", 0).AddDomNodeNameQuery(true);
            searchNameOrParam.AddOptionItem("with a parameter", 0).AddDomNodePropertyQuery();
            m_rootNode.AddSeparator();
            m_rootNode.RegisterSearchButtonPress(m_rootNode.AddButton("Search"));

            // Entering text into the toolstrip will trigger a search, changing an option rebuilds the toolstrip GUI
            m_rootNode.SearchTextEntered += searchSubStrip_SearchTextEntered;
            m_rootNode.OptionChanged += searchSubStrip_OptionsChanged;

            //
            // Build toolStrip GUI by retrieving toolstrip item list from tree, and adding
            // them to ToolStrip.Items
            //
            SuspendLayout();
            List<ToolStripItem> toolStripItems = new List<ToolStripItem>();
            m_rootNode.GetToolStripItems(toolStripItems);
            Items.AddRange(toolStripItems.ToArray());

            // Initialize ToolStrip
            Location = new System.Drawing.Point(0, 0);
            Name = "Event Sequence Document Search";
            Size = new System.Drawing.Size(292, 25);
            TabIndex = 0;
            Text = "Event Sequence Document Search";
            GripStyle = ToolStripGripStyle.Hidden;

            // Done
            ResumeLayout(false);
        }

        /// <summary>
        /// Event handler called when user has entered search text parameters into the ToolStrip</summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void searchSubStrip_SearchTextEntered(object sender, System.EventArgs e)
        {
            // Using search predicates generated by the user via this class, trigger the bound data set to start query on its data
            DoSearch();
        }

        /// <summary>
        /// Event handler called when user has changed the search options in the ToolStrip</summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event arguments</param>
        private void searchSubStrip_OptionsChanged(object sender, System.EventArgs e)
        {
            // Add to the tool strip the items newly represented by this QueryOption
            QueryOption changedOption = sender as QueryOption;
            if (changedOption != null)
            {
                Items.Clear();
                List<ToolStripItem> itemList = new List<ToolStripItem>();
                m_rootNode.GetToolStripItems(itemList);
                Items.AddRange(itemList.ToArray());
                UIChanged.Raise(sender, System.EventArgs.Empty);
            }
        }

        /// <summary>
        /// Adds instances of classes that implement IQueryPredicate to define what is searched</summary>
        /// <returns>IQueryPredicate search predicates</returns>
        public override IQueryPredicate GetPredicate()
        {
            // Parse query tree to build list of predicates, with which the search will be made
            return m_rootNode.GetPredicate();
        }

        /// <summary>
        /// Event raised by client when UI has graphically changed</summary>
        public override event EventHandler UIChanged;

        private readonly DomNodeQueryRoot m_rootNode;

        internal bool QueryDirty
        {
            get { return m_rootNode.QueryDirty; }
            set { m_rootNode.QueryDirty = value; }
        }

        internal bool QueryWithEmptyFields
        {
            get
            {
                foreach (var item in Items)
                {
                    var inputTextBox = item as ToolStripTextBox;
                    if (inputTextBox != null && !string.IsNullOrWhiteSpace(inputTextBox.Text))
                        return false;
                }
                return true;
            }
        }
    }
}

