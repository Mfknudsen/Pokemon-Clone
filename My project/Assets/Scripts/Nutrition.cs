public enum State {Per100G, Per100ML}

[System.Serializable]
public class Nutrition
{
    public State state;

    public Nutrition()
    {
        state = State.Per100ML;
    }
}
