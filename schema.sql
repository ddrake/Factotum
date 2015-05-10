/*
Created		9/30/2007
Modified		5/10/2015
Project		Factotum FAC Reporting
Model			FAC
Company		Aged Man Consulting
Author		Dow Drake
Version		7
Database		MS SQL 2005 
*/


Create table [Sites]
(
	[SitDBid] Uniqueidentifier rowguidcol NOT NULL,
	[SitName] Nvarchar(20) NOT NULL,
	[SitCstID] Uniqueidentifier NOT NULL,
	[SitClpID] Uniqueidentifier NULL,
	[SitFullName] Nvarchar(100) NOT NULL,
	[SitIsActive] Bit Default 1 NOT NULL,
Primary Key ([SitDBid])
) 
go

Create table [Outages]
(
	[OtgDBid] Uniqueidentifier rowguidcol NOT NULL,
	[OtgName] Nvarchar(50) NOT NULL,
	[OtgUntID] Uniqueidentifier NOT NULL,
	[OtgClpID] Uniqueidentifier NULL,
	[OtgCptID] Uniqueidentifier NULL,
	[OtgCouplantBatch] Nvarchar(50) NULL,
	[OtgFacPhone] Nvarchar(50) NULL,
	[OtgStartedOn] Datetime NULL,
	[OtgEndedOn] Datetime NULL,
	[OtgConfigExportedOn] Datetime NULL,
	[OtgDataImportedOn] Datetime NULL,
	[OtgGridColDefaultCCW] Bit Default 0 NOT NULL,
Primary Key ([OtgDBid])
) 
go

Create table [Units]
(
	[UntDBid] Uniqueidentifier rowguidcol NOT NULL,
	[UntName] Nvarchar(10) NOT NULL,
	[UntSitID] Uniqueidentifier NOT NULL,
	[UntIsActive] Bit Default 1 NOT NULL,
Primary Key ([UntDBid])
) 
go

Create table [Customers]
(
	[CstDBid] Uniqueidentifier rowguidcol NOT NULL,
	[CstName] Nvarchar(20) NOT NULL,
	[CstFullName] Nvarchar(100) NOT NULL,
	[CstIsActive] Bit Default 1 NOT NULL,
Primary Key ([CstDBid])
) 
go

Create table [CalibrationProcedures]
(
	[ClpDBid] Uniqueidentifier rowguidcol NOT NULL,
	[ClpName] Nvarchar(50) NOT NULL,
	[ClpDescription] Nvarchar(255) NULL,
	[ClpIsLclChg] Bit Default 0 NOT NULL,
	[ClpIsActive] Bit Default 1 NOT NULL,
Primary Key ([ClpDBid])
) 
go

Create table [InspectedComponents]
(
	[IscDBid] Uniqueidentifier rowguidcol NOT NULL,
	[IscName] Nvarchar(50) NOT NULL,
	[IscWorkOrder] Nvarchar(50) NOT NULL,
	[IscCmpID] Uniqueidentifier NOT NULL,
	[IscEdsNumber] Integer Default 0 NOT NULL,
	[IscOtgID] Uniqueidentifier NULL,
	[IscGrpID] Uniqueidentifier NULL,
	[IscInsID] Uniqueidentifier NULL,
	[IscIsReadyToInspect] Bit Default 0 NOT NULL,
	[IscMinCount] Smallint Default 0 NOT NULL,
	[IscIsFinal] Bit Default 0 NOT NULL,
	[IscIsUtFieldComplete] Bit Default 0 NOT NULL,
	[IscReportPrintedOn] Datetime NULL,
	[IscReportSubmittedOn] Datetime NULL,
	[IscCompletionReportedOn] Datetime NULL,
	[IscAreaSpecifier] Nvarchar(25) NULL,
	[IscPageCountOverride] Smallint NULL,
Primary Key ([IscDBid])
) 
go

Create table [Components]
(
	[CmpDBid] Uniqueidentifier rowguidcol NOT NULL,
	[CmpName] Nvarchar(50) NOT NULL,
	[CmpUntID] Uniqueidentifier NOT NULL,
	[CmpCmtID] Uniqueidentifier NULL,
	[CmpLinID] Uniqueidentifier NULL,
	[CmpSysID] Uniqueidentifier NULL,
	[CmpCtpID] Uniqueidentifier NULL,
	[CmpPslID] Uniqueidentifier NULL,
	[CmpDrawing] Nvarchar(50) NULL,
	[CmpUpMainOd] Numeric(6,3) NULL,
	[CmpUpMainTnom] Numeric(5,3) NULL,
	[CmpUpMainTscr] Numeric(5,3) NULL,
	[CmpDnMainOd] Numeric(6,3) NULL,
	[CmpDnMainTnom] Numeric(5,3) NULL,
	[CmpDnMainTscr] Numeric(5,3) NULL,
	[CmpBranchOd] Numeric(6,3) NULL,
	[CmpBranchTnom] Numeric(5,3) NULL,
	[CmpBranchTscr] Numeric(5,3) NULL,
	[CmpUpExtOd] Numeric(6,3) NULL,
	[CmpUpExtTnom] Numeric(5,3) NULL,
	[CmpUpExtTscr] Numeric(5,3) NULL,
	[CmpDnExtOd] Numeric(6,3) NULL,
	[CmpDnExtTnom] Numeric(5,3) NULL,
	[CmpDnExtTscr] Numeric(5,3) NULL,
	[CmpBrExtOd] Numeric(6,3) NULL,
	[CmpBrExtTnom] Numeric(5,3) NULL,
	[CmpBrExtTscr] Numeric(5,3) NULL,
	[CmpTimesInspected] Integer Default 0 NOT NULL,
	[CmpAvgInspectionTime] Float Default 0 NOT NULL,
	[CmpAvgCrewDose] Float Default 0 NOT NULL,
	[CmpPctChromeMain] Numeric(5,2) NULL,
	[CmpPctChromeBranch] Numeric(5,2) NULL,
	[CmpPctChromeUsExt] Numeric(5,2) NULL,
	[CmpPctChromeDsExt] Numeric(5,2) NULL,
	[CmpPctChromeBrExt] Numeric(5,2) NULL,
	[CmpMisc1] Nvarchar(25) NULL,
	[CmpMisc2] Nvarchar(25) NULL,
	[CmpNote] Nvarchar(256) NULL,
	[CmpHighRad] Bit Default 0 NOT NULL,
	[CmpHardToAccess] Bit Default 0 NOT NULL,
	[CmpHasDs] Bit Default 0 NOT NULL,
	[CmpHasBranch] Bit Default 0 NOT NULL,
	[CmpIsLclChg] Bit Default 0 NOT NULL,
	[CmpUsedInOutage] Bit Default 0 NOT NULL,
	[CmpIsActive] Bit Default 1 NOT NULL,
Primary Key ([CmpDBid])
) 
go

Create table [CouplantTypes]
(
	[CptDBid] Uniqueidentifier rowguidcol NOT NULL,
	[CptName] Nvarchar(50) NOT NULL,
	[CptIsLclChg] Bit Default 0 NOT NULL,
	[CptUsedInOutage] Bit Default 0 NOT NULL,
	[CptIsActive] Bit Default 1 NOT NULL,
Primary Key ([CptDBid])
) 
go

Create table [GridProcedures]
(
	[GrpDBid] Uniqueidentifier rowguidcol NOT NULL,
	[GrpName] Nvarchar(50) NOT NULL,
	[GrpDescription] Nvarchar(255) NULL,
	[GrpDsDiameters] Smallint NULL,
	[GrpIsLclChg] Bit Default 0 NOT NULL,
	[GrpUsedInOutage] Bit Default 0 NOT NULL,
	[GrpIsActive] Bit Default 1 NOT NULL,
Primary Key ([GrpDBid])
) 
go

Create table [OutageGridProcedures]
(
	[OgpGrpID] Uniqueidentifier NOT NULL,
	[OgpOtgID] Uniqueidentifier NOT NULL,
Primary Key ([OgpGrpID],[OgpOtgID])
) 
go

Create table [OutageInspectors]
(
	[OinOtgID] Uniqueidentifier NOT NULL,
	[OinInsID] Uniqueidentifier NOT NULL,
Primary Key ([OinOtgID],[OinInsID])
) 
go

Create table [Kits]
(
	[KitDBid] Uniqueidentifier rowguidcol NOT NULL,
	[KitName] Nvarchar(10) NOT NULL,
Primary Key ([KitDBid])
) 
go

Create table [MeterModels]
(
	[MmlDBid] Uniqueidentifier rowguidcol NOT NULL,
	[MmlName] Nvarchar(50) NOT NULL,
	[MmlManfName] Nvarchar(50) NOT NULL,
	[MmlIsLclChg] Bit Default 0 NOT NULL,
	[MmlIsActive] Bit Default 1 NOT NULL,
	[MmlUsedInOutage] Bit Default 0 NOT NULL,
Primary Key ([MmlDBid])
) 
go

Create table [DucerModels]
(
	[DmdDBid] Uniqueidentifier rowguidcol NOT NULL,
	[DmdName] Nvarchar(50) NOT NULL,
	[DmdFrequency] Numeric(4,2) NOT NULL,
	[DmdSize] Numeric(4,3) NOT NULL,
	[DmdUsedInOutage] Bit Default 0 NOT NULL,
	[DmdIsLclChg] Bit Default 0 NOT NULL,
	[DmdIsActive] Bit Default 1 NOT NULL,
Primary Key ([DmdDBid])
) 
go

Create table [MeterDucers]
(
	[MtdMmlID] Uniqueidentifier NOT NULL,
	[MtdDmdID] Uniqueidentifier NOT NULL,
Primary Key ([MtdMmlID],[MtdDmdID])
) 
go

Create table [Systems]
(
	[SysDBid] Uniqueidentifier rowguidcol NOT NULL,
	[SysName] Nvarchar(40) NOT NULL,
	[SysUntID] Uniqueidentifier NOT NULL,
	[SysIsLclChg] Bit Default 0 NOT NULL,
	[SysIsActive] Bit Default 1 NOT NULL,
Primary Key ([SysDBid])
) 
go

Create table [Lines]
(
	[LinDBid] Uniqueidentifier rowguidcol NOT NULL,
	[LinName] Nvarchar(40) NOT NULL,
	[LinUntID] Uniqueidentifier NOT NULL,
	[LinIsLclChg] Bit Default 0 NOT NULL,
	[LinIsActive] Bit Default 1 NOT NULL,
Primary Key ([LinDBid])
) 
go

Create table [ComponentMaterials]
(
	[CmtDBid] Uniqueidentifier rowguidcol NOT NULL,
	[CmtName] Nvarchar(25) NOT NULL,
	[CmtSitID] Uniqueidentifier NOT NULL,
	[CmtCalBlockMaterial] Tinyint Default 1 NOT NULL,
	[CmtIsLclChg] Bit Default 0 NOT NULL,
	[CmtIsActive] Bit Default 1 NOT NULL,
Primary Key ([CmtDBid])
) 
go

Create table [Measurements]
(
	[MsrDBid] Uniqueidentifier NOT NULL,
	[MsrSvyID] Uniqueidentifier NOT NULL,
	[MsrRow] Smallint Default 0 NOT NULL,
	[MsrCol] Smallint Default 0 NOT NULL,
	[MsrThickness] Numeric(5,3) Default 0 NULL,
	[MsrRowOffset] Numeric(6,3) Default 0 NULL,
	[MsrColOffset] Numeric(6,3) Default 0 NULL,
	[MsrDiameter] Numeric(5,3) NULL,
	[MsrCount] Smallint Default 1 NULL,
	[MsrIsObstruction] Bit Default 0 NOT NULL,
	[MsrIsError] Bit Default 0 NOT NULL,
Primary Key ([MsrDBid])
) 
go

Create table [Inspections]
(
	[IspDBid] Uniqueidentifier rowguidcol NOT NULL,
	[IspName] Nvarchar(25) NOT NULL,
	[IspIscID] Uniqueidentifier NOT NULL,
	[IspNotes] Nvarchar(4000) NULL,
	[IspReportOrder] Smallint Default 0 NOT NULL,
	[IspPersonHours] Float NULL,
Primary Key ([IspDBid])
) 
go

Create table [Graphics]
(
	[GphDBid] Uniqueidentifier rowguidcol NOT NULL,
	[GphIspID] Uniqueidentifier NOT NULL,
	[GphImage] Image NOT NULL,
	[GphBgImageType] Tinyint NOT NULL,
	[GphBgImageFileName] Nvarchar(255) NOT NULL,
Primary Key ([GphDBid])
) 
go

Create table [Dsets]
(
	[DstDBid] Uniqueidentifier rowguidcol NOT NULL,
	[DstName] Nvarchar(20) NOT NULL,
	[DstIspID] Uniqueidentifier NOT NULL,
	[DstGrdID] Uniqueidentifier NULL,
	[DstInsID] Uniqueidentifier NULL,
	[DstDcrID] Uniqueidentifier NULL,
	[DstMtrID] Uniqueidentifier NULL,
	[DstCbkID] Uniqueidentifier NULL,
	[DstThmID] Uniqueidentifier NULL,
	[DstGridPriority] Smallint Default 0 NULL,
	[DstTextFileName] Nvarchar(255) NULL,
	[DstTextFileFormat] Tinyint NULL,
	[DstCompTemp] Smallint NULL,
	[DstCalBlockTemp] Smallint NULL,
	[DstRange] Numeric(5,3) NULL,
	[DstThin] Numeric(5,3) NULL,
	[DstThick] Numeric(5,3) NULL,
	[DstVelocity] Numeric(6,4) NULL,
	[DstGainDb] Numeric(3,1) NULL,
	[DstMinWall] Numeric(5,3) NULL,
	[DstMaxWall] Numeric(5,3) NULL,
	[DstCrewDose] Smallint NULL,
Primary Key ([DstDBid])
) 
go

Create table [Ducers]
(
	[DcrDBid] Uniqueidentifier rowguidcol NOT NULL,
	[DcrSerialNumber] Nvarchar(50) NOT NULL,
	[DcrDmdID] Uniqueidentifier NOT NULL,
	[DcrKitID] Uniqueidentifier NULL,
	[DcrUsedInOutage] Bit Default 0 NOT NULL,
	[DcrIsLclChg] Bit Default 0 NOT NULL,
	[DcrIsActive] Bit Default 1 NOT NULL,
Primary Key ([DcrDBid])
) 
go

Create table [Meters]
(
	[MtrDBid] Uniqueidentifier rowguidcol NOT NULL,
	[MtrSerialNumber] Nvarchar(50) NOT NULL,
	[MtrMmlID] Uniqueidentifier NOT NULL,
	[MtrKitID] Uniqueidentifier NULL,
	[MtrCalDueDate] Datetime NULL,
	[MtrUsedInOutage] Bit Default 0 NOT NULL,
	[MtrIsLclChg] Bit Default 0 NOT NULL,
	[MtrIsActive] Bit Default 1 NOT NULL,
Primary Key ([MtrDBid])
) 
go

Create table [CalBlocks]
(
	[CbkDBid] Uniqueidentifier rowguidcol NOT NULL,
	[CbkSerialNumber] Nvarchar(25) NOT NULL,
	[CbkCalMin] Numeric(5,3) NOT NULL,
	[CbkCalMax] Numeric(5,3) NOT NULL,
	[CbkKitID] Uniqueidentifier NULL,
	[CbkMaterialType] Tinyint NOT NULL,
	[CbkType] Tinyint NOT NULL,
	[CbkUsedInOutage] Bit Default 0 NOT NULL,
	[CbkIsLclChg] Bit Default 0 NOT NULL,
	[CbkIsActive] Bit Default 1 NOT NULL,
Primary Key ([CbkDBid])
) 
go

Create table [Thermos]
(
	[ThmDBid] Uniqueidentifier rowguidcol NOT NULL,
	[ThmSerialNumber] Nvarchar(50) NOT NULL,
	[ThmKitID] Uniqueidentifier NULL,
	[ThmUsedInOutage] Bit Default 0 NOT NULL,
	[ThmIsLclChg] Bit Default 0 NOT NULL,
	[ThmIsActive] Bit Default 1 NOT NULL,
Primary Key ([ThmDBid])
) 
go

Create table [Grids]
(
	[GrdDBid] Uniqueidentifier rowguidcol NOT NULL,
	[GrdIspID] Uniqueidentifier NOT NULL,
	[GrdParentID] Uniqueidentifier NULL,
	[GrdGszID] Uniqueidentifier NULL,
	[GrdRdlID] Uniqueidentifier NULL,
	[GrdAxialLocOverride] Nvarchar(50) NULL,
	[GrdParentStartRow] Smallint NULL,
	[GrdParentStartCol] Smallint NULL,
	[GrdIsFullScan] Bit Default 0 NOT NULL,
	[GrdIsColumnCCW] Bit Default 0 NOT NULL,
	[GrdHideColumnLayoutGraphic] Bit Default 0 NOT NULL,
	[GrdAxialDistance] Numeric(6,3) NULL,
	[GrdRadialDistance] Numeric(6,3) NULL,
	[GrdUpMainStartRow] Smallint NULL,
	[GrdUpMainEndRow] Smallint NULL,
	[GrdDnMainStartRow] Smallint NULL,
	[GrdDnMainEndRow] Smallint NULL,
	[GrdUpExtStartRow] Smallint NULL,
	[GrdUpExtEndRow] Smallint NULL,
	[GrdDnExtStartRow] Smallint NULL,
	[GrdDnExtEndRow] Smallint NULL,
	[GrdBranchStartRow] Smallint NULL,
	[GrdBranchEndRow] Smallint NULL,
	[GrdBranchExtStartRow] Smallint NULL,
	[GrdBranchExtEndRow] Smallint NULL,
	[GrdUpMainPreDivider] Tinyint Default 0 NOT NULL,
	[GrdDnMainPreDivider] Tinyint Default 0 NOT NULL,
	[GrdUpExtPreDivider] Tinyint Default 0 NOT NULL,
	[GrdDnExtPreDivider] Tinyint Default 0 NOT NULL,
	[GrdBranchPreDivider] Tinyint Default 0 NOT NULL,
	[GrdBranchExtPreDivider] Tinyint Default 0 NOT NULL,
	[GrdPostDivider] Tinyint Default 0 NOT NULL,
Primary Key ([GrdDBid])
) 
go

Create table [Inspectors]
(
	[InsDBid] Uniqueidentifier rowguidcol NOT NULL,
	[InsName] Nvarchar(50) NOT NULL,
	[InsKitID] Uniqueidentifier NULL,
	[InsLevel] Tinyint NOT NULL,
	[InsIsLclChg] Bit Default 0 NOT NULL,
	[InsUsedInOutage] Bit Default 0 NOT NULL,
	[InsIsActive] Bit Default 1 NOT NULL,
Primary Key ([InsDBid])
) 
go

Create table [Surveys]
(
	[SvyDBid] Uniqueidentifier rowguidcol NOT NULL,
	[SvyDstID] Uniqueidentifier NOT NULL,
	[SvyNumber] Smallint NULL,
	[SvyTransducer] Nvarchar(10) NULL,
	[SvyVelocity] Numeric(6,4) NULL,
	[SvyGainDb] Smallint NULL,
	[SvyUnits] Nchar(2) NULL,
Primary Key ([SvyDBid])
) 
go

Create table [DrawingControls]
(
	[DctDBid] Uniqueidentifier NOT NULL,
	[DctGphID] Uniqueidentifier NOT NULL,
	[DctType] Tinyint NOT NULL,
	[DctHasText] Bit Default 0 NOT NULL,
	[DctHasStroke] Bit Default 0 NOT NULL,
	[DctHasFill] Bit Default 0 NOT NULL,
	[DctHasTranspBackground] Bit Default 0 NOT NULL,
	[DctFillColor] Integer Default 0 NOT NULL,
	[DctStrokeColor] Integer Default 0 NOT NULL,
	[DctTextColor] Integer Default 0 NOT NULL,
	[DctStroke] Integer Default 1 NOT NULL,
	[DctZindex] Integer Default 0 NOT NULL,
	[DctText] Nvarchar(255) NULL,
	[DctFontFamily] Nvarchar(50) NOT NULL,
	[DctFontPoints] Float Default 0 NOT NULL,
	[DctFontIsBold] Bit Default 0 NOT NULL,
	[DctFontIsItalic] Bit Default 0 NOT NULL,
	[DctFontIsUnderlined] Bit Default 0 NOT NULL,
Primary Key ([DctDBid])
) 
go

Create table [Arrows]
(
	[AroDBid] Uniqueidentifier rowguidcol NOT NULL,
	[AroDctID] Uniqueidentifier NOT NULL,
	[AroShaftWidth] Integer Default 0 NOT NULL,
	[AroBarb] Integer Default 0 NOT NULL,
	[AroTip] Integer Default 0 NOT NULL,
	[AroStartX] Integer Default 0 NOT NULL,
	[AroStartY] Integer Default 0 NOT NULL,
	[AroEndX] Integer Default 0 NOT NULL,
	[AroEndY] Integer Default 0 NOT NULL,
	[AroHeadCount] Tinyint NOT NULL,
Primary Key ([AroDBid])
) 
go

Create table [Notations]
(
	[NtnDBid] Uniqueidentifier rowguidcol NOT NULL,
	[NtnDctID] Uniqueidentifier NOT NULL,
	[NtnTop] Integer Default 0 NOT NULL,
	[NtnLeft] Integer Default 0 NOT NULL,
	[NtnWidth] Integer Default 0 NOT NULL,
	[NtnHeight] Integer Default 0 NOT NULL,
Primary Key ([NtnDBid])
) 
go

Create table [Globals]
(
	[DatabaseVersion] Integer Default 0 NOT NULL,
	[CompatibleDBVersion] Integer Default 0 NOT NULL,
	[SiteActivationKey] Nvarchar(20) NULL,
	[MasterRegCheckedOn] Datetime NULL,
	[UnverifiedSessionCount] Integer Default 0 NOT NULL,
	[IsMasterDB] Bit Default 0 NOT NULL,
	[IsNewDB] Bit Default 0 NOT NULL,
	[IsInactivatedDB] Bit Default 0 NOT NULL
) 
go

Create table [InspectionPeriods]
(
	[IpdDBid] Uniqueidentifier rowguidcol NOT NULL,
	[IpdDstID] Uniqueidentifier NOT NULL,
	[IpdInAt] Datetime NOT NULL,
	[IpdOutAt] Datetime NOT NULL,
	[IpdCalCheck1At] Datetime NULL,
	[IpdCalCheck2At] Datetime NULL,
Primary Key ([IpdDBid])
) 
go

Create table [RadialLocations]
(
	[RdlDBid] Uniqueidentifier rowguidcol NOT NULL,
	[RdlName] Nvarchar(50) NOT NULL,
	[RdlIsLclChg] Bit Default 0 NOT NULL,
	[RdlUsedInOutage] Bit Default 0 NOT NULL,
	[RdlIsActive] Bit Default 1 NOT NULL,
Primary Key ([RdlDBid])
) 
go

Create table [GridSizes]
(
	[GszDBid] Uniqueidentifier rowguidcol NOT NULL,
	[GszName] Nvarchar(20) NOT NULL,
	[GszAxialDistance] Numeric(6,3) NOT NULL,
	[GszRadialDistance] Numeric(6,3) NOT NULL,
	[GszMaxDiameter] Numeric(6,3) NOT NULL,
	[GszIsLclChg] Bit Default 0 NOT NULL,
	[GszUsedInOutage] Bit Default 0 NOT NULL,
	[GszIsActive] Bit Default 1 NOT NULL,
Primary Key ([GszDBid])
) 
go

Create table [Boundaries]
(
	[BdrDBid] Uniqueidentifier rowguidcol NOT NULL,
	[BdrDctID] Uniqueidentifier NOT NULL,
	[BdrAlpha] Tinyint Default 0 NOT NULL,
Primary Key ([BdrDBid])
) 
go

Create table [BoundaryPoints]
(
	[BptDBid] Uniqueidentifier rowguidcol NOT NULL,
	[BptBdrID] Uniqueidentifier NOT NULL,
	[BptX] Integer NOT NULL,
	[BptY] Integer NOT NULL,
	[BptIdx] Tinyint NOT NULL,
Primary Key ([BptDBid])
) 
go

Create table [ComponentTypes]
(
	[CtpDBid] Uniqueidentifier rowguidcol NOT NULL,
	[CtpName] Nvarchar(25) NOT NULL,
	[CtpSitID] Uniqueidentifier NOT NULL,
	[CtpIsLclChg] Bit Default 0 NOT NULL,
	[CtpIsActive] Bit Default 1 NOT NULL,
Primary Key ([CtpDBid])
) 
go

Create table [AdditionalMeasurements]
(
	[AdmDBid] Uniqueidentifier rowguidcol NOT NULL,
	[AdmName] Nvarchar(20) NOT NULL,
	[AdmDstID] Uniqueidentifier NOT NULL,
	[AdmDescription] Nvarchar(255) NULL,
	[AdmThickness] Numeric(5,3) NULL,
	[AdmIncludeInStats] Bit Default 1 NOT NULL,
	[AdmComponentSection] Smallint NULL,
Primary Key ([AdmDBid])
) 
go

Create table [PipeScheduleLookup]
(
	[PslDBid] Uniqueidentifier rowguidcol NOT NULL,
	[PslOd] Numeric(6,3) NOT NULL,
	[PslNomWall] Numeric(5,3) NOT NULL,
	[PslSchedule] Nvarchar(20) NOT NULL,
	[PslNomDia] Numeric(5,3) NOT NULL,
	[PslIsLclChg] Bit Default 0 NOT NULL,
Primary Key ([PslDBid])
) 
go

Create table [GridCells]
(
	[GclGrdID] Uniqueidentifier NOT NULL,
	[GclMsrID] Uniqueidentifier NOT NULL
) 
go

Create table [SpecialCalParams]
(
	[ScpDBid] Uniqueidentifier rowguidcol NOT NULL,
	[ScpName] Nvarchar(25) NOT NULL,
	[ScpUnits] Nvarchar(15) NOT NULL,
	[ScpReportOrder] Smallint Default 0 NOT NULL,
	[ScpUsedInOutage] Bit Default 0 NOT NULL,
	[ScpIsLclChg] Bit Default 0 NOT NULL,
	[ScpIsActive] Bit Default 1 NOT NULL,
Primary Key ([ScpDBid])
) 
go

Create table [SpecialCalValues]
(
	[ScvDBid] Uniqueidentifier rowguidcol NOT NULL,
	[ScvScpID] Uniqueidentifier NOT NULL,
	[ScvDstID] Uniqueidentifier NOT NULL,
	[ScvValue] Nvarchar(10) NOT NULL,
Primary Key ([ScvDBid])
) 
go

Create table [TmpStatusReport]
(
	[IscName] Nvarchar(50) NULL,
	[CmpName] Nvarchar(50) NULL,
	[IscIsReadyToInspect] Bit Default 0 NULL,
	[IscInsID] Uniqueidentifier NULL,
	[IscOtgID] Uniqueidentifier NULL,
	[IscMinCount] Smallint Default 0 NULL,
	[IscIsFinal] Bit Default 0 NULL,
	[IscIsUtFieldComplete] Bit Default 0 NULL,
	[IscReportSubmittedOn] Datetime NULL,
	[IscCompletionReportedOn] Datetime NULL,
	[IscWorkOrder] Nvarchar(50) NULL,
	[IscEdsNumber] Integer Default 0 NULL,
	[TotalCrewDose] Float NULL,
	[TotalPersonHours] Float NULL
) 
go

Create table [TmpComponentListing]
(
	[CmpName] Nvarchar(50) NULL,
	[CtpName] Nvarchar(25) NULL,
	[CmtName] Nvarchar(25) NULL,
	[LinName] Nvarchar(40) NULL,
	[SysName] Nvarchar(40) NULL,
	[PslSchedule] Nvarchar(20) NULL,
	[PslNomDia] Numeric(5,3) NULL,
	[CmpTimesInspected] Integer Default 0 NULL,
	[CmpAvgInspectionTime] Float Default 0 NULL,
	[CmpAvgCrewDose] Float Default 0 NULL,
	[CmpHighRad] Bit Default 0 NULL,
	[CmpHardToAccess] Bit Default 0 NULL,
	[CmpNote] Nvarchar(256) NULL
) 
go


Create UNIQUE Index [SitNameX] ON [Sites] ([SitName] ) 
go
Create Index [IdxSitCstID] ON [Sites] ([SitCstID] ) 
go
Create Index [IdxSitClpID] ON [Sites] ([SitClpID] ) 
go
Create Index [OtgNameX] ON [Outages] ([OtgName] ) 
go
Create Index [IdxOtgUntID] ON [Outages] ([OtgUntID] ) 
go
Create Index [IdxOtgClpID] ON [Outages] ([OtgClpID] ) 
go
Create Index [IdxOtgCptID] ON [Outages] ([OtgCptID] ) 
go
Create Index [UntNameX] ON [Units] ([UntName] ) 
go
Create Index [IdxUntSitID] ON [Units] ([UntSitID] ) 
go
Create UNIQUE Index [CstNameX] ON [Customers] ([CstName] ) 
go
Create UNIQUE Index [ClpName] ON [CalibrationProcedures] ([ClpName] ) 
go
Create Index [IdxIscCmpID] ON [InspectedComponents] ([IscCmpID] ) 
go
Create Index [IdxIscOtgID] ON [InspectedComponents] ([IscOtgID] ) 
go
Create Index [IdxIscGrpID] ON [InspectedComponents] ([IscGrpID] ) 
go
Create Index [IdxIscInsID] ON [InspectedComponents] ([IscInsID] ) 
go
Create Index [CmpNameX] ON [Components] ([CmpName] ) 
go
Create Index [IdxCmpUntID] ON [Components] ([CmpUntID] ) 
go
Create Index [IdxCmpCmtID] ON [Components] ([CmpCmtID] ) 
go
Create Index [IdxCmpLinID] ON [Components] ([CmpLinID] ) 
go
Create Index [IdxCmpSysID] ON [Components] ([CmpSysID] ) 
go
Create Index [IdxCmpCtpID] ON [Components] ([CmpCtpID] ) 
go
Create Index [IdxCmpPslID] ON [Components] ([CmpPslID] ) 
go
Create UNIQUE Index [CptNameX] ON [CouplantTypes] ([CptName] ) 
go
Create UNIQUE Index [GrpNameX] ON [GridProcedures] ([GrpName] ) 
go
Create Index [OgpGrpIDX] ON [OutageGridProcedures] ([OgpGrpID] ) 
go
Create Index [OgpOtgIDX] ON [OutageGridProcedures] ([OgpOtgID] ) 
go
Create Index [IdxOinOtgID] ON [OutageInspectors] ([OinOtgID] ) 
go
Create Index [IdxOinInsID] ON [OutageInspectors] ([OinInsID] ) 
go
Create Index [IdxMtdMmlID] ON [MeterDucers] ([MtdMmlID] ) 
go
Create Index [IdxMtdDmdID] ON [MeterDucers] ([MtdDmdID] ) 
go
Create Index [SysNameX] ON [Systems] ([SysName] ) 
go
Create Index [IdxSysUntID] ON [Systems] ([SysUntID] ) 
go
Create Index [LinNameX] ON [Lines] ([LinName] ) 
go
Create Index [IdxLinUntID] ON [Lines] ([LinUntID] ) 
go
Create Index [CmtNameX] ON [ComponentMaterials] ([CmtName] ) 
go
Create Index [IdxCmtSitID] ON [ComponentMaterials] ([CmtSitID] ) 
go
Create Index [IdxMsrSvyID] ON [Measurements] ([MsrSvyID] ) 
go
Create Index [IdxIspIscID] ON [Inspections] ([IspIscID] ) 
go
Create Index [IdxGphIspID] ON [Graphics] ([GphIspID] ) 
go
Create Index [IdxDstIspID] ON [Dsets] ([DstIspID] ) 
go
Create Index [IdxDstGrdID] ON [Dsets] ([DstGrdID] ) 
go
Create Index [IdxDstInsID] ON [Dsets] ([DstInsID] ) 
go
Create Index [IdxDstDcrID] ON [Dsets] ([DstDcrID] ) 
go
Create Index [IdxDstMtrID] ON [Dsets] ([DstMtrID] ) 
go
Create Index [IdxDstCbkID] ON [Dsets] ([DstCbkID] ) 
go
Create Index [IdxDstThmID] ON [Dsets] ([DstThmID] ) 
go
Create Index [IdxDcrDmdID] ON [Ducers] ([DcrDmdID] ) 
go
Create Index [IdxDcrKitID] ON [Ducers] ([DcrKitID] ) 
go
Create Index [IdxMtrMmlID] ON [Meters] ([MtrMmlID] ) 
go
Create Index [IdxMtrKitID] ON [Meters] ([MtrKitID] ) 
go
Create Index [IdxCbkKitID] ON [CalBlocks] ([CbkKitID] ) 
go
Create Index [IdxThmKitID] ON [Thermos] ([ThmKitID] ) 
go
Create Index [IdxGrdIspID] ON [Grids] ([GrdIspID] ) 
go
Create Index [IdxGrdParentID] ON [Grids] ([GrdParentID] ) 
go
Create Index [IdxGrdGszID] ON [Grids] ([GrdGszID] ) 
go
Create Index [IdxGrdRdlID] ON [Grids] ([GrdRdlID] ) 
go
Create Index [IdxInsKitID] ON [Inspectors] ([InsKitID] ) 
go
Create Index [IdxSvyDstID] ON [Surveys] ([SvyDstID] ) 
go
Create Index [IdxDctGphID] ON [DrawingControls] ([DctGphID] ) 
go
Create Index [IdxAroDctID] ON [Arrows] ([AroDctID] ) 
go
Create Index [IdxNtnDctID] ON [Notations] ([NtnDctID] ) 
go
Create Index [IdxIpdDstID] ON [InspectionPeriods] ([IpdDstID] ) 
go
Create Index [IdxBdrDctID] ON [Boundaries] ([BdrDctID] ) 
go
Create Index [IdxBptBdrID] ON [BoundaryPoints] ([BptBdrID] ) 
go
Create Index [CtpNameX] ON [ComponentTypes] ([CtpName] ) 
go
Create Index [IdxCtpSitID] ON [ComponentTypes] ([CtpSitID] ) 
go
Create Index [IdxAdmDstID] ON [AdditionalMeasurements] ([AdmDstID] ) 
go
Create Index [IdxGclGrdID] ON [GridCells] ([GclGrdID] ) 
go
Create Index [IdxGclMsrID] ON [GridCells] ([GclMsrID] ) 
go
Create Index [IdxScvScpID] ON [SpecialCalValues] ([ScvScpID] ) 
go
Create Index [IdxScvDstID] ON [SpecialCalValues] ([ScvDstID] ) 
go


Alter table [Units] add  foreign key([UntSitID]) references [Sites] ([SitDBid])  on update no action on delete no action 
go
Alter table [ComponentTypes] add  foreign key([CtpSitID]) references [Sites] ([SitDBid])  on update no action on delete no action 
go
Alter table [ComponentMaterials] add  foreign key([CmtSitID]) references [Sites] ([SitDBid])  on update no action on delete no action 
go
Alter table [InspectedComponents] add  foreign key([IscOtgID]) references [Outages] ([OtgDBid])  on update no action on delete no action 
go
Alter table [OutageGridProcedures] add  foreign key([OgpOtgID]) references [Outages] ([OtgDBid])  on update no action on delete cascade 
go
Alter table [OutageInspectors] add  foreign key([OinOtgID]) references [Outages] ([OtgDBid])  on update no action on delete cascade 
go
Alter table [Outages] add  foreign key([OtgUntID]) references [Units] ([UntDBid])  on update no action on delete no action 
go
Alter table [Lines] add  foreign key([LinUntID]) references [Units] ([UntDBid])  on update no action on delete no action 
go
Alter table [Systems] add  foreign key([SysUntID]) references [Units] ([UntDBid])  on update no action on delete no action 
go
Alter table [Components] add  foreign key([CmpUntID]) references [Units] ([UntDBid])  on update no action on delete no action 
go
Alter table [Sites] add  foreign key([SitCstID]) references [Customers] ([CstDBid])  on update no action on delete no action 
go
Alter table [Outages] add  foreign key([OtgClpID]) references [CalibrationProcedures] ([ClpDBid])  on update no action on delete no action 
go
Alter table [Sites] add  foreign key([SitClpID]) references [CalibrationProcedures] ([ClpDBid])  on update no action on delete no action 
go
Alter table [Inspections] add  foreign key([IspIscID]) references [InspectedComponents] ([IscDBid])  on update no action on delete no action 
go
Alter table [InspectedComponents] add  foreign key([IscCmpID]) references [Components] ([CmpDBid])  on update no action on delete no action 
go
Alter table [Outages] add  foreign key([OtgCptID]) references [CouplantTypes] ([CptDBid])  on update no action on delete no action 
go
Alter table [OutageGridProcedures] add  foreign key([OgpGrpID]) references [GridProcedures] ([GrpDBid])  on update no action on delete no action 
go
Alter table [InspectedComponents] add  foreign key([IscGrpID]) references [GridProcedures] ([GrpDBid])  on update no action on delete no action 
go
Alter table [Inspectors] add  foreign key([InsKitID]) references [Kits] ([KitDBid])  on update no action on delete Set Null 
go
Alter table [CalBlocks] add  foreign key([CbkKitID]) references [Kits] ([KitDBid])  on update no action on delete Set Null 
go
Alter table [Ducers] add  foreign key([DcrKitID]) references [Kits] ([KitDBid])  on delete Set Null 
go
Alter table [Thermos] add  foreign key([ThmKitID]) references [Kits] ([KitDBid])  on update no action on delete Set Null 
go
Alter table [Meters] add  foreign key([MtrKitID]) references [Kits] ([KitDBid])  on update no action on delete Set Null 
go
Alter table [MeterDucers] add  foreign key([MtdMmlID]) references [MeterModels] ([MmlDBid])  on update no action on delete cascade 
go
Alter table [Meters] add  foreign key([MtrMmlID]) references [MeterModels] ([MmlDBid])  on update no action on delete no action 
go
Alter table [MeterDucers] add  foreign key([MtdDmdID]) references [DucerModels] ([DmdDBid])  on update no action on delete cascade 
go
Alter table [Ducers] add  foreign key([DcrDmdID]) references [DucerModels] ([DmdDBid])  on update no action on delete no action 
go
Alter table [Components] add  foreign key([CmpSysID]) references [Systems] ([SysDBid])  on update no action on delete no action 
go
Alter table [Components] add  foreign key([CmpLinID]) references [Lines] ([LinDBid])  on update no action on delete no action 
go
Alter table [Components] add  foreign key([CmpCmtID]) references [ComponentMaterials] ([CmtDBid])  on update no action on delete no action 
go
Alter table [GridCells] add  foreign key([GclMsrID]) references [Measurements] ([MsrDBid])  on update no action on delete cascade 
go
Alter table [Graphics] add  foreign key([GphIspID]) references [Inspections] ([IspDBid])  on update no action on delete no action 
go
Alter table [Grids] add  foreign key([GrdIspID]) references [Inspections] ([IspDBid])  on update no action on delete no action 
go
Alter table [Dsets] add  foreign key([DstIspID]) references [Inspections] ([IspDBid])  on update no action on delete no action 
go
Alter table [DrawingControls] add  foreign key([DctGphID]) references [Graphics] ([GphDBid])  on update no action on delete cascade 
go
Alter table [InspectionPeriods] add  foreign key([IpdDstID]) references [Dsets] ([DstDBid])  on update no action on delete cascade 
go
Alter table [Surveys] add  foreign key([SvyDstID]) references [Dsets] ([DstDBid])  on update no action on delete cascade 
go
Alter table [AdditionalMeasurements] add  foreign key([AdmDstID]) references [Dsets] ([DstDBid])  on update no action on delete cascade 
go
Alter table [SpecialCalValues] add  foreign key([ScvDstID]) references [Dsets] ([DstDBid])  on update no action on delete cascade 
go
Alter table [Dsets] add  foreign key([DstDcrID]) references [Ducers] ([DcrDBid])  on update no action on delete no action 
go
Alter table [Dsets] add  foreign key([DstMtrID]) references [Meters] ([MtrDBid])  on update no action on delete no action 
go
Alter table [Dsets] add  foreign key([DstCbkID]) references [CalBlocks] ([CbkDBid])  on update no action on delete no action 
go
Alter table [Dsets] add  foreign key([DstThmID]) references [Thermos] ([ThmDBid])  on update no action on delete no action 
go
Alter table [Dsets] add  foreign key([DstGrdID]) references [Grids] ([GrdDBid])  on update no action on delete Set Null 
go
Alter table [Grids] add  foreign key([GrdParentID]) references [Grids] ([GrdDBid])  on update no action on delete no action 
go
Alter table [GridCells] add  foreign key([GclGrdID]) references [Grids] ([GrdDBid])  on update no action on delete no action 
go
Alter table [Dsets] add  foreign key([DstInsID]) references [Inspectors] ([InsDBid])  on update no action on delete no action 
go
Alter table [OutageInspectors] add  foreign key([OinInsID]) references [Inspectors] ([InsDBid])  on update no action on delete no action 
go
Alter table [InspectedComponents] add  foreign key([IscInsID]) references [Inspectors] ([InsDBid])  on update no action on delete Set Null 
go
Alter table [Measurements] add  foreign key([MsrSvyID]) references [Surveys] ([SvyDBid])  on update no action on delete cascade 
go
Alter table [Notations] add  foreign key([NtnDctID]) references [DrawingControls] ([DctDBid])  on update no action on delete cascade 
go
Alter table [Arrows] add  foreign key([AroDctID]) references [DrawingControls] ([DctDBid])  on update no action on delete cascade 
go
Alter table [Boundaries] add  foreign key([BdrDctID]) references [DrawingControls] ([DctDBid])  on update no action on delete cascade 
go
Alter table [Grids] add  foreign key([GrdRdlID]) references [RadialLocations] ([RdlDBid])  on update no action on delete no action 
go
Alter table [Grids] add  foreign key([GrdGszID]) references [GridSizes] ([GszDBid])  on update no action on delete no action 
go
Alter table [BoundaryPoints] add  foreign key([BptBdrID]) references [Boundaries] ([BdrDBid])  on update no action on delete cascade 
go
Alter table [Components] add  foreign key([CmpCtpID]) references [ComponentTypes] ([CtpDBid])  on update no action on delete no action 
go
Alter table [Components] add  foreign key([CmpPslID]) references [PipeScheduleLookup] ([PslDBid])  on update no action on delete no action 
go
Alter table [SpecialCalValues] add  foreign key([ScvScpID]) references [SpecialCalParams] ([ScpDBid])  on update no action on delete no action 
go


Set quoted_identifier on
go


Set quoted_identifier off
go


