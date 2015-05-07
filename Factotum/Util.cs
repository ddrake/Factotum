using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DowUtils
{
	public class Util
	{
		// Returns true if you pass a string that is null or empty i.e. zero length ("")
		public static bool IsNullOrEmpty(string s)
		{
            if (s != null) s = s.Trim();
            return (s == null || s.Length == 0);
		}

		// Returns null for empty strings
		public static string NullifyEmpty(string s)
		{
            if (s != null) s = s.Trim();
			if (s != null) return s.Length == 0 ? null : s;
			else return null;
		}

		// Returns DBNull for null objects or empty Guid?s
		public static object DbNullForNull(object item)
		{
			if (item is Guid?) return (Guid?)item == null ? DBNull.Value : item;
			else return item == null ? DBNull.Value : item;
		}

		// Returns null for DBNull 
		public static object NullForDbNull(object item)
		{
			return item == DBNull.Value ? null : item;
		}

		// Centers a control in a form, resizing the form if necessary to fit the control
		public static void CenterControlHorizInForm(Control ctrl, Form frm)
		{
			if (ctrl.Width + ctrl.Left + 20 > frm.Width)
			{
				frm.Size = new System.Drawing.Size(ctrl.Width + ctrl.Left + 20, frm.Height);
				if (ctrl.Width + ctrl.Left + 20 > frm.MinimumSize.Width)
				{
					frm.MinimumSize = new System.Drawing.Size(ctrl.Width + ctrl.Left + 20, frm.MinimumSize.Height);
				}
			}
			ctrl.Left = (frm.Width - ctrl.Width) / 2;
		}
		public static decimal? GetNullableDecimalForString(string text)
		{
            return IsNullOrEmpty(text) ? (decimal?)null : decimal.Parse(text);
		}

		public static short? GetNullableShortForString(string text)
		{
            return IsNullOrEmpty(text) ? (short?)null : short.Parse(text);
		}

		public static int? GetNullableIntForString(string text)
		{
            return IsNullOrEmpty(text) ? (int?)null : int.Parse(text);
		}

		public static float? GetNullableFloatForString(string text)
		{
            return IsNullOrEmpty(text) ? (float?)null : float.Parse(text);
		}

		public static int GetIntForString(string text)
		{
            return IsNullOrEmpty(text) ? 0 : int.Parse(text);
		}

		public static float GetFloatForString(string text)
		{
            return IsNullOrEmpty(text) ? 0 : float.Parse(text);
		}

		// Standard 3-decimal place format
		public static string GetFormattedDecimal(decimal? number)
		{
			return GetFormattedDecimal(number, 3);
		}

		// General-purpose format
		public static string GetFormattedDecimal(decimal? number, int places)
		{
			string fmt = "{0:0." + new string('0', places) + "}";
			return number == null ? null :
				string.Format(fmt, number);
		}

		// General-purpose format
		public static string GetFormattedFloat(float? number, int places)
		{
			string fmt = "{0:0." + new string('0', places) + "}";
			return number == null ? null :
				string.Format(fmt, number);
		}

		public static string GetFormattedShort(short? number)
		{
			return number == null ? null :
				string.Format("{0}", number);
		}

		// General-purpose format
		public static string GetFormattedFloat_NA(float? number, int places)
		{
			string fmt = "{0:0." + new string('0', places) + "}";
			return number == null ? "N/A" :
				string.Format(fmt, number);
		}

		// General-purpose format
		public static string GetFormattedDecimal_NA(decimal? number, int places)
		{
			string fmt = "{0:0." + new string('0', places) + "}";
			return number == null ? "N/A" :
				string.Format(fmt, number);
		}

		// Standard 3-decimal place format
		public static string GetFormattedDecimal_NA(decimal? number)
		{
			return GetFormattedDecimal_NA(number, 3);
		}

		public static string GetFormattedInt_NA(int? number)
		{
			return number == null ? "N/A" :
				string.Format("{0}", number);
		}

		public static string GetFormattedDecimal_Percent_NA(decimal? d)
		{
			if (d == null) return "N/A";
			return string.Format("{0:0.00}", d) + "%";
		}

		// Returns the version of this assembly -- The assembly of theproject that includes this file, 
		// not the assembly that is calling this function.
		public static Version GetAssemblyVersion()
		{
			return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		}

		public static string GetMajorVersion()
		{
			string ver = GetAssemblyVersion().ToString();
			return ver.Substring(0, ver.IndexOf("."));
		}

	}
}
