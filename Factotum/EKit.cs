using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EKit : IEntity
	{
		public static event EventHandler<EntityChangedEventArgs> Changed;

		protected virtual void OnChanged(Guid? ID)
		{
			// Copy to a temporary variable to be thread-safe.
			EventHandler<EntityChangedEventArgs> temp = Changed;
			if (temp != null)
				temp(this, new EntityChangedEventArgs(ID));
		}
		// Mapped database columns
		// Use Guid?s for Primary Keys and foreign keys (whether they're nullable or not).
		// Use int?, decimal?, etc for numbers (whether they're nullable or not).
		// Strings, images, etc, are reference types already
		private Guid? KitDBid;
		private string KitName;

		// Textbox limits
		public static int KitNameCharLimit = 10;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string KitNameErrMsg;

		// Form level validation message
		private string KitErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return KitDBid; }
		}

		public string ToolKitName
		{
			get { return KitName; }
			set { KitName = Util.NullifyEmpty(value); }
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string ToolKitNameErrMsg
		{
			get { return KitNameErrMsg; }
		}

		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string ToolKitErrMsg
		{
			get { return KitErrMsg; }
			set { KitErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool ToolKitNameLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > KitNameCharLimit)
			{
				KitNameErrMsg = string.Format("Tool Kit Names cannot exceed {0} characters", KitNameCharLimit);
				return false;
			}
			else
			{
				KitNameErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		public bool ToolKitNameValid(string value)
		{
			if (!ToolKitNameLengthOk(value)) return false;

			if (NameExists(value, KitDBid))
			{
				KitNameErrMsg = "That Tool Kit Name is already in use.";
				return false;
			}
			KitNameErrMsg = null;
			return true;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EKit()
		{
			
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EKit(Guid? id) : this()
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
			if (id == null)
			{
				KitName = GetUniqueKitName();
				return;
			}

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			SqlCeDataReader dr;
			cmd.CommandType = CommandType.Text;
			cmd.CommandText = 
				@"Select 
				KitDBid,
				KitName
				from Kits
				where KitDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For all nullable values, replace dbNull with null
				KitDBid = (Guid?)dr[0];
				KitName = (string)dr[1];
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
				KitDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", KitDBid),
					new SqlCeParameter("@p1", KitName)
					});
				cmd.CommandText = @"Insert Into Kits (
					KitDBid,
					KitName
				) values (@p0,@p1)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Tool Kits row");
				}
			}
			else
			{
				// we are updating an existing record
				
				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", KitDBid),
					new SqlCeParameter("@p1", KitName)});

				cmd.CommandText =
					@"Update Kits 
					set					
					KitName = @p1
					Where KitDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update tool kits row");
				}
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
			if (!ToolKitNameValid(KitName)) return false;

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
			if (KitDBid == null)
			{
				KitErrMsg = "Unable to delete. Record not found.";
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
					@"Delete from Kits 
					where KitDBid = @p0";
				cmd.Parameters.Add("@p0", KitDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					KitErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					KitErrMsg = null;
					OnChanged(ID);
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of kits
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EKitCollection ListByName(bool addNoSelection)
		{
			EKit kit;
			EKitCollection kits = new EKitCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				KitDBid,
				KitName
				from Kits";

			qry += "	order by KitName";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				kit = new EKit();
				kit.ToolKitName = "<No Selection>";
				kits.Add(kit);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				kit = new EKit((Guid?)dr[0]);
				kit.KitName = (string)(dr[1]);

				kits.Add(kit);	
			}
			// Finish up
			dr.Close();
			return kits;
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

			// Delete the temp tables if any exist.  They shouldn't under normal circumstances.
			Globals.DeleteTempTableIfExists("tmpIns");
			Globals.DeleteTempTableIfExists("tmpDcr");
			Globals.DeleteTempTableIfExists("tmpMtr");
			Globals.DeleteTempTableIfExists("tmpCbk");
			Globals.DeleteTempTableIfExists("tmpThm");

			// Changing the booleans to 'Yes' and 'No' eliminates the silly checkboxes and
			// makes the column sortable.
			// You'll likely want to modify this query further, joining in other tables, etc.
			string qry = "Create table tmpIns (id uniqueidentifier not null,ct int not null)";
			cmd.CommandText = qry;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			cmd.ExecuteNonQuery();
			qry = "Create table tmpDcr (id uniqueidentifier not null, ct int not null)";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();
			qry = "Create table tmpMtr (id uniqueidentifier not null, ct int not null)";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();
			qry = "Create table tmpCbk (id uniqueidentifier not null, ct int not null)";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();
			qry = "Create table tmpThm (id uniqueidentifier not null, ct int not null)";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();

			cmd = Globals.cnn.CreateCommand();
			qry =
				@"insert into tmpIns (id, ct)
				Select KitDBid, Count(InsDBid) from Kits
				left outer join Inspectors on KitDBid = InsKitID
				group by KitDbid";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();
			qry =
				@"insert into tmpDcr (id, ct)
				Select KitDBid, Count(DcrDBid) from Kits
				left outer join Ducers on KitDBid = DcrKitID
				group by KitDbid";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();
			qry =
				@"insert into tmpMtr (id, ct)
				Select KitDBid, Count(MtrDBid) from Kits
				left outer join Meters on KitDBid = MtrKitID
				group by KitDbid";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();
			qry =
				@"insert into tmpCbk (id, ct)
				Select KitDBid, Count(CbkDBid) from Kits
				left outer join CalBlocks on KitDBid = CbkKitID
				group by KitDbid";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();
			qry =
				@"insert into tmpThm (id, ct)
				Select KitDBid, Count(ThmDBid) from Kits
				left outer join Thermos on KitDBid = ThmKitID
				group by KitDbid;";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();

			cmd = Globals.cnn.CreateCommand();
			qry =
				@"select distinct
				KitDBid as ID, 
				KitName, 
				tmpIns.ct as KitInspectorCt, 
				tmpMtr.ct as KitMeterCt, 
				tmpDcr.ct as KitDucerCt, 
				tmpCbk.ct as KitCalBlockCt, 
				tmpThm.ct as KitThermoCt
				from Kits 
				inner join tmpIns on KitDBid = tmpIns.id
				inner join tmpMtr on KitDBid = tmpMtr.id 
				inner join tmpDcr on KitDBid = tmpDcr.id 
				inner join tmpCbk on KitDBid = tmpCbk.id 
				inner join tmpThm on KitDBid = tmpThm.id 
				order by KitName;";

			cmd.CommandText = qry;
			da.SelectCommand = cmd;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			da.Fill(ds);
			dv = new DataView(ds.Tables[0]);

			cmd = Globals.cnn.CreateCommand();
			qry = "drop table tmpIns";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();
			qry = "drop table tmpMtr";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();
			qry = "drop table tmpDcr";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();
			qry = "drop table tmpCbk";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();
			qry = "drop table tmpThm";
			cmd.CommandText = qry;
			cmd.ExecuteNonQuery();
			return dv;
		}
		//--------------------------------------
		// Private utilities
		//--------------------------------------

		// Check if the name exists for any records besides the current one
		// This is used to show an error when the user tabs away from the field.  
		// We don't want to show an error if the user has left the field blank.
		// If it's a required field, we'll catch it when the user hits save.
		private bool NameExists(string name, Guid? id)
		{
			if (Util.IsNullOrEmpty(name)) return false;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;

			cmd.Parameters.Add(new SqlCeParameter("@p1", name));
			if (id == null)
			{
				cmd.CommandText = "Select KitDBid from Kits where KitName = @p1";
			}
			else
			{
				cmd.CommandText = "Select KitDBid from Kits where KitName = @p1 and KitDBid != @p0";
				cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			}
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			bool exists = (val != null);
			return exists;
		}


		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (KitName == null)
			{
				KitNameErrMsg = "A Tool Kit Name is required";
				allFilled = false;
			}
			else
			{
				KitNameErrMsg = null;
			}
			return allFilled;
		}
		private string GetUniqueKitName()
		{
			int i;
			SqlCeCommand cmd = Globals.cnn.CreateCommand();

			cmd.CommandText = "Select Count(KitDBid) from Kits";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			i = (int)cmd.ExecuteScalar() + 1; // Start with 'Kit 1'

			cmd = Globals.cnn.CreateCommand(); 
			cmd.Parameters.Add(new SqlCeParameter("@p1", "Kit " + i));
			cmd.CommandText = "Select KitName from Kits where KitName = @p1";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object val = cmd.ExecuteScalar();
			while (val != null)
			{
				i++;
				cmd.Parameters["@p1"].Value = "Kit " + i;
				val = cmd.ExecuteScalar();
			}
			return "Kit " + i;
		}
	}

	//--------------------------------------
	// Kit Collection class
	//--------------------------------------
	public class EKitCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EKitCollection()
		{ }
		//the indexer of the collection
		public EKit this[int index]
		{
			get
			{
				return (EKit)this.List[index];
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
			foreach (EKit kit in InnerList)
			{
				if (kit.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EKit item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EKit item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EKit item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EKit item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
