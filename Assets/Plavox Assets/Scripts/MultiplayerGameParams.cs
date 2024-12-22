using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MultiplayerGameParams
{
    public string gameId, player1Id, player2Id, matchId, returnURL, token;
    public override string ToString()
    {
        return $"GAME PARAMS: gameId(${gameId}), player1Id({player1Id}), player2Id({player2Id}), matchID({matchId}), returnURL({returnURL}), token({token})";
    }
}
