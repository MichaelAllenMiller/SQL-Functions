//------------------------------------------------------------------------------
// <copyright file="CSSqlFunction.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace ElevenSquared {
    public partial class SqlFunctions {
        [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = true)]
        public static SqlString GetFullAddress(String address, String address2, String city, String state, String zipcode) {
            if (String.IsNullOrEmpty(address)) { return new SqlString(""); }
            if (String.IsNullOrEmpty(city)) { return new SqlString(""); }
            if (String.IsNullOrEmpty(state)) { return new SqlString(""); }
            address2 = address2 ?? "";
            zipcode = zipcode ?? "";
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            result.Append((address.Trim() + " " + address2.Trim()).Trim());
            result.Append(", ");
            result.Append(city.Trim());
            result.Append(", ");
            result.Append(state.Trim());
            result.Append(" ");
            result.Append(zipcode.Trim());
            return new SqlString(System.Text.RegularExpressions.Regex.Replace(System.Text.RegularExpressions.Regex.Replace(result.ToString(), "((APT|TRLR|LOT|STE|UNIT|#) [ a-z0-9])", " $1", System.Text.RegularExpressions.RegexOptions.IgnoreCase), "  *", " "));
        }

        [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = true)]
        public static SqlString GetFullAddressWithoutUnit(String address, String city, String state, String zipcode) {
            if (String.IsNullOrEmpty(address)) { return new SqlString(""); }
            if (String.IsNullOrEmpty(city)) { return new SqlString(""); }
            if (String.IsNullOrEmpty(state)) { return new SqlString(""); }
            address = System.Text.RegularExpressions.Regex.Replace(address, @"(DR|DRIVE|LN|LANE|RDG|RIDGE|RD|ROAD|AVE|AVENUE)[\. ]*(#|APT|BLDG|LOT|ROOM|RM|STE|TRLR|UNIT|CLUB|PUBLIC).*", "$1", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            zipcode = zipcode ?? "";
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            result.Append(address.Trim());
            result.Append(", ");
            result.Append(city.Trim());
            result.Append(", ");
            result.Append(state.Trim());
            result.Append(" ");
            result.Append(zipcode.Trim());
            return new SqlString(System.Text.RegularExpressions.Regex.Replace(result.ToString(), "&amp;", "&"));
        }

        [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = true)]
        public static SqlString AddFlag(String flags, String flag) {
            if (String.IsNullOrEmpty(flags)) { flags = ""; }
            System.Collections.Generic.List<String> temp = new System.Collections.Generic.List<String>();

            foreach (String a in flags.Split(new Char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries)) {
                if (!temp.Contains(a.Trim())) { temp.Add(a.Trim()); }
            }

            foreach (String a in flag.Split(new Char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries)) {
                if (!temp.Contains(a.Trim())) { temp.Add(a.Trim()); }
            }
            
            temp.Sort();

            return new SqlString(String.Join(";", temp.ToArray()));
        }
        
        [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = true)]
        public static SqlBoolean HasFlag(String flags, String flag) {
            if (String.IsNullOrEmpty(flags)) { flags = ""; }
            return new SqlBoolean(System.Text.RegularExpressions.Regex.IsMatch(flags, "(;|^)" + flag + "(;|$)", System.Text.RegularExpressions.RegexOptions.IgnoreCase));
        }
        
        [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = true)]
        public static SqlString RemoveFlag(String flags, String flag) {
            if (String.IsNullOrEmpty(flags)) { flags = ""; }
            System.Collections.Generic.List<String> temp = new System.Collections.Generic.List<String>();
            foreach (String a in flags.Split(new Char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries)) {
                if (a != flag) { temp.Add(a); }
            }
            temp.Sort();

            return new SqlString(String.Join(";", temp.ToArray()));
        }

        [Microsoft.SqlServer.Server.SqlFunction(IsDeterministic = true)]
        public static SqlString ToggleFlag(String flags, String flag) {
            if (String.IsNullOrEmpty(flags)) { flags = ""; }
            if (HasFlag(flags, flag).Value) {
                return RemoveFlag(flags, flag);
            } else {
                return AddFlag(flags, flag);
            }
            
        }
    }
}