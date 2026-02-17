private string buildSDESOAP(int masID, bool doNow, string scheduleDate, List<int> siteIDs, HouseHoldRelationShip data)
		{
			StringBuilder soapRequest = new StringBuilder();
			//string soapHead = @"<Save xmlns=""http://realpage.com/webservices""><InputXML>"
			soapRequest.Append(@"<root><SaveType rptType="""" bdID=""0"" />");
			soapRequest.Append(@"<Detail><Row grpID="""" scheduleID="""" fmtID=""-1"" email="""" postto="""" doNow=""" + (doNow ? "0" : "1") + "");
			soapRequest.Append(@"""");
			soapRequest.Append(@" schedDate=""" + (doNow ? DateTime.Today.AddMinutes(5).ToShortDateString() : scheduleDate + ""));
			soapRequest.Append(@"""");
			soapRequest.Append(@" schedTime=""" + (doNow ? DateTime.Today.AddMinutes(5).ToShortTimeString() : scheduleDate + ""));
			soapRequest.Append(@"""");
			soapRequest.Append(@"/> </Detail>");


			soapRequest.Append(@"<Params><row statusID=""ALL"" subID=""ALL"" methodID="""" days="""" masID=""" + masID.ToString() + "");
			soapRequest.Append(@""" fmtID=""-1"" /></Params>");

			soapRequest.Append(@"<Custom>");
			soapRequest.Append(@"<Row name=""masID"" value=""" + masID.ToString() + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_SID"" value=""" + data.SID.ToString() + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_DISABLEDBIT"" value=""" + data.DisabledBit + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_CODENAME"" value=""" + data.CodeName + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_CODEDISPLAYNAME"" value=""" + data.CodeDisplayname + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_HUDSTATUS"" value=""" + data.HudStatus + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_HUD202DSTATUS"" value=""" + data.Hud202dStatus + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_PHSTATUS"" value=""" + data.PHstatus + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_RDSTATUS"" value=""" + data.RDstatus + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_TCSTATUS"" value=""" + data.TCstatus + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_ATCID"" value=""" + data.ATCID + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_IsLeaseSignerForOnline"" value=""" + data.IsLeaseSignerForOnline + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_IsMinor"" value=""" + data.IsMinor + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_DisplayOnline"" value=""" + data.DisplayOnline + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_IsOccupant"" value=""" + data.IsOccupant + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_IsCoSigner"" value=""" + data.IsCoSigner + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_IsGuarantor"" value=""" + data.IsGuarantor + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_IsEmployee"" value=""" + data.IsEmployee + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"<Row name=""IPVC_IsBusinessCorporation"" value=""" + data.IsBusinessCorporation + "");
			soapRequest.Append(@""" />");
			soapRequest.Append(@"</Custom>");

			soapRequest.Append(@"<Sites>");
			foreach (int site in siteIDs)
			{
				soapRequest.Append(@"<Row siteID=""" + site.ToString() + "");
				soapRequest.Append(@""" Selected = ""1"" />");
			}

			soapRequest.Append(@"</Sites></root>");//</InputXML></Save> ";
			return soapRequest.ToString();
		}

		private string ConvertToCamelCase(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;

			string[] words = System.Text.RegularExpressions.Regex.Split(input, @"(?<!^)(?=[A-Z])|_|-|\s+");
			StringBuilder result = new StringBuilder();

			for (int i = 0; i < words.Length; i++)
			{
				if (string.IsNullOrEmpty(words[i]))
					continue;

				if (i == 0)
				{
					result.Append(char.ToLower(words[i][0]));
					if (words[i].Length > 1)
						result.Append(words[i].Substring(1).ToLower());
				}
				else
				{
					result.Append(char.ToUpper(words[i][0]));
					if (words[i].Length > 1)
						result.Append(words[i].Substring(1).ToLower());
				}
			}

			return result.ToString();
		}

		private string BuildGUTransactionCategoryXml(string guid, string uniqueid, List<int> transactionCategoryIDs)
		{
			StringBuilder xmlBuilder = new StringBuilder();
			xmlBuilder.Append(@"<root>");
			xmlBuilder.Append(@"<GU guid=""" + guid + @""" uniqueid=""" + uniqueid + @""">");
			xmlBuilder.Append(@"<IncludedList></IncludedList>");
			xmlBuilder.Append(@"<UI>");
			xmlBuilder.Append(@"<TransactionCategory action=""UPDATE"">");
			
			foreach (int id in transactionCategoryIDs)
			{
				xmlBuilder.Append(@"<Row ID=""" + id + @"""/>");
			}
			
			xmlBuilder.Append(@"</TransactionCategory>");
			xmlBuilder.Append(@"</UI>");
			xmlBuilder.Append(@"</GU>");
			xmlBuilder.Append(@"</root>");
			
			return xmlBuilder.ToString();
		}

		private string BuildXmlFromTemplate(string xmlTemplate, string guid, string uniqueid, List<int> transactionCategoryIDs)
		{
			string result = xmlTemplate;
			result = result.Replace("$guid", guid);
			result = result.Replace("$uniqueid", uniqueid);
			
			if (transactionCategoryIDs != null && transactionCategoryIDs.Count > 0)
			{
				StringBuilder rowsBuilder = new StringBuilder();
				foreach (int id in transactionCategoryIDs)
				{
					rowsBuilder.AppendLine(@"				<Row ID=""" + id + @"""/>");
				}
				string rowsXml = rowsBuilder.ToString().TrimEnd('\r', '\n');
				result = result.Replace(@"<Row ID=""$ID""/>", rowsXml);
			}
			else
			{
				result = result.Replace(@"<Row ID=""$ID""/>", string.Empty);
			}
			
			return result;
		}

		private string GenerateInsertStatementForXmlTemplate(string tableName, string xmlColumnName)
		{
			string xmlTemplate = @"<root>
    <GU guid=""$guid"" uniqueid=""$uniqueid"">
        <IncludedList></IncludedList>
        <UI>
            <TransactionCategory action=""UPDATE"">
                <Row ID=""$ID""/>
            </TransactionCategory>
        </UI>
    </GU>
</root>";

			string escapedXml = xmlTemplate.Replace("'", "''");
			
			string insertStatement = string.Format(
				"INSERT INTO {0} ({1}) VALUES ('{2}');",
				tableName,
				xmlColumnName,
				escapedXml
			);
			
			return insertStatement;
		}

		private string buildGUTransactionSOAP(string guid, int uniqueid, List<SiteInfo> sites, List<int> transactionCategoryIDs)
		{
			StringBuilder soapRequest = new StringBuilder();
			soapRequest.Append(@"<root>");
			soapRequest.Append(@"<GU guid=""" + guid + @""" uniqueid=""" + uniqueid + @""">");
			
			soapRequest.Append(@"<IncludedList>");
			foreach (var site in sites)
			{
				soapRequest.Append(@"<Row siteid=""" + site.SiteID + @"""");
				soapRequest.Append(@" sitename=""" + site.SiteName + @"""");
				soapRequest.Append(@" siteVersion=""" + site.SiteVersion + @"""");
				soapRequest.Append(@" selected=""" + (site.Selected ? "1" : "0") + @"""");
				soapRequest.Append(@" sourceBMName=""" + site.SourceBMName + @"""");
				soapRequest.Append(@" excludedBit=""" + site.ExcludedBit + @"""/>");
			}
			soapRequest.Append(@"</IncludedList>");
			
			soapRequest.Append(@"<UI>");
			soapRequest.Append(@"<TransactionCategory action=""UPDATE"">");
			foreach (int categoryID in transactionCategoryIDs)
			{
				soapRequest.Append(@"<Row ID=""" + categoryID + @"""/>");
			}
			soapRequest.Append(@"</TransactionCategory>");
			soapRequest.Append(@"</UI>");
			
			soapRequest.Append(@"</GU>");
			soapRequest.Append(@"</root>");
			return soapRequest.ToString();
		}