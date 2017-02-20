﻿using System;
using System.Collections.Generic;

namespace DevelopmentInProgress.DipMapper.Test
{
    public class Activity
    {
        public Activity()
        {
            Activities_1 = new List<Activity>();
            Activities_2 = new List<Activity>();
            GroupIds = new[] { 1, 2, 3 };
            Description = "Desc...";
            AssociatedActivityId = 3;
            ParentActivityId = 5;
        }

        // Supported by DipMap ORM
        public int Id { get; set; }
        public string Name { get; set; }
        public double Level { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public ActivityTypeEnum ActivityType { get; set; }

        // Not supported by DipMap ORM
        public Activity ParentActivity { get; set; }
        public IEnumerable<Activity> Activities_1 { get; set; }
        public IList<Activity> Activities_2 { get; set; }
        public int[] GroupIds { get; set; }
        internal string Description { get; set; }
        protected int AssociatedActivityId { get; set; }
        private int ParentActivityId { get; set; }
    }
}
