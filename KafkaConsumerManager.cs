using com.realpage.avro.implementation;
using Newtonsoft.Json;
using RealPage.Common.UPMBaseSetup.Base.DAO.Interface;
using RealPage.Common.UPMBaseSetup.Base.Logic.Class;
using RealPage.OneSite.Affordable.DataHubLRTC.BusinessLogic.Interface;
using RealPage.OneSite.Affordable.DataHubLRTC.BusinessObjects.Class;
using RealPage.OneSite.Affordable.DataHubLRTC.BusinessObjects.Enum;
using RealPage.OneSite.Affordable.DataHubLRTC.DAO.Interface;
using RealPage.OneSite.Affordable.DataHubLRTC.Utilities.Class;
using RealPage.OneSite.All.Common.Base.DAO;
using RealPage.OneSite.Common.Base;
using RealPage.OneSite.Common.BusinessLogic;
using RealPage.OneSite.Common.BusinessObjects;
using RealPage.OneSite.Common.CommonFunctions;
using RealPage.OneSite.Common.DAO;
using RealPage.OneSite.Data;
using RealPage.OneSite.FeatureFlags.LaunchDarkly.BusinessLogic.Class;
using RealPage.OneSite.Logging.BusinessLogic;
using RealPage.OneSite.Messaging.Kafka;
using RealPage.OneSite.Messaging.Kafka.DataHubImplementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using UPMModel = RealPage.Common.UPMBaseSetup.SetupModel;

namespace RealPage.OneSite.Affordable.DataHubLRTC.BusinessLogic.Class
{
    public class KafkaConsumerManager : UPMBaseManager, IKafkaConsumerManager
    {
        #region "Private Variables Declaration"
        /// <summary>
        /// The DAO
        /// </summary>

        private readonly IKafkaConsumerDAO _KafkaConsumerDAO;
        private static string _product_books = "KongBooksAPI";
        private int _pmcID;
        private int _siteID;
        private RealPage.OneSite.Environment _env = null;
        private readonly WebApiContext _context = null;
        CommonFunctions _cf = new CommonFunctions();
        private const string DH_SETASIDE_EXTRACT = "dh-to-os-setaside-extract";
        #endregion "Private Variables Declaration"

        #region "Constructor"

        /// <summary>
        /// Initializes a new instance of the <see cref="KafkaConsumerManager"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="dao">The DAO.</param>
        public KafkaConsumerManager(IWebApiContext context, IKafkaConsumerDAO dao) : base(context, string.Empty)
        {
            _KafkaConsumerDAO = dao;
            _context = (WebApiContext)context;
        }
        #endregion "Constructor"

        /// <summary>
        /// Processes the LRTC product input Request from Datahub
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<UPMModel.ApiResponse<string>> ProcessKafkaMessage(DHTCImplementationApplyValue message)
        {
            UPMModel.ApiResponse<string> response = null;

            if (message == null)
            {
                //AddApiError("Failed to Save LRTC Product Code Site Implementation Details");
                return new UPMModel.ApiResponse<string>("Error", 0, HttpStatusCode.OK, ErrorList);
            }
            ImplementationStatusValue implementationStatus = new ImplementationStatusValue()
            {
                implementation_uuid = message.implementation_uuid,
                status_datetime = DateTime.Now.ToString(),
                errors = new List<Error>(),
                message = ""
            };
            var siteMessage = PrepareSiteImplementationKafkaMessage(message);
            try
            {
                if (message != null)
                {
                    var existingRecord = await GetSiteImplementationKafkaMessage(siteMessage);
                    if (existingRecord != null && existingRecord.Status == ImplementationStatusType.SUCCESS.ToString())
                    {
                        return new UPMModel.ApiResponse<string>("This request is a duplicate", 1, HttpStatusCode.OK);
                    }
                    if (existingRecord.SiteImplementationKafkaMessageID > 0)
                    {
                        existingRecord.Message = siteMessage.Message;
                        siteMessage = existingRecord;

                    }
                    siteMessage = await SaveSiteImplementationKafkaMessage(siteMessage, implementationStatus);


                    switch (message.datahub_productcodes.FirstOrDefault())
                    {
                        case "LRTC":
                            implementationStatus = await ProcessLRTCMessageRequest(message);
                            break;
                    }

                }
                else
                {
                    // AddApiError("Failed to Save LRTC Product Code Site Implementation Details");
                    return new UPMModel.ApiResponse<string>("Error", 0, System.Net.HttpStatusCode.OK, ErrorList);
                }
                if (implementationStatus.status == ImplementationStatusType.SUCCESS)
                {
                    siteMessage.Status = ImplementationStatusType.SUCCESS.ToString();
                    siteMessage.StatusMessage = "Successfully processed LRTC";
                    siteMessage = await SaveSiteImplementationKafkaMessage(siteMessage, implementationStatus);
                    response = new UPMModel.ApiResponse<string>("Successfully processed", 1, System.Net.HttpStatusCode.OK);
                }
                else
                {
                    //  AddApiError("Failed to Save LRTC Product Code Site Implementation Details");
                    siteMessage.Status = ImplementationStatusType.ERROR.ToString();
                    siteMessage.StatusMessage = JsonConvert.SerializeObject(implementationStatus.errors);
                    if(string.IsNullOrWhiteSpace(implementationStatus.message))
                    {
                        implementationStatus.message = "Error while processing the LRTC Kafka message";
                    }
                    siteMessage = await SaveSiteImplementationKafkaMessage(siteMessage, implementationStatus);
                    return new UPMModel.ApiResponse<string>(ImplementationStatusType.ERROR.ToString(), 0, System.Net.HttpStatusCode.OK, ErrorList);
                }

            }
            catch (Exception ex)
            {
                siteMessage.Status = "FAIL";
                siteMessage.StatusMessage = ex.Message;
                implementationStatus.status = ImplementationStatusType.ERROR;
                implementationStatus.errors.Add(new Error { message = ex.Message,value="",name="" });
                implementationStatus.message = "Error while processing the LRTC Kafka message";
                siteMessage = await SaveSiteImplementationKafkaMessage(siteMessage, implementationStatus);
                return new UPMModel.ApiResponse<string>("Error", 0, System.Net.HttpStatusCode.OK, ErrorList);
            }
            return response;
        }

        private async Task<ImplementationStatusValue> ProcessLRTCMessageRequest(DHTCImplementationApplyValue dHTCImplementationApplyValue)
        {
            ImplementationStatusValue implementationStatusValue = new ImplementationStatusValue() { };
            implementationStatusValue.errors = new List<Error>();
            DHTCSiteImplementationRequest dHTCSiteImplementationRequest = new DHTCSiteImplementationRequest();
            General implementGenralObj = new General() { };
            List<UnitType> _unitTypes = new List<UnitType>() { };
            List<HouseholdType> _housholdTypes = new List<HouseholdType>() { };
            List<UtilityAllowanceSource> _utilityAllowanceSources = new List<UtilityAllowanceSource>() { };
            List<UtilityAllowance> _utilityAllowances = new List<UtilityAllowance>() { };
            List<Floorplan> _floorplans = new List<Floorplan>() { };
            List<IncomeLimit> _incomeLimitSources = new List<IncomeLimit>() { };
            List<IncomeLimitDetail> _incomeLimits = new List<IncomeLimitDetail>() { };
            List<Program> _programs = new List<Program>() { };
            List<SetAside> _setAside = new List<SetAside>();
            List<Building> _buildings = new List<Building>();
            List<SetAsideRule> _setAsideRules = new List<SetAsideRule>();
            List<RentFloor> _rentFloors = new List<RentFloor>();
            dHTCSiteImplementationRequest.PmcId = _pmcID;
            dHTCSiteImplementationRequest.SiteId = _siteID;
            ConnectionManager cm = InitiateDBConnection(DBEntity.Site, new IUPMBaseDAO[] { _KafkaConsumerDAO }, _siteID);
            var osAllBuildings = await GetOnesiteBudilingsInfo(dHTCSiteImplementationRequest);
            bool isSetAsideLDFlagEnabled = LaunchDarklyAccessor.IsEnabled(DH_SETASIDE_EXTRACT, new FlagContext(), Context);

            //SiteImplementationRequest Request = new SiteImplementationRequest();
            //Request.ImplementionObjects.Add(new Units() { Unittypes = _unitTypes });

            try
            {
                foreach (var genralInfoField in dHTCImplementationApplyValue.keys)
                {
                    switch (genralInfoField.name)
                    {
                        case "lrtc_formData__propertyDate":
                            break;

                        case "lrtc_formData__programTypes":
                            break;

                        case "lrtc_formData__unitCount":
                            break;

                        case "lrtc_formData__hasTics":
                            break;

                        case "lrtc_formData__hasMarketUnits":
                            break;

                        case "lrtc_formData__hasExemptUnits":
                            break;

                        case "lrtc_formData__requireVerifications":
                            implementGenralObj.RequireVerifications = !string.IsNullOrEmpty(genralInfoField.value) && Convert.ToBoolean(genralInfoField.value);
                            break;

                        case "lrtc_formData__defaultTic":
                            if (!string.IsNullOrEmpty(genralInfoField.value))
                                implementGenralObj.DefaultTic = (LrtcDefaultTics)short.Parse(genralInfoField.value);
                            break;

                        case "lrtc_formData__needsHotma":
                            implementGenralObj.NeedsHotma = !string.IsNullOrEmpty(genralInfoField.value) && Convert.ToBoolean(genralInfoField.value);
                            break;

                        case "lrtc_formData__useHouseholdTypes":
                            break;

                        case "lrtc_formData__useUnitTypes":
                            break;

                        case "lrtc_formData__useUtilityAllowances":
                            break;

                        case "lrtc_formData__useRentFloors":
                            implementGenralObj.UseRentFloors = !string.IsNullOrEmpty(genralInfoField.value) && Convert.ToBoolean(genralInfoField.value);
                            break;
                    }
                }
                dHTCSiteImplementationRequest.DHTCGeneralInfo = implementGenralObj;

                foreach (var table in dHTCImplementationApplyValue.tables)
                {
                    if (table.name == "lrtc_unitTypes")
                    {
                        foreach (var value in table.values)
                        {
                            foreach (var column in value.columns)
                            {
                                if (column.name == "lrtc_unitTypes__name" && !string.IsNullOrEmpty(column.value))
                                {
                                    _unitTypes.Add(new UnitType()
                                    {
                                        Id = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_unitTypes__id").value),
                                        Name = value.columns.FirstOrDefault(v => v.name == "lrtc_unitTypes__name").value
                                    });
                                }
                            }
                        }
                        dHTCSiteImplementationRequest.DHTCUnitTypes = _unitTypes;
                    }
                }
                foreach (var table in dHTCImplementationApplyValue.tables)
                {
                    if (table.name == "lrtc_householdTypes")
                    {
                        foreach (var value in table.values)
                        {
                            foreach (var column in value.columns)
                            {
                                if (column.name == "lrtc_householdTypes__name" && !string.IsNullOrEmpty(column.value))
                                {

                                    HouseholdType hhType = new HouseholdType()
                                    {
                                        Id = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_householdTypes__id").value),
                                        Name = value.columns.FirstOrDefault(v => v.name == "lrtc_householdTypes__name").value,
                                        GroupId = value.columns.FirstOrDefault(v => v.name == "lrtc_householdTypes__group").value,
                                    };
                                    hhType.Group = ((HouseholdCategory)short.Parse(hhType.GroupId)).ToString();
                                    _housholdTypes.Add(hhType);
                                }
                            }
                        }
                        dHTCSiteImplementationRequest.DHTCHouseholdTypes = _housholdTypes;
                    }
                }

                //floorplan look up from DH
                foreach (var table in dHTCImplementationApplyValue.tables)
                {
                    if (table.name == "lrtc_floorPlans")
                    {
                        foreach (var value in table.values)
                        {
                            foreach (var column in value.columns)
                            {
                                if (column.name == "lrtc_floorPlans__code" && !string.IsNullOrEmpty(column.value))
                                {

                                    Floorplan floorplan = new Floorplan()
                                    {
                                        FloorplanId = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_floorPlans__id").value),
                                        FloorplanCode = value.columns.FirstOrDefault(v => v.name == "lrtc_floorPlans__code").value,
                                        BedCount = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_floorPlans__bedCount").value),
                                        UnitCount = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_floorPlans__unitCount").value)
                                    };
                                    _floorplans.Add(floorplan);
                                }
                            }
                        }
                        dHTCSiteImplementationRequest.DHTCFloorplan = _floorplans;
                    }
                }
                // Process Buildings
                foreach (var table in dHTCImplementationApplyValue.tables)
                {
                    if (table.name == "lrtc_buildings")
                    {
                        foreach (var value in table.values)
                        {
                            foreach (var column in value.columns)
                            {
                                if (column.name == "lrtc_buildings__buildingNumber" && !string.IsNullOrEmpty(column.value))
                                {
                                    Building building = new Building()
                                    {
                                        Id = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__id")?.value),
                                        BuildingNumber = value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__buildingNumber")?.value ?? string.Empty,
                                        Description = value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__description")?.value ?? string.Empty,
                                        AddressLine1 = value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__addressLine1")?.value ?? string.Empty,
                                        AddressLine2 = value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__addressLine2")?.value ?? string.Empty,
                                        City = value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__city")?.value ?? string.Empty,
                                        State = value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__state")?.value ?? string.Empty,
                                        Zip = value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__zip")?.value ?? string.Empty,
                                        County = value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__county")?.value ?? string.Empty,
                                        RentUp = value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__rentUp")?.value ?? string.Empty,
                                        Bin = value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__bin")?.value ?? string.Empty,
                                        PlacedInServiceDate = SafeTypes.ToDateTime(value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__placedInServiceDate")?.value),
                                        MaxRentMethod = value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__maxRentMethod")?.value ?? string.Empty,
                                        MaxRentDate = SafeTypes.ToDateTime(value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__maxRentDate")?.value),
                                        ApplicableFraction = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__applicableFraction")?.value),
                                        TaxCreditAllocation = value.columns.FirstOrDefault(v => v.name == "lrtc_buildings__taxCreditAllocation")?.value
                                    };

                                    var buildingID = osAllBuildings.FirstOrDefault(b => b.OnesiteBuildingNumber == building.BuildingNumber)?.Id ?? 0;
                                    if (buildingID == 0)
                                    {
                                        throw new Exception("The Building Number - "+ building.BuildingNumber + " could not be located in OneSite" );
                                    }
                                    else
                                    {
                                        building.OnesiteBuildingID = buildingID;
                                    }
                                    _buildings.Add(building);
                                }

                            }
                        }
                        dHTCSiteImplementationRequest.DHTCBuildings = _buildings;
                    }
                }
                // Process Utility Allowance Sources
                foreach (var table in dHTCImplementationApplyValue.tables)
                {
                    if (table.name == "lrtc_utilityAllowanceSources")
                    {
                        foreach (var value in table.values)
                        {

                            var nameColumn = value.columns.FirstOrDefault(v => v.name == "lrtc_utilityAllowanceSources__name");
                            if (nameColumn != null && !string.IsNullOrEmpty(nameColumn.value))
                            {
                                var utilityAllowanceSource = new UtilityAllowanceSource
                                {
                                    Id = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_utilityAllowanceSources__id").value),
                                    Name = nameColumn?.value ?? string.Empty,
                                    EffectiveDate = SafeTypes.ToDateTime(value.columns.FirstOrDefault(v => v.name == "lrtc_utilityAllowanceSources__effectiveDate").value)

                                };

                                _utilityAllowanceSources.Add(utilityAllowanceSource);
                            }
                        }
                        dHTCSiteImplementationRequest.DHTCUtilityAllowanceSources = _utilityAllowanceSources;
                    }
                }

                // Process Utility Allowances
                foreach (var table in dHTCImplementationApplyValue.tables)
                {
                    if (table.name == "lrtc_utilityAllowances")
                    {
                        foreach (var value in table.values)
                        {
                            foreach (var column in value.columns)
                            {
                                if (column.name == "lrtc_utilityAllowances__amount" && !string.IsNullOrEmpty(column.value))
                                {
                                    var fpId = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_utilityAllowances__floorPlanId").value);
                                    var uaSourceID = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_utilityAllowances__utilityAllowanceSourceId").value);
                                    string fpName = dHTCSiteImplementationRequest.DHTCFloorplan.FirstOrDefault(f => f.FloorplanId == fpId)?.FloorplanCode;
                                    string uaSourceName = dHTCSiteImplementationRequest.DHTCUtilityAllowanceSources.FirstOrDefault(u => u.Id == uaSourceID)?.Name;
                                    var uaSourceDate = dHTCSiteImplementationRequest.DHTCUtilityAllowanceSources.FirstOrDefault(u => u.Id == uaSourceID)?.EffectiveDate;

                                    UtilityAllowance utilityAllowance = new UtilityAllowance()
                                    {
                                        Id = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_utilityAllowances__id").value),
                                        FloorPlanId = fpId,
                                        UtilityAllowanceSourceId = uaSourceID,
                                        Amount = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_utilityAllowances__amount").value),
                                        FloorPlanCode = fpName,
                                        UtilityAllowanceSourceName = uaSourceName,
                                        uaSourceEffectiveDate = uaSourceDate
                                    };
                                    _utilityAllowances.Add(utilityAllowance);
                                }
                            }
                        }
                        dHTCSiteImplementationRequest.DHTCUtilityAllowances = _utilityAllowances;
                    }
                }

                // Process Income Limit Sources
                foreach (var table in dHTCImplementationApplyValue.tables)
                {
                    if (table.name == "lrtc_incomeLimitSources")
                    {
                        foreach (var value in table.values)
                        {
                            foreach (var column in value.columns)
                            {
                                if (column.name == "lrtc_incomeLimitSources__id" && !string.IsNullOrEmpty(column.value))
                                {
                                    var dateCoulmn = value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimitSources__effectiveDate");
                                    var typeCoulmn = value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimitSources__incomeLimitType");
                                    var countyValue = value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimitSources__county")?.value ?? string.Empty;
                                    IncomeLimit incomeLimitSource = new IncomeLimit()
                                    {
                                        Id = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimitSources__id")?.value ?? "0"),
                                        IncomeLimitType = (IncomeLimitType)short.Parse(typeCoulmn?.value ?? "0"),
                                        OtherType = value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimitSources__otherType")?.value ?? string.Empty,
                                        EffectiveDate = !string.IsNullOrEmpty(dateCoulmn?.value) ? SafeTypes.ToDateTime(dateCoulmn.value) : (DateTime?)null,
                                        County = countyValue?.Replace(" ", ""),
                                        ExpandPersons = value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimitSources__expandPersons")?.value ?? string.Empty
                                    };
                                    _incomeLimitSources.Add(incomeLimitSource);
                                }
                            }
                        }
                        dHTCSiteImplementationRequest.DHTCIncomeLimits = _incomeLimitSources;
                    }
                }

                // Process Income Limits
                foreach (var table in dHTCImplementationApplyValue.tables)
                {
                    if (table.name == "lrtc_incomeLimits")
                    {
                        foreach (var value in table.values)
                        {
                            foreach (var column in value.columns)
                            {
                                if (column.name == "lrtc_incomeLimits__id" && !string.IsNullOrEmpty(column.value))
                                {
                                    var sourceIdColumn = value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__incomeLimitSourceId");
                                    var sourceId = SafeTypes.ToInt(sourceIdColumn?.value ?? "0");

                                    // Look up source name from previously processed income limit sources
                                    string sourceName = string.Empty;
                                    if (dHTCSiteImplementationRequest.DHTCIncomeLimits != null && dHTCSiteImplementationRequest.DHTCIncomeLimits.Any())
                                    {
                                        var sourceRecord = dHTCSiteImplementationRequest.DHTCIncomeLimits.FirstOrDefault(s => s.Id == sourceId);
                                        sourceName = !string.IsNullOrEmpty(sourceRecord?.OtherType) ? sourceRecord.OtherType.Trim() : sourceRecord?.IncomeLimitType.ToString();
                                    }

                                    IncomeLimitDetail incomeLimit = new IncomeLimitDetail()
                                    {
                                        Id = Convert.ToInt32(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__id")?.value ?? "0"),
                                        IncomeLimitSourceId = sourceId,
                                        IncomeLimitSourceName = sourceName,
                                        PercentageLimit = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__percentageLimit")?.value),
                                        OnePerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__onePerson")?.value),
                                        TwoPerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__twoPerson")?.value),
                                        ThreePerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__threePerson")?.value),
                                        FourPerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__fourPerson")?.value),
                                        FivePerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__fivePerson")?.value),
                                        SixPerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__sixPerson")?.value),
                                        SevenPerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__sevenPerson")?.value),
                                        EightPerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__eightPerson")?.value),
                                        NinePerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__ninePerson")?.value),
                                        TenPerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__tenPerson")?.value),
                                        ElevenPerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__elevenPerson")?.value),
                                        TwelvePerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__twelvePerson")?.value),
                                        ThirteenPerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__thirteenPerson")?.value),
                                        FourteenPerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__fourteenPerson")?.value),
                                        FifteenPerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__fifteenPerson")?.value),
                                        SixteenPerson = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_incomeLimits__sixteenPerson")?.value)
                                    };
                                    _incomeLimits.Add(incomeLimit);
                                }
                            }
                        }
                        dHTCSiteImplementationRequest.DHTCIncomeLimitDetails = _incomeLimits;
                    }
                }

                // Process Programs
                foreach (var table in dHTCImplementationApplyValue.tables)
                {
                    if (table.name == "lrtc_programs")
                    {
                        foreach (var value in table.values)
                        {
                            foreach (var column in value.columns)
                            {
                                if (column.name == "lrtc_programs__name" && !string.IsNullOrEmpty(column.value))
                                {
                                    Program program = new Program()
                                    {
                                        Id = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__id")?.value),
                                        ProgramType = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__programType")?.value),
                                        OtherProgramType = value.columns.FirstOrDefault(v => v.name == "lrtc_programs__otherProgramType")?.value ?? string.Empty,
                                        Name = value.columns.FirstOrDefault(v => v.name == "lrtc_programs__name")?.value ?? string.Empty,
                                        StateIdentifier = value.columns.FirstOrDefault(v => v.name == "lrtc_programs__stateIdentifier")?.value ?? string.Empty,
                                        IncomeLimitSourceId = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__incomeLimitSourceId")?.value),
                                        UtilityAllowanceSourceIds = value.columns.FirstOrDefault(v => v.name == "lrtc_programs__utilityAllowanceSourceIds")?.value ?? string.Empty,
                                        UtilityAllowanceSourceId = value.columns.FirstOrDefault(v => v.name == "lrtc_programs__utilityAllowanceSourceId")?.value ?? string.Empty,
                                        Lrtc10cElection = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__lrtc10cElection")?.value),
                                        DesignationStartDate = SafeTypes.ToDateTime(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__designationStartDate")?.value),
                                        MinimumUnitsPercentage = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__minimumUnitsPercentage")?.value),
                                        MinimumSetAsidePercentage = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__minimumSetAsidePercentage")?.value),
                                        Lrtc8bElection = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__lrtc8bElection")?.value),
                                        BuildingIds = value.columns.FirstOrDefault(v => v.name == "lrtc_programs__buildingIds")?.value ?? string.Empty,
                                        ApplyBuildingTransferRule = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__applyBuildingTransferRule")?.value),
                                        ApplyUvr = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__applyUvr")?.value),
                                        UvrScope = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__uvrScope")?.value),
                                        UvrViolation = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__uvrViolation")?.value),
                                        UvrComparableUnit = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__uvrComparableUnit")?.value),
                                        UvrUnitLargerPercentage = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__uvrUnitLargerPercentage")?.value),
                                        ApplyNaur = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__applyNaur")?.value),
                                        NaurScope = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__naurScope")?.value),
                                        NaurViolation = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__naurViolation")?.value),
                                        ApplyHome = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__applyHome")?.value),
                                        HomeRuleUnitVariance = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__homeRuleUnitVariance")?.value),
                                        LowHomeUnitCount = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__lowHomeUnitCount")?.value),
                                        HighHomeUnitCount = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__highHomeUnitCount")?.value),
                                        AdjustedIncomeOrExpensesType = value.columns.FirstOrDefault(v => v.name == "lrtc_programs__adjustedIncomeOrExpensesType")?.value ?? string.Empty,
                                        LevelOverIncome = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__levelOverIncome")?.value),
                                        ApplyStudent = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__applyStudent")?.value),
                                        StudentRuleType = value.columns.FirstOrDefault(v => v.name == "lrtc_programs__studentRuleType")?.value ?? string.Empty,
                                        IsFilingJointTaxReturn = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__isFilingJointTaxReturn")?.value),
                                        IsSingleParent = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__isSingleParent")?.value),
                                        IsReceivingAfdc = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__isReceivingAfdc")?.value),
                                        IsEnrolledInJobTraining = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__isEnrolledInJobTraining")?.value),
                                        IsOtherException = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__isOtherException")?.value),
                                        IsFosterCare = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__isFosterCare")?.value),
                                        IsExtendedUse = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__isExtendedUse")?.value),
                                        IsFinancialAid = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__isFinancialAid")?.value),
                                        IsVulnerableYouth = SafeTypes.ToBool(value.columns.FirstOrDefault(v => v.name == "lrtc_programs__isVulnerableYouth")?.value)
                                    };
                                    program.ProgramTypeName = Enum.GetName(typeof(ProgramTypeEnum), program.ProgramType);
                                    if(!string.IsNullOrEmpty(program.OtherProgramType))
                                    program.OtherProgramTypeName =  Enum.GetName(typeof(OtherProgramTypeEnum), SafeTypes.ToInt(program.OtherProgramType));

                                    //get Selected income limit name
                                    var incomelimit = dHTCSiteImplementationRequest.DHTCIncomeLimits?.FirstOrDefault(i => i.Id == program.IncomeLimitSourceId);
                                    if(incomelimit != null)
                                    program.IncomeLimitSourceName = incomelimit.IncomeLimitType.ToString() == "Other" ? incomelimit.OtherType : incomelimit.IncomeLimitType.ToString();
                                    //get UA name
                                   program.DefaultUASourceName = dHTCSiteImplementationRequest.DHTCUtilityAllowanceSources?.FirstOrDefault(u => u.Id == SafeTypes.ToInt(program.UtilityAllowanceSourceId))?.Name;

                                    //if (string.IsNullOrEmpty(program.DefaultUASourceName))
                                    //{
                                    //    throw new Exception("Unable to find default utility allownace source Id for the program" + program.Name);
                                    //}
                                    if (program.ProgramType == 0)
                                    {
                                        throw new Exception("Invalid program type for program" + program.Name);
                                    }
                                    _programs.Add(program);
                                }

                            }
                        }
                        dHTCSiteImplementationRequest.DHTCPrograms = _programs;
                    }
                }
                if(isSetAsideLDFlagEnabled)
                { 
                // Process Set Asides
                foreach (var table in dHTCImplementationApplyValue.tables)
                {
                    if (table.name == "lrtc_setAsides")
                    {
                        foreach (var value in table.values)
                        {
                            var columns = value.columns;
                            SetAside setAside = new SetAside
                            {
                                //General Settings
                                SetAsideId = SafeTypes.TryInt(columns, "lrtc_setAsides__id"),
                                ProgramId = SafeTypes.TryInt(columns, "lrtc_setAsides__programId"),
                                ShortName = SafeTypes.TryString(columns, "lrtc_setAsides__shortName"),
                                Name = SafeTypes.TryString(columns, "lrtc_setAsides__name"),
                                StartDate = SafeTypes.TryDate(columns, "lrtc_setAsides__startDate"),
                                HomeSetAsideType = SafeTypes.TryString(columns, "lrtc_setAsides__homeSetAsideType"),
                                // Set-aside Population
                                UnitCount = SafeTypes.TryInt(columns, "lrtc_setAsides__unitCount"),
                                PopulationPercentage = SafeTypes.TryDecimal(columns, "lrtc_setAsides__populationPercentage"),
                                PopulationRestriction = SafeTypes.TryString(columns, "lrtc_setAsides__populationRestriction"),
                                BuildingIds = SafeTypes.TryString(columns, "lrtc_setAsides__buildingIds"),
                                UnitTypeIds = SafeTypes.TryString(columns, "lrtc_setAsides__unitTypeIds"),
                                FloorPlanIds = SafeTypes.TryString(columns, "lrtc_setAsides__floorPlanIds"),
                                UnitsByFloorPlan = SafeTypes.TryString(columns, "lrtc_setAsides__unitsByFloorPlan"),
                                UnitsByBedroom = SafeTypes.TryString(columns, "lrtc_setAsides__unitsByBedroom"),
                                //Income Restriction at Move-in
                                IsIncomeRelated = SafeTypes.TryBool(columns, "lrtc_setAsides__isIncomeRelated"),
                                OverIncomePercentage = SafeTypes.TryDecimal(columns, "lrtc_setAsides__overIncomePercentage"),
                                OverMedianIncome = SafeTypes.TryDecimal(columns, "lrtc_setAsides__overMedianIncome"),
                                //Rent Restriction
                                MaxIncomeMedianPercentage = SafeTypes.TryDecimal(columns, "lrtc_setAsides__maxIncomeMedianPercentage"),
                                MaxIncomeAnnualAmount = SafeTypes.TryDecimal(columns, "lrtc_setAsides__maxIncomeAnnualAmount"),
                                MaxRentDetermination = SafeTypes.TryString(columns, "lrtc_setAsides__maxRentDetermination"),
                                MaxRentPercentage = SafeTypes.TryDecimal(columns, "lrtc_setAsides__maxRentPercentage"),
                                MaxRentMedianPercentage = SafeTypes.TryDecimal(columns, "lrtc_setAsides__maxRentMedianPercentage"),
                                //Additional Restrictions
                                HasHouseholdTypeRestriction = SafeTypes.TryBool(columns, "lrtc_setAsides__hasHouseholdTypeRestriction"),
                                HouseholdTypeIds = SafeTypes.TryString(columns, "lrtc_setAsides__householdTypeIds"),
                                HasHouseholdSizeRestriction = SafeTypes.TryBool(columns, "lrtc_setAsides__hasHouseholdSizeRestriction"),
                                MinimumMembersCount = SafeTypes.TryInt(columns, "lrtc_setAsides__minimumMembersCount"),
                                MaximumMembersCount = SafeTypes.TryInt(columns, "lrtc_setAsides__maximumMembersCount"),
                                HasAgeRestriction = SafeTypes.TryBool(columns, "lrtc_setAsides__hasAgeRestriction"),
                                AgeBasedRequirement = SafeTypes.TryString(columns, "lrtc_setAsides__ageBasedRequirement"),
                                HouseholdRelationships = SafeTypes.TryString(columns, "lrtc_setAsides__householdRelationships"),
                                MemberAge = SafeTypes.TryInt(columns, "lrtc_setAsides__memberAge"),
                                MemberAgeComparison = SafeTypes.TryString(columns, "lrtc_setAsides__memberAgeComparison"),
                                HasAdditionalAgeRestriction = SafeTypes.TryBool(columns, "lrtc_setAsides__hasAdditionalAgeRestriction"),
                                AdditionalAgeBasedRequirement = SafeTypes.TryString(columns, "lrtc_setAsides__additionalAgeBasedRequirement"),
                                AdditionalHouseholdRelationships = SafeTypes.TryString(columns, "lrtc_setAsides__additionalHouseholdRelationships"),
                                AdditionalMemberAge = SafeTypes.TryInt(columns, "lrtc_setAsides__additionalMemberAge"),
                                AdditionalMemberAgeComparison = SafeTypes.TryString(columns, "lrtc_setAsides__additionalMemberAgeComparison"),
                                HasMinimumIncomeRestriction = SafeTypes.TryBool(columns, "lrtc_setAsides__hasMinimumIncomeRestriction"),
                                MinimumIncomeMedianPercentage = SafeTypes.TryDecimal(columns, "lrtc_setAsides__minimumIncomeMedianPercentage"),
                                MinimumIncomeAmount = SafeTypes.TryDecimal(columns, "lrtc_setAsides__minimumIncomeAmount"),
                                MinimumIncomeRentMultiplier = SafeTypes.TryDecimal(columns, "lrtc_setAsides__minimumIncomeRentMultiplier")
                            };

                            _setAside.Add(setAside);
                            dHTCSiteImplementationRequest.DHTCSetAsides = _setAside;
                        }
                    }
                }

                // Process Set Aside Rules
                foreach (var table in dHTCImplementationApplyValue.tables)
                {
                    if (table.name == "lrtc_setAsideRules")
                    {
                        foreach (var value in table.values)
                        {
                            var columns = value.columns;
                            var rule = new SetAsideRule
                            {
                                Id = SafeTypes.TryInt(columns, "lrtc_setAsideRules__id"),
                                PrimarySetAsideId = SafeTypes.TryInt(columns, "lrtc_setAsideRules__primarySetAsideId"),
                                Relationship = SafeTypes.TryString(columns, "lrtc_setAsideRules__relationship"),
                                SecondarySetAsideId = SafeTypes.TryInt(columns, "lrtc_setAsideRules__secondarySetAsideId")
                            };

                            if (rule.PrimarySetAsideId.HasValue && rule.SecondarySetAsideId.HasValue && !string.IsNullOrEmpty(rule.Relationship))
                            {
                                rule.PrimarySetAsideName = dHTCSiteImplementationRequest.DHTCSetAsides.FirstOrDefault(s => s.SetAsideId == rule.PrimarySetAsideId)
                                    ?.ShortName;
                                rule.SecondarySetAsideName = dHTCSiteImplementationRequest.DHTCSetAsides.FirstOrDefault(s => s.SetAsideId == rule.SecondarySetAsideId)
                                    ?.ShortName;
                                _setAsideRules.Add(rule);
                            }
                        }
                        dHTCSiteImplementationRequest.DHTCSetAsideRules = _setAsideRules;
                    }
                }
                }

                // Process Rent Floors
                foreach (var table in dHTCImplementationApplyValue.tables)
                {
                    if (table.name == "lrtc_rentFloors")
                    {
                        foreach (var value in table.values)
                        {
                            foreach (var column in value.columns)
                            {
                                if (column.name == "lrtc_rentFloors__amount" && !string.IsNullOrEmpty(column.value))
                                {
                                    var floorPlanId = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_rentFloors__floorPlanId")?.value);
                                    var setAsideId = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_rentFloors__setAsideId")?.value);
                                    
                                    // Look up floor plan code from previously processed floor plans
                                    string floorPlanCode = string.Empty;
                                    if (dHTCSiteImplementationRequest.DHTCFloorplan != null && dHTCSiteImplementationRequest.DHTCFloorplan.Any())
                                    {
                                        var floorPlanRecord = dHTCSiteImplementationRequest.DHTCFloorplan.FirstOrDefault(f => f.FloorplanId == floorPlanId);
                                        floorPlanCode = floorPlanRecord?.FloorplanCode ?? string.Empty;
                                    }

                                    // Look up set aside name from previously processed set asides
                                    string setAsideName = string.Empty;
                                    string setAsideShortName = string.Empty;
                                    if (dHTCSiteImplementationRequest.DHTCSetAsides != null && dHTCSiteImplementationRequest.DHTCSetAsides.Any())
                                    {
                                        var setAsideRecord = dHTCSiteImplementationRequest.DHTCSetAsides.FirstOrDefault(s => s.SetAsideId == setAsideId);
                                        setAsideName = setAsideRecord?.Name ?? string.Empty;
                                        setAsideShortName = setAsideRecord?.ShortName ?? string.Empty;
                                    }

                                    RentFloor rentFloor = new RentFloor()
                                    {
                                        Id = SafeTypes.ToInt(value.columns.FirstOrDefault(v => v.name == "lrtc_rentFloors__id")?.value),
                                        FloorPlanId = floorPlanId,
                                        FloorPlanCode = floorPlanCode,
                                        SetAsideId = setAsideId,
                                        SetAsideName = setAsideName,
                                        SetAsideShortName = setAsideShortName,
                                        Amount = SafeTypes.ToDecimal(value.columns.FirstOrDefault(v => v.name == "lrtc_rentFloors__amount")?.value)
                                    };
                                    _rentFloors.Add(rentFloor);
                                }
                            }
                        }
                        dHTCSiteImplementationRequest.DHTCRentFloors = _rentFloors;
                    }
                }

                implementationStatusValue = _KafkaConsumerDAO.SaveDHTCSiteImplementationRequest(cm.GetConnection(), dHTCSiteImplementationRequest, _pmcID, _siteID);

                foreach (var error in implementationStatusValue.errors)
                {
                    if (string.IsNullOrWhiteSpace(error.name))
                        error.name = ""; 

                    if (string.IsNullOrWhiteSpace(error.value))
                        error.value = "";
                    if (string.IsNullOrWhiteSpace(error.message))
                        error.message = "Error while Processing request - ProcessLRTCMessageRequest";
                }
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                implementationStatusValue.status = ImplementationStatusType.ERROR;
                implementationStatusValue.message = "Error while Processing request - ProcessLRTCMessageRequest";
                implementationStatusValue.errors.Add(new Error() { message = ex.ToString(), value = "Error while Processing request - ProcessLRTCMessageRequest",name= "ProcessLRTCMessageRequest" });
            }
            finally
            {
                DisconnectDBConnection(cm);
            }
            return implementationStatusValue;
        }

        private async Task<SiteImplementationKafkaMessage> GetSiteImplementationKafkaMessage(SiteImplementationKafkaMessage message)
        {
            ConnectionManager cm = InitiateDBConnection(DBEntity.PMC, message.PMCID, new IUPMBaseDAO[] { _KafkaConsumerDAO });
            try
            {
                var msg = await _KafkaConsumerDAO.GetSiteImplementationKafkaMessage(cm.GetConnection(), message.ImplementationUUID, message.PMCID);
                return msg;
            }
            finally
            {
                DisconnectDBConnection(cm);
            }
        }
        private async Task<List<Building>> GetOnesiteBudilingsInfo(DHTCSiteImplementationRequest request)
        {
            ConnectionManager cm = InitiateDBConnection(DBEntity.Site, new IUPMBaseDAO[] { _KafkaConsumerDAO }, request.SiteId);
            try
            {
                return await _KafkaConsumerDAO.GetOnesiteBuilidings(cm.GetConnection(), request);
            }
            finally
            {
                DisconnectDBConnection(cm);
            }
        }

        private async Task<SiteImplementationKafkaMessage> SaveSiteImplementationKafkaMessage(SiteImplementationKafkaMessage message, ImplementationStatusValue implementationStatus)
        {
            ConnectionManager cm = InitiateDBConnection(DBEntity.PMC, message.PMCID, new IUPMBaseDAO[] { _KafkaConsumerDAO });
            try
            {
                message = await _KafkaConsumerDAO.SaveSiteImplementationKafkaMessage(cm.GetConnection(), message);
                if(!string.IsNullOrWhiteSpace(message.ImplementationUUID))
                {
                    implementationStatus.implementation_uuid = Guid.Parse(message.ImplementationUUID);
                }
                implementationStatus.status_datetime = DateTime.UtcNow.ToString();
                PublishImplementationStatusEvents(_context, _pmcID, _siteID, implementationStatus);
                return message;
            }
            finally
            {
                DisconnectDBConnection(cm);
            }
        }

        private SiteImplementationKafkaMessage PrepareSiteImplementationKafkaMessage(DHTCImplementationApplyValue message)
        {
            SiteImplementationKafkaMessage siteImplementationKafkaMessage = new SiteImplementationKafkaMessage();
            siteImplementationKafkaMessage.ImplementationUUID = message.implementation_uuid.ToString();
            siteImplementationKafkaMessage.Message = JsonConvert.SerializeObject(message);
            siteImplementationKafkaMessage.ProductCode = message.datahub_productcodes[0];
            siteImplementationKafkaMessage.Status = ImplementationStatusType.INPROGRESS.ToString();
            siteImplementationKafkaMessage.StatusMessage = "INPROGRESS";
            siteImplementationKafkaMessage.ModifiedBy = 1;
            siteImplementationKafkaMessage.CreatedBy = 1;
            var booksCompanyInstance = GetCompanyInstance(message.company_id, message.source_id);
            if (booksCompanyInstance != null)
            {
                siteImplementationKafkaMessage.PMCID = _pmcID = Convert.ToInt32(booksCompanyInstance.data.attributes.translatedCompanyInstances[0].companyInstanceSourceId);
            }
            else
            {
                siteImplementationKafkaMessage.StatusMessage = "company_id not found in books";
            }
            var booksPropertyInstance = GetPropertyInstance(message.property_id, message.source_id);
            if (booksPropertyInstance != null)
            {
                siteImplementationKafkaMessage.SiteID = _siteID = Convert.ToInt32(booksPropertyInstance.data.attributes.translatedPropertyInstances[0].propertyInstanceSourceId);
            }
            else
            {
                siteImplementationKafkaMessage.StatusMessage = "property_id not found in books ";
            }
            return siteImplementationKafkaMessage;

        }

        private T GetBooksInstance<T>(string id, string sourceCode, string endpoint) where T : class
        {
            HttpResponseMessage response = GetClientRequest(endpoint, id, sourceCode.ToUpperInvariant());

            if (response != null && response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(json);
            }
            return null;
        }

        private BooksCompanyInstance GetCompanyInstance(string companyId, string sourceCode)
        {
            return GetBooksInstance<BooksCompanyInstance>(companyId, sourceCode, "translate/v2/companyinstance");
        }

        private BooksPropertyInstance GetPropertyInstance(string propertyId, string sourceCode)
        {
            return GetBooksInstance<BooksPropertyInstance>(propertyId, sourceCode, "translate/v2/propertyinstance");
        }

        private void GetAPIKey(out string envUrl, out string apiKey, string productName, string environmentCode)
        {
            environmentCode = (environmentCode == "demo" || environmentCode == "training") ? "prod" : environmentCode;
            if (environmentCode == "preprod")
            {
                environmentCode = "ocrt";
            }

            WebApiContext context = (WebApiContext)this.Context;
            IKongEnvironmentDAO kongDao = new KongEnvironmentDAO(context);
            IKongEnvironmentManager kongManager = new KongEnvironmentManager(new ConnectionManager(), kongDao, context);

            IKongEnvironment env = kongManager.GetKongAPIKey(productName, environmentCode.ToUpperInvariant());
            envUrl = env.environmentURL;
            apiKey = Encoding.UTF8.GetString(Convert.FromBase64String(env.apiKey));
        }

        private string GetEnvironment()
        {
            RealPage.OneSite.IEnvironmentData envData = Env["AppEnvironment"] ?? Env["appenvironment"];

            if (envData == null)
            {
                throw new ApplicationException("AppEnvironment entry could not be determined.");
            }

            return envData.Value1.Trim().ToUpperInvariant();
        }

        private HttpResponseMessage GetClientRequest(string endpoint, string id, string sourceCode)
        {
            string envUrl, apiKey;
            GetAPIKey(out envUrl, out apiKey, _product_books, GetEnvironment());

            if (!string.IsNullOrWhiteSpace(envUrl) && !string.IsNullOrWhiteSpace(apiKey))
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("apikey", apiKey);

                    string requestUrl = string.Format("{0}{1}/{2}/{3}/OS", envUrl, endpoint, id, sourceCode);
                    var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                    return client.SendAsync(httpRequest).Result;
                }
            }

            return null;
        }

        private void PublishImplementationStatusEvents(WebApiContext context, int companyID, int siteID, ImplementationStatusValue implementationStatusValue)
        {
            long eventPublishDetailID = 0, eventPublishDetailLogID = 0;
            ConnectionManager cm = InitiateDBConnection(DBEntity.PMC, _pmcID, new IUPMBaseDAO[] { _KafkaConsumerDAO });
            try
            {
                ObjectCache memoryCache = MemoryCache.Default;
                ManageEnvironment manageEnvironment = new ManageEnvironment();
                _env = manageEnvironment.GetEnv();
                bool isEU = isEUEnv();
                string topicName = isEU ? "RPEU-implementation-status" : "implementation-status";
                string kafkaClassName = isEU ? "KafkaEnvironment" : "CloudKafkaEnvironment";

                IKafkaConfiguration kafkaEnvironment = _cf.GetKafkaConfiguration(context, memoryCache, _env, topicName, kafkaClassName);
                if (kafkaEnvironment != null)
                {
                    ConfluentProducerOptions producerOptions = new ConfluentProducerOptions
                    {
                        BootstrapServers = kafkaEnvironment.BrokerBootstrapServers,
                        SchemaRegistryURL = kafkaEnvironment.SchemaRegistryURL,
                        AutoRegisterSchemas = kafkaEnvironment.AutoRegisterSchemas,
                        BufferBytes = kafkaEnvironment.BufferBytes,
                        UseSSL = kafkaEnvironment.UseSSL,
                        Debug = kafkaEnvironment.Debug,
                        TopicName = GetTopicName(),// kafkaEnvironment.TopicName,
                        CloudTarget = kafkaEnvironment.CloudTarget,
                        SchemaRegistrykey = kafkaEnvironment.SchemaRegistrykey,
                        SchemaRegistrySecret = kafkaEnvironment.SchemaRegistrySecret,
                        BootstrapUserName = kafkaEnvironment.BootstrapUserName,
                        BootstrapPassWord = kafkaEnvironment.BootstrapPassWord
                    };
                    ImplementationStatusEventProducer<string, ImplementationStatusValue> implementationStatusEventProducer = new ImplementationStatusEventProducer<string, ImplementationStatusValue>(producerOptions);
                    string eventID = Guid.NewGuid().ToString();
                    DateTime sentTime = DateTime.UtcNow;
                    implementationStatusValue.status_datetime = sentTime.ToString();
                    string requestContent = JsonConvert.SerializeObject(implementationStatusValue);
                    eventPublishDetailID = _KafkaConsumerDAO.SaveKafkaEventDetails(cm.GetConnection(),_pmcID,_siteID,0, requestContent, 0, eventID,topicName);
                    eventPublishDetailLogID = _KafkaConsumerDAO.SaveKafkaEventDetailLog(cm.GetConnection(),_pmcID, 0, eventPublishDetailID, sentTime, DateTime.MaxValue, DateTime.MaxValue, "");
                    var messageSent =  implementationStatusEventProducer.Produce(context, eventID, eventID, implementationStatusValue).Result;
                    if (!messageSent.Item1) 
                    { 
                        eventPublishDetailLogID = _KafkaConsumerDAO.SaveKafkaEventDetailLog(cm.GetConnection(), _pmcID, eventPublishDetailLogID, eventPublishDetailID, sentTime, DateTime.UtcNow, DateTime.UtcNow, messageSent.Item2);
                    }
                    else
                    {
                        _KafkaConsumerDAO.SaveKafkaEventDetails(cm.GetConnection(), _pmcID, _siteID, eventPublishDetailID, requestContent, 1, eventID, topicName);
                        _KafkaConsumerDAO.SaveKafkaEventDetailLog(cm.GetConnection(), companyID, eventPublishDetailLogID, eventPublishDetailID, sentTime, DateTime.UtcNow, DateTime.MaxValue, messageSent.Item2);
                    }

                }
            }
            catch (Exception ex)
            {
                LoggingManager loggingManager = new LoggingManager();
                loggingManager.LogApiError(ex);
                _KafkaConsumerDAO.SaveKafkaEventDetailLog(cm.GetConnection(), companyID, eventPublishDetailLogID, eventPublishDetailID, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow, ex.ToString());
            }
        }

        private bool isEUEnv()
        {
            string hostHeader = _env["install_onesitehostheader"].Value1.ToUpper();
            return _cf.IsOneSiteEUEnvironment(hostHeader);

        }

        private string GetTopicName()
        {
            string env = Env["appenvironment"].Value1.ToUpper();
            string topicName;
            if (env.Equals("DEV"))
            {
                topicName = "implementation-status-dev";
            }
            else if (env.Equals("PCT"))
            {
                topicName = "implementation-status-qa";
            }
            else if (env.Equals("SAT"))
            {
                topicName = "implementation-status-sat";
            }
            else if (env.Equals("OCRT") || env.Equals("PREPROD"))
            {
                topicName = "implementation-status-ppd";
            }
            else
            {
                topicName = "implementation-status";
            }
            return topicName;
        }
    }
}
