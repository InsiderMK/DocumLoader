using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Globalization;

namespace DocumLoader
{
    /// <summary>
    /// Parser of exported documents
    /// </summary>
    internal class DocumParser
    {
        public CaseInfo ParseFile(Stream dataStream)
        {
            CaseInfo caseInfo = new CaseInfo();

            using (XmlReader reader = XmlReader.Create(dataStream))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Case")
                        {
                            CheckRootElement(reader);
                            ParseCase(reader, caseInfo);
                            break;
                        }
                        else if (reader.NodeType == XmlNodeType.Element && reader.Name == "DeletedCases")
                        {
                            break;
                        }
                        else
                        {
                            throw new ApplicationException("Parse error. Wrong root tag.");
                        }
                    }
                }
            }

            return caseInfo;
        }

        private void CheckRootElement(XmlReader reader)
        {
            string systemName = reader.GetAttribute("sad");
            string courtName = reader.GetAttribute("section");
            string version = reader.GetAttribute("version");

            if (systemName != "kodeks")
            {
                throw new ApplicationException("Parse error. Unsupported sad system.");
            }

            if (courtName != "Арбитражный суд Курганской области")
            {
                throw new ApplicationException("Parse error. Unsupported court name.");
            }

            if (version != "4")
            {
                throw new ApplicationException("Parse error. Unsupported sad version.");
            }
        }

        private void ParseCase(XmlReader reader, CaseInfo caseInfo)
        {
            string elementName = reader.Name;
            bool isInsideCase = true;

            while (isInsideCase && reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "Caseid":
                            ParseCaseId(reader, caseInfo);
                            break;
                        case "TS":
                            ParseCaseUpdateDate(reader, caseInfo);
                            break;
                        case "Instance1":
                            ParseCaseName(reader, caseInfo);
                            break;
                        case "RegDate":
                            ParseRegDate(reader, caseInfo);
                            break;
                        case "ArgumentCategory":
                            ParseArgumentCategory(reader, caseInfo);
                            break;
                        case "ArgumentSubCategory":
                            ParseArgumentSubCategory(reader, caseInfo);
                            break;
                        case "ArgumentSort":
                            ParseArgumentSort(reader, caseInfo);
                            break;
                        case "CaseChars":
                            ParseCaseChars(reader, caseInfo);
                            break;
                        case "IsArchived":
                            ParseIsArchived(reader, caseInfo);
                            break;
                        case "IsDestroyed":
                            ParseIsDestroyed(reader, caseInfo);
                            break;
                        case "ClaimSum":
                            ParseClaimSum(reader, caseInfo);
                            break;
                        case "Considerations":
                            ParseConsiderations(reader, caseInfo);
                            break;
                        default:
                            SkipElement(reader);
                            break;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == elementName)
                {
                    isInsideCase = false;
                }
            }
        }

        private void ParseCaseId(XmlReader reader, CaseInfo caseInfo)
        {
            int oid;
            bool result = int.TryParse(GetElementValue(reader), out oid);

            if (!result)
            {
                throw new ApplicationException("Syntax error. Case OID is not numeric.");
            }

            caseInfo.Oid = oid;
        }

        private void ParseCaseUpdateDate(XmlReader reader, CaseInfo caseInfo)
        {
            DateTime updateDate;
            bool result = DateTime.TryParse(GetElementValue(reader), out updateDate);

            if (!result)
            {
                throw new ApplicationException("Syntax error. Incorrect timestamp format.");
            }

            caseInfo.LastUpdateDate = updateDate;
        }

        private void ParseRegDate(XmlReader reader, CaseInfo caseInfo)
        {
            DateTime createDate;
            bool result = DateTime.TryParse(GetElementValue(reader), out createDate);

            if (!result)
            {
                throw new ApplicationException("Syntax error. Incorrect registration date format.");
            }

            caseInfo.CreateDate = createDate;
        }

        private void ParseCaseName(XmlReader reader, CaseInfo caseInfo)
        {
            string elementName = reader.Name;
            bool foundCaseName = SkipToChildElement(reader, "CaseNum");

            if (!foundCaseName)
            {
                throw new ApplicationException("Syntax error. No <CaseNum> inside <Instance1>.");
            }

            caseInfo.Name = GetElementValue(reader);
            SkipElement(reader, elementName);
        }

        private void ParseArgumentCategory(XmlReader reader, CaseInfo caseInfo)
        {
            string elementName = reader.Name;
            bool foundId = SkipToChildElement(reader, "Id");

            if (!foundId)
            {
                throw new ApplicationException("Syntax error. No <Id> inside <ArgumentCategory>.");
            }

            int oid;
            bool result = int.TryParse(GetElementValue(reader), out oid);

            if (!result)
            {
                throw new ApplicationException("Syntax error. ArgumentCategory OID is not numeric.");
            }

            caseInfo.CategoryOid = oid;
            SkipElement(reader, elementName);
        }

        private void ParseArgumentSubCategory(XmlReader reader, CaseInfo caseInfo)
        {
            string elementName = reader.Name;
            bool foundId = SkipToChildElement(reader, "Id");

            if (!foundId)
            {
                throw new ApplicationException("Syntax error. No <Id> inside <ArgumentSubCategory>.");
            }

            int oid;
            bool result = int.TryParse(GetElementValue(reader), out oid);

            if (!result)
            {
                throw new ApplicationException("Syntax error. ArgumentSubCategory OID is not numeric.");
            }

            caseInfo.SubcategoryOid = oid;
            SkipElement(reader, elementName);
        }

        private void ParseArgumentSort(XmlReader reader, CaseInfo caseInfo)
        {
            string elementName = reader.Name;
            bool foundId = SkipToChildElement(reader, "Id");

            if (!foundId)
            {
                throw new ApplicationException("Syntax error. No <Id> inside <ArgumentSort>.");
            }

            int oid;
            bool result = int.TryParse(GetElementValue(reader), out oid);

            if (!result)
            {
                throw new ApplicationException("Syntax error. ArgumentSort OID is not numeric.");
            }

            caseInfo.TypeOid = oid;
            SkipElement(reader, elementName);
        }

        private void ParseCaseChars(XmlReader reader, CaseInfo caseInfo)
        {
            string elementName = reader.Name;
            bool foundId = SkipToChildElement(reader, "Id");

            if (!foundId)
            {
                throw new ApplicationException("Syntax error. No <Id> inside <CaseChars>.");
            }

            int oid;
            bool result = int.TryParse(GetElementValue(reader), out oid);

            if (!result)
            {
                throw new ApplicationException("Syntax error. CaseChars OID is not numeric.");
            }

            caseInfo.NatureOid = oid;
            SkipElement(reader, elementName);
        }

        private void ParseIsArchived(XmlReader reader, CaseInfo caseInfo)
        {
            bool isArchived;
            bool result = bool.TryParse(GetElementValue(reader), out isArchived);

            if (!result)
            {
                throw new ApplicationException("Syntax error. Incorrect archived status format.");
            }

            caseInfo.IsArchived = isArchived;
        }

        private void ParseIsDestroyed(XmlReader reader, CaseInfo caseInfo)
        {
            bool isDestroyed;
            bool result = bool.TryParse(GetElementValue(reader), out isDestroyed);

            if (!result)
            {
                throw new ApplicationException("Syntax error. Incorrect destroyed status format.");
            }

            caseInfo.IsDestroyed = isDestroyed;
        }

        private void ParseClaimSum(XmlReader reader, CaseInfo caseInfo)
        {
            decimal claimSum;
            bool result = decimal.TryParse(GetElementValue(reader), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out claimSum);

            if (!result)
            {
                throw new ApplicationException("Syntax error. Incorrect claim sum format.");
            }

            caseInfo.ClaimSum = claimSum;
        }

        private void ParseConsiderations(XmlReader reader, CaseInfo caseInfo)
        {
            string elementName = reader.Name;
            int countConsiderations = 0;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Consideration")
                {
                    countConsiderations++;
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == elementName)
                {
                    break;
                }
            }

            if (countConsiderations > 2)
                Console.WriteLine(caseInfo.Oid.ToString());
        }

        private bool SkipToChildElement(XmlReader reader, string childName)
        {
            string elementName = reader.Name;
            bool found = false;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == childName)
                {
                    found = true;
                    break;
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == elementName)
                {
                    break;
                }
            }

            return found;
        }

        private string GetElementValue(XmlReader reader)
        {
            string elementName = reader.Name;
            string elementValue = null;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Text)
                {
                    elementValue = reader.Value;
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == elementName)
                {
                    break;
                }
                else
                {
                    throw new ApplicationException("Parse error. Unexpected content inside text element.");
                }
            }

            return elementValue;
        }

        private void SkipElement(XmlReader reader)
        {
            SkipElement(reader, reader.Name);
        }

        private void SkipElement(XmlReader reader, string elementName)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == elementName)
                {
                    break;
                }
            }
        }
    }
}
