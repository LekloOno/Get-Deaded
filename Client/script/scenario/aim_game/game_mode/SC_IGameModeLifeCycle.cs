public interface SC_GameModeLifeCycle
{
    bool Init(GE_IActiveCombatEntity starter);
    bool Start();
    bool Interrupt(GameModeEnd outcome);
}