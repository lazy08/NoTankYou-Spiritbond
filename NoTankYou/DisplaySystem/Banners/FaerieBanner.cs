﻿using Dalamud.Interface.Windowing;
using ImGuiNET;
using ImGuiScene;
using System;
using System.Linq;
using System.Numerics;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Buddy;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using Dalamud.Game.ClientState.Objects;

namespace NoTankYou.DisplaySystem
{
    internal class FaerieBanner : WarningBanner
    {
        protected override ref bool RepositionModeBool => ref Service.Configuration.RepositionModeFaerieBanner;
        protected override ref bool ForceShowBool => ref Service.Configuration.ForceShowFaerieBanner;
        protected override ref bool SoloModeBool => ref Service.Configuration.EnableFaerieBannerWhileSolo;

        public FaerieBanner(TextureWrap faerieImage) : base("Partner Up Faerie Warning Banner", faerieImage)
        {

        }

        protected override void UpdateInPartyInDuty()
        {
            // Scholar Job id is 28
            var scholarPlayers = Service.PartyList.Where(p => p.ClassJob.Id is 28).ToHashSet();

            // If they are untargetable, remove them from the count
            foreach(var player in scholarPlayers)
            {
                if(!IsTargetable(player))
                {
                    scholarPlayers.Remove(player);
                }
            }

            var scholarPlayerIDs = scholarPlayers.Select(r => r.ObjectId);

            // Get the objects that have owner ids matching those of our scholars
            var objectsWithPartyMemberOwner = Service.ObjectTable
                .Where(r => scholarPlayerIDs.Contains(r.OwnerId));

            // id 791 is dissipation id
            var dissipationEffects = scholarPlayers
                .Where(r => r.Statuses.Any(s => s.StatusId is 791));

            // If these two lists match, then everyone's doing their job
            if (scholarPlayers.Count == objectsWithPartyMemberOwner.Count() + dissipationEffects.Count())
            {
                Visible = false;
            }
            else
            {
                // Show Warning Banner
                Visible = true;
            }
        }

        protected override void UpdateSoloInDuty()
        {
            var player = Service.ClientState.LocalPlayer;

            if (player == null) return;

            // If the player isn't a Scholar return
            if (player.ClassJob.Id != 28) return;

            // find any pet that has the player as an owner
            var objectWithPlayerOwnerExists = Service.ObjectTable
                .Any(r => r.OwnerId == player.ObjectId);

            // id 791 is dissipation id
            // Check if the player has dissipation
            bool playerHasDissipation = player.StatusList.Any(s => s.StatusId is 791);

            // If player has dissipation, or a faerie out, they are doing their job
            if (playerHasDissipation || objectWithPlayerOwnerExists)
            {
                Visible = false;
            }
            // If not, then we need to show the warning banner
            else
            {
                Visible = true;
            }
        }
    }
}
