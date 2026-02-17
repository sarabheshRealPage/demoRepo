using com.realpage.avro.implementation;
using Newtonsoft.Json;
using RealPage.Common.UPMBaseSetup.Base.DAO.Class;
using RealPage.Common.UPMBaseSetup.Base.Logic.Interface;
using RealPage.OneSite.Affordable.DataHubLRTC.BusinessObjects.Class;
using RealPage.OneSite.Affordable.DataHubLRTC.BusinessObjects.Enum;
using RealPage.OneSite.Affordable.DataHubLRTC.DAO.Interface;
using RealPage.OneSite.Affordable.DataHubLRTC.DBObjects.Interface;
using RealPage.OneSite.Affordable.DataHubLRTC.Utilities.Class;
using RealPage.OneSite.Common.Base;
using RealPage.OneSite.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RealPage.OneSite.Affordable.DataHubLRTC.DAO.Class
{
    public class KafkaConsumerDAO : UPMBaseDAO, IKafkaConsumerDAO
    {
        #region PRIVATE VARIABLES
        private readonly IKafkaConsumerDB _IKafkaConsumerDB;
        private List<UtilityAllowanceSource> _utilityAllowanceSourcesFromOS = new List<UtilityAllowanceSource>();
        private List<Floorplan> _flooplansFromOS = new List<Floorplan>();
        private List<IncomeLimitArea> _incomeLimitArea = new List<IncomeLimitArea>();

        #endregion

        public string topicName = "implementation-status";

        #region Constructors
        /// <summary>
        /// Create a basic instance of the KafkaConsumerDAO class
        /// </summary>
        /// <param name="context"></param>
        /// <param name="db"></param>
        public KafkaConsumerDAO(IWebApiContext webContext, IKafkaConsumerDB servicesDB, IUPMBaseManager baseManager) : base(webContext, baseManager)
        {
            _IKafkaConsumerDB = servicesDB;
        }


        #endregion Constructors
        /// <summary>
		/// Saves the data into kafka event details table
		/// </summary>
		/// <param name="companyID">PMC ID</param>
		/// <param name="siteID">site ID</param>
		/// <param name="eventPublishDetailID">event publish detail ID</param>
		/// <param name="payloadContent">Json data which will be sent to kafka</param>
		/// <param name="published">published to kafka</param>
		/// <param name="guID">unique ID</param>
		/// <param name="topicName">kafka topic name</param>
		/// <returns>event detail ID</returns>
		public long SaveKafkaEventDetails(SqlConnection con,
                                             long companyID,
                                             long siteID,
                                             long eventPublishDetailID,
                                             string payloadContent,
                                             int published,
                                             string guID,
                                             string topicName)
        {
            int updated = 0;
            Hashtable parameters = new Hashtable();
            parameters.Add("@EventPublishDetailID", eventPublishDetailID);
            parameters.Add("@EventPMCId", companyID);
            parameters.Add("@EventSiteId", siteID);
            parameters.Add("@TopicName", topicName);
            parameters.Add("@Payload", payloadContent);
            parameters.Add("@Published", published);
            parameters.Add("@PubGUID", guID);

            string query = _IKafkaConsumerDB.SaveKafkaEventPublishDetail;
            int eventSubsciptionID = 0;
            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, con))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    eventSubsciptionID = readhelper.GetInt(updated);
                }
            }
            return eventSubsciptionID;
        }

        /// <summary>
        /// Saves the data into kafka event details log table
        /// </summary>
        /// <param name="con"> sql connection </param>
        /// <param name="companyID">PMC ID</param>
        /// <param name="eventPublishDetailLogID">log ID</param>
        /// <param name="eventPublishDetailID">event publish detail ID</param>
        /// <param name="sentDateTime">message sent time</param>
        /// <param name="ackDateTime">ack date time</param>
        /// <param name="errorDateTime">error datetime if any/param>
        /// <param name="errorMessage">error message if any</param>
        /// <returns>event detail ID</returns>
        public long SaveKafkaEventDetailLog(SqlConnection con,
                                                long companyID,
                                                long eventPublishDetailLogID,
                                                long eventPublishDetailID,
                                                DateTime sentDateTime,
                                                DateTime? ackDateTime,
                                                DateTime? errorDateTime,
                                                string errorMessage)
        {
            int updated = 0;
            Hashtable parameters = new Hashtable();
            parameters.Add("@EventPublishDetailLogID", eventPublishDetailLogID);
            parameters.Add("@EventPublishDetailID", eventPublishDetailID);
            parameters.Add("@SentDateTime", sentDateTime);
            parameters.Add("@AckDateTime", ackDateTime);
            parameters.Add("@ErrorDateTime", errorDateTime);
            parameters.Add("@ErrorMessage", errorMessage);

            string query = _IKafkaConsumerDB.SaveKafkaEventPublishDetailLog;
            int eventSubscriptionLogID = 0;

            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, con))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    eventSubscriptionLogID = readhelper.GetInt(updated);
                }
            }
            return eventSubscriptionLogID;
        }

        /// <summary>
        /// GetSiteImplementationKafkaMessage
        /// </summary>
        /// <param name="companyID">PMC ID</param>
        /// <param name="ImplementationUUID"> Implementation UUID</param>
        /// <returns>SiteImplementationKafkaMessage</returns>
        public async Task<SiteImplementationKafkaMessage> GetSiteImplementationKafkaMessage(SqlConnection con, string ImplementationUUID, int companyID)
        {
            const int F_SITEIMPLEMENTATIONKAFKAMESSAGEID = 0;
            const int F_PMCID = 1;
            const int F_SITEID = 2;
            const int F_IMPLEMENTATIONUUID = 3;
            const int F_PRODUCTCODE = 4;
            const int F_MESSAGE = 5;
            const int F_STATUS = 6;
            const int F_STATUSMESSAGE = 7;
            const int F_CREATEDDATE = 8;
            const int F_CREATEDBY = 9;
            const int F_MODIFIEDDATE = 10;
            const int F_MODIFIEDBY = 11;
            Hashtable parameters = new Hashtable();
            parameters.Add("@InternalEntityID", companyID);
            parameters.Add("@ImplementationUUID", ImplementationUUID);
            string query = _IKafkaConsumerDB.GetSiteImplementationKafkaMessage;
            SiteImplementationKafkaMessage siteImplementationKafkaMessage = new SiteImplementationKafkaMessage();

            using (SqlDataReader reader = await ExecuteSqlDataReaderAsync(query, parameters, con))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (await reader.ReadAsync())
                {
                    siteImplementationKafkaMessage.SiteImplementationKafkaMessageID = readhelper.GetInt(F_SITEIMPLEMENTATIONKAFKAMESSAGEID);

                    siteImplementationKafkaMessage.PMCID = readhelper.GetInt(F_PMCID);

                    siteImplementationKafkaMessage.SiteID = readhelper.GetInt(F_SITEID);

                    siteImplementationKafkaMessage.ImplementationUUID = readhelper.GetString(F_IMPLEMENTATIONUUID);

                    siteImplementationKafkaMessage.ProductCode = readhelper.GetString(F_PRODUCTCODE);

                    siteImplementationKafkaMessage.Message = readhelper.GetString(F_MESSAGE);

                    siteImplementationKafkaMessage.Status = readhelper.GetString(F_STATUS);

                    siteImplementationKafkaMessage.StatusMessage = readhelper.GetString(F_STATUSMESSAGE);

                    siteImplementationKafkaMessage.CreatedDate = readhelper.GetDateTime(F_CREATEDDATE);

                    siteImplementationKafkaMessage.CreatedBy = readhelper.GetInt(F_CREATEDBY);

                    siteImplementationKafkaMessage.ModifiedDate = readhelper.GetDateTime(F_MODIFIEDDATE);

                    siteImplementationKafkaMessage.ModifiedBy = readhelper.GetInt(F_MODIFIEDBY);
                }
            }
            return siteImplementationKafkaMessage;
        }

        /// <summary>
        /// GetSiteImplementationKafkaMessage
        /// </summary>
        /// <param name="siteImplementationKafkaMessage"> siteImplementationKafkaMessage</param>
        /// <returns>SiteImplementationKafkaMessage</returns>
        public async Task<SiteImplementationKafkaMessage> SaveSiteImplementationKafkaMessage(SqlConnection con,
                                                                                             SiteImplementationKafkaMessage siteImplementationKafkaMessage)
        {
            const int F_SITEIMPLEMENTATIONKAFKAMESSAGEID = 0;

            Hashtable parameters = new Hashtable();
            parameters.Add("@SiteImplementationKafkaMessageID", siteImplementationKafkaMessage.SiteImplementationKafkaMessageID);
            parameters.Add("@InternalEntityID", siteImplementationKafkaMessage.PMCID);
            parameters.Add("@InternalSiteID", siteImplementationKafkaMessage.SiteID);
            parameters.Add("@ImplementationUUID", siteImplementationKafkaMessage.ImplementationUUID);
            parameters.Add("@ProductCode", siteImplementationKafkaMessage.ProductCode);
            parameters.Add("@Message", siteImplementationKafkaMessage.Message);
            parameters.Add("@Status", siteImplementationKafkaMessage.Status);
            parameters.Add("@StatusMessage", siteImplementationKafkaMessage.StatusMessage);
            parameters.Add("@InternalUserID", siteImplementationKafkaMessage.ModifiedBy);
            string query = _IKafkaConsumerDB.SaveSiteImplementationKafkaMessage;

            using (SqlDataReader reader = await ExecuteSqlDataReaderAsync(query, parameters, con))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (await reader.ReadAsync())
                {
                    siteImplementationKafkaMessage.SiteImplementationKafkaMessageID = readhelper.GetInt(F_SITEIMPLEMENTATIONKAFKAMESSAGEID);
                }
            }
            return siteImplementationKafkaMessage;
        }

        public ImplementationStatusValue SaveDHTCSiteImplementationRequest(SqlConnection con, DHTCSiteImplementationRequest implementationRequest, int _pmcID, int _siteID)
        {
            SqlTransaction trans = null;
            ImplementationStatusValue implementationResponse = new ImplementationStatusValue()
            {
                status = ImplementationStatusType.SUCCESS,
                errors = new List<Error>()
            };

            try
            {
                if (con.State != ConnectionState.Open)
                {
                    con.Open();
                }

                trans = con.BeginTransaction();

                SaveGeneralHasHotma(con, trans, implementationRequest, implementationResponse);
                SaveGeneralRqeCertVerification(con, trans, implementationRequest, implementationResponse);
                //For now, we have to skip it until we get further clarity on this.
                //SaveGeneralDefaultTic(con, trans, implementationRequest, implementationResponse);

                if (implementationRequest.DHTCUnitTypes.Count > 0)
                    SaveTCUnitType(con, trans, implementationRequest, implementationResponse);
                if (implementationRequest.DHTCHouseholdTypes.Count > 0)
                    SaveTCHouseholdType(con, trans, implementationRequest, implementationResponse);
                if (implementationRequest.DHTCUtilityAllowanceSources.Count > 0)
                {
                    _flooplansFromOS = GetOnesiteFloorplan(con, trans, implementationRequest);
                    SaveUtilityAllowanceSource(con, trans, implementationRequest, implementationResponse);
                }
                if (implementationRequest.DHTCUtilityAllowances.Any(d => d.Amount > 0))
                {
                    SaveUtilityAllowanceDetails(con, trans, implementationRequest, implementationResponse);
                }
                if (implementationRequest.DHTCIncomeLimits.Count > 0)
                {
                    _incomeLimitArea = GetIncomeLimitArea(con, trans, implementationRequest);
                    CreateUpdateIncomelimit(con, trans, implementationRequest, implementationResponse);
                }
                if (implementationRequest.DHTCIncomeLimitDetails.Count > 0)
                {
                    SaveIncomeLimitDetails(con, trans, implementationRequest, implementationResponse);
                }
                if (implementationRequest.DHTCPrograms.Count > 0)
                {
                    CreateUpdateProgram(con, trans, implementationRequest, implementationResponse);
                }
                if (implementationRequest.DHTCSetAsides.Count > 0)
                {
                    CreateUpdateSetAside(con, trans, implementationRequest, implementationResponse);
                    UpdatePopulationRestriction(con, trans, implementationRequest, implementationResponse);
                }
                if (implementationRequest.DHTCSetAsideRules.Count > 0)
                {
                    SaveSetAsideRules(con, trans, implementationRequest, implementationResponse);
                   
                }
                if (implementationRequest.DHTCRentFloors.Count > 0)
                {
                    SaveRentFloors(con, trans, implementationRequest, implementationResponse);
                }
                // Aborting the transaction if any error occurs.If this line is removed, all changes made up to the point of failure will still be committed.
                if (implementationResponse.errors.Any())
                    throw new Exception("Logical errors occurred — rolling back.");
                else
                    trans.Commit();
            }
            catch (Exception ex)
            {
                trans?.Rollback();
                implementationResponse.errors.Add(new Error()
                {
                    value = ex.Message.ToString(),
                    message = "Error while processing SaveDHTCSiteImplementationRequest"
                });
            }
            finally
            {
                if (implementationResponse.errors.Any())
                {
                    implementationResponse.status = ImplementationStatusType.ERROR;
                }
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return implementationResponse;
        }

        #region Look UPs
        public List<DefaultTic> GetTenantIncomeCertificationsList(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_ID = 0;
            const int F_Name = 1;
            const int F_DefaultID = 2;
            const int F_SelectedBit = 3;

            List<DefaultTic> ticList = new List<DefaultTic>();
            var dBEntity = Entity;
            try
            {
                string query = _IKafkaConsumerDB.GetTenantIncomeCertificationsList;

                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                parameters.Add("@InternalUserID", 1);  // No trailing spaces, use actual int
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                Entity = DBEntity.PMC;
                using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
                {
                    SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                    while (reader.Read())
                    {
                        var ticObject = new DefaultTic()
                        {
                            DefaultTIC = readhelper.GetBoolean(F_DefaultID),
                            id = readhelper.GetInt(F_ID),
                            Name = readhelper.GetString(F_Name),
                            SelectedBit = readhelper.GetBoolean(F_SelectedBit)

                        };
                        ticList.Add(ticObject);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Entity = dBEntity;
            }
            return ticList;
        }
        public List<UnitType> GetTCUnittypes(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_ID = 0;
            const int F_Name = 1;


            List<UnitType> tcUnitTypeList = new List<UnitType>();
            try
            {
                string query = _IKafkaConsumerDB.GetTcUnitType;

                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                parameters.Add("@InternalUserID", 1);
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
                {
                    SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                    while (reader.Read())
                    {
                        var unitType = new UnitType()
                        {
                            Id = readhelper.GetInt(F_ID),
                            Name = readhelper.GetString(F_Name)
                        };
                        tcUnitTypeList.Add(unitType);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return tcUnitTypeList;
        }
        public List<HouseholdType> GetTCHouseholdtypes(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_ID = 0;
            const int F_Name = 1;
            const int F_GroupId = 2;
            const int F_GroupName = 3;


            List<HouseholdType> tcHouseholdTypeList = new List<HouseholdType>();
            try
            {
                string query = _IKafkaConsumerDB.GetTcHouseHoldType;

                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                parameters.Add("@InternalUserID", 1);  // No trailing spaces, use actual int
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
                {
                    SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                    while (reader.Read())
                    {
                        var unitType = new HouseholdType()
                        {
                            Id = readhelper.GetInt(F_ID),
                            Name = readhelper.GetString(F_Name),
                            GroupId = readhelper.GetString(F_GroupId),
                            Group = readhelper.GetString(F_GroupName),
                        };
                        tcHouseholdTypeList.Add(unitType);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return tcHouseholdTypeList;
        }
        public List<HouseholdType> GetTCHouseholdGroup(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {

            const int F_GroupId = 0;
            const int F_GroupName = 1;


            List<HouseholdType> tcHouseholdGroupList = new List<HouseholdType>();
            try
            {
                string query = _IKafkaConsumerDB.GetTcHouseHoldGroupId;

                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalUserID", 1);  // No trailing spaces, use actual int
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
                {
                    SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                    while (reader.Read())
                    {
                        var HouseholdGroup = new HouseholdType()
                        {
                            GroupId = readhelper.GetString(F_GroupId),
                            Group = readhelper.GetString(F_GroupName),
                        };
                        tcHouseholdGroupList.Add(HouseholdGroup);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return tcHouseholdGroupList;
        }
        public List<Floorplan> GetOnesiteFloorplan(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_FPNAME = 1;
            const int F_FPID = 2;

            try
            {
                string query = _IKafkaConsumerDB.GetFlooplans;

                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalUserID", 1);  // No trailing spaces, use actual int
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
                {
                    SqlDataReaderHelper readHelper = new SqlDataReaderHelper(reader);
                    while (reader.Read())
                    {
                        var floorplanCodeRaw = readHelper.GetString(F_FPNAME);
                        var parts = floorplanCodeRaw?.Split('|');
                        var fpCode = parts?.Length > 0 ? SafeTypes.TrimSpaces(parts[0]) : string.Empty;

                        var floorplan = new Floorplan
                        {
                            FloorplanId = readHelper.GetInt(F_FPID),
                            FloorplanCode = fpCode
                        };

                        _flooplansFromOS.Add(floorplan);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _flooplansFromOS;
        }
        public List<UtilityAllowanceSource> GetUtilityAllowanceSources(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {

            const int F_UtilityID = 0;
            const int F_UtilityAllowanceName = 1;
            const int F_StartDate = 2;
            var uaSourcesFromOS = new List<UtilityAllowanceSource>();

            try
            {
                string query = _IKafkaConsumerDB.GetUtilityAllowanceSource;
                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalUserID", 1);  // No trailing spaces, use actual int
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
                {
                    SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                    while (reader.Read())
                    {
                        var source = new UtilityAllowanceSource()
                        {
                            Id = readhelper.GetInt(F_UtilityID),
                            Name = readhelper.GetString(F_UtilityAllowanceName),
                            EffectiveDate = readhelper.GetDateTime(F_StartDate)
                        };
                        uaSourcesFromOS.Add(source);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return uaSourcesFromOS;
        }
        public List<UtilityAllowance> GetUtilityAllowanceDetail(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {

            const int F_UTILITYDETAILID = 0;
            const int F_UTILITYSOURCEID = 1;
            const int F_UTILITYFPID = 2;

            List<UtilityAllowance> UtilityAllowances = new List<UtilityAllowance>();
            try
            {
                string query = _IKafkaConsumerDB.GetUtilityAllowances;

                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                parameters.Add("@InternalUserID", 1);  // No trailing spaces, use actual int
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
                {
                    SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                    while (reader.Read())
                    {
                        var utilityAllowance = new UtilityAllowance()
                        {
                            Id = readhelper.GetInt(F_UTILITYDETAILID),
                            FloorPlanId = readhelper.GetInt(F_UTILITYFPID),
                            UtilityAllowanceSourceId = readhelper.GetInt(F_UTILITYSOURCEID)
                        };
                        UtilityAllowances.Add(utilityAllowance);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return UtilityAllowances;
        }

        public List<IncomelimitType> GetIncomeLimitType(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_ID = 0;
            const int F_IncomeLimitType = 1;
            const int F_IncomelimitTypeVal = 2;
            const int F_IsOther = 3;

            List<IncomelimitType> incomeLimitTypeList = new List<IncomelimitType>();
            try
            {
                string query = _IKafkaConsumerDB.GetIncomeLimitType;

                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                parameters.Add("@InternalUserID", 1);  // No trailing spaces, use actual int
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
                {
                    SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                    while (reader.Read())
                    {
                        var incomeLimitTypeObject = new IncomelimitType()
                        {
                            IlTypeName = SafeTypes.TrimSpaces(readhelper.GetString(F_IncomeLimitType)),
                            IlTypeID = readhelper.GetString(F_IncomelimitTypeVal),
                            IsOtherIlType = readhelper.GetBoolean(F_IsOther)
                        };
                        if (incomeLimitTypeObject.IlTypeName != "Other")
                            incomeLimitTypeList.Add(incomeLimitTypeObject);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return incomeLimitTypeList;
        }
        public List<IncomeLimitArea> GetIncomeLimitArea(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_ID = 0;
            const int F_Internalcode = 1;
            const int F_FipsCode = 2;
            const int F_StateCode = 4;
            const int F_CountyName = 5;
            const int F_AreaLable = 7;

            try
            {
                string query = _IKafkaConsumerDB.GetIncomeLimitArea;

                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                parameters.Add("@InternalUserID", 1);  // No trailing spaces, use actual int
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
                {
                    SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                    while (reader.Read())
                    {
                        var incomeLimitTypeObject = new IncomeLimitArea()
                        {
                            ID = readhelper.GetInt(F_ID),
                            CountyName = SafeTypes.TrimSpaces(readhelper.GetString(F_CountyName)),
                            InternalCode = readhelper.GetString(F_Internalcode),
                            FipsCode = readhelper.GetString(F_FipsCode),
                            Label = readhelper.GetString(F_AreaLable),

                        };
                        _incomeLimitArea.Add(incomeLimitTypeObject);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return _incomeLimitArea;
        }

        public List<IncomeLimit> GetIncomeLimits(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_ID = 0;
            const int F_IncomeLimitType = 1;
            const int F_IncomeLimitTypeID = 3;
            const int F_AreafipsCode = 4;
            const int F_StartDate = 5;
            const int F_EffectiveDate = 16;
            const int F_typeValue = 18;
            const int F_ExpandPersons = 12;

            List<IncomeLimit> incomeLimitList = new List<IncomeLimit>();
            try
            {
                string query = _IKafkaConsumerDB.GetIncomeLimits;

                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                parameters.Add("@InternalUserID", 1);  // No trailing spaces, use actual int
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
                {
                    SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                    while (reader.Read())
                    {
                        var incomeLimitObject = new IncomeLimit()
                        {
                            Id = readhelper.GetInt(F_ID),
                            IncomeLimitType = (IncomeLimitType)readhelper.GetInt(F_IncomeLimitTypeID),
                            OtherType = readhelper.GetString(F_IncomeLimitType),
                            IncomeLimitTypeID = readhelper.GetInt(F_IncomeLimitTypeID),
                            EffectiveDate = readhelper.GetDateTime(F_EffectiveDate),
                            ExpandPersons = readhelper.GetString(F_ExpandPersons),
                            StartDate = readhelper.GetDateTime(F_StartDate),
                            County = readhelper.GetString(F_typeValue),
                            AreaFipsCode = readhelper.GetString(F_AreafipsCode)
                        };
                        incomeLimitList.Add(incomeLimitObject);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return incomeLimitList;
        }
        public List<IncomeLimitDetail> GetIncomeLimitDetails(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_ID = 0;
            const int F_IncomeLimitType = 1;
            const int F_IncomeLimitTypeValue = 28;
            const int F_IncomeLimitPercentage = 19;


            List<IncomeLimitDetail> incomeLimitDetails = new List<IncomeLimitDetail>();
            try
            {
                string query = _IKafkaConsumerDB.GetIncomeLimitDetails;

                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                parameters.Add("@InternalUserID", 1);  // No trailing spaces, use actual int
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
                {
                    SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                    while (reader.Read())
                    {
                        var incomeLimitDetailObj = new IncomeLimitDetail()
                        {
                            Id = readhelper.GetInt(F_ID),
                            IncomeLimitSourceId = readhelper.GetInt(F_IncomeLimitType),
                            IncomeLimitSourceName = SafeTypes.TrimSpaces(readhelper.GetString(F_IncomeLimitTypeValue)),
                            PercentageLimit = readhelper.GetInt(F_IncomeLimitPercentage)
                        };
                        incomeLimitDetails.Add(incomeLimitDetailObj);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return incomeLimitDetails;
        }
        public async Task<List<Building>> GetOnesiteBuilidings(SqlConnection conn, DHTCSiteImplementationRequest implementationRequest)
        {
            int F_ID = 0;
            int F_NAME = 1;
            Hashtable parameters = new Hashtable();
            List<Building> buildings = new List<Building>();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GetBuildingsList;

            // Execute and process results
            using (SqlDataReader reader = await ExecuteSqlDataReaderAsync(query, parameters, conn))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (await reader.ReadAsync())
                {
                    buildings.Add(new Building()
                    {
                        Id = readhelper.GetInt(F_ID),
                        OnesiteBuildingNumber = readhelper.GetString(F_NAME)
                    });
                }
            }
            return buildings;
        }

        #endregion

        #region Private Methods - Save Data to OS
        private void SaveGeneralHasHotma(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, ImplementationStatusValue implementationRes)
        {
            try
            {
                string query = _IKafkaConsumerDB.SaveGeneralNeedsHotma;

                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                parameters.Add("@InternalUserID", 1);  // No trailing spaces, use actual int
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                parameters.Add("@EnableHOTMAValue", implementationRequest.DHTCGeneralInfo.NeedsHotma);

                ExecuteNonQuery(query, DBEntity.Site, parameters, transaction);
            }
            catch (SqlException ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.Message,
                    message = $"SQL Error in SaveGeneralHasHotma: {ex.Message}",
                    name = "SaveGeneralHasHotma"
                });
                throw;
            }
        }
        private void SaveGeneralRqeCertVerification(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, ImplementationStatusValue implementationRes)
        {
            try
            {
                string query = _IKafkaConsumerDB.SaveGeneralReqVerification;

                Hashtable parameters = new Hashtable();
                parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                parameters.Add("@InternalUserID", 1);  // No trailing spaces, use actual int
                parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                parameters.Add("@ReqVerification", implementationRequest.DHTCGeneralInfo.RequireVerifications);

                ExecuteNonQuery(query, DBEntity.Site, parameters, transaction);
            }
            catch (SqlException ex)
            {
                implementationRes.errors.Add(new Error() { value = ex.Message.ToString(), message = "Error while saving the General ReqVerification before completing certifications", });
                throw;
            }
        }
        private void SaveGeneralDefaultTic(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, ImplementationStatusValue implementationRes)
        {
            try
            {
                var ticList = GetTenantIncomeCertificationsList(cm.GetConnection(), transaction, implementationRequest);
                int? defaultTicID = ticList.Where(x => x.Name == implementationRequest.DHTCGeneralInfo.DefaultTic.ToString()).FirstOrDefault()?.id;
                if (defaultTicID != null && defaultTicID > 0)
                {
                    string query = _IKafkaConsumerDB.SaveGeneralDefaultTic;

                    Hashtable parameters = new Hashtable();
                    parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                    parameters.Add("@InternalUserID", 1);
                    parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                    parameters.Add("@SELECTEDBIT", 1);
                    parameters.Add("@DEFAULTTIC", 1);
                    parameters.Add("@ID", defaultTicID);

                    ExecuteNonQuery(query, DBEntity.Site, parameters, transaction);
                }
            }
            catch (SqlException ex)
            {
                implementationRes.errors.Add(new Error() { value = ex.Message.ToString(), message = "Error while saving the General DefaultTic.", });
                throw;
            }
        }
        private void SaveTCUnitType(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, ImplementationStatusValue implementationRes)
        {
            try
            {
                string query = _IKafkaConsumerDB.SaveTcUnitType;
                // Fetch existing unit types and group mappings
                List<UnitType> unitTypes = GetTCUnittypes(conn, transaction, implementationRequest);
                foreach (var item in implementationRequest.DHTCUnitTypes)
                {
                    //skip if unit type is duplicate
                    if (unitTypes.Any(u => string.Equals(u.Name, item.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }
                    Hashtable parameters = new Hashtable();
                    parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                    parameters.Add("@InternalUserID", 1);
                    parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                    parameters.Add("@description", item.Name);
                    ExecuteNonQuery(query, DBEntity.Site, parameters, transaction);
                }
            }
            catch (SqlException ex)
            {
                implementationRes.errors.Add(new Error() { value = ex.Message.ToString(), message = "Error while saving the tax credit Unit Types." });
                throw;
            }
        }
        private void SaveTCHouseholdType(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, ImplementationStatusValue implementationRes)
        {
            try
            {
                string query = _IKafkaConsumerDB.SaveTcHouseHoldType;

                // Fetch existing household types and group mappings
                List<HouseholdType> houseHoldTypesFromOS = GetTCHouseholdtypes(conn, transaction, implementationRequest);
                List<HouseholdType> hhGroupList = GetTCHouseholdGroup(conn, transaction, implementationRequest);
                foreach (var item in implementationRequest.DHTCHouseholdTypes)
                {
                    // Safely find the group ID from OneSite
                    HouseholdType matchedGroup = hhGroupList.Find(g =>
                        string.Equals(g.Group, item.Group, StringComparison.OrdinalIgnoreCase));

                    int hhGroupId = matchedGroup != null ? Convert.ToInt32(matchedGroup.GroupId) : 0;

                    // Skip if group ID is missing or name is a duplicate
                    bool isDuplicate = houseHoldTypesFromOS.Any(h =>
                        string.Equals(h.Name, item.Name, StringComparison.OrdinalIgnoreCase));

                    if (isDuplicate || hhGroupId == 0)
                        continue;

                    Hashtable parameters = new Hashtable();
                    parameters.Add("@InternalUserID", 1);
                    parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                    parameters.Add("@HouseholdgroupID", hhGroupId);
                    parameters.Add("@HouseholdType", item.Name);
                    ExecuteNonQuery(query, DBEntity.Site, parameters, transaction);
                }
            }
            catch (SqlException ex)
            {
                implementationRes.errors.Add(new Error() { value = ex.Message.ToString(), message = "Error while saving the tax credit Household Types." });
                throw;
            }
        }
        private void SaveUtilityAllowanceSource(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, ImplementationStatusValue implementationRes)
        {
            try
            {
                string query = _IKafkaConsumerDB.UpdateutilityAllowanceSource;
                var uASourcesFromOS = GetUtilityAllowanceSources(conn, transaction, implementationRequest);
                foreach (var item in implementationRequest.DHTCUtilityAllowanceSources)
                {
                    int uaID = uASourcesFromOS.FirstOrDefault(u => string.Equals(u.Name, item.Name, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;

                    Hashtable parameters = new Hashtable();
                    parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                    parameters.Add("@InternalUserID", 1);
                    parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                    parameters.Add("@UtilityAllowanceName", item.Name);
                    parameters.Add("@StartDate", item.EffectiveDate);
                    parameters.Add("@UtilityID", uaID <= 0 ? (object)DBNull.Value : uaID.ToString());
                    ExecuteNonQuery(query, DBEntity.Site, parameters, transaction);
                }
            }
            catch (SqlException ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.ToString(),
                    message = "Error while saving the tax credit Utility Allowance Sources - " + ex.ToString(),
                    name = "SaveUtilityAllowanceSource"

                });
                throw;
            }
        }
        private void SaveUtilityAllowanceDetails(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, ImplementationStatusValue implementationRes)
        {
            try
            {
                string query = _IKafkaConsumerDB.UpdateutilityAllowanceDetails;

                foreach (var item in implementationRequest.DHTCUtilityAllowances)
                {
                    // Get UA Source Id from OS
                    var uaSourcesFromOS = GetUtilityAllowanceSources(conn, transaction, implementationRequest);
                    var uaSourceIDfromOS = uaSourcesFromOS.FirstOrDefault(u => string.Equals(u.Name, item.UtilityAllowanceSourceName, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;

                    //Get flooplans from OS and match with the Datahub flooplan
                    var fpId = _flooplansFromOS.FirstOrDefault(f => string.Equals(item.FloorPlanCode, f.FloorplanCode, StringComparison.OrdinalIgnoreCase))?.FloorplanId ?? 0;

                    if (uaSourceIDfromOS == 0 || fpId == 0)
                    {
                        var message = "Data mismatch with Onesite for Utility Allowance Details - Name: " + item.UtilityAllowanceSourceName + "." +
                            uaSourceIDfromOS + ",FloorplanID " + fpId;
                        implementationRes.errors.Add(new Error()
                        {
                            value = string.Empty,
                            message = message,
                            name = "SaveUtilityAllowanceDetails"
                        });
                        throw new Exception(message);
                    }
                    //Get Utility Allowance Details ID Using UA Source id and Fp ID from UtilityAllowanceDetails - OS
                    var utilityAllowanceDetails = GetUtilityAllowanceDetail(conn, transaction, implementationRequest);
                    var uadID = utilityAllowanceDetails.
                                        Where(u => u.UtilityAllowanceSourceId == uaSourceIDfromOS && u.FloorPlanId == fpId)
                                        .FirstOrDefault()?.Id;
                    if (uadID <= 0)
                    {
                        continue;
                    }
                    Hashtable parameters = new Hashtable();
                    parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                    parameters.Add("@InternalUserID", 1);
                    parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                    parameters.Add("@uadID", uadID.ToString());
                    parameters.Add("@Amount", item.Amount?.ToString());
                    ExecuteNonQuery(query, DBEntity.Site, parameters, transaction);
                }
            }
            catch (SqlException ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.Message.ToString(),
                    message = "Error while saving the tax credit Utility Allowance Details - SaveUtilityAllowanceDetails."
                });
                throw;
            }
        }

        public void CreateUpdateIncomelimit(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, ImplementationStatusValue implementationRes)
        {
            try
            {
                string query = _IKafkaConsumerDB.CreateIncomeLimit;
                //get the type and compare with ID 
                var incomelimitTypes = GetIncomeLimitType(conn, transaction, implementationRequest);
                var incomelimits = GetIncomeLimits(conn, transaction, implementationRequest);

                foreach (var item in implementationRequest.DHTCIncomeLimits)
                {
                    // Step 1: Get Income Limit Type ID (case-insensitive)
                    var incomeLimitTypeName = item.IncomeLimitType.ToString() == "Other" ? item.OtherType : item.IncomeLimitType.ToString();
                    var incomeLimitTypeId = incomelimitTypes
                        .FirstOrDefault(i => string.Equals(i.IlTypeName, incomeLimitTypeName, StringComparison.OrdinalIgnoreCase))
                        ?.IlTypeID;
                    if (incomeLimitTypeId == "0" && !string.IsNullOrEmpty(item.OtherType))
                    {
                        implementationRes.errors.Add(new Error()
                        {
                            value = string.Empty,
                            message = "IncomeLimitTypeId mismatch in CreateOrUpdateIncomeLimit  - " + incomeLimitTypeName + ".",
                            name = "CreateUpdateIncomelimit"
                        });
                        continue;
                    }
                    // Step 2: Get Area FIPS Code (case-insensitive) 
                    //_incomeLimitArea = GetIncomeLimitArea(conn, transaction, implementationRequest);
                    var internalAFipsCode = _incomeLimitArea
                        .FirstOrDefault(i => string.Equals(i.CountyName, item.County, StringComparison.OrdinalIgnoreCase))?.InternalCode;

                    if (string.IsNullOrEmpty(internalAFipsCode))
                    {
                        implementationRes.errors.Add(new Error()
                        {
                            value = string.Empty,
                            message = "County name not found in Onesite for incomeLimitTypeName  - " + incomeLimitTypeName + "," + incomeLimitTypeId + "."
                        });
                        throw new Exception("Invalid County for incomeLimitTypeName - " + incomeLimitTypeName);
                    }

                    // Step 3: Get Income Limit ID
                    var incomeLimitObj = incomelimits.OrderByDescending(i => i.StartDate)
                        .FirstOrDefault(i => i.IncomeLimitTypeID == Convert.ToInt32(incomeLimitTypeId)
                                          && i.AreaFipsCode == internalAFipsCode);
                    var incomeLimitId = incomeLimitObj?.Id ?? 0;
                    var incomeLimitEffectiveDate = incomeLimitObj?.StartDate;

                    var name = (object)DBNull.Value;
                    if (incomeLimitId > 0 && string.IsNullOrEmpty(item.OtherType))
                        name = incomeLimitTypeName;
                    else if (!string.IsNullOrEmpty(item.OtherType))
                        name = item.OtherType;

                    Hashtable parameters = new Hashtable();
                    parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                    parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                    parameters.Add("@InternalUserID", 1);
                    parameters.Add("@ID", incomeLimitId > 0 ? incomeLimitId.ToString() : (object)DBNull.Value);
                    parameters.Add("@name", name);
                    parameters.Add("@groupid", string.IsNullOrEmpty(item.OtherType) ? incomeLimitTypeId.ToString() : "Other");
                    parameters.Add("@InternalamfCode", internalAFipsCode);
                    parameters.Add("@startdate", item.EffectiveDate ?? (object)DBNull.Value);
                    // we dont have this option in DH 
                    parameters.Add("@round50flag", "0");
                    parameters.Add("@limiteffectivedate", incomeLimitId > 0 ? incomeLimitEffectiveDate : (object)DBNull.Value);

                    ExecuteNonQuery(query, DBEntity.Site, parameters, transaction);
                }
            }
            catch (SqlException ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.Message.ToString(),
                    message = "Error while creating/updating Income Limits - CreateUpdateIncomelimit."
                });
                throw;
            }
        }

        public void SaveIncomeLimitDetails(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, ImplementationStatusValue implementationRes)
        {
            try
            {
                string query = _IKafkaConsumerDB.SaveIncomeLimitDetails;
                //get the income limits and types for processing
                var incomelimits = GetIncomeLimits(conn, transaction, implementationRequest);
                var incomelimitDetails = GetIncomeLimitDetails(conn, transaction, implementationRequest);

                foreach (var item in implementationRequest.DHTCIncomeLimitDetails)
                {
                    var incomelimitObj = incomelimits
                        .OrderByDescending(i => i.StartDate)
                         .FirstOrDefault(i =>
                            string.Equals(i.OtherType?.Trim(), item.IncomeLimitSourceName?.Trim(), StringComparison.OrdinalIgnoreCase) &&
                            i.AreaFipsCode == _incomeLimitArea[0].InternalCode);

                    if (incomelimitObj == null)
                    {
                        var message = "Income Limits Source record not found At - IncomeLimitSourceName" + item.IncomeLimitSourceName;
                        implementationRes.errors.Add(new Error()
                        {
                            message = message,
                            name = "SaveIncomeLimitDetails"
                        });
                        throw new Exception(message);
                    }
                    var type = incomelimitObj?.County?.Replace("_", "$@");
                    var iltdID = incomelimitDetails
                                 .FirstOrDefault(i =>
                                     i.IncomeLimitSourceId == incomelimitObj.Id &&
                                     string.Equals(i.IncomeLimitSourceName, SafeTypes.TrimSpaces(type), StringComparison.OrdinalIgnoreCase)
                                     && i.PercentageLimit == item.PercentageLimit)
                                 ?.Id ?? 0;

                    Hashtable parameters = new Hashtable();
                    parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                    parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                    parameters.Add("@InternalUserID", 1);
                    parameters.Add("@typevalue", type);
                    parameters.Add("@startDate", incomelimitObj.StartDate);
                    parameters.Add("@id", iltdID > 0 ? iltdID.ToString() : (object)DBNull.Value);
                    parameters.Add("@medianpercent", (int)item.PercentageLimit);
                    parameters.Add("@member1", item.OnePerson);
                    parameters.Add("@member2", item.TwoPerson);
                    parameters.Add("@member3", item.ThreePerson);
                    parameters.Add("@member4", item.FourPerson);
                    parameters.Add("@member5", item.FivePerson);
                    parameters.Add("@member6", item.SixPerson);
                    parameters.Add("@member7", item.SevenPerson);
                    parameters.Add("@member8", item.EightPerson);
                    parameters.Add("@member9", item.NinePerson);
                    parameters.Add("@member10", item.TenPerson);
                    parameters.Add("@member11", item.ElevenPerson);
                    parameters.Add("@member12", item.TwelvePerson);
                    parameters.Add("@member13", item.ThirteenPerson);
                    parameters.Add("@member14", item.FourteenPerson);
                    parameters.Add("@member15", item.FifteenPerson);
                    parameters.Add("@member16", item.SixteenPerson);
                    parameters.Add("@unlockEdit", 0); // we dont have this option in DH

                    ExecuteNonQuery(query, DBEntity.Site, parameters, transaction);
                }
            }
            catch (SqlException ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.Message.ToString(),
                    message = "Error while saving Income Limit Details in SaveIncomeLimitDetails Method",
                    name = "SaveIncomeLimitDetails"
                });
                throw;
            }
        }

        /// <summary>
        /// Gets tax credit program types picklist
        /// </summary>
        /// <param name="conn">SQL connection</param>
        /// <param name="transaction">SQL transaction</param>
        /// <param name="implementationRequest">Implementation request</param>
        /// <returns>List of tax credit program types</returns>
        public List<ProgramType> GetTaxCreditProgramType(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_ID = 1;
            const int F_NAME = 0;

            List<ProgramType> programTypes = new List<ProgramType>();
            Hashtable parameters = new Hashtable();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GetTaxCreditProgramType;

            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    ProgramType programType = new ProgramType()
                    {
                        Id = readhelper.GetInt(F_ID),
                        Name = readhelper.GetString(F_NAME)
                    };
                    programTypes.Add(programType);
                }
            }
            return programTypes;
        }
        /// <summary>
        /// Gets existing TC program names
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transaction"></param>
        /// <param name="implementationRequest"></param>
        /// <returns></returns>
        public List<CommonLookUp> GetProgramNamesList(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            int F_NAME = 0;
            int F_ID = 1;
            Hashtable parameters = new Hashtable();
            List<CommonLookUp> lookUps = new List<CommonLookUp>();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GetTaxCreditProgramNamesPicklist;

            // Execute and process results
            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    var ProgramName = SafeTypes.TrimSpaces(readhelper.GetString(F_NAME));
                    var parts = ProgramName?.Split('|');
                    ProgramName = parts?.Length > 0 ? parts[1] : string.Empty;

                    lookUps.Add(new CommonLookUp()
                    {
                        ID = readhelper.GetInt(F_ID),
                        Value = ProgramName
                    });
                }
            }
            return lookUps;
        }

        /// <summary>
        /// Gets tax credit program type names picklist
        /// </summary>
        /// <param name="conn">SQL connection</param>
        /// <param name="transaction">SQL transaction</param>
        /// <param name="implementationRequest">Implementation request</param>
        /// <returns>List of tax credit program type names</returns>
        public List<ProgramTypeName> GetTaxCreditProgramTypeNames(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_ID = 0;
            const int F_NAME = 1;

            List<ProgramTypeName> programTypeNames = new List<ProgramTypeName>();
            Hashtable parameters = new Hashtable();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GetTaxCreditProgramTypeNames;

            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    ProgramTypeName programTypeName = new ProgramTypeName()
                    {
                        Id = readhelper.GetInt(F_ID),
                        Name = readhelper.GetString(F_NAME)
                    };
                    programTypeNames.Add(programTypeName);
                }
            }
            return programTypeNames;
        }

        public List<ProgramTypeName> GetProgramFedral10cList(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            Hashtable parameters = new Hashtable();
            const int F_ID = 1;
            const int F_NAME = 0;

            List<ProgramTypeName> programTypeF80b = new List<ProgramTypeName>();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.ProgramFedral10c;

            // Execute and process results
            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    ProgramTypeName programTypeName = new ProgramTypeName()
                    {
                        Id = readhelper.GetInt(F_ID),
                        Name = SafeTypes.TrimSpaces(readhelper.GetString(F_NAME))
                    };
                    programTypeF80b.Add(programTypeName);
                }
            }
            return programTypeF80b;
        }
        public List<ProgramTypeName> GetProgramFedral8bList(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            Hashtable parameters = new Hashtable();
            const int F_ID = 1;
            const int F_NAME = 0;

            List<ProgramTypeName> programTypeF10C = new List<ProgramTypeName>();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.ProgramFedral8b;

            // Execute and process results
            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    ProgramTypeName programTypeName = new ProgramTypeName()
                    {
                        Id = readhelper.GetInt(F_ID),
                        Name = SafeTypes.TrimSpaces(readhelper.GetString(F_NAME))?.Replace("-", "_")
                    };
                    programTypeF10C.Add(programTypeName);
                }
            }
            return programTypeF10C;
        }
        public List<CommonLookUp> GetProgramRuleScopesList(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            int F_NAME = 0;
            int F_ID = 1;
            Hashtable parameters = new Hashtable();
            List<CommonLookUp> lookUps = new List<CommonLookUp>();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GetProgramRuleScopes;

            // Execute and process results
            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    lookUps.Add(new CommonLookUp()
                    {
                        ID = readhelper.GetInt(F_ID),
                        Value = SafeTypes.TrimSpaces(readhelper.GetString(F_NAME))
                    });
                }
            }
            return lookUps;
        }
        public List<CommonLookUp> GetUnitViolationPenalty(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            int F_ID = 1;
            int F_NAME = 0;
            Hashtable parameters = new Hashtable();
            List<CommonLookUp> lookUps = new List<CommonLookUp>();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GetProgramViolationPenalty;

            // Execute and process results
            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    lookUps.Add(new CommonLookUp()
                    {
                        ID = readhelper.GetInt(F_ID),
                        Value = SafeTypes.TrimSpaces(readhelper.GetString(F_NAME))
                    });
                }
            }
            return lookUps;
        }

        public List<CommonLookUp> GetHomeStudentRules(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            int F_ID = 0;
            int F_NAME = 1;
            Hashtable parameters = new Hashtable();
            List<CommonLookUp> lookUps = new List<CommonLookUp>();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.Gethomestudentrules;

            // Execute and process results
            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    lookUps.Add(new CommonLookUp()
                    {
                        ID = readhelper.GetInt(F_ID),
                        Value = SafeTypes.TrimSpaces(readhelper.GetString(F_NAME))
                    });
                }
            }
            return lookUps;
        }
        public List<CommonLookUp> GetStudentRules(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            int F_ID = 1;
            int F_NAME = 0;
            Hashtable parameters = new Hashtable();
            List<CommonLookUp> lookUps = new List<CommonLookUp>();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GeStudentrules;

            // Execute and process results
            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    lookUps.Add(new CommonLookUp()
                    {
                        ID = readhelper.GetInt(F_ID),
                        Value = SafeTypes.TrimSpaces(readhelper.GetString(F_NAME))
                    });
                }
            }
            return lookUps;
        }

        public List<CommonLookUp> GetLihtcStudentRules(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            int F_ID = 0;
            int F_NAME = 1;
            Hashtable parameters = new Hashtable();
            List<CommonLookUp> lookUps = new List<CommonLookUp>();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GeStudentrules;

            // Execute and process results
            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    lookUps.Add(new CommonLookUp()
                    {
                        ID = readhelper.GetInt(F_ID),
                        Value = SafeTypes.TrimSpaces(readhelper.GetString(F_NAME))
                    });
                }
            }
            return lookUps;
        }


        /// <summary>
        /// Creates or updates a tax credit program
        /// </summary>
        /// <param name="conn">SQL connection</param>
        /// <param name="internalEntityID">Internal Entity ID</param>
        /// <param name="internalUserID">Internal User ID</param>
        /// <param name="internalSiteID">Internal Site ID</param>
        /// <param name="programID">Program ID</param>
        /// <param name="programName">Program Name</param>
        /// <param name="incomeLimitType">Income Limit Type</param>
        /// <param name="defaultUtilityAllowance">Default Utility Allowance</param>
        /// <param name="stateIDNumber">State ID Number</param>
        /// <param name="lihcStudentRule">LIHC Student Rule</param>
        /// <param name="uvr">UVR</param>
        /// <param name="naur">NAUR</param>
        /// <param name="sameBuildingTransfer">Same Building Transfer</param>
        /// <param name="uvrBuildingScope">UVR Building Scope</param>
        /// <param name="uvrOneForOnePenalty">UVR One For One Penalty</param>
        /// <param name="comparableUnitByBedrooms">Comparable Unit By Bedrooms</param>
        /// <param name="comparableUnitSqFtPercen">Comparable Unit Sq Ft Percentage</param>
        /// <param name="programType">Program Type</param>
        /// <param name="naurBuildingScope">NAUR Building Scope</param>
        /// <param name="naurOneForOnePenalty">NAUR One For One Penalty</param>
        /// <param name="userID">User ID</param>
        /// <param name="tcmrrtID">TCMRRT ID</param>
        /// <param name="displaced">Displaced</param>
        /// <param name="displacementTypeCode">Displacement Type Code</param>
        /// <param name="grossRentIncludesAssistancePayment">Gross Rent Includes Assistance Payment</param>
        /// <param name="percentageActual">Percentage Actual</param>
        /// <param name="reportOnTICFlag">Report On TIC Flag</param>
        /// <param name="reportOnTICFlagMarket">Report On TIC Flag Market</param>
        /// <param name="trackCompliancePerBuildingFlag">Track Compliance Per Building Flag</param>
        /// <param name="trackProjectComplianceFlag">Track Project Compliance Flag</param>
        /// <param name="defineRequiredUnitsFlag">Define Required Units Flag</param>
        /// <param name="minimumSetAsidePercentage">Minimum Set Aside Percentage</param>
        /// <param name="includeExemptLIHTCUnitsFlag">Include Exempt LIHTC Units Flag</param>
        /// <param name="bondQualifiedProjectPeriodStartedFlag">Bond Qualified Project Period Started Flag</param>
        /// <param name="bondQualifiedProjectStartDate">Bond Qualified Project Start Date</param>
        /// <param name="homeRulesFlag">Home Rules Flag</param>
        /// <param name="homeFixedFlag">Home Fixed Flag</param>
        /// <param name="homeBuildingWideFlag">Home Building Wide Flag</param>
        /// <param name="lowHomeUnits">Low Home Units</param>
        /// <param name="highHomeUnits">High Home Units</param>
        /// <param name="homeUseExpensesFlag">Home Use Expenses Flag</param>
        /// <param name="homeUseExpensesIncludeOtherSAFlag">Home Use Expenses Include Other SA Flag</param>
        /// <param name="highOIPercentage">High OI Percentage</param>
        /// <param name="doesNotRequireAnnualRecertFlag">Does Not Require Annual Recert Flag</param>
        /// <param name="incomeMinimumSetAside">Income Minimum Set Aside</param>
        /// <param name="customUA">Custom UA</param>
        /// <param name="siteID">Site ID</param>
        /// <param name="federal10cOption">Federal 10c Option</param>
        /// <param name="federal8bOption">Federal 8b Option</param>
        /// <param name="maxRentIncludesAssistancePayment">Max Rent Includes Assistance Payment</param>
        /// <param name="utilityAllowanceRequired">Utility Allowance Required</param>
        /// <param name="selectedUATableID">Selected UA Table ID</param>
        /// <param name="programTypeConditionsCategory">Program Type Conditions Category</param>
        /// <param name="lihtcExceptions">LIHTC Exceptions</param>
        /// <param name="studentExceptions">Student Exceptions</param>
        /// <param name="wavierOption">Wavier Option</param>
        /// <param name="recertInclude">Recert Include</param>
        /// <param name="incomeMinimumSetAsideTraditional">Income Minimum Set Aside Traditional</param>
        /// <param name="definerequiredunitshomeflag">Define Required Units Home Flag</param>
        /// <param name="incomeLimitTypeAHDP">Income Limit Type AHDP</param>
        /// <param name="otherDescription">Other Description</param>
        /// <param name="unitDesignationHistoryStartDate">Unit Designation History Start Date</param>
        /// <param name="minimumLowIncomeUnits">Minimum Low Income Units</param>
        /// <returns>Number of affected rows</returns>
        public void CreateUpdateProgram(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationReq, ImplementationStatusValue implementationRes)
        {
            try
            {
                var programNames = GetProgramNamesList(conn, transaction, implementationReq);
                var programtype = GetTaxCreditProgramType(conn, transaction, implementationReq);
                var incomelimitSource = GetIncomeLimits(conn, transaction, implementationReq);
                var incomelimitTypes = GetIncomeLimitType(conn, transaction, implementationReq);
                var uaSourcesTable = GetUtilityAllowanceSources(conn, transaction, implementationReq);
                var federal10cOptions = GetProgramFedral10cList(conn, transaction, implementationReq);
                var federal8bOptions = GetProgramFedral8bList(conn, transaction, implementationReq);
                var programRuleScopes = GetProgramRuleScopesList(conn, transaction, implementationReq);
                var programViolationPenalty = GetUnitViolationPenalty(conn, transaction, implementationReq);
                var enableStudentrules = GetStudentRules(conn, transaction, implementationReq);
                var homeStudentRules = GetHomeStudentRules(conn, transaction, implementationReq);
                var lihtcStudentRules = GetLihtcStudentRules(conn, transaction, implementationReq);

                foreach (var program in implementationReq.DHTCPrograms)
                {
                    string uaSourcesTableList = string.Empty;
                    string defaultUASource = string.Empty;
                    var Lrtc10cElectionName = string.Empty;
                    var federal10cOption = 0;
                    var federal8bOption = 0;
                    string isUVR = null;
                    string isNUVR = null;
                    string IncomeMinimumSetAsideTraditional = null;
                    var isLIHTC = program.ProgramType == 1;
                    string homeStudentExceptionRule = null;
                    string lihtcStudentExceptionRule = null;

                    //programname comapre
                    var ProgramId = programNames.FirstOrDefault(p => string.Equals(p.Value, program.Name, StringComparison.OrdinalIgnoreCase))?.ID ?? 0;
                    if (ProgramId > 0)
                    {
                        program.OnesiteProgramId = ProgramId;
                    }
                    //get program type id
                    string programtypeDH = program.ProgramTypeName == (ProgramTypeEnum.Other).ToString() ? program.OtherProgramType : program.ProgramTypeName;
                    var programTypeID = programtype.FirstOrDefault(p => string.Equals(p.Name, programtypeDH, StringComparison.OrdinalIgnoreCase))?.Id ?? 0;
                    if (programTypeID == 0)
                    {
                        var message = "Invalid Program Type ID for the Program - " + program.Name +" , "+ programTypeID.ToString();
                        implementationRes.errors.Add(new Error()
                        {
                            value = string.Empty,
                            message = message,
                            name = "CreateUpdateProgram"
                        });
                        throw new Exception(message);
                    }
                    else
                    {
                        program.OnesiteProgramTypeID = programTypeID;
                    }
                    var incomeLimitypeId = incomelimitSource
                      .FirstOrDefault(i => string.Equals(i.OtherType, program.IncomeLimitSourceName, StringComparison.OrdinalIgnoreCase))
                      ?.Id;
                    var defaultUaSourceID = uaSourcesTable
                                            .FirstOrDefault(u => string.Equals(
                                                u.Name,
                                                program.DefaultUASourceName,
                                                StringComparison.InvariantCultureIgnoreCase))?.Id ?? 0;
                    //throw exception if it is 0
                    //if (defaultUaSourceID == 0)
                    //{
                    //    var message = "unable to find default utility allownace source Id for the program" + program.Name;
                    //    implementationRes.errors.Add(new Error()
                    //    {
                    //        value = string.Empty,
                    //        message = message,
                    //        name = "CreateUpdateProgram"
                    //    });
                    //    throw new Exception(message);
                    //}
                    //extract Uatbale IDs 
                    var parsedList = SafeTypes.SafeStringToIntArray(program.UtilityAllowanceSourceIds);
                    foreach (var item in parsedList)
                    {
                        var uaSourcName = implementationReq.DHTCUtilityAllowanceSources.FirstOrDefault(us => us.Id == item)?.Name;
                        var uaSourceIdOS = uaSourcesTable.FirstOrDefault(u => string.Equals(u.Name, uaSourcName, StringComparison.InvariantCultureIgnoreCase))
                                             ?.Id ?? 0;
                        uaSourcesTableList = string.IsNullOrEmpty(uaSourcesTableList) ? uaSourceIdOS.ToString() : uaSourceIdOS + "|";
                    }                  

                    if (isLIHTC)
                    {
                        if (program.Lrtc10cElection == 1 || program.Lrtc10cElection == 2)
                            Lrtc10cElectionName = "TraditionalLIHTC";
                        else
                            Lrtc10cElectionName = ((Lrtc10cElections)program.Lrtc10cElection).ToString();

                        federal10cOption = federal10cOptions
                           .FirstOrDefault(f => string.Equals(f.Name, Lrtc10cElectionName, StringComparison.InvariantCultureIgnoreCase))?.Id ?? 0;

                        federal8bOption = federal8bOptions
                           .FirstOrDefault(f => string.Equals(f.Name,
                               ((Lrtc8bElections)program.Lrtc8bElection).ToString(),
                                   StringComparison.InvariantCultureIgnoreCase))?.Id ?? 0;

                        //as per OS, Tradional income is selected when Lrtc10cElection is Tradiional.
                        if (program.Lrtc10cElection == 1)
                        {
                            IncomeMinimumSetAsideTraditional = "50.000";
                        }
                        else if (program.Lrtc10cElection == 2)
                        {
                            IncomeMinimumSetAsideTraditional = "60.000";
                        }
                        //for program type 1 [LIHTC], this should be 1.
                        isUVR = "1"; ;
                    }
                    else
                    {
                        isNUVR = program.ApplyNaur ? "1" : "0";
                    }

                    //prepare exceptions
                    if (program.ApplyStudent)
                    {
                        if (SafeTypes.ToInt(program.StudentRuleType) == (int)StudentRules.HOMEStudentRule)
                        {
                            var financialAid = homeStudentRules.FirstOrDefault(h => string.Equals(h.Value, "FinancialAidDetermination", StringComparison.InvariantCultureIgnoreCase))?.ID;
                            var vulnerableYouth = homeStudentRules.FirstOrDefault(h => string.Equals(h.Value, "VulnerableYouth", StringComparison.InvariantCultureIgnoreCase))?.ID;
                            if (program.IsFinancialAid)
                                homeStudentExceptionRule = string.IsNullOrEmpty(homeStudentExceptionRule) ? financialAid?.ToString() : financialAid + "|";
                            if (program.IsVulnerableYouth)
                                homeStudentExceptionRule = string.IsNullOrEmpty(homeStudentExceptionRule) ? vulnerableYouth?.ToString() : vulnerableYouth + "|";
                        }

                        if (SafeTypes.ToInt(program.StudentRuleType) == (int)StudentRules.LIHTCStudentRule)
                        {
                            var marriedfilingJointTR = lihtcStudentRules.FirstOrDefault(l => string.Equals(l.Value, "MarriedfilingJointTaxReturn", StringComparison.InvariantCultureIgnoreCase))?.ID;
                            var singleParent = lihtcStudentRules.FirstOrDefault(l => string.Equals(l.Value, "SingleParent", StringComparison.InvariantCultureIgnoreCase))?.ID;
                            var isreceivingAfdc = lihtcStudentRules.FirstOrDefault(l => string.Equals(l.Value, "ReceivingAFDC", StringComparison.InvariantCultureIgnoreCase))?.ID;
                            var isGovtJobEnrolled = lihtcStudentRules.FirstOrDefault(l => string.Equals(l.Value, "EnrolledinGovernmentJobTraining", StringComparison.InvariantCultureIgnoreCase))?.ID;
                            var isOtherExcep = lihtcStudentRules.FirstOrDefault(l => string.Equals(l.Value, "OtherException", StringComparison.InvariantCultureIgnoreCase))?.ID;
                            var isPrevFosterCare = lihtcStudentRules.FirstOrDefault(l => string.Equals(l.Value, "PreviousFosterCareAssistance", StringComparison.InvariantCultureIgnoreCase))?.ID;
                            var isExtendedUsePeriod = lihtcStudentRules.FirstOrDefault(l => string.Equals(l.Value, "ExtendedUsePeriod", StringComparison.InvariantCultureIgnoreCase))?.ID;

                            if (program.IsFilingJointTaxReturn)
                                lihtcStudentExceptionRule = string.IsNullOrEmpty(lihtcStudentExceptionRule) ? marriedfilingJointTR?.ToString() : marriedfilingJointTR + "|";
                            if (program.IsSingleParent)
                                lihtcStudentExceptionRule = string.IsNullOrEmpty(lihtcStudentExceptionRule) ? singleParent?.ToString() : singleParent + "|";
                            if (program.IsReceivingAfdc)
                                lihtcStudentExceptionRule = string.IsNullOrEmpty(lihtcStudentExceptionRule) ? isreceivingAfdc?.ToString() : isreceivingAfdc + "|";
                            if (program.IsEnrolledInJobTraining)
                                lihtcStudentExceptionRule = string.IsNullOrEmpty(lihtcStudentExceptionRule) ? isGovtJobEnrolled?.ToString() : isGovtJobEnrolled + "|";
                            if (program.IsOtherException)
                                lihtcStudentExceptionRule = string.IsNullOrEmpty(lihtcStudentExceptionRule) ? isOtherExcep?.ToString() : isOtherExcep + "|";
                            if (program.IsFosterCare)
                                lihtcStudentExceptionRule = string.IsNullOrEmpty(lihtcStudentExceptionRule) ? isPrevFosterCare?.ToString() : isPrevFosterCare + "|";
                            if (program.IsExtendedUse)
                                lihtcStudentExceptionRule = string.IsNullOrEmpty(lihtcStudentExceptionRule) ? isExtendedUsePeriod?.ToString() : isExtendedUsePeriod + "|";
                        }
                    }
                    if (program.ApplyUvr)
                    {
                        isUVR = "1"; ;
                    }
                    var uvrScopeName = GetEnumValues.GetDescription((LrtcProgramRuleScopes)program.UvrScope);
                    var uvrRuleScopeID = programRuleScopes.FirstOrDefault(p => p.Value == uvrScopeName)?.ID;

                    var NuvrScopeName = GetEnumValues.GetDescription((LrtcProgramRuleScopes)program.NaurScope);
                    var NuvrRuleScopeID = programRuleScopes.FirstOrDefault(p => p.Value == NuvrScopeName)?.ID;

                    var uvrViolationID = programViolationPenalty.FirstOrDefault(v => string.Equals(v.Value, (LrtcProgramRuleViolations)program.UvrViolation))?.ID;
                    var nUvrViolationID = programViolationPenalty.FirstOrDefault(v => string.Equals(v.Value, (LrtcProgramRuleViolations)program.NaurViolation))?.ID;

                    //Home
                    var UnitVarianceName = GetEnumValues.GetDescription((LrtcHomeRuleUnitVariancesEnum)program.HomeRuleUnitVariance);
                    var UnitVarianceID = programRuleScopes.FirstOrDefault(p => p.Value == UnitVarianceName)?.ID;

                    //student rules
                    var studentRule = (StudentRules)SafeTypes.ToInt(program.StudentRuleType);
                    var enableStudentRules = enableStudentrules.FirstOrDefault(v => string.Equals(v.Value, studentRule))?.ID;
                    conn.ChangeDatabase("S" + implementationReq.SiteId);
                    using (SqlCommand cmd = new SqlCommand(_IKafkaConsumerDB.CreateUpdateProgramNoParams, conn))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        var parameters = cmd.Parameters;
                        // Basic identifiers
                        parameters.AddWithValue("@InternalEntityID", implementationReq.PmcId);
                        parameters.AddWithValue("@InternalUserID", 1); // Default user ID
                        parameters.AddWithValue("@InternalSiteID", implementationReq.SiteId);

                        // Program Settings information
                        parameters.AddWithValue("@ProgramID", ProgramId > 0 ? ProgramId.ToString() : (object)DBNull.Value);
                        parameters.AddWithValue("@ProgramName", program.Name);
                        parameters.AddWithValue("@ProgramType", programTypeID);
                        parameters.AddWithValue("@IncomeLimitType", program.IncomeLimitSourceId > 0 ? incomeLimitypeId : (object)DBNull.Value);
                        parameters.AddWithValue("@UtilityAllowanceRequired", string.IsNullOrEmpty(uaSourcesTableList) ? "1" : "0");
                        parameters.AddWithValue("@DefaultUtilityAllowance", defaultUaSourceID.ToString());
                        parameters.AddWithValue("@SelectedUATableID", uaSourcesTableList);
                        parameters.AddWithValue("@StateIDNumber", string.IsNullOrEmpty(program.StateIdentifier) ? (object)DBNull.Value : program.StateIdentifier);
                        parameters.AddWithValue("@SameBuildingTransfer", program.ApplyBuildingTransferRule ? "1" : "0");
                        parameters.AddWithValue("@OtherDescription", string.IsNullOrEmpty(program.OtherProgramType) ? (object)DBNull.Value : program.OtherProgramType);

                        //LIHTC Settings
                        parameters.AddWithValue("@Federal10cOption", isLIHTC ? federal10cOption.ToString() : (object)DBNull.Value);
                        parameters.AddWithValue("@Federal8bOption", isLIHTC ? federal8bOption.ToString() : (object)DBNull.Value);
                        parameters.AddWithValue("@UnitDesignationHistoryStartDate", (isLIHTC && federal10cOption == 2) ? program.DesignationStartDate.ToString() : (object)DBNull.Value);
                        parameters.AddWithValue("@MinimumLowIncomeUnits", isLIHTC ? program.MinimumUnitsPercentage.ToString() : (object)DBNull.Value);
                        parameters.AddWithValue("@MinimumSetAsidePercentage", isLIHTC ? program.MinimumSetAsidePercentage.ToString() : (object)DBNull.Value);
                        parameters.AddWithValue("@IncomeMinimumSetAsideTraditional", IncomeMinimumSetAsideTraditional ?? (object)DBNull.Value);

                        //Unit Vacancy Rule (UVR)
                        parameters.AddWithValue("@UVR", isUVR ?? (object)DBNull.Value);
                        parameters.AddWithValue("@UVRBuildingScope", isUVR == "1" ? uvrRuleScopeID.ToString() : "0");
                        parameters.AddWithValue("@UVROneForOnePenalty", isUVR == "1" ? uvrViolationID.ToString() : "0");
                        parameters.AddWithValue("@ComparableUnitByBedrooms", program.UvrComparableUnit.ToString());
                        parameters.AddWithValue("@ComparableUnitSqFtPercen", program.UvrUnitLargerPercentage.ToString());
                        //NUVR
                        parameters.AddWithValue("@NAUR", isNUVR ?? (object)DBNull.Value);
                        parameters.AddWithValue("@NAURBuildingScope", isNUVR == "1" ? NuvrRuleScopeID.ToString() : "0");
                        parameters.AddWithValue("@NAUROneForOnePenalty", isNUVR == "1" ? nUvrViolationID.ToString() : "0");

                        // HOME program settings
                        parameters.AddWithValue("@HomeRulesFlag", program.ApplyHome ? "1" : "0");
                        parameters.AddWithValue("@HomeFixedFlag", program.HomeRuleUnitVariance.ToString() == "1" ? program.HomeRuleUnitVariance.ToString() : "0"); // Default value
                        parameters.AddWithValue("@HomeBuildingWideFlag", UnitVarianceID); // Default value
                        parameters.AddWithValue("@LowHomeUnits", program.LowHomeUnitCount);
                        parameters.AddWithValue("@HighHomeUnits", program.HighHomeUnitCount);
                        parameters.AddWithValue("@HomeUseExpensesFlag", program.AdjustedIncomeOrExpensesType == "1" ? "1" : (object)DBNull.Value); // Default value
                        parameters.AddWithValue("@HomeUseExpensesIncludeOtherSAFlag", program.AdjustedIncomeOrExpensesType == "2" ? "2" : (object)DBNull.Value); // Default value
                        parameters.AddWithValue("@HighOIPercentage", program.LevelOverIncome.ToString() ?? "0"); // Default value

                        //student rule
                        parameters.AddWithValue("@LIHCStudentRule", program.StudentRuleType == "1" ? enableStudentRules : (object)DBNull.Value);
                        parameters.AddWithValue("@HOMEStudentRule", program.StudentRuleType == "2" ? enableStudentRules : (object)DBNull.Value);

                        //home rule exceptions
                        parameters.AddWithValue("@STUDENTExceptions", homeStudentExceptionRule ?? (object)DBNull.Value);

                        //Lihtc rule exceptions
                        parameters.AddWithValue("@LIHTCExceptions", lihtcStudentExceptionRule ?? (object)DBNull.Value);

                        // User and tracking information
                        parameters.AddWithValue("@userID", 1); // Default user ID
                        parameters.AddWithValue("@TcmrrtID", "1"); // Default TCMRRT ID as per proc
                        parameters.AddWithValue("@Displaced", (object)DBNull.Value); // Default value
                        parameters.AddWithValue("@DisplacementTypeCode", (object)DBNull.Value); // Default value
                        parameters.AddWithValue("@GrossRentIncludesAssistancePayment", (object)DBNull.Value); // Default value
                        parameters.AddWithValue("@PercentageActual", (object)DBNull.Value);

                        // Reporting flags
                        parameters.AddWithValue("@ReportOnTICFlag", (object)DBNull.Value); // Default value
                        parameters.AddWithValue("@ReportOnTICFlagMarket", (object)DBNull.Value); // Default value
                        parameters.AddWithValue("@TrackCompliancePerBuildingFlag", (object)DBNull.Value); // Default value
                        parameters.AddWithValue("@TrackProjectComplianceFlag", (object)DBNull.Value); // Default value
                        parameters.AddWithValue("@DefineRequiredUnitsFlag", (object)DBNull.Value);
                        parameters.AddWithValue("@IncludeExemptLIHTCUnitsFlag", (object)DBNull.Value);

                        // Bond and project settings
                        parameters.AddWithValue("@BondQualifiedProjectPeriodStartedFlag", (object)DBNull.Value); // Default value
                        parameters.AddWithValue("@BondQualifiedProjectStartDate", (object)DBNull.Value);

                        // Additional settings
                        parameters.AddWithValue("@CustomUA", "0"); // Default value
                        parameters.AddWithValue("@SiteID", implementationReq.SiteId);
                        parameters.AddWithValue("@MaxRentIncludesAssistancePayment", (object)DBNull.Value); // Default value
                        parameters.AddWithValue("@ProgramTypeConditionsCategory", (object)DBNull.Value); // Default value

                        // Exception and compliance settings
                        parameters.AddWithValue("@WavierOption", (object)DBNull.Value); // Default value
                        parameters.AddWithValue("@RecertInclude", "0");
                        parameters.AddWithValue("@definerequiredunitshomeflag", (object)DBNull.Value);
                        parameters.AddWithValue("@IncomeLimitTypeAHDP", (object)DBNull.Value); // Default value

                        // optional feilds
                        parameters.AddWithValue("@ProgramNameMarket", (object)DBNull.Value); // or appropriate default
                        parameters.AddWithValue("@programLevelCalculationFlag", (object)DBNull.Value); // or appropriate default  
                        parameters.AddWithValue("@UseFloorPlanMethodFlag", (object)DBNull.Value); // or appropriate default
                        parameters.AddWithValue("@DoesNotRequireAnnualRecertFlag", "0"); // or appropriate default
                        parameters.AddWithValue("@IncomeMinimumSetAside", (object)DBNull.Value); // or appropriate default

                        cmd.Transaction = transaction;
                        cmd.ExecuteNonQuery();
                    }
                    //Insert Builings Info to the progrms
                    UpdateBuildingToPrograms(conn, transaction, program, implementationReq, implementationRes);
                }
            }
            catch (Exception ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.ToString(),
                    message = "Error while creating/updating programs - Message:" + ex.Message,
                    name = "CreateUpdateProgram"
                });
                throw;
            }
        }

        public void UpdateBuildingToPrograms(SqlConnection conn, SqlTransaction transaction, Program program, DHTCSiteImplementationRequest request, ImplementationStatusValue implementationRes)
        {
            try
            {
                var tcAllocationtypes = GetTCAllocationType(conn, transaction, request);
                var programNames = GetProgramNamesList(conn, transaction, request);
                //extract Uatbale IDs 
                var buildingsFromDH = SafeTypes.SafeStringToIntArray(program.BuildingIds);
                List<int> buildingIdsForOS = new List<int>();
                foreach (var item in buildingsFromDH)
                {
                    var buildingID = request.DHTCBuildings.FirstOrDefault(b => b.Id == item)?.Id ?? 0;
                    if (buildingID > 0)
                    {
                        buildingIdsForOS.Add(buildingID);
                    }
                }

                foreach (var item in request.DHTCBuildings)
                {
                    var tcAllocationtypeID = tcAllocationtypes.FirstOrDefault(u => u.Name == ((LrtcTaxCreditAllocations) SafeTypes.ToInt(item.TaxCreditAllocation)).ToString())?.Value ?? 0;
                    var ProgramId = programNames.FirstOrDefault(p => string.Equals(p.Value, program.Name, StringComparison.OrdinalIgnoreCase))?.ID ?? 0;
                    program.OnesiteProgramId = ProgramId;

                    conn.ChangeDatabase("S" + request.SiteId);
                    using (SqlCommand cmd = new SqlCommand(_IKafkaConsumerDB.UpdateBuildingsToProgramsWithNoParams, conn))
                    {

                        cmd.CommandType = CommandType.StoredProcedure;
                        var parameters = cmd.Parameters;
                        // Basic identifiers
                        parameters.AddWithValue("@InternalEntityID", request.PmcId);
                        parameters.AddWithValue("@InternalUserID", 1); // Default user ID
                        parameters.AddWithValue("@InternalSiteID", request.SiteId);
                        parameters.AddWithValue("@SiteID", request.SiteId);
                        //apply selected buildings if there are building Ids, if not. it will apply to all buildings. 
                        if (buildingIdsForOS?.Count > 0)
                        {

                            var osbuilingID = buildingIdsForOS.Contains(item.Id) ? item.OnesiteBuildingID : 0;

                            parameters.AddWithValue("@buildingID", osbuilingID);
                            if (osbuilingID <= 0)
                                continue;
                        }
                        else
                        {
                            parameters.AddWithValue("@buildingID", item.OnesiteBuildingID);
                        }
                        parameters.AddWithValue("@programID", ProgramId);
                        parameters.AddWithValue("@BIN", item.Bin);
                        parameters.AddWithValue("@rentUpFlag", item.RentUp == "True" ? "1" : "0");
                        parameters.AddWithValue("@serviceDate", item.PlacedInServiceDate?.ToString());
                        parameters.AddWithValue("@AllocationValue", tcAllocationtypeID.ToString());
                        parameters.AddWithValue("@IncludeInProgram", "1");
                        parameters.AddWithValue("@ProgramType", program.OnesiteProgramTypeID);
                        parameters.AddWithValue("@applicableFractionGoal", (item.ApplicableFraction ?? 0.0000m).ToString("0.0000"));
                        //parameters.AddWithValue("@Post89Flag", "1");//default value

                        cmd.Transaction = transaction;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.ToString(),
                    message = "Error while creating/updating programs - Message:" + ex.Message,
                    name = "CreateUpdateProgram"
                });
                throw;
            }
        }

        public void CreateUpdateSetAside(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationReq, ImplementationStatusValue implementationRes)
        {
            try
            {
                var programNames = GetProgramNamesList(conn, transaction, implementationReq);
                var programtype = GetTaxCreditProgramType(conn, transaction, implementationReq);
                var setAsideDetails = GetSetAsideDetails(conn, transaction, implementationReq);
                var incomelimitSource = GetIncomeLimits(conn, transaction, implementationReq);

                foreach (var setAside in implementationReq.DHTCSetAsides)
                {
                    // General Settings
                    //get program
                    var program = implementationReq.DHTCPrograms.FirstOrDefault(p => p.Id == setAside.ProgramId);
                    //programname comapre
                    var ProgramId = programNames.FirstOrDefault(p => string.Equals(p.Value, program.Name, StringComparison.OrdinalIgnoreCase))?.ID ?? 0;
                    setAside.OSProgramId = ProgramId;
                    //get program type id
                    string programtypeDH = program.ProgramTypeName == (ProgramTypeEnum.Other).ToString() ? program.OtherProgramType : program.ProgramTypeName;
                    var programTypeID = programtype.FirstOrDefault(p => string.Equals(p.Name, programtypeDH, StringComparison.OrdinalIgnoreCase))?.Id;
                    setAside.OSProgramType = programTypeID;

                    var incomeLimitypeId = incomelimitSource
                                                        .FirstOrDefault(i => string.Equals(SafeTypes.TrimSpaces(i.OtherType), SafeTypes.TrimSpaces(program.IncomeLimitSourceName), StringComparison.OrdinalIgnoreCase))
                                                        ?.Id;

                    var setAsideDetail = setAsideDetails.FirstOrDefault(p => string.Equals(SafeTypes.TrimSpaces(p.SetAsideShortName), SafeTypes.TrimSpaces(setAside.ShortName), StringComparison.OrdinalIgnoreCase) && string.Equals(SafeTypes.TrimSpaces(p.SetAsideName), SafeTypes.TrimSpaces(setAside.Name), StringComparison.OrdinalIgnoreCase));
                    if (setAsideDetail != null)
                    {
                        setAside.OSSetAsideId = setAsideDetail.SetAsideID;
                        setAside.SpId = setAsideDetail.SpId;
                        setAsideDetail.ProgramID = ProgramId;
                        setAsideDetail.Tcptid = programTypeID??0;
                    }

                    conn.ChangeDatabase("S" + implementationReq.SiteId);
                    if ((setAside.OSSetAsideId == null || setAside.OSSetAsideId <= 0) && setAside.SpId <= 0)
                    {
                        using (SqlCommand cmd = new SqlCommand(_IKafkaConsumerDB.SaveSetAsideTempDetails, conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = transaction;
                            var parameters = cmd.Parameters;
                            // General Settings
                            // Basic identifiers
                            parameters.AddWithValue("@InternalEntityID", implementationReq.PmcId); // e.g., '4341841'
                            parameters.AddWithValue("@InternalUserID", 1); // Default user ID
                            parameters.AddWithValue("@InternalSiteID", implementationReq.SiteId); // e.g., '4341842'

                            // Program and Set-Aside details
                            parameters.AddWithValue("@tcptidnew", setAside.OSProgramType); // Example value
                            parameters.AddWithValue("@programnamenew", setAside.OSProgramId); // Example value

                            parameters.AddWithValue("@setAsideID", DBNull.Value); // Default value
                            parameters.AddWithValue("@setAsideName", setAside.Name); // Example name
                            parameters.AddWithValue("@setasideshortname", setAside.ShortName); // Example short name

                            // Date fields
                            parameters.AddWithValue("@allocationstartdate", DBNull.Value); // Default value
                            parameters.AddWithValue("@allocationEndDate", DBNull.Value); // Default value
                            parameters.AddWithValue("@recertificationUntilDate", DBNull.Value); // Default value
                            // Execute the stored procedure                                                                    
                            cmd.ExecuteNonQuery();
                        }
                        setAsideDetails = GetSetAsideDetails(conn, transaction, implementationReq, null);
                        setAsideDetail = setAsideDetails.FirstOrDefault(p => string.Equals(SafeTypes.TrimSpaces(p.SetAsideShortName), SafeTypes.TrimSpaces(setAside.ShortName), StringComparison.OrdinalIgnoreCase) && string.Equals(SafeTypes.TrimSpaces(p.SetAsideName), SafeTypes.TrimSpaces(setAside.Name), StringComparison.OrdinalIgnoreCase));

                    }
                    setAsideDetail.AllocationStartDate = setAside.StartDate;
                    //Set - aside Population
                    setAsideDetail.PercentageGoal = setAside.PopulationPercentage;
                    LrtcPopulationRestrictions restriction;

                    if (!string.IsNullOrEmpty(setAside.PopulationRestriction) &&
                        Enum.TryParse(setAside.PopulationRestriction, true, out restriction))
                    {

                        if (restriction == LrtcPopulationRestrictions.Buildings)
                        {
                            // Handle Buildings-specific logic
                        }

                        if (restriction == LrtcPopulationRestrictions.UnitTypes)
                        {
                            setAsideDetail.UnitTypesId = 1;
                        }

                        if (restriction == LrtcPopulationRestrictions.FloorPlans)
                        {
                            setAsideDetail.FloorPlansId = 1;
                        }

                        if (restriction == LrtcPopulationRestrictions.RequiredUnitsByBedrooms)
                        {
                            setAsideDetail.DefineRequiredUnitsFlag = 1;
                        }

                        if (restriction == LrtcPopulationRestrictions.RequiredUnitsByFloorPlan)
                        {
                            setAsideDetail.DefineRequiredUnitsFlag = 2;
                        }

                    }
                    // Home

                    var hometype = string.Empty;

                    if (!string.IsNullOrEmpty(setAside.HomeSetAsideType))
                    {
                        LrtcHomeSetAsideTypes lrtcHomeSetAsideTypes;
                        if (Enum.TryParse(setAside.HomeSetAsideType, true, out lrtcHomeSetAsideTypes))
                        {
                            hometype = lrtcHomeSetAsideTypes.ToString();

                            var homeTypes = GetSetAsideHomeTypes(conn, transaction, implementationReq);
                            foreach (var item in homeTypes)
                            {
                                var label = SafeTypes.TrimSpaces(item.Label).ToLower();
                                var target = SafeTypes.TrimSpaces(hometype).ToLower();

                                if (string.Equals(label, target, StringComparison.OrdinalIgnoreCase))
                                {
                                    setAsideDetail.HomeType = item.Value.ToString();
                                    break;
                                }
                            }
                        }
                    }

                    
                        //Income Restriction at Move-in 
                        setAsideDetail.MaximumIncomePercentMedian = setAside.MaxIncomeMedianPercentage;

                    // Income Restriction at Recertification
                    setAsideDetail.OverIncomePercent = setAside.OverIncomePercentage;
                    setAsideDetail.ApplicableIncomeLimitPercent = setAside.OverMedianIncome;

                    //Rent Restriction
                    setAsideDetail.MaxRentMedianIncomePercent = setAside.MaxRentMedianPercentage;
                    //Rent Restriction Others type
                    LrtcRentDeterminations rentDeterminations;

                    //if (!string.IsNullOrEmpty(setAside.MaxRentDetermination) &&
                    //    Enum.TryParse(setAside.MaxRentDetermination, true, out rentDeterminations))
                    //{

                    //    if (rentDeterminations == LrtcRentDeterminations.None)
                    //    {
                    //        setAsideDetail.DeterminationOfRentsId = 3;
                    //    }

                    //    else if (rentDeterminations == LrtcRentDeterminations.RentsByFloorplan)
                    //    {
                    //        setAsideDetail.DeterminationOfRentsId = 2;
                    //    }
                    //    //else
                    //    //{
                    //    //    setAsideDetail.DeterminationOfRentsId = 1;
                    //    //}

                    //}

                    
                    setAsideDetail.IncomeLimitTableIdIncomeReq = incomeLimitypeId ?? 0;
                    SaveSetAsideDetails(conn, transaction, implementationReq, implementationRes, setAsideDetail);
                    //Set - Aside Age - Based Requirements

                }
            }
            catch (SqlException ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.Message.ToString(),
                    message = "Error while creating/updating SetAside - CreateUpdateSetAside."
                });
                throw;
            }
        }

        public List<SetAsideDetail> GetSetAsideDetails(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, int? setAsideId = null)
        {
            List<SetAsideDetail> setAsideDetails = new List<SetAsideDetail>();
            const int F_RowNo = 0;
            const int F_SetAsideID = 1;
            const int F_ProgramID = 2;
            const int F_TcptIdNew = 3;
            const int F_ProgramNameNew = 4;
            const int F_TcptId = 5;
            const int F_ProgramType = 6;
            const int F_ProgramName = 7;
            const int F_SetAsideName = 8;
            const int F_SetAsideShortName = 9;
            const int F_AllocationStartDate = 10;
            const int F_RecertificationUntilDate = 11;
            const int F_AllocationEndDate = 12;
            const int F_MinimumSetAside = 13;
            const int F_OverIncomePercent = 14;
            const int F_MaxRentMedianIncomePercent = 15;
            const int F_PercentageGoal = 16;
            const int F_UnitOfPolution = 17;
            const int F_IncomeLimitTableIdForRent = 18;
            const int F_IncomeLimitTableIdIncomeReq = 19;
            const int F_IncomeLimitTableIdIncomeReqNonLihtc = 20;
            const int F_HoldHarmlessFlag = 21;
            const int F_DeterminationOfIncomeEligibility = 22;
            const int F_IncomeAmountSelected = 23;
            const int F_MaximumIncomeAmount = 24;
            const int F_MaxIncomeAmount_MoveInRequirement = 25;
            const int F_MinimumIncomeAmount = 26;
            const int F_MinIncomeAmount_MoveInRequirement = 27;
            const int F_MedianIncomeBySelected = 28;
            const int F_MaximumIncomePercentMedian = 29;
            const int F_MaximumIncomePercentMedianHome = 30;
            const int F_MinimumIncomePercentMedian = 31;
            const int F_MinIncomePercent_MoveInRequirement = 32;
            const int F_MaxIncomePercent_MoveInRequirement = 33;
            const int F_TimesRentBySelectedId = 34;
            const int F_MinimumIncomeTimesRent = 35;
            const int F_MinimumIncomeTimesRent_MoveInRequirement = 36;
            const int F_HouseholdsSelectedIds = 37;
            const int F_ApplyTransferMoveInHousehold = 38;
            const int F_MinimumHouseholdSizeChecked = 39;
            const int F_MinHouseholdSize = 40;
            const int F_MaximumHouseholdSize = 41;
            const int F_DeterminationOfRentsId = 42;
            const int F_NextAvailableUnitRule = 43;
            const int F_ChkMaxMinRentsId = 44;
            const int F_MaximumRentPercentageOf = 45;
            const int F_MaxRentPercentOfIncome = 46;
            const int F_MaxRentPercentOfGrossIncome = 47;
            const int F_MaxRentPercentOfPercentMedian = 48;
            const int F_MaxRentPercentOfMedian = 49;
            const int F_MaxRentCalcMethodHouseholdSizeFlag = 50;
            const int F_NumOfPersonsPerBedroom = 51;
            const int F_MinRentPercentage = 52;
            const int F_MinRentPercentOfMaxIncome = 53;
            const int F_MinRentPercentOfGrossIncome = 54;
            const int F_UnitTypesId = 55;
            const int F_FloorPlansId = 56;
            const int F_DefineRequiredUnitsFlag = 57;
            const int F_AssignVacantUnits = 58;
            const int F_ApplicableIncomeLimitPercent = 59;
            const int F_ApplyHomeRulesToRentUpOnlyFlag = 60;
            const int F_HomeType = 61;
            const int F_DeterminationOfRentsStartDate = 62;
            const int F_DeterminationOfRentsMaximumRentType = 63;
            const int F_DeterminationOfMedianIncomePercent = 64;
            //const int F_EditableRow = 65;
            //const int F_DeletableRow = 66;
            //const int F_HoldHarmlessFlag_EditableCol = 67;
            //const int F_SetAsideID_EditableCol = 68;
            //const int F_ProgramID_EditableCol = 69;
            //const int F_TcptId_EditableCol = 70;
            //const int F_ProgramType_EditableCol = 71;
            //const int F_ProgramName_EditableCol = 72;
            //const int F_SetAsideName_EditableCol = 73;
            //const int F_SetAsideShortName_EditableCol = 74;
            //const int F_AllocationStartDate_EditableCol = 75;
            //const int F_AllocationEndDate_EditableCol = 76;
            //const int F_RecertificationUntilDate_EditableCol = 77;
            //const int F_MinimumSetAside_EditableCol = 78;
            //const int F_OverIncomePercent_EditableCol = 79;
            //const int F_MaxRentMedianIncomePercent_EditableCol = 80;
            //const int F_PercentageGoal_EditableCol = 81;
            //const int F_MaximumIncomePercentMedian_EditableCol = 82;
            //const int F_IncomeLimitTableIdIncomeReq_EditableCol = 83;
            //const int F_IncomeLimitTableIdIncomeReqNonLihtc_EditableCol = 84;
            //const int F_DeterminationOfIncomeEligibility_EditableCol = 85;
            //const int F_IncomeAmountSelected_EditableCol = 86;
            //const int F_MaximumIncomeAmount_EditableCol = 87;
            //const int F_MaxIncomeAmount_MoveInRequirement_EditableCol = 88;
            //const int F_MinimumIncomeAmount_EditableCol = 89;
            //const int F_MinIncomeAmount_MoveInRequirement_EditableCol = 90;
            //const int F_MedianIncomeBySelected_EditableCol = 91;
            //const int F_MinimumIncomePercentMedian_EditableCol = 92;
            //const int F_MaxIncomePercent_MoveInRequirement_EditableCol = 93;
            //const int F_TimesRentBySelectedId_EditableCol = 94;
            //const int F_MinimumIncomeTimesRent_EditableCol = 95;
            //const int F_MinimumIncomeTimesRent_MoveInRequirement_EditableCol = 96;
            //const int F_HouseholdsSelectedIds_EditableCol = 97;
            //const int F_ApplyTransferMoveInHousehold_EditableCol = 98;
            //const int F_MinimumHouseholdSizeChecked_EditableCol = 99;
            //const int F_MinHouseholdSize_EditableCol = 100;
            //const int F_MaximumHouseholdSize_EditableCol = 101;
            //const int F_DeterminationOfRentsId_EditableCol = 102;
            //const int F_MaximumRentPercentageOf_EditableCol = 103;
            //const int F_MaxRentPercentOfIncome_EditableCol = 104;
            //const int F_MaxRentPercentOfGrossIncome_EditableCol = 105;
            //const int F_MaxRentPercentOfPercentMedian_EditableCol = 106;
            //const int F_MaxRentPercentOfMedian_EditableCol = 107;
            //const int F_MaxRentCalcMethodHouseholdSizeFlag_EditableCol = 108;
            //const int F_NumOfPersonsPerBedroom_EditableCol = 109;
            //const int F_MinRentPercentage_EditableCol = 110;
            //const int F_MinRentPercentOfMaxIncome_EditableCol = 111;
            //const int F_MinRentPercentOfGrossIncome_EditableCol = 112;
            //const int F_UnitOfPolution_EditableCol = 113;
            //const int F_UnitTypesId_EditableCol = 114;
            //const int F_FloorPlansId_EditableCol = 115;
            //const int F_PDefineRequiredUnits = 116;
            //const int F_DefineRequiredUnitsFlag_EditableCol = 117;
            //const int F_AssignVacantUnits_EditableCol = 118;
            //const int F_ApplicableIncomeLimitPercent_EditableCol = 119;
            //const int F_DefineRequiredUnitsFlag_HiddenCol = 120;
            //const int F_UnitTypesId_HiddenCol = 121;
            //const int F_FloorPlansId_HiddenCol = 122;
            //const int F_DeterminationOfRentsStartDate_EditableCol = 123;
            //const int F_DeterminationOfRentsMaximumRentType_EditableCol = 124;
            //const int F_DeterminationOfMedianIncomePercent_EditableCol = 125;
            const int F_SpId = 126;
            //const int F_TcptIdNew_HiddenCol = 127;
            //const int F_ProgramNameNew_HiddenCol = 128;
            //const int F_IsEdit = 129;
            //const int F_HideColumns = 130;

            Hashtable parameters = new Hashtable();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);
            parameters.Add("@setAsideID", setAsideId.HasValue ? setAsideId.Value : (object)DBNull.Value);

            string query = _IKafkaConsumerDB.GetSetAsideDetails;

            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    SetAsideDetail detail = new SetAsideDetail
                    {
                        RowNo = readhelper.GetInt(F_RowNo),
                        SetAsideID = readhelper.GetInt(F_SetAsideID),
                        ProgramID = readhelper.GetInt(F_ProgramID),
                        TcptidNew = readhelper.GetInt(F_TcptIdNew),
                        ProgramNameNew = readhelper.GetString(F_ProgramNameNew),
                        Tcptid = readhelper.GetInt(F_TcptId),
                        ProgramType = readhelper.GetString(F_ProgramType),
                        ProgramName = readhelper.GetString(F_ProgramName),
                        SetAsideName = readhelper.GetString(F_SetAsideName),
                        SetAsideShortName = readhelper.GetString(F_SetAsideShortName),
                        AllocationStartDate = readhelper.GetDateTime(F_AllocationStartDate),
                        RecertificationUntilDate = readhelper.GetDateTime(F_RecertificationUntilDate),
                        AllocationEndDate = readhelper.GetDateTime(F_AllocationEndDate),
                        MinimumSetAside = readhelper.GetDecimal(F_MinimumSetAside),
                        OverIncomePercent = readhelper.GetDecimal(F_OverIncomePercent),
                        MaxRentMedianIncomePercent = readhelper.GetDecimal(F_MaxRentMedianIncomePercent),
                        PercentageGoal = readhelper.GetDecimal(F_PercentageGoal),
                        UnitOfPolution = readhelper.GetString(F_UnitOfPolution),
                        IncomeLimitTableIdForRent = readhelper.GetInt(F_IncomeLimitTableIdForRent),
                        IncomeLimitTableIdIncomeReq = readhelper.GetInt(F_IncomeLimitTableIdIncomeReq),
                        IncomeLimitTableIdIncomeReqNonLihtc = readhelper.GetInt(F_IncomeLimitTableIdIncomeReqNonLihtc),
                        HoldHarmlessFlag = readhelper.GetBoolean(F_HoldHarmlessFlag),
                        DeterminationOfIncomeEligibility = readhelper.GetString(F_DeterminationOfIncomeEligibility),
                        IncomeAmountSelected = readhelper.GetString(F_IncomeAmountSelected),
                        MaximumIncomeAmount = readhelper.GetDecimal(F_MaximumIncomeAmount),
                        MaxIncomeAmount_MoveInRequirement = readhelper.GetDecimal(F_MaxIncomeAmount_MoveInRequirement),
                        MinimumIncomeAmount = readhelper.GetDecimal(F_MinimumIncomeAmount),
                        MinIncomeAmount_MoveInRequirement = readhelper.GetDecimal(F_MinIncomeAmount_MoveInRequirement),
                        MedianIncomeBySelected = readhelper.GetDecimal(F_MedianIncomeBySelected),
                        MaximumIncomePercentMedian = readhelper.GetDecimal(F_MaximumIncomePercentMedian),
                        MaximumIncomePercentMedianHome = readhelper.GetDecimal(F_MaximumIncomePercentMedianHome),
                        MinimumIncomePercentMedian = readhelper.GetDecimal(F_MinimumIncomePercentMedian),
                        MinIncomePercent_MoveInRequirement = readhelper.GetDecimal(F_MinIncomePercent_MoveInRequirement),
                        MaxIncomePercent_MoveInRequirement = readhelper.GetDecimal(F_MaxIncomePercent_MoveInRequirement),
                        TimesRentBySelectedId = readhelper.GetInt(F_TimesRentBySelectedId),
                        MinimumIncomeTimesRent = readhelper.GetDecimal(F_MinimumIncomeTimesRent),
                        MinimumIncomeTimesRent_MoveInRequirement = readhelper.GetDecimal(F_MinimumIncomeTimesRent_MoveInRequirement),
                        HouseholdsSelectedIds = readhelper.GetString(F_HouseholdsSelectedIds),
                        ApplyTransferMoveInHousehold = readhelper.GetBoolean(F_ApplyTransferMoveInHousehold),
                        MinimumHouseholdSizeChecked = readhelper.GetBoolean(F_MinimumHouseholdSizeChecked),
                        MinHouseholdSize = readhelper.GetInt(F_MinHouseholdSize),
                        MaxHouseholdSize = readhelper.GetInt(F_MaximumHouseholdSize),
                        DeterminationOfRentsId = readhelper.GetInt(F_DeterminationOfRentsId),
                        NextAvailableUnitRule = readhelper.GetString(F_NextAvailableUnitRule),
                        ChkMaxMinRentsId = readhelper.GetInt(F_ChkMaxMinRentsId),
                        MaximumRentPercentageOf = readhelper.GetDecimal(F_MaximumRentPercentageOf),
                        MaxRentPercentOfIncome = readhelper.GetDecimal(F_MaxRentPercentOfIncome),
                        MaxRentPercentOfGrossIncome = readhelper.GetDecimal(F_MaxRentPercentOfGrossIncome),
                        MaxRentPercentOfPercentMedian = readhelper.GetDecimal(F_MaxRentPercentOfPercentMedian),
                        MaxRentPercentOfMedian = readhelper.GetDecimal(F_MaxRentPercentOfMedian),
                        MaxRentCalcMethodHouseholdSizeFlag = readhelper.GetBoolean(F_MaxRentCalcMethodHouseholdSizeFlag),
                        NumOfPersonsPerBedroom = readhelper.GetInt(F_NumOfPersonsPerBedroom),
                        MinRentPercentage = readhelper.GetDecimal(F_MinRentPercentage),
                        MinRentPercentOfMaxIncome = readhelper.GetDecimal(F_MinRentPercentOfMaxIncome),
                        MinRentPercentOfGrossIncome = readhelper.GetDecimal(F_MinRentPercentOfGrossIncome),
                        UnitTypesId = readhelper.GetInt(F_UnitTypesId),
                        FloorPlansId = readhelper.GetInt(F_FloorPlansId),
                        DefineRequiredUnitsFlag = readhelper.GetInt(F_DefineRequiredUnitsFlag),
                        AssignVacantUnits = readhelper.GetBoolean(F_AssignVacantUnits),
                        ApplicableIncomeLimitPercent = readhelper.GetDecimal(F_ApplicableIncomeLimitPercent),
                        ApplyHomeRulesToRentUpOnlyFlag = readhelper.GetBoolean(F_ApplyHomeRulesToRentUpOnlyFlag),
                        HomeType = readhelper.GetString(F_HomeType),
                        DeterminationOfRentsStartDate = readhelper.GetDateTime(F_DeterminationOfRentsStartDate),
                        DeterminationOfRentsMaximumRentType = readhelper.GetString(F_DeterminationOfRentsMaximumRentType),
                        DeterminationOfMedianIncomePercent = readhelper.GetDecimal(F_DeterminationOfMedianIncomePercent),
                        SpId = readhelper.GetInt(F_SpId)
                    };
                    setAsideDetails.Add(detail);
                }
            }

            return setAsideDetails;
        }

        public void SaveSetAsideDetails(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationReq, ImplementationStatusValue implementationRes, SetAsideDetail setAside)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(_IKafkaConsumerDB.UpdateSetAsideDetails, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = transaction;

                    var parameters = cmd.Parameters;
                    #region  General Settings
                    //  General Identifiers
                    parameters.AddWithValue("@InternalEntityID", implementationReq.PmcId.ToString());
                    parameters.AddWithValue("@InternalUserID", 1); // Default user ID
                    parameters.AddWithValue("@InternalSiteID", implementationReq.SiteId.ToString());

                    //  Program and Set-Aside Details
                    parameters.AddWithValue("@setAsideID", setAside.SetAsideID.ToString());
                    parameters.AddWithValue("@programID", setAside.ProgramID.ToString());
                    parameters.AddWithValue("@tcptid", setAside.Tcptid.ToString());
                    parameters.AddWithValue("@programname", DBNull.Value); // default
                    parameters.AddWithValue("@setAsideName", setAside.SetAsideName.ToString() ?? (object)DBNull.Value);
                    parameters.AddWithValue("@setasideshortname", setAside.SetAsideShortName.ToString() ?? (object)DBNull.Value);

                    // Dates
                    parameters.AddWithValue("@allocationstartdate", setAside.AllocationStartDate ?? (object)DBNull.Value);
                    parameters.AddWithValue("@allocationEndDate", setAside.AllocationEndDate != DateTime.MinValue ? setAside.AllocationEndDate : (object)DBNull.Value);
                    parameters.AddWithValue("@recertificationUntilDate", setAside.RecertificationUntilDate != DateTime.MinValue ? setAside.RecertificationUntilDate : (object)DBNull.Value);
                    #endregion
                    // 🔹 Financials
                    parameters.AddWithValue("@MinimumSetAside", setAside.MinimumSetAside.ToString() ?? (object)DBNull.Value);
                    parameters.AddWithValue("@overincomepercent", setAside.OverIncomePercent.ToString() ?? (object)DBNull.Value);
                    parameters.AddWithValue("@maxrentmedianincomepercent", setAside.MaxRentMedianIncomePercent.ToString() ?? (object)DBNull.Value);
                    parameters.AddWithValue("@PercentageGoal", setAside.PercentageGoal ?? (object)DBNull.Value);
                    parameters.AddWithValue("@MaximumIncomePercentMedian", setAside.MaximumIncomePercentMedian.ToString() ?? (object)DBNull.Value);

                    // 🔹 Income Limits
                    parameters.AddWithValue("@incomelimittableidincomereq", setAside.IncomeLimitTableIdIncomeReq.ToString());
                    parameters.AddWithValue("@holdharmlessflag", setAside.HoldHarmlessFlag);
                    parameters.AddWithValue("@determinationofincomeeligibility", setAside.DeterminationOfIncomeEligibility.ToString() ?? (object)DBNull.Value);
                    parameters.AddWithValue("@IncomeAmountSelected", DBNull.Value);
                    parameters.AddWithValue("@MaximumIncomeAmount", DBNull.Value);
                    parameters.AddWithValue("@maxincomeamount_moveinrequirement", DBNull.Value);
                    parameters.AddWithValue("@minimumincomeamount", DBNull.Value);
                    parameters.AddWithValue("@minincomeamount_moveinrequirement", DBNull.Value);
                    parameters.AddWithValue("@medianincomebyselected", setAside.MedianIncomeBySelected.ToString());
                    parameters.AddWithValue("@minimumincomepercentmedian", DBNull.Value);
                    parameters.AddWithValue("@maxincomepercent_moveinrequirement", DBNull.Value);

                    // 🔹 Rent Calculations
                    parameters.AddWithValue("@timesrentbyselectedid", DBNull.Value);
                    parameters.AddWithValue("@minimumincometimesrent", DBNull.Value);
                    parameters.AddWithValue("@minimumincometimesrent_moveinrequirement", DBNull.Value);

                    // 🔹 Household Settings
                    parameters.AddWithValue("@householdsselectedids", DBNull.Value);
                    parameters.AddWithValue("@minimumhouseholdsizechecked", setAside.MinimumHouseholdSizeChecked.ToString());
                    parameters.AddWithValue("@minhouseholdsize", DBNull.Value);
                    parameters.AddWithValue("@maximumhouseholdsize", DBNull.Value);

                    // 🔹 Rent Determination
                    parameters.AddWithValue("@determinationofrentsid", setAside.DeterminationOfRentsId.ToString());
                    parameters.AddWithValue("@maximumrentpercentageof", DBNull.Value);
                    parameters.AddWithValue("@maxrentpercentofincome", DBNull.Value);
                    parameters.AddWithValue("@maxrentpercentofgrossincome", DBNull.Value);
                    parameters.AddWithValue("@maxrentpercentofpercentmedian", DBNull.Value);
                    parameters.AddWithValue("@maxrentpercentofmedian", DBNull.Value);
                    parameters.AddWithValue("@maxrentcalcmethodhouseholdsizeflag", DBNull.Value);
                    parameters.AddWithValue("@numofpersonsperbedroom", DBNull.Value);
                    parameters.AddWithValue("@minrentpercentage", DBNull.Value);
                    parameters.AddWithValue("@minrentpercentofmaxincome", DBNull.Value);
                    parameters.AddWithValue("@minrentpercentofgrossincome", DBNull.Value);

                    // 🔹 Miscellaneous
                    parameters.AddWithValue("@unitofpolution", DBNull.Value);
                    parameters.AddWithValue("@unittypesid", setAside.UnitTypesId.ToString());
                    parameters.AddWithValue("@floorplansid", setAside.FloorPlansId.ToString());
                    parameters.AddWithValue("@DefineRequiredUnitsFlag", setAside.DefineRequiredUnitsFlag.ToString());
                    parameters.AddWithValue("@assignvacantunits", DBNull.Value);
                    parameters.AddWithValue("@MinIncomePercent_moveinrequirement", DBNull.Value);
                    parameters.AddWithValue("@ApplicableIncomeLimitPercent", setAside.ApplicableIncomeLimitPercent.ToString() ?? (object)DBNull.Value);

                    // 🔹 Additional Income Fields
                    parameters.AddWithValue("@MaximumIncomePercentMedian_I", DBNull.Value);
                    parameters.AddWithValue("@MinimumIncomeAmount_I", DBNull.Value);
                    parameters.AddWithValue("@MinimumIncomePercentMedian_I", DBNull.Value);
                    parameters.AddWithValue("@MinimumIncomeTimesRent_I", DBNull.Value);
                    parameters.AddWithValue("@MinimumHouseholdSize_I", DBNull.Value);
                    parameters.AddWithValue("@MaximumHouseholdSize_I", DBNull.Value);
                    parameters.AddWithValue("@chkMaxMinRentsid", setAside.ChkMaxMinRentsId);
                    parameters.AddWithValue("@MaximumIncomeAmount_I", DBNull.Value);

                    //  HOME Rules
                    parameters.AddWithValue("@ApplyHomeRulesToRentUpOnlyFlag", DBNull.Value);
                    parameters.AddWithValue("@HomeType", DBNull.Value);

                    //  Additional Income Limit Tables
                    parameters.AddWithValue("@incomelimittableidincomereqnonlihtc", DBNull.Value);
                    parameters.AddWithValue("@MaximumIncomePercentMedianHome", DBNull.Value);
                    parameters.AddWithValue("@incomelimittableidforrent", setAside.IncomeLimitTableIdForRent);
                    parameters.AddWithValue("@applytransfermoveinhousehold", DBNull.Value);

                    //  Rent Start & Type
                    parameters.AddWithValue("@determinationOfRentsStartDate", DBNull.Value);
                    parameters.AddWithValue("@determinationOfRentsMaximumRentType", DBNull.Value);
                    parameters.AddWithValue("@determinationOfMedianIncomePercent", DBNull.Value);

                    //  Metadata
                    parameters.AddWithValue("@SpId", setAside.SpId.ToString());

                    //  Execute
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.Message.ToString(),
                    message = "Error while creating/updating SetAside - SaveSetAsideDetails."
                });
                throw;
            }
        }

        /// <summary>
        /// Gets required unit counts for tax credit Unit Counts
        /// </summary>
        /// <param name="conn">SQL connection</param>
        /// <param name="transaction">SQL transaction</param>
        /// <param name="implementationRequest">Implementation request</param>
        /// <returns>List of required unit count types</returns>
        public List<UnitCountType> GetRequiredUnitCountsTC(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_VALUE = 0;
            const int F_LABEL = 1;
            const int F_PARENTVALUE = 2;

            List<UnitCountType> unitCountTypes = new List<UnitCountType>();
            Hashtable parameters = new Hashtable();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GetRequiredUnitCountsTC;

            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    UnitCountType unitCountType = new UnitCountType()
                    {
                        Value = readhelper.GetInt(F_VALUE),
                        Label = readhelper.GetString(F_LABEL),
                        ParentValue = readhelper.GetInt(F_PARENTVALUE)
                    };
                    unitCountTypes.Add(unitCountType);
                }
            }
            return unitCountTypes;
        }

        public List<SetAsideRules> GetSetAsideRulesList(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            int F_NAME = 0;
            int F_ID = 1;
            Hashtable parameters = new Hashtable();
            List<SetAsideRules> setAsideRules = new List<SetAsideRules>();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GetAffordableSetAsideAssignedRules;

            // Execute and process results
            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    setAsideRules.Add(new SetAsideRules()
                    {
                        Lable = readhelper.GetString(F_ID),
                        Value = readhelper.GetString(F_NAME)
                    });
                }
            }
            return setAsideRules;
        }
        public List<TCAllocationType> GetTCAllocationType(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            int F_ID = 0;
            int F_VALUE = 1;
            int F_NAME = 2;
            Hashtable parameters = new Hashtable();
            List<TCAllocationType> tcAllocationType = new List<TCAllocationType>();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GetTCAllocationType;

            // Execute and process results
            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    tcAllocationType.Add(new TCAllocationType()
                    {
                        ID = readhelper.GetInt(F_ID),
                        Value = readhelper.GetInt(F_VALUE),
                        Name = readhelper.GetString(F_NAME)
                    });
                }
            }
            return tcAllocationType;
        }

        public void SaveSetAsideRules(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, ImplementationStatusValue response)
        {
            try
            {
                var rulesFromOS = GetSetAsideRulesList(conn, transaction, implementationRequest);
                string query = _IKafkaConsumerDB.SaveSetAsideRules;
                foreach (var rules in implementationRequest.DHTCSetAsideRules)
                {

                    var relationLabel = GetEnumValues.GetDescription((SetAsideRelations)short.Parse(rules.Relationship));
                    var saRule = rulesFromOS
                        .FirstOrDefault(r => string.Equals(r.Lable?.Trim(), relationLabel, StringComparison.InvariantCultureIgnoreCase))
                        ?.Value;

                    Hashtable parameters = new Hashtable();
                    parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                    parameters.Add("@InternalUserID", "1");
                    parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                    parameters.Add("@SAName", rules.PrimarySetAsideName);
                    parameters.Add("@RelaltedSaName", rules.SecondarySetAsideName);
                    parameters.Add("@SASRule", saRule);

                    ExecuteNonQuery(query, DBEntity.Site, parameters, transaction);
                }
            }
            catch (Exception ex)
            {
                response.errors.Add(new Error()
                {
                    value = ex.ToString(),
                    message = "Error occured while saving SetAsideRules " + ex.Message,
                    name = "SaveSetAsideRules"
                });
                throw;
            }
        }

        public void SaveRentFloors(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, ImplementationStatusValue response)
        {
            try
            {
                string query = _IKafkaConsumerDB.SaveRentFloors;
                
                foreach (var rentFloor in implementationRequest.DHTCRentFloors)
                {
                    // Get floor plan ID from OneSite by matching floor plan code
                    var fpId = _flooplansFromOS.FirstOrDefault(f => string.Equals(rentFloor.FloorPlanCode, f.FloorplanCode, StringComparison.OrdinalIgnoreCase))?.FloorplanId ?? 0;
                    
                    // Get set aside ID from OneSite by matching set aside short name
                    var setAsides = GetSetAsideDetails(conn, transaction, implementationRequest);
                    var setAsideID = setAsides.FirstOrDefault(s => string.Equals(s.SetAsideShortName?.Trim(), rentFloor.SetAsideShortName?.Trim(), StringComparison.OrdinalIgnoreCase))?.SetAsideID ?? 0;

                    if (fpId == 0 || setAsideID == 0)
                    {
                        var message = "Data mismatch with OneSite for Rent Floors - FloorPlanCode: " + rentFloor.FloorPlanCode + 
                                      ", FloorPlanID: " + fpId + ", SetAsideShortName: " + rentFloor.SetAsideShortName + ", SetAsideID: " + setAsideID;
                        response.errors.Add(new Error()
                        {
                            value = string.Empty,
                            message = message,
                            name = "SaveRentFloors"
                        });
                        throw new Exception(message);
                    }

                    Hashtable parameters = new Hashtable();
                    parameters.Add("@InternalEntityID", implementationRequest.PmcId);
                    parameters.Add("@InternalUserID", 1);
                    parameters.Add("@InternalSiteID", implementationRequest.SiteId);
                    parameters.Add("@SetAsideID", setAsideID);
                    parameters.Add("@FloorPlanID", fpId);
                    parameters.Add("@Amount", rentFloor.Amount?.ToString() ?? "0");

                    ExecuteNonQuery(query, DBEntity.Site, parameters, transaction);
                }
            }
            catch (Exception ex)
            {
                response.errors.Add(new Error()
                {
                    value = ex.ToString(),
                    message = "Error while saving Rent Floors - SaveRentFloors: " + ex.Message,
                    name = "SaveRentFloors"
                });
                throw;
            }
        }

        public List<SetAsideBuildingAssignment> GetSetAsideBuildingAssignments(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest, int? SetAsideID)
        {
            // Column Index Constants
            const int F_SAID = 0;
            const int F_PID = 1;
            const int F_SANAME = 2;
            const int F_SASHORTNAME = 3;
            const int F_SAPERCENTAGEGOAL = 4;
            const int F_SAALLOCATIONSTARTDATE = 5;
            const int F_SAALLOCATIONENDDATE = 6;
            const int F_PNAME = 7;
            const int F_PDESCRIPTION = 8;
            const int F_TCPTID = 9;
            const int F_TRACKCOMPLIANCEFLAG = 10;
            const int F_TCPTDESCRIPTION = 11;
            const int F_TCPTCODE = 12;
            const int F_BPIID = 13;
            const int F_BLDGID = 14;
            const int F_BPIEXEMPTFLAG = 15;
            const int F_BPIRENTUPFLAG = 16;
            const int F_BIN = 17;
            const int F_SERVICEDATE = 18;
            const int F_APPLICABLEFRACTIONGOAL = 19;
            const int F_FLAG89 = 20;
            const int F_ELECTDATE = 21;
            const int F_FLAG89DESC = 22;
            const int F_BLDGNUMBER = 23;
            const int F_BLDGNAME = 24;
            const int F_BLDGDESCRIPTION = 25;
            const int F_BLDGUNITCOUNT = 26;
            const int F_BSAIIDDISPLAY = 27;
            const int F_BSAIID = 28;
            const int F_BSAIAPPFRACGOAL = 29;
            const int F_BSAIAPPFRAC = 30;
            const int F_APPROVEDDATE = 31;
            const int F_REVOKEDDATE = 32;
            const int F_APPLIEDDATE = 33;
            const int F_UAID = 34;
            const int F_UANAME = 35;
            const int F_ENABLERENTFLOORMINIMUM = 36;
            const int F_PROGRAMETYPENAME = 37;
            const int F_TOTALUNITSPERSENT = 38;
            const int F_TOTALUNITSNUMBERS = 39;
            const int F_FEDERAL10COPTION = 40;
            const int F_ASSIGNVACANTUNITS = 41;
            const int F_DESIGNATEDUNITS = 42;
            const int F_DESIGNATEDUNITS_HIDDEN = 43;
            const int F_DESIGNATEDUNITPARENT = 44;
            const int F_DELETABLEROW = 45;

            List<SetAsideBuildingAssignment> assignments = new List<SetAsideBuildingAssignment>();
            Hashtable parameters = new Hashtable();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId.ToString());
            parameters.Add("@InternalUserID", "1"); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId.ToString());
            //parameters.Add("@setAsideID", SetAsideID.ToString());
            
            string query = _IKafkaConsumerDB.GetSetAsideBuildingAssignments;

            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {

                    DateTime sqlMinDate = new DateTime(1753, 1, 1);
                    SetAsideBuildingAssignment assignment = new SetAsideBuildingAssignment();

                    assignment.SaID = readhelper.GetInt(F_SAID);
                    assignment.PID = readhelper.GetInt(F_PID);
                    assignment.SaName = readhelper.GetString(F_SANAME);
                    assignment.SaShortName = readhelper.GetString(F_SASHORTNAME);
                    assignment.SaPercentageGoal = readhelper.GetDecimal(F_SAPERCENTAGEGOAL);
                    assignment.SaAllocationStartDate = readhelper.GetDateTime(F_SAALLOCATIONSTARTDATE);
                    assignment.SaAllocationEndDate = readhelper.GetDateTime(F_SAALLOCATIONENDDATE);
                    assignment.PName = readhelper.GetString(F_PNAME);
                    assignment.PDescription = readhelper.GetString(F_PDESCRIPTION);
                    assignment.TcptID = readhelper.GetInt(F_TCPTID);
                    assignment.PTrackCompliancePerBuildingFlag = readhelper.GetBoolean(F_TRACKCOMPLIANCEFLAG);
                    assignment.TcptDescription = readhelper.GetString(F_TCPTDESCRIPTION);
                    assignment.TcptCode = readhelper.GetString(F_TCPTCODE);
                    assignment.BpiID = readhelper.GetInt(F_BPIID);
                    assignment.BldgID = readhelper.GetInt(F_BLDGID);
                    assignment.BpiExemptFlag = readhelper.GetBoolean(F_BPIEXEMPTFLAG);
                    assignment.BpiRentupFlag = readhelper.GetBoolean(F_BPIRENTUPFLAG);
                    assignment.Bin = readhelper.GetString(F_BIN);                    
                    assignment.ServiceDate = readhelper.GetDateTime(F_SERVICEDATE);
                    assignment.ApplicableFractionGoal = readhelper.GetDecimal(F_APPLICABLEFRACTIONGOAL);
                    assignment.Flag89 = readhelper.GetBoolean(F_FLAG89);
                    assignment.ElectDate = readhelper.GetString(F_ELECTDATE);
                    assignment.Flag89Desc = readhelper.GetString(F_FLAG89DESC);
                    assignment.BldgNumber = readhelper.GetInt(F_BLDGNUMBER);
                    assignment.BldgName = readhelper.GetString(F_BLDGNAME);
                    assignment.BldgDescription = readhelper.GetString(F_BLDGDESCRIPTION);
                    assignment.BldgUnitCount = readhelper.GetInt(F_BLDGUNITCOUNT);
                    assignment.BsaiIDDisplay = readhelper.GetString(F_BSAIIDDISPLAY);
                    assignment.BsaiID = readhelper.GetInt(F_BSAIID);
                    assignment.BsaiAppFracGoal = readhelper.GetDecimal(F_BSAIAPPFRACGOAL);
                    assignment.BsaiAppFrac =  readhelper.GetDecimal(F_BSAIAPPFRAC);
                    assignment.ApprovedDate =  readhelper.GetDateTime(F_APPROVEDDATE);
                    assignment.RevokedDate =  readhelper.GetDateTime(F_REVOKEDDATE);
                    assignment.AppliedDate =  readhelper.GetDateTime(F_APPLIEDDATE);
                    assignment.UaID = readhelper.GetInt(F_UAID);
                    assignment.UaName = readhelper.GetString(F_UANAME);
                    assignment.EnableRentFloorMinimum = readhelper.GetBoolean(F_ENABLERENTFLOORMINIMUM);
                    assignment.ProgrameTypeName = readhelper.GetString(F_PROGRAMETYPENAME);
                    assignment.TotalUnitsPersent =  readhelper.GetDecimal(F_TOTALUNITSPERSENT);
                    assignment.TotalUnitsNumbers =  readhelper.GetInt(F_TOTALUNITSNUMBERS);
                    assignment.Federal10cOption = readhelper.GetString(F_FEDERAL10COPTION);
                    assignment.AssignVacantUnits = readhelper.GetString(F_ASSIGNVACANTUNITS);
                    assignment.DesignatedUnits = readhelper.GetString(F_DESIGNATEDUNITS);
                    assignment.DesignatedUnitsHiddenCol = readhelper.GetBoolean(F_DESIGNATEDUNITS_HIDDEN);
                    assignment.DesignatedUnitParent = readhelper.GetString(F_DESIGNATEDUNITPARENT);
                    assignment.DeletableRow = readhelper.GetBoolean(F_DELETABLEROW);

                    assignments.Add(assignment);
                }
            }

            return assignments;
        }

        public void SaveSetAsideBuildingAssignmentDetails(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationReq, ImplementationStatusValue implementationRes, SetAsideBuildingAssignment assignment)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(_IKafkaConsumerDB.AddEditSetAsideBuildingAssignmentWithNoParams, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = transaction;

                    var parameters = cmd.Parameters;

                    //  General Identifiers
                    parameters.AddWithValue("@InternalEntityID", implementationReq.PmcId.ToString());
                    parameters.AddWithValue("@InternalUserID", "1");
                    parameters.AddWithValue("@InternalSiteID", implementationReq.SiteId.ToString());

                    // Assignment Details
                    parameters.AddWithValue("@bsaiiddisplay", assignment.BsaiIDDisplay);
                    parameters.AddWithValue("@bldgID", assignment.BldgID);
                    parameters.AddWithValue("@saID", assignment.SaID);
                    parameters.AddWithValue("@pID", assignment.PID);
                    parameters.AddWithValue("@BIN", assignment.Bin ?? (object)DBNull.Value);
                    parameters.AddWithValue("@bpiRentupFlag", assignment.BpiRentupFlag);
                    parameters.AddWithValue("@ServiceDate", assignment.ServiceDate== DateTime.MinValue ? (object)DBNull.Value : assignment.ServiceDate);
                    parameters.AddWithValue("@Flag89", assignment.Flag89);
                    parameters.AddWithValue("@ElectDate", assignment.ServiceDate == DateTime.MinValue ? (object)DBNull.Value : assignment.ElectDate);
                    parameters.AddWithValue("@bsaiAppFracGoal", assignment.BsaiAppFracGoal ?? (object)DBNull.Value);
                    parameters.AddWithValue("@bsaiAppFrac", assignment.BsaiAppFrac ?? (object)DBNull.Value);
                    parameters.AddWithValue("@uaID", assignment.UaID);
                    parameters.AddWithValue("@bsaiID", assignment.BsaiID);
                    parameters.AddWithValue("@TotalUnitsPersent", assignment.TotalUnitsPersent > decimal.MinValue && assignment.TotalUnitsPersent < decimal.MaxValue ? assignment.TotalUnitsPersent : (object)DBNull.Value);
                    parameters.AddWithValue("@TotalUnitsNumbers", (assignment.TotalUnitsNumbers > int.MinValue && assignment.TotalUnitsNumbers < int.MaxValue)  ? assignment.TotalUnitsNumbers:(object)DBNull.Value);
                    parameters.AddWithValue("@bldgunitcount", assignment.BldgUnitCount);
                    parameters.AddWithValue("@EnableRentFloorMinimum", assignment.EnableRentFloorMinimum);
                    parameters.AddWithValue("@assignvacantunits", string.IsNullOrEmpty(assignment.AssignVacantUnits) ? (object)DBNull.Value : assignment.AssignVacantUnits);
                    parameters.AddWithValue("@designatedunits", string.IsNullOrEmpty(assignment.DesignatedUnits) ? (object)DBNull.Value : assignment.DesignatedUnits);

                    // Execute
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.Message,
                    message = "Error while saving SetAsideBuildingAssignment - SaveSetAsideBuildingAssignmentDetails."
                });
                throw;
            }
        }

        public List<SetAsideUnitTypeAssignment> GetSetAsideUnitTypeAssignments(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            // Column Index Constants
            const int F_PNAME = 0;
            const int F_PID = 1;
            const int F_SANAME = 2;
            const int F_SAID = 3;
            const int F_UNITTYPEID = 4;
            const int F_UNITDESCRIPTION = 5;
            const int F_APPLYPERCENTAGE = 6;
            const int F_UTPOPID = 7;
            const int F_EDITABLEROW = 8;
            const int F_DELETABLEROW = 9;
            const int F_PID_EDITABLE = 10;
            const int F_SAID_EDITABLE = 11;
            const int F_UNITDESCRIPTION_EDITABLE = 12;
            const int F_APPLYPERCENTAGE_EDITABLE = 13;

            List<SetAsideUnitTypeAssignment> assignments = new List<SetAsideUnitTypeAssignment>();
            Hashtable parameters = new Hashtable
                {
                    { "@InternalEntityID", implementationRequest.PmcId },
                    { "@InternalUserID", "1"},
                    { "@InternalSiteID", implementationRequest.SiteId }
                };

            string query = _IKafkaConsumerDB.GetSetAsideUnitTypeAssignments;

            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    var assignment = new SetAsideUnitTypeAssignment
                    {
                        ProgramName = readhelper.GetString(F_PNAME),
                        ProgramID = readhelper.GetInt(F_PID),
                        SetAsideName = readhelper.GetString(F_SANAME),
                        SetAsideID = readhelper.GetInt(F_SAID),
                        UnitTypeID = readhelper.GetInt(F_UNITTYPEID),
                        UnitDescription = readhelper.GetString(F_UNITDESCRIPTION),
                        ApplyPercentage = readhelper.GetBoolean(F_APPLYPERCENTAGE),
                        UnitTypePopulationID = readhelper.GetInt(F_UTPOPID),
                        EditableRow = readhelper.GetBoolean(F_EDITABLEROW),
                        DeletableRow = readhelper.GetBoolean(F_DELETABLEROW),
                        ProgramIDEditable = readhelper.GetBoolean(F_PID_EDITABLE),
                        SetAsideIDEditable = readhelper.GetBoolean(F_SAID_EDITABLE),
                        UnitDescriptionEditable = readhelper.GetBoolean(F_UNITDESCRIPTION_EDITABLE),
                        ApplyPercentageEditable = readhelper.GetBoolean(F_APPLYPERCENTAGE_EDITABLE)
                    };

                    assignments.Add(assignment);
                }
            }

            return assignments;
        }

        public void SaveSetAsideUnitTypeAssignment(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationReq, ImplementationStatusValue implementationRes, SetAsideUnitTypeAssignment unitTypeAssignment)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(_IKafkaConsumerDB.AssignSetAsideUnittypesUpdateNoParams, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = transaction;

                    var parameters = cmd.Parameters;

                    // 🔹 General Identifiers
                    parameters.AddWithValue("@InternalEntityID", implementationReq.PmcId.ToString());
                    parameters.AddWithValue("@InternalUserID", "1");
                    parameters.AddWithValue("@InternalSiteID", implementationReq.SiteId.ToString());

                    // 🔹 Unit Type Assignment Details
                    parameters.AddWithValue("@SaID", unitTypeAssignment.SetAsideID);
                    parameters.AddWithValue("@ApplyPercentage", unitTypeAssignment.ApplyPercentage);
                    parameters.AddWithValue("@Unitdescription", unitTypeAssignment.UnitDescription ?? (object)DBNull.Value);
                    parameters.AddWithValue("@UtPopId", unitTypeAssignment.UnitTypePopulationID);

                    // 🔹 Execute
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.Message,
                    message = "Error while saving SetAsideUnitTypeAssignment - SaveSetAsideUnitTypeAssignment."
                });
                throw;
            }
        }

        public List<SetAsideFloorPlanAssignment> GetSetAsideFloorPlanAssignments(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_PNAME = 0;
            const int F_PID = 1;
            const int F_SANAME = 2;
            const int F_SAID = 3;
            const int F_FP_ID = 4;
            const int F_FP_DESCRIPTION = 5;
            const int F_APPLYPERCENTAGE = 6;
            const int F_FP_POPID = 7;
            const int F_EDITABLEROW = 8;
            const int F_DELETABLEROW = 9;
            const int F_PID_EDITABLE = 10;
            const int F_SAID_EDITABLE = 11;
            const int F_UNITDESC_EDITABLE = 12;
            const int F_FP_DESC_EDITABLE = 13;
            const int F_APPLYPERCENTAGE_EDITABLE = 14;

            List<SetAsideFloorPlanAssignment> assignments = new List<SetAsideFloorPlanAssignment>();

            Hashtable parameters = new Hashtable();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GetSetAsideFloorPlanAssignments;

            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    SetAsideFloorPlanAssignment assignment = new SetAsideFloorPlanAssignment()
                    {
                        ProgramName = readhelper.GetString(F_PNAME),
                        ProgramID = readhelper.GetInt(F_PID),
                        SetAsideName = readhelper.GetString(F_SANAME),
                        SetAsideID = readhelper.GetInt(F_SAID),
                        FloorPlanID = readhelper.GetInt(F_FP_ID),
                        FloorPlanDescription = readhelper.GetString(F_FP_DESCRIPTION),
                        ApplyPercentage = readhelper.GetBoolean(F_APPLYPERCENTAGE),
                        FloorPlanPopulationID = readhelper.GetInt(F_FP_POPID),
                        EditableRow = readhelper.GetBoolean(F_EDITABLEROW),
                        DeletableRow = readhelper.GetBoolean(F_DELETABLEROW),
                        ProgramIDEditable = readhelper.GetBoolean(F_PID_EDITABLE),
                        SetAsideIDEditable = readhelper.GetBoolean(F_SAID_EDITABLE),
                        UnitDescriptionEditable = readhelper.GetBoolean(F_UNITDESC_EDITABLE),
                        FloorPlanDescriptionEditable = readhelper.GetBoolean(F_FP_DESC_EDITABLE),
                        ApplyPercentageEditable = readhelper.GetBoolean(F_APPLYPERCENTAGE_EDITABLE)
                    };
                    assignments.Add(assignment);
                }
            }

            return assignments;
        }


        public void SaveSetAsideFloorPlanAssignment(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationReq, ImplementationStatusValue implementationRes, SetAsideFloorPlanAssignment floorPlanAssignment)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(_IKafkaConsumerDB.AssignSetAsideFloorPlanUpdateNoParams, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = transaction;

                    var parameters = cmd.Parameters;

                    // 🔹 General Identifiers
                    parameters.AddWithValue("@InternalEntityID", implementationReq.PmcId.ToString());
                    parameters.AddWithValue("@InternalUserID", "1");
                    parameters.AddWithValue("@InternalSiteID", implementationReq.SiteId.ToString());

                    // 🔹 Floor Plan Assignment Details
                    parameters.AddWithValue("@SaID", floorPlanAssignment.SetAsideID);
                    parameters.AddWithValue("@ApplyPercentage", floorPlanAssignment.ApplyPercentage);
                    parameters.AddWithValue("@FPDescription", floorPlanAssignment.FloorPlanDescription ?? (object)DBNull.Value);
                    parameters.AddWithValue("@FPPopId", floorPlanAssignment.FloorPlanPopulationID);

                    // 🔹 Execute
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.Message,
                    message = "Error while saving SetAsideFloorPlanAssignment - SaveSetAsideFloorPlanAssignment."
                });
                throw;
            }
        }

        public List<SetAsideRequiredUnitsByFloorPlan> GetSetAsideRequiredUnitsByFloorPlan(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_SAFLOORPLANID = 0;
            const int F_PROGRAMTYPE = 1;
            const int F_PTID = 2;
            const int F_PNAME = 3;
            const int F_PID = 4;
            const int F_SAID = 5;
            const int F_FLOORPLANID = 6;
            const int F_FLOORPLANS = 7;
            const int F_NOOFUNITS = 8;
            const int F_REQUIREDUNITS = 9;
            const int F_EDITABLEROW = 10;
            const int F_DELETABLEROW = 11;
            const int F_PID_EDITABLE = 12;
            const int F_PTID_EDITABLE = 13;
            const int F_SAID_EDITABLE = 14;
            const int F_FLOORPLANS_EDITABLE = 15;
            const int F_NOOFUNITS_EDITABLE = 16;
            const int F_REQUIREDUNITS_EDITABLE = 17;

            List<SetAsideRequiredUnitsByFloorPlan> requiredUnitsList = new List<SetAsideRequiredUnitsByFloorPlan>();
            Hashtable parameters = new Hashtable();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", 1); // Default user ID
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GetSetAsideRequiredUnitsByFloorPlan;

            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    SetAsideRequiredUnitsByFloorPlan item = new SetAsideRequiredUnitsByFloorPlan()
                    {
                        SafloorPlanID = readhelper.GetInt(F_SAFLOORPLANID),
                        ProgramType = readhelper.GetString(F_PROGRAMTYPE),
                        PTID = readhelper.GetInt(F_PTID),
                        ProgramName = readhelper.GetString(F_PNAME),
                        PID = readhelper.GetInt(F_PID),
                        SaID = readhelper.GetInt(F_SAID),
                        FloorPlanID = readhelper.GetInt(F_FLOORPLANID),
                        FloorPlans = readhelper.GetString(F_FLOORPLANS),
                        NoOfUnits = readhelper.GetInt(F_NOOFUNITS),
                        RequiredUnits = readhelper.GetInt(F_REQUIREDUNITS),
                        EditableRow = readhelper.GetBoolean(F_EDITABLEROW),
                        DeletableRow = readhelper.GetBoolean(F_DELETABLEROW),
                        PIDEditable = readhelper.GetBoolean(F_PID_EDITABLE),
                        PTIDEditable = readhelper.GetBoolean(F_PTID_EDITABLE),
                        SaIDEditable = readhelper.GetBoolean(F_SAID_EDITABLE),
                        FloorPlansEditable = readhelper.GetBoolean(F_FLOORPLANS_EDITABLE),
                        NoOfUnitsEditable = readhelper.GetBoolean(F_NOOFUNITS_EDITABLE),
                        RequiredUnitsEditable = readhelper.GetBoolean(F_REQUIREDUNITS_EDITABLE)
                    };
                    requiredUnitsList.Add(item);
                }
            }

            return requiredUnitsList;
        }

        public void SaveSetAsideRequiredUnitsByFloorPlan(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationReq, ImplementationStatusValue implementationRes, SetAsideRequiredUnitsByFloorPlan unitData)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(_IKafkaConsumerDB.AssignSetAsideRequiredUnitsByFloorPlanNoParams, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = transaction;

                    var parameters = cmd.Parameters;

                    // 🔹 General Identifiers
                    parameters.AddWithValue("@InternalEntityID", implementationReq.PmcId.ToString());
                    parameters.AddWithValue("@InternalUserID", "1");
                    parameters.AddWithValue("@InternalSiteID", implementationReq.SiteId.ToString());

                    // 🔹 Required Unit Details
                    parameters.AddWithValue("@SaID", unitData.SaID.ToString());
                    parameters.AddWithValue("@NoOfUnits", unitData.NoOfUnits.ToString());
                    parameters.AddWithValue("@SafloorPlanID", unitData.SafloorPlanID.ToString());
                    parameters.AddWithValue("@RequiredUnits", unitData.RequiredUnits.ToString());

                    // 🔹 Execute
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.Message,
                    message = "Error while saving Required Units by Floor Plan - SaveSetAsideRequiredUnitsByFloorPlan."
                });
                throw;
            }
        }

        public List<SetAsideRequiredUnitsByBedroomCount> GetSetAsideRequiredUnitsByBedroomCount(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            const int F_BEDROOMCOUNTID = 0;
            const int F_PROGRAMTYPE = 1;
            const int F_PTID = 2;
            const int F_PNAME = 3;
            const int F_PID = 4;
            const int F_SAID = 5;
            const int F_BEDROOMCOUNT = 6;
            const int F_NOOFUNITS = 7;
            const int F_REQUIREDUNITS = 8;
            const int F_EDITABLEROW = 9;
            const int F_DELETABLEROW = 10;
            const int F_PID_EDITABLE = 11;
            const int F_PTID_EDITABLE = 12;
            const int F_SAID_EDITABLE = 13;
            const int F_BEDROOMCOUNT_EDITABLE = 14;
            const int F_NOOFUNITS_EDITABLE = 15;
            const int F_REQUIREDUNITS_EDITABLE = 16;

            List<SetAsideRequiredUnitsByBedroomCount> bedroomUnitList = new List<SetAsideRequiredUnitsByBedroomCount>();
            Hashtable parameters = new Hashtable();
            parameters.Add("@InternalEntityID", implementationRequest.PmcId);
            parameters.Add("@InternalUserID", "1");
            parameters.Add("@InternalSiteID", implementationRequest.SiteId);

            string query = _IKafkaConsumerDB.GetSetAsideRequiredUnitsByBedroomCount;

            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readhelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    SetAsideRequiredUnitsByBedroomCount item = new SetAsideRequiredUnitsByBedroomCount()
                    {
                        BedroomCountID = readhelper.GetInt(F_BEDROOMCOUNTID),
                        ProgramType = readhelper.GetString(F_PROGRAMTYPE),
                        PTID = readhelper.GetInt(F_PTID),
                        ProgramName = readhelper.GetString(F_PNAME),
                        PID = readhelper.GetInt(F_PID),
                        SaID = readhelper.GetInt(F_SAID),
                        BedroomCount = readhelper.GetInt(F_BEDROOMCOUNT),
                        NoOfUnits = readhelper.GetInt(F_NOOFUNITS),
                        RequiredUnits = readhelper.GetInt(F_REQUIREDUNITS),
                        EditableRow = readhelper.GetBoolean(F_EDITABLEROW),
                        DeletableRow = readhelper.GetBoolean(F_DELETABLEROW),
                        PIDEditable = readhelper.GetBoolean(F_PID_EDITABLE),
                        PTIDEditable = readhelper.GetBoolean(F_PTID_EDITABLE),
                        SaIDEditable = readhelper.GetBoolean(F_SAID_EDITABLE),
                        BedroomCountEditable = readhelper.GetBoolean(F_BEDROOMCOUNT_EDITABLE),
                        NoOfUnitsEditable = readhelper.GetBoolean(F_NOOFUNITS_EDITABLE),
                        RequiredUnitsEditable = readhelper.GetBoolean(F_REQUIREDUNITS_EDITABLE)
                    };
                    bedroomUnitList.Add(item);
                }
            }

            return bedroomUnitList;
        }

        public void SaveSetAsideRequiredUnitsByBedroomCount(
                                                            SqlConnection conn,
                                                            SqlTransaction transaction,
                                                            DHTCSiteImplementationRequest implementationReq,
                                                            ImplementationStatusValue implementationRes,
                                                            SetAsideRequiredUnitsByBedroomCount bedroomUnitData)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(_IKafkaConsumerDB.SaveSetAsideRequiredUnitsByBedroomCountNoParams, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Transaction = transaction;

                    var parameters = cmd.Parameters;

                    // 🔹 General Identifiers
                    parameters.AddWithValue("@InternalEntityID", implementationReq.PmcId.ToString());
                    parameters.AddWithValue("@InternalUserID", "1");
                    parameters.AddWithValue("@InternalSiteID", implementationReq.SiteId.ToString());

                    // 🔹 Bedroom Count Assignment Details
                    parameters.AddWithValue("@SaID", bedroomUnitData.SaID.ToString());
                    parameters.AddWithValue("@NoOfUnits", bedroomUnitData.NoOfUnits.ToString());
                    parameters.AddWithValue("@BedroomCountID", bedroomUnitData.BedroomCountID.ToString());
                    parameters.AddWithValue("@RequiredUnits", bedroomUnitData.RequiredUnits.ToString());

                    // 🔹 Execute
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.Message,
                    message = "Error while saving Required Units by Bedroom Count - SaveSetAsideRequiredUnitsByBedroomCount."
                });
                throw;
            }
        }

        public void UpdatePopulationRestriction(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationReq, ImplementationStatusValue implementationRes)
        {
            try
            {
                var setAsideDetails = GetSetAsideDetails(conn, transaction, implementationReq);
                var setAsideBuildings = GetSetAsideBuildingAssignments(conn, transaction, implementationReq, 0);
                var setAsideUnitTypes = GetSetAsideUnitTypeAssignments(conn, transaction, implementationReq);
                var setAsideFlpnAss = GetSetAsideFloorPlanAssignments(conn, transaction, implementationReq);
                var setAsidereqUFloorplan = GetSetAsideRequiredUnitsByFloorPlan(conn, transaction, implementationReq);
                var setAsideReqUnitsByBedrooms = GetSetAsideRequiredUnitsByBedroomCount(conn, transaction, implementationReq);
                // Fetch existing unit types and group mappings
                List<UnitType> unitTypes = GetTCUnittypes(conn, transaction, implementationReq);
                var uaSourcesTable = GetUtilityAllowanceSources(conn, transaction, implementationReq);
                foreach (var setAside in implementationReq.DHTCSetAsides)
                {

                    var setAsideDetail = setAsideDetails.FirstOrDefault(p => string.Equals(SafeTypes.TrimSpaces(p.SetAsideShortName), SafeTypes.TrimSpaces(setAside.ShortName), StringComparison.OrdinalIgnoreCase) && string.Equals(SafeTypes.TrimSpaces(p.SetAsideName), SafeTypes.TrimSpaces(setAside.Name), StringComparison.OrdinalIgnoreCase));
                    // string to array converstion
                    var buildingids = SafeTypes.SafeStringToIntArray(setAside.BuildingIds);
                    var unitTypeIds = SafeTypes.SafeStringToIntArray(setAside.UnitTypeIds);
                    var floorPlanIds = SafeTypes.SafeStringToIntArray(setAside.FloorPlanIds);
                    var unitsByBedroom = SafeStringToList(setAside.UnitsByBedroom);
                    var unitsByFloorPlan = SafeStringToList(setAside.UnitsByFloorPlan);

                    //Set - aside Population
                    LrtcPopulationRestrictions restriction;

                    if (!string.IsNullOrEmpty(setAside.PopulationRestriction) &&
                        Enum.TryParse(setAside.PopulationRestriction, true, out restriction))
                    {

                        if (restriction == LrtcPopulationRestrictions.Buildings)
                        {
                            var setAsideBuilding = setAsideBuildings.Where(f => f.SaID == setAsideDetail.SetAsideID && string.Equals(SafeTypes.TrimSpaces(f.SaShortName), SafeTypes.TrimSpaces(setAsideDetail.SetAsideShortName), StringComparison.OrdinalIgnoreCase)).ToList();

                            var filteredBuildings = implementationReq.DHTCBuildings
                                                                            .Where(b => buildingids.Contains(b.Id))
                                                                            .ToList();
                            var missingBuildings = filteredBuildings
                                                                 .Where(fb => !setAsideBuilding.Any(sb => sb.BldgID == fb.OnesiteBuildingID))
                                                                 .ToList();
                            foreach (var building in setAsideBuilding)
                            {
                                bool isBuilding = filteredBuildings.Any(f => f.OnesiteBuildingID == building.BldgID);
                                bool isupdate = false;

                                if (isBuilding)
                                {
                                    if (building.BsaiIDDisplay == "0")
                                    {
                                        building.BsaiIDDisplay = "1";
                                        isupdate = true;
                                    }
                                }
                                else
                                {
                                    if (building.BsaiIDDisplay == "1")
                                    {
                                        building.BsaiIDDisplay = "0";
                                        isupdate = true;
                                    }

                                }
                                if (isupdate)
                                {
                                    //update set Aside Building
                                    SaveSetAsideBuildingAssignmentDetails(conn, transaction, implementationReq, implementationRes, building);
                                }

                            }
                            foreach (var building in missingBuildings)
                            {
                                SetAsideBuildingAssignment setAsideBuildingAssignment = new SetAsideBuildingAssignment();
                                setAsideBuildingAssignment.BsaiIDDisplay = "1";
                                setAsideBuildingAssignment.BldgID = building.OnesiteBuildingID;
                                setAsideBuildingAssignment.SaID = setAsideDetail.SetAsideID;
                                setAsideBuildingAssignment.PID = setAsideDetail.ProgramID;
                                setAsideBuildingAssignment.Bin = building.Bin;
                                setAsideBuildingAssignment.BpiRentupFlag = building.RentUp == "1" ? true : false;
                                setAsideBuildingAssignment.ServiceDate = building.PlacedInServiceDate;
                                //missing  need to implement 
                                //setAsideBuildingAssignment.Flag89=building.MaxRentMethod
                                //setAsideBuildingAssignment.ElectDate=
                                setAsideBuildingAssignment.BsaiAppFracGoal = building.ApplicableFraction;
                                var program = implementationReq.DHTCPrograms.FirstOrDefault(f => f.Id == setAside.ProgramId);

                                if (program != null)
                                {
                                    var defaultUaSourceID = uaSourcesTable
                                                        .FirstOrDefault(u => string.Equals(
                                                            u.Name,
                                                            program.DefaultUASourceName,
                                                            StringComparison.InvariantCultureIgnoreCase))?.Id ?? null;
                                    setAsideBuildingAssignment.UaID = defaultUaSourceID ?? 0;
                                }
                                SaveSetAsideBuildingAssignmentDetails(conn, transaction, implementationReq, implementationRes, setAsideBuildingAssignment);
                            }

                        }

                        if (restriction == LrtcPopulationRestrictions.UnitTypes)
                        {
                            // Get all unit types assigned to the current SetAside and Program
                            var setAsideUnitTypesList = setAsideUnitTypes
                                .Where(f => f.ProgramID == setAsideDetail.ProgramID && f.SetAsideID == setAsideDetail.SetAsideID)
                                .ToList();

                            // Filter DataHub unit types based on provided unitTypeIds
                            var filteredUnitTypes = implementationReq.DHTCUnitTypes
                                .Where(ut => unitTypeIds.Contains(ut.Id))
                                .ToList();

                            // Identify unit types in DataHub that are missing from SetAside assignments
                            var missingUnitTypes = filteredUnitTypes
                                .Where(ut => !setAsideUnitTypesList.Any(sa =>
                                    string.Equals(
                                        SafeTypes.TrimSpaces(ut.Name),
                                        SafeTypes.TrimSpaces(sa.UnitDescription),
                                        StringComparison.OrdinalIgnoreCase)))
                                .ToList();

                            // Create a lookup for quick matching
                            var unitTypeLookup = filteredUnitTypes
                                .ToDictionary(
                                    ut => SafeTypes.TrimSpaces(ut.Name),
                                    ut => ut.Id,
                                    StringComparer.OrdinalIgnoreCase);

                            // Update ApplyPercentage flag based on matched unit types
                            foreach (var unitTypeAssignment in setAsideUnitTypesList)
                            {
                                var trimmedDescription = SafeTypes.TrimSpaces(unitTypeAssignment.UnitDescription);
                                bool isMatched = unitTypeLookup.ContainsKey(trimmedDescription);

                                // Only update if there's a change in ApplyPercentage status
                                if (unitTypeAssignment.ApplyPercentage != isMatched)
                                {
                                    unitTypeAssignment.ApplyPercentage = isMatched;
                                    SaveSetAsideUnitTypeAssignment(conn, transaction, implementationReq, implementationRes, unitTypeAssignment);
                                }
                            }
                        }                      

                        if (restriction == LrtcPopulationRestrictions.FloorPlans)
                        {
                            // Get all floor plans assigned to the current SetAside and Program
                            var setAsideFloorPlans = setAsideFlpnAss
                                .Where(f => f.ProgramID == setAsideDetail.ProgramID && f.SetAsideID == setAsideDetail.SetAsideID)
                                .ToList();

                            // Filter DataHub floor plans based on provided floorPlanIds
                            var dhFilteredFloorPlans = implementationReq.DHTCFloorplan
                                .Where(fp => floorPlanIds.Contains(fp.FloorplanId))
                                .ToList();

                            // Identify floor plans in DataHub that are missing from SetAside assignments
                            var missingFloorPlans = dhFilteredFloorPlans
                                .Where(fp => !setAsideFloorPlans.Any(sa =>
                                    string.Equals(
                                        SafeTypes.TrimSpaces(fp.FloorplanCode),
                                        SafeTypes.TrimSpaces(sa.FloorPlanDescription.Split(' ')[0]),
                                        StringComparison.OrdinalIgnoreCase)))
                                .ToList();

                            // Match floor plans from OS with those in DataHub
                            var matchedFromOS = _flooplansFromOS
                                .Where(os => dhFilteredFloorPlans.Any(fp =>
                                    string.Equals(os.FloorplanCode, fp.FloorplanCode, StringComparison.OrdinalIgnoreCase)))
                                .ToList();

                            // Update ApplyPercentage flag based on matched floor plans
                            foreach (var assignment in setAsideFloorPlans)
                            {
                                bool isMatched = matchedFromOS.Any(os => os.FloorplanId == assignment.FloorPlanID);

                                // Only update if there's a change in ApplyPercentage status
                                if (assignment.ApplyPercentage != isMatched)
                                {
                                    assignment.ApplyPercentage = isMatched;
                                    SaveSetAsideFloorPlanAssignment(conn, transaction, implementationReq, implementationRes, assignment);
                                }
                            }
                        }

                        if (restriction == LrtcPopulationRestrictions.RequiredUnitsByFloorPlan)
                        {
                            // Get all floor plans assigned to the current SetAside and Program
                            var setAsideFloorPlans = setAsidereqUFloorplan
                                .Where(f => f.PID == setAsideDetail.ProgramID && f.SaID == setAsideDetail.SetAsideID)
                                .ToList();
                            // Filter DataHub floor plans based on provided floorPlanIds
                            var dhFilteredFloorPlans = implementationReq.DHTCFloorplan
                                                        .Where(fp => unitsByFloorPlan.Any(u => u.ID == fp.FloorplanId))
                                                        .Select(fp =>
                                                        {
                                                            var unit = unitsByFloorPlan.FirstOrDefault(u => u.ID == fp.FloorplanId);
                                                            return new
                                                            {
                                                                fp.FloorplanId,
                                                                fp.FloorplanCode,
                                                                fp.UnitCount,
                                                                Count = unit != null ? unit.Count : 0
                                                            };
                                                        })
                                                        .ToList();


                            // Match floor plans from OS with those in DataHub
                            var matchedFromOS = _flooplansFromOS
                                .Where(os => dhFilteredFloorPlans.Any(fp =>
                                    string.Equals(os.FloorplanCode, fp.FloorplanCode, StringComparison.OrdinalIgnoreCase)))
                                .Select(fp =>
                                {
                                    var unit = dhFilteredFloorPlans.FirstOrDefault(u => string.Equals(u.FloorplanCode, fp.FloorplanCode, StringComparison.OrdinalIgnoreCase));
                                    return new
                                    {
                                        fp,
                                        Count = unit != null ? unit.Count : 0
                                    };
                                })
                                .ToList();

                            // Update ApplyPercentage flag based on matched floor plans
                            foreach (var assignment in setAsideFloorPlans)
                            {
                                var count = matchedFromOS.FirstOrDefault(os => os.fp.FloorplanId == assignment.FloorPlanID);

                                // Only update if there's a change in ApplyPercentage status

                                if (count!=null)
                                {
                                    assignment.RequiredUnits = count.Count??0;
                                    SaveSetAsideRequiredUnitsByFloorPlan(conn, transaction, implementationReq, implementationRes, assignment);
                                }
                            }
                        }

                        if (restriction == LrtcPopulationRestrictions.RequiredUnitsByBedrooms)
                        {
                        }

                    }
                }
            }
            catch (SqlException ex)
            {
                implementationRes.errors.Add(new Error()
                {
                    value = ex.Message.ToString(),
                    message = "Error while creating/updating SetAside - CreateUpdateSetAside."
                });
                throw;
            }
        }

        public List<LookupItem> GetSetAsideHomeTypes(SqlConnection conn, SqlTransaction transaction, DHTCSiteImplementationRequest implementationRequest)
        {
            int F_VALUE = 0;
            int F_LABEL = 1;

            var parameters = new Hashtable
            {
                { "@InternalEntityID", implementationRequest.PmcId },
                { "@InternalUserID", "1" }, // Default user ID
                { "@InternalSiteID", implementationRequest.SiteId }
            };

            var homeTypes = new List<LookupItem>();
            string query = _IKafkaConsumerDB.GetSetAsideHomeTypesQuery; // Stored procedure name

            using (SqlDataReader reader = ExecuteSqlDataReader(query, parameters, conn, transaction))
            {
                SqlDataReaderHelper readHelper = new SqlDataReaderHelper(reader);
                while (reader.Read())
                {
                    homeTypes.Add(new LookupItem
                    {
                        Value = readHelper.GetInt(F_VALUE),
                        Label = readHelper.GetString(F_LABEL)
                    });
                }
            }

            return homeTypes;
        }

        public List<PopulationRestrictionLookup> SafeStringToList(string input)
        {
            var result = new List<PopulationRestrictionLookup>();

            if (string.IsNullOrWhiteSpace(input))
                return result;

            var entries = input.Split(new[] { "], [" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var entry in entries)
            {
                var cleaned = entry.Replace("[", "").Replace("]", "").Trim();
                var parts = cleaned.Split('|');

                if (parts.Length == 2)
                {
                    string idstr = parts[0].Trim();
                    string valueStr = parts[1].Trim();

                    int? count = null;
                    int parsedValue;
                    if (int.TryParse(valueStr, out  parsedValue))
                    {
                        count = parsedValue;
                    }
                    int? id = null;
                    if (int.TryParse(idstr, out parsedValue))
                    {
                        id = parsedValue;
                    }

                    result.Add(new PopulationRestrictionLookup
                    {
                        ID = id,
                        Count = count
                    });
                }
            }

            return result;
        }

        #endregion
    }
}
