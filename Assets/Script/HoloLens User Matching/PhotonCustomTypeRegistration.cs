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

[Serializable]
public class MatchInfo
{
    public string userWhoSend {get;set;}
    public string userWhoReceive {get;set;}
    public string matchRequest {get;set;}
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
        
        PhotonPeer.RegisterType(
            typeof(List<UserInfo>),
            (byte)'L', // 고유 식별자
            SerializeUserInfoList,
            DeserializeUserInfoList
        );

        PhotonPeer.RegisterType(
            typeof(MatchInfo),
            (byte)'M',
            SerializeMatchInfo,
            DeserializeMatchInfo
        );
    }

    private static byte[] SerializeMatchInfo(object data)
    {
        MatchInfo matchInfo = (MatchInfo)data;
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(matchInfo.userWhoSend ?? "");
                writer.Write(matchInfo.userWhoReceive ?? "");
                writer.Write(matchInfo.matchRequest ?? "");
            }
            return stream.ToArray();
        }
    }
    private static object DeserializeMatchInfo(byte[] data)
    {
        MatchInfo matchInfo = new MatchInfo();
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                matchInfo.userWhoSend = reader.ReadString();
                matchInfo.userWhoReceive = reader.ReadString();
                matchInfo.matchRequest = reader.ReadString();
            }
        }
        return matchInfo;
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
    
    private static byte[] SerializeUserInfoList(object customObject)
    {
        List<UserInfo> userInfoList = (List<UserInfo>)customObject;
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(userInfoList.Count); // 리스트 크기 기록
                foreach (var userInfo in userInfoList)
                {
                    writer.Write(userInfo.currentRoomNumber ?? "");
                    writer.Write(userInfo.photonRole ?? "");
                    writer.Write(userInfo.photonUserName ?? "");
                    writer.Write(userInfo.currentState ?? "");
                }
            }
            return stream.ToArray();
        }
    }

    private static object DeserializeUserInfoList(byte[] data)
    {
        List<UserInfo> userInfoList = new List<UserInfo>();
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int count = reader.ReadInt32(); // 리스트 크기 읽기
                for (int i = 0; i < count; i++)
                {
                    UserInfo userInfo = new UserInfo
                    {
                        currentRoomNumber = reader.ReadString(),
                        photonRole = reader.ReadString(),
                        photonUserName = reader.ReadString(),
                        currentState = reader.ReadString()
                    };
                    userInfoList.Add(userInfo);
                }
            }
        }
        return userInfoList;
    }

}
