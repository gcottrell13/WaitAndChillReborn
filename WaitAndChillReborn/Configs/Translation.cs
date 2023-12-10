namespace WaitAndChillReborn.Configs
{
    using Exiled.API.Interfaces;

    public sealed class Translation : ITranslation
    {
        public string RoundCount { get; private set; } = "<size=60>Round {rounds}</size>";

        public string TopMessage { get; private set; } = "<size=40><color=yellow><b>The game will be starting soon, {seconds}</b></color></size>";

        public string WaitingForReadyCheck { get; private set; } = "<size=40><color=yellow>Waiting for players to leave C-D: {readyCount} / {players}</color></size>";

        public string BottomMessage { get; private set; } = "<size=30><i>{players}</i></size>";

        public string ServerIsPaused { get; private set; } = "Server is paused";

        public string RoundIsBeingStarted { get; private set; } = "Round is being started";

        public string OneSecondRemain { get; private set; } = "second remains";

        public string XSecondsRemains { get; private set; } = "seconds remain";

        public string OnePlayerConnected { get; private set; } = "player has connected";

        public string XPlayersConnected { get; private set; } = "players have connected";
    }
}
