using UnityEngine;
using System.Collections;

public static class Defines
{
    public delegate void Delegate_None();
    public delegate void Delegate_Bool(bool a_nValue);
    public delegate void Delegate_Int(int a_nValue);
    public delegate void Delegate_Intint(int a_nValue1, int a_nValue2);
    public delegate void Delegate_BoolStrStr(bool b,string str1, string str2);

    public const bool       DEF_STAGE_ALL_UnLock    = false;
    public const bool       DEF_PRODUCTION          = false;
    public const int        DEF_VERSION             = 1;
    public const int        DEF_MAX_Summon_Upgrade  = 5;
    public const int        DEF_MAX_TEAM            = 3;
    public const int        DEF_MAX_TEAM_ITEM       = 4;
    public const int        DEF_MAX_Ememy           = 5;
    public static int []    DEF_ATTACK_ORDER        = new int[]{ 0, 2, 4, 1, 3, 5 };
    public const int        DEF_ADD_GAUGE           = 15;

    public const int        DEF_MAX_STAGE_ROUND     = 10;

    public const int 		DEF_DEGINE_SCREEN_WIDHT  = 720;
	public const int 		DEF_DEGINE_SCREEN_HEIGHT = 1280;    
    public const int        DEF_MAX_GRADE            = 6;   
    public const int        DEF_MAX_SUMMON_EX_SLOT   = 30;   
    public const int        DEF_MAX_TRUN_DRAG        = 30;    
}
