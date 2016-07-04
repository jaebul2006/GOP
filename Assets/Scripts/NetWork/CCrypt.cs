using UnityEngine;
using System;


 
public class CCrypt
{
    private const int C1 = 52845;
    private const int C2 = 22719;
    private const int KEY = 72957;

    //┌───────────────────────────────────────────────────┐.
    //│ 이 름 : Encrypt.
    //│ 설 명 : 데이터를 암호화 한다.
    //└───────────────────────────────────────────────────┘.     
    public static bool Encrypt(
        byte[] a_Source, uint a_SourceIndex, // 보통데이터 , 시작위치.
        byte[] a_Destination, uint a_DestinationIndex, // 함호화된 데이터가 저장될 배열, , 시작위치.
        uint a_Length)// 암호화할 데이터 길이.
    {
        uint i;
        int Key = KEY;

        if (a_Source == null || a_Destination == null || a_Length <= 0)
        {
            System.Console.WriteLine("Encrypt Error");
            return false;
        }

        for (i = 0; i < a_Length; i++)
        {
            a_Destination[a_DestinationIndex + i] = (byte)((int)a_Source[a_SourceIndex + i] ^ Key);
            Key = (a_Destination[a_DestinationIndex + i] + Key) * C1 + C2;
        }
        return true;
    }

    //┌───────────────────────────────────────────────────┐.        
    //│ 이 름 : Decrypt.
    //│ 설 명 : 암호화된 데이터를 해독한다. 
    //└───────────────────────────────────────────────────┘.
    public static bool Decrypt(
        byte[] a_Source, uint a_SourceIndex, // 해독할 데이터 , 시작위치 .
        byte[] a_Destination, uint a_DestinationIndex, // 해독된 데이터를 저장할 배열 , 저장시작위치. 
        uint a_Length) // 해독할 데이터 길이
    {
        uint i;
        byte PreviousBlock;
        int Key = KEY;

        if (a_Source == null || a_Destination == null || a_Length <= 0)
        {
            System.Console.WriteLine("Decrypt Error");
            return false;
        }

        for (i = 0; i < a_Length; i++)
        {
            PreviousBlock = a_Source[a_SourceIndex + i];
            a_Destination[a_DestinationIndex + i] = (byte)((int)a_Source[a_SourceIndex + i] ^ Key);
            Key = (PreviousBlock + Key) * C1 + C2;
        }

        return true;
    }
}

