using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlServerCe;
using DowUtils;

namespace Factotum{

	class ChgComponent : ChangeFinder
	{
		public ChgComponent(SqlCeConnection cnnOrig, SqlCeConnection cnnMod)	: base(cnnOrig, cnnMod)
		{
			tableName = "Components";
			tableName_friendly = "Components";

			// Select Command for Modified Table
			cmdSelMod = cnnMod.CreateCommand();
			cmdSelMod.CommandType = CommandType.Text;
			cmdSelMod.CommandText = @"Select
					CmpDBid as ID,
					CmpName as Name,
					CmpUntID,
					CmtName as [Component Material Type],
					CmpCmtID,
					LinName as [Line],
					CmpLinID,
					SysName as [System],
					CmpSysID,
					CtpName as [Component Type],
					CmpCtpID,
					PslSchedule + ' Nom Dia: ' + STR(PslNomDia,6,3) as [Schedule],
					CmpPslID,
					CmpDrawing as [Drawing],
					CmpUpMainOd as [U/S Main OD],
					CmpUpMainTnom as [U/S Main Tnom],
					CmpUpMainTscr as [U/S Main Tscr],
					CmpDnMainOd as [D/S Main OD],
					CmpDnMainTnom as [D/S Main Tnom],
					CmpDnMainTscr as [D/S Main Tscr],
					CmpBranchOd as [Branch OD],
					CmpBranchTnom as [Branch Tnom],
					CmpBranchTscr as [Branch Tscr],
					CmpUpExtOd as [U/S Ext OD],
					CmpUpExtTnom as [U/S Ext Tnom],
					CmpUpExtTscr as [U/S Ext Tscr],
					CmpDnExtOd as [D/S Ext OD],
					CmpDnExtTnom as [D/S Ext Tnom],
					CmpDnExtTscr as [D/S Ext Tscr],
					CmpBrExtOd as [Branch Ext OD],
					CmpBrExtTnom as [Branch Ext Tnom],
					CmpBrExtTscr as [Branch Ext Tscr],
					CmpTimesInspected as [Times Inspected],
					CmpAvgInspectionTime as [Avg Inspection Time],
					CmpAvgCrewDose as [Avg Crew Dose],
					CmpPctChromeMain as [Pct Chromium Main],
					CmpPctChromeBranch as [Pct Chromium Branch],
					CmpPctChromeUsExt as [Pct Chromium U/S Ext],
					CmpPctChromeDsExt as [Pct Chromium D/S Ext],
					CmpPctChromeBrExt as [Pct Chromium Branch Ext],
					CmpMisc1 as [Miscellaneous 1],
					CmpMisc2 as [Miscellaneous 2],
					CmpNote as [Note],
					CASE
						WHEN CmpHighRad = 1 THEN 'Yes'
						WHEN CmpHighRad = 0 THEN 'No'
					END as [High Rad],
					CmpHighRad,
					CASE
						WHEN CmpHardToAccess = 1 THEN 'Yes'
						WHEN CmpHardToAccess = 0 THEN 'No'
					END as [Hard To Access],
					CmpHardToAccess,
					CASE
						WHEN CmpHasDs = 1 THEN 'Yes'
						WHEN CmpHasDs = 0 THEN 'No'
					END as [Has Downstream],
					CmpHasDs,
					CASE
						WHEN CmpHasBranch = 1 THEN 'Yes'
						WHEN CmpHasBranch = 0 THEN 'No'
					END as [Has Branch],
					CmpHasBranch,
					CmpIsLclChg,
					CmpUsedInOutage,
					CASE
						WHEN CmpIsActive = 1 THEN 'Active'
						WHEN CmpIsActive = 0 THEN 'Inactive'
					END as [Status],
					CmpIsActive
					from Components
					left outer join ComponentMaterials on CmpCmtID = CmtDBid
					left outer join Lines on CmpLinID = LinDBid
					left outer join Systems on CmpSysID = SysDBid
					left outer join ComponentTypes on CmpCtpID = CtpDBid
					left outer join PipeScheduleLookup on CmpPslID = PslDBid";

			// Select Command for Original Table
			cmdSelOrig = cnnOrig.CreateCommand();
			cmdSelOrig.CommandType = CommandType.Text;
			cmdSelOrig.CommandText = @"Select
					CmpDBid as ID,
					CmpName as Name,
					CmpUntID,
					CmtName as [Component Material Type],
					CmpCmtID,
					LinName as [Line],
					CmpLinID,
					SysName as [System],
					CmpSysID,
					CtpName as [Component Type],
					CmpCtpID,
					PslSchedule + ' Nom Dia: ' + STR(PslNomDia,6,3) as [Schedule],
					CmpPslID,
					CmpDrawing as [Drawing],
					CmpUpMainOd as [U/S Main OD],
					CmpUpMainTnom as [U/S Main Tnom],
					CmpUpMainTscr as [U/S Main Tscr],
					CmpDnMainOd as [D/S Main OD],
					CmpDnMainTnom as [D/S Main Tnom],
					CmpDnMainTscr as [D/S Main Tscr],
					CmpBranchOd as [Branch OD],
					CmpBranchTnom as [Branch Tnom],
					CmpBranchTscr as [Branch Tscr],
					CmpUpExtOd as [U/S Ext OD],
					CmpUpExtTnom as [U/S Ext Tnom],
					CmpUpExtTscr as [U/S Ext Tscr],
					CmpDnExtOd as [D/S Ext OD],
					CmpDnExtTnom as [D/S Ext Tnom],
					CmpDnExtTscr as [D/S Ext Tscr],
					CmpBrExtOd as [Branch Ext OD],
					CmpBrExtTnom as [Branch Ext Tnom],
					CmpBrExtTscr as [Branch Ext Tscr],
					CmpTimesInspected as [Times Inspected],
					CmpAvgInspectionTime as [Avg Inspection Time],
					CmpAvgCrewDose as [Avg Crew Dose],
					CmpPctChromeMain as [Pct Chromium Main],
					CmpPctChromeBranch as [Pct Chromium Branch],
					CmpPctChromeUsExt as [Pct Chromium U/S Ext],
					CmpPctChromeDsExt as [Pct Chromium D/S Ext],
					CmpPctChromeBrExt as [Pct Chromium Branch Ext],
					CmpMisc1 as [Miscellaneous 1],
					CmpMisc2 as [Miscellaneous 2],
					CmpNote as [Note],
					CASE
						WHEN CmpHighRad = 1 THEN 'Yes'
						WHEN CmpHighRad = 0 THEN 'No'
					END as [High Rad],
					CmpHighRad,
					CASE
						WHEN CmpHardToAccess = 1 THEN 'Yes'
						WHEN CmpHardToAccess = 0 THEN 'No'
					END as [Hard To Access],
					CmpHardToAccess,
					CASE
						WHEN CmpHasDs = 1 THEN 'Yes'
						WHEN CmpHasDs = 0 THEN 'No'
					END as [Has Downstream],
					CmpHasDs,
					CASE
						WHEN CmpHasBranch = 1 THEN 'Yes'
						WHEN CmpHasBranch = 0 THEN 'No'
					END as [Has Branch],
					CmpHasBranch,
					CmpIsLclChg,
					CmpUsedInOutage,
					CASE
						WHEN CmpIsActive = 1 THEN 'Active'
						WHEN CmpIsActive = 0 THEN 'Inactive'
					END as [Status],
					CmpIsActive
					from Components
					left outer join ComponentMaterials on CmpCmtID = CmtDBid
					left outer join Lines on CmpLinID = LinDBid
					left outer join Systems on CmpSysID = SysDBid
					left outer join ComponentTypes on CmpCtpID = CtpDBid
					left outer join PipeScheduleLookup on CmpPslID = PslDBid";

			// Update Command for Original Table
			cmdUpdOrig = cnnOrig.CreateCommand();
			cmdUpdOrig.CommandType = CommandType.Text;
			cmdUpdOrig.CommandText = @"Update Components set					
					CmpName = @p1,					
					CmpUntID = @p2,					
					CmpCmtID = @p3,					
					CmpLinID = @p4,					
					CmpSysID = @p5,					
					CmpCtpID = @p6,					
					CmpPslID = @p7,					
					CmpDrawing = @p8,					
					CmpUpMainOd = @p9,					
					CmpUpMainTnom = @p10,					
					CmpUpMainTscr = @p11,					
					CmpDnMainOd = @p12,					
					CmpDnMainTnom = @p13,					
					CmpDnMainTscr = @p14,					
					CmpBranchOd = @p15,					
					CmpBranchTnom = @p16,					
					CmpBranchTscr = @p17,					
					CmpUpExtOd = @p18,					
					CmpUpExtTnom = @p19,					
					CmpUpExtTscr = @p20,					
					CmpDnExtOd = @p21,					
					CmpDnExtTnom = @p22,					
					CmpDnExtTscr = @p23,					
					CmpBrExtOd = @p24,					
					CmpBrExtTnom = @p25,					
					CmpBrExtTscr = @p26,					
					CmpTimesInspected = @p27,					
					CmpAvgInspectionTime = @p28,					
					CmpAvgCrewDose = @p29,					
					CmpPctChromeMain = @p30,					
					CmpPctChromeBranch = @p31,					
					CmpPctChromeUsExt = @p32,					
					CmpPctChromeDsExt = @p33,					
					CmpPctChromeBrExt = @p34,					
					CmpMisc1 = @p35,					
					CmpMisc2 = @p36,					
					CmpNote = @p37,					
					CmpHighRad = @p38,					
					CmpHardToAccess = @p39,					
					CmpHasDs = @p40,					
					CmpHasBranch = @p41,					
					CmpIsLclChg = @p42,					
					CmpUsedInOutage = @p43,					
					CmpIsActive = @p44
					Where CmpDBid = @p0";
					
			cmdUpdOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdUpdOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdUpdOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "CmpUntID");					
			cmdUpdOrig.Parameters.Add("@p3", SqlDbType.UniqueIdentifier, 16, "CmpCmtID");					
			cmdUpdOrig.Parameters.Add("@p4", SqlDbType.UniqueIdentifier, 16, "CmpLinID");					
			cmdUpdOrig.Parameters.Add("@p5", SqlDbType.UniqueIdentifier, 16, "CmpSysID");					
			cmdUpdOrig.Parameters.Add("@p6", SqlDbType.UniqueIdentifier, 16, "CmpCtpID");					
			cmdUpdOrig.Parameters.Add("@p7", SqlDbType.UniqueIdentifier, 16, "CmpPslID");
			cmdUpdOrig.Parameters.Add("@p8", SqlDbType.NVarChar, 50, "Drawing");
			cmdUpdOrig.Parameters.Add("@p9", SqlDbType.Decimal, 6, "U/S Main OD");
			cmdUpdOrig.Parameters.Add("@p10", SqlDbType.Decimal, 5, "U/S Main Tnom");
			cmdUpdOrig.Parameters.Add("@p11", SqlDbType.Decimal, 5, "U/S Main Tscr");
			cmdUpdOrig.Parameters.Add("@p12", SqlDbType.Decimal, 6, "D/S Main OD");
			cmdUpdOrig.Parameters.Add("@p13", SqlDbType.Decimal, 5, "D/S Main Tnom");
			cmdUpdOrig.Parameters.Add("@p14", SqlDbType.Decimal, 5, "D/S Main Tscr");
			cmdUpdOrig.Parameters.Add("@p15", SqlDbType.Decimal, 6, "Branch OD");
			cmdUpdOrig.Parameters.Add("@p16", SqlDbType.Decimal, 5, "Branch Tnom");
			cmdUpdOrig.Parameters.Add("@p17", SqlDbType.Decimal, 5, "Branch Tscr");
			cmdUpdOrig.Parameters.Add("@p18", SqlDbType.Decimal, 6, "U/S Ext OD");
			cmdUpdOrig.Parameters.Add("@p19", SqlDbType.Decimal, 5, "U/S Ext Tnom");
			cmdUpdOrig.Parameters.Add("@p20", SqlDbType.Decimal, 5, "U/S Ext Tscr");
			cmdUpdOrig.Parameters.Add("@p21", SqlDbType.Decimal, 6, "D/S Ext OD");
			cmdUpdOrig.Parameters.Add("@p22", SqlDbType.Decimal, 5, "D/S Ext Tnom");
			cmdUpdOrig.Parameters.Add("@p23", SqlDbType.Decimal, 5, "D/S Ext Tscr");
			cmdUpdOrig.Parameters.Add("@p24", SqlDbType.Decimal, 6, "Branch Ext OD");
			cmdUpdOrig.Parameters.Add("@p25", SqlDbType.Decimal, 5, "Branch Ext Tnom");
			cmdUpdOrig.Parameters.Add("@p26", SqlDbType.Decimal, 5, "Branch Ext Tscr");
			cmdUpdOrig.Parameters.Add("@p27", SqlDbType.Int, 4, "Times Inspected");
			cmdUpdOrig.Parameters.Add("@p28", SqlDbType.Float, 8, "Avg Inspection Time");
			cmdUpdOrig.Parameters.Add("@p29", SqlDbType.Float, 8, "Avg Crew Dose");
			cmdUpdOrig.Parameters.Add("@p30", SqlDbType.Decimal, 5, "Pct Chromium Main");
			cmdUpdOrig.Parameters.Add("@p31", SqlDbType.Decimal, 5, "Pct Chromium Branch");
			cmdUpdOrig.Parameters.Add("@p32", SqlDbType.Decimal, 5, "Pct Chromium U/S Ext");
			cmdUpdOrig.Parameters.Add("@p33", SqlDbType.Decimal, 5, "Pct Chromium D/S Ext");
			cmdUpdOrig.Parameters.Add("@p34", SqlDbType.Decimal, 5, "Pct Chromium Branch Ext");
			cmdUpdOrig.Parameters.Add("@p35", SqlDbType.NVarChar, 25, "Miscellaneous 1");
			cmdUpdOrig.Parameters.Add("@p36", SqlDbType.NVarChar, 25, "Miscellaneous 2");
			cmdUpdOrig.Parameters.Add("@p37", SqlDbType.NVarChar, 256, "Note");					
			cmdUpdOrig.Parameters.Add("@p38", SqlDbType.Bit, 1, "CmpHighRad");					
			cmdUpdOrig.Parameters.Add("@p39", SqlDbType.Bit, 1, "CmpHardToAccess");					
			cmdUpdOrig.Parameters.Add("@p40", SqlDbType.Bit, 1, "CmpHasDs");					
			cmdUpdOrig.Parameters.Add("@p41", SqlDbType.Bit, 1, "CmpHasBranch");					
			cmdUpdOrig.Parameters.Add("@p42", SqlDbType.Bit, 1, "CmpIsLclChg");					
			cmdUpdOrig.Parameters.Add("@p43", SqlDbType.Bit, 1, "CmpUsedInOutage");					
			cmdUpdOrig.Parameters.Add("@p44", SqlDbType.Bit, 1, "CmpIsActive");					
			cmdUpdOrig.Parameters["@p0"].SourceVersion = DataRowVersion.Original;					

			// Insert Command for Original Table
			cmdInsOrig = cnnOrig.CreateCommand();
			cmdInsOrig.CommandType = CommandType.Text;
			cmdInsOrig.CommandText = @"Insert into Components (					
				CmpDBid,					
				CmpName,					
				CmpUntID,					
				CmpCmtID,					
				CmpLinID,					
				CmpSysID,					
				CmpCtpID,					
				CmpPslID,					
				CmpDrawing,					
				CmpUpMainOd,					
				CmpUpMainTnom,					
				CmpUpMainTscr,					
				CmpDnMainOd,					
				CmpDnMainTnom,					
				CmpDnMainTscr,					
				CmpBranchOd,					
				CmpBranchTnom,					
				CmpBranchTscr,					
				CmpUpExtOd,					
				CmpUpExtTnom,					
				CmpUpExtTscr,					
				CmpDnExtOd,					
				CmpDnExtTnom,					
				CmpDnExtTscr,					
				CmpBrExtOd,					
				CmpBrExtTnom,					
				CmpBrExtTscr,					
				CmpTimesInspected,					
				CmpAvgInspectionTime,					
				CmpAvgCrewDose,					
				CmpPctChromeMain,					
				CmpPctChromeBranch,					
				CmpPctChromeUsExt,					
				CmpPctChromeDsExt,					
				CmpPctChromeBrExt,					
				CmpMisc1,					
				CmpMisc2,					
				CmpNote,					
				CmpHighRad,					
				CmpHardToAccess,					
				CmpHasDs,					
				CmpHasBranch,					
				CmpIsLclChg,					
				CmpUsedInOutage,					
				CmpIsActive				
				) values (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, @p14, @p15, @p16, @p17, @p18, @p19, @p20, @p21, @p22, @p23, @p24, @p25, @p26, @p27, @p28, @p29, @p30, @p31, @p32, @p33, @p34, @p35, @p36, @p37, @p38, @p39, @p40, @p41, @p42, @p43, @p44)";
					
			cmdInsOrig.Parameters.Add("@p0", SqlDbType.UniqueIdentifier, 16, "ID");					
			cmdInsOrig.Parameters.Add("@p1", SqlDbType.NVarChar, 50, "Name");					
			cmdInsOrig.Parameters.Add("@p2", SqlDbType.UniqueIdentifier, 16, "CmpUntID");					
			cmdInsOrig.Parameters.Add("@p3", SqlDbType.UniqueIdentifier, 16, "CmpCmtID");					
			cmdInsOrig.Parameters.Add("@p4", SqlDbType.UniqueIdentifier, 16, "CmpLinID");					
			cmdInsOrig.Parameters.Add("@p5", SqlDbType.UniqueIdentifier, 16, "CmpSysID");					
			cmdInsOrig.Parameters.Add("@p6", SqlDbType.UniqueIdentifier, 16, "CmpCtpID");					
			cmdInsOrig.Parameters.Add("@p7", SqlDbType.UniqueIdentifier, 16, "CmpPslID");
			cmdInsOrig.Parameters.Add("@p8", SqlDbType.NVarChar, 50, "Drawing");
			cmdInsOrig.Parameters.Add("@p9", SqlDbType.Decimal, 6, "U/S Main OD");
			cmdInsOrig.Parameters.Add("@p10", SqlDbType.Decimal, 5, "U/S Main Tnom");
			cmdInsOrig.Parameters.Add("@p11", SqlDbType.Decimal, 5, "U/S Main Tscr");
			cmdInsOrig.Parameters.Add("@p12", SqlDbType.Decimal, 6, "D/S Main OD");
			cmdInsOrig.Parameters.Add("@p13", SqlDbType.Decimal, 5, "D/S Main Tnom");
			cmdInsOrig.Parameters.Add("@p14", SqlDbType.Decimal, 5, "D/S Main Tscr");
			cmdInsOrig.Parameters.Add("@p15", SqlDbType.Decimal, 6, "Branch OD");
			cmdInsOrig.Parameters.Add("@p16", SqlDbType.Decimal, 5, "Branch Tnom");
			cmdInsOrig.Parameters.Add("@p17", SqlDbType.Decimal, 5, "Branch Tscr");
			cmdInsOrig.Parameters.Add("@p18", SqlDbType.Decimal, 6, "U/S Ext OD");
			cmdInsOrig.Parameters.Add("@p19", SqlDbType.Decimal, 5, "U/S Ext Tnom");
			cmdInsOrig.Parameters.Add("@p20", SqlDbType.Decimal, 5, "U/S Ext Tscr");
			cmdInsOrig.Parameters.Add("@p21", SqlDbType.Decimal, 6, "D/S Ext OD");
			cmdInsOrig.Parameters.Add("@p22", SqlDbType.Decimal, 5, "D/S Ext Tnom");
			cmdInsOrig.Parameters.Add("@p23", SqlDbType.Decimal, 5, "D/S Ext Tscr");
			cmdInsOrig.Parameters.Add("@p24", SqlDbType.Decimal, 6, "Branch Ext OD");
			cmdInsOrig.Parameters.Add("@p25", SqlDbType.Decimal, 5, "Branch Ext Tnom");
			cmdInsOrig.Parameters.Add("@p26", SqlDbType.Decimal, 5, "Branch Ext Tscr");
			cmdInsOrig.Parameters.Add("@p27", SqlDbType.Int, 4, "Times Inspected");
			cmdInsOrig.Parameters.Add("@p28", SqlDbType.Float, 8, "Avg Inspection Time");
			cmdInsOrig.Parameters.Add("@p29", SqlDbType.Float, 8, "Avg Crew Dose");
			cmdInsOrig.Parameters.Add("@p30", SqlDbType.Decimal, 5, "Pct Chromium Main");
			cmdInsOrig.Parameters.Add("@p31", SqlDbType.Decimal, 5, "Pct Chromium Branch");
			cmdInsOrig.Parameters.Add("@p32", SqlDbType.Decimal, 5, "Pct Chromium U/S Ext");
			cmdInsOrig.Parameters.Add("@p33", SqlDbType.Decimal, 5, "Pct Chromium D/S Ext");
			cmdInsOrig.Parameters.Add("@p34", SqlDbType.Decimal, 5, "Pct Chromium Branch Ext");
			cmdInsOrig.Parameters.Add("@p35", SqlDbType.NVarChar, 25, "Miscellaneous 1");
			cmdInsOrig.Parameters.Add("@p36", SqlDbType.NVarChar, 25, "Miscellaneous 2");
			cmdInsOrig.Parameters.Add("@p37", SqlDbType.NVarChar, 256, "Note");
			cmdInsOrig.Parameters.Add("@p38", SqlDbType.Bit, 1, "CmpHighRad");
			cmdInsOrig.Parameters.Add("@p39", SqlDbType.Bit, 1, "CmpHardToAccess");
			cmdInsOrig.Parameters.Add("@p40", SqlDbType.Bit, 1, "CmpHasDs");
			cmdInsOrig.Parameters.Add("@p41", SqlDbType.Bit, 1, "CmpHasBranch");
			cmdInsOrig.Parameters.Add("@p42", SqlDbType.Bit, 1, "CmpIsLclChg");
			cmdInsOrig.Parameters.Add("@p43", SqlDbType.Bit, 1, "CmpUsedInOutage");
			cmdInsOrig.Parameters.Add("@p44", SqlDbType.Bit, 1, "CmpIsActive");
		}
	}
}
