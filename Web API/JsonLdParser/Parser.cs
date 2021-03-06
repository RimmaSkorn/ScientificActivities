﻿using System;
using System.Collections.Generic;
using System.Linq;

using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace JsonLdParser
{
    public class Parser
    {
        private const string xpathString = "//script[@type=\"application/ld+json\"]";
        private const string bracketString = "[";
        private const string contextString = "@context";
        private const string typeString = "@type";

        public static IList<JObject> Parse(string html, string schemaType)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            IList<JObject> objects = new List<JObject>();

            IList<HtmlNode> scriptNodes = doc.DocumentNode.SelectNodes(xpathString);
            if (scriptNodes != null)
            {
                foreach (HtmlNode snode in scriptNodes)
                {
                    string script = snode.InnerText.Trim();
                    if (script.StartsWith(bracketString))
                    {
                        JArray arr = JArray.Parse(script);
                        foreach (JObject obj in arr)
                        {
                            if (ObjIsOfType(obj, schemaType))
                                objects.Add(obj);
                        };
                    }
                    else
                    {
                        JObject obj = JObject.Parse(script);
                        if (ObjIsOfType(obj, schemaType))
                            objects.Add(obj);
                    }
                }

            }

            return objects;
        }

        private static bool ObjIsOfType(JObject obj, string schemaType)
        {
            if (String.IsNullOrEmpty(schemaType))
                return true;

            if (obj == null)
                return false;

            string[] types = schemaType.Split(',').Select(s => s.Trim()).ToArray();
            return (obj["@context"] != null) && (obj["@type"] != null) && (types.Contains((string)(obj["@type"])));
        }

    }
}
