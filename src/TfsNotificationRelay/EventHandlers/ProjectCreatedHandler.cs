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
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class ProjectCreatedHandler : BaseHandler<ProjectCreatedEvent>
    {
        protected override IEnumerable<INotification> CreateNotifications(IVssRequestContext requestContext, ProjectCreatedEvent ev, int maxLines)
        {
            var locationService = requestContext.GetService<ILocationService>();

            string projectUrl = String.Format("{0}/{1}/{2}",
                locationService.GetAccessMapping(requestContext, "PublicAccessMapping").AccessPoint,
                requestContext.ServiceHost.Name,
                ev.Name);

            if (!ProjectsNames.ContainsKey(ev.Uri))
                ProjectsNames.Add(ev.Uri, ev.Name);

            yield return new ProjectCreatedNotification() { TeamProjectCollection = requestContext.ServiceHost.Name, ProjectUrl = projectUrl, ProjectName = ev.Name };
        }
    }
}
