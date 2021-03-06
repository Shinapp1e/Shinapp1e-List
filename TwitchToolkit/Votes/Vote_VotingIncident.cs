﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwitchToolkit.Store;
using Verse;

namespace TwitchToolkit.Votes
{
    public class Vote_VotingIncident : Vote
    {
        public Vote_VotingIncident(Dictionary<int, VotingIncident> incidents) : base (new List<int>(incidents.Keys))
        {
            List<VoteLabelType> voteLabels = Enum.GetValues(typeof(VoteLabelType)).Cast<VoteLabelType>().ToList();
            voteLabels.Shuffle();

            labelType = voteLabels[0];

            this.incidents = incidents;
        }

        public override void EndVote()
        {
            Find.WindowStack.TryRemove(typeof(VoteWindow));
            Ticker.IncidentHelpers.Enqueue(incidents[DecideWinner()].helper);
        }

        public override void StartVote()
        {
            if (ToolkitSettings.VotingWindow || (!ToolkitSettings.VotingWindow && !ToolkitSettings.VotingChatMsgs))
            {
                VoteWindow window = new VoteWindow(this);
                Find.WindowStack.Add(window);
            }

            if (ToolkitSettings.VotingChatMsgs)
            {
                Toolkit.client.SendMessage("TwitchStoriesChatMessageNewVote".Translate() + ": " + "TwitchToolKitVoteInstructions".Translate());
                foreach (KeyValuePair<int, VotingIncident> pair in incidents)
                {
                    Toolkit.client.SendMessage($"[{pair.Key + 1}]  {VoteKeyLabel(pair.Key)}");
                }
            }
        }

        public override string VoteKeyLabel(int id)
        {
            switch(labelType)
            {
                case VoteLabelType.Category:
                    return incidents[id].eventCategory.ToString();
                case VoteLabelType.Type:
                    return incidents[id].eventType.ToString();
                default:
                    return incidents[id].LabelCap;
            }
        }

        private VoteLabelType labelType = VoteLabelType.Label;

        private Dictionary<int, VotingIncident> incidents = new Dictionary<int, VotingIncident>();

        public enum VoteLabelType
        {
            Label,
            Category,
            Type
        }
    }
}
