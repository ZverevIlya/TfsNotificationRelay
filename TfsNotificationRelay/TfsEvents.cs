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

using System;

namespace DevCore.TfsNotificationRelay
{
    [Flags]
    public enum TfsEvents : uint
    {
        None = 0,
        GitPush = 1 << 0,
        BuildCompleted = 1 << 1,
        ProjectCreated = 1 << 2,
        ProjectDeleted = 1 << 3,
        Checkin = 1 << 4,
        WorkItemCreated = 1 << 5,
        WorkItemChanged = 1 << 6,
        WorkItemComment = 1 << 7,
        PullRequestCreated = 1 << 8,
        PullRequestStatusUpdate = 1 << 9,
        PullRequestReviewerVote = 1 << 10,
        BuildQualityChanged = 1 << 11,
        RepositoryCreated = 1 << 12,
        GitCommitComment = 1 << 13,
        PullRequestComment = 1 << 14,
        ChangesetComment = 1 << 15,
        ReleaseCreated = 1 << 16,
        ReleaseEnvironmentCompleted = 1 << 17,
        RepositoryRenamed = 1 << 18,
        RepositoryDeleted = 1 << 19,
        PolicyViolation = 1 << 20,

        All = 0xFFFFFFFF
    }
}
