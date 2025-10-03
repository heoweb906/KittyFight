using System;
using UnityEngine;

[Serializable] public struct ReadyPayload { public int r; }        // round token
[Serializable] public struct StartPayload { public int r; public int d; } // r=round, d=delay(ms)
[Serializable] public struct PlayingPayload { public int r; }        // ACK