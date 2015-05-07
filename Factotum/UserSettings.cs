using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;


namespace Factotum
{
    public struct Settings
    {
        // put default values here.
        public string CurrentOutageID;
        public string VendorName;
        public string FieldDataSheetName;
        public int DbVersion;
        public bool IsPreviousMaster;
        public string FactotumDataFolder;
        public string DefaultImageFolder;
        public string MeterDataFolder;
        public bool ValidateEpriRecommended;
        public bool ValidateTemperatures;
        public bool ValidateNoUpMain;
        public bool ValidateNoUpExt;
        public bool ValidateNoDnExt;
        public bool ValidateGridColCountWithCeiling;
        public bool ValidateDsExtRowCountWithCeiling;
    }

    public static class UserSettings
    {
        public static Settings sets;

        public static void set_defaults()
        {
            sets.CurrentOutageID = "";
            sets.VendorName = "Westinghouse";
            sets.FieldDataSheetName = "Field Data Sheets.xls";
            sets.DbVersion = 7;
            sets.IsPreviousMaster = false;
            sets.FactotumDataFolder = "";
            sets.DefaultImageFolder = "";
            sets.MeterDataFolder = "";
            sets.ValidateEpriRecommended = true;
            sets.ValidateTemperatures = true;
            sets.ValidateNoUpMain = true;
            sets.ValidateNoUpExt = true;
            sets.ValidateNoDnExt = true;
            sets.ValidateGridColCountWithCeiling = true;
            sets.ValidateDsExtRowCountWithCeiling = true;
        }
        public static void Save()
        {
            using (StreamWriter sw = new StreamWriter(Globals.AppDataFolder + "\\settings.xml"))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Settings));
                xs.Serialize(sw, sets);
            }
         }

        public static void Load()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Globals.AppDataFolder + "\\settings.xml"))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Settings));
                    sets = (Settings)xs.Deserialize(sr);
                }
            }
            catch 
            {
                // if the file can't be read, use the defaults.
                set_defaults();
            }
        }       
    }

}
