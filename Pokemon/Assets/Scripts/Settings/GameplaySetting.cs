#region Packages

using Mfknudsen.AI;

#endregion

namespace Mfknudsen.Settings
{
    public static class GameplaySetting
    {
        private static readonly EvaluatorSetting EasyEvaluatorSetting, MediumEvaluatorSetting, HardEvaluatorSetting;

        static GameplaySetting()
        {
            PersonalitySetting personalitySetting = new PersonalitySetting();

            EasyEvaluatorSetting =
                new EvaluatorSetting(1, 10, false, false, personalitySetting);
            MediumEvaluatorSetting =
                new EvaluatorSetting(3, 5, true, false, personalitySetting);
            HardEvaluatorSetting =
                new EvaluatorSetting(5, 0, true, true, personalitySetting);
        }

        #region Getters

        public static EvaluatorSetting GetDefaultEvaluatorSetting(Difficultly difficultly)
        {
            return difficultly switch
            {
                Difficultly.Easy => EasyEvaluatorSetting,
                Difficultly.Medium => MediumEvaluatorSetting,
                _ => HardEvaluatorSetting,
            };
        }

        #endregion
    }
}