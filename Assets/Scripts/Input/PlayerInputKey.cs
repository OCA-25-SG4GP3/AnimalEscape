using UnityEngine;

[System.Serializable]
public struct PlayerInputKeys
{
    public KeyCode forward;
    public KeyCode backward;
    public KeyCode left;
    public KeyCode right;
    public KeyCode jump;
    public KeyCode specialAction; //Slide / Throw
    public KeyCode horizontalAxis; //Slide / Throw
    public KeyCode verticalAxis; //Slide / Throw
}