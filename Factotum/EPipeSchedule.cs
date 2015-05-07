using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using System.Collections;
using System.Windows.Forms;
using DowUtils;

namespace Factotum{

	public class EPipeSchedule : IEntity
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
		private Guid? PslDBid;
		private decimal? PslOd;
		private decimal? PslNomWall;
		private string PslSchedule;
		private decimal? PslNomDia;
		private bool PslIsLclChg;

		// Textbox limits
		public static int PslOdCharLimit = 8;
		public static int PslNomWallCharLimit = 7;
		public static int PslScheduleCharLimit = 20;
		public static int PslNomDiaCharLimit = 7;
		
		// Field-specific error message strings (normally just needed for textbox data)
		private string PslOdErrMsg;
		private string PslNomWallErrMsg;
		private string PslScheduleErrMsg;
		private string PslNomDiaErrMsg;

		// Form level validation message
		private string PslErrMsg;

		
		//--------------------------------------------------------
		// Field Properties 
		//--------------------------------------------------------

		// Primary key accessor
		public Guid? ID
		{
			get { return PslDBid; }
		}

		public decimal? PipeScheduleOd
		{
			get { return PslOd; }
			set { PslOd = value; }
		}

		public decimal? PipeScheduleNomWall
		{
			get { return PslNomWall; }
			set { PslNomWall = value; }
		}

		public string PipeScheduleSchedule
		{
			get { return PslSchedule; }
			set { PslSchedule = Util.NullifyEmpty(value); }
		}

		public decimal? PipeScheduleNomDia
		{
			get { return PslNomDia; }
			set { PslNomDia = value; }
		}

		public bool PipeScheduleIsLclChg
		{
			get { return PslIsLclChg; }
			set { PslIsLclChg = value; }
		}

		// Set the property to null to set the default string
		// or set the property to a specific string.
		public string PipeScheduleAndNomDiaText
		{
			get 
			{
				string pipeScheduleAndNomDiaText;
				if (PslSchedule != null && PslNomDia != null)
				{
					pipeScheduleAndNomDiaText = string.Format("{0} - {1:0.000} in. Nom",
						new object[] { PslSchedule, PslNomDia });
				}
				else
				{
					pipeScheduleAndNomDiaText = "N/A";
				}
				return pipeScheduleAndNomDiaText;
			}
		}


		//-----------------------------------------------------------------
		// Field Level Error Messages.  
		// Include one for every text column
		// In cases where we need to ensure data consistency, we may need
		// them for other types.
		//-----------------------------------------------------------------

		public string PipeScheduleOdErrMsg
		{
			get { return PslOdErrMsg; }
		}

		public string PipeScheduleNomWallErrMsg
		{
			get { return PslNomWallErrMsg; }
		}

		public string PipeScheduleScheduleErrMsg
		{
			get { return PslScheduleErrMsg; }
		}

		public string PipeScheduleNomDiaErrMsg
		{
			get { return PslNomDiaErrMsg; }
		}


		//--------------------------------------
		// Form level Error Message
		//--------------------------------------

		public string PipeScheduleErrMsg
		{
			get { return PslErrMsg; }
			set { PslErrMsg = Util.NullifyEmpty(value); }
		}

		//--------------------------------------
		// Textbox Name Length Validation
		//--------------------------------------

		public bool PipeScheduleOdLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > PslOdCharLimit)
			{
				PslOdErrMsg = string.Format("Pipe Schedule OD's cannot exceed {0} characters", PslOdCharLimit);
				return false;
			}
			else
			{
				PslOdErrMsg = null;
				return true;
			}
		}

		public bool PipeScheduleNomWallLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > PslNomWallCharLimit)
			{
				PslNomWallErrMsg = string.Format("Pipe Schedule Nominal Wall thicknesses cannot exceed {0} characters", PslNomWallCharLimit);
				return false;
			}
			else
			{
				PslNomWallErrMsg = null;
				return true;
			}
		}

		public bool PipeScheduleScheduleLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > PslScheduleCharLimit)
			{
				PslScheduleErrMsg = string.Format("Pipe Schedules cannot exceed {0} characters", PslScheduleCharLimit);
				return false;
			}
			else
			{
				PslScheduleErrMsg = null;
				return true;
			}
		}

		public bool PipeScheduleNomDiaLengthOk(string s)
		{
			if (s == null) return true;
			if (s.Length > PslNomDiaCharLimit)
			{
				PslNomDiaErrMsg = string.Format("Pipe Schedule Nominal Diameters cannot exceed {0} characters", PslNomDiaCharLimit);
				return false;
			}
			else
			{
				PslNomDiaErrMsg = null;
				return true;
			}
		}

		//--------------------------------------
		// Field-Specific Validation
		// sets and clears error messages
		//--------------------------------------

		public bool PipeScheduleOdValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				PslOdErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0)
			{
				PslOdErrMsg = null;
				return true;
			}
			PslOdErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool PipeScheduleNomWallValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				PslNomWallErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0)
			{
				PslNomWallErrMsg = null;
				return true;
			}
			PslNomWallErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		public bool PipeScheduleScheduleValid(string value)
		{
			if (!PipeScheduleScheduleLengthOk(value)) return false;

			PslScheduleErrMsg = null;
			return true;
		}

		public bool PipeScheduleNomDiaValid(string value)
		{
			decimal result;
			if (Util.IsNullOrEmpty(value))
			{
				PslNomDiaErrMsg = null;
				return true;
			}
			if (decimal.TryParse(value, out result) && result > 0)
			{
				PslNomDiaErrMsg = null;
				return true;
			}
			PslNomDiaErrMsg = string.Format("Please enter a positive number");
			return false;
		}

		//--------------------------------------
		// Constructors
		//--------------------------------------
		
		// Default constructor.  Field defaults must be set here.
		// Any defaults set by the database will be overridden.
		public EPipeSchedule()
		{
			this.PslIsLclChg = false;
		}

		// Constructor which loads itself from the supplied id.
		// If the id is null, this gives the same result as using the default constructor.
		public EPipeSchedule(Guid? id) : this()
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
				PslDBid,
				PslOd,
				PslNomWall,
				PslSchedule,
				PslNomDia,
				PslIsLclChg
				from PipeScheduleLookup
				where PslDBid = @p0";
			cmd.Parameters.Add(new SqlCeParameter("@p0", id));
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();
			// The query should return one record.
			// If it doesn't return anything (no match) the object is not affected
			if (dr.Read())
			{
				// For nullable foreign keys, set field to null for dbNull case
				// For other nullable values, replace dbNull with null
				PslDBid = (Guid?)dr[0];
				PslOd = (decimal?)dr[1];
				PslNomWall = (decimal?)dr[2];
				PslSchedule = (string)dr[3];
				PslNomDia = (decimal?)dr[4];
				PslIsLclChg = (bool)dr[5];
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

				// If this is not a master db, set the local change flag to true.
				if (!Globals.IsMasterDB) PslIsLclChg = true;

				// first ask the database for a new Guid
				cmd.CommandText = "Select Newid()";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				PslDBid = (Guid?)(cmd.ExecuteScalar());

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", PslDBid),
					new SqlCeParameter("@p1", PslOd),
					new SqlCeParameter("@p2", PslNomWall),
					new SqlCeParameter("@p3", PslSchedule),
					new SqlCeParameter("@p4", PslNomDia),
					new SqlCeParameter("@p5", PslIsLclChg)
					});
				cmd.CommandText = @"Insert Into PipeScheduleLookup (
					PslDBid,
					PslOd,
					PslNomWall,
					PslSchedule,
					PslNomDia,
					PslIsLclChg
				) values (@p0,@p1,@p2,@p3,@p4,@p5)";
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to insert Pipe Schedule Lookup row");
				}
			}
			else
			{
				// we are updating an existing record

				// Replace any nulls with dbnull
				cmd.Parameters.AddRange(new SqlCeParameter[] {

					new SqlCeParameter("@p0", PslDBid),
					new SqlCeParameter("@p1", PslOd),
					new SqlCeParameter("@p2", PslNomWall),
					new SqlCeParameter("@p3", PslSchedule),
					new SqlCeParameter("@p4", PslNomDia),
					new SqlCeParameter("@p5", PslIsLclChg)});

				cmd.CommandText =
					@"Update PipeScheduleLookup 
					set					
					PslOd = @p1,					
					PslNomWall = @p2,					
					PslSchedule = @p3,					
					PslNomDia = @p4,					
					PslIsLclChg = @p5					
					Where PslDBid = @p0";

				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				if (cmd.ExecuteNonQuery() != 1)
				{
					throw new Exception("Unable to update pipe schedule lookup row");
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
			if (PslDBid == null)
			{
				PipeScheduleErrMsg = "Unable to delete. Record not found.";
				return false;
			}
			if (!PslIsLclChg && !Globals.IsMasterDB)
			{
				PipeScheduleErrMsg = "Unable to delete because this PipeSchedule was not added during this outage.";
				return false;
			}

			if (HasChildren())
			{
				PipeScheduleErrMsg = "Unable to delete because there are components that reference this pipe schedule.";
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
					@"Delete from PipeScheduleLookup 
					where PslDBid = @p0";
				cmd.Parameters.Add("@p0", PslDBid);
				if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
				int rowsAffected = cmd.ExecuteNonQuery();

				// Todo: figure out how I really want to do this.
				// Is there a problem with letting the database try to do cascading deletes?
				// How should the user be notified of the problem??
				if (rowsAffected < 1)
				{
					PipeScheduleErrMsg = "Unable to delete.  Please try again later.";
					return false;
				}
				else
				{
					OnChanged(ID);
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
				@"Select CmpDBid from Components
					where CmpPslID = @p0";
			cmd.Parameters.Add("@p0", PslDBid);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			bool hasChildren = false;
			object result = cmd.ExecuteScalar();
			if (result != null) hasChildren = true;
			return hasChildren;
		}

		//--------------------------------------------------------------------
		// Static listing methods which return collections of pipeschedules
		//--------------------------------------------------------------------

		// This helper function builds the collection for you based on the flags you send it
		// I originally had a flag that would let you indicate inactive items by appending '(inactive)'
		// to the name.  This was a bad idea, because sometimes the objects in this collection
		// will get modified and saved back to the database -- with the extra text appended to the name.
		public static EPipeScheduleCollection ListByScheduleAndNomDia(
			bool showactive, bool showinactive, bool addNoSelection)
		{
			EPipeSchedule pipeschedule;
			EPipeScheduleCollection pipeschedules = new EPipeScheduleCollection();

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandType = CommandType.Text;
			string qry = @"Select 

				PslDBid,
				PslOd,
				PslNomWall,
				PslSchedule,
				PslNomDia,
				PslIsLclChg
				from PipeScheduleLookup";

			qry += "	order by PslSchedule, PslNomDia";
			cmd.CommandText = qry;

			if (addNoSelection)
			{
				// Insert a default item with name "<No Selection>"
				pipeschedule = new EPipeSchedule();
				pipeschedules.Add(pipeschedule);
			}
			SqlCeDataReader dr;
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			dr = cmd.ExecuteReader();

			// Build new objects and add them to the collection
			while (dr.Read())
			{
				pipeschedule = new EPipeSchedule((Guid?)dr[0]);
				pipeschedule.PslOd = (decimal?)(dr[1]);
				pipeschedule.PslNomWall = (decimal?)(dr[2]);
				pipeschedule.PslSchedule = (string)(dr[3]);
				pipeschedule.PslNomDia = (decimal?)(dr[4]);
				pipeschedule.PslIsLclChg = (bool)(dr[5]);
				// The following sets the default text
				pipeschedules.Add(pipeschedule);	
			}
			// Finish up
			dr.Close();
			return pipeschedules;
		}

		public static EPipeSchedule FindForOdAndTnom(string sOd, string sTnom)
		{
			decimal od;
			decimal tNom;
			if (!Decimal.TryParse(sOd, out od) || !Decimal.TryParse(sTnom, out tNom))
				return null;

			return FindForOdAndTnom(od, tNom);
		}

		public static EPipeSchedule FindForOdAndTnom(decimal od, decimal tNom)
		{
			if (od <= 0 || od >= 1000 || tNom <= 0 || tNom >= 100) return null;

			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select PslDBid From PipeScheduleLookup
				Where PslOd >= @p1 and PslOd <= @p2
				and PslNomWall >=@p3 and PslNomWall <=@p4";

			cmd.Parameters.Add("@p1", od - 0.001m);
			cmd.Parameters.Add("@p2", od + 0.001m);
			cmd.Parameters.Add("@p3", tNom - 0.001m);
			cmd.Parameters.Add("@p4", tNom + 0.001m);

			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = cmd.ExecuteScalar();
			if (result == null) return null;

			EPipeSchedule ps = new EPipeSchedule((Guid?)result);
			return ps;
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
					PslDBid as ID,
					PslOd as PipeScheduleOd,
					PslNomWall as PipeScheduleNomWall,
					PslSchedule as PipeScheduleSchedule,
					PslNomDia as PipeScheduleNomDia,
					CASE
						WHEN PslIsLclChg = 0 THEN 'No'
						ELSE 'Yes'
					END as PipeScheduleIsLclChg
					from PipeScheduleLookup";
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


		// Check for required fields, setting the individual error messages
		private bool RequiredFieldsFilled()
		{
			bool allFilled = true;

			if (PipeScheduleOd == null)
			{
				PslOdErrMsg = "A Pipe Schedule Outside Diameter is required";
				allFilled = false;
			}
			else
			{
				PslOdErrMsg = null;
			}
			if (PipeScheduleNomWall == null)
			{
				PslNomWallErrMsg = "A Pipe Schedule Nominal Wall Thickness is required";
				allFilled = false;
			}
			else
			{
				PslNomWallErrMsg = null;
			}
			if (PipeScheduleSchedule == null)
			{
				PslScheduleErrMsg = "A Pipe Schedule Schedule is required";
				allFilled = false;
			}
			else
			{
				PslScheduleErrMsg = null;
			}
			if (PipeScheduleNomDia == null)
			{
				PslNomDiaErrMsg = "A Pipe Schedule Nominal Diameter is required";
				allFilled = false;
			}
			else
			{
				PslNomDiaErrMsg = null;
			}
			return allFilled;
		}
	}

	//--------------------------------------
	// PipeSchedule Collection class
	//--------------------------------------
	public class EPipeScheduleCollection : CollectionBase
	{
		//this event is fired when the collection's items have changed
		public event EventHandler Changed;
		//this is the constructor of the collection.
		public EPipeScheduleCollection()
		{ }
		//the indexer of the collection
		public EPipeSchedule this[int index]
		{
			get
			{
				return (EPipeSchedule)this.List[index];
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
			foreach (EPipeSchedule pipeschedule in InnerList)
			{
				if (pipeschedule.ID == ID)
					return true;
			}
			return false;
		}

		//returns the index of an item in the collection
		public int IndexOf(EPipeSchedule item)
		{
			return InnerList.IndexOf(item);
		}
		//adds an item to the collection
		public void Add(EPipeSchedule item)
		{
			this.List.Add(item);
			OnChanged(EventArgs.Empty);
		}
		//inserts an item in the collection at a specified index
		public void Insert(int index, EPipeSchedule item)
		{
			this.List.Insert(index, item);
			OnChanged(EventArgs.Empty);
		}
		//removes an item from the collection.
		public void Remove(EPipeSchedule item)
		{
			this.List.Remove(item);
			OnChanged(EventArgs.Empty);
		}
	}
}
