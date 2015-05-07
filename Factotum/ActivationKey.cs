using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum
{
	class ActivationKey
	{

		public static string LoadActivationKey()
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Select SiteActivationKey from Globals";
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			object result = Util.NullForDbNull(cmd.ExecuteScalar());
			return (string)result;
		}

		public static void SaveActivationKey(string key)
		{
			SqlCeCommand cmd = Globals.cnn.CreateCommand();
			cmd.CommandText =
				@"Update Globals set SiteActivationKey = @p1";
			cmd.Parameters.Add("@p1", key);
			if (Globals.cnn.State != ConnectionState.Open) Globals.cnn.Open();
			int recordsModified = cmd.ExecuteNonQuery();
		}

		public static string CreateKey(DateTime startDate, int numberOfdays)
		{
			string last2OfYear = startDate.Year.ToString().Substring(2);
			string month = startDate.Month.ToString("00");
			string day = startDate.Day.ToString("00");
			string days = numberOfdays.ToString("000");
			int[] ikey = new int[9];
			StringBuilder sb = new StringBuilder(10);

			ikey[0] = int.Parse(days.Substring(0,1));
			ikey[1] = int.Parse(last2OfYear.Substring(1,1));
			ikey[2] = int.Parse(month.Substring(0,1));
			ikey[3] = int.Parse(day.Substring(1,1));
			ikey[4] = int.Parse(days.Substring(1,1));
			ikey[5] = int.Parse(last2OfYear.Substring(0,1));
			ikey[6] = int.Parse(month.Substring(1,1));
			ikey[7] = int.Parse(day.Substring(0,1));
			ikey[8] = int.Parse(days.Substring(2,1));

			Random r = new Random(DateTime.Now.Second);
			int shifty = r.Next(1,8);
			int idx;
			int val;
			for (int i = 0; i < 9; i++)
			{
				idx = (i + shifty) % 9; // shift but keep in range 0:8
				val = (ikey[idx] + shifty) % 10; // tweak the values
				sb.Append(val.ToString());
			}
			sb.Append(shifty);
			return sb.ToString();
		}
		public static bool parseKey(string key, out DateTime startDate, out int numberOfDays)
		{
			int[] ikey = new int[9];
			int idx;
			int val;
			startDate = DateTime.MinValue;
			numberOfDays = 0;
			if (key == null) return false;
			int shifty = int.Parse(key.Substring(9,1));
			for (int i = 0; i < 9; i++)
			{
				val = int.Parse(key.Substring(i, 1));
				val = (val + 10 - shifty) % 10;
				idx = (i + shifty) % 9;
				ikey[idx] = val;
			}

			numberOfDays = 100 * ikey[0] + 10 * ikey[4] + ikey[8];
			int year = 2000 + 10 * ikey[5] + ikey[1];
			int month = 10 * ikey[2] + ikey[6];
			int day = 10 * ikey[7] + ikey[3];
			try
			{
				startDate = new DateTime(year,month,day);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static bool IsKeyValid(string key, out bool isFuture)
		{
			DateTime keyDate;
			int days;
			parseKey(key, out keyDate, out days);
			isFuture = (keyDate > DateTime.Today);

			return (keyDate.AddDays(days) >= DateTime.Today && !isFuture);
		}

	}
}

