using RealPage.OneSite.Affordable.DataHubLRTC.DBObjects.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPage.OneSite.Affordable.DataHubLRTC.DBObjects.Class
{
    public class KafkaConsumerDB : IKafkaConsumerDB
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public KafkaConsumerDB()
        {
        }

        /// <summary>
        /// SaveKafkaEventSubscriptionDetail
        /// </summary>
        public string SaveKafkaEventPublishDetail
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC dbo.uspSaveKafkaEventPublishDetail
					  @EventPublishDetailID = @EventPublishDetailID
                    , @EventPMCId = @EventPMCId
                    , @EventSiteId = @EventSiteId	
                    , @TopicName = @TopicName
                    , @Payload = @Payload
                    , @Published = @Published
                    , @PubGUID= @PubGUID";

                return query;
            }
        }

        /// <summary>
        /// SaveKafkaEventPublishDetailLog
        /// </summary>
        public string SaveKafkaEventPublishDetailLog
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC dbo.uspSaveKafkaEventPublishDetailLog
					  @EventPublishDetailLogID = @EventPublishDetailLogID
                    , @EventPublishDetailID = @EventPublishDetailID
                    , @SentDateTime = @SentDateTime	
                    , @AckDateTime =  @AckDateTime 
                    , @ErrorDateTime = @ErrorDateTime	
                    , @ErrorMessage =  @ErrorMessage ";

                return query;
            }
        }

        /// <summary>
        /// GetSiteImplementationKafkaMessage
        /// </summary>
        public string GetSiteImplementationKafkaMessage
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC dbo.uspGetSiteImplementationKafkaMessage
					  @InternalEntityID = @InternalEntityID
                    , @ImplementationUUID = @ImplementationUUID ";

                return query;
            }
        }

        /// <summary>
        /// SaveSiteImplementationKafkaMessage SQL
        /// </summary>
        public string SaveSiteImplementationKafkaMessage
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC dbo.uspSaveSiteImplementationKafkaMessage
					  @SiteImplementationKafkaMessageID = @SiteImplementationKafkaMessageID
                    , @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @ImplementationUUID =  @ImplementationUUID
                    , @ProductCode = @ProductCode
                    , @Message = @Message	
                    , @Status =  @Status
                    , @StatusMessage = @StatusMessage	
                    , @InternalUserID =  @InternalUserID";

                return query;
            }
        }
        public string SaveGeneralNeedsHotma
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[UnitySettings_uspUpdateHOTMA]
                      @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID
                    , @EnableHOTMAValue = @EnableHOTMAValue";

                return query;
            }
        }
        public string SaveGeneralReqVerification
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspverificationsetupupdate]
                      @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID
                    , @ReqVerification = @ReqVerification";

                return query;
            }
        }

        public string SaveGeneralDefaultTic
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_usptenantincomecertificationupdate]
                      @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID
                    , @SELECTEDBIT =  @SELECTEDBIT
                    , @DEFAULTTIC=@DEFAULTTIC
                    ,@ID=@ID";

                return query;
            }
        }
        public string GetTenantIncomeCertificationsList
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[UnitySettings_uspTenantIncomeCertificationsList]
                      @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID";

                return query;
            }
        }
        public string SaveTcUnitType
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_usptaxcreditunittypesinsert]
                      @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID
                    ,@description = @description";

                return query;
            }
        }
        public string GetTcUnitType
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_usptaxcreditunittypesselect]
                      @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID";

                return query;
            }
        }
        public string GetTcHouseHoldType
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspgethouseholdtypes]
                     @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID";

                return query;
            }
        }
        public string SaveTcHouseHoldType
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[UnitySettings_uspSaveHouseholdType]
                     @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID
                    ,@HouseholdgroupID = @HouseholdgroupID
                    ,@HouseholdType = @HouseholdType";

                return query;
            }
        }

        public string GetTcHouseHoldGroupId
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspgetcontracttchouseholdgroups]
                      @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID";

                return query;
            }
        }
        public string UpdateutilityAllowanceSource
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspaddupdateutilityallowance]
                       @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID
                     ,@UtilityAllowanceName = @UtilityAllowanceName
                     ,@StartDate = @StartDate,
                      @UtilityID=@UtilityID";

                return query;
            }
        }
        public string UpdateutilityAllowanceDetails
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspupdateutilityallowancedetails]
                       @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID
                    ,@uadID = @uadID
                     ,@Amount = @Amount ";

                return query;
            }
        }
        public string GetUtilityAllowanceSource
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspgettaxcreditutilityallowances]
                       @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID";

                return query;
            }
        }
        public string GetUtilityAllowances
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[UnitySettings_uspGetTaxCreditUtilityAllowancesDetails]
                       @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID";

                return query;
            }
        }
        public string GetFlooplans
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[UnitySettings_uspUtilityAllowanceFloorPlanPickList]
                       @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID";

                return query;
            }
        }
        public string GetIncomeLimitType
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspaffordableincomelimitstype]
                       @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID";

                return query;
            }
        }
        public string GetIncomeLimits
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspaffordableincomelimits]
                       @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID =  @InternalUserID";

                return query;
            }
        }
        public string CreateIncomeLimit
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspaddeditincomelimits]
                       @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID = @InternalUserID
                    , @ID = @ID
                    , @name = @name
                    , @groupid = @groupid
                    , @InternalamfCode = @InternalamfCode
                    , @startdate = @startdate
                    , @round50flag = @round50flag
                    ,@limiteffectivedate = @limiteffectivedate";

                return query;
            }
        }

        public string SaveIncomeLimitDetails
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspaddeditincomelimitdetails]
                       @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID = @InternalUserID
                    , @typevalue = @typevalue
                    , @startDate = @startDate
                    , @id = @id
                    , @medianpercent = @medianpercent
                    , @member1 = @member1
                    , @member2 = @member2
                    , @member3 = @member3
                    , @member4 = @member4
                    , @member5 = @member5
                    , @member6 = @member6
                    , @member7 = @member7
                    , @member8 = @member8
                    , @member9 = @member9
                    , @member10 = @member10
                    , @member11 = @member11
                    , @member12 = @member12
                    , @member13 = @member13
                    , @member14 = @member14
                    , @member15 = @member15
                    , @member16 = @member16
                    , @unlockEdit = @unlockEdit";

                return query;
            }
        }

        public string GetIncomeLimitArea
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspaffordableincomelimitsarea]
                       @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID = @InternalUserID";

                return query;
            }
        }

        public string GetIncomeLimitDetails
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspaffordbleincomelimitdetails]
                       @InternalEntityID = @InternalEntityID
                    , @InternalSiteID = @InternalSiteID	
                    , @InternalUserID = @InternalUserID";

                return query;
            }
        }

        public string GetTaxCreditProgramType
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_usptaxcreditprogramspicklist]
                       @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }
        public string GetTaxCreditProgramTypeNames
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspgetprogramtype_programnames]
                       @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }
        public string GetTaxCreditPrograms
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspgettaxcreditprogramsdata]
                       @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string CreateUpdateProgram
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_uspinsertupdatetaxcreditprogramsdata]
                       @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID
                    , @ProgramID = @ProgramID
                    , @ProgramName = @ProgramName
                    , @IncomeLimitType = @IncomeLimitType
                    , @DefaultUtilityAllowance = @DefaultUtilityAllowance
                    , @StateIDNumber = @StateIDNumber
                    , @LIHCStudentRule = @LIHCStudentRule
                    , @UVR = @UVR
                    , @NAUR = @NAUR
                    , @SameBuildingTransfer = @SameBuildingTransfer
                    , @UVRBuildingScope = @UVRBuildingScope
                    , @UVROneForOnePenalty = @UVROneForOnePenalty
                    , @ComparableUnitByBedrooms = @ComparableUnitByBedrooms
                    , @ComparableUnitSqFtPercen = @ComparableUnitSqFtPercen
                    , @ProgramType = @ProgramType
                    , @NAURBuildingScope = @NAURBuildingScope
                    , @NAUROneForOnePenalty = @NAUROneForOnePenalty
                    , @userID = @userID
                    , @TcmrrtID = @TcmrrtID
                    , @Displaced = @Displaced
                    , @DisplacementTypeCode = @DisplacementTypeCode
                    , @GrossRentIncludesAssistancePayment = @GrossRentIncludesAssistancePayment
                    , @PercentageActual = @PercentageActual
                    , @ReportOnTICFlag = @ReportOnTICFlag
                    , @ReportOnTICFlagMarket = @ReportOnTICFlagMarket
                    , @TrackCompliancePerBuildingFlag = @TrackCompliancePerBuildingFlag
                    , @TrackProjectComplianceFlag = @TrackProjectComplianceFlag
                    , @DefineRequiredUnitsFlag = @DefineRequiredUnitsFlag
                    , @MinimumSetAsidePercentage = @MinimumSetAsidePercentage
                    , @IncludeExemptLIHTCUnitsFlag = @IncludeExemptLIHTCUnitsFlag
                    , @BondQualifiedProjectPeriodStartedFlag = @BondQualifiedProjectPeriodStartedFlag
                    , @BondQualifiedProjectStartDate = @BondQualifiedProjectStartDate
                    ,@HOMEStudentRule = @HOMEStudentRule
                    , @HomeRulesFlag = @HomeRulesFlag
                    , @HomeFixedFlag = @HomeFixedFlag
                    , @HomeBuildingWideFlag = @HomeBuildingWideFlag
                    , @LowHomeUnits = @LowHomeUnits
                    , @HighHomeUnits = @HighHomeUnits
                    , @HomeUseExpensesFlag = @HomeUseExpensesFlag
                    , @HomeUseExpensesIncludeOtherSAFlag = @HomeUseExpensesIncludeOtherSAFlag
                    , @HighOIPercentage = @HighOIPercentage
                    , @DoesNotRequireAnnualRecertFlag = @DoesNotRequireAnnualRecertFlag
                    , @IncomeMinimumSetAside = @IncomeMinimumSetAside
                    , @CustomUA = @CustomUA
                    , @SiteID = @SiteID
                    , @Federal10cOption = @Federal10cOption
                    , @Federal8bOption = @Federal8bOption
                    , @MaxRentIncludesAssistancePayment = @MaxRentIncludesAssistancePayment
                    , @UtilityAllowanceRequired = @UtilityAllowanceRequired
                    , @SelectedUATableID = @SelectedUATableID
                    , @ProgramTypeConditionsCategory = @ProgramTypeConditionsCategory
                    , @LIHTCExceptions = @LIHTCExceptions
                    , @STUDENTExceptions = @STUDENTExceptions
                    , @WavierOption = @WavierOption
                    , @RecertInclude = @RecertInclude
                    , @IncomeMinimumSetAsideTraditional = @IncomeMinimumSetAsideTraditional
                    , @definerequiredunitshomeflag = @definerequiredunitshomeflag
                    , @IncomeLimitTypeAHDP = @IncomeLimitTypeAHDP
                    , @OtherDescription = @OtherDescription
                    , @UnitDesignationHistoryStartDate = @UnitDesignationHistoryStartDate
                    , @UseFloorPlanMethodFlag = @UseFloorPlanMethodFlag
                    , @programLevelCalculationFlag = @programLevelCalculationFlag
                    , @ProgramNameMarket = @ProgramNameMarket
                    , @MinimumLowIncomeUnits = @MinimumLowIncomeUnits";

                return query;
            }
        }

        public string CreateUpdateProgramNoParams
        {
            get
            {
                string query = @"[dbo].[unitysettings_uspinsertupdatetaxcreditprogramsdata]";
                return query;
            }
        }
        public string ProgramFedral10c
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_usptaxcreditprogramfedral10cpicklist]
                       @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string ProgramFedral8b
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_usptaxcreditprogramfedral8bpicklist]
                       @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string GetProgramRuleScopes
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_usptaxcreditprogramfloatingunitspicklist]
                       @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }
        public string GetProgramViolationPenalty
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[UnitySettings_uspTaxCreditProgramViolationPenaltyPickList]
                       @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }
        public string GeStudentrules
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[UnitySettings_uspTaxCreditProgramLihtcHomeRulesPickList]
                       @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }
        public string Gethomestudentrules
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_taxcredit_programs_homestudentrules]
                       @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string GetNonLihtcStudentRules
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_taxcredit_programs_nonlihcstudentrules]
                       @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string GetTaxCreditProgramNamesPicklist
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC [dbo].[unitysettings_usptaxcreditprogramnamespicklist]
                       @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string SaveSetAsideTempDetails
        {
            get
            {
                string query = @"[dbo].[unitysettings_uspinsertsetasidetempdetails]";
                return query;
            }
        }

        public string GetSetAsideDetails
        {
            get
            {
                string query = @"
            SET NOCOUNT ON

            EXEC [dbo].[unitysettings_uspgetsetasidedetails]
                   @InternalEntityID = @InternalEntityID
                 , @InternalUserID = @InternalUserID
                 , @InternalSiteID = @InternalSiteID
                 , @setAsideID = DEFAULT";

                return query;
            }
        }
        public string GetRequiredUnitCountsTC
        {
            get
            {
                string query = @"
            SET NOCOUNT ON

            EXEC [dbo].[UnitySettings_uspenablerequiredunitcountsTC]
                   @InternalEntityID = @InternalEntityID
                 , @InternalUserID = @InternalUserID
                 , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string UpdateSetAsideDetails
        {
            get
            {
                string query = @"[dbo].[unitysettings_uspupdatesetasidedetails]";
                return query;
            }
        }
        public string SaveSetAsideRules
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC dbo.UnitySettings_uspAssignSetAsideRulesSave
                      @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID
                    , @SAName = @SAName
                    , @RelaltedSaName = @RelaltedSaName
                    , @SASRule = @SASRule";

                return query;
            }
        }

        public string GetAffordableSetAsideAssignedRules
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC dbo.UnitySettings_uspAffordableSetAsideAssinedRulesPL
                      @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string UpdateTaxCreditsAssignBuildingToPrograms
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC unitysettings_uspupdatetaxcreditsassignbuildingtoprograms
                      @InternalEntityID = @InternalEntityID
                    , @InternalUserId = @InternalUserId
                    , @InternalSiteId = @InternalSiteId
                    , @buildingID = @buildingID
                    , @programID = @programID
                    , @BIN = @BIN
                    , @exemptFlag = @exemptFlag
                    , @rentUpFlag = @rentUpFlag
                    , @serviceDate = @serviceDate
                    , @Post89Flag = @Post89Flag
                    , @electionDate = @electionDate
                    , @applicableFractionGoal = @applicableFractionGoal
                    , @applicableFraction = @applicableFraction
                    , @applicableFractionBySquarefoot = @applicableFractionBySquarefoot
                    , @buildingCompliancePercentageGoal = @buildingCompliancePercentageGoal
                    , @buildingComplianceUnitGoal = @buildingComplianceUnitGoal
                    , @userID = @userID
                    , @AcquisitionFlag = @AcquisitionFlag
                    , @SiteID = @SiteID
                    , @AllocationValue = @AllocationValue
                    , @IncludeInProgram = @IncludeInProgram
                    , @RecertificationWaiver = @RecertificationWaiver
                    , @UnitCount = @UnitCount
                    , @ProgramType = @ProgramType
                    , @RevokedDate = @RevokedDate
                    , @ApprovedDate = @ApprovedDate
                    , @serviceDateOn = @serviceDateOn";

                return query;
            }
        }
        public string UpdateBuildingsToProgramsWithNoParams
        {
            get
            {
                return @"dbo.unitysettings_uspupdatetaxcreditsassignbuildingtoprograms";
            }
        }
        public string GetBuildingsList
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC dbo.UnitySettings_uspGetBuildinsList
                      @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string GetSetAsideBuildingAssignments
        {
            get
            {
                string query = @"
            SET NOCOUNT ON

            EXEC [dbo].[unitysettings_uspgetsetasidebuildingassignments]
                 @InternalEntityID = @InternalEntityID
               , @InternalUserID = @InternalUserID
               , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string AddEditSetAsideBuildingAssignmentWithNoParams
        {
            get
            {
                return @"dbo.unitysettings_uspaddeditsetasidebuildingassignments";
            }
        }

        public string GetSetAsideUnitTypeAssignments
        {
            get
            {
                string query = @"
                SET NOCOUNT ON

                EXEC [dbo].[UnitySettings_uspAssignSetAsideUnitTypesGet]
                     @InternalEntityID = @InternalEntityID
                   , @InternalUserID = @InternalUserID
                   , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string AssignSetAsideFloorPlanUpdateNoParams
        {
            get
            {
                return @"dbo.unitysettings_uspassignsetasidefloorplanupdate";
            }
        }

        public string AssignSetAsideUnittypesUpdateNoParams
        {
            get
            {
                return @"dbo.unitysettings_uspassignsetasideunittypesupdate";
            }
        }

        public string GetSetAsideFloorPlanAssignments
        {
            get
            {
                string query = @"
                SET NOCOUNT ON

                EXEC [dbo].[UnitySettings_uspAssignSetAsideFloorPlanGet]
                     @InternalEntityID = @InternalEntityID
                   , @InternalUserID = @InternalUserID
                   , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string GetSetAsideRequiredUnitsByFloorPlan
        {
            get
            {
                string query = @"
            SET NOCOUNT ON

            EXEC [dbo].[UnitySettings_uspSetAsideRequiredUnitsByFloorPlanGet]
                 @InternalEntityID = @InternalEntityID
               , @InternalUserID = @InternalUserID
               , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string GetTCAllocationType
        {
            get
            {
                string query = @"
				SET NOCOUNT ON

				EXEC dbo.UnitySettings_uspGetTCAllocationType
                      @InternalEntityID = @InternalEntityID
                    , @InternalUserID = @InternalUserID
                    , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }


        public string AssignSetAsideRequiredUnitsByFloorPlanNoParams
        {
            get
            {
                return @"dbo.UnitySettings_uspSetAsideRequiredUnitsByFloorPlanSave";
            }
        }

        public string GetSetAsideRequiredUnitsByBedroomCount
        {
            get
            {
                string query = @"
            SET NOCOUNT ON

            EXEC [dbo].[UnitySettings_uspSetAsideRequiredUnitsByBedroomCountGet]
                 @InternalEntityID = @InternalEntityID
               , @InternalUserID = @InternalUserID
               , @InternalSiteID = @InternalSiteID";

                return query;
            }
        }

        public string SaveSetAsideRequiredUnitsByBedroomCountNoParams
        {
            get
            {
                return @"dbo.UnitySettings_uspSetAsideRequiredUnitsByBedroomCountSave";
            }
        }

        public string GetSetAsideHomeTypesQuery
        {
            get
            {
                string query = @"
                SET NOCOUNT ON

                EXEC [dbo].[UnitySettings_uspSetAside_HomeTypes_PL]
                     @InternalEntityID = @InternalEntityID
                   , @InternalSiteID = @InternalSiteID
                   , @InternalUserID = @InternalUserID";

                return query;
            }
        }

        public string SaveRentFloors
        {
            get
            {
                string query = @"
                SET NOCOUNT ON

                EXEC [dbo].[UnitySettings_uspRentFloorsSave]
                     @InternalEntityID = @InternalEntityID
                   , @InternalUserID = @InternalUserID
                   , @InternalSiteID = @InternalSiteID
                   , @SetAsideID = @SetAsideID
                   , @FloorPlanID = @FloorPlanID
                   , @Amount = @Amount";

                return query;
            }
        }
    }
}
