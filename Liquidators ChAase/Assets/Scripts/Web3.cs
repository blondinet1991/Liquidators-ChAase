using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class Web3 : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Text WalletHolderTxt;


    [DllImport("__Internal")]
    private static extern void loginReq();

        [DllImport("__Internal")]
    private static extern void logOutReq();

    public void ConnectWalletRequest(){
        loginReq();
    }

    public void WalletConnected(string address){
        WalletHolderTxt.text = address;
    }

    public void WalletDisConnected(){
        WalletHolderTxt.text = "Not Connected!";
    }
}
