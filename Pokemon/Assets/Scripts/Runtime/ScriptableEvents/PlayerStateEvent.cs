using Runtime.Player;
using UnityEngine;

namespace Runtime.ScriptableEvents
{
    [CreateAssetMenu(menuName = "Variables/Event/Player State")]
    public class PlayerStateEvent : ScriptableEvent<PlayerState>
    {
    }
}
