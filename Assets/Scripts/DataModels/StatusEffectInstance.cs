namespace DataModels
{
    // TODO: implement necessary fields for runtime status effect instances, such as:
    // - Reference to the StatusEffectBase class from the database (for effect definition and behavior)
    // - NumberOfStacks (if the effect is temporary)
    public class StatusEffectInstance
    {
        public StatusEffectBase Definition { get; private set; }
        public int NumberOfStacks { get; set; }  // in turns, for example
        public int RemainingTurns { get; private set; }
        public bool IsExpired => RemainingTurns <= 0;

        public StatusEffectInstance(StatusEffectBase definition, int stacks, int duration)
        {
            Definition = definition;
            NumberOfStacks = stacks;
            RemainingTurns = duration;
        }

        public void Tick(Combatant combatant)
        {
            // TODO: apply effect behavior to the combatant. For now, a basic switch statement could be enough.
        }
        
    }
}