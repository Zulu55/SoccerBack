﻿using System.Data.Entity.ModelConfiguration;

namespace Domain
{
    internal class MatchesMap : EntityTypeConfiguration<Match>
    {
        public MatchesMap()
        {
            HasRequired(o => o.Local)
                .WithMany(m => m.Locals)
                .HasForeignKey(m => m.LocalId);

            HasRequired(o => o.Visitor)
                .WithMany(m => m.Visitors)
                .HasForeignKey(m => m.VisitorId);
        }
    }
}