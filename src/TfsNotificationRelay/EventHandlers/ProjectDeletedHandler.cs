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
using System.Collections.Generic;

namespace DevCore.TfsNotificationRelay.EventHandlers
{
    class ProjectDeletedHandler : BaseHandler<ProjectDeletedEvent>
    {
        protected override IEnumerable<INotification> CreateNotifications(IVssRequestContext requestContext, ProjectDeletedEvent ev, int maxLines)
        {
            string projectName;
            if (ProjectsNames.TryGetValue(ev.Uri, out projectName))
            {
                ProjectsNames.Remove(ev.Uri);
            }

            yield return new ProjectDeletedNotification() { TeamProjectCollection = requestContext.ServiceHost.Name, ProjectUri = ev.Uri, ProjectName = projectName };
        }
    }
}
