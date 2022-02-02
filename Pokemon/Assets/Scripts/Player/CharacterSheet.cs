#region Packages

using System;
using Mfknudsen.Files;
using Sirenix.OdinInspector;

#endregion

namespace Mfknudsen.Player
{
    [Serializable]
    public class   CharacterSheet
    {
        public int badgeCount;

        [FoldoutGroup("Pronouns")]
        [HorizontalGroup("Pronouns/Horizontal")]
        [HideLabel]
        [BoxGroup("Pronouns/Horizontal/1", false)]
        public string pronoun1 = "They";

        [HideLabel] [BoxGroup("Pronouns/Horizontal/2", false)]
        public string pronoun2 = "Them";

        [HideLabel] [BoxGroup("Pronouns/Horizontal/3", false)]
        public string pronoun3 = "Theirs";

        public CharacterSheet(PlayerData data)
        {
            try
            {
                badgeCount = data.badgeCount;

                string[] pronouns = data.pronouns;
                pronoun1 = pronouns[0];
                pronoun2 = pronouns[1];
                pronoun3 = pronouns[2];
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}