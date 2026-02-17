using RealPage.OneSite.Affordable.DataHubLRTC.BusinessObjects.Enum;
using System;
using System.Collections.Generic;

namespace RealPage.OneSite.Affordable.DataHubLRTC.BusinessObjects.Class
{

    public class DHTCSiteImplementationRequest
    {
        public int PmcId { get; set; }
        public int SiteId { get; set; }
        public General DHTCGeneralInfo { get; set; }
        public List<PassbookRate> DHTCPassbookRates { get; set; } = new List<PassbookRate> { };
        public List<HouseholdType> DHTCHouseholdTypes { get; set; } = new List<HouseholdType> { };
        public List<UnitType> DHTCUnitTypes { get; set; } = new List<UnitType> { };
        public List<Floorplan> DHTCFloorplan { get; set; } = new List<Floorplan> { };
        public List<UtilityAllowanceSource> DHTCUtilityAllowanceSources { get; set; } = new List<UtilityAllowanceSource> { };
        public List<UtilityAllowance> DHTCUtilityAllowances { get; set; } = new List<UtilityAllowance> { };
        public List<IncomeLimit> DHTCIncomeLimits { get; set; } = new List<IncomeLimit> { };
        public List<IncomeLimitDetail> DHTCIncomeLimitDetails { get; set; } = new List<IncomeLimitDetail> { };
        public List<Program> DHTCPrograms { get; set; } = new List<Program> { };
        public List<SetAside> DHTCSetAsides { get; set; } = new List<SetAside> { };
        public List<SetAsideRule> DHTCSetAsideRules { get; set; } = new List<SetAsideRule> { };
        public List<Building> DHTCBuildings { get; set; } = new List<Building> { };
        public List<RentFloor> DHTCRentFloors { get; set; } = new List<RentFloor> { };
    }
    public class General
    {
        //can we add pmc id, site id here...?
        public DateTime? PropertyDate { get; set; }
        public List<int> ProgramTypes { get; set; }
        public int? UnitCount { get; set; } = 0;
        public bool HasTics { get; set; }
        public bool HasMarketUnits { get; set; }
        public bool HasExemptUnits { get; set; }
        public bool RequireVerifications { get; set; }
        public LrtcDefaultTics DefaultTic { get; set; }
        public bool NeedsHotma { get; set; }
        public bool UseHouseholdTypes { get; set; }
        public bool UseUnitTypes { get; set; }
        public bool UseUtilityAllowances { get; set; }
        public bool UseRentFloors { get; set; }
    }

    public class PassbookRate
    {
        public int Id { get; set; }
        public decimal Rate { get; set; }
        public DateTime StartDate { get; set; }
        public int AppliesTo { get; set; }
    }
    public class HouseholdType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string GroupId { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
    }

    public class UnitType
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UtilityAllowanceSource
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? EffectiveDate { get; set; }
    }

    public class UtilityAllowance
    {
        public int Id { get; set; }
        public int FloorPlanId { get; set; }
        public string FloorPlanCode { get; set; } = string.Empty;
        public int UtilityAllowanceSourceId { get; set; }
        public string UtilityAllowanceSourceName { get; set; } = string.Empty;
        public DateTime? uaSourceEffectiveDate { get; set; }
        public decimal? Amount { get; set; }
    }

    public class Floorplan
    {
        public int FloorplanId { get; set; }
        public string FloorplanCode { get; set; }
        public int BedCount { get; set; }
        public int UnitCount { get; set; }
    }

    public class IncomeLimit
    {
        public int Id { get; set; }
        public IncomeLimitType IncomeLimitType { get; set; }
        public int IncomeLimitTypeID { get; set; } = 0;
        public string OtherType { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string County { get; set; }
        public string AreaFipsCode { get; set; }
        public string ExpandPersons { get; set; }
        public DateTime? StartDate { get; set; }
    }

    public class IncomeLimitDetail
    {
        public int Id { get; set; }
        public int IncomeLimitSourceId { get; set; }
        public string IncomeLimitSourceName { get; set; } = string.Empty;
        public decimal PercentageLimit { get; set; } = 0.00m;

        public decimal OnePerson { get; set; } = 0.00m;
        public decimal TwoPerson { get; set; } = 0.00m;
        public decimal ThreePerson { get; set; } = 0.00m;
        public decimal FourPerson { get; set; } = 0.00m;
        public decimal FivePerson { get; set; } = 0.00m;
        public decimal SixPerson { get; set; } = 0.00m;
        public decimal SevenPerson { get; set; } = 0.00m;
        public decimal EightPerson { get; set; } = 0.00m;
        public decimal NinePerson { get; set; } = 0.00m;
        public decimal TenPerson { get; set; } = 0.00m;
        public decimal ElevenPerson { get; set; } = 0.00m;
        public decimal TwelvePerson { get; set; } = 0.00m;
        public decimal ThirteenPerson { get; set; } = 0.00m;
        public decimal FourteenPerson { get; set; } = 0.00m;
        public decimal FifteenPerson { get; set; } = 0.00m;
        public decimal SixteenPerson { get; set; } = 0.00m;
    }

    public class Program
    {
        public int Id { get; set; }
        public int OnesiteProgramId { get; set; }
        public int ProgramType { get; set; }
        public int OnesiteProgramTypeID { get; set; }
        public string ProgramTypeName { get; set; } = string.Empty;
        public string OtherProgramType { get; set; } = string.Empty;
        public string OtherProgramTypeName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string StateIdentifier { get; set; } = string.Empty;
        public int IncomeLimitSourceId { get; set; }
        public string IncomeLimitSourceName { get; set; } = string.Empty;
        public string UtilityAllowanceSourceIds { get; set; } = string.Empty;
        public string UtilityAllowanceSourceId { get; set; } = string.Empty;
        public List<string> UtilityAllowanceSourcesName { get; set; } = new List<string>();
        public string DefaultUASourceName { get; set; } = string.Empty;
        public int Lrtc10cElection { get; set; }
        public DateTime? DesignationStartDate { get; set; }
        public decimal MinimumUnitsPercentage { get; set; }
        public decimal MinimumSetAsidePercentage { get; set; }
        public int Lrtc8bElection { get; set; }
        public string BuildingIds { get; set; } = string.Empty;
        public bool ApplyBuildingTransferRule { get; set; }
        public bool ApplyUvr { get; set; }
        public int UvrScope { get; set; }
        public int UvrViolation { get; set; }
        public int UvrComparableUnit { get; set; }
        public decimal UvrUnitLargerPercentage { get; set; }
        public bool ApplyNaur { get; set; }
        public int NaurScope { get; set; }
        public int NaurViolation { get; set; }
        public bool ApplyHome { get; set; }
        public int HomeRuleUnitVariance { get; set; }
        public int LowHomeUnitCount { get; set; }
        public int HighHomeUnitCount { get; set; }
        public string AdjustedIncomeOrExpensesType { get; set; } = string.Empty;
        public decimal LevelOverIncome { get; set; }
        public bool ApplyStudent { get; set; }
        public string StudentRuleType { get; set; } = string.Empty;
        public bool IsFilingJointTaxReturn { get; set; }
        public bool IsSingleParent { get; set; }
        public bool IsReceivingAfdc { get; set; }
        public bool IsEnrolledInJobTraining { get; set; }
        public bool IsOtherException { get; set; }
        public bool IsFosterCare { get; set; }
        public bool IsExtendedUse { get; set; }
        public bool IsFinancialAid { get; set; }
        public bool IsVulnerableYouth { get; set; }
    }

    public class ProgramType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    
    public class ProgramTypeName
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class SetAside
    {
        public int? SetAsideId { get; set; }
        public int? OSSetAsideId { get; set; }
        public int? ProgramId { get; set; }
        public int? OSProgramId { get; set; }
        public string ProgramName { get; set; }
        public int? ProgramType { get; set; }
        public int? OSProgramType { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public string HomeSetAsideType { get; set; }
        public int? UnitCount { get; set; }
        public decimal? PopulationPercentage { get; set; }
        public string PopulationRestriction { get; set; }
        public string BuildingIds { get; set; }
        public string UnitTypeIds { get; set; }
        public string FloorPlanIds { get; set; }
        public string UnitsByFloorPlan { get; set; }
        public string UnitsByBedroom { get; set; }
        public bool? IsIncomeRelated { get; set; }
        public decimal? OverIncomePercentage { get; set; }
        public decimal? OverMedianIncome { get; set; }
        public decimal? MaxIncomeMedianPercentage { get; set; }
        public decimal? MaxIncomeAnnualAmount { get; set; }
        public string MaxRentDetermination { get; set; }
        public decimal? MaxRentPercentage { get; set; }
        public decimal? MaxRentMedianPercentage { get; set; }
        public bool? HasHouseholdTypeRestriction { get; set; }
        public string HouseholdTypeIds { get; set; }
        public bool? HasHouseholdSizeRestriction { get; set; }
        public int? MinimumMembersCount { get; set; }
        public int? MaximumMembersCount { get; set; }
        public bool? HasAgeRestriction { get; set; }
        public string AgeBasedRequirement { get; set; }
        public string HouseholdRelationships { get; set; }
        public int? MemberAge { get; set; }
        public string MemberAgeComparison { get; set; }
        public bool? HasAdditionalAgeRestriction { get; set; }
        public string AdditionalAgeBasedRequirement { get; set; }
        public string AdditionalHouseholdRelationships { get; set; }
        public int? AdditionalMemberAge { get; set; }
        public string AdditionalMemberAgeComparison { get; set; }
        public bool? HasMinimumIncomeRestriction { get; set; }
        public decimal? MinimumIncomeMedianPercentage { get; set; }
        public decimal? MinimumIncomeAmount { get; set; }
        public decimal? MinimumIncomeRentMultiplier { get; set; }
        public int SpId { get; set; }
    }

    public class SetAsideDetail
    {
        public int RowNo { get; set; }
        public int SetAsideID { get; set; }
        public int ProgramID { get; set; }
        public int TcptidNew { get; set; }
        public string ProgramNameNew { get; set; }
        public int Tcptid { get; set; }
        public string ProgramType { get; set; }
        public string ProgramName { get; set; }
        public string SetAsideName { get; set; }
        public string SetAsideShortName { get; set; }
        public DateTime? AllocationStartDate { get; set; }
        public DateTime? RecertificationUntilDate { get; set; }
        public DateTime? AllocationEndDate { get; set; }
        public decimal? MinimumSetAside { get; set; }
        public decimal? OverIncomePercent { get; set; }
        public decimal? MaxRentMedianIncomePercent { get; set; }
        public decimal? PercentageGoal { get; set; }
        public string UnitOfPolution { get; set; }
        public int IncomeLimitTableIdForRent { get; set; }
        public int IncomeLimitTableIdIncomeReq { get; set; }
        public int IncomeLimitTableIdIncomeReqNonLihtc { get; set; }
        public bool HoldHarmlessFlag { get; set; }
        public string DeterminationOfIncomeEligibility { get; set; }
        public string IncomeAmountSelected { get; set; }
        public decimal? MaximumIncomeAmount { get; set; }
        public decimal? MaxIncomeAmount_MoveInRequirement { get; set; }
        public decimal? MinimumIncomeAmount { get; set; }
        public decimal? MinIncomeAmount_MoveInRequirement { get; set; }
        public decimal? MedianIncomeBySelected { get; set; }
        public decimal? MaximumIncomePercentMedian { get; set; }
        public decimal? MaximumIncomePercentMedianHome { get; set; }
        public decimal? MinimumIncomePercentMedian { get; set; }
        public decimal? MinIncomePercent_MoveInRequirement { get; set; }
        public decimal? MaxIncomePercent_MoveInRequirement { get; set; }
        public int TimesRentBySelectedId { get; set; }
        public decimal? MinimumIncomeTimesRent { get; set; }
        public decimal? MinimumIncomeTimesRent_MoveInRequirement { get; set; }
        public string HouseholdsSelectedIds { get; set; }
        public bool ApplyTransferMoveInHousehold { get; set; }
        public bool MinimumHouseholdSizeChecked { get; set; }
        public int MinHouseholdSize { get; set; }
        public int MaxHouseholdSize { get; set; }
        public int DeterminationOfRentsId { get; set; }
        public string NextAvailableUnitRule { get; set; }
        public int ChkMaxMinRentsId { get; set; }
        public decimal? MaximumRentPercentageOf { get; set; }
        public decimal? MaxRentPercentOfIncome { get; set; }
        public decimal? MaxRentPercentOfGrossIncome { get; set; }
        public decimal? MaxRentPercentOfPercentMedian { get; set; }
        public decimal? MaxRentPercentOfMedian { get; set; }
        public bool MaxRentCalcMethodHouseholdSizeFlag { get; set; }
        public int NumOfPersonsPerBedroom { get; set; }
        public decimal? MinRentPercentage { get; set; }
        public decimal? MinRentPercentOfMaxIncome { get; set; }
        public decimal? MinRentPercentOfGrossIncome { get; set; }
        public int UnitTypesId { get; set; }
        public int FloorPlansId { get; set; }
        public int DefineRequiredUnitsFlag { get; set; }
        public bool AssignVacantUnits { get; set; }
        public decimal? ApplicableIncomeLimitPercent { get; set; }
        public bool ApplyHomeRulesToRentUpOnlyFlag { get; set; }
        public string HomeType { get; set; }
        public DateTime? DeterminationOfRentsStartDate { get; set; }
        public string DeterminationOfRentsMaximumRentType { get; set; }
        public decimal? DeterminationOfMedianIncomePercent { get; set; }

        // Additional metadata
        public int SpId { get; set; }
    }

    public class UnitCountType
    {
        public int Value { get; set; }
        public string Label { get; set; }
        public int? ParentValue { get; set; } // Nullable in case it's optional
    }

    public class SetAsideRule
    {
        public int? Id { get; set; }
        public int? PrimarySetAsideId { get; set; }
        public string PrimarySetAsideName { get; set; } = string.Empty;
        public string Relationship { get; set; }
        public int? SecondarySetAsideId { get; set; }
        public string SecondarySetAsideName { get; set; } = string.Empty;
    }

    public class Building
    {
        public int Id { get; set; }
        public int OnesiteBuildingID { get; set; }
        public string BuildingNumber { get; set; } = string.Empty;
        public string OnesiteBuildingNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string County { get; set; } = string.Empty;
        public string RentUp { get; set; } = string.Empty;
        public string Bin { get; set; } = string.Empty;
        public DateTime? PlacedInServiceDate { get; set; }
        public string MaxRentMethod { get; set; } = string.Empty;
        public DateTime? MaxRentDate { get; set; }
        public decimal? ApplicableFraction { get; set; }
        public string TaxCreditAllocation { get; set; }
    }

    public class RentFloor
    {
        public int Id { get; set; }
        public int FloorPlanId { get; set; }
        public string FloorPlanCode { get; set; } = string.Empty;
        public int SetAsideId { get; set; }
        public string SetAsideName { get; set; } = string.Empty;
        public string SetAsideShortName { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
    }

    public class SetAsideBuildingAssignment
    {
        public int SaID { get; set; }
        public int PID { get; set; }
        public string SaName { get; set; }
        public string SaShortName { get; set; }
        public decimal SaPercentageGoal { get; set; }
        public DateTime? SaAllocationStartDate { get; set; }
        public DateTime? SaAllocationEndDate { get; set; }
        public string PName { get; set; }
        public string PDescription { get; set; }
        public int TcptID { get; set; }
        public bool PTrackCompliancePerBuildingFlag { get; set; }
        public string TcptDescription { get; set; }
        public string TcptCode { get; set; }
        public int BpiID { get; set; }
        public int BldgID { get; set; }
        public bool BpiExemptFlag { get; set; }
        public bool BpiRentupFlag { get; set; }
        public string Bin { get; set; }
        public DateTime? ServiceDate { get; set; }
        public decimal ApplicableFractionGoal { get; set; }
        public bool Flag89 { get; set; }
        public string ElectDate { get; set; }
        public string Flag89Desc { get; set; }
        public int BldgNumber { get; set; }
        public string BldgName { get; set; }
        public string BldgDescription { get; set; }
        public int BldgUnitCount { get; set; }
        public string BsaiIDDisplay { get; set; }
        public int BsaiID { get; set; }
        public decimal? BsaiAppFracGoal { get; set; }
        public decimal? BsaiAppFrac { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? RevokedDate { get; set; }
        public DateTime? AppliedDate { get; set; }
        public int UaID { get; set; }
        public string UaName { get; set; }
        public bool EnableRentFloorMinimum { get; set; }
        public string ProgrameTypeName { get; set; }
        public decimal? TotalUnitsPersent { get; set; }
        public int? TotalUnitsNumbers { get; set; }
        public string Federal10cOption { get; set; }
        public string AssignVacantUnits { get; set; }
        public string DesignatedUnits { get; set; }
        public bool DesignatedUnitsHiddenCol { get; set; }
        public string DesignatedUnitParent { get; set; }
        public bool DeletableRow { get; set; }

        // Hidden columns
        public bool UaNameHiddenCol { get; set; }
        public bool BsaiAppFracGoalHiddenCol { get; set; }
        public bool BsaiAppFracHiddenCol { get; set; }
        public bool ApprovedDateHiddenCol { get; set; }
        public bool RevokedDateHiddenCol { get; set; }
        public bool TotalUnitsPersentHiddenCol { get; set; }
        public bool TotalUnitsNumbersHiddenCol { get; set; }
        public bool UaIDHiddenCol { get; set; }
        public bool Flag89DescHiddenCol { get; set; }

        // Editable columns
        public bool ProgrameTypeNameEditableCol { get; set; }
        public bool SaIDEditableCol { get; set; }
        public bool SaNameEditableCol { get; set; }
        public bool BldgNumberEditableCol { get; set; }
        public bool BpiRentupFlagEditableCol { get; set; }
        public bool UaNameEditableCol { get; set; }
        public bool EnableRentFloorMinimumEditableCol { get; set; }
        public bool BsaiAppFracGoalEditableCol { get; set; }
        public bool BsaiAppFracEditableCol { get; set; }
        public bool BldgUnitCountEditableCol { get; set; }
        public bool ApprovedDateEditableCol { get; set; }
        public bool RevokedDateEditableCol { get; set; }
        public bool Flag89DescEditableCol { get; set; }
        public bool BinEditableCol { get; set; }
        public bool ServiceDateEditableCol { get; set; }
        public bool PIDEditableCol { get; set; }
        public bool BsaiIDDisplayEditableCol { get; set; }
        public bool TotalUnitsPersentEditableCol { get; set; }
        public bool TotalUnitsNumbersEditableCol { get; set; }
    }

    public class SetAsideUnitTypeAssignment
    {
        public string ProgramName { get; set; }
        public int ProgramID { get; set; }
        public string SetAsideName { get; set; }
        public int SetAsideID { get; set; }
        public int UnitTypeID { get; set; }
        public string UnitDescription { get; set; }
        public bool ApplyPercentage { get; set; }
        public int UnitTypePopulationID { get; set; }
        public bool EditableRow { get; set; }
        public bool DeletableRow { get; set; }
        public bool ProgramIDEditable { get; set; }
        public bool SetAsideIDEditable { get; set; }
        public bool UnitDescriptionEditable { get; set; }
        public bool ApplyPercentageEditable { get; set; }
    }

    public class SetAsideFloorPlanAssignment
    {
        public string ProgramName { get; set; }
        public int ProgramID { get; set; }
        public string SetAsideName { get; set; }
        public int SetAsideID { get; set; }
        public int FloorPlanID { get; set; }
        public string FloorPlanDescription { get; set; }
        public bool ApplyPercentage { get; set; }
        public int FloorPlanPopulationID { get; set; }
        public bool EditableRow { get; set; }
        public bool DeletableRow { get; set; }
        public bool ProgramIDEditable { get; set; }
        public bool SetAsideIDEditable { get; set; }
        public bool UnitDescriptionEditable { get; set; }
        public bool FloorPlanDescriptionEditable { get; set; }
        public bool ApplyPercentageEditable { get; set; }
    }

    public class SetAsideRequiredUnitsByFloorPlan
    {
        public int SafloorPlanID { get; set; }
        public string ProgramType { get; set; }
        public int PTID { get; set; }
        public string ProgramName { get; set; }
        public int PID { get; set; }
        public int SaID { get; set; }
        public int FloorPlanID { get; set; }
        public string FloorPlans { get; set; }
        public int NoOfUnits { get; set; }
        public int RequiredUnits { get; set; }
        public bool EditableRow { get; set; }
        public bool DeletableRow { get; set; }
        public bool PIDEditable { get; set; }
        public bool PTIDEditable { get; set; }
        public bool SaIDEditable { get; set; }
        public bool FloorPlansEditable { get; set; }
        public bool NoOfUnitsEditable { get; set; }
        public bool RequiredUnitsEditable { get; set; }
    }

    public class SetAsideRequiredUnitsByBedroomCount
    {
        public int BedroomCountID { get; set; }
        public string ProgramType { get; set; }
        public int PTID { get; set; }
        public string ProgramName { get; set; }
        public int PID { get; set; }
        public int SaID { get; set; }
        public int BedroomCount { get; set; }
        public int NoOfUnits { get; set; }
        public int RequiredUnits { get; set; }
        public bool EditableRow { get; set; }
        public bool DeletableRow { get; set; }
        public bool PIDEditable { get; set; }
        public bool PTIDEditable { get; set; }
        public bool SaIDEditable { get; set; }
        public bool BedroomCountEditable { get; set; }
        public bool NoOfUnitsEditable { get; set; }
        public bool RequiredUnitsEditable { get; set; }
    }

    public class LookupItem
    {
        public int Value { get; set; }
        public string Label { get; set; }
    }

    public class PopulationRestrictionLookup
    {
        public int? ID { get; set; }
        public int? Count { get; set; } // Nullable to allow missing values
    }
}
