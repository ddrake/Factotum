using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class ECustomer : IEntity
	{
		public static event EventHandler<EntityChangedEventArgs> Changed;
		public static event EventHandler<EntityChangedEventArgs> Added;
		public static event EventHandler<EntityChangedEventArgs> Updated;
		public static event EventHandler<EntityChangedEventArgs> Deleted;

		protected virtual void OnChanged(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Changed;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}
		protected virtual void OnAdded(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Added;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}
		protected virtual void OnUpdated(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Updated;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}
		protected virtual void OnDeleted(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Deleted;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}
		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for nullable numbers
		// Strings, images, etc, are reference types already
		private Guid? CstDBid;
		private string CstName;
		private string CstFullName;
		private bool CstIsActive;

		// Textbox limits
		public static int CstNameCharLimit = 20;
		public static int CstFullNameCharLimit = 100;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string CstNameErrMsg;
		private string CstFullNameErrMsg;

		// Form level validation message
		private string CstErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return CstDBid; }
		}

		public string CustomerName
		{
			get { return CstName; }
			set { CstName = Util.NullifyEmpty(value); }
		}

		public string CustomerFullName
		{
			get { return CstFullName; }
			set { CstFullName = Util.NullifyEmpty(value); }
		}

		public bool CustomerIsActive
		{
			get { return CstIsActive; }
			set { CstIsActive = value; }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string CustomerNameErrMsg
		{
			get { return CstNameErrMsg; }
		}

		public string CustomerFullNameErrMsg
		{
			get { return CstFullNameErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string CustomerErrMsg
		{
			get { return CstErrMsg; }
			set { CstErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool CustomerNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CstNameCharLimit)
			{
				CstNameErrMsg = string.Format("Customer Names cannot exceed {0} characters", CstNameCharLimit);
				return false;
			}
			else
			{
				CstNameErrMsg = null;
				return true;
			}
		}

		public bool CustomerFullNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > CstFullNameCharLimit)
			{
				CstFullNameErrMsg = string.Format("Customer Full Names cannot exceed {0} characters", CstFullNameCharLimit);
				return false;
			}
			else
			{
				CstFullNameErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		
		public bool CustomerNameValid(string name)
		{
			if (!CustomerNameLengthOk(name)) return false;
			
			// KEEP, MODIFY OR REMOVE THIS AS REQUIRED
			// YOU MAY NEED THE NAME TO BE UNIQUE FOR A SPECIFIC PARENT, ETC..
			bool existingIsInactive;
			if (NameExists(name, CstDBid, out existingIsInactive))
			{
				CstNameErrMsg = existingIsInactive ? 
					"That Customer Name exists but its status has been set to inactive." :
					"That Customer Name is already in use.";
				 
				return false;
			}
			CstNameErrMsg = null;
			return true;
		}

		public bool CustomerFullNameValid(string value)
		{
			if (!CustomerFullNameLengthOk(value)) return false;

			CstFullNameErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public ECustomer()
		{
			this.CstIsActive = true;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public ECustomer(Guid? id) : this()
		{
			Load(id);
		}

		//--------------------------------------
		// Public Methods
		//--------------------------------------

		//----------------------------------------------------
		// Load the object from the database given a Guid?
		//----------------------------------------------------
		public void Load(Guid? id)
		{
			if (id == null) return;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			SqlCeDataReader dr;
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = 
				@"Select 
				CstDBid,
				CstName,
				CstFullName,
				CstIsActive
				from Customers
				where CstDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				CstDBid = (Guid?)dr[0];
				CstName = (string)dr[1];
				CstFullName = (string)dr[2];
				CstIsActive = (bool)dr[3];
			}
			dr.Close();
		}

		//--------------------------------------
		// Save the current record if it's valid
		//--------------------------------------
		public Guid? Save()
		{
			if (!Valid())
			{
				// Note: We're returning null if we fail,
				// so don't just assume you're going to get your id back 
				// and set your id to the result of this function call.
				return null;
			}

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			if (ID == null)
			{
				// we are inserting a new record

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				CstDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", CstDBid),
					new SqlCeParameter("@p1", CstName),
					new SqlCeParameter("@p2", CstFullName),
					new SqlCeParameter("@p3", CstIsActive)
					});
				cmd.CommandText = @"Insert Into Customers (
					CstDBid,
					CstName,
					CstFullName,
					CstIsActive
				) values (@p0,@p1,@p2,@p3)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Customers row");
				}
				OnAdded(ID);
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", CstDBid),
					new SqlCeParameter("@p1", CstName),
					new SqlCeParameter("@p2", CstFullName),
					new SqlCeParameter("@p3", CstIsActive)});

				cmd.CommandText =
					@"Update Customers 
					set					
					CstName = @p1,					
					CstFullName = @p2,					
					CstIsActive = @p3
					Where CstDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update customers row");
				}
				OnUpdated(ID);
			}
			OnChanged(ID); 
			return ID;
		}

		//--------------------------------------
		// Validate the current record
		//--------------------------------------
		// Make this public so that the UI can check validation itself
		// if it chooses to do so.  This is also called by the Save function.
		public bool Valid()
		{
			// First check each field to see if it's valid from the UI perspective
			if (!CustomerNameValid(CustomerName)) return false;
			if (!CustomerFullNameValid(CustomerFullName)) return false;

			// Check form to make sure all required fields have been filled in
			if (!RequiredFieldsFilled()) return false;

			// Check for incorrect field interactions...

			return true;
		}

		//--------------------------------------
		// Delete the current record
		//--------------------------------------
		public bool Delete(bool promptUser)
		{

			// If the current object doesn't reference a database record, there's nothing to do.
			if (CstDBid == null)
			{
				CustomerErrMsg = "Unable to delete. Record not found.";
				return false;
			}
			if (HasChildren())
			{
				CustomerErrMsg = "Unable to delete because Sites exist for this Customer.";
				return false;
			}
			DialogResult rslt = DialogResult.None;
			if (promptUser)
			{
				rslt = MessageBox.Show("Are you sure?", "Factotum: Deleting...",
					MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			}

			if (!promptUser || rslt == DialogResult.OK)
			{
				SqlCeCommand cmd = Globals.cnn.CreateCommand();
				cmd.CommandType = CommandType.Text;
				cmd.CommandText =
					@"Delete from Customers 
					where CstDBid = @p0";
				cmd.Parameters.Add("@p0", CstDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					CustomerErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					CustomerErrMsg = null;
					OnChanged(ID);
					OnDeleted(ID);
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		private bool HasChildren()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			cmd.CommandText =
				@"Select SitDBid from Sites 
					where SitCstID = @p0";
			cmd.Parameters.Add("@p0", CstDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			return result != null;
		}
		//--------------------------------------------------------------------
		// Static listing methods which return collections of customers
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static ECustomerCollection ListByName(
			bool showactive, bool showinactive, bool addNoSelection)
		{
			ECustomer customer;
			ECustomerCollection customers = new ECustomerCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				CstDBid,
				CstName,
				CstFullName,
				CstIsActive
				from Customers";
			if (showactive && !showinactive)
				qry += " where CstIsActive = 1";
			else if (!showactive && showinactive)
				qry += " where CstIsActive = 0";

			qry += "	order by CstName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				customer = new ECustomer();
				customer.CstName = "<No Selection>";
				customers.Add(customer);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				customer = new ECustomer((Guid?)dr[0]);
				customer.CstName = (string)(dr[1]);
				customer.CstFullName = (string)(dr[2]);
				customer.CstIsActive = (bool)(dr[3]);

				customers.Add(customer);	
			}
			// Finish up
			dr.Close();
			return customers;
		}

		// Get a Default data view with all columns that a user would likely want to see.
		// You can bind this view to a DataGridView, hide the columns you don't need, filter, etc.
		// I decided not to indicate inactive in the names of inactive items. The 'user'
		// can always show the inactive column if they wish.
		public static DataView GetDefaultDataView()
		{
			DataSet ds = new DataSet();
			DataView dv;
			SqlCeDataAdapter da = new SqlCeDataAdapter();
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			// Changing the booleans to 'Yes' and 'No' eliminates the silly checkboxes and
			// makes the column sortable.
			// You'll likely want to modify this query further, joining in other tables, etc.
			string qry = @"Select 
					CstDBid as ID,
					CstName as CustomerName,
					CstFullName as CustomerFullName,
					CASE
						WHEN CstIsActive = 0 THEN 'No'
						ELSE 'Yes'
					END as CustomerIsActive
					from Customers";
			cmd.CommandText = qry;
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);
			return dv;
		}
		//--------------------------------------
		// Private utilities
		//--------------------------------------

		// Check if the name exists for any records besides the current one
		// This is used to show an error when the user tabs away from the field.  
		// We don't want to show an error if the user has left the field blank.
		// If it's a required field, we'll catch it when the user hits save.
		private bool NameExists(string name, Guid? id, out bool existingIsInactive)
		{
			existingIsInactive = false;
			if (Util.IsNullOrEmpty(name)) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", name));
			if (id == null)
			{
				cmd.CommandText = "Select CstIsActive from Customers where CstName = @p1";
			}
			else
			{
				cmd.CommandText = "Select CstIsActive from Customers where CstName = @p1 and CstDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			if (exists) existingIsInactive = !(bool)val;  
			return exists;

		}


		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (CustomerName == null)
			{
				CstNameErrMsg = "A unique Customer Name is required";
				allFilled = false;
			}
			else
			{
				CstNameErrMsg = null;
			}
			if (CustomerFullName == null)
			{
				CstFullNameErrMsg = "A Customer Full Name is required";
				allFilled = false;
			}
			else
			{
				CstFullNameErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// Customer Collection class
	//--------------------------------------
	public class ECustomerCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public ECustomerCollection()
		{ }
		//the indexer of the collection
		public ECustomer this[int index]
		{
			get
			{
				return (ECustomer)this.List[index];
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

		public bool ContainsID(Guid? ID)
		{
			if (ID == null) return false;
			foreach (ECustomer customer in InnerList)
			{
				if (customer.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(ECustomer item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(ECustomer item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, ECustomer item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(ECustomer item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
