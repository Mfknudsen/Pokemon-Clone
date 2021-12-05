#region Packages

using System;
using Sirenix.OdinInspector;

#endregion

namespace Mfknudsen.Player
{
    [Serializable]
    public class CharacterSheet 
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
    }
}
