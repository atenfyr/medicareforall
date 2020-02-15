using MonoMod.Cil;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using static Mono.Cecil.Cil.OpCodes;

namespace MedicareForAll
{
    public class M4APlayer : ModPlayer
    {
        public int healCD = 0;

        public override bool Autoload(ref string name)
        {
            IL.Terraria.Main.GUIChatDrawInner += HookFreeHealthcare;
            return base.Autoload(ref name);
        }

        /* We use IL injection because if we make it completely free with the hook alone it assumes we're at full health */
        private void HookFreeHealthcare(ILContext il)
        {
            var c = new ILCursor(il).Goto(0);

            // First we jump to the first nurse section (To change price label to say "Free")
            if (!c.TryGotoNext(i => i.MatchCall(typeof(PlayerHooks).GetMethod(nameof(PlayerHooks.ModifyNursePrice))))) return;
            c.Index++;
            var label = il.DefineLabel();
            c.Emit(Br, label);
            if (!c.TryGotoNext(i => i.MatchCall(typeof(NPCLoader).GetMethod(nameof(NPCLoader.SetChatButtons))))) return;
            c.Index -= 3;
            c.EmitDelegate<Func<string>>(() =>
            {
#pragma warning disable CS0618 // Type or member is obsolete
                return Lang.inter[54].Value + " (Free?)";
#pragma warning restore CS0618 // Type or member is obsolete
            });
            c.Index -= 3;
            c.MarkLabel(label);

            // Then we jump to the second nurse section (To skip past the check that it's not equal to zero)
            if (!c.TryGotoNext(i => i.MatchCall(typeof(Terraria.GameContent.Achievements.AchievementsHelper).GetMethod(nameof(Terraria.GameContent.Achievements.AchievementsHelper.HandleNurseService))))) return;
            if (!c.TryGotoPrev(i => i.Match(Ble))) return;

            // Place branch instruction before if statement
            var label2 = il.DefineLabel();
            c.Index -= 2;
            c.Emit(Br, label2);

            // Place branch endpoint after if statement
            c.Index += 3;
            c.MarkLabel(label2);

            //mod.Logger.Info(il.ToString());
        }

        public override bool ModifyNurseHeal(NPC nurse, ref int health, ref bool removeDebuffs, ref string chatText)
        {
            if (health == 0)
            {
                int num25 = Main.rand.Next(3);
                if (!ChildSafety.Disabled) num25 = Main.rand.Next(1, 3);
#pragma warning disable CS0618 // Type or member is obsolete
                chatText = Lang.dialog(55 + num25);
#pragma warning restore CS0618 // Type or member is obsolete
                return false;
            }
            if (healCD > 0)
            {
                chatText = "Sorry, I'm afraid it isn't time for your appointment yet. You'll have to wait another " + MedicareForAll.UsefulThings.SecondsToHMS((int)Math.Floor(healCD / 60.0)) + ".";
                return false;
            }
            return true;
        }

        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
            if (M4AWorld.serverConfig.IdealisticMode)
            {
                healCD = 0;
            }
            else
            {
                healCD = M4AWorld.serverConfig.WaitTime * 60;
            }
        }

        public override void ModifyNursePrice(NPC nurse, int health, bool removeDebuffs, ref int price)
        {
            price = 0;
        }

        public override void PostUpdate()
        {
            if (healCD > 0) healCD -= Main.dayRate;
        }
    }
}
