using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    // Object관련 이벤트 관리
    public enum WorldObject
    {
        Unknown,
        Player,
        Monster,
    }

    // Controller관련 이벤트 관리
    public enum State
	{
		Die,
		Moving,
		Idle,
		Skill,
	}

    // Layer관련 이벤트 관리
    public enum Layer
    {
        Monster = 8,
        Ground = 9,
        Block = 10,
    }

    // Scene관련 이벤트 관리
    public enum Scene
    {
        Unknown,
        Start,
        Main,
        Game,
    }

    // Sound관련 이벤트 관리
    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    // UI관련 이벤트 관리
    public enum UIEvent
    {
        Click,
        Drag,
    }

    // Mouse관련 이벤트 관리
    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
    }

    // Camera관련 이벤트 관리
    public enum CameraMode
    {
        QuarterView,
    }
}
