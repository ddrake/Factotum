using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{
	public partial class CustomerConfigView : Form
	{
		// Constructor
		public CustomerConfigView()
		{
			InitializeComponent();
			// This collection holds any inactive nodes (and any children they contain)
			// when the user is hiding inactive items.
			inactiveNodes = new TreeNodeEntityCollection();

		}
		// 
		private void CustomerConfigView_Load(object sender, EventArgs e)
		{
			// Fill the TreeView with all nodes
			FillTree();
			// Hide inactive items by default
			ckShowInactive.Checked = false;
			HideInactive(tvwCustomerConfig.Nodes);
			// Update the button text and hide/show buttons as required.
			UpdateButtonText();
			// Add this after initialization.  We only want user-initiated events.
			this.ckShowInactive.CheckedChanged += new System.EventHandler(this.ckShowInactive_CheckedChanged);
			// Wire up the handlers for the Entity changed events
			ECustomer.Added += new EventHandler<EntityChangedEventArgs>(Entity_Added);
			ESite.Added += new EventHandler<EntityChangedEventArgs>(Entity_Added);
			EUnit.Added += new EventHandler<EntityChangedEventArgs>(Entity_Added);
			EOutage.Added += new EventHandler<EntityChangedEventArgs>(Entity_Added);

			ECustomer.Updated += new EventHandler<EntityChangedEventArgs>(Entity_Updated);
			ESite.Updated += new EventHandler<EntityChangedEventArgs>(Entity_Updated);
			EUnit.Updated += new EventHandler<EntityChangedEventArgs>(Entity_Updated);
			EOutage.Updated += new EventHandler<EntityChangedEventArgs>(Entity_Updated);

			ECustomer.Deleted += new EventHandler<EntityChangedEventArgs>(Entity_Deleted);
			ESite.Deleted += new EventHandler<EntityChangedEventArgs>(Entity_Deleted);
			EUnit.Deleted += new EventHandler<EntityChangedEventArgs>(Entity_Deleted);
			EOutage.Deleted += new EventHandler<EntityChangedEventArgs>(Entity_Deleted);

			if (Globals.CurrentOutageID != null)
			{
				TreeNodeEntity node = FindByID(this.tvwCustomerConfig.Nodes, Globals.CurrentOutageID);
				if (node != null)
				{
					node.EnsureVisible();
					this.tvwCustomerConfig.SelectedNode = node;
				}
			}
		}

		private void CustomerConfigView_FormClosed(object sender, FormClosedEventArgs e)
		{
			// Unwire the handlers for the Entity changed events
			ECustomer.Added -= new EventHandler<EntityChangedEventArgs>(Entity_Added);
			ESite.Added -= new EventHandler<EntityChangedEventArgs>(Entity_Added);
			EUnit.Added -= new EventHandler<EntityChangedEventArgs>(Entity_Added);
			EOutage.Added -= new EventHandler<EntityChangedEventArgs>(Entity_Added);

			ECustomer.Updated -= new EventHandler<EntityChangedEventArgs>(Entity_Updated);
			ESite.Updated -= new EventHandler<EntityChangedEventArgs>(Entity_Updated);
			EUnit.Updated -= new EventHandler<EntityChangedEventArgs>(Entity_Updated);
			EOutage.Updated -= new EventHandler<EntityChangedEventArgs>(Entity_Updated);

			ECustomer.Deleted -= new EventHandler<EntityChangedEventArgs>(Entity_Deleted);
			ESite.Deleted -= new EventHandler<EntityChangedEventArgs>(Entity_Deleted);
			EUnit.Deleted -= new EventHandler<EntityChangedEventArgs>(Entity_Deleted);
			EOutage.Deleted -= new EventHandler<EntityChangedEventArgs>(Entity_Deleted);
		}


		//------------------------------------------------------------------------
		// Entity Change (ADD/DELETE/UPDATE) event handlers
		//------------------------------------------------------------------------
		void Entity_Deleted(object sender, EntityChangedEventArgs e)
		{
            int level = 0;
            Guid? parentID = null;
            EntityType type;
            String name = null;
            bool active = false;
            
            // Just remove the node with the given ID
			if (!DeleteForID(tvwCustomerConfig.Nodes, e.ID))
				DeleteForID(inactiveNodes, e.ID);

            GetEntityInfo(sender, e, out level, out parentID, out type, out name, out active);
            if (type == EntityType.Outage)
            {
                if (Globals.CurrentOutageID == e.ID)
                {
                    UserSettings.sets.CurrentOutageID = "";
                    UserSettings.Save();
                    Globals.SetCurrentOutageID();

                    // Handle enabling of the outage menu.
                    AppMain main = (AppMain)this.MdiParent;
                    main.handleMenuEnabling();
                }
            }
        }

		void Entity_Added(object sender, EntityChangedEventArgs e)
		{
			int level = 0;
			Guid? parentID = null;
			EntityType type;
			String name = null;
			bool active = false;

			// Get the info from the entity that changed.  We will use it to set up the node.
			GetEntityInfo(sender, e, out level, out parentID, out type, out name, out active);

            if (type == EntityType.Outage)
            {
                DialogResult rslt = MessageBox.Show("Set this new outage as the current default outage?",
                    "Factotum",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                if (rslt == DialogResult.Yes)
                {
                    UserSettings.sets.CurrentOutageID = e.ID.ToString();
                    UserSettings.Save();
                    Globals.SetCurrentOutageID();

                    // Handle enabling of the outage menu.
                    AppMain main = (AppMain)this.MdiParent;
                    main.handleMenuEnabling();
                }
            }
			// Create the new node to be added
			TreeNodeEntity newNode = new TreeNodeEntity(name, e.ID, parentID,
					(int)ActiveImageIdx(type), (int)InactiveImageIdx(type), active, (int)type);

			if (active || ckShowInactive.Checked)
			{
				AddNode(tvwCustomerConfig.Nodes, 0, level, ref newNode);
				newNode.EnsureVisible();
				tvwCustomerConfig.SelectedNode = newNode;
				tvwCustomerConfig.Focus();
			}
			else
			{
				// We just added a new inactive entity...

				// There shouldn't be any orphans to worry about since this is a NEW node...
				inactiveNodes.Add(newNode);
			}
		}

		void Entity_Updated(object sender, EntityChangedEventArgs e)
		{
			int level = 0;
			Guid? parentID = null;
			EntityType type;
			String name = null;
			bool active = false;

			// Get the info from the entity that changed.  We will use it to set up the node.
			GetEntityInfo(sender, e, out level, out parentID, out type, out name, out active);

			// We are only concerned with three types of changes:  Status (active/inactive), Name, Parent
			TreeNodeEntity node = GetNode(e.ID);
			 
			if (node.ParentID != parentID || node.EntityName != name || node.IsActive != active)
			{
				// Remove the node (and its children) from its current tree
				if (!DeleteForID(tvwCustomerConfig.Nodes, e.ID))
					DeleteForID(inactiveNodes, e.ID);

				if (node.ParentID != parentID) node.ParentID = parentID;
				if (node.EntityName != name) node.EntityName = name;

				// If it's no longer active and we're hiding inactive items, move it to the hidden collection.
				if (node.IsActive &&  !active && !ckShowInactive.Checked)
				{
					node.IsActive = active;
					// This will add to the inactive items, but also add in any orphans
					AddToHiddenCollection(node);
				}
				else
				{
					// Ether it's active or we're showing inactive items, so make it visible
					if (node.IsActive != active) node.IsActive = active;

					AddNode(tvwCustomerConfig.Nodes, 0, level, ref node);
					node.EnsureVisible();
					tvwCustomerConfig.SelectedNode = node;
					tvwCustomerConfig.Focus();
				}
			}					
		}


		//------------------------------------------------------------------------
		// Form control event handlers
		//------------------------------------------------------------------------

		private void tvwCustomerConfig_AfterSelect(object sender, TreeViewEventArgs e)
		{
			UpdateButtonText();
		}

		private void btnAddCustomer_Click(object sender, EventArgs e)
		{
			AddCustomer();
		}

		private void btnAddSelected_Click(object sender, EventArgs e)
		{
			AddForSelected();
		}

		private void btnEditSelected_Click(object sender, EventArgs e)
		{
			EditSelectedItem();
		}

		private void btnDeleteSelected_Click(object sender, EventArgs e)
		{
			DeleteSelectedItem();
		}

		private void btnToggleStatusSelected_Click(object sender, EventArgs e)
		{
			ToggleActiveStatusOfSelectedItem();
		}

		private void ckShowInactive_CheckedChanged(object sender, EventArgs e)
		{
			if (!ckShowInactive.Checked)
			{
				HideInactive(tvwCustomerConfig.Nodes);
			}
			else
			{
				ShowInactive();
			}
			UpdateButtonText();
			tvwCustomerConfig.Select();
		}

		private void tvwCustomerConfig_DoubleClick(object sender, EventArgs e)
		{
			if (tvwCustomerConfig.SelectedNode != null)
			{
				// Added this because double-clicking a node toggles its expanded/contracted state
				// This is a cheesy way to counter that.  The right way is to derive a new class
				// from treeview, override its WndProc method, etc...
				tvwCustomerConfig.SelectedNode.Toggle();
				EditSelectedItem();
			}
		}


		//------------------------------------------------------------------------
		// ADDING, EDITING, DELETING, AND TOGGLING ACTIVE STATUS
		//
		// Note: These functions do not modify the tree view or inactive nodes collection
		// directly.  Instead, they 
		//------------------------------------------------------------------------

		// Add a new customer
		private void AddCustomer()
		{
			TreeNodeEntity node = null;
			if (tvwCustomerConfig.SelectedNode != null) node = (TreeNodeEntity)tvwCustomerConfig.SelectedNode;
			else if (tvwCustomerConfig.TopNode != null) node = (TreeNodeEntity)tvwCustomerConfig.TopNode;
			CustomerEdit frm = new CustomerEdit(null);
			frm.MdiParent = this.MdiParent;
			frm.Show();
		}

		// Adds Sites to Customers, adds Units to Sites, and adds Systems to Units
		private void AddForSelected()
		{
			if (tvwCustomerConfig.SelectedNode == null)
			{
				MessageBox.Show("Please select an item to add to first", "Factotum");
				return;
			}
			TreeNodeEntity parentNode = (TreeNodeEntity)tvwCustomerConfig.SelectedNode;
			// initialize to parent so we have a node to sort and display if user cancels
			TreeNodeEntity node = parentNode;
			switch ((EntityType)parentNode.EntityType)
			{
				case EntityType.Customer:
					// Add a new site to the customer
					SiteEdit frmCst = new SiteEdit(null, parentNode.ID);
					frmCst.MdiParent = this.MdiParent;
					frmCst.Show();
					break;
				case EntityType.Site:
					UnitEdit frmSit = new UnitEdit(null, parentNode.ID);
					frmSit.MdiParent = this.MdiParent;
					frmSit.Show();
					break;
				case EntityType.Unit:
					OutageEdit frm = new OutageEdit(null, parentNode.ID);
					frm.MdiParent = this.MdiParent;
					frm.Show();
					break;
				default:
					return;
			}
		}

		// Edit the entity corresponding to the currently selected node
		private void EditSelectedItem()
		{
			if (tvwCustomerConfig.SelectedNode == null)
			{
				MessageBox.Show("Please select an item to edit first", "Factotum");
				return;
			}
			TreeNodeEntity node = (TreeNodeEntity)tvwCustomerConfig.SelectedNode;
			switch ((EntityType)node.EntityType)
			{
				case EntityType.Customer:
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "CustomerEdit", node.ID))
					{
						// Open the edit form with the currently selected ID.
						CustomerEdit frmCst = new CustomerEdit(node.ID);
						frmCst.MdiParent = this.MdiParent;
						frmCst.Show();
					}
					break;
				case EntityType.Site:
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "SiteEdit", node.ID))
					{
						// Open the edit form with the currently selected ID.
						SiteEdit frmSit = new SiteEdit(node.ID);
						frmSit.MdiParent = this.MdiParent;
						frmSit.Show();
					}
					break;
				case EntityType.Unit:
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "UnitEdit", node.ID))
					{
						// Open the edit form with the currently selected ID.
						UnitEdit frmUnt = new UnitEdit(node.ID);
						frmUnt.MdiParent = this.MdiParent;
						frmUnt.Show();
					}
					break;
				case EntityType.Outage:
					// First check to see if an instance of the form set to the selected ID already exists
					if (!Globals.CanActivateForm(this, "OutageEdit", node.ID))
					{
						// Open the edit form with the currently selected ID.
						OutageEdit frmOtg = new OutageEdit(node.ID);
						frmOtg.MdiParent = this.MdiParent;
						frmOtg.Show();
					}
					break;
				default:
					break;
			}
		}

		bool CantDelete_FormOpen(string EditFormName, Guid? id)
		{
			if (Globals.IsFormOpen(this, EditFormName, id))
			{
				MessageBox.Show("Can't delete because that item is currently being edited.", "Factotum");
				return true;
			}
			return false;
		}

		// Delete the entity corresponding to the currently selected node.
		private void DeleteSelectedItem()
		{
			if (tvwCustomerConfig.SelectedNode == null)
			{
				MessageBox.Show("Please select an item to delete first", "Factotum");
				return;
			}
			TreeNodeEntity node = (TreeNodeEntity)tvwCustomerConfig.SelectedNode;
			switch ((EntityType)node.EntityType)
			{
				case EntityType.Customer:
					if (CantDelete_FormOpen("CustomerEdit", node.ID)) return;
					ECustomer customer = new ECustomer(node.ID);

					customer.Delete(true);
					if (customer.CustomerErrMsg != null)
					{
						MessageBox.Show(customer.CustomerErrMsg, "Factotum");
						customer.CustomerErrMsg = null;
					}
					break;
				case EntityType.Site:
					if (CantDelete_FormOpen("SiteEdit", node.ID)) return;
					ESite site = new ESite(node.ID);
					site.Delete(true);
					if (site.SiteErrMsg != null)
					{
						MessageBox.Show(site.SiteErrMsg, "Factotum");
						site.SiteErrMsg = null;
					}
					break;
				case EntityType.Unit:
					if (CantDelete_FormOpen("UnitEdit", node.ID)) return;
					EUnit unit = new EUnit(node.ID);
					unit.Delete(true);
					if (unit.UnitErrMsg != null)
					{
						MessageBox.Show(unit.UnitErrMsg, "Factotum");
						unit.UnitErrMsg = null;
					}
					break;
				case EntityType.Outage:
					if (CantDelete_FormOpen("OutageEdit", node.ID)) return;
					EOutage outage = new EOutage(node.ID);
					outage.Delete(true);
					if (outage.OutageErrMsg != null)
					{
						MessageBox.Show(outage.OutageErrMsg, "Factotum");
						outage.OutageErrMsg = null;
					}
					break;
			}
			
		}

		// Toggle the active/inactive status of the entity corresponding to the selected node.
		// All entities except outages can be inactivated.
		private void ToggleActiveStatusOfSelectedItem()
		{
			TreeNodeEntity node = (TreeNodeEntity)tvwCustomerConfig.SelectedNode;
			if (node == null) return;

			switch ((EntityType)node.EntityType)
			{
				case EntityType.Customer:
					ECustomer customer = new ECustomer(node.ID);
					customer.CustomerIsActive = !customer.CustomerIsActive;
					customer.Save();
					break;
				case EntityType.Site:
					ESite site = new ESite(node.ID);
					site.SiteIsActive = !site.SiteIsActive;
					site.Save();
					break;
				case EntityType.Unit:
					EUnit unit = new EUnit(node.ID);
					unit.UnitIsActive = !unit.UnitIsActive;
					unit.Save();
					break;
			}
		}


		//------------------------------------------------------------------------
		// Node Management Helper Functions
		//------------------------------------------------------------------------

		// Add a node into a TreeNodeEntityCollection (for visible items)
		// The TreeNodeEntity 'node' is passed by ref.  It's a reference type, so this 
		// should not be necessary as we are not creating a new node object and assigning
		// to it from within this function.

		// However, a number of ObjectDisposedExceptions "Cannot access a disposed object" are 
		// being thrown when this form is not active and the active form is generating events that cause
		// event handlers in this form to add nodes to the treeview, then use the new node's
		// Parent property to access methods of the Treeview such as EnsureVisible().  
		// Passing by ref prevents these -- or at least it prevents the only one that I could reproduce.
		// which occurred whenever a new system was inserted using the component importer with this
		// form open.
		// I don't fully understand why these exceptions were getting thrown though...
		// ... All figured out now.  Todo: update the above comment and revisit this issue
		bool AddNode(TreeNodeCollection nodes, int curLevel, int level, ref TreeNodeEntity nodeToAdd)
		{
			if (curLevel == level)
			{
				int i;
				for (i = 0; i < nodes.Count; i++)
				{
					TreeNodeEntity sibling = (TreeNodeEntity)nodes[i];
					if (sibling.EntityType >= nodeToAdd.EntityType)
					{
						if (String.Compare(sibling.Name, nodeToAdd.Name) > 0)
						{
							nodes.Insert(i, nodeToAdd);
							return true;
						}
					}
				}
				nodes.Insert(i, nodeToAdd);
				return true;
			}
			else if (curLevel == level - 1)
			{
				for (int i = 0; i < nodes.Count; i++)
				{
					if (((TreeNodeEntity)nodes[i]).ID == nodeToAdd.ParentID)
					{
						AddNode(nodes[i].Nodes, curLevel + 1, level, ref nodeToAdd);
						return true;
					}
				}
				return false;
			}
			else // curLevel < level - 1
			{
				for (int i = 0; i < nodes.Count; i++)
				{
					if (AddNode(nodes[i].Nodes, curLevel + 1, level, ref nodeToAdd))
					{
						return true;
					}
				}
			}
			return false;
		}

		void AddToHiddenCollection(TreeNodeEntity node)
		{
			// Just add the node at the top level of inactive nodes tree
			// The order is not important...
			inactiveNodes.Add(node);

			// Now add any children which had been previously inactivated to the node.
			AddOrphans(node);
		}

		// Check among the inactive nodes for any nodes which have the current node (or one of its subnodes)
		// as their parent.  If any are found, add them to the appropriate node.
		void AddOrphans(TreeNodeEntity node)
		{
			TreeNodeEntityCollection nodes = FindAllByParentID(inactiveNodes, node.ID);
			for (int i = 0; i < nodes.Count; i++) 
			{
				TreeNodeEntity childNode = nodes[i];
				inactiveNodes.Remove(childNode);
				AddNode(node.Nodes, 0, 0, ref childNode);
			}
			foreach (TreeNodeEntity curChild in node.Nodes)
			{
				AddOrphans(curChild);
			}
		}


		//------------------------------------------------------------------------
		// Private helper functions
		//------------------------------------------------------------------------

		// Given the changed event args and sender, return the info needed to make a TreeNodeEntity
		void GetEntityInfo(object sender, EntityChangedEventArgs e, out int level, out Guid? parentID,
			out EntityType type, out string name, out bool active)
		{
			level = 0;
			parentID = null;
			type = EntityType.Customer;
			name = null;
			active = true;
			if (sender is ECustomer)
			{
				level = 0;
				type = EntityType.Customer;
				ECustomer customer = new ECustomer(e.ID);
				parentID = null;
				name = customer.CustomerName;
				active = customer.CustomerIsActive;
			}
			else if (sender is ESite)
			{
				level = 1;
				type = EntityType.Site;
				ESite site = new ESite(e.ID);
				parentID = site.SiteCstID;
				name = site.SiteName;
				active = site.SiteIsActive;
			}
			else if (sender is EUnit)
			{
				level = 2;
				type = EntityType.Unit;
				EUnit unit = new EUnit(e.ID);
				parentID = unit.UnitSitID;
				name = unit.UnitName;
				active = unit.UnitIsActive;
			}
			else if (sender is EOutage)
			{
				level = 3;
				type = EntityType.Outage;
				EOutage outage = new EOutage(e.ID);
				parentID = outage.OutageUntID;
				name = outage.OutageName;
				active = true;
			}
		}

		// Recursively find inactive nodes in the tree view and moves them 
		// (with their children) to the inactive list.
		private void HideInactive(TreeNodeCollection nodes)
		{
			int i = 0;
			while (i < nodes.Count)
			{
				TreeNodeEntity node = (TreeNodeEntity)nodes[i];
				if (!node.IsActive)
				{
					node.Remove();
					AddToHiddenCollection(node);
				}
				else
				{
					if (node.Nodes.Count > 0) HideInactive((TreeNodeCollection)node.Nodes);
					i++;
				}
			}
		}

		// Moves inactive nodes from the inactive list back into the appropriate
		// locations in the tree view.
		private void ShowInactive()
		{
			int i = 0;
			int inactiveCount = inactiveNodes.Count;
			while (i < inactiveCount)
			{
				TreeNodeEntity node = inactiveNodes[0];
				// 
				if (node.ParentID == null)
				{
					inactiveNodes.Remove(node);
					AddNode(tvwCustomerConfig.Nodes, 0, 0, ref node);
				}
				else
				{
					TreeNodeEntity parentNode = FindByID(tvwCustomerConfig.Nodes, (Guid?)node.ParentID);
					if (parentNode != null)
					{
						inactiveNodes.Remove(node);
						AddNode(parentNode.Nodes, 0, 0, ref node);
					}
				}
				i++;
			}
		}

		// Delete a node from a TreeNodeCollection (normally the TreeView) 
		private bool DeleteForID(TreeNodeCollection nodes, Guid? id)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				if (((TreeNodeEntity)nodes[i]).ID == id)
				{
					nodes[i].Remove();
					return true;
				}
				else if (nodes[i].Nodes.Count > 0)
				{
					if (DeleteForID(nodes[i].Nodes, id))
						return true;
				}
			}
			return false;
		}

		// Delete a node from a TreeNodeEntityCollection (normally the Inactive nodes) 
		private bool DeleteForID(TreeNodeEntityCollection nodes, Guid? id)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				if (((TreeNodeEntity)nodes[i]).ID == id)
				{
					nodes[i].Remove();
					return true;
				}
				else if (nodes[i].Nodes.Count > 0)
				{
					if (DeleteForID(nodes[i].Nodes, id))
						return true;
				}
			}
			return false;
		}

		private TreeNodeEntity GetNode(Guid? id)
		{
			TreeNodeEntity node;
			node = FindByID(tvwCustomerConfig.Nodes, id);
			if (node == null) node = FindByID(inactiveNodes, id);
			return node;
		}

		// Find a node in a TreeNodeCollection (normally the TreeView) if it is there
		// If the node is not found, returns null.
		private TreeNodeEntity FindByID(TreeNodeCollection nodes, Guid? id)
		{
			foreach (TreeNodeEntity node in nodes)
			{
				TreeNodeEntity childNode = null;
				if (node.ID == id) return node;
				if (node.Nodes.Count > 0)
					childNode = FindByID(node.Nodes, id);
				if (childNode != null) return childNode;
			}
			return null;
		}

		// Find a node in a TreeNodeEntityCollection (normally the InactiveNodes list)
		// if it is there.  If the node is not found, returns null.
		private TreeNodeEntity FindByID(TreeNodeEntityCollection nodes, Guid? id)
		{
			foreach (TreeNodeEntity node in nodes)
			{
				TreeNodeEntity childNode = null;
				if (node.ID == id) return node;
				if (node.Nodes.Count > 0)
					childNode = FindByID(node.Nodes, id);
				if (childNode != null) return childNode;
			}
			return null;
		}

		// Get a TreeNodeEntityCollection of all nodes that belong to the given parent. 
		// Used when Inactivating a node to find any nodes in the inactive nodes list 
		// That belong to the node being inacativated.  
		// These children will need to be added back to the newly inactivated node.
		private TreeNodeEntityCollection FindAllByParentID(TreeNodeEntityCollection nodes, Guid? parentId)
		{
			TreeNodeEntityCollection childNodes = new TreeNodeEntityCollection();
			foreach (TreeNodeEntity node in nodes)
			{
				if (node.ParentID == parentId) childNodes.Add(node);
			}
			return childNodes;
		}

		// Fill the tree with nodes for all entities (Customers, Sites, Units, etc..
		// Nodes are added for both Active and Inactive entities.
		private void FillTree()
		{
			ECustomerCollection customers = ECustomer.ListByName(true, true, false);
			tvwCustomerConfig.TreeViewNodeSorter = (IComparer)new NodeSorter();

			foreach (ECustomer customer in customers)
			{
				TreeNodeEntity customerNode = new TreeNodeEntity(customer.CustomerName,
					customer.ID, null, (int)ImageIdx.CustomerActive, (int)ImageIdx.CustomerInactive,
					customer.CustomerIsActive, (int)EntityType.Customer);
				tvwCustomerConfig.Nodes.Add(customerNode);

				ESiteCollection sites = ESite.ListForCustomer((Guid)customer.ID, false, true, true, false);
				foreach (ESite site in sites)
				{
					TreeNodeEntity siteNode = new TreeNodeEntity(site.SiteName,
						site.ID, site.SiteCstID, (int)ImageIdx.SiteActive, (int)ImageIdx.SiteInactive,
						site.SiteIsActive, (int)EntityType.Site);
					customerNode.Nodes.Add(siteNode);

					EUnitCollection units = EUnit.ListForSite((Guid)site.ID, false, true, true, false);
					foreach (EUnit unit in units)
					{
						TreeNodeEntity unitNode = new TreeNodeEntity(unit.UnitName,
							unit.ID, unit.UnitSitID, (int)ImageIdx.UnitActive, (int)ImageIdx.UnitInactive,
							unit.UnitIsActive, (int)EntityType.Unit);
						siteNode.Nodes.Add(unitNode);

						EOutageCollection outages = EOutage.ListForUnit((Guid)unit.ID, false);
						foreach (EOutage outage in outages)
						{
							TreeNodeEntity outageNode = new TreeNodeEntity(outage.OutageName,
								outage.ID, outage.OutageUntID, (int)ImageIdx.Outage, (int)ImageIdx.Outage,
								true, (int)EntityType.Outage);
							unitNode.Nodes.Add(outageNode);
						}

					}
				}
			}
			if (tvwCustomerConfig.Nodes.Count > 0 && tvwCustomerConfig.SelectedNode == null)
				tvwCustomerConfig.SelectedNode = tvwCustomerConfig.TopNode;
		}

		// Update the Text on the ADD/EDIT/DELETE/TOGGLE ACTIVE buttons
		// Also manage visibility.
		private void UpdateButtonText()
		{
			TreeNodeEntity node = (TreeNodeEntity)tvwCustomerConfig.SelectedNode;
			btnAddSelected.Visible = false;
			btnToggleStatusSelected.Visible = true;
			if (node == null)
			{
				btnAddSelected.Text = "Add To Selected";
                toolTip1.SetToolTip(this.btnAddSelected, "");
				btnDeleteSelected.Text = "Delete Selected";
				btnEditSelected.Text = "Edit Selected";
				btnToggleStatusSelected.Text = "(In)activate Selected";
				return;
			}
			btnAddSelected.Visible = node.IsActive;
			btnDeleteSelected.Visible = node.Nodes.Count == 0;
			switch ((EntityType)node.EntityType)
			{
				case EntityType.Customer:
					btnAddSelected.Text = "Add Site";
                    toolTip1.SetToolTip(this.btnAddSelected, "Add a new site for customer: " + node.Text);
                    btnDeleteSelected.Text = "Delete Customer";
					btnEditSelected.Text = "Edit Customer";
					btnToggleStatusSelected.Text = node.IsActive ? "Inactivate Customer" : "Activate Customer";
					break;
				case EntityType.Site:
					btnAddSelected.Text = "Add Unit";
                    toolTip1.SetToolTip(this.btnAddSelected, "Add a new unit for site: " + node.Text);
                    btnDeleteSelected.Text = "Delete Site";
					btnEditSelected.Text = "Edit Site";
					btnToggleStatusSelected.Text = node.IsActive ? "Inactivate Site" : "Activate Site";
					break;
				case EntityType.Unit:
					btnAddSelected.Text = "Add Outage";
                    toolTip1.SetToolTip(this.btnAddSelected, "Add a new outage for unit: " + node.Text);
                    btnDeleteSelected.Text = "Delete Unit";
					btnEditSelected.Text = "Edit Unit";
					btnToggleStatusSelected.Text = node.IsActive ? "Inactivate Unit" : "Activate Unit";
					break;
				case EntityType.Outage:
					btnAddSelected.Visible = false;
                    toolTip1.SetToolTip(this.btnAddSelected, "");
                    btnToggleStatusSelected.Visible = false;
					btnDeleteSelected.Text = "Delete Outage";
					btnEditSelected.Text = "Edit Outage";
					btnToggleStatusSelected.Text = node.IsActive ? "Inactivate Outage" : "Activate Outage";
					break;
			}
		}

		// Re-sort the tree view, make sure that the supplied node is visible,
		// and that the supplied node and the treeview itself are selected.
		private void sortAndDisplay(TreeNodeEntity node)
		{
			TreeNodeEntity tempNode = node;
			if (tvwCustomerConfig.Nodes.Count == 0) return;
			if (!tvwCustomerConfig.Nodes.Contains(tempNode)) tempNode = (TreeNodeEntity)tvwCustomerConfig.TopNode;
			tvwCustomerConfig.Sort();
			node.EnsureVisible();
			tvwCustomerConfig.SelectedNode = node;
			tvwCustomerConfig.Select();
		}

		private ImageIdx ActiveImageIdx(EntityType type)
		{
			switch (type)
			{
				case EntityType.Customer:
					return ImageIdx.CustomerActive;
				case EntityType.Site:
					return ImageIdx.SiteActive;
				case EntityType.Unit:
					return ImageIdx.UnitActive;
				case EntityType.System:
					return ImageIdx.SystemActive;
				case EntityType.Line:
					return ImageIdx.LineActive;
				case EntityType.Outage:
					return ImageIdx.Outage;
				default:
					throw new Exception("Unexpected entity type");
			}
		}
		private ImageIdx InactiveImageIdx(EntityType type)
		{
			switch (type)
			{
				case EntityType.Customer:
					return ImageIdx.CustomerInactive;
				case EntityType.Site:
					return ImageIdx.SiteInactive;
				case EntityType.Unit:
					return ImageIdx.UnitInactive;
				case EntityType.System:
					return ImageIdx.SystemInactive;
				case EntityType.Line:
					return ImageIdx.LineInactive;
				case EntityType.Outage:
					return ImageIdx.Outage;
				default:
					throw new Exception("Unexpected entity type");
			}
		}

		//------------------------------------------------------------------------
		// Private variables
		//------------------------------------------------------------------------

		private TreeNodeEntityCollection inactiveNodes;

		//------------------------------------------------------------------------
		// Private constants and enumerations
		//------------------------------------------------------------------------
		// Types of entities displayed in this form
		private enum EntityType
		{
			Customer,
			Site,
			Unit,
			System,
			Line,
			Outage
		}
		// Image Indices.  The order of items in this enum must correspond to
		// the order of the images in the image list for the treeview
		private enum ImageIdx
		{
			CustomerActive,
			SiteActive,
			UnitActive,
			SystemActive,
			LineActive,
			CustomerInactive,
			SiteInactive,
			UnitInactive,
			SystemInactive,
			LineInactive,
			Outage
		}

	}

	// Create a node sorter that implements the IComparer interface.
	public class NodeSorter : IComparer
	{
		 // Sort first by entity type, then by Text (i.e. entity name)
		 public int Compare(Object x, Object y)
		 {
			 TreeNodeEntity tx = x as TreeNodeEntity;
			 TreeNodeEntity ty = y as TreeNodeEntity;

			 if (tx.EntityType == ty.EntityType)
				 return string.Compare(tx.Text, ty.Text);
			 else
				 return tx.EntityType - ty.EntityType;
		 }
	}
}