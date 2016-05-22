﻿/*
 * TfsNotificationRelay - http://github.com/kria/TfsNotificationRelay
 * 
 * Copyright (C) 2014 Kristian Adrup
 * 
 * This file is part of TfsNotificationRelay.
 * 
 * TfsNotificationRelay is free software: you can redistribute it and/or 
 * modify it under the terms of the GNU General Public License as published 
 * by the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. See included file COPYING for details.
 */

using DevCore.TfsNotificationRelay.Configuration;
using DevCore.TfsNotificationRelay.EventHandlers;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class WorkItemChangedNotification : WorkItemNotification
    {
        public bool IsNew { get; set; }
        public bool IsStateChanged { get; set; }
        public bool IsAssignmentChanged { get; set; }
        public string State { get; set; }
        public CoreFieldsType CoreFields { get; set; }
        public ChangedFieldsType ChangedFields { get; set; }

        private string FormatAction(BotElement bot)
        {
            return IsNew ? bot.Text.Created : bot.Text.Updated;
        }

        public override IList<string> ToMessage(BotElement bot, Func<string, string> transform)
        {
            var lines = new List<string>();
            var formatter = new
            {
                TeamProjectCollection = transform(TeamProjectCollection),
                DisplayName = transform(DisplayName),
                ProjectName = transform(ProjectName),
                WiUrl,
                WiType = transform(WiType),
                WiId,
                WiTitle = transform(WiTitle),
                IsStateChanged,
                IsAssignmentChanged,
                AssignedTo = transform(AssignedTo),
                State = transform(State),
                UserName = transform(UserName),
                Action = FormatAction(bot),
                AssignedToUserName = transform(AssignedToUserName),
                MappedAssignedToUser = bot.GetMappedUser(AssignedToUniqueName),
                MappedUser = bot.GetMappedUser(UniqueName)
            };
            lines.Add(bot.Text.WorkItemchangedFormat.FormatWith(formatter));

            var searchType = IsNew ? SearchFieldsType.Core : SearchFieldsType.Changed;
            var displayFieldsKey = IsNew ? "wiCreatedDisplayFields" : "wiChangedDisplayFields";
            var pattern = IsNew ? "{name}: {newValue}" : "{name}: " + bot.Text.WorkItemFieldTransitionFormat;

            foreach (var fieldId in bot.GetCsvSetting(displayFieldsKey, Defaults.WorkItemFields))
            {
                var field = GetUnifiedField(fieldId, searchType);
                if (field != null)
                    lines.Add(pattern.FormatWith(field));
            }

            return lines;
        }

        public UnifiedField GetUnifiedField(string fieldId, SearchFieldsType searchType)
        {
            bool core = searchType == SearchFieldsType.Core;

            var stringFields = core ? CoreFields.StringFields : ChangedFields.StringFields;
            var integerFields = core ? CoreFields.IntegerFields : ChangedFields.IntegerFields;

            var sfield = stringFields.FirstOrDefault(f => f.ReferenceName == fieldId);
            if (!string.IsNullOrEmpty(sfield?.NewValue) && (core || sfield.OldValue != sfield.NewValue))
                return new UnifiedField(sfield);

            var ifield = integerFields.FirstOrDefault(f => f.ReferenceName == fieldId);
            if (ifield != null && (core || ifield.OldValue != ifield.NewValue))
                return new UnifiedField(ifield);

            return null;
        }

        public override EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rule = eventRules.FirstOrDefault(r =>
                (r.Events.HasFlag(TfsEvents.WorkItemCreated) && IsNew
                || r.Events.HasFlag(TfsEvents.WorkItemChanged) && IsChangedFieldMatchOrNotSet(ChangedFields, r.WorkItemFieldItems))
                && collection.IsMatchOrNoPattern(r.TeamProjectCollection)
                && ProjectName.IsMatchOrNoPattern(r.TeamProject)
                && TeamNames.IsMatchOrNoPattern(r.TeamName)
                && WiType.IsMatchOrNoPattern(r.WorkItemType)
                && AreaPath.IsMatchOrNoPattern(r.AreaPath));

            return rule;
        }

        private static bool IsChangedFieldMatchOrNotSet(ChangedFieldsType changedFields, IEnumerable<string> ruleFieldNames)
        {
            if (changedFields == null) return false; // no fields have changed on the work item
            if (!ruleFieldNames.Any()) return true; // no explicit fields set in rule means we allow any field

            if (changedFields.StringFields.Any(f => ruleFieldNames.Contains(f.ReferenceName))) return true;
            if (changedFields.IntegerFields.Any(f => ruleFieldNames.Contains(f.ReferenceName))) return true;

            return false;
        }
    }

    public enum SearchFieldsType
    {
        Core,
        Changed
    }

    public class UnifiedField
    {
        public UnifiedField() { }

        public UnifiedField(StringField field)
        {
            Name = field.Name;
            ReferenceName = field.ReferenceName;

            switch (field.ReferenceName)
            {
                case "System.AssignedTo":
                case "System.ChangedBy":
                case "System.CreatedBy":
                case "System.AuthorizedAs":
                    NewValue = GetDisplayName(field.NewValue);
                    OldValue = GetDisplayName(field.OldValue);
                    break;
                default:
                    NewValue = field.NewValue;
                    OldValue = field.OldValue;
                    break;
            }
        }

        public UnifiedField(IntegerField field)
        {
            Name = field.Name;
            NewValue = field.NewValue.ToString();
            OldValue = field.OldValue.ToString();
            ReferenceName = field.ReferenceName;
        }

        public string Name { get; set; }
        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string ReferenceName { get; set; }

        private string GetDisplayName(string value)
        {
            UserField field = null;
            if (UserField.TryParse(value, out field)) return field.DisplayName;

            return value;
        }
    }
}
