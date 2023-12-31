﻿using Models.Domain.Conferences;
using Models.Domain.Officials;
using Models.Domain.Seasons;
using Models.Domain.Teams;
using System;
using System.Collections.Generic;

namespace Models.Domain
{
    public class Game
    {
        public int Id { get; set; }
        public LookUp GameStatus { get; set; }
        public BaseSeason Season { get; set; }
        public int Week { get; set; }
        public BaseConference Conference { get; set; }
        public Team HomeTeam { get; set; }
        public Team VisitingTeam { get; set; }
        public DateTime StartTime { get; set; }
        public bool IsNonConference { get; set; }
        public BaseVenue Venue { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public  List<BaseOfficial> Officials { get; set; }
        public bool IsDeleted { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }




    }
}
