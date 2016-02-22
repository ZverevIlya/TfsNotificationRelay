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

using DevCore.TfsNotificationRelay.Notifications;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class BuildQualityChangedHandler : BaseHandler<BuildQualityChangedNotificationEvent>
    {
        protected override IEnumerable<INotification> CreateNotifications(TeamFoundationRequestContext requestContext, BuildQualityChangedNotificationEvent buildNotification, int maxLines)
        {
            BuildDetail build = buildNotification.Build;
            var locationService = requestContext.GetService<TeamFoundationLocationService>();
            var buildService = requestContext.GetService<TeamFoundationBuildService>();

            using (var buildReader = buildService.QueryQueuedBuildsById(requestContext, build.QueueIds, new[] { "*" }, QueryOptions.None))
            {
                var result = buildReader.Current<BuildQueueQueryResult>();
                QueuedBuild qb = result.QueuedBuilds.FirstOrDefault();
                string buildUrl = String.Format("{0}/{1}/{2}/_build#buildUri={3}&_a=summary", 
                    locationService.GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint,
                    requestContext.ServiceHost.Name,
                    build.TeamProject,
                    build.Uri);

                var notification = new BuildQualityChangedNotification()
                {
                    TeamProjectCollection = requestContext.ServiceHost.Name,
                    BuildUrl = buildUrl,
                    ProjectName = build.TeamProject,
                    BuildNumber = build.BuildNumber,
                    BuildStatus = build.Status,
                    BuildReason = build.Reason,
                    StartTime = build.StartTime,
                    FinishTime = build.FinishTime,
                    RequestedForUniqueName = qb.RequestedFor,
                    RequestedForDisplayName = qb.RequestedForDisplayName,
                    BuildDefinition = build.Definition.Name,
                    DropLocation = build.DropLocation,
                    OldValue = buildNotification.OldValue,
                    NewValue = buildNotification.NewValue,
                    TeamNames = GetUserTeamsByProjectName(requestContext, build.TeamProject, qb.RequestedFor)
                };

                yield return notification;
            }
        }
    }
}
