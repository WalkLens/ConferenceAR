using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

[Serializable]
public class UserInfo{
    public string currentRoomNumber {get;set;}
    public string photonRole {get;set;}
    public string photonUserName {get;set;}
    public string currentState {get;set;}
}

public class PhotonCustomTypeRegistration : MonoBehaviourPunCallbacks
{
    private void Awake()
    {
        // UserInfo 직렬화 등록
        PhotonPeer.RegisterType(
            typeof(UserInfo),             // 타입 지정
            (byte)'U',                    // 타입의 식별자 (고유해야 함)
            SerializeUserInfo,            // 직렬화 메서드
            DeserializeUserInfo           // 역직렬화 메서드
        );
    }
    
    // UserInfo를 직렬화하는 함수
    private static byte[] SerializeUserInfo(object customObject)
    {
        UserInfo userInfo = (UserInfo)customObject;
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(userInfo.currentRoomNumber ?? "");
                writer.Write(userInfo.photonRole ?? "");
                writer.Write(userInfo.photonUserName ?? "");
                writer.Write(userInfo.currentState ?? "");
            }
            return stream.ToArray();
        }
    }

    // UserInfo를 역직렬화하는 함수
    private static object DeserializeUserInfo(byte[] data)
    {
        UserInfo userInfo = new UserInfo();
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                userInfo.currentRoomNumber = reader.ReadString();
                userInfo.photonRole = reader.ReadString();
                userInfo.photonUserName = reader.ReadString();
                userInfo.currentState = reader.ReadString();
            }
        }
        return userInfo;
    }
}
