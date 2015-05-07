using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Factotum
{

	public class TreeNodeEntity : TreeNode
	{
		public TreeNodeEntity() : base()
		{
		}
		public TreeNodeEntity(string text) : base(text)
		{
		}
		public TreeNodeEntity(string text, TreeNodeEntity[] children)
			: base(text, children)
		{
		}
		public TreeNodeEntity(string text, int imageIndex, int selectedImageIndex)
			: base(text, imageIndex, selectedImageIndex)
		{
		}
		public TreeNodeEntity(string text, int imageIndex, int selectedImageIndex, TreeNodeEntity[] children)
			: base(text, imageIndex, selectedImageIndex, children)
		{
		}
		public TreeNodeEntity(string text, Guid? id, Guid? parentId, 
			int activeImageIndex, int inactiveImageIndex,
			bool isActive, int entityType)
			: base(text, 
				isActive ? activeImageIndex : inactiveImageIndex,
				isActive ? activeImageIndex : inactiveImageIndex)
		{
			this.entityName = text;
			this.id = id;
			this.parentId = parentId;
			this.entityType = entityType;
			this.activeImageIndex = activeImageIndex;
			this.inactiveImageIndex = inactiveImageIndex;
			IsActive = isActive;
			Name = id.ToString();

		}
		public Guid? ID
		{
			get { return id; }
			set { id = value; }
		}

		public Guid? ParentID
		{
			get { return parentId; }
			set { parentId = value; }
		}

		public bool IsActive
		{
			get { return isActive; }
			set 
			{ 
				isActive = value;
				SelectedImageIndex = isActive ? activeImageIndex : inactiveImageIndex;
				ImageIndex = isActive ? activeImageIndex : inactiveImageIndex;
				Text = isActive ? entityName : entityName + " (inactive)";
			}
		}
			private string entityName;

			public string EntityName
			{
				get { return entityName; }
				set 
				{ 
					entityName = value;
					Text = isActive ? entityName : entityName + " (inactive)";
				}
			}
	
		public int EntityType
		{
			get { return entityType; }
			set { entityType = value; }
		}
	
		private Guid? id;
		private Guid? parentId;
		private bool isActive;
		private int entityType;
		private int activeImageIndex;
		private int inactiveImageIndex;
		
	}



	//--------------------------------------
	// TreeNodeEntity Collection class
	//--------------------------------------
	public class TreeNodeEntityCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public TreeNodeEntityCollection()
		{ }
		//the indexer of the collection
		public TreeNodeEntity this[int index]
		{
			get
			{
				return (TreeNodeEntity)this.List[index];
			}
		}
		//this method fires the Changed event.
		protected virtual void OnChanged(EventArgs e)
		{
			if (Changed != null)
			{
				Changed(this, e);
			}
		}

		public bool ContainsName(Guid name)
		{
			foreach (TreeNodeEntity node in InnerList)
			{
				if (new Guid(node.Name) == name)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(TreeNodeEntity item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(TreeNodeEntity item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, TreeNodeEntity item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(TreeNodeEntity item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
