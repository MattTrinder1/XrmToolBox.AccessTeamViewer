namespace MoD.CAMS.Plugins.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.IO;
    using System.Runtime.Serialization;

    using Microsoft.Xrm.Sdk.Messages;
    using Microsoft.Crm.Sdk.Messages;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk.Metadata;
    using System.Xml;
    using Microsoft.Xrm.Sdk.Metadata.Query;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class DynamicsServiceHelper
    {
        #region constants

        private const string ROLENAME = "NonExecutionRole";

        #endregion


        public static void Assign(this IOrganizationService service, EntityReference target, EntityReference assignee)
        {
            var req = new AssignRequest();
            req.Assignee = assignee;
            req.Target = target;
            service.Execute(req);
        }

        public static void Assign(this IOrganizationService service, Entity target, Entity assignee)
        {
            Assign(service, target.ToEntityReference(), assignee.ToEntityReference());
        }

        public static void Assign(this IOrganizationService service, EntityReference target, Entity assignee)
        {
            Assign(service, target, assignee.ToEntityReference());
        }

        public static void Assign(this IOrganizationService service, Entity target, EntityReference assignee)
        {
            Assign(service, target.ToEntityReference(), assignee);
        }

        public static void RevokeSharedAccess(this IOrganizationService service, EntityReference target, EntityReference revokee)
        {
            var revoke = new RevokeAccessRequest();
            revoke.Target = target;
            revoke.Revokee = revokee ;
            service.Execute(revoke);
        }

        public static T GetValue<T>(this Entity entity, string attributeName)
        {
            //get the attribute

            if (entity.Attributes.ContainsKey(attributeName))
            {
                var attr = entity[attributeName];
                if (attr is AliasedValue)
                {
                    return GetAliasedValueValue<T>(entity, attributeName);
                }
                else
                {
                    return entity.GetAttributeValue<T>(attributeName);
                }
            }
            else
            {
                return default(T);
            }

        }

        public static decimal SumMoneyField(this IEnumerable<Entity> entities, string fieldName)
        {
            return entities.Where(x => x.Attributes.ContainsKey(fieldName))
                           .Sum(x => x.GetValue<Money>(fieldName).Value);
        }

        public static T GetAliasedValueValue<T>(this Entity entity, string attributeName)
        {
            var attr = entity.GetAttributeValue<AliasedValue>(attributeName);
            if (attr != null)
            {
                return (T)attr.Value;
            }
            else
            {
                return default(T);
            }

        }

        public static Entity CloneEntity(Entity entityToClone)
        {
            //does not work in sandbox - use cloneentitysandbox instead
            var earlyBoundSerializer = new DataContractSerializer(typeof(Entity));
            var newEntity = new Entity();
            using (var stream = new MemoryStream())
            {
                // Write the XML to disk.B
                earlyBoundSerializer.WriteObject(stream, entityToClone);
                stream.Position = 0;

                newEntity = (Entity)earlyBoundSerializer.ReadObject(stream);

            }

            return newEntity;

        }

        public static Entity CloneEntitySandbox(Entity entityToClone, string newLogicalName = "")
        {
            Entity newEntity;

            if (newLogicalName != string.Empty)
            {
                newEntity = new Entity(newLogicalName);
            }
            else
            {
                newEntity = new Entity(entityToClone.LogicalName);
            }

            var systemAttributes = new List<string>();
            systemAttributes.Add("createdon");
            systemAttributes.Add("createdby");
            systemAttributes.Add("modifiedon");
            systemAttributes.Add("modifiedby");
            systemAttributes.Add("owninguser");
            systemAttributes.Add("owningbusinessunit");
            systemAttributes.Add("organizationid");


            foreach (var attribute in entityToClone.Attributes
                .Where(x => x.Key != entityToClone.LogicalName + "id")
                .Where(x => !systemAttributes.Contains(x.Key))) 
            {

                switch (attribute.Value.GetType().Name)
                {
                    case "Money":
                        var m = attribute.Value as Money;
                        newEntity[attribute.Key] = new Money(m.Value);
                        break;
                    case "EntityReference":
                        var er = attribute.Value as EntityReference;
                        newEntity[attribute.Key] = new EntityReference(er.LogicalName, er.Id);
                        break;
                    case "OptionSetValue":
                        var os = attribute.Value as OptionSetValue;
                        newEntity[attribute.Key] = new OptionSetValue(os.Value);
                        break;
                    default:
                        newEntity[attribute.Key] = attribute.Value;
                        break;
                }
                
            }

            return newEntity;
        }

        public static Entity CloneEntitySandbox(Entity entityToClone, List<string> fieldNamesToSkip)
        {
            var e = new Entity(entityToClone.LogicalName);

            foreach (var attr in entityToClone.Attributes)
            {
                if (attr.Key != entityToClone.LogicalName + "id" && !fieldNamesToSkip.Contains(attr.Key))
                {
                    e.SetAttributeValue(attr.Key, attr.Value);
                }
            }

            return e;
        }

        public static T CloneEntitySandbox<T>(T entityToClone) where T : Entity, new()
        {
            var e = new T();
            e.LogicalName = entityToClone.LogicalName;

            foreach (var attr in entityToClone.Attributes)
            {
                if (attr.Key != entityToClone.LogicalName + "id")
                {
                    e.SetAttributeValue(attr.Key, attr.Value);
                }
            }

            foreach (var f in entityToClone.FormattedValues)
            {
                e.FormattedValues.Add(f.Key, f.Value);
            }


            return e;
        }

        public static string GetFormattedValue(this Entity entity, string attributeName)
        {
            if (entity.Attributes.Contains(attributeName))
            {
                return entity.FormattedValues[attributeName];
            }
            else
            {
                return null;
            }

        }


        public static bool IsSystemAdmin(this IOrganizationService service, Guid userId)
        {
            // All MS Dynamics CRM instances share the same System Admin role GUID.
            // Hence, we can hardode it as this will not represent a security issue
            Guid adminId = new Guid("627090FF-40A3-4053-8790-584EDC5BE201");

            // Defining our primary Entity for the query
            var q = new QueryExpression("role");

            // Adding validation to search based on our System Admin Role GUID
            q.Criteria.AddCondition("roletemplateid", ConditionOperator.Equal, adminId);

            // Adding a Link to SystemUserRoles Entity so we can check the User roles
            var link = q.AddLink("systemuserroles", "roleid", "roleid");

            // To filter only roles of the giving user
            link.LinkCriteria.AddCondition("systemuserid", ConditionOperator.Equal, userId);

            // Return result
            return service.GetMultiple(q).Count > 0;
            
        }

        public static T GetAttributeValue<T>(this Entity entity, string attributeName, Entity image)
        {
            if (entity.Attributes.Contains(attributeName))
            {
                return entity.GetAttributeValue<T>(attributeName);
            }
            else if (image != null && image.Attributes.Contains(attributeName))
            {
                return image.GetAttributeValue<T>(attributeName);
            }
            else
            {
                return default(T);
            }
        }

        public static void SetAttributeValue(this Entity entity, string attributeName, object attributeValue)
        {
            if (entity.Attributes.Contains(attributeName))
            {
                entity.Attributes[attributeName] = attributeValue;
            }
            else
            {
                entity.Attributes.Add(attributeName, attributeValue);
            }
        }

        public static string SerializeToString(this Entity entity)
        {
            string result = string.Empty;
            using (MemoryStream memStm = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(Entity));
                serializer.WriteObject(memStm, entity);

                memStm.Seek(0, SeekOrigin.Begin);
                result = new StreamReader(memStm).ReadToEnd();
            }

            return result;
        }

        /// <summary>
        /// Gets specified entity metadata (include attributes)
        /// </summary>
        /// <param name="service">CRM organization service</param>
        /// <param name="logicalName">Logical name of the entity</param>
        /// <returns>Entity metadata</returns>
        public static EntityMetadata RetrieveEntityMetadata(IOrganizationService service, string logicalName)
        {
            try
            {
                var request = new RetrieveEntityRequest
                {
                    LogicalName = logicalName,
                    EntityFilters = EntityFilters.Attributes,
                    RetrieveAsIfPublished = true

                };

                var response = (RetrieveEntityResponse)service.Execute(request);

                return response.EntityMetadata;
            }
            catch (Exception error)
            {
                throw new Exception("RetrieveAuditHistory Error while retrieving entity metadata: " + error.StackTrace);
            }
        }

        /// <summary>
        /// Get metadata of an attribute
        /// </summary>
        /// <param name="service">the instance of the organisation service</param>
        /// <param name="entityName">the entity name</param>
        /// <param name="attributeName">the attribute name</param>
        /// <returns>the attribute metadata</returns>
        public static AttributeMetadata RetrieveAttributeMetadata(IOrganizationService service, string entityName, string attributeName)
        {
            try
            {
                var attributeRequest = new RetrieveAttributeRequest
                {
                    EntityLogicalName = entityName,
                    LogicalName = attributeName,
                    RetrieveAsIfPublished = true
                };

                // Execute the request
                var attributeResponse =
                    (RetrieveAttributeResponse)service.Execute(attributeRequest);

                return attributeResponse.AttributeMetadata;
            }
            catch (Exception error)
            {
                throw new Exception("RetrieveAuditHistory Error while retrieving attribute metadata: " + error);
            }
        }

        /// <summary>
        /// Get the name of the target entity
        /// </summary>
        /// <param name="service">the instance of the organisation service</param>
        /// <param name="entityName">the schema name of the entity</param>
        /// <param name="entityId">the GUID id of the entity</param>
        /// <param name="primaryAttributeName">the primary attribute name of the entity</param>
        /// <returns>the name of the entity record</returns>
        public static string RetrieveTargetName(IOrganizationService service, string entityName, Guid entityId, string primaryAttributeName)
        {
            try
            {
                var record = service.Retrieve(entityName, entityId, new ColumnSet(primaryAttributeName));
                return record.GetAttributeValue<string>(primaryAttributeName);
            }
            catch (Exception error)
            {
                throw new Exception("RetrieveAuditHistory Error while retrieving name for one record: " + error);
            }
        }


        public static void ExecuteMultiple(IOrganizationService service, IEnumerable<OrganizationRequest> requests)
        {
            var req = new ExecuteMultipleRequest();
            req.Requests = new OrganizationRequestCollection();
            req.Settings = new ExecuteMultipleSettings();
            req.Settings.ContinueOnError = false;
            req.Settings.ReturnResponses = false;

            req.Requests.AddRange(requests);

            service.Execute(req);
        }



        public static void SetStatus(this IOrganizationService service, EntityReference entity, int state, int status)
        {
            var req = new SetStateRequest();
            req.EntityMoniker = entity;
            req.State = new OptionSetValue(state);
            req.Status = new OptionSetValue(status);
            service.Execute(req);
        }

        public static Entity GetEntityByName(this IOrganizationService service, string entityName,string nameFieldName, string nameValue)
        {
            var q = new QueryExpression(entityName);
            q.ColumnSet = new ColumnSet(true);
            q.AddCriteria(nameFieldName, nameValue);

            return service.GetMultiple(q).SingleOrDefault();
        }

        public static Entity GetEntity(this IOrganizationService service, string entityName, Guid id)
        {
            return service.Retrieve(entityName, id, new ColumnSet(true));
        }

        public static Entity GetEntity(this IOrganizationService service, string entityName, Guid id, string[] columns)
        {
            return service.Retrieve(entityName, id, new ColumnSet(columns));
        }
        public static Entity GetEntity(this IOrganizationService service, string entityName, Guid id, ColumnSet columns)
        {
            return service.Retrieve(entityName, id, columns);
        }

        public static Entity GetEntity(this IOrganizationService service, EntityReference entity)
        {
            return service.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(true));
        }

        public static Entity GetEntity(this IOrganizationService service, EntityReference entity, string[] columns)
        {
            return service.Retrieve(entity.LogicalName, entity.Id,  new ColumnSet(columns));
        }
        public static Entity GetEntity(this IOrganizationService service, EntityReference entity, ColumnSet columns)
        {
            return service.Retrieve(entity.LogicalName, entity.Id, columns);
        }

        public static bool AccessTeamExists(this IOrganizationService service, EntityReference record, Guid teamTemplateId)
        {
            var q = new QueryExpression("team");
            q.AddCriteria("teamtype", 1);
            q.AddCriteria("teamtemplateid", teamTemplateId);
            q.AddCriteria("regardingobjectid", record.Id);

            return service.GetMultiple(q).Any();

        }

        public static AddUserToRecordTeamResponse AddUserToAccessTeam(this IOrganizationService service, EntityReference record,Guid userId,Guid teamTemplateId)
        {
            AddUserToRecordTeamRequest teamAddRequest2 = new AddUserToRecordTeamRequest();
            teamAddRequest2.Record = record;
            teamAddRequest2.SystemUserId = userId;
            teamAddRequest2.TeamTemplateId = teamTemplateId;
            return (AddUserToRecordTeamResponse)service.Execute(teamAddRequest2) ;
        }

        public static RemoveUserFromRecordTeamResponse RemoveUserFromAccessTeam(this IOrganizationService service, EntityReference record, Guid userId, Guid teamTemplateId, ITracingService trace = null)
        {
            RemoveUserFromRecordTeamRequest teamAddRequest = new RemoveUserFromRecordTeamRequest();
            teamAddRequest.Record = record;
            teamAddRequest.SystemUserId = userId;
            teamAddRequest.TeamTemplateId = teamTemplateId;
            return (RemoveUserFromRecordTeamResponse)service.Execute(teamAddRequest);

        }

        public static Entity GetTeamTemplateByName(this IOrganizationService service, string attName)
        {
            var q = new QueryExpression("teamtemplate");
            q.ColumnSet = new ColumnSet(true);
            q.Criteria.AddCondition("teamtemplatename", ConditionOperator.Equal, attName);
            var tt = service.GetMultiple(q).Single();
            return tt;
        }


        /// <summary> 
        /// Find the Logical Name from the entity type code - this needs a reference to the Organization Service to look up metadata 
        /// </summary> 
        /// <param name="service"></param> 
        /// <returns></returns> 
        public static string GetEntityLogicalName(IOrganizationService service,int entityTypeCode)
        {
            var entityFilter = new MetadataFilterExpression(LogicalOperator.And);
            entityFilter.Conditions.Add(new MetadataConditionExpression("ObjectTypeCode ", MetadataConditionOperator.Equals, entityTypeCode));
            var propertyExpression = new MetadataPropertiesExpression { AllProperties = false };
            propertyExpression.PropertyNames.Add("LogicalName");
            var entityQueryExpression = new EntityQueryExpression()
            {
                Criteria = entityFilter,
                Properties = propertyExpression
            };

            var retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest()
            {
                Query = entityQueryExpression
            };

            var response = (RetrieveMetadataChangesResponse)service.Execute(retrieveMetadataChangesRequest);

            if (response.EntityMetadata.Count == 1)
            {
                return response.EntityMetadata[0].LogicalName;
            }
            return null;
        }
        public static Entity GetEntity(IOrganizationService service, string entityName, string fieldName, object fieldValue)
        {
            var query = new QueryExpression(entityName);
            query.Criteria.AddCondition(fieldName, ConditionOperator.Equal, fieldValue);
            query.ColumnSet = new ColumnSet(true);
            return GetMultiple(service, query).SingleOrDefault();

        }
        public static List<Entity> GetMultiple(this IOrganizationService service, string fetchXml)
        {
            var fetchReq = new FetchXmlToQueryExpressionRequest();
            fetchReq.FetchXml = fetchXml;

            var res = (FetchXmlToQueryExpressionResponse)service.Execute(fetchReq);

            return GetMultiple(service, res.Query);
        }

        public static void UpdateMultipleAttributes(this IOrganizationService service, string entityName, Guid entityId, Dictionary<string, object> attributes)
        {

            UpdateMultipleAttributes(service, new EntityReference(entityName, entityId), attributes);
        }
        public static void UpdateMultipleAttributes(this IOrganizationService service, Entity e, Dictionary<string, object> attributes)
        {
            UpdateMultipleAttributes(service, e.ToEntityReference(), attributes);
        }

        public static void UpdateMultipleAttributes(this IOrganizationService service, EntityReference e, Dictionary<string, object> attributes)
        {
            //using this format rather than constructor for CRM2011 compatibility
            var entity = new Entity { LogicalName = e.LogicalName, Id = e.Id };
            foreach (var kvp in attributes)
            {
                entity[kvp.Key] = kvp.Value;
            }
            service.Update(entity);

        }

        public static void UpdateSingleAttribute(this IOrganizationService service, string entityName, Guid entityId, string attributeName, object attributeValue)
        {
            UpdateSingleAttribute(service, new EntityReference(entityName, entityId), attributeName, attributeValue);
        }

        public static void UpdateSingleAttribute(this IOrganizationService service, Entity e, string attributeName, object attributeValue)
        {
            UpdateSingleAttribute(service, e.ToEntityReference(), attributeName, attributeValue);
        }

        public static void UpdateSingleAttribute(this IOrganizationService service, EntityReference e, string attributeName, object attributeValue)
        {
            //using this format rather than constructor for CRM2011 compatibility
            var entity = new Entity { LogicalName = e.LogicalName, Id = e.Id };
            entity[attributeName] = attributeValue;
            service.Update(entity);

        }

        public static List<Entity> GetMultiple(this IOrganizationService service, QueryBase query)
        {
            
            var resp = service.RetrieveMultiple(query);
            if (resp != null && resp.Entities != null)
            {
                return resp.Entities.ToList();
            }
            else
            {
                return new List<Entity>();
            }
        }

        public static void AddCriteria(this QueryExpression q, string fieldName, object value)
        {
            q.Criteria.AddCondition(fieldName, ConditionOperator.Equal, value);
        }

        public static List<T> GetMultiple<T>(this IOrganizationService service, QueryBase query)
        {
            var resp = service.RetrieveMultiple(query);
            if (resp != null && resp.Entities != null)
            {
                return resp.Entities.Cast<T>().ToList();
            }
            else
            {
                return new List<T>();
            }
        }

        public static string GetOptionsetText(IOrganizationService service, string optionSetName, int optionSetValue, string entityName = "")
        {
            try
            {
                var options = GetOptionSetMetadata(service, optionSetName, entityName);
                IList<OptionMetadata> optionsList = (from o in options.Options
                                                     where o.Value != null && o.Value.Value == optionSetValue
                                                     select o).ToList();
                var optionSetLabel = (optionsList.Count > 0) ? optionsList.First().Label.UserLocalizedLabel.Label : "(Value No Found)";
                return optionSetLabel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public static int GetOptionsetValue(IOrganizationService service, string optionSetName, string optionSetText, string entityName = "")
        {
            try
            {
                var options = GetOptionSetMetadata(service, optionSetName, entityName);
                IList<OptionMetadata> optionsList = (from o in options.Options
                                                     where o.Value != null && o.Label.UserLocalizedLabel.Label == optionSetText
                                                     select o).ToList();
                var optionSetLabel = (optionsList.Count > 0) ? optionsList.First().Value.Value : 0;
                return optionSetLabel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static Guid Create(IOrganizationService service, Entity entity)
        {
            return service.Create(entity);
        }

        public static void DeleteEntity(this IOrganizationService service, Entity entity)
        {
            service.Delete(entity.LogicalName,entity.Id );
        }

        /// <summary>
        /// Executes a query expression and return a List of Entities
        /// </summary>
        /// <param name="service">CRM service</param>
        /// <param name="query">query expression</param>
        /// <returns>list of entities</returns>
        
        public static List<Entity> GetAll(this IOrganizationService service, QueryExpression query)
        {
            var entities = new List<Entity>();

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = 5000;
            query.PageInfo.PagingCookie = null;
            query.PageInfo.PageNumber = 1;
            var res = service.RetrieveMultiple(query);
            entities.AddRange(res.Entities);
            while (res.MoreRecords == true)
            {
                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = res.PagingCookie;
                res = service.RetrieveMultiple(query);
                entities.AddRange(res.Entities);
            }

            return entities;
        }
        public static List<Entity> GetAll(this IOrganizationService service, string entityName)
        {
            var entities = new List<Entity>();

            var query = new QueryExpression(entityName);
            query.ColumnSet = new ColumnSet(true);

            query.PageInfo = new PagingInfo();
            query.PageInfo.Count = 5000;
            query.PageInfo.PagingCookie = null;
            query.PageInfo.PageNumber = 1;
            var res = service.RetrieveMultiple(query);
            entities.AddRange(res.Entities);
            while (res.MoreRecords == true)
            {
                query.PageInfo.PageNumber++;
                query.PageInfo.PagingCookie = res.PagingCookie;
                res = service.RetrieveMultiple(query);
                entities.AddRange(res.Entities);
            }

            return entities;
        }

        /// <summary>
        /// Get the text of the OptionSet
        /// </summary>
        /// <param name="service">the instance of the CRM Organization service</param>
        /// <param name="optionSetName">the name of the OptionSet attribute</param>
        /// <param name="optionSetValue">the value of the OptionSet attribute</param>
        /// <param name="entityName">the name of the entity (optional parameter)</param>
        /// <returns>The label of the OptionSet field</returns>


        /// <summary>
        /// Get the value of the OptionSet field
        /// </summary>
        /// <param name="service">the instance of the CRM Organization service</param>
        /// <param name="optionSetName">the name of the OptionSet attribute</param>
        /// <param name="optionSetText">the text of the OptionSet attribute</param>
        /// <param name="entityName">the name of the entity (optional parameter)</param>
        /// <returns>The value of the OptionSet field</returns>


        private static string GetRoleName(XmlDocument doc)
        {

            string result = string.Empty;
            XmlNode node = doc.SelectSingleNode(ROLENAME);

            if (node != null)
            {
                result = node.InnerText;
            }

            return result;

        }


        
        /// <summary>
        /// Get the Metadata of the OptionSet
        /// </summary>
        /// <param name="service">the instance of the service</param>
        /// <param name="optionsetName">the name of attribute</param>
        /// <param name="entityName">the name of the entity (optional parameter, default as empty if not passed)</param>
        /// <returns>the MetaData of the OptionSet</returns>
        public static OptionSetMetadata GetOptionSetMetadata(IOrganizationService service, string optionsetName, string entityName = "")
        {

            try
            {
                OptionSetMetadata optionSetMetadata = null;

                if (string.IsNullOrEmpty(entityName))
                {
                    var retrieveOptionSetRequest = new RetrieveOptionSetRequest
                    {
                        Name = optionsetName,
                        RetrieveAsIfPublished = true
                    };

                    // Execute the request.
                    var retrieveOptionSetResponse = (RetrieveOptionSetResponse)service.Execute(retrieveOptionSetRequest);

                    // Access the retrieved OptionSetMetadata.
                    optionSetMetadata = (OptionSetMetadata)retrieveOptionSetResponse.OptionSetMetadata;
                }
                else
                {
                    var request = new RetrieveAttributeRequest
                    {
                        EntityLogicalName = entityName,
                        LogicalName = optionsetName,
                        RetrieveAsIfPublished = true
                    };

                    var resp = (RetrieveAttributeResponse)service.Execute(request);

                    if (optionsetName.Contains("statecode"))
                    {
                        var retrievedPicklistAttributeMetadata = (StateAttributeMetadata)resp.AttributeMetadata;
                        optionSetMetadata = retrievedPicklistAttributeMetadata.OptionSet;
                    }
                    else if (optionsetName.Contains("statuscode"))
                    {
                        var retrievedPicklistAttributeMetadata = (StatusAttributeMetadata)resp.AttributeMetadata;
                        optionSetMetadata = retrievedPicklistAttributeMetadata.OptionSet;
                    }
                    else
                    {
                        try
                        {
                            var retrievedPicklistAttributeMetadata = (PicklistAttributeMetadata)resp.AttributeMetadata;
                            optionSetMetadata = retrievedPicklistAttributeMetadata.OptionSet;
                        }
                        catch (Exception)
                        {

                            //return nothing
                        }
                        
                    }
                }

                return optionSetMetadata;
            }
            catch (Exception)
            {
                throw;
            }
        }

        
    }
}