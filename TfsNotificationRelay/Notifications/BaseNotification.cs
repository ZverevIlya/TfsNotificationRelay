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
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevCore.TfsNotificationRelay.Notifications
{
    public abstract class BaseNotification : INotification
    {
        public string TeamProjectCollection { get; set; }
        public IEnumerable<string> TeamNames { get; set; }

        public virtual IList<string> ToMessage(BotElement bot, Func<string, string> transform)
        {
            return ToMessage(bot, bot.Text, transform);
        }

        public abstract IList<string> ToMessage(BotElement bot, TextElement text, Func<string, string> transform);

        public abstract EventRuleElement GetRuleMatch(string collection, IEnumerable<EventRuleElement> eventRules);

        public virtual IEnumerable<string> TargetUserNames => Enumerable.Empty<string>();
    }
}
