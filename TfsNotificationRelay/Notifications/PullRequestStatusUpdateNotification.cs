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
using Microsoft.TeamFoundation.Git.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public class PullRequestStatusUpdateNotification : PullRequestNotification
    {
        public PullRequestStatus Status { get; set; } 
        private string FormatAction(BotElement bot)
        {
            switch (Status)
                {
                    case PullRequestStatus.Abandoned: return bot.Text.Abandoned;
                    case PullRequestStatus.Active: return bot.Text.Reactivated;
                    case PullRequestStatus.Completed: return bot.Text.Completed;
                    default:
                        return $"updated status to {Status} for";
                }
        }

        public override IList<string> ToMessage(BotElement bot, Func<string, string> transform)
        {
            var formatter = new
            {
                TeamProjectCollection = transform(TeamProjectCollection),
                Status = transform(Status.ToString()),
                DisplayName = transform(DisplayName),
                ProjectName = transform(ProjectName),
                RepoUri,
                RepoName = transform(RepoName),
                PrId,
                PrUrl,
                PrTitle = transform(PrTitle),
                UserName = transform(UserName),
                Action = FormatAction(bot),
                SourceBranchName = transform(SourceBranch.Name),
                TargetBranchName = transform(TargetBranch.Name),
                CreatorUserName = transform(CreatorUserName),
                MappedCreatorUser = bot.GetMappedUser(CreatorUniqueName),
                MappedUser = bot.GetMappedUser(UniqueName)
            };
            return new[] { bot.Text.PullRequestStatusUpdateFormat.FormatWith(formatter) };
        }

        public override EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules)
        {
            var rule = GetRulesMatch(collection, eventRules).FirstOrDefault(r => r.Events.HasFlag(TfsEvents.PullRequestStatusUpdate));

            return rule;
        }
    }
}
