using System.Collections.Generic;

public class GameManagerData
{
    public int NumberOfPlayers => playersData.Count;

    private List<PlayerData> playersData;

    public void CreateGame(int numberOfCPUPlayers)
    {
        playersData = new List<PlayerData>();

        var basePlayer = new PlayerData();
        playersData.Add(basePlayer);

        for (int i = 0; i < numberOfCPUPlayers; i++)
        {
            var newCPU = basePlayer.Clone() as PlayerData;
            newCPU.SetupId(playersData.Count);
            playersData.Add(newCPU);
        }
    }
}
