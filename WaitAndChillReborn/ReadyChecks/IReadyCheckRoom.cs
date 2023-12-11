using Exiled.API.Features;

namespace WaitAndChillReborn
{
    public interface IReadyCheckRoom
    {
        /// <summary>
        /// Called before everyone spawns in.
        /// Should at least set spawn points and interactable doors
        /// </summary>
        void SetUpRoom();

        /// <summary>
        /// Called once per second to see if the player is ready
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        bool IsPlayerReady(Player player);

        /// <summary>
        /// Clean up interactable doors and stuff
        /// </summary>
        void OnRoundStart();

        /// <summary>
        /// Instructions on how to become ready
        /// </summary>
        /// <returns></returns>
        string Instructions();
    }
}
