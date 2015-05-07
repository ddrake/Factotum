using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using RegUtility;

namespace Factotum
{
	class MasterRegistration
	{

		public static bool CheckMasterRegStatus(int timeout, out bool timedOut, 
			out bool gracePeriodEnded, out string message)
		{
			message = null;
			string key;
			gracePeriodEnded = false;
			timedOut = false;

			// See if a registration key exists.  If not, don't set.
			key = LicenceKey.ReadKeyFromRegistry();
			if (key == null)
			{
				message = "This machine is not registered as the System Master.";
				return false;
			}
			
			// If we've already verified today, don't bother checking everything
			if (Globals.MasterRegCheckedOn == DateTime.Today 
				&& Globals.UnverifiedSessionCount == 0)
				return true;

			// See if the registration key is right for the machine. If not, don't set.
			if (!LicenceKey.IsKeyValidForMachine(key, DowUtils.Util.GetMajorVersion()))
			{
				message = "This machine is not recognized as the System Master.\n" +
					"Major software revisions or certain hardware changes may make it necessary to re-register.";
				return false;
			}
			Globals.MasterRegistrationKey = key;

			if (IsBlacklisted(key, timeout, out timedOut, out gracePeriodEnded))
			{
				message = "This machine's System Master Registration has been revoked.";
				return false;
			}
			return true;
		}

		// This final step may be called by the Timer handler, if we needed to retry.
		// Check if a key is blacklisted.  If so, un-register it.
		// Also, if we were able to check and the key is ok, update MasterRegCheckedOn
		public static bool IsBlacklisted(string key, int timeout, 
			out bool timedOut, out bool gracePeriodEnded)
		{
			// localhost.MasterLicSvc svc = new localhost.MasterLicSvc();
            com.dowdrake.master_licence lic = new com.dowdrake.master_licence();
            com.dowdrake.ReturnStatus status = new com.dowdrake.ReturnStatus();

			bool blacklisted = false;
			timedOut = false;
			gracePeriodEnded = false;
			try
			{
                status = lic.is_blacklisted(key, Globals.BlacklistCheckRetries);
                blacklisted = status.returnval;
			}
			catch(Exception)
			{
				// regardless what the exception is, we timed out.
				timedOut = true;
			}
			if (blacklisted) LicenceKey.UnRegister();

			if (timedOut)
			{
				if (Globals.UnverifiedSessionCount >= Globals.maxUnverifiedSessions)
				{
					gracePeriodEnded = true;
					// Don't say blacklisted yet, but if this is still the case after all 
					// timeouts, the timer method in app main will un-register and close the app.
					return false;
				}
			}
			else
			{
				// Passed the last stage of verifying registration, so update MasterRegCheckedOn
				// and reset the Unverified Session count
				Globals.MasterRegCheckedOn = DateTime.Today;
				Globals.UnverifiedSessionCount = 0;
			}
			return blacklisted;
		}
	}
}
