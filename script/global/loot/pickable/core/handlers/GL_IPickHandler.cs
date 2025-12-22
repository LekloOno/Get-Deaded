namespace Pew;

public interface GL_IPickHandler
{
    /// <summary>
    /// Handle the pick interaction with the given picker.
    /// </summary>
    /// <param name="picker">The picker to interract with.</param>
    /// <returns>
    /// true if it was able to complete its interraction false otherwise.
    /// <para>Typically, if the target can't gather more of this item, or if the target can't handle this type of item at all, it should return false.</para>
    /// </returns>
    bool HandlePick(GL_Picker picker);
}