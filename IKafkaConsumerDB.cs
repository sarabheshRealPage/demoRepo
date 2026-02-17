using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealPage.OneSite.Affordable.DataHubLRTC.DBObjects.Interface
{
   public interface IKafkaConsumerDB
    {
        /// <summary>
        /// SaveKafkaEventSubscriptionDetail SQL
        /// </summary>
        string SaveKafkaEventPublishDetail { get; }

        /// <summary>
        /// SaveKafkaEventPublishDetailLog SQL
        /// </summary>
        string SaveKafkaEventPublishDetailLog { get; }

        /// <summary>
        /// GetSiteImplementationKafkaMessage SQL
        /// </summary>
        string GetSiteImplementationKafkaMessage { get; }

        /// <summary>
        /// SaveSiteImplementationKafkaMessage SQL
        /// </summary>
        string SaveSiteImplementationKafkaMessage { get; }

        /// <summary>
        /// SaveGeneralNeedsHotma SQL
        /// </summary>
        string SaveGeneralNeedsHotma { get; }
        /// <summary>
        /// 
        /// </summary>
        string SaveGeneralReqVerification { get; }
        /// <summary>
        /// 
        /// </summary>
        string SaveGeneralDefaultTic { get; }
        /// <summary>
        /// 
        /// </summary>
        string GetTenantIncomeCertificationsList { get; }
        /// <summary>
        /// 
        /// </summary>
        string SaveTcUnitType { get; }
        string GetTcUnitType { get; }
        string SaveTcHouseHoldType { get; }
        string GetTcHouseHoldType { get; }
        string GetTcHouseHoldGroupId { get; }
        string UpdateutilityAllowanceSource { get; }
        string UpdateutilityAllowanceDetails { get; }
        string GetUtilityAllowanceSource { get; }
        string GetUtilityAllowances { get; }
        string GetFlooplans { get; }
        string GetIncomeLimitType { get; }
        string GetIncomeLimits { get; }
        string CreateIncomeLimit { get; }
        string GetIncomeLimitArea { get; }
        string SaveIncomeLimitDetails { get; }
        string GetIncomeLimitDetails { get;}
        string GetTaxCreditProgramType { get; }
        string GetTaxCreditProgramTypeNames { get; }
        string GetTaxCreditPrograms { get; }
        /// <summary>
        /// CreateUpdateProgram SQL
        /// </summary>
        string CreateUpdateProgram { get; }
        string CreateUpdateProgramNoParams { get; }
        /// <summary>
        /// ProgramFedral10c SQL
        /// </summary>
        string ProgramFedral10c { get; }
        /// <summary>
        /// ProgramFedral8b SQL
        /// </summary>
        string ProgramFedral8b { get; }
        /// <summary>
        /// GetProgramRuleScopes SQL
        /// </summary>
        string GetProgramRuleScopes { get; }
        string GetProgramViolationPenalty { get; }
        string Gethomestudentrules { get; }
        string GeStudentrules { get; }
        string GetNonLihtcStudentRules { get; }
        string GetTaxCreditProgramNamesPicklist { get; }
        /// <summary>
        /// SaveSetAsideRules SQL
        /// </summary>
        string SaveSetAsideRules { get; }

        /// <summary>
        /// GetAffordableSetAsideAssignedRules SQL
        /// </summary>
        string GetAffordableSetAsideAssignedRules { get; }

        /// <summary>
        /// UpdateTaxCreditsAssignBuildingToPrograms SQL
        /// </summary>
        string UpdateTaxCreditsAssignBuildingToPrograms { get; }
        /// <summary>
        /// SaveSetAsideTempDetails SQL
        /// </summary>
        string SaveSetAsideTempDetails { get; }
        /// <summary>
        /// GetSetAsideDetails SQL
        /// </summary>
        string GetSetAsideDetails { get; }
        /// <summary>
        /// GetRequiredUnitCountsTC SQL
        /// </summary>
        string GetRequiredUnitCountsTC { get; }

        /// <summary>
        /// UpdateSetAsideDetails SQL
        /// </summary>
        string UpdateSetAsideDetails { get; }
        /// <summary>
        /// Update Buildings To Programs With No Params SQL
        /// </summary>
        string UpdateBuildingsToProgramsWithNoParams { get; }
        string GetBuildingsList { get; }
        string GetTCAllocationType { get; }

        /// <summary>
        /// Update Buildings To Programs With No Params SQL
        /// </summary>
        string GetSetAsideBuildingAssignments { get; }

        /// <summary>
        /// AddEditSetAsideBuildingAssignmentWithNoParams SQL
        /// </summary>

        string AddEditSetAsideBuildingAssignmentWithNoParams { get; }

        /// <summary>
        /// GetSetAsideUnitTypeAssignments SQL
        /// </summary>
        string GetSetAsideUnitTypeAssignments { get; }

        /// <summary>
        /// GetSetAsideUnitTypeAssignments SQL
        /// </summary>

        string AssignSetAsideFloorPlanUpdateNoParams { get; }

        /// <summary>
        /// AssignSetAsideUnittypesUpdateNoParams SQL
        /// </summary>

        string AssignSetAsideUnittypesUpdateNoParams { get; }
        /// <summary>
        /// GetSetAsideFloorPlanAssignments SQL
        /// </summary>
        string GetSetAsideFloorPlanAssignments { get; }

        /// <summary>
        /// GetSetAsideFloorPlanAssignments SQL
        /// </summary>

        string GetSetAsideRequiredUnitsByFloorPlan { get; }

        /// <summary>
        /// AssignSetAsideRequiredUnitsByFloorPlanNoParams SQL
        /// </summary>

        string AssignSetAsideRequiredUnitsByFloorPlanNoParams { get; }
        /// <summary>
        /// GetSetAsideRequiredUnitsByBedroomCount SQL
        /// </summary>
        string GetSetAsideRequiredUnitsByBedroomCount { get; }
        /// <summary>
        /// GetSetAsideRequiredUnitsByBedroomCount SQL
        /// </summary>
        string SaveSetAsideRequiredUnitsByBedroomCountNoParams { get; }
        /// <summary>
        /// GetSetAsideHomeTypesQuery SQL
        /// </summary>
        string GetSetAsideHomeTypesQuery { get; }

        /// <summary>
        /// SaveRentFloors SQL
        /// </summary>
        string SaveRentFloors { get; }

    }
}
